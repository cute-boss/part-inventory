/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALLog.cs                                                                                            */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPSLib;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SPSServer
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

                    sqlcmd.Parameters.AddWithValue("@LogTime", DateTime.UtcNow);
                    sqlcmd.Parameters.AddWithValue("@LogType", iLogType);
                    sqlcmd.Parameters.AddWithValue("@LogDesc", sLogDesc);
                    sqlcmd.Parameters.AddWithValue("@UserName", sUsername);
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
