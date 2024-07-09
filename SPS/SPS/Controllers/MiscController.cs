/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* MiscController.cs                                                                                    */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Update session idle time                                        Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.DAL;
using SPS.Filters;
using SPS.Models;
using SPSLib;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace SPS.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [CheckSession]
    public class MiscController : Controller
    {
        // GET: Misc
        public ActionResult Index()
        {
            ViewBag.Current = "Misc";

            MiscModels modelMisc = DALMisc.GetMiscList();

            // Load dropdownlists
            LoadDdl(modelMisc);

            if (modelMisc.EmailPassword != "")
            {
                // Decrypt email password
                modelMisc.EmailPassword = Common.Decrypt(modelMisc.EmailPassword);
            }

            return View(modelMisc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MiscModels miscModel)
        {
            ViewBag.Current = "Misc";

            if (miscModel.EmailPassword != "")
            {
                // Encrypt email password
                miscModel.EmailPassword = Common.Encrypt(miscModel.EmailPassword);
            }

            if (ModelState.IsValid)
            {
                if (DALMisc.UpdMisc(miscModel))
                {
                    string sLogDesc = "Edit [";
                    sLogDesc += "SMTP Server: " + miscModel.EmailSmtp + " | ";
                    sLogDesc += "Port: " + miscModel.EmailPort + " | ";
                    sLogDesc += "Protocol: " + Convert.ToInt32(miscModel.EmailProtocol) + " | ";
                    sLogDesc += "Username: " + miscModel.EmailUsername + " | ";
                    sLogDesc += "Data Retention Period: " + Convert.ToInt32(miscModel.RetentionPeriod) + " | ";
                    sLogDesc += "Max File Upload Size (MB): " + Convert.ToInt32(miscModel.AttachmentSize) + " | ";
                    sLogDesc += "Auto Logout Time (Min): " + Convert.ToInt32(miscModel.IdleTime) + " | ";
                    sLogDesc += "Password Reset Validation Time (Min): " + Convert.ToInt32(miscModel.TokenResetTime) + " | ";
                    sLogDesc += "Sender's Email: " + miscModel.DefaultEmail + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_MISC, WebSecurity.CurrentUserName);

                    // Update session Idle Time
                    Session["IdleTime"] = miscModel.IdleTime;

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_UPD_MISC;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_UPD_MISC;
                }
            }

            LoadDdl(miscModel);

            return View(miscModel);
        }

        /// <summary>
        /// Load dropdownlist
        /// </summary>
        /// <param name="model"></param>
        private void LoadDdl(MiscModels model)
        {
            model.EmailProtocolList = LoadEmailProtocolDdl();
        }

        /// <summary>
        /// Load email protocol drop down
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private SelectList LoadEmailProtocolDdl()
        {
            List<SelectListItem> itemsList = new List<SelectListItem>
            {
                new SelectListItem { Text = "-", Value = ((int)EnumEx.EmailProtocol.NONE).ToString() },
                new SelectListItem { Text = "TLS/SSL", Value = ((int)EnumEx.EmailProtocol.TLSSSL).ToString() }
            };

            return new SelectList(itemsList, "Value", "Text");
        }
    }
}