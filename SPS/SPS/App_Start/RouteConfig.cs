/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RouteConfig.cs                                                                                       */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System.Web.Mvc;
using System.Web.Routing;

namespace SPS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Record", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ResetPassword",
                url: "{controller}/{action}/{id}/{token}",
                defaults: new { controller = "Account", action = "ResetPassword", id = UrlParameter.Optional, token = UrlParameter.Optional },
                constraints: new { id = @"\d+" }    // id is integer
            );
        }
    }
}
