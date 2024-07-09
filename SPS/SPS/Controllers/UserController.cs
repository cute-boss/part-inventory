/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* UserController.cs                                                                                    */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
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
using System.Web.Security;
using WebMatrix.WebData;

namespace SPS.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [CheckSession]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            ViewBag.Current = "User";

            IList<UserModels> modelList = DALUser.GetUserProfile();
            return View(modelList);
        }

        public ActionResult Detail(int id)
        {
            ViewBag.Current = "User";

            UserModels userModel = DALUser.GetUserProfileById(id);

            // Check if id is invalid
            if (userModel.UserId == 0)
            {
                TempData["ErrMsg"] = CommonMsg.INVALID_ID;
                return RedirectToAction("Index", "User");
            }

            UserDetailModels model = new UserDetailModels
            {
                UserModel = userModel,
            };

            return View(model);
        }

        public ActionResult Add()
        {
            ViewBag.Current = "User";

            UserEditModels model = new UserEditModels
            {
                UserModel = new UserModels
                {
                    //set default value to false
                    IsActive = true,
                },
            };

            // Load dropdownlists
            LoadUserEditModelDdl(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(UserEditModels model)
        {
            ViewBag.Current = "User";

            // Load dropdownlist
            LoadUserEditModelDdl(model);

            // Trim model properties
            TrimStrProperty(model);

            if (ModelState.IsValid)
            {
                #region Validation
                // Check if username is exist
                if (WebSecurity.UserExists(model.UserModel.UserName))
                {
                    ViewBag.Message = CommonMsg.DUP_USRNAME;
                    return View(model);
                }

                // Check if staff no is exist
                DALUser.ChkExistStaffNo(out bool bExist, model.UserModel.StaffNo);
                if (bExist)
                {
                    ViewBag.Message = CommonMsg.DUP_STAFFNO;
                    return View(model);
                }

                // Check if email is exist
                DALUser.ChkExistEmail(out bExist, model.UserModel.Email);
                if (bExist)
                {
                    ViewBag.Message = CommonMsg.DUP_EMAIL;
                    return View(model);
                }
                #endregion

                #region Register user
                try
                {
                    // Create user login and role
                    WebSecurity.CreateUserAndAccount(model.UserModel.UserName, Common.g_sDefPwd,
                        propertyValues: new
                        {
                            model.UserModel.StaffName,
                            model.UserModel.StaffNo,
                            model.UserModel.Email,
                            model.UserModel.IsActive
                        });

                    Roles.AddUserToRole(model.UserModel.UserName, model.UserModel.RoleName);

                    #region Log
                    string sLogDesc = "Add [";
                    sLogDesc += "Username: " + model.UserModel.UserName + " | ";
                    sLogDesc += "Role: " + model.UserModel.RoleName + " | ";
                    sLogDesc += "Status: " + Convert.ToInt32(model.UserModel.IsActive) + " | ";
                    sLogDesc += "Staff No.: " + model.UserModel.StaffNo + " | ";
                    sLogDesc += "Name: " + model.UserModel.StaffName + " | ";
                    sLogDesc += "Email: " + model.UserModel.Email + "] ";

                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_USER, WebSecurity.CurrentUserName);
                    #endregion

                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_USER, WebSecurity.CurrentUserName);

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_REG_USER;
                }
                catch
                {
                    ViewBag.Message = CommonMsg.FAIL_REG_USER;
                    return View(model);
                }
                #endregion
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Current = "User";

            UserModels userModel = DALUser.GetUserProfileById(id);

            // Check if id is invalid
            if (userModel.UserId == 0)
            {
                TempData["ErrMsg"] = CommonMsg.INVALID_ID;
                return RedirectToAction("Index", "User");
            }

            #region Load model
            UserEditModels model = new UserEditModels
            {
                UserModel = userModel,
            };

            model.UserModel.OldRoleName = model.UserModel.RoleName;
            model.UserModel.OldStaffNo = model.UserModel.StaffNo;
            model.UserModel.OldEmail = model.UserModel.Email;

            // Load dropdownlists
            LoadUserEditModelDdl(model);

            #endregion

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditModels model)
        {
            ViewBag.Current = "User";

            // Load dropdownlists
            LoadUserEditModelDdl(model);

            // Trim model properties
            TrimStrProperty(model);

            if (ModelState.IsValid)
            {
                bool bExist;

                // Check if staff no is exist
                if (model.UserModel.StaffNo != model.UserModel.OldStaffNo)
                {
                    DALUser.ChkExistStaffNo(out bExist, model.UserModel.StaffNo);
                    if (bExist)
                    {
                        ViewBag.Message = CommonMsg.DUP_STAFFNO;
                        return View(model);
                    }
                }

                // Check if email is exist
                if (model.UserModel.Email.ToLower() != model.UserModel.OldEmail.ToLower())
                {
                    DALUser.ChkExistEmail(out bExist, model.UserModel.Email);
                    if (bExist)
                    {
                        ViewBag.Message = CommonMsg.DUP_EMAIL;
                        return View(model);
                    }
                }

                #region Update user
                // Update user
                if (DALUser.UpdUser(model.UserModel))
                {
                    if (model.UserModel.RoleName != model.UserModel.OldRoleName)
                    {
                        // Remove old user role
                        Roles.RemoveUserFromRole(model.UserModel.UserName, model.UserModel.OldRoleName);

                        // Add new user role
                        Roles.AddUserToRole(model.UserModel.UserName, model.UserModel.RoleName);
                    }

                    #region Log
                    string sLogDesc = "Edit [";
                    sLogDesc += "Username: " + model.UserModel.UserName + " | ";
                    sLogDesc += "Role: " + model.UserModel.RoleName + " | ";
                    sLogDesc += "Status: " + Convert.ToInt32(model.UserModel.IsActive) + " | ";
                    sLogDesc += "Staff No.: " + model.UserModel.StaffNo + " | ";
                    sLogDesc += "Name: " + model.UserModel.StaffName + " | ";
                    sLogDesc += "Email: " + model.UserModel.Email + "] ";

                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_USER, WebSecurity.CurrentUserName);
                    #endregion

                    ViewBag.Status = true;
                    ViewBag.Message = CommonMsg.SUCC_UPD_USER;
                }
                else
                {
                    ViewBag.Message = CommonMsg.FAIL_UPD_USER;
                }
                #endregion
            }

            return View(model);
        }

        /// <summary>
        /// Remove user role and account
        /// </summary>
        /// <param name="id"></param>
        [HttpPost]
        public string Delete(int id)
        {
            bool bResult = false;
            UserModels model = DALUser.GetUserProfileById(id);

            #region Delete user

            if (model.UserName != null)
            {
                // TODO: Add delete logic here
                if (WebSecurity.CurrentUserName != model.UserName)
                {
                    if (Roles.GetRolesForUser(model.UserName).Count() > 0)
                    {
                        Roles.RemoveUserFromRoles(model.UserName, Roles.GetRolesForUser(model.UserName));
                    }

                    // Deletes record from webpages_Membership table
                    ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(model.UserName);

                    // Deletes record from UserProfile table
                    ((SimpleMembershipProvider)Membership.Provider).DeleteUser(model.UserName, true);

                    #region Log
                    string sLogDesc = "Delete [";
                    sLogDesc += "Username: " + model.UserName + " | ";
                    sLogDesc += "Role: " + model.RoleName + " | ";
                    sLogDesc += "Status: " + Convert.ToInt32(model.IsActive) + " | ";
                    sLogDesc += "Staff No.: " + model.StaffNo + " | ";
                    sLogDesc += "Name: " + model.StaffName + "] ";
                    DALLog.SetLog(sLogDesc, (int)EnumEx.LogType.SYS_USER, WebSecurity.CurrentUserName);
                    #endregion

                    bResult = true;
                }
            }
            #endregion

            if (bResult)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// Staff name validation (used by ajax)
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public string ChkUsername(string username)
        {
            if (WebSecurity.UserExists(username))
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// Staff no validation (used by ajax)
        /// </summary>
        /// <param name="staffNo"></param>
        /// <returns></returns>
        [HttpPost]
        public string ChkStaffNo(string staffNo)
        {
            DALUser.ChkExistStaffNo(out bool bExist, staffNo);

            if (bExist)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// Email validation (used by ajax)
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public string ChkEmail(string email)
        {
            DALUser.ChkExistEmail(out bool bExist, email);

            if (bExist)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// Load lists for dropdown list
        /// </summary>
        /// <param name="model"></param>
        [NonAction]
        private void LoadUserEditModelDdl(UserEditModels model)
        {
            model.RoleList = new SelectList(DALUser.GetRoleList(), "RoleName", "RoleName", model.UserModel.RoleName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        [NonAction]
        private void TrimStrProperty(UserEditModels model)
        {
            // Trim model properties
            model.UserModel.StaffNo = model.UserModel.StaffNo.Trim();
            model.UserModel.UserName = model.UserModel.UserName.Trim();
            model.UserModel.Email = model.UserModel.Email.Trim();
        }
    }
}