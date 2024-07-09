/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALUser.cs                                                                                           */
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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SPS.DAL
{
    public class DALUser
    {
        /// <summary>
        /// Get user list
        /// </summary>
        /// <returns></returns>
        public static IList<UserModels> GetUserProfile()
        {
            List<UserModels> modelList = new List<UserModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetUserProfile", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        UserModels model = new UserModels
                        {
                            UserId = Convert.ToInt32(sqlreader["UserId"]),
                            UserName = sqlreader["UserName"].ToString(),
                            RoleId = Convert.ToInt32(sqlreader["RoleId"]),
                            RoleName = sqlreader["RoleName"].ToString(),
                            IsActive = Convert.ToBoolean(sqlreader["IsActive"]),
                            StaffNo = sqlreader["StaffNo"].ToString(),
                            StaffName = sqlreader["StaffName"].ToString(),
                            Email = sqlreader["Email"].ToString()
                        };
                        modelList.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return null;
                }
            }

            return modelList;
        }

        /// <summary>
        /// Get user model
        /// </summary>
        /// <param name="iUserId"></param>
        /// <returns></returns>
        public static UserModels GetUserProfileById(int iUserId)
        {
            UserModels model = new UserModels();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetUserProfileById", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@UserId", iUserId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        model.UserId = Convert.ToInt32(sqlreader["UserId"]);
                        model.UserName = sqlreader["UserName"].ToString();
                        model.RoleId = Convert.ToInt32(sqlreader["RoleId"]);
                        model.RoleName = sqlreader["RoleName"].ToString();
                        model.IsActive = Convert.ToBoolean(sqlreader["IsActive"]);
                        model.StaffNo = sqlreader["StaffNo"].ToString();
                        model.StaffName = sqlreader["StaffName"].ToString();
                        model.Email = sqlreader["Email"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return null;
                }
            }

            return model;
        }

        /// <summary>
        /// Check if staff no exist
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="sStaffNo"></param>
        /// <returns></returns>
        public static bool ChkExistStaffNo(out bool bResult, string sStaffNo)
        {
            bResult = false;

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

                    SqlCommand sqlcmd = new SqlCommand("usp_ChkExistStaffNo", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@StaffNo", sStaffNo);

                    bResult = Convert.ToBoolean((int)sqlcmd.ExecuteScalar());
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
        /// Check if email exist
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="sEmail"></param>
        /// <returns></returns>
        public static bool ChkExistEmail(out bool bResult, string sEmail)
        {
            bResult = false;

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

                    SqlCommand sqlcmd = new SqlCommand("usp_ChkExistEmail", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@Email", sEmail);

                    bResult = Convert.ToBoolean((int)sqlcmd.ExecuteScalar());
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
        /// Update user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdUser(UserModels model)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_UpdUser", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@UserId", model.UserId);
                    sqlcmd.Parameters.AddWithValue("@StaffName", model.StaffName);
                    sqlcmd.Parameters.AddWithValue("@StaffNo", model.StaffNo);
                    sqlcmd.Parameters.AddWithValue("@Email", model.Email);
                    sqlcmd.Parameters.AddWithValue("@IsActive", model.IsActive);
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
        /// Get membership information by user ID
        /// </summary>
        /// <param name="tblResult">Output result</param>
        /// <param name="iUserId">User ID</param>
        /// <returns></returns>
        public static bool GetMembershipById(out DataTable tblResult, int? iUserId)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetMembershipById", sqlcon)
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
        /// Get role model list
        /// </summary>
        /// <returns></returns>
        public static IList<RoleModels> GetRoleList()
        {
            IList<RoleModels> models = new List<RoleModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRole", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        RoleModels model = new RoleModels
                        {
                            RoleId = Convert.ToInt32(sqlreader["RoleId"]),
                            RoleName = sqlreader["RoleName"].ToString()
                        };
                        models.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return null;
                }
            }

            return models;
        }

        /// <summary>
        /// Get username by reset password token
        /// </summary>
        /// <param name="sUsername"></param>
        /// <param name="sToken"></param>
        /// <returns></returns>
        public static bool GetUsernameByResetToken(out string sUsername, string sToken)
        {
            sUsername = "";

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetUsernameByResetToken", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PasswordVerificationToken", sToken);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        sUsername = sqlreader["UserName"].ToString();
                    }
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
        /// Get email by user id
        /// </summary>
        /// <param name="sEmail"></param>
        /// <param name="iUserId"></param>
        /// <returns></returns>
        public static bool GetEmailByUserId(out string sEmail, int iUserId)
        {
            sEmail = "";

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetEmailByUserId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@UserId", iUserId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        sEmail = sqlreader["Email"].ToString();
                    }
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