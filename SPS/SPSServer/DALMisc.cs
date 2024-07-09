/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALMisc.cs                                                                                           */
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
    public class DALMisc
    {
        /// <summary>
        /// Get miscellaneous settings
        /// </summary>
        /// <param name="misc">Misc Settings</param>
        /// <returns></returns>
        public static bool GetMisc(out Misc misc)
        {
            misc = new Misc();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetMisc", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        #region read data
                        misc.UseEmail = Convert.ToBoolean(sqlreader["UseEmail"]);
                        misc.EmailSmtp = sqlreader["EmailSmtp"].ToString();
                        misc.EmailPort = sqlreader["EmailPort"].ToString();
                        misc.EmailProtocol = Convert.ToInt32(sqlreader["EmailProtocol"]);
                        misc.EmailUsername = sqlreader["EmailUsername"].ToString();
                        misc.EmailPassword = sqlreader["EmailPassword"].ToString();
                        misc.RetentionPeriod = Convert.ToInt32(sqlreader["RetentionPeriod"]);
                        misc.AttachmentSize = Convert.ToInt32(sqlreader["AttachmentSize"]);
                        misc.IdleTime = Convert.ToInt32(sqlreader["IdleTime"]);
                        misc.TokenResetTime = Convert.ToInt32(sqlreader["TokenResetTime"]);
                        misc.DefaultEmail = sqlreader["DefaultEmail"].ToString();
                        #endregion
                    }
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
    }
}
