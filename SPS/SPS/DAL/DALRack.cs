/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALRack.cs                                                                                           */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add GetRackByBuildingId, ChkExistRackNameCode, ChkExistRackName Azmir               */
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
    public class DALRack
    {
        /// <summary>
        /// Get rack model list
        /// </summary>
        /// <returns></returns>
        public static IList<RackModels> GetRackList()
        {
            List<RackModels> models = new List<RackModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRack", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        RackModels model = new RackModels
                        {
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackName = sqlreader["RackName"].ToString(),
                            RackCode = sqlreader["RackCode"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
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
        /// Register rack
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SetRack(RackModels model)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_SetRack", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackName", model.RackName);
                    sqlcmd.Parameters.AddWithValue("@RackCode", model.RackCode);
                    sqlcmd.Parameters.AddWithValue("@BuildingId", model.BuildingId);
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
        /// Update rack by id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdRack(RackModels model)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_UpdRack", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackId", model.RackId);
                    sqlcmd.Parameters.AddWithValue("@RackName", model.RackName);
                    sqlcmd.Parameters.AddWithValue("@RackCode", model.RackCode);
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
        /// Delete rack by id
        /// </summary>
        /// <param name="iRackId"></param>
        /// <returns></returns>
        public static bool DelRack(int iRackId)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_DelRack", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);
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
        /// Get rack model by id
        /// </summary>
        /// <param name="iRackId"></param>
        /// <returns></returns>
        public static RackModels GetRackBuildingById(int iRackId)
        {
            RackModels model = new RackModels();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRackBuildingById", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        model.RackId = Convert.ToInt32(sqlreader["RackId"]);
                        model.RackName = sqlreader["RackName"].ToString();
                        model.RackCode = sqlreader["RackCode"].ToString();
                        model.BuildingId = Convert.ToInt32(sqlreader["BuildingId"]);
                        model.BuildingName = sqlreader["BuildingName"].ToString();
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
        /// Check if rack exist under same building
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="sRackName"></param>
        /// <param name="sRackCode"></param>
        /// <param name="iBuildingId"></param>
        /// <returns></returns>
        public static bool ChkExistRack(out bool bResult, string sRackName, string sRackCode, int iBuildingId)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_ChkExistRack", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackName", sRackName);
                    sqlcmd.Parameters.AddWithValue("@RackCode", sRackCode);
                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);

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
        /// Check if rack code exist
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="sRackCode"></param>
        /// <returns></returns>
        public static bool ChkExistRackCode(out bool bResult, string sRackCode)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_ChkExistRackCode", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackCode", sRackCode);

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
        /// Get rack id by Rack code
        /// </summary>
        /// <param name="iRackId"></param>
        /// <param name="sRackCode"></param>
        /// <returns></returns>
        public static bool GetRackId(out int iRackId, string sRackCode)
        {
            iRackId = 0;

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRackId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackCode", sRackCode);

                    iRackId = Convert.ToInt32(sqlcmd.ExecuteScalar());
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
        /// Get rack by building id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <returns></returns>
        public static IList<RackModels> GetRackByBuildingId(int iBuildingId)
        {
            List<RackModels> models = new List<RackModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRackByBuildingId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        RackModels model = new RackModels
                        {
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackName = sqlreader["RackName"].ToString(),
                            RackCode = sqlreader["RackCode"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString()
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
        /// Check if rack name and code exist
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="sRackName"></param>
        /// <param name="sRackCode"></param>
        /// <returns></returns>
        public static bool ChkExistRackNameCode(out bool bResult, string sRackName, string sRackCode)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_ChkExistRackNameCode", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackName", sRackName);
                    sqlcmd.Parameters.AddWithValue("@RackCode", sRackCode);

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
        /// Check if rack name exist
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="sRackName"></param>
        /// <returns></returns>
        public static bool ChkExistRackName(out bool bResult, string sRackName)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_ChkExistRackName", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackName", sRackName);

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
        /// Get rack by building id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <returns></returns>
        public static IList<RackModels> GetRackListByBuildingId(int iBuildingId)
        {
            List<RackModels> models = new List<RackModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRackListByBuildingId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        RackModels model = new RackModels
                        {
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString()
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
    }
}