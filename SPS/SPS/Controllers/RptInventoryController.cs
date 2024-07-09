/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RptInventoryController.cs                                                                            */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      01/06/2022  Add all rack and all building report func                       Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.DAL;
using SPS.Filters;
using SPS.Models;
using SPSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SPS.Controllers
{
    public class RptInventoryController : Controller
    {
        // GET: RptInventory
        [Authorize(Roles = "ADMIN, USER")]
        [CheckSession]
        public ActionResult Index()
        {
            ViewBag.Report = true;
            ViewBag.Current = "RptInventory";

            RptInventoryModels model = new RptInventoryModels
            {
                AllBuilding = true,
                AllRack = true,
                AllPart = true
            };

            // Load drop down list
            LoadRptInventoryModelDdl(model);
            return View(model);
        }

        /// <summary>
        /// Get rack by building id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetRackByBuildingId(int iBuildingId)
        {
            //IList<RackModels> modelList = DALRack.GetRackByBuildingId(iBuildingId);

            var modelList = DALRack.GetRackByBuildingId(iBuildingId);

            var modelData = modelList.Select(m => new SelectListItem()
            {
                Text = "[" + m.RackCode + "] - " + m.RackName,
                Value = m.RackId.ToString(),
            });

            return Json(modelData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get part by rack id
        /// </summary>
        /// <param name="iRackId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPartByRackId(int iRackId)
        {
            //IList<PartRackModels> modelList = DALPartRack.GetPartRackByRackId(iRackId);

            var modelList = DALPartRack.GetPartRackByRackId(iRackId);

            var modelData = modelList.Select(m => new SelectListItem()
            {
                Text = "[" + m.PartCode + "] - " + m.PartName,
                Value = m.PartId.ToString(),
            });

            return Json(modelData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all inventory list
        /// </summary>
        /// <returns></returns>
        public JsonResult GetInventoryList()
        {
            IList<RptInventoryModels> modelList = DALPartRack.GetInventoryList();

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all inventory by building id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <returns></returns>
        public JsonResult GetInventoryByBuildingId(int iBuildingId)
        {
            IList<RptInventoryModels> modelList = DALPartRack.GetInventoryByBuildingId(iBuildingId);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all inventory by building id, rack id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <returns></returns>
        public JsonResult GetInventoryByBuildingIdRackId(int iBuildingId, int iRackId)
        {
            IList<RptInventoryModels> modelList = DALPartRack.GetInventoryByBuildingIdRackId(iBuildingId, iRackId);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all inventory by building id, rack id, part id
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public JsonResult GetInventoryByBuildingIdRackIdPartId(int iBuildingId, int iRackId, int iPartId)
        {
            IList<RptInventoryModels> modelList = DALPartRack.GetInventoryByBuildingIdRackIdPartId(iBuildingId, iRackId, iPartId);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all inventory by building id, rack id, part id
        /// </summary>
        /// <param name="iPartId"></param>
        /// <returns></returns>
        public JsonResult GetInventoryByPartId(int iPartId)
        {
            IList<RptInventoryModels> modelList = DALPartRack.GetInventoryByPartId(iPartId);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Load lists for dropdown list
        /// </summary>
        /// <param name="model"></param>
        [NonAction]
        private void LoadRptInventoryModelDdl(RptInventoryModels model)
        {
            model.BuildingList = new SelectList(DALBuilding.GetBuildingList(), "BuildingId", "BuildingName", model.BuildingId);
            model.RackList = new SelectList("", "RackId", "RackCode", model.RackId);
            model.PartList = new SelectList("", "PartId", "PartCode", model.PartId);
            //model.PartSearchList = new SelectList(DALPart.GetPartList(), "PartId", "PartCode", model.PartId);

            // Get list
            var modelList = DALPart.GetPartList();
            // Set combobox text display 
            var modelData = modelList.Select(m => new SelectListItem()
            {
                Text = "[" + m.PartCode + "] - " + m.PartName,
                Value = m.PartId.ToString(),
            });
            // Add to model list
            model.PartSearchList = new SelectList(modelData, "Value", "Text");
        }
    }
}