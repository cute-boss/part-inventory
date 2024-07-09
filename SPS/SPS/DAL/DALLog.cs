/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALLog.cs                                                                                            */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      29/06/2022  Chg RegLog to set date time now instead of utc                  Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.Models;
using SPSLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SPS.DAL
{
    public class DALLog
    {
        /// <summary>
        /// Register log
        /// </summary>
        /// <param name="sLogDesc"></param>
        /// <param name="iLogType"></param>
        /// <param name="sUsername"></param>
        /// <returns></returns>
        public static bool SetLog(string sLogDesc, int iLogType, string sUsername)
        {
            string sCon = Common.GetDBStrCon();
            if (sCon == null)
            {
                return false;
            }

            using (SqlConnection sqlcon = new SqlConnection(sCon))
            {
                try
                {
                    sqlcon.Open();

                    SqlCommand sqlcmd = new SqlCommand("usp_SetLog", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    //sqlcmd.Parameters.AddWithValue("@LogTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    sqlcmd.Parameters.AddWithValue("@LogTime", DateTime.Now);
                    sqlcmd.Parameters.AddWithValue("@LogType", iLogType);
                    sqlcmd.Parameters.AddWithValue("@LogDesc", sLogDesc);
                    sqlcmd.Parameters.AddWithValue("@UserName", sUsername);
                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //Write error to log
                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get log
        /// </summary>
        /// <returns></returns>
        public static IList<LogDetailModel> GetLogList(int iLogTypeId, DateTime dtStart, DateTime dtEnd)
        {
            List<LogDetailModel> ldModel = new List<LogDetailModel>();

            string sCon = Common.GetDBStrCon();

            if (sCon == null)
            {
                return null;
            }

            using (SqlConnection sqlcon = new SqlConnection(sCon))
            {
                try
                {
                    sqlcon.Open();

                    SqlCommand sqlcmd = new SqlCommand("usp_GetLog", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@LogTypeId", iLogTypeId);
                    sqlcmd.Parameters.AddWithValue("@StartDateTime", dtStart);
                    sqlcmd.Parameters.AddWithValue("@EndDateTime", dtEnd);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        LogDetailModel model = new LogDetailModel
                        {
                            LogId = Convert.ToInt32(sqlreader["LogId"]),
                            LogTime = Convert.ToDateTime(sqlreader["LogTime"]),
                            LogTypeId = Convert.ToInt32(sqlreader["LogType"]),
                            LogDesc = sqlreader["LogDesc"].ToString(),
                            UserName = sqlreader["UserName"].ToString(),
                            SLogTime = Convert.ToDateTime(sqlreader["LogTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt")
                        };

                        ldModel.Add(model);
                    }
                }

                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return null;
                }
            }

            return ldModel;
        }
    }
}