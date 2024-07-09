/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* CheckSession.cs                                                                                      */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Update session time                                             Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.DAL;
using SPSLib;
using System;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using WebMatrix.WebData;

namespace SPS.Filters
{
    /// <summary>
    /// This filter is used to check the session validity
    /// </summary>
    public class CheckSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Get misc settings
            Misc misc = new Misc();
            DALMisc.GetMisc(out misc);

            // Get user session
            DALSession.GetSessionById(out DataTable tblSession, WebSecurity.CurrentUserId);

            string sCurSessionID = filterContext.HttpContext.Session.SessionID;

            if (tblSession.Rows.Count > 0)
            {
                string sSessionID = tblSession.Rows[0]["SessionId"].ToString();
                DateTime dtLastSessionTime = Convert.ToDateTime(tblSession.Rows[0]["LastSessionTime"]);

                // Check if current session ID is same as valid session ID
                if (sCurSessionID != sSessionID)
                {
                    GoToLoginPage(filterContext);
                    return;
                }

                // Check if session has expired
                //if (dtLastSessionTime.AddMinutes(misc.IdealTime) < DateTime.UtcNow)
                if (dtLastSessionTime < DateTime.UtcNow)
                {
                    GoToLoginPage(filterContext);
                    return;
                }
            }

            // Update Session Time (Session Time = now + ideal time)
            DALSession.SetSession(WebSecurity.CurrentUserId, sCurSessionID, misc.IdleTime);

            base.OnActionExecuting(filterContext);
        }

        private void GoToLoginPage(ActionExecutingContext filterContext)
        {
            // Force logout
            //WebSecurity.Logout();

            //filterContext.Controller.TempData.Add("Msg", sMsg);

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                action = "AutoLogOff",
                controller = "Account",
            }));
        }
    }
}