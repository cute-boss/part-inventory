/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* Common.cs                                                                                            */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add DelUploadedFile, ChkExistDirectory, ReadCSVToDataTable      Azmir               */
/*      16/06/2022  Add License for pf server                                       Azmir               */
/*      09/03/2023  Upd SendEmailNoti to accept html in body msg                    Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

namespace SPSLib
{
    public class Common
    {
        // Mac address
        public static string[] G_SLICENSES =
        {
            "E4B97AF3A8C1",                                 // 7MLDHY2
            "0050568FA2EB",                                 // PSNMVSVPF\admin-pf
            "0050568FF23D",                                 // SESVR-TEST
        };

        public const int G_IMAX_LOGIN = 10;

        static string m_sSPSDBStr;

        public static DateTime g_dtDemo = new DateTime(2023, 12, 31, 23, 59, 59);
        public static bool g_bUseDongle = false;
        public static bool g_bServer;
        public static string g_sDefPwd = "12345678";

        #region Common function

        public static void SetDBStrCon(string sCon)
        {
            m_sSPSDBStr = sCon;
        }

        public static string GetDBStrCon()
        {
            return m_sSPSDBStr;
        }

        // <summary>
        /// Check license status
        /// </summary>
        /// <returns></returns>
        public static bool ChkLicense()
        {
            List<string> sMacList = new List<string>();
            string[] sLicenses = Common.G_SLICENSES;

            bool bMatch = false;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                sMacList.Add(nic.GetPhysicalAddress().ToString());
            }

            for (int i = 0; i < sMacList.Count; i++)
            {
                string sMac = sMacList[i];

                for (int j = 0; j < sLicenses.Length; j++)
                {
                    if (sMac == sLicenses[j])
                    {
                        bMatch = true;
                        break;
                    }
                }
            }

            return bMatch;
        }

        public static bool ChkMSSQLDBStatus(string sDBstr)
        {
            string sCon = sDBstr;

            try
            {
                using (SqlConnection con = new SqlConnection(sCon))
                {
                    con.Open();
                }
            }
            catch (Exception ex)
            {
                WriteToLog("SPS Server Error: " + ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write error log
        /// </summary>
        /// <param name="sErr">Error desc</param>
        public static void WriteToLog(string sErr)
        {
            string sLogName = (g_bServer) ? "SPSServer" : "SPS";
            string sDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + "SPS" + "\\" + "Log";
            string sFilename = sDir + "\\" + sLogName + "_" + DateTime.Now.ToString("yyyyMM") + "." + "LOG";

            try
            {
                if (!Directory.Exists(sDir))
                {
                    Directory.CreateDirectory(sDir);
                }

                if (!File.Exists(sFilename))
                {
                    using (var myFile = File.Create(sFilename))
                    {
                        GrantAccess(sFilename);
                    }
                }

                using (StreamWriter swLog = new StreamWriter(sFilename, true))
                {
                    if (swLog != null)
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.Append(DateTime.Now);
                        sb.Append("\r\n");

                        sb.Append(sErr);
                        swLog.WriteLine(sb.ToString());
                        swLog.WriteLine();
                        swLog.Flush();
                    }
                }
            }
            catch
            {
                ;
            }
        }

        /// <summary>
        /// Grant access right to folder
        /// </summary>
        /// <param name="sFullPath">Folder path</param>
        private static void GrantAccess(string sFullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(sFullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
            PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }

        /// <summary>
        /// Send Email Notification
        /// </summary>
        /// <param name="sTo">To email</param>
        /// <param name="sSubject">Subject</param>
        /// <param name="sBody">Email message</param>
        /// <returns></returns>
        public static bool SendEmailNoti(string sServer, string sPort, string sUsername, string sPassword, int iProtocol, string sFrom, string sTo, string sSubject, string sBody)
        {
            #region Send email
            //MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient();

            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(sFrom),
                    Subject = sSubject,
                    Body = sBody,
                    IsBodyHtml = true,
                };

                mail.To.Add(sTo);

                SmtpServer.Host = sServer;
                SmtpServer.Port = Int32.Parse(sPort);

                if (sUsername != "" || sPassword != "")
                {
                    SmtpServer.Credentials = new System.Net.NetworkCredential(sUsername, sPassword);
                }

                if (iProtocol == (int)EnumEx.EmailProtocol.TLSSSL)
                {
                    SmtpServer.EnableSsl = true;
                }
                else
                {
                    SmtpServer.EnableSsl = false;
                }

                SmtpServer.Timeout = 1000;
                SmtpServer.Send(mail);
            }
            catch
            {
                //Common.WriteToLog(ex.ToString());
                return false;
            }
            #endregion

            return true;
        }

        /// <summary>
        /// Get IP address
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {
            IPHostEntry host;
            string sLocalIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    sLocalIP = ip.ToString();
                    break;
                }
            }

            return sLocalIP;
        }

        /// <summary>
        /// Convert password to encrypted string
        /// </summary>
        /// <param name="sText"></param>
        /// <returns>Encrypted password</returns>
        public static string Encrypt(string sText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(sText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Common.g_sDefPwd, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    sText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return sText;
        }

        /// <summary>
        /// Decrypt encrypted string
        /// </summary>
        /// <param name="sText"></param>
        /// <returns>Decrypted string</returns>
        public static string Decrypt(string sText)
        {
            sText = sText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(sText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Common.g_sDefPwd, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    sText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return sText;
        }

        /// <summary>
        /// Delete uploaded file if exists
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void DelUploadedFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        /// <summary>
        /// Create directory path if not exists/found
        /// </summary>
        /// <param name="sGetDir"></param>
        /// <returns></returns>
        public static void ChkExistDirectory(string sGetDir)
        {
            if (!Directory.Exists(sGetDir))
            {
                Directory.CreateDirectory(sGetDir);
            }
        }

        /// <summary>
        /// Read CSV date to data table format 
        /// </summary>
        /// <param name="sfileInput"></param>
        /// <returns></returns>
        public static DataTable ReadCSVToDataTable(string sfileInput)
        {
            string line = string.Empty;
            string[] strArray;
            DataTable dt = new DataTable();
            DataRow row;

            // Work out where we should split on comma, but not in a sentence
            //Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            using (StreamReader sr = new StreamReader(sfileInput))
            {
                // Read the first line and split the string at , with our regular expression in to an array
                line = sr.ReadLine();
                strArray = r.Split(line);

                // For each item in the new split array, dynamically builds our Data columns
                Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn()));

                // Read each line in the CVS file until it’s empty
                while ((line = sr.ReadLine()) != null)
                {
                    row = dt.NewRow();

                    // Add our current value to our data row
                    row.ItemArray = r.Split(line.ToUpper());
                    dt.Rows.Add(row);
                }

                // Tidy Streameader up
                sr.Dispose();
            }

            return dt;
        }
        #endregion
    }
}
