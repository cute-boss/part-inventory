/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALSession.cs                                                                                        */
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

namespace SPS.DAL
{
    public class DALSession
    {
        /// <summary>
        /// Get session info by user ID
        /// </summary>
        /// <param name="tblResult">Output result</param>
        /// <param name="iUserId">User ID</param>
        /// <returns></returns>
        public static bool GetSessionById(out DataTable tblResult, int iUserId)
        {
            tblResult = new DataTable();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetSessionById", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@UserId", iUserId);

                    SqlDataAdapter sqlda = new SqlDataAdapter(sqlcmd);
                    sqlda.Fill(tblResult);
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Register/update session information
        /// </summary>
        /// <param name="iUserId"></param>
        /// <param name="sSessionId"></param>
        /// <param name="iIdleTime"></param>
        /// <returns></returns>
        public static bool SetSession(int iUserId, string sSessionId, int iIdleTime)
        {
            // Last session = now + ideal time
            DateTime dtLastSession = DateTime.UtcNow.AddMinutes(iIdleTime);

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

                    SqlCommand sqlcmd = new SqlCommand("usp_SetSession", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@UserId", iUserId);
                    sqlcmd.Parameters.AddWithValue("@SessionId", sSessionId);
                    sqlcmd.Parameters.AddWithValue("@LastSessionTime", dtLastSession);
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

        /// <summary>
        /// Update session time when logout
        /// </summary>
        /// <param name="sSessionId"></param>
        /// <returns></returns>
        public static bool UpdSessionLogout(string sSessionId)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_UpdSessionLogout", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@SessionId", sSessionId);
                    sqlcmd.Parameters.AddWithValue("@LastSessionTime", DateTime.UtcNow);
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

        /// <summary>
        /// Get no. of session still valid
        /// </summary>
        /// <param name="iLogin"></param>
        /// <returns></returns>
        public static bool GetValidSession(out int iLogin)
        {
            iLogin = 0;

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetValidSession", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@LastSessionTime", DateTime.UtcNow);
                    iLogin = (int)sqlcmd.ExecuteScalar();
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