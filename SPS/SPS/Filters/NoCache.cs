/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* NoCache.cs                                                                                           */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System;
using System.Web;
using System.Web.Mvc;

namespace SPS.Filters
{
    /// <summary>
    /// This filter is used to clear cache for important page 
    /// in order to prevent user navigate using back/forward buttons after logout
    /// </summary>
    public class NoCache : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnActionExecuting(filterContext);
        }
    }
}