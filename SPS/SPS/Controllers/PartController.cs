/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* PartController.cs                                                                                    */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Remove import func & add GetRecordByPartId                      Azmir               */
/*      25/07/2022  Add GetPartNBalQtyList &  GetPartNBalQtyById                    Azmir               */
/*      14/02/2023  Check if part min qty is less than 0                            Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.DAL;
using SPS.Filters;
using SPS.Models;
using SPSLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace SPS.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [CheckSession]
    public class PartController : Controller
    {
        // GET: Part
        public ActionResult Index()
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Part";

            IList<PartModels> modelList = DALPart.GetPartNBalQtyList();
            TempData["UploadPath"] = ConfigurationManager.AppSettings["UploadPath"];
            return View(modelList);
        }

        public ActionResult Detail(int id)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Part";

            PartModels model = DALPart.GetPartNBalQtyById(id);

            // Check if id is invalid
            if (model.PartId == 0)
            {
                TempData["ErrMsg"] = CommonMsg.INVALID_ID;
                return RedirectToAction("Index", "Part");
            }

            string sUploadPath = ConfigurationManager.AppSettings["UploadPath"];
            TempData["UploadPath"] = sUploadPath + model.PartGUIDFileName;
            return View(model);
        }

        public ActionResult Add()
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Part";

            var part = new PartModels();
            return View(part);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PartModels model)
        {
            ViewBag.Current = "Part";

            // Trim property
            model.PartCode = model.PartCode.Trim();
            model.PartName = model.PartName.Trim();
            model.PartDesc = model.PartDesc.Trim();
            model.PartFileName = "";
            model.PartGUIDFileName = "";
            string sUploadPath = ConfigurationManager.AppSettings["UploadPath"];

            if (ModelState.IsValid)
            {
                // Check if part code is exist
                DALPart.ChkExistPartCode(out bool bExist, model.PartCode);
                if (bExist)
                {
                    ViewBag.Message = CommonMsg.DUP_PART;
                    return View(model);
                }

                // Check if part min qty is less than 0
                if (model.PartMinQty < 0)
                {
                    ViewBag.Message = CommonMsg.INV_PART_MIN_QTY;
                    return View(model);
                }

                #region Check uploaded file
                // Get Uploaded files
                if (Request.Files.Count > 0)
                {
                    var files = Request.Files[0];

                    if (files != null && files.ContentLength > 0)
                    {
                        try
                        {
                            var GetDir = Server.MapPath(sUploadPath);
                            Common.ChkExistDirectory(GetDir);

                            // Get misc settings
                            Misc misc = new Misc();
                            DALMisc.GetMisc(out misc);

                            int iMaxKBSize = misc.AttachmentSize * 1024 * 1024;
                            var fileName = Path.GetFileName(files.FileName);
                            string fileExt = Path.GetExtension(fileName);

                            // Check file format
                            if (fileExt == ".jpg" || fileExt == ".png" || fileExt == ".jpeg")
                            {
                                // Check max file size
                                if (files.ContentLength > iMaxKBSize)
                                {
                                    ViewBag.Message = CommonMsg.MAX_PART_FILE_SIZE + (misc.AttachmentSize).ToString() + "MB";
                                    return View(model);
                                }

                                string sNewFileName = model.Guid + fileExt;
                                var newPath = Path.Combine(GetDir, sNewFileName);

                                files.SaveAs(newPath);  // Copy file to newpath

                                model.PartFileName = fileName;
                                model.PartGUIDFileName = sNewFileName;
                            }
                            else
                            {
                                ViewBag.Message = CommonMsg.INV_PART_FILE_FORMAT;
                                return View(model);
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Error: " + ex.Message.ToString();
                            return View(model);
                        }
                    }
                }
                #endregion

                if (DALPart.SetPart(model))
                {
                    string sLogDesc = "Add [";
                    sLogDesc += "Part Code: " + model.PartCode + " | ";
                    sLogDesc += "Part Name: " + model.PartName + " | ";
                    sLogDesc += "Part Desc: " + model.PartDesc + " | ";
                    sLogDesc += "Part Min Qty: " + model.PartDesc + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_PART, WebSecurity.CurrentUserName);

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_REG_PART;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_REG_PART;
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Part";

            PartModels model = DALPart.GetPartById(id);

            // Check if id is invalid
            if (model.PartId == 0)
            {
                TempData["ErrMsg"] = CommonMsg.INVALID_ID;
                return RedirectToAction("Index", "Part");
            }

            string sUploadPath = ConfigurationManager.AppSettings["UploadPath"];
            TempData["UploadPath"] = sUploadPath + model.PartGUIDFileName;

            model.OldPartCode = model.PartCode;
            model.OldPartFileName = model.PartFileName;
            model.OldPartGUIDFileName = model.PartGUIDFileName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PartModels model)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Part";

            // Trim property
            model.PartName = model.PartName.Trim();
            string sUploadPath = ConfigurationManager.AppSettings["UploadPath"];

            if (ModelState.IsValid)
            {
                // Check if part code is exist
                if (model.PartCode != model.OldPartCode)
                {
                    DALPart.ChkExistPartCode(out bool bExist, model.PartCode);
                    if (bExist)
                    {
                        ViewBag.Message = CommonMsg.DUP_PART;
                        TempData["UploadPath"] = sUploadPath + model.OldPartGUIDFileName;
                        return View(model);
                    }
                }

                // Check if part min qty is less than 0
                if (model.PartMinQty < 0)
                {
                    ViewBag.Message = CommonMsg.INV_PART_MIN_QTY;
                    return View(model);
                }

                #region Check uploaded file
                // Validate if upload new file
                if (model.PartFileName != null)
                {
                    model.PartFileName = "";
                    model.PartGUIDFileName = "";

                    string GetDir = Server.MapPath(sUploadPath);
                    Common.ChkExistDirectory(GetDir);

                    // Get uploaded file
                    if (Request.Files.Count > 0)
                    {
                        var files = Request.Files[0];

                        if (files != null && files.ContentLength > 0)
                        {
                            try
                            {
                                // Get misc settings
                                Misc misc = new Misc();
                                DALMisc.GetMisc(out misc);

                                int iMaxKBSize = misc.AttachmentSize * 1024 * 1024;
                                var fileName = Path.GetFileName(files.FileName);
                                string fileExt = Path.GetExtension(fileName);

                                // Check file format
                                if (fileExt == ".jpg" || fileExt == ".png" || fileExt == ".jpeg")
                                {
                                    // Check max file size
                                    if (files.ContentLength > iMaxKBSize)
                                    {
                                        ViewBag.Message = CommonMsg.MAX_PART_FILE_SIZE + (misc.AttachmentSize).ToString() + "MB";
                                        TempData["UploadPath"] = sUploadPath + model.OldPartGUIDFileName;
                                        return View(model);
                                    }

                                    string sNewFileName = model.Guid + fileExt;
                                    var newPath = Path.Combine(GetDir, sNewFileName);

                                    files.SaveAs(newPath);  // Copy file to newpath

                                    model.PartFileName = fileName;
                                    model.PartGUIDFileName = sNewFileName;
                                }
                                else
                                {
                                    ViewBag.Message = CommonMsg.INV_PART_FILE_FORMAT;
                                    TempData["UploadPath"] = sUploadPath + model.OldPartGUIDFileName;
                                    return View(model);
                                }
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = "Error: " + ex.Message.ToString();
                                TempData["UploadPath"] = sUploadPath + model.OldPartGUIDFileName;
                                return View(model);
                            }
                        }
                    }

                    try
                    {
                        string sFullFilePath = GetDir + "/" + model.OldPartGUIDFileName;
                        Common.DelUploadedFile(sFullFilePath);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "Error: " + ex.Message.ToString();
                    }
                }
                else
                {
                    if (model.OldPartFileName == null || model.OldPartGUIDFileName == null)
                    {
                        model.PartFileName = "";
                        model.PartGUIDFileName = "";
                    }
                    else
                    {
                        model.PartFileName = model.OldPartFileName;
                        model.PartGUIDFileName = model.OldPartGUIDFileName;
                    }
                }
                #endregion

                if (DALPart.UpdPart(model))
                {
                    string sLogDesc = "Edit [";
                    sLogDesc += "Part Code: " + model.PartCode + " | ";
                    sLogDesc += "Part Name: " + model.PartName + " | ";
                    sLogDesc += "Part Desc: " + model.PartDesc + " | ";
                    sLogDesc += "Part Min Qty: " + model.PartDesc + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_PART, WebSecurity.CurrentUserName);

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_UPD_PART;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_UPD_PART;
                }
            }

            TempData["UploadPath"] = sUploadPath + model.PartGUIDFileName;
            return View(model);
        }

        /// <summary>
        /// Get record by part id
        /// </summary>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public JsonResult GetRecordByPartId(int iPartId)
        {
            IList<RecordInOutModels> modelList = DALRecord.GetRecordByPartId(iPartId);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string Delete(int id)
        {
            PartModels model = DALPart.GetPartById(id);

            string sUploadPath = ConfigurationManager.AppSettings["UploadPath"];
            string sUploadFullPath = Server.MapPath(sUploadPath);
            string sFullFilePath = sUploadFullPath + "/" + model.PartGUIDFileName;

            try
            {
                Common.DelUploadedFile(sFullFilePath);

                if (DALPart.DelPart(id))
                {
                    string sLogDesc = "Delete [";
                    sLogDesc += "Part Code: " + model.PartCode + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_PART, WebSecurity.CurrentUserName);

                    return "1"; ;
                }
                else
                {
                    return "0";
                }
            }
            catch
            {
                return "0";
            }
        }
    }
}