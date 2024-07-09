/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* BLLEvent.cs                                                                                          */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      10/03/2023  Add PartQtyStatusTh, PartQtyStatus func                         Azmir               */
/*      07/08/2023  Add SQLDBBackupTh, SQLDBBackup func                             Azmir               */
/*      30/04/2024  Change thread location to reduce pc resources                   Azmir               */
/*      20/06/2024  Modify database backup to 1 day                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using SPSLib;

namespace SPSServer
{
    public class BLLEvent
    {
        /// <summary>
        /// Check demo end date event
        /// </summary>
        public void ChkDemoEndTh()
        {
            while (true)
            {
                if (DateTime.Now > Common.g_dtDemo)
                {
                    DALLog.SetLog("SPS Server is stopped due to demo period has finished.", (int)EnumEx.LogType.EVTLOG_SVR, "Event");
                    Environment.Exit(1);
                }

                Thread.Sleep(60 * 1000);    // 60 secs loop
            }
        }

        /// <summary>
        /// Remove history data event
        /// </summary>
        public void RemoveHistoryTh()
        {
            // Run at server startup
            RemoveHistory();

            while (true)
            {
                DateTime dtNow = DateTime.Now;
                DateTime dtStart = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 1, 0, 0);   //trigger at 1am
                DateTime dtEnd = dtStart.AddSeconds(5);

                if (dtNow >= dtStart && dtNow < dtEnd)
                {
                    RemoveHistory();
                    Thread.Sleep(10 * 1000);    // Wait 10 sec
                }

                Thread.Sleep(2000);             // Wait 2 sec
            }
        }

        /// <summary>
        /// Delete history data
        /// </summary>
        private void RemoveHistory()
        {
            if (!DALMisc.GetMisc(out Misc misc))
            {
                return;
            }

            DateTime dtCutOff = DateTime.UtcNow.AddYears(-misc.RetentionPeriod);
            string sDesc;

            if (DALEvent.DelHistoryData(dtCutOff))
            {
                sDesc = "History event data has been removed successfully";
            }
            else
            {
                sDesc = "Failed to remove history event data";
            }

            DALLog.SetLog(sDesc, (int)EnumEx.LogType.EVTLOG_REMOVE_HISTORY, "Event");
        }


        /// <summary>
        /// Send email for part status data event
        /// </summary>
        public void PartQtyStatusTh()
        {
            // Run at server startup
            PartQtyStatus();

            while (true)
            {
                DateTime dtNow = DateTime.Now;
                string sCurDate = dtNow.DayOfWeek.ToString();

                if (sCurDate == "Monday")
                {
                    //DateTime dtNow = DateTime.Now;
                    DateTime dtStart = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 7, 00, 0);   // trigger at 7:00am
                    DateTime dtEnd = dtStart.AddSeconds(5);

                    if (dtNow >= dtStart && dtNow < dtEnd)
                    {
                        PartQtyStatus();
                        Thread.Sleep(10 * 1000);    // Wait 10 sec
                    }
                }

                Thread.Sleep(2000);             // Wait 2 sec
            }
        }

        /// <summary>
        /// Send email when part is in critical level for every monday
        /// </summary>
        public void PartQtyStatus()
        {
            if (!DALMisc.GetMisc(out Misc misc))
            {
                return;
            }

            // Get active admin user
            if (!DALPartRack.GetPartRackBelowMinQtyList(out DataTable tblPartRack))
            {
                return;
            }

            string sContent = @"<style>
                                    table.PartTable {
                                      width: 100%;
                                      background-color: #ffffff;
                                      border-collapse: collapse;
                                      border-width: 2px;
                                      border-color: #a9eaff;
                                      border-style: solid;
                                      color: #000000;
                                      font-family: Verdana, Geneva, Tahoma, sans-serif;
                                      font-size: 12px;
                                    }

                                    table.PartTable td, table.PartTable th {
                                      border-width: 2px;
                                      border-color: #a9eaff;
                                      border-style: solid;
                                      padding: 3px;
                                    }

                                    table.PartTable thead {
                                      background-color: #a9eaff;
                                    }

                                    .align-mid {
                                      text-align: center;
                                    }
                                </style>";

            sContent += "<table class='PartTable'><thead><tr>";
            sContent += "<th><b>Part Code</b></th>";
            sContent += "<th><b>Part Name<b></th>";
            sContent += "<th><b>Part Desc.<b></th>";
            sContent += "<th><b>Min. Qty.<b></th>";
            sContent += "<th><b>Bal. Qty.<b></th>";
            sContent += "</tr></thead><tbody>";

            for (int r = 0; r < tblPartRack.Rows.Count; r++)
            {
                sContent += "<tr>";

                string sPartCode = tblPartRack.Rows[r]["PartCode"].ToString();
                string sPartName = tblPartRack.Rows[r]["PartName"].ToString();
                string sPartDesc = tblPartRack.Rows[r]["PartDesc"].ToString();
                int iPartMinQty = Convert.ToInt32(tblPartRack.Rows[r]["PartMinQty"]);
                int iPartQty = Convert.ToInt32(tblPartRack.Rows[r]["PartQty"]);

                sContent += "<td>" + sPartCode + "</td>";
                sContent += "<td>" + sPartName + "</td>";
                sContent += "<td>" + sPartDesc + "</td>";
                sContent += "<td class='align-mid'>" + iPartMinQty + "</td>";
                sContent += "<td class='align-mid'>" + iPartQty + "</td>";

                sContent += "</tr>";
            }

            sContent += "</tbody></table><br><br>";

            // Get active admin user
            if (!DALUser.GetActUserAdminAcc(out DataTable tblUser, (int)EnumEx.UserRoles.ROLES_ADMIN))
            {
                return;
            }

            if (tblUser.Rows.Count > 0)
            {
                for (int i = 0; i < tblUser.Rows.Count; i++)
                {
                    string sEmail = tblUser.Rows[i]["Email"].ToString();
                    string sSubject, sBody;

                    sSubject = "[SPS] Part Status";
                    sBody = "<html><body style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size: 12px;'>"
                          + "Hi,<br><br>Good day. To whom it may concern,<br><br>Below are the parts that has reached or below the minimum quantity.<br><br>"
                          + sContent
                          + "Kindly please take note.<br><br>"
                          + "Thank you.<br><br>"
                          + "<i>Please do not reply to this email.<br>This is an automated application used only for sending notifications.</i></body></html>";

                    Common.SendEmailNoti(misc.EmailSmtp, misc.EmailPort, misc.EmailUsername, Common.Decrypt(misc.EmailPassword),
                                        misc.EmailProtocol, misc.DefaultEmail, sEmail, sSubject, sBody);
                }
            }
        }

        /// <summary>
        /// Backup sql data event
        /// </summary>
        public void SQLDBBackupTh()
        {
            // Run at server startup
            SQLDBBackup();

            while (true)
            {
                DateTime dtNow = DateTime.Now;
                string sCurDate = dtNow.DayOfWeek.ToString();

                //DateTime dtNow = DateTime.Now;
                DateTime dtStart = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 6, 00, 0);   // trigger at 6:00am
                DateTime dtEnd = dtStart.AddSeconds(5);

                if (dtNow >= dtStart && dtNow < dtEnd)
                {
                    SQLDBBackup();
                    Thread.Sleep(10 * 1000);    // Wait 10 sec
                }

                Thread.Sleep(2000);             // Wait 2 sec
            }
        }

        /// <summary>
        /// Backup sql data
        /// </summary>
        private void SQLDBBackup()
        {
            string sDesc;

            var backupFolder = ConfigurationManager.AppSettings["BackupFolder"];
            string[] files = Directory.GetFiles(backupFolder);

            try
            {
                // Delete files that are older than 1 days old
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddDays(-1))
                        fi.Delete();
                }
            }
            catch
            {
                return;
            }

            // Backup database
            if (DALEvent.SQLDBBackup())
            {
                sDesc = "Database backup event data has been added successfully";
            }
            else
            {
                sDesc = "Failed to add database backup event data";
            }

            DALLog.SetLog(sDesc, (int)EnumEx.LogType.EVTLOG_DB_BACKUP, "Event");
        }
    }
}
