/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALPartRack.cs                                                                                       */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      26/04/2022  Initial version                                                 Azmir               */
/*      01/06/2022  Add GetInventoryList, GetInventoryByBuildingId                  Azmir               */
/*      09/03/2023  Add PartMinQty to display minimum qty                           Azmir               */
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
    public class DALPartRack
    {
        /// <summary>
        /// Get part rack model list
        /// </summary>
        /// <returns></returns>
        public static IList<PartRackModels> GetPartRackList()
        {
            List<PartRackModels> models = new List<PartRackModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartRackList", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        PartRackModels model = new PartRackModels
                        {
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
                            PartName = sqlreader["PartName"].ToString(),
                            PartQty = Convert.ToInt32(sqlreader["PartQty"]),
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
        /// Check if part rack exist
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="iRackId"></param>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static bool ChkExistPartRack(out bool bResult, int iRackId, int iPartId)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_ChkExistPartRack", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);
                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

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
        /// Get part by rack id and part id
        /// </summary>
        /// <param name="iRackId"></param>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static PartRackModels GetPartByRackIdPartId(int iRackId, int iPartId)
        {
            PartRackModels model = new PartRackModels();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartByRackIdPartId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);
                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {

                        model.PartId = Convert.ToInt32(sqlreader["PartId"]);
                        model.PartCode = sqlreader["PartCode"].ToString();
                        model.PartQty = Convert.ToInt32(sqlreader["PartQty"]);
                        model.PartGUIDFileName = sqlreader["PartGUIDFileName"].ToString();
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
        /// Get part balance qty by rack id and part id
        /// </summary>
        /// <param name="iPartQty"></param>
        /// <param name="iRackId"></param>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static bool GetPartQtyByRackIdPartId(out int iPartQty, int iRackId, int iPartId)
        {
            iPartQty = 0;

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartQtyByRackIdPartId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);
                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

                    iPartQty = Convert.ToInt32(sqlcmd.ExecuteScalar());
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
        /// Get part by rack id
        /// </summary>
        /// <param name="iRackId"></param>
        /// <returns></returns>
        public static IList<PartRackModels> GetPartRackByRackId(int iRackId)
        {
            List<PartRackModels> models = new List<PartRackModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartRackByRackId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        PartRackModels model = new PartRackModels
                        {
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
                            PartName = sqlreader["PartName"].ToString(),
                            PartDesc = sqlreader["PartDesc"].ToString()
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
        /// Report - Get all inventory list
        /// </summary>
        /// <returns></returns>
        public static IList<RptInventoryModels> GetInventoryList()
        {
            List<RptInventoryModels> models = new List<RptInventoryModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetInventoryList", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInventoryModels model = new RptInventoryModels
                        {
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
                            PartName = sqlreader["PartName"].ToString(),
                            PartDesc = sqlreader["PartDesc"].ToString(),
                            PartMinQty = Convert.ToInt32(sqlreader["PartMinQty"]),
                            PartQty = Convert.ToInt32(sqlreader["PartQty"])
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
        /// Report - Get all inventory by building id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <returns></returns>
        public static IList<RptInventoryModels> GetInventoryByBuildingId(int iBuildingId)
        {
            List<RptInventoryModels> models = new List<RptInventoryModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetInventoryByBuildingId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInventoryModels model = new RptInventoryModels
                        {
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
                            PartName = sqlreader["PartName"].ToString(),
                            PartDesc = sqlreader["PartDesc"].ToString(),
                            PartMinQty = Convert.ToInt32(sqlreader["PartMinQty"]),
                            PartQty = Convert.ToInt32(sqlreader["PartQty"])
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
        /// Report - Get all inventory by building id, rack id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <returns></returns>
        public static IList<RptInventoryModels> GetInventoryByBuildingIdRackId(int iBuildingId, int iRackId)
        {
            List<RptInventoryModels> models = new List<RptInventoryModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetInventoryByBuildingIdRackId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);
                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInventoryModels model = new RptInventoryModels
                        {
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
                            PartName = sqlreader["PartName"].ToString(),
                            PartDesc = sqlreader["PartDesc"].ToString(),
                            PartMinQty = Convert.ToInt32(sqlreader["PartMinQty"]),
                            PartQty = Convert.ToInt32(sqlreader["PartQty"])
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
        /// Report - Get all inventory by building id, rack id, part id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static IList<RptInventoryModels> GetInventoryByBuildingIdRackIdPartId(int iBuildingId, int iRackId, int iPartId)
        {
            List<RptInventoryModels> models = new List<RptInventoryModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetInventoryByBuildingIdRackIdPartId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);
                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);
                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInventoryModels model = new RptInventoryModels
                        {
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
                            PartName = sqlreader["PartName"].ToString(),
                            PartDesc = sqlreader["PartDesc"].ToString(),
                            PartMinQty = Convert.ToInt32(sqlreader["PartMinQty"]),
                            PartQty = Convert.ToInt32(sqlreader["PartQty"])
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
        /// Report - Get all inventory by part id
        /// </summary>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static IList<RptInventoryModels> GetInventoryByPartId(int iPartId)
        {
            List<RptInventoryModels> models = new List<RptInventoryModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetInventoryByPartId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInventoryModels model = new RptInventoryModels
                        {
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
                            PartName = sqlreader["PartName"].ToString(),
                            PartDesc = sqlreader["PartDesc"].ToString(),
                            PartMinQty = Convert.ToInt32(sqlreader["PartMinQty"]),
                            PartQty = Convert.ToInt32(sqlreader["PartQty"])
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