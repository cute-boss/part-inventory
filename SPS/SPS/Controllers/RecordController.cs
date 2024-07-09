/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RecordController.cs                                                                                  */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add import func                                                 Azmir               */
/*      28/07/2022  Remain qty if part is exist in record information               Azmir               */
/*      10/03/2023  Add record transfer, increase max column header in excel sheet, Azmir               */
/*                  add min qty to excel sheet, add StringToCSVCell func to handle                      */
/*                  double quotes during import & load part based on rack for                           */
/*                  out and transfer function                                                           */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.DAL;
using SPS.Filters;
using SPS.Models;
using SPSLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace SPS.Controllers
{
    [Authorize(Roles = "ADMIN, USER")]
    [CheckSession]
    public class RecordController : Controller
    {
        // GET: Record
        public ActionResult Index()
        {
            ViewBag.Current = "Record";

            IList<RecordInOutModels> modelList = DALRecord.GetRecordList();
            return View(modelList);
        }

        public ActionResult Detail(int id)
        {
            ViewBag.Current = "Record";

            RecordInOutModels model = DALRecord.GetRecordById(id);

            // Check if id is invalid
            if (model.RecordId == 0)
            {
                TempData["ErrMsg"] = CommonMsg.INVALID_ID;
                return RedirectToAction("Index", "Record");
            }

            return View(model);
        }

        public ActionResult RecordType()
        {
            ViewBag.Current = "Record";

            RecordTypeModels model = new RecordTypeModels();
            LoadRecordTypeModelDdl(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecordType(RecordTypeModels model)
        {
            ViewBag.Current = "Record";

            // Trim property
            //model.RackCode = model.RackCode.Trim().ToUpper();

            if (ModelState.IsValid)
            {
                // Check if rack is exist
                DALRack.ChkExistRackCode(out bool bExist, model.RackCode);
                if (bExist == false)
                {
                    ViewBag.Message = CommonMsg.INV_RACK;
                    LoadRecordTypeModelDdl(model);
                    return View(model);
                }
            }

            // Set record status
            if (model.RecordStatus == (int)EnumEx.RecordStatus.STATUS_TRANSFER)
            {
                ViewBag.Status = "Transfer";
            }
            else
            {
                ViewBag.Status = "InOut";
            }

            ViewBag.Message = CommonMsg.PROCEED_RECORD_TYPE;
            LoadRecordTypeModelDdl(model);
            return View(model);
        }

        public ActionResult RecordInOut(string sRackCode, int iRecordStatus)
        {
            ViewBag.Current = "Record";

            RecordInOutModels model = new RecordInOutModels();

            // Get rack id
            DALRack.GetRackId(out int iRackId, sRackCode);
            // Check if id is invalid
            if (iRackId < 1)
            {
                ViewBag.Message = CommonMsg.INV_RACK;
                return View(model);
            }

            model.RackId = iRackId;
            model.RecordTypeModel.RackCode = sRackCode;
            model.RecordTypeModel.RecordStatus = iRecordStatus;
            TempData["UploadPath"] = ConfigurationManager.AppSettings["UploadPath"];
            model.PartImage = ConfigurationManager.AppSettings["UploadPath"] + "no_image.png";

            LoadRecordModelDdl(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecordInOut(RecordInOutModels model)
        {
            ViewBag.Current = "Record";

            // Trim property
            model.RecordRemark = model.RecordRemark.Trim();

            // Set initial view value
            model.RackCode = model.RecordTypeModel.RackCode;
            model.RecordDateTime = DateTime.UtcNow;
            model.RecordBy = WebSecurity.CurrentUserName;
            model.RecordStatus = model.RecordTypeModel.RecordStatus;

            if (ModelState.IsValid)
            {
                // Get part code
                PartModels partModel = DALPart.GetPartById(model.PartId);
                // Check if id is invalid
                if (partModel.PartId < 1)
                {
                    ViewBag.Message = CommonMsg.INVALID_ID;
                    LoadRecordModelDdl(model);
                    return View(model);
                }

                model.PartCode = partModel.PartCode;

                // Check if record qty is 0
                if(model.RecordQty == 0)
                {
                    ViewBag.Message = CommonMsg.INV_RECORD_QTY;
                    LoadRecordModelDdl(model);
                    return View(model);
                }

                // Check if part rack exist
                DALPartRack.ChkExistPartRack(out bool bExist, model.RackId, model.PartId);
                if (bExist)
                {
                    model.IsPartRackExist = true;
                }

                // Check record status and update part quantity
                if (model.RecordStatus == (int)EnumEx.RecordStatus.STATUS_IN)
                {
                    model.PartQty += model.RecordQty;
                }
                else
                {
                    // Check part qty is not lower than insert part qty
                    if (model.PartQty < model.RecordQty)
                    {
                        ViewBag.Message = CommonMsg.NEGATIVE_UPD_RECORD;
                        LoadRecordModelDdl(model);
                        return View(model);
                    }

                    model.PartQty -= model.RecordQty;
                }

                if (DALRecord.SetRecord(model))
                {
                    #region Log
                    string sLogDesc = "Add [";
                    sLogDesc += "Part Code: " + model.PartCode + " | ";
                    sLogDesc += "Rack Code: " + model.RackCode + " | ";
                    sLogDesc += "Date/Time: " + model.RecordDateTime + " | ";
                    sLogDesc += "Quantity: " + model.RecordQty + " | ";
                    sLogDesc += "Record Status: " + model.RecordStatus + " | ";
                    sLogDesc += "Record By: " + model.RecordBy + " | ";
                    sLogDesc += "Remark: " + model.RecordRemark + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.MAIN_RECORD, WebSecurity.CurrentUserName);
                    #endregion

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_REG_RECORD;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_REG_RECORD;
                }
            }

            LoadRecordModelDdl(model);
            return View(model);
        }

        public JsonResult GetPartByRackIdPartId(int iRackId, int iPartId)
        {
            PartRackModels model = DALPartRack.GetPartByRackIdPartId(iRackId, iPartId);

            string sFilePath;
            string sUploadPath = ConfigurationManager.AppSettings["UploadPath"];
            RecordInOutModels RecordInOutModel = new RecordInOutModels();

            // Get existing part image
            if (model.PartGUIDFileName == null)
            {
                RecordInOutModel.RackId = iRackId;
                RecordInOutModel.PartId = iPartId;
                RecordInOutModel.PartCode = "";
                RecordInOutModel.PartQty = 0;

                // Check if part image exists in table part
                PartModels partModel = DALPart.GetPartById(iPartId);

                if(partModel.PartGUIDFileName != "")
                {
                    sFilePath = sUploadPath + partModel.PartGUIDFileName;
                }
                else
                {
                    sFilePath = sUploadPath + "no_image.png";
                }

                RecordInOutModel.PartImage = sFilePath;
            }
            else if (model.PartGUIDFileName != "")
            {
                sFilePath = sUploadPath + model.PartGUIDFileName;

                RecordInOutModel.RackId = iRackId;
                RecordInOutModel.PartId = model.PartId;
                RecordInOutModel.PartCode = model.PartCode;
                RecordInOutModel.PartQty = model.PartQty;
                RecordInOutModel.PartImage = sFilePath;
            }
            else  // Check if part image is empty
            {
                sFilePath = sUploadPath + "no_image.png";

                RecordInOutModel.RackId = iRackId;
                RecordInOutModel.PartId = model.PartId;
                RecordInOutModel.PartCode = model.PartCode;
                RecordInOutModel.PartQty = model.PartQty;
                RecordInOutModel.PartImage = sFilePath;
            }

            return Json(RecordInOutModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecordTransfer(string sRackCode, int iRecordStatus)
        {
            ViewBag.Current = "Record";

            RecordTransferModels model = new RecordTransferModels();

            // Get rack id
            DALRack.GetRackId(out int iRackId, sRackCode);
            // Check if id is invalid
            if (iRackId < 1)
            {
                ViewBag.Message = CommonMsg.INV_RACK;
                return View(model);
            }

            model.RackId = iRackId;
            model.RecordTypeModel.RackCode = sRackCode;
            model.RecordTypeModel.RecordStatus = iRecordStatus;
            TempData["UploadPath"] = ConfigurationManager.AppSettings["UploadPath"];
            model.PartImage = ConfigurationManager.AppSettings["UploadPath"] + "no_image.png";

            LoadRecordTransferModelDdl(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecordTransfer(RecordTransferModels model)
        {
            ViewBag.Current = "Record";

            // Trim property
            model.RecordRemark = model.RecordRemark.Trim();

            // Set initial view value
            model.RackCode = model.RecordTypeModel.RackCode;
            model.RecordDateTime = DateTime.UtcNow;
            model.RecordBy = WebSecurity.CurrentUserName;
            model.RecordStatus = model.RecordTypeModel.RecordStatus;

            if (ModelState.IsValid)
            {
                // Get part code
                PartModels partModel = DALPart.GetPartById(model.PartId);
                // Check if id is invalid
                if (partModel.PartId < 1)
                {
                    ViewBag.Message = CommonMsg.INVALID_ID;
                    LoadRecordTransferModelDdl(model);
                    return View(model);
                }

                model.PartCode = partModel.PartCode;

                // Check if record qty is 0
                if (model.RecordQty == 0)
                {
                    ViewBag.Message = CommonMsg.INV_RECORD_QTY;
                    LoadRecordTransferModelDdl(model);
                    return View(model);
                }

                int iNewRackId = Convert.ToInt32(model.NewRackCode);
                RackModels RackModel = DALRack.GetRackBuildingById(iNewRackId);
                model.NewRackId = RackModel.RackId;

                // Check current rack code and new rack code is same
                if (model.RackCode == RackModel.RackCode)
                {
                    ViewBag.Message = CommonMsg.INV_RACK_TRANSFER;
                    LoadRecordTransferModelDdl(model);
                    return View(model);
                }

                // Check if part rack exist
                DALPartRack.ChkExistPartRack(out bool bExist, model.RackId, model.PartId);
                if (bExist)
                {
                    model.IsPartRackExist = true;
                }

                // Check insert qty is more than balance qty
                if (model.RecordQty > model.PartQty)
                {
                    ViewBag.Message = CommonMsg.NEGATIVE_UPD_TRANSFER;
                    LoadRecordTransferModelDdl(model);
                    return View(model);
                }

                // Check insert qty is same as balance qty
                if (model.RecordQty == model.PartQty)
                {
                    model.RecordTransfer = true;

                    // Check if new rack exist
                    DALPartRack.ChkExistPartRack(out bExist, model.NewRackId, model.PartId);

                    // Full transfer
                    if (bExist)
                    {
                        model.RecordTransferFullExists = true;

                        // Get part qty for new rack
                        DALPartRack.GetPartQtyByRackIdPartId(out int iPartQty, model.NewRackId, model.PartId);
                        // Get ttl amount of transfer qty for new rack
                        iPartQty += model.RecordQty;
                        model.PartQty = iPartQty;
                    }
                }
                else
                {
                    // Partial transfer
                    model.RecordTransfer = false;

                    // Check if new rack exist
                    DALPartRack.ChkExistPartRack(out bExist, model.NewRackId, model.PartId);
                    if (bExist)
                    {
                        model.RecordTransferPartiallyExists = true;

                        // Get part qty for old rack
                        DALPartRack.GetPartQtyByRackIdPartId(out int iPartQty, model.RackId, model.PartId);
                        // Get balance qty for old rack
                        iPartQty -= model.RecordQty;
                        model.OldPartQty = iPartQty;

                        // Get part qty for new rack
                        DALPartRack.GetPartQtyByRackIdPartId(out iPartQty, model.NewRackId, model.PartId);
                        // Get ttl amount of transfer qty for new rack
                        iPartQty += model.RecordQty;
                        model.TtlPartQty = iPartQty;
                    }
                    else
                    {
                        model.RecordTransferPartiallyExists = false;

                        // Get part qty for old rack
                        DALPartRack.GetPartQtyByRackIdPartId(out int iPartQty, model.RackId, model.PartId);
                        // Set balance qty for old rack
                        iPartQty -= model.RecordQty;
                        model.OldPartQty = iPartQty;

                        // Set amount of transfer qty for new rack
                        model.PartQty = model.RecordQty;
                    }
                }

                if (DALRecord.SetRecordTransfer(model))
                {
                    #region Log
                    string sLogDesc = "Add [";
                    sLogDesc += "Part Code: " + model.PartCode + " | ";
                    sLogDesc += "Old Rack Code: " + model.RackCode + " | ";
                    sLogDesc += "New Rack Code: " + RackModel.RackCode + " | ";
                    sLogDesc += "Date/Time: " + model.RecordDateTime + " | ";
                    sLogDesc += "Quantity: " + model.RecordQty + " | ";
                    sLogDesc += "Record Status: " + model.RecordStatus + " | ";
                    sLogDesc += "Record By: " + model.RecordBy + " | ";
                    sLogDesc += "Remark: " + model.RecordRemark + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.MAIN_RECORD, WebSecurity.CurrentUserName);
                    #endregion

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_TRANSFER;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_TRANSFER;
                }
            }

            LoadRecordTransferModelDdl(model);
            return View(model);
        }

        /// <summary>
        /// Load lists for dropdown list
        /// </summary>
        /// <param name="model"></param>
        [NonAction]
        private void LoadRecordTypeModelDdl(RecordTypeModels model)
        {
            // Get list
            var rackList = DALRack.GetRackList();
            // Set combobox text display 
            var rackData = rackList.Select(m => new SelectListItem()
            {
                Text = "[" + m.RackCode + "] - " + m.RackName,
                Value = m.RackCode,
            });
            // Add to model list
            model.RackList = new SelectList(rackData, "Value", "Text");
        }

        /// <summary>
        /// Load lists for dropdown list
        /// </summary>
        /// <param name="model"></param>
        [NonAction]
        private void LoadRecordModelDdl(RecordInOutModels model)
        {
            //model.PartRackList = new SelectList(DALPart.GetPartList(), "PartId", "PartCode", model.PartId);

            if (model.RecordTypeModel.RecordStatus == (int)EnumEx.RecordStatus.STATUS_IN)
            {
                // Get list
                var partRackList = DALPart.GetPartList();

                // Set combobox text display 
                var partRackData = partRackList.Select(m => new SelectListItem()
                {
                    Text = "[" + m.PartCode + "] - " + m.PartName,
                    Value = m.PartId.ToString(),
                });
                // Add to model list
                model.PartRackList = new SelectList(partRackData, "Value", "Text");
            }
            else
            {
                // Get list based on rack selection
                var partRackList = DALPartRack.GetPartRackByRackId(model.RackId);
                // Set combobox text display 
                var partRackData = partRackList.Select(m => new SelectListItem()
                {
                    Text = "[" + m.PartCode + "] - " + m.PartName,
                    Value = m.PartId.ToString(),
                });
                // Add to model list
                model.PartRackList = new SelectList(partRackData, "Value", "Text");
            }
        }

        /// <summary>
        /// Load lists for dropdown list
        /// </summary>
        /// <param name="model"></param>
        [NonAction]
        private void LoadRecordTransferModelDdl(RecordTransferModels model)
        {
            // Get rack list
            var rackList = DALRack.GetRackList();

            // Set combobox text display 
            var rackData = rackList.Select(m => new SelectListItem()
            {
                Text = "[" + m.RackCode + "] - " + m.RackName,
                Value = m.RackId.ToString(),
            });
            // Add to model list
            model.RackList = new SelectList(rackData, "Value", "Text");

            // Get list based on rack selection
            var partRackList = DALPartRack.GetPartRackByRackId(model.RackId);
            // Set combobox text display 
            var partRackData = partRackList.Select(m => new SelectListItem()
            {
                Text = "[" + m.PartCode + "] - " + m.PartName,
                Value = m.PartId.ToString(),
            });
            // Add to model list
            model.PartRackList = new SelectList(partRackData, "Value", "Text");
        }

        /// <summary>
        /// Get template file
        /// </summary>
        public FileResult DownloadTemplate()
        {
            string filename = "Import_Batch_File_Template.csv";
            string sWebRootPath = ConfigurationManager.AppSettings["UploadPathCSV"];
            string documentationPath = Server.MapPath(sWebRootPath);
            var filePath = Path.GetFullPath(Path.Combine(documentationPath, filename));

            return File(filePath, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        /// <summary>
        /// View import function
        /// </summary>
        /// <returns></returns>
        public ActionResult Import()
        {
            ViewBag.Current = "Record";

            var part = new RecordImportModels();
            return View(part);
        }

        /// <summary>
        /// Import record function
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(RecordImportModels model)
        {
            ViewBag.Current = "Record";

            if (ModelState.IsValid)
            {
                string sFullUploadPath = string.Empty;
                DataTable dt = new DataTable();
                List<ImportErrorModel> errorModels = new List<ImportErrorModel>();

                // Check if attached file is not empty
                if (Request.Files.Count > 0)
                {
                    var files = Request.Files[0];

                    if (files != null && files.ContentLength > 0)
                    {
                        try
                        {
                            #region Get Uploaded File Path
                            string sfileName = Path.GetFileName(files.FileName);
                            string sGetDir = Server.MapPath(ConfigurationManager.AppSettings["UploadPathCSV"]);

                            Common.ChkExistDirectory(sGetDir);
                            sFullUploadPath = Path.Combine(sGetDir, sfileName);
                            #endregion

                            files.SaveAs(sFullUploadPath);                      // Saved to Uploads folder
                            dt = Common.ReadCSVToDataTable(sFullUploadPath);    // Populate CSV date into DataTable 

                            #region Check max column in header data
                            int iMaxCol = 8;

                            // Check if data column is same
                            if (dt.Columns.Count != iMaxCol)
                            {
                                Common.DelUploadedFile(sFullUploadPath);
                                ViewBag.Message = CommonMsg.INV_COL_RECORD_FILE;
                                return View(model);
                            }
                            #endregion

                            int iSqlErrCount = 0;
                            int iImportedRows = 0;

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                #region Default Value
                                model = new RecordImportModels
                                {
                                    BuildingModel = new BuildingModels
                                    {
                                        // Set default value
                                        BuildingId = 0,
                                        BuildingName = "",
                                    },

                                    RackModel = new RackModels
                                    {
                                        // Set default value
                                        RackId = 0,
                                        RackName = "",
                                        RackCode = "",
                                        BuildingId = 0,
                                        BuildingName = "",
                                    },

                                    PartModel = new PartModels
                                    {
                                        // Set default value
                                        PartId = 0,
                                        PartCode = "",
                                        PartName = "",
                                        PartDesc = "",
                                        PartFileName = "",
                                        PartGUIDFileName = "",
                                        PartMinQty = 0,
                                    },

                                    RecordInOutModel = new RecordInOutModels
                                    {
                                        // Set default value
                                        RecordDateTime = DateTime.UtcNow,
                                        RecordQty = 0,
                                        RecordStatus = (int)EnumEx.RecordStatus.STATUS_IN,
                                        RecordBy = WebSecurity.CurrentUserName,
                                        RecordRemark = "",
                                        RackId = 0,
                                        RackCode = "",
                                        PartId = 0,
                                        PartCode = "",
                                        PartQty = 0,
                                        PartImage = "",
                                        PartGUIDFileName = "",
                                        IsPartRackExist = false,
                                    },

                                    ImportErrorModel = new ImportErrorModel
                                    {
                                        Error = "",
                                        ErrorRow = 0,
                                    },

                                    ImportErrorListModel = errorModels,
                                };
                                #endregion

                                #region Populate CSV data into Model

                                model.InputRow = i + 2;
                                model.BuildingModel.BuildingName = StringToCSVCell(dt.Rows[i][0].ToString().Trim());
                                model.RackModel.RackName = StringToCSVCell(dt.Rows[i][1].ToString().Trim());
                                model.RackModel.RackCode = StringToCSVCell(dt.Rows[i][2].ToString().Trim());
                                model.PartModel.PartCode = StringToCSVCell(dt.Rows[i][3].ToString().Trim());
                                model.PartModel.PartName = StringToCSVCell(dt.Rows[i][4].ToString().Trim());
                                model.PartModel.PartDesc = StringToCSVCell(dt.Rows[i][5].ToString().Trim());
                                try { model.PartModel.PartMinQty = int.Parse((string)dt.Rows[i][6]); } catch { }
                                try { model.RecordInOutModel.PartQty = int.Parse((string)dt.Rows[i][7]); } catch { }
                                #endregion

                                #region Validation
                                bool bValidCSV = true;

                                bValidCSV = ValidateCSV(model);

                                if (bValidCSV == false)
                                {
                                    errorModels.Add(new ImportErrorModel
                                    {
                                        ErrorRow = model.ImportErrorModel.ErrorRow,
                                        Error = model.ImportErrorModel.Error,
                                    });
                                    model.ImportErrorListModel = errorModels;
                                    continue;
                                }
                                #endregion

                                #region Register Batch
                                // Register collected data by batch
                                if (DALRecord.SetRecordByBatch(model))
                                {
                                    #region Log
                                    string sLogDesc = "Add batch [";
                                    sLogDesc += "Building Name: " + model.BuildingModel.BuildingName + " | ";
                                    sLogDesc += "Rack Name: " + model.RackModel.RackName + " | ";
                                    sLogDesc += "Rack Code: " + model.RackModel.RackCode + " | ";
                                    sLogDesc += "Part Name: " + model.PartModel.PartName + " | ";
                                    sLogDesc += "Part Code: " + model.RecordInOutModel.PartCode + " | ";
                                    sLogDesc += "Part Desc: " + model.PartModel.PartDesc + " | ";
                                    sLogDesc += "Part Min Qty: " + model.PartModel.PartMinQty + " | ";
                                    sLogDesc += "Date/Time: " + model.RecordInOutModel.RecordDateTime + " | ";
                                    sLogDesc += "Balance Quantity: " + model.RecordInOutModel.PartQty + " | ";
                                    sLogDesc += "Record Status: " + model.RecordInOutModel.RecordStatus + " | ";
                                    sLogDesc += "Record By: " + model.RecordInOutModel.RecordBy + "] ";
                                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.BATCH_UPLOAD, WebSecurity.CurrentUserName);
                                    #endregion
                                }
                                else
                                {
                                    iSqlErrCount++;
                                }

                                iImportedRows++;
                                #endregion
                            }

                            #region Display Imported Status
                            bool bInvalidRow = false;

                            if (errorModels.Count != 0 || iSqlErrCount > 0)
                            {
                                bInvalidRow = true;
                            }

                            string sViewBagMsg = "Total rows to import: " + dt.Rows.Count + ". ";
                            sViewBagMsg += "Total row(s) imported: " + iImportedRows + ". ";

                            if (!bInvalidRow)
                            {
                                ViewBag.Status = true;

                                ViewBag.Message = CommonMsg.SUCC_IMPORT_RECORD;
                                ViewBag.MessageDetails = sViewBagMsg;
                            }
                            else
                            {
                                ViewBag.Status = false;

                                ViewBag.Message = CommonMsg.FAIL_IMPORT_RECORD;
                                sViewBagMsg += "Total row(s) with invalid data: " + errorModels.Count + ". ";
                                sViewBagMsg += "SQL error count: " + iSqlErrCount + ". ";
                                ViewBag.MessageDetails = sViewBagMsg;
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = ex.Message;
                        }
                    }
                    else
                    {
                        ViewBag.Message = CommonMsg.NOT_FOUND_RECORD_FILE;
                    }
                }

                Common.DelUploadedFile(sFullUploadPath);
                dt.Dispose();
            }

            return View(model);
        }

        /// <summary>
        /// Validate CSV input
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [NonAction]
        private static bool ValidateCSV(RecordImportModels model)
        {
            // BuildingName: Check if empty
            if (model.BuildingModel.BuildingName == "")
            {
                model.ImportErrorModel.ErrorRow = model.InputRow;
                model.ImportErrorModel.Error = CommonMsg.EMPTY_BUILDING_NAME;
                return false;
            }

            // RackName: Check if empty
            if (model.RackModel.RackName == "")
            {
                model.ImportErrorModel.ErrorRow = model.InputRow;
                model.ImportErrorModel.Error = CommonMsg.EMPTY_RACK_NAME;
                return false;
            }

            // RackCode: Check if empty
            if (model.RackModel.RackCode == "")
            {
                model.ImportErrorModel.ErrorRow = model.InputRow;
                model.ImportErrorModel.Error = CommonMsg.EMPTY_RACK_CODE;
                return false;
            }

            // PartCode: Check if empty
            if (model.PartModel.PartCode == "")
            {
                model.ImportErrorModel.ErrorRow = model.InputRow;
                model.ImportErrorModel.Error = CommonMsg.EMPTY_PART_CODE;
                return false;
            }

            // PartName: Check if empty
            if (model.PartModel.PartName == "")
            {
                model.ImportErrorModel.ErrorRow = model.InputRow;
                model.ImportErrorModel.Error = CommonMsg.EMPTY_PART_NAME;
                return false;
            }

            // PartMinQty: Check if below 0
            if (model.PartModel.PartMinQty < 0)
            {
                model.ImportErrorModel.ErrorRow = model.InputRow;
                model.ImportErrorModel.Error = CommonMsg.INV_PART_QTY;
                return false;
            }

            // PartQty: Check if below 0
            if (model.RecordInOutModel.PartQty < 0)
            {
                model.ImportErrorModel.ErrorRow = model.InputRow;
                model.ImportErrorModel.Error = CommonMsg.INV_PART_QTY;
                return false;
            }

            // BuildingName: Check exists
            DALBuilding.ChkExistBuilding(out bool bExist, model.BuildingModel.BuildingName);
            if (bExist)
            {
                DALBuilding.GetBuildingIdByName(out int iBuildingId, model.BuildingModel.BuildingName);
                model.BuildingModel.BuildingId = iBuildingId;
            }

            // RackName and RackCode: Check exists
            DALRack.ChkExistRackNameCode(out bExist, model.RackModel.RackName, model.RackModel.RackCode);
            if (bExist)
            {
                DALRack.GetRackId(out int iRackId, model.RackModel.RackCode);
                model.RackModel.RackId = iRackId;
            }

            // RackName and RackCode: Check either one is exists
            DALRack.ChkExistRackName(out bool bRackNameExist, model.RackModel.RackName);
            DALRack.ChkExistRackCode(out bool bRackCodeExist, model.RackModel.RackCode);
            if ((!bRackNameExist && bRackCodeExist) || (bRackNameExist && !bRackCodeExist))
            {
                model.ImportErrorModel.ErrorRow = model.InputRow;
                model.ImportErrorModel.Error = CommonMsg.DUP_RACK;
                return false;
            }

            // PartCode: Check exists
            DALPart.ChkExistPartCode(out bExist, model.PartModel.PartCode);
            if (bExist)
            {
                DALPart.GetPartId(out int iPartId, model.PartModel.PartCode);
                model.PartModel.PartId = iPartId;

                DALPart.GetPartFileNameByPartId(out string sPartFileName, model.PartModel.PartId);
                model.PartModel.PartFileName = sPartFileName;

                DALPart.GetPartGUIDFileNameByPartId(out string sPartGUIDFileName, model.PartModel.PartId);
                model.PartModel.PartGUIDFileName = sPartGUIDFileName;
            }

            model.RecordInOutModel.RecordQty = -1;

            // PartRack: Check exists
            DALPartRack.ChkExistPartRack(out bExist, model.RackModel.RackId, model.PartModel.PartId);
            if (bExist)
            {
                model.RecordInOutModel.IsPartRackExist = true;
                // Get part qty
                DALPartRack.GetPartQtyByRackIdPartId(out int iPartQty, model.RackModel.RackId, model.PartModel.PartId);
                // Only upd part qty if there is no data in database (tbl PartRack)
                model.RecordInOutModel.PartQty = iPartQty;
#if false
                // Check if part qty in excel is more than part qty in database
                // Add additional part qty into database (In record)
                if (model.RecordInOutModel.PartQty > iPartQty)
                {
                    // Set record qty value
                    //model.RecordInOutModel.RecordQty = model.RecordInOutModel.PartQty - iPartQty;
                    model.RecordInOutModel.PartQty = iPartQty;
                }
                // Remove additional part qty from database (Out record)
                else if (model.RecordInOutModel.PartQty < iPartQty)
                {
                    model.RecordInOutModel.PartQty = iPartQty;
                }
                // Same qty both excel and database 
                else
                {
                    model.RecordInOutModel.PartQty = iPartQty;
                }
#endif
            }
            else
            {
                // Set record qty value
                model.RecordInOutModel.RecordQty = model.RecordInOutModel.PartQty;
            }

            return true;
        }

        /// <summary>
        /// Turn a string into a CSV cell output
        /// </summary>
        /// <param name="str">String to output</param>
        /// <returns>The CSV cell formatted string</returns>
        [NonAction]
        public static string StringToCSVCell(string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                str = str.Replace("\"", "");
                return string.Format("{0}", str);
            }

            return str;
        }
    }
}