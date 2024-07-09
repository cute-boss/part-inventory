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

using SPS.Models;
using SPSLib;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SPS.DAL
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

        /// <summary>
        /// Get misc list
        /// </summary>
        /// <returns></returns>
        public static MiscModels GetMiscList()
        {
            MiscModels miscModels = new MiscModels();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetMisc", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        miscModels.UseEmail = Convert.ToBoolean(sqlreader["UseEmail"]);
                        miscModels.EmailSmtp = sqlreader["EmailSmtp"].ToString();
                        miscModels.EmailPort = sqlreader["EmailPort"].ToString();
                        miscModels.EmailProtocol = Convert.ToInt32(sqlreader["EmailProtocol"]);
                        miscModels.EmailUsername = sqlreader["EmailUsername"].ToString();
                        miscModels.EmailPassword = sqlreader["EmailPassword"].ToString();
                        miscModels.RetentionPeriod = Convert.ToInt32(sqlreader["RetentionPeriod"]);
                        miscModels.AttachmentSize = Convert.ToInt32(sqlreader["AttachmentSize"]);
                        miscModels.IdleTime = Convert.ToInt32(sqlreader["IdleTime"]);
                        miscModels.TokenResetTime = Convert.ToInt32(sqlreader["TokenResetTime"]);
                        miscModels.DefaultEmail = sqlreader["DefaultEmail"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return null;
                }
            }

            return miscModels;
        }

        /// <summary>
        /// Update miscellaneous settings
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public static bool UpdMisc(MiscModels model)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_UpdMisc", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@UseEmail", model.UseEmail);
                    sqlcmd.Parameters.AddWithValue("@EmailSmtp", model.EmailSmtp);
                    sqlcmd.Parameters.AddWithValue("@EmailPort", model.EmailPort);
                    sqlcmd.Parameters.AddWithValue("@EmailProtocol", model.EmailProtocol);
                    sqlcmd.Parameters.AddWithValue("@EmailUsername", model.EmailUsername);
                    sqlcmd.Parameters.AddWithValue("@EmailPassword", model.EmailPassword);
                    sqlcmd.Parameters.AddWithValue("@RetentionPeriod", model.RetentionPeriod);
                    sqlcmd.Parameters.AddWithValue("@AttachmentSize", model.AttachmentSize);
                    sqlcmd.Parameters.AddWithValue("@IdleTime", model.IdleTime);
                    sqlcmd.Parameters.AddWithValue("@TokenResetTime", model.TokenResetTime);
                    sqlcmd.Parameters.AddWithValue("@DefaultEmail", model.DefaultEmail);
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
    }
}