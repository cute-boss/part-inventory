/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RptLogController.cs                                                                                  */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      07/08/2023  Add database backup to GetLogTypeList                           Azmir               */
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
    public class RptLogController : Controller
    {
        [Authorize(Roles = "ADMIN, USER")]
        [CheckSession]
        // GET: Log
        public ActionResult Index()
        {
            ViewBag.Report = true;
            ViewBag.Current = "RptLog";

            RptLogModels model = new RptLogModels();

            // Load drop down list
            LoadDdl(model);

            return View(model);
        }

        /// <summary>
        /// Load drop down list
        /// </summary>
        /// <param name="model"></param>
        private void LoadDdl(RptLogModels model)
        {
            model.LogTypeList = GetLogTypeList();
        }

        /// <summary>
        /// Get log type list
        /// </summary>
        /// <returns></returns>
        public SelectList GetLogTypeList()
        {
            List<SelectListItem> logTypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "ALL", Value = (0).ToString() },
                new SelectListItem { Text = "LOGIN", Value = ((int)EnumEx.LogType.LOGIN).ToString() },
                new SelectListItem { Text = "LOGOUT", Value = ((int)EnumEx.LogType.LOGOUT).ToString() },
                new SelectListItem { Text = "RESET PASSWORD", Value = ((int)EnumEx.LogType.CHG_PWD).ToString() },
                new SelectListItem { Text = "CHANGE PASSWORD", Value = ((int)EnumEx.LogType.RESET_PWD).ToString() },
                new SelectListItem { Text = "RECORD", Value = ((int)EnumEx.LogType.MAIN_RECORD).ToString() },
                new SelectListItem { Text = "USER", Value = ((int)EnumEx.LogType.SYS_USER).ToString() },
                new SelectListItem { Text = "MISCELLANEOUS", Value = ((int)EnumEx.LogType.SYS_MISC).ToString() },
                new SelectListItem { Text = "BUILDING", Value = ((int)EnumEx.LogType.SYS_BUILDING).ToString() },
                new SelectListItem { Text = "RACK", Value = ((int)EnumEx.LogType.SYS_RACK).ToString() },
                new SelectListItem { Text = "PART", Value = ((int)EnumEx.LogType.SYS_PART).ToString() },
                new SelectListItem { Text = "PART ASSIGNMENT", Value = ((int)EnumEx.LogType.SYS_PART_RACK).ToString() },
                new SelectListItem { Text = "BATCH UPLOAD", Value = ((int)EnumEx.LogType.BATCH_UPLOAD).ToString() },
                new SelectListItem { Text = "EVENT LOG", Value = ((int)EnumEx.LogType.EVTLOG_SVR).ToString() },
                new SelectListItem { Text = "REMOVE HISTORY", Value = ((int)EnumEx.LogType.EVTLOG_REMOVE_HISTORY).ToString() },
                new SelectListItem { Text = "SEND NOTIFICATION", Value = ((int)EnumEx.LogType.EVTLOG_NOTIFICATION).ToString() },
                new SelectListItem { Text = "DATABASE BACKUP", Value = ((int)EnumEx.LogType.EVTLOG_DB_BACKUP).ToString() }
            };

            // Sort list
            logTypeList.Sort((x, y) => x.Text.CompareTo(y.Text));

            return new SelectList(logTypeList, "Value", "Text");
        }

        /// <summary>
        /// Get log detail
        /// </summary>
        /// <param name="iLogTypeId"></param>
        /// <param name="sStartDateTime"></param>
        /// <param name="sEndDateTime"></param>
        /// <returns></returns>
        public JsonResult GetLog(int iLogTypeId, string sStartDateTime, string sEndDateTime)
        {
            // Check start and end date time
            DateTime dtStart = DateTime.Parse(sStartDateTime.Substring(1, 19)).ToUniversalTime();
            DateTime dtEnd = DateTime.Parse(sEndDateTime.Substring(1, 19)).ToUniversalTime();

            if (dtStart >= dtEnd)
            {
                ViewBag.Message = CommonMsg.INVALID_DATETIME;
                return null;
            }

            IList<LogDetailModel> modelList = DALLog.GetLogList(iLogTypeId, dtStart, dtEnd);
            RptLogModels model = new RptLogModels
            {
                LogTypeList = GetLogTypeList()
            };

            foreach (var m in modelList)
            {
                m.LogTypeName = model.LogTypeList.Where(p => p.Value == m.LogTypeId.ToString()).First().Text;
            }

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }
    }
}