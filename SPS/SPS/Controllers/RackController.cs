/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RackController.cs                                                                                    */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add GetRecordByRackId                                           Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.DAL;
using SPS.Filters;
using SPS.Models;
using SPSLib;
using System.Collections.Generic;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace SPS.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [CheckSession]
    public class RackController : Controller
    {
        // GET: Rack
        public ActionResult Index()
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Rack";

            IList<RackModels> modelList = DALRack.GetRackList();
            return View(modelList);
        }

        public ActionResult Detail(int id)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Rack";

            RackModels model = DALRack.GetRackBuildingById(id);

            // Check if id is invalid
            if (model.RackId == 0)
            {
                TempData["ErrMsg"] = CommonMsg.INVALID_ID;
                return RedirectToAction("Index", "Rack");
            }

            return View(model);
        }

        public ActionResult Add()
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Rack";

            RackModels model = new RackModels();
            LoadRackModelDdl(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(RackModels model)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Rack";

            LoadRackModelDdl(model);

            // Trim property
            model.RackName = model.RackName.Trim();
            model.RackCode = model.RackCode.Trim();

            if (ModelState.IsValid)
            {
                // Check if rack is exist
                DALRack.ChkExistRack(out bool bExist, model.RackName, model.RackCode, model.BuildingId);
                if (bExist)
                {
                    ViewBag.Message = CommonMsg.DUP_RACK;
                    return View(model);
                }

                if (DALRack.SetRack(model))
                {
                    string sLogDesc = "Add [";
                    sLogDesc += "Rack name: " + model.RackName + " | ";
                    sLogDesc += "Rack code: " + model.RackCode + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_RACK, WebSecurity.CurrentUserName);

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_REG_RACK;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_REG_RACK;
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Rack";

            RackModels model = DALRack.GetRackBuildingById(id);

            // Check if id is invalid
            if (model.RackId == 0)
            {
                TempData["ErrMsg"] = CommonMsg.INVALID_ID;
                return RedirectToAction("Index", "Rack");
            }

            model.OldRackName = model.RackName;
            model.OldRackCode = model.RackCode;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RackModels model)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Rack";

            // Trim property
            model.RackName = model.RackName.Trim();
            model.RackCode = model.RackCode.Trim();

            if (ModelState.IsValid)
            {
                // Check if rack is exist
                if (model.RackName != model.OldRackName || model.RackCode != model.OldRackCode)
                {
                    DALRack.ChkExistRack(out bool bExist, model.RackName, model.RackCode, model.BuildingId);
                    if (bExist)
                    {
                        ViewBag.Message = CommonMsg.DUP_RACK;
                        return View(model);
                    }
                }

                if (DALRack.UpdRack(model))
                {
                    string sLogDesc = "Edit [";
                    sLogDesc += "Rack name: " + model.RackName + " | ";
                    sLogDesc += "Rack code: " + model.RackCode + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_RACK, WebSecurity.CurrentUserName);

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_UPD_RACK;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_UPD_RACK;
                }
            }

            return View(model);
        }

        [HttpPost]
        public string Delete(int id)
        {
            RackModels model = DALRack.GetRackBuildingById(id);

            if (DALRack.DelRack(id))
            {
                string sLogDesc = "Delete [";
                sLogDesc += "Rack name: " + model.RackName + "] ";
                DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_RACK, WebSecurity.CurrentUserName);

                return "1"; ;
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// Get record by rack id
        /// </summary>
        /// <param name="iRackId"></param>
        /// <returns></returns>
        public JsonResult GetRecordByRackId(int iRackId)
        {
            IList<RecordInOutModels> modelList = DALRecord.GetRecordByRackId(iRackId);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Load lists for dropdown list
        /// </summary>
        /// <param name="model"></param>
        [NonAction]
        private void LoadRackModelDdl(RackModels model)
        {
            model.BuildingList = new SelectList(DALBuilding.GetBuildingList(), "BuildingId", "BuildingName", model.BuildingId);
        }
    }
}