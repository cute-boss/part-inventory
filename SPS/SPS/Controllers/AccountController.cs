/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* AccountController.cs                                                                                 */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add auto logout function                                        Azmir               */
/*      09/03/2023  Update body email message                                       Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.DAL;
using SPS.Models;
using SPS.Filters;
using SPSLib;
using System;
using System.Data;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace SPS.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string returnUrl)
        {
            if (WebSecurity.IsAuthenticated)
            {
                //return RedirectToAction("LogOff", "Account");
                return RedirectToAction("Index", "Record");
            }

            // TempData set by SessionFilter
            if (TempData["Msg"] != null)
            {
                ViewBag.AlertMsg = TempData["Msg"];
            }

            ViewBag.ReturnUrl = returnUrl;
            var model = new LoginModel();
            return View(model);
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.UserExists(model.UserName))
                {
                    using (UsersContext db = new UsersContext())
                    {
                        UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

                        // Check if user active is active
                        if (user.IsActive)
                        {
                            // Check if the current login user has exceed limit
                            if (!ChkMaxLogin())
                            {
                                ViewBag.Message = CommonMsg.INVALID_MAX_USER;
                                return View(model);
                            }

                            if (WebSecurity.Login(model.UserName, model.Password, persistCookie: false))
                            {
                                // Get misc settings
                                Misc misc = new Misc();
                                DALMisc.GetMisc(out misc);

                                // Allocate SessionID to session object, otherwise Session ID will keep changing on every request
                                // SessionID will be used to verify the validity of current login session
                                Session["SessionID"] = HttpContext.Session.SessionID;

                                // Session for auto logout
                                Session["IdleTime"] = misc.IdleTime;

                                // Set latest session ID into database
                                if (!DALSession.SetSession(WebSecurity.GetUserId(model.UserName), Session["SessionID"].ToString(), misc.IdleTime))
                                {
                                    // Force logout if error occurs in Login process
                                    WebSecurity.Logout();

                                    return RedirectToAction("Error", "Error", new { id = 500 });
                                }
                                else
                                {
                                    string sLogDesc = "Login";
                                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.LOGIN, model.UserName);

                                    //return RedirectToAction("Index", "Home");
                                    return RedirectToLocal(returnUrl);
                                }
                            }
                        }
                    }
                }

                ViewBag.Message = CommonMsg.INVALID_USRNAME_PWD;
                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        // POST: /Account/LogOff
        [Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            string sLogDesc = "Logout";
            DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.LOGOUT, WebSecurity.CurrentUserName);

            if (Session["SessionID"] != null)
            {
                DALSession.UpdSessionLogout(Session["SessionID"].ToString());
            }

            WebSecurity.Logout();

            // Assign logout flag to display logout message in login page
            TempData["Logout"] = true;

            return RedirectToAction("Login", "Account");
        }

        // POST: /Account/LogOff
        [Authorize]
        public ActionResult AutoLogOff()
        {
            string sLogDesc = "Auto logout";
            DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.LOGOUT, WebSecurity.CurrentUserName);

            if (Session["SessionID"] != null)
            {
                DALSession.UpdSessionLogout(Session["SessionID"].ToString());
            }

            WebSecurity.Logout();

            TempData["Msg"] = CommonMsg.SESSION_EXPIRED;

            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            var model = new ForgotPasswordModel();
            return View(model);
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.UserExists(model.UserName))
                {
                    using (UsersContext db = new UsersContext())
                    {
                        UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

                        if (!DALUser.GetEmailByUserId(out string sEmail, user.UserId))
                        {
                            return RedirectToAction("Error", "Error", new { id = 500 });
                        }

                        // Check if username and email are matched
                        if (sEmail.ToLower() == model.Email.ToLower())
                        {
                            if (!DALMisc.GetMisc(out Misc misc))
                            {
                                //ViewBag.Message = CommonMsg.SQL_ERR;
                                return RedirectToAction("Error", "Error", new { id = 500 });
                            }

                            // Generate reset token which lasts for 15 mins
                            string sResetToken = WebSecurity.GeneratePasswordResetToken(model.UserName, misc.TokenResetTime);

                            // Get current URL
                            Uri myUri = new Uri(Request.Url.ToString());

                            // Get root URL
                            string sApplicationAlias = HostingEnvironment.ApplicationVirtualPath;
                            string sURL = "";
                            string sMsg = "";

                            if (sApplicationAlias != "/")
                            {
                                sURL = myUri.GetLeftPart(UriPartial.Authority) + sApplicationAlias + "/Account/ResetPassword";
                            }
                            else
                            {
                                sURL = myUri.GetLeftPart(UriPartial.Authority) + "/Account/ResetPassword";
                            }

                            string sContent = sURL + "/" + WebSecurity.GetUserId(model.UserName) + "/" + sResetToken + "<br><br>";
                            sContent += "This link is valid for " + misc.TokenResetTime + " minutes.<br>";

                            sMsg = "<html><body style= 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size: 12px;'>"
                                        + "Hi,<br><br>Good day.<br><br>To reset your password, please click on the link below:<br><br>"
                                        + sContent
                                        + "If you didn't make this request, you can ignore this email.<br><br>"
                                        + "Thank you.<br><br>"
                                        + "<i>Please do not reply to this email.<br>This is an automated application used only for sending notifications.</i></body></html>";

                            if (Common.SendEmailNoti(misc.EmailSmtp, misc.EmailPort, misc.EmailUsername, Common.Decrypt(misc.EmailPassword), misc.EmailProtocol, misc.DefaultEmail,
                                sEmail, "SPS - Reset Password", sMsg))
                            {
                                ViewBag.Status = true;
                                ViewBag.Message = CommonMsg.SENT_TOKEN;
                            }
                            else
                            {
                                ViewBag.Message = CommonMsg.FAIL_SEND_EMAIL;
                            }
                        }
                        else
                        {
                            ViewBag.Message = CommonMsg.INVALID_USRNAME_EMAIL;
                        }
                    }
                }
                else
                {
                    ViewBag.Message = CommonMsg.INVALID_USRNAME_EMAIL;
                }

                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        // GET: /Account/ResetPassword
        public ActionResult ResetPassword(int? id, string token)
        {
            ResetPasswordModel model = new ResetPasswordModel();

            if (id == null)
            {
                ViewBag.Message = CommonMsg.INVALID_TOKEN;
                return View(model);
            }

            // Check if user id is valid
            //int iUserId = WebSecurity.GetUserIdFromPasswordResetToken(token);
            int id_ = WebSecurity.GetUserIdFromPasswordResetToken(token);
            if (id != id_)
            {
                ViewBag.Message = CommonMsg.INVALID_TOKEN;
                return View(model);
            }

            if (!DALUser.GetMembershipById(out DataTable tblUser, id))
            {
                ViewBag.Message = CommonMsg.SQL_ERR;
            }
            else
            {
                if (tblUser.Rows.Count > 0)
                {
                    // Check if token has expired
                    DateTime dtTokenExpiry = Convert.ToDateTime(tblUser.Rows[0]["PasswordVerificationTokenExpirationDate"]);
                    if (dtTokenExpiry <= DateTime.UtcNow)
                    {
                        ViewBag.Message = CommonMsg.TOKEN_EXPIRY;
                    }
                }
            }

            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.ResetPassword(model.Token, model.NewPassword))
                {
                    DALUser.GetUsernameByResetToken(out string sUsername, model.Token);

                    string sLogDesc = "Reset password";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.RESET_PWD, sUsername);

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.PWD_RESET;
                }
                else
                {
                    ViewBag.Message = CommonMsg.TOKEN_EXPIRY;
                }
            }

            return View(model);
        }

        // GET: /Account/ChangePassword
        [Authorize]
        [CheckSession]
        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordModel();
            return View(model);
        }

        // POST: /Account/ChangePassword
        [Authorize]
        [CheckSession]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    string sLogDesc = "Change password";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.CHG_PWD, WebSecurity.CurrentUserName);

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.PWD_CHANGE;
                    return View(model);
                }
                else
                {
                    ViewBag.Message = CommonMsg.INVALID_OLD_PWD;
                    return View(model);
                }
            }

            return View(model);
        }

        private bool ChkMaxLogin()
        {
            DALSession.GetValidSession(out int iLogin);

            if (iLogin >= Common.G_IMAX_LOGIN)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Record");
            }
        }
    }
}