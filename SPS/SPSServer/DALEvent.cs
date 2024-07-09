/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALEvent.cs                                                                                          */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      07/08/2023  Add SQLDBBackup                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPSLib;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SPSServer
{
    public class DALEvent
    {
        /// <summary>
        /// Delete history data
        /// </summary>
        /// <param name="dtCutOff"></param>
        /// <returns></returns>
        public static bool DelHistoryData(DateTime dtCutOff)
        {
            string sCon = Common.GetDBStrCon();

            if (sCon == null)
            {
                return false;
            }

            SqlTransaction sqltran = null;

            using (SqlConnection sqlcon = new SqlConnection(sCon))
            {
                try
                {
                    sqlcon.Open();
                    sqltran = sqlcon.BeginTransaction();

                    // Delete data in Log
                    SqlCommand sqlcmd = new SqlCommand("usp_DelLog", sqlcon, sqltran)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@CutOfDate", dtCutOff);
                    sqlcmd.ExecuteNonQuery();

                    // Delete data in Record
                    sqlcmd = new SqlCommand("usp_DelRecord", sqlcon, sqltran)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@CutOfDate", dtCutOff);
                    sqlcmd.ExecuteNonQuery();

                    sqltran.Commit();
                }

                catch (Exception ex)
                {
                    if (sqltran != null)
                    {
                        sqltran.Rollback();
                    }

                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Backup database
        /// </summary>
        /// <returns></returns>
        public static bool SQLDBBackup()
        {
            string sCon = Common.GetDBStrCon();

            // read backup folder from config file ("C:/ProgramData/SPS/")
            var backupFolder = ConfigurationManager.AppSettings["BackupFolder"];
            var sqlConStrBuilder = new SqlConnectionStringBuilder(sCon);

            var backupFileName = String.Format("{0}{1}-{2}.bak", 
                backupFolder, sqlConStrBuilder.InitialCatalog, 
                DateTime.Now.ToString("yyyy-MM-dd"));

            if (sCon == null)
            {
                return false;
            }
            using (SqlConnection sqlcon = new SqlConnection(sCon))
            {
                try
                {
                    sqlcon.Open();

                    // Delete data in Log
                    SqlCommand sqlcmd = new SqlCommand("usp_SQLDBBackup", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@db", sqlConStrBuilder.InitialCatalog);
                    sqlcmd.Parameters.AddWithValue("@path", backupFileName);
                    sqlcmd.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }
    }
}
