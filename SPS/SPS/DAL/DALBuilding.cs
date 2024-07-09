/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALBuilding.cs                                                                                       */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add GetBuildingIdByName                                         Azmir               */
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
    public class DALBuilding
    {
        /// <summary>
        /// Get building model list
        /// </summary>
        /// <returns></returns>
        public static IList<BuildingModels> GetBuildingList()
        {
            List<BuildingModels> models = new List<BuildingModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetBuilding", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        BuildingModels model = new BuildingModels
                        {
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
        /// Get building model by id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <returns></returns>
        public static BuildingModels GetBuildingById(int iBuildingId)
        {
            BuildingModels model = new BuildingModels();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetBuildingById", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
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
        /// Register building
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SetBuilding(BuildingModels model)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_SetBuilding", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingName", model.BuildingName);
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
        /// Update building by id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdBuilding(BuildingModels model)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_UpdBuilding", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", model.BuildingId);
                    sqlcmd.Parameters.AddWithValue("@BuildingName", model.BuildingName);
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
        /// Delete building by id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <returns></returns>
        public static bool DelBuilding(int iBuildingId)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_DelBuilding", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);
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
        /// Check if building exist
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="sBuildingName"></param>
        /// <returns></returns>
        public static bool ChkExistBuilding(out bool bResult, string sBuildingName)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_ChkExistBuilding", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingName", sBuildingName);
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
        /// Get building Id by building name
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="sBuildingName"></param>
        /// <returns></returns>
        public static bool GetBuildingIdByName(out int iBuildingId, string sBuildingName)
        {
            iBuildingId = 0;

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetBuildingIdByName", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingName", sBuildingName);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        iBuildingId = Convert.ToInt32(sqlreader["BuildingId"]);
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