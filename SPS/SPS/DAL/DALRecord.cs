/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALRecord.cs                                                                                         */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add GetRecordByRackId, GetRecordByPartId, SetRecordByBatch,     Azmir               */
/*                  GetRptRecord, GetRecordByOnePartAllRack,                                            */
/*                  GetRecordByAllPartOneRack, GetRecordByAllPartOneRack,                               */
/*                  GetRecordByPartRack                                                                 */
/*      01/06/2022  Add GetRecordFromDtToDtStatus, GetRecordFromDtToDt,             Azmir               */
/*                  GetRecordByBuildingIdFromDtToDtStatus,                                              */
/*                  GetRecordByBuildingIdFromDtToDt                                                     */
/*      10/03/2023  Add SetRecordTransfer, check for partrack exist in SetRecord,   Azmir               */
/*                  check record qty is more than 0, Set another stored procedure                       */
/*                  to handle transfer function in all report DAL                                       */
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
    public class DALRecord
    {
        /// <summary>
        /// Get record model list
        /// </summary>
        /// <returns></returns>
        public static IList<RecordInOutModels> GetRecordList()
        {
            List<RecordInOutModels> models = new List<RecordInOutModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRecord", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        RecordInOutModels model = new RecordInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString(),
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

        /// <summary>
        /// Get record model by id
        /// </summary>
        /// <param name="iRecordId"></param>
        /// <returns></returns>
        public static RecordInOutModels GetRecordById(int iRecordId)
        {
            RecordInOutModels model = new RecordInOutModels();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRecordById", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RecordId", iRecordId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        model.RecordId = Convert.ToInt32(sqlreader["RecordId"]);
                        model.RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime();
                        model.RecordQty = Convert.ToInt32(sqlreader["RecordQty"]);
                        model.RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]);
                        model.RecordBy = sqlreader["RecordBy"].ToString();
                        model.RecordRemark = sqlreader["RecordRemark"].ToString();
                        model.PartId = Convert.ToInt32(sqlreader["PartId"]);
                        model.PartCode = sqlreader["PartCode"].ToString();
                        model.RackId = Convert.ToInt32(sqlreader["RackId"]);
                        model.RackCode = sqlreader["RackCode"].ToString();
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
        /// Register record 
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public static bool SetRecord(RecordInOutModels models)
        {
            string sCon = Common.GetDBStrCon();
            if (sCon == null)
            {
                return false;
            }

            SqlTransaction sqltran = null;

            using (SqlConnection sqlcon = new SqlConnection(sCon))
            {
                try
                {
                    sqlcon.Open();
                    sqltran = sqlcon.BeginTransaction();
                    SqlCommand sqlcmd = new SqlCommand();

                    sqlcmd = new SqlCommand("usp_SetRecord", sqlcon, sqltran)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RecordDateTime", models.RecordDateTime);
                    sqlcmd.Parameters.AddWithValue("@RecordQty", models.RecordQty);
                    sqlcmd.Parameters.AddWithValue("@RecordStatus", models.RecordStatus);
                    sqlcmd.Parameters.AddWithValue("@RecordBy", models.RecordBy);
                    sqlcmd.Parameters.AddWithValue("@RecordRemark", models.RecordRemark);
                    sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                    sqlcmd.Parameters.AddWithValue("@RackId", models.RackId);
                    sqlcmd.ExecuteNonQuery();

                    if (models.IsPartRackExist == true)
                    {
                        // Update existing part rack quantity
                        sqlcmd = new SqlCommand("usp_UpdPartRack", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@RackId", models.RackId);
                        sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                        sqlcmd.Parameters.AddWithValue("@PartQty", models.PartQty);
                        sqlcmd.ExecuteNonQuery();
#if false
                        if (models.PartQty == 0)
                        {
                            // Delete part rack
                            sqlcmd = new SqlCommand("usp_DelPartRack", sqlcon, sqltran)
                            {
                                CommandType = CommandType.StoredProcedure
                            };

                            sqlcmd.Parameters.AddWithValue("@RackId", models.RackId);
                            sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                            sqlcmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // Update existing part rack quantity
                            sqlcmd = new SqlCommand("usp_UpdPartRack", sqlcon, sqltran)
                            {
                                CommandType = CommandType.StoredProcedure
                            };

                            sqlcmd.Parameters.AddWithValue("@RackId", models.RackId);
                            sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                            sqlcmd.Parameters.AddWithValue("@PartQty", models.PartQty);
                            sqlcmd.ExecuteNonQuery();
                        }
#endif
                    }
                    else
                    {
                        // Register part rack
                        sqlcmd = new SqlCommand("usp_SetPartRack", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@RackId", models.RackId);
                        sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                        sqlcmd.Parameters.AddWithValue("@PartQty", models.PartQty);
                        sqlcmd.ExecuteNonQuery();
                    }

                    sqltran.Commit();
                }
                catch (Exception ex)
                {
                    if (sqltran != null)
                    {
                        sqltran.Rollback();
                    }

                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get record by rack id
        /// </summary>
        /// <param name="iRackId"></param>
        /// <returns></returns>
        public static IList<RecordInOutModels> GetRecordByRackId(int iRackId)
        {
            List<RecordInOutModels> models = new List<RecordInOutModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRecordByRackId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        RecordInOutModels model = new RecordInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString()
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
        /// Get record by part id
        /// </summary>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public static IList<RecordInOutModels> GetRecordByPartId(int iPartId)
        {
            List<RecordInOutModels> models = new List<RecordInOutModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRecordByPartId", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        RecordInOutModels model = new RecordInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            PartId = Convert.ToInt32(sqlreader["PartId"]),
                            PartCode = sqlreader["PartCode"].ToString()
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
        /// Register record by transfer
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public static bool SetRecordTransfer(RecordTransferModels models)
        {
            string sCon = Common.GetDBStrCon();
            if (sCon == null)
            {
                return false;
            }

            SqlTransaction sqltran = null;

            using (SqlConnection sqlcon = new SqlConnection(sCon))
            {
                try
                {
                    sqlcon.Open();
                    sqltran = sqlcon.BeginTransaction();
                    SqlCommand sqlcmd = new SqlCommand();

                    sqlcmd = new SqlCommand("usp_SetRecord", sqlcon, sqltran)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@RecordDateTime", models.RecordDateTime);
                    sqlcmd.Parameters.AddWithValue("@RecordQty", models.RecordQty);
                    sqlcmd.Parameters.AddWithValue("@RecordStatus", models.RecordStatus);
                    sqlcmd.Parameters.AddWithValue("@RecordBy", models.RecordBy);
                    sqlcmd.Parameters.AddWithValue("@RecordRemark", models.RecordRemark);
                    sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                    sqlcmd.Parameters.AddWithValue("@RackId", models.NewRackId);
                    sqlcmd.ExecuteNonQuery();

                    if (models.IsPartRackExist == true)
                    {
                        // Transfer all
                        if (models.RecordTransfer == true)
                        {
                            if (models.RecordTransferFullExists == false)
                            {
                                // Update rack id part id and part qty
                                sqlcmd = new SqlCommand("usp_UpdPartRackByRackIdPartIdPartQty", sqlcon, sqltran)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };

                                sqlcmd.Parameters.AddWithValue("@NewRackId", models.NewRackId);
                                sqlcmd.Parameters.AddWithValue("@RackId", models.RackId);
                                sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                                sqlcmd.Parameters.AddWithValue("@PartQty", models.PartQty);
                                sqlcmd.ExecuteNonQuery();
                            }
                            else
                            {
                                // If exists, update existing part qty
                                sqlcmd = new SqlCommand("usp_UpdPartRackByPartQty", sqlcon, sqltran)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };

                                sqlcmd.Parameters.AddWithValue("@RackId", models.NewRackId);
                                sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                                sqlcmd.Parameters.AddWithValue("@PartQty", models.PartQty);
                                sqlcmd.ExecuteNonQuery();

                                // After full transfer, delete old part rack
                                sqlcmd = new SqlCommand("usp_DelPartRack", sqlcon, sqltran)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };

                                sqlcmd.Parameters.AddWithValue("@RackId", models.RackId);
                                sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                                sqlcmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Transfer partially
                            if (models.RecordTransferPartiallyExists == false)
                            {
                                // Register new rack
                                sqlcmd = new SqlCommand("usp_SetPartRack", sqlcon, sqltran)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };

                                sqlcmd.Parameters.AddWithValue("@RackId", models.NewRackId);
                                sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                                sqlcmd.Parameters.AddWithValue("@PartQty", models.PartQty);
                                sqlcmd.ExecuteNonQuery();
                            }
                            else
                            {
                                // If part exist, update part quantity
                                sqlcmd = new SqlCommand("usp_UpdPartRack", sqlcon, sqltran)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };

                                sqlcmd.Parameters.AddWithValue("@RackId", models.NewRackId);
                                sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                                sqlcmd.Parameters.AddWithValue("@PartQty", models.TtlPartQty);
                                sqlcmd.ExecuteNonQuery();
                            }

                            // Update balance (part) quantity
                            sqlcmd = new SqlCommand("usp_UpdPartRack", sqlcon, sqltran)
                            {
                                CommandType = CommandType.StoredProcedure
                            };

                            sqlcmd.Parameters.AddWithValue("@RackId", models.RackId);
                            sqlcmd.Parameters.AddWithValue("@PartId", models.PartId);
                            sqlcmd.Parameters.AddWithValue("@PartQty", models.OldPartQty);
                            sqlcmd.ExecuteNonQuery();
                        }
                    }

                    sqltran.Commit();
                }
                catch (Exception ex)
                {
                    if (sqltran != null)
                    {
                        sqltran.Rollback();
                    }

                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Register/update building, rack, part and record(batch registration)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SetRecordByBatch(RecordImportModels model)
        {
            string sCon = Common.GetDBStrCon();
            if (sCon == null)
            {
                return false;
            }

            SqlTransaction sqltran = null;

            using (SqlConnection sqlcon = new SqlConnection(sCon))
            {
                try
                {
                    sqlcon.Open();
                    sqltran = sqlcon.BeginTransaction();
                    SqlCommand sqlcmd = new SqlCommand();

                    // Check if building exists in database
                    if (model.BuildingModel.BuildingId > 0)
                    {
                        // Update building
                        sqlcmd = new SqlCommand("usp_UpdBuilding", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@BuildingId", model.BuildingModel.BuildingId);
                        sqlcmd.Parameters.AddWithValue("@BuildingName", model.BuildingModel.BuildingName);
                        sqlcmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Register building
                        sqlcmd = new SqlCommand("usp_SetBuildingWithOutputId", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@BuildingName", model.BuildingModel.BuildingName);
                        model.BuildingModel.BuildingId = (int)sqlcmd.ExecuteScalar();
                    }

                    // Check if rack exists in database
                    if (model.RackModel.RackId > 0)
                    {
                        // Update rack
                        sqlcmd = new SqlCommand("usp_UpdRack", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@RackId", model.RackModel.RackId);
                        sqlcmd.Parameters.AddWithValue("@RackName", model.RackModel.RackName);
                        sqlcmd.Parameters.AddWithValue("@RackCode", model.RackModel.RackCode);
                        sqlcmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Register rack
                        sqlcmd = new SqlCommand("usp_SetRackWithOutputId", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@RackName", model.RackModel.RackName);
                        sqlcmd.Parameters.AddWithValue("@RackCode", model.RackModel.RackCode);
                        sqlcmd.Parameters.AddWithValue("@BuildingId", model.BuildingModel.BuildingId);
                        model.RackModel.RackId = (int)sqlcmd.ExecuteScalar();
                    }

                    // Check if part exists in database
                    if (model.PartModel.PartId > 0)
                    {
                        // Update part
                        sqlcmd = new SqlCommand("usp_UpdPart", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@PartId", model.PartModel.PartId);
                        sqlcmd.Parameters.AddWithValue("@PartCode", model.PartModel.PartCode);
                        sqlcmd.Parameters.AddWithValue("@PartName", model.PartModel.PartName);
                        sqlcmd.Parameters.AddWithValue("@PartDesc", model.PartModel.PartDesc);
                        sqlcmd.Parameters.AddWithValue("@PartFileName", model.PartModel.PartFileName);
                        sqlcmd.Parameters.AddWithValue("@PartGUIDFileName", model.PartModel.PartGUIDFileName);
                        sqlcmd.Parameters.AddWithValue("@PartMinQty", model.PartModel.PartMinQty);
                        sqlcmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Register part
                        sqlcmd = new SqlCommand("usp_SetPartWithOutputId", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@PartCode", model.PartModel.PartCode);
                        sqlcmd.Parameters.AddWithValue("@PartName", model.PartModel.PartName);
                        sqlcmd.Parameters.AddWithValue("@PartDesc", model.PartModel.PartDesc);
                        sqlcmd.Parameters.AddWithValue("@PartFileName", model.PartModel.PartFileName);
                        sqlcmd.Parameters.AddWithValue("@PartGUIDFileName", model.PartModel.PartGUIDFileName);
                        sqlcmd.Parameters.AddWithValue("@PartMinQty", model.PartModel.PartMinQty);
                        model.PartModel.PartId = (int)sqlcmd.ExecuteScalar();
                    }

                    // Check if part rack exists in database
                    if (model.RecordInOutModel.IsPartRackExist)
                    {
                        // Update part rack
                        sqlcmd = new SqlCommand("usp_UpdPartRack", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@RackId", model.RackModel.RackId);
                        sqlcmd.Parameters.AddWithValue("@PartId", model.PartModel.PartId);
                        sqlcmd.Parameters.AddWithValue("@PartQty", model.RecordInOutModel.PartQty);
                        sqlcmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Register part rack
                        sqlcmd = new SqlCommand("usp_SetPartRack", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@RackId", model.RackModel.RackId);
                        sqlcmd.Parameters.AddWithValue("@PartId", model.PartModel.PartId);
                        sqlcmd.Parameters.AddWithValue("@PartQty", model.RecordInOutModel.PartQty);
                        sqlcmd.ExecuteNonQuery();
                    }

                    // Check if record quantity is more than 0
                    if (model.RecordInOutModel.RecordQty >= 0)
                    {
                        // Set record
                        sqlcmd = new SqlCommand("usp_SetRecord", sqlcon, sqltran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        sqlcmd.Parameters.AddWithValue("@RecordDateTime", model.RecordInOutModel.RecordDateTime);
                        sqlcmd.Parameters.AddWithValue("@RecordQty", model.RecordInOutModel.RecordQty);
                        sqlcmd.Parameters.AddWithValue("@RecordStatus", model.RecordInOutModel.RecordStatus);
                        sqlcmd.Parameters.AddWithValue("@RecordBy", model.RecordInOutModel.RecordBy);
                        sqlcmd.Parameters.AddWithValue("@RecordRemark", model.RecordInOutModel.RecordRemark);
                        sqlcmd.Parameters.AddWithValue("@PartId", model.PartModel.PartId);
                        sqlcmd.Parameters.AddWithValue("@RackId", model.RackModel.RackId);
                        sqlcmd.ExecuteNonQuery();
                    }

                    sqltran.Commit();
                }
                catch (Exception ex)
                {
                    if (sqltran != null)
                    {
                        sqltran.Rollback();
                    }

                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        #region Report Record

        /// <summary>
        /// Report - Get all record by date from, date to and status
        /// </summary>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordFromDtToDtStatus(DateTime dtFrom, DateTime dtTo, int iStatus, bool bEqualStatus)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd;

                    if (bEqualStatus == true)
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordFromDtToDtStatus", sqlcon);
                    }
                    else
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordFromDtToDtNotEqualStatus", sqlcon);
                    }

                    sqlcmd.CommandType = CommandType.StoredProcedure;


                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);
                    sqlcmd.Parameters.AddWithValue("@RecordStatus", iStatus);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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
        /// Report - Get all record by building id, date from, date to and status
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordByBuildingIdFromDtToDtStatus(int iBuildingId, DateTime dtFrom, DateTime dtTo, int iStatus, bool bEqualStatus)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd;

                    if (bEqualStatus == true)
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordByBuildingIdFromDtToDtStatus", sqlcon);
                    }
                    else
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordByBuildingIdFromDtToDtNotEqualStatus", sqlcon);
                    }

                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);
                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);
                    sqlcmd.Parameters.AddWithValue("@RecordStatus", iStatus);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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
        /// Report - Get all record by building id, rack id, date from, date to and status
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordByBuildingIdRackIdFromDtToDtStatus(int iBuildingId, int iRackId, DateTime dtFrom, DateTime dtTo, int iStatus, bool bEqualStatus)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd;

                    if (bEqualStatus == true)
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordByBuildingIdRackIdFromDtToDtStatus", sqlcon);
                    }
                    else
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordByBuildingIdRackIdFromDtToDtNotEqualStatus", sqlcon);
                    }

                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);
                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);
                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);
                    sqlcmd.Parameters.AddWithValue("@RecordStatus", iStatus);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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
        /// Report - Get all record by building id, rack id, part id, date from, date to and status
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="iPartId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordByBuildingIdRackIdPartIdFromDtToDtStatus(int iBuildingId, int iRackId, int iPartId, DateTime dtFrom, DateTime dtTo, int iStatus, bool bEqualStatus)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd;

                    if (bEqualStatus == true)
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordByBuildingIdRackIdPartIdFromDtToDtStatus", sqlcon);
                    }
                    else
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordByBuildingIdRackIdPartIdFromDtToDtNotEqualStatus", sqlcon);
                    }

                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);
                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);
                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);
                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);
                    sqlcmd.Parameters.AddWithValue("@RecordStatus", iStatus);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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
        /// Report - Get all record by part id, date from, date to and status 
        /// </summary>
        /// <param name="iPartId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordByPartIdFromDtToDtStatus(int iPartId, DateTime dtFrom, DateTime dtTo, int iStatus, bool bEqualStatus)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd;

                    if (bEqualStatus == true)
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordByPartIdFromDtToDtStatus", sqlcon);
                    }
                    else
                    {
                        sqlcmd = new SqlCommand("usp_GetRecordByPartIdFromDtToDtNotEqualStatus", sqlcon);
                    }

                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);
                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);
                    sqlcmd.Parameters.AddWithValue("@RecordStatus", iStatus);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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
        /// Report - Get all record by date from and date to 
        /// </summary>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordFromDtToDt(DateTime dtFrom, DateTime dtTo)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRecordFromDtToDt", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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
        /// Report - Get all record by building id, date from and date to 
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordByBuildingIdFromDtToDt(int iBuildingId, DateTime dtFrom, DateTime dtTo)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRecordByBuildingIdFromDtToDt", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);
                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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
        /// Report - Get all record by building id, rack id, date from and date to 
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordByBuildingIdRackIdFromDtToDt(int iBuildingId, int iRackId, DateTime dtFrom, DateTime dtTo)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRecordByBuildingIdRackIdFromDtToDt", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);
                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);
                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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
        /// Report - Get all record by building id, rack id, part id, date from and date to 
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="iPartId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordByBuildingIdRackIdPartIdFromDtToDt(int iBuildingId, int iRackId, int iPartId, DateTime dtFrom, DateTime dtTo)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRecordByBuildingIdRackIdPartIdFromDtToDt", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@BuildingId", iBuildingId);
                    sqlcmd.Parameters.AddWithValue("@RackId", iRackId);
                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);
                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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
        /// Report - Get all record by part id, date from and date to 
        /// </summary>
        /// <param name="iPartId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public static IList<RptInOutModels> GetRecordByPartIdFromDtToDt(int iPartId, DateTime dtFrom, DateTime dtTo)
        {
            List<RptInOutModels> models = new List<RptInOutModels>();

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

                    SqlCommand sqlcmd = new SqlCommand("usp_GetRecordByPartIdFromDtToDt", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@PartId", iPartId);
                    sqlcmd.Parameters.AddWithValue("@FromDT", dtFrom);
                    sqlcmd.Parameters.AddWithValue("@ToDT", dtTo);

                    SqlDataReader sqlreader = sqlcmd.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        RptInOutModels model = new RptInOutModels
                        {
                            RecordId = Convert.ToInt32(sqlreader["RecordId"]),
                            RecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime(),
                            SRecordDateTime = Convert.ToDateTime(sqlreader["RecordDateTime"]).ToLocalTime().ToString("dd-MM-yyyy hh:mm tt"),
                            RecordQty = Convert.ToInt32(sqlreader["RecordQty"]),
                            RecordStatus = Convert.ToInt32(sqlreader["RecordStatus"]),
                            RecordBy = sqlreader["RecordBy"].ToString(),
                            RecordRemark = sqlreader["RecordRemark"].ToString(),
                            BuildingId = Convert.ToInt32(sqlreader["BuildingId"]),
                            BuildingName = sqlreader["BuildingName"].ToString(),
                            RackId = Convert.ToInt32(sqlreader["RackId"]),
                            RackCode = sqlreader["RackCode"].ToString(),
                            RackName = sqlreader["RackName"].ToString(),
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

        #endregion
    }
}