/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* BaseController.cs                                                                                    */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.DAL;
using SPS.Models;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace SPS.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public ActionResult NavBar()
        {
            UserModels model = DALUser.GetUserProfileById(WebSecurity.CurrentUserId);
            return PartialView("~/Views/Shared/_NavBar.cshtml", model);
        }
    }
}