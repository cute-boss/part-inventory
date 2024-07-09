/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RptInOutController.cs                                                                                */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      01/06/2022  Add all rack and all building report func                       Azmir               */
/*      09/03/2023  Add status for type - transfer                                  Azmir               */
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
    public class RptInOutController : Controller
    {
        // GET: RptInOut
        [Authorize(Roles = "ADMIN, USER")]
        [CheckSession]
        public ActionResult Index()
        {
            ViewBag.Report = true;
            ViewBag.Current = "RptIn/Out";

            RptInOutModels model = new RptInOutModels
            {
                InType = true,
                OutType = true,
                TransferType = true,
                AllBuilding = true,
                AllRack = true,
                AllPart = true
            };

            // Load drop down list
            LoadRptInOutModelDdl(model);
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
        /// Get all record date from, date to, status
        /// </summary>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public JsonResult GetRecordFromDtToDtStatus(string sFromDT, string sToDT, int iStatus, bool bEqualStatus)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordFromDtToDtStatus(dtFrom, dtTo, iStatus, bEqualStatus);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all record by building id, date from, date to, status
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public JsonResult GetRecordByBuildingIdFromDtToDtStatus(int iBuildingId, string sFromDT, string sToDT, int iStatus, bool bEqualStatus)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordByBuildingIdFromDtToDtStatus(iBuildingId, dtFrom, dtTo, iStatus, bEqualStatus);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all record by building id, rack id, date from, date to, status
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public JsonResult GetRecordByBuildingIdRackIdFromDtToDtStatus(int iBuildingId, int iRackId, string sFromDT, string sToDT, int iStatus, bool bEqualStatus)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordByBuildingIdRackIdFromDtToDtStatus(iBuildingId, iRackId, dtFrom, dtTo, iStatus, bEqualStatus);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all record by building id, rack id, part id, sdate from date to, status
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="iPartId"></param>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>.
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public JsonResult GetRecordByBuildingIdRackIdPartIdFromDtToDtStatus(int iBuildingId, int iRackId, int iPartId, string sFromDT, string sToDT, int iStatus, bool bEqualStatus)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordByBuildingIdRackIdPartIdFromDtToDtStatus(iBuildingId, iRackId, iPartId, dtFrom, dtTo, iStatus, bEqualStatus);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all record by building id, rack id, part id, sdate from date to, status
        /// </summary>
        /// <param name="iPartId"></param>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>.
        /// <param name="iStatus"></param>
        /// <returns></returns>
        public JsonResult GetRecordByPartIdFromDtToDtStatus(int iPartId, string sFromDT, string sToDT, int iStatus, bool bEqualStatus)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordByPartIdFromDtToDtStatus(iPartId, dtFrom, dtTo, iStatus, bEqualStatus);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all record date from, date to
        /// </summary>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>
        /// <returns></returns>
        public JsonResult GetRecordFromDtToDt(string sFromDT, string sToDT)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordFromDtToDt(dtFrom, dtTo);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all record by building id, date from, date to, status
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>
        /// <returns></returns>
        public JsonResult GetRecordByBuildingIdFromDtToDt(int iBuildingId, string sFromDT, string sToDT)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordByBuildingIdFromDtToDt(iBuildingId, dtFrom, dtTo);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all record by building id, rack id, date from, date to
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>
        /// <returns></returns>
        public JsonResult GetRecordByBuildingIdRackIdFromDtToDt(int iBuildingId, int iRackId, string sFromDT, string sToDT)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordByBuildingIdRackIdFromDtToDt(iBuildingId, iRackId, dtFrom, dtTo);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all record by building id, rack id, part id, sdate from date to
        /// </summary>
        /// <param name="iBuildingId"></param>
        /// <param name="iRackId"></param>
        /// <param name="iPartId"></param>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>.
        /// <returns></returns>
        public JsonResult GetRecordByBuildingIdRackIdPartIdFromDtToDt(int iBuildingId, int iRackId, int iPartId, string sFromDT, string sToDT)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordByBuildingIdRackIdPartIdFromDtToDt(iBuildingId, iRackId, iPartId, dtFrom, dtTo);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all record by building id, rack id, part id, sdate from date to
        /// </summary>
        /// <param name="iPartId"></param>
        /// <param name="sFromDT"></param>
        /// <param name="sToDT"></param>.
        /// <returns></returns>
        public JsonResult GetRecordByPartIdFromDtToDt(int iPartId, string sFromDT, string sToDT)
        {
            DateTime dtFrom = DateTime.Parse(sFromDT).ToUniversalTime();
            DateTime dtTo = DateTime.Parse(sToDT).ToUniversalTime();

            if (dtFrom >= dtTo)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<RptInOutModels> modelList = DALRecord.GetRecordByPartIdFromDtToDt(iPartId, dtFrom, dtTo);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Load lists for dropdown list
        /// </summary>
        /// <param name="model"></param>
        [NonAction]
        private void LoadRptInOutModelDdl(RptInOutModels model)
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