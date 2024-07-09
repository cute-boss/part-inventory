/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* BuildingController.cs                                                                                */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add GetRackByBuildingId                                         Azmir               */
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
    public class BuildingController : Controller
    {
        // GET: Building
        public ActionResult Index()
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Building";

            IList<BuildingModels> modelList = DALBuilding.GetBuildingList();
            return View(modelList);
        }

        public ActionResult Detail(int id)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Building";

            BuildingModels model = DALBuilding.GetBuildingById(id);

            // Check if id is invalid
            if (model.BuildingId == 0)
            {
                TempData["ErrMsg"] = CommonMsg.INVALID_ID;
                return RedirectToAction("Index", "Building");
            }

            return View(model);
        }

        public ActionResult Add()
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Building";

            var model = new BuildingModels();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(BuildingModels model)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Building";

            // Trim property
            model.BuildingName = model.BuildingName.Trim();

            if (ModelState.IsValid)
            {
                // Check if building is exist
                DALBuilding.ChkExistBuilding(out bool bExist, model.BuildingName);
                if (bExist)
                {
                    ViewBag.Message = CommonMsg.DUP_BUILDING;
                    return View(model);
                }

                if (DALBuilding.SetBuilding(model))
                {
                    string sLogDesc = "Add [";
                    sLogDesc += "Building name: " + model.BuildingName + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_BUILDING, WebSecurity.CurrentUserName);

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_REG_BUILDING;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_REG_BUILDING;
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Building";

            BuildingModels model = DALBuilding.GetBuildingById(id);

            // Check if id is invalid
            if (model.BuildingId == 0)
            {
                TempData["ErrMsg"] = CommonMsg.INVALID_ID;
                return RedirectToAction("Index", "Building");
            }

            model.OldBuildingName = model.BuildingName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BuildingModels model)
        {
            ViewBag.Maintenance = true;
            ViewBag.Current = "Building";

            // Trim property
            model.BuildingName = model.BuildingName.Trim();

            if (ModelState.IsValid)
            {
                // Check if building is exist
                if (model.BuildingName != model.OldBuildingName)
                {
                    DALBuilding.ChkExistBuilding(out bool bExist, model.BuildingName);
                    if (bExist)
                    {
                        ViewBag.Message = CommonMsg.DUP_BUILDING;
                        return View(model);
                    }
                }

                if (DALBuilding.UpdBuilding(model))
                {
                    string sLogDesc = "Edit [";
                    sLogDesc += "Building name: " + model.BuildingName + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_BUILDING, WebSecurity.CurrentUserName);

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_UPD_BUILDING;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_UPD_BUILDING;
                }
            }

            return View(model);
        }

        [HttpPost]
        public string Delete(int id)
        {
            BuildingModels model = DALBuilding.GetBuildingById(id);

            if (DALBuilding.DelBuilding(id))
            {
                string sLogDesc = "Delete [";
                sLogDesc += "Building name: " + model.BuildingName + "] ";
                DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_BUILDING, WebSecurity.CurrentUserName);

                return "1"; ;
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// Get rack by building id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <returns></returns>
        public JsonResult GetRackByBuildingId(int iBuildingId)
        {
            IList<RackModels> modelList = DALRack.GetRackByBuildingId(iBuildingId);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }
    }
}