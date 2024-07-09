/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALPart.cs                                                                                           */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      25/07/2022  Add GetPartNBalQtyList &  GetPartNBalQtyById                    Azmir               */
/*      14/02/2023  Add PartMinQty to GetPartById, UpdPart, SetPart & check         Azmir               */
/*                  balance quantity is null at GetPartNBalQtyById                                      */
/*      14/02/2023  Add PartMinQty input & focus when error                         Azmir               */
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
    public class DALPart
    {
        /// <summary>
        /// Get part model list
        /// </summary>
        /// <returns></returns>
        public static IList<PartModels> GetPartList()
        {
            List<PartModels> models = new List<PartModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPart", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        PartModels model = new PartModels
                        {
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
                            PartName = sqlreader["PartName"].ToString(),
                            PartDesc = sqlreader["PartDesc"].ToString(),
                            PartFileName = sqlreader["PartFileName"].ToString(),
                            PartGUIDFileName = sqlreader["PartGUIDFileName"].ToString()
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
        /// Get part model by id
        /// </summary>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static PartModels GetPartById(int iPartId)
        {
            PartModels model = new PartModels();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartById", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        model.PartId = Convert.ToInt32(sqlreader["PartId"]);
                        model.PartCode = sqlreader["PartCode"].ToString();
                        model.PartName = sqlreader["PartName"].ToString();
                        model.PartDesc = sqlreader["PartDesc"].ToString();
                        model.PartFileName = sqlreader["PartFileName"].ToString();
                        model.PartGUIDFileName = sqlreader["PartGUIDFileName"].ToString();
                        model.PartMinQty = Convert.ToInt32(sqlreader["PartMinQty"]);
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
        /// Register part
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SetPart(PartModels model)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_SetPart", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartCode", model.PartCode);
                    sqlcmd.Parameters.AddWithValue("@PartName", model.PartName);
                    sqlcmd.Parameters.AddWithValue("@PartDesc", model.PartDesc);
                    sqlcmd.Parameters.AddWithValue("@PartFileName", model.PartFileName);
                    sqlcmd.Parameters.AddWithValue("@PartGUIDFileName", model.PartGUIDFileName);
                    sqlcmd.Parameters.AddWithValue("@PartMinQty", model.PartMinQty);
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
        /// Update part by id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdPart(PartModels model)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_UpdPart", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartId", model.PartId);
                    sqlcmd.Parameters.AddWithValue("@PartCode", model.PartCode);
                    sqlcmd.Parameters.AddWithValue("@PartName", model.PartName);
                    sqlcmd.Parameters.AddWithValue("@PartDesc", model.PartDesc);
                    sqlcmd.Parameters.AddWithValue("@PartFileName", model.PartFileName);
                    sqlcmd.Parameters.AddWithValue("@PartGUIDFileName", model.PartGUIDFileName);
                    sqlcmd.Parameters.AddWithValue("@PartMinQty", model.PartMinQty);
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
        /// Delete part by id
        /// </summary>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static bool DelPart(int iPartId)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_DelPart", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);
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
        /// Check if part code exist
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="iPartCode"></param>
        /// <returns></returns>
        public static bool ChkExistPartCode(out bool bResult, string iPartCode)
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

                    SqlCommand sqlcmd = new SqlCommand("usp_ChkExistPartCode", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartCode", iPartCode);

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
        /// Get part id by part code
        /// </summary>
        /// <param name="iPartId"></param>
        /// <param name="sPartCode"></param>
        /// <returns></returns>
        public static bool GetPartId(out int iPartId, string sPartCode)
        {
            iPartId = 0;

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartCode", sPartCode);

                    iPartId = Convert.ToInt32(sqlcmd.ExecuteScalar());
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
        /// Get part file name by part id
        /// </summary>
        /// <param name="sPartFileName"></param>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static bool GetPartFileNameByPartId(out string sPartFileName, int iPartId)
        {
            sPartFileName = "";

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartFileNameByPartId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

                    sPartFileName = Convert.ToString(sqlcmd.ExecuteScalar());
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
        /// Get part GUID file name by part id
        /// </summary>
        /// <param name="sPartGUIDFileName"></param>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static bool GetPartGUIDFileNameByPartId(out string sPartGUIDFileName, int iPartId)
        {
            sPartGUIDFileName = "";

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartGUIDFileNameByPartId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

                    sPartGUIDFileName = Convert.ToString(sqlcmd.ExecuteScalar());
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
        /// Get part and balance qty model list
        /// </summary>
        /// <returns></returns>
        public static IList<PartModels> GetPartNBalQtyList()
        {
            List<PartModels> models = new List<PartModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartNBalQtyList", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        PartModels model = new PartModels
                        {
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
                            PartName = sqlreader["PartName"].ToString(),
                            PartDesc = sqlreader["PartDesc"].ToString(),
                            PartFileName = sqlreader["PartFileName"].ToString(),
                            PartGUIDFileName = sqlreader["PartGUIDFileName"].ToString(),
                            PartMinQty = Convert.ToInt32(sqlreader["PartMinQty"])
                        };

                        // Check if balance quantity is null
                        if (sqlreader["BalanceQty"] != DBNull.Value)
                        {
                            model.BalanceQty = Convert.ToInt32(sqlreader["BalanceQty"]);
                        }
                        else
                        {
                            model.BalanceQty = 0;
                        }

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
        /// Get part and balance qty model by id
        /// </summary>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static PartModels GetPartNBalQtyById(int iPartId)
        {
            PartModels model = new PartModels();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartNBalQtyById", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        model.PartId = Convert.ToInt32(sqlreader["PartId"]);
                        model.PartCode = sqlreader["PartCode"].ToString();
                        model.PartName = sqlreader["PartName"].ToString();
                        model.PartDesc = sqlreader["PartDesc"].ToString();
                        model.PartFileName = sqlreader["PartFileName"].ToString();
                        model.PartGUIDFileName = sqlreader["PartGUIDFileName"].ToString();
                        model.PartMinQty = Convert.ToInt32(sqlreader["PartMinQty"]);

                        // Check if balance quantity is null
                        if (sqlreader["BalanceQty"] != DBNull.Value)
                        {
                            model.BalanceQty = Convert.ToInt32(sqlreader["BalanceQty"]);
                        }
                        else
                        {
                            model.BalanceQty = 0;
                        }
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
    }
}