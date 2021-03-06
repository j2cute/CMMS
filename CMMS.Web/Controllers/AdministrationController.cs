﻿using ClassLibrary.Common;
using ClassLibrary.Models;
using ILS.UserManagement.Models;
using ILS.UserManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication.Helpers;
using Microsoft.AspNet.Identity.Owin;
using static ClassLibrary.Common.Enums;
using Microsoft.AspNet.Identity;
using NLog;
using ClassLibrary.ViewModels;
using CMMS.Web.Helper;

namespace WebApplication.Controllers
{
    public class AdministrationController : Controller
    {
        private static Logger _logger;
        private Entities _context;

        public AdministrationController()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        #region UserCheck

        [CheckUserSession]
        public JsonResult CheckUserName(string Pno)
        {
            string actionName = "CheckUserName";
            try
            {
                using (var _context = new Entities())
                {
                    _logger.Log(LogLevel.Trace, actionName + " :: started.");

                    var SearchData = _context.tbl_User.Where(x => x.Pno == Pno);
                    if (SearchData.Any())
                    {
                        return Json(1);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            return Json(0);
        }

        #endregion UserCheck

        #region roles

        [CustomAuthorization]
        public ActionResult ViewRoles()
        {
            string actionName = "ViewRoles";
            RolesViewModel vm = new RolesViewModel();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");

                using (var _context = new Entities())
                {
                    vm = new RolesViewModel()
                    {
                        tbl_Role_list = GetActiveRoles(),
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            return View(vm);
        }

        [CustomAuthorization]
        public ActionResult Create()
        {
            string actionName = "Create";
            RolesViewModel vm = new RolesViewModel();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                List<TreeViewNode> nodesMaster = new List<TreeViewNode>();

                using (var _context = new Entities())
                {
                    foreach (var p in _context.tbl_Permission)
                    {
                        if (p.ParentId == "0")
                        {
                            p.ParentId = "#";
                        }
                        nodesMaster.Add(new TreeViewNode { id = p.PermissionId, parent = p.ParentId, text = p.DisplayName });
                    }
                    vm.PermissionsList = (new JavaScriptSerializer()).Serialize(nodesMaster);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            return PartialView("_CreateRole", vm);
        }

        [HttpPost]
        [CustomAuthorization]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_Role tbl_Role, string selectedItems)
        {
            using (var _context = new Entities())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    List<TreeViewNode> parentItems = new List<TreeViewNode>();

                    var vm = new RolesViewModel();
                    try
                    {
                        if (tbl_Role != null)
                        {
                            _context.tbl_Role.Add(tbl_Role);
                            _context.SaveChanges();
                        }
                        List<TreeViewNode> items = (new JavaScriptSerializer()).Deserialize<List<TreeViewNode>>(selectedItems);
                        List<string> newListOfParent = new List<string>();
                        foreach (var temp in items.Select(x => x.parents))
                        {
                            newListOfParent = newListOfParent.Concat(temp).ToList();
                        }
                        newListOfParent = newListOfParent.Distinct().ToList();

                        foreach (var selected in items)
                        {
                            if (selected.parent == "#")
                            {
                                selected.parent = "0";
                            }

                            if (!_context.tbl_RolePermission.Where(x => x.RoleId == tbl_Role.RoleId).Any(x => x.PermissionId == selected.id))
                            {
                                var model = new tbl_RolePermission
                                {
                                    PermissionId = selected.id,
                                    RoleId = tbl_Role.RoleId,
                                };
                                _context.tbl_RolePermission.Add(model);
                            }
                        }

                        foreach (var selected in newListOfParent)
                        {
                            if (!items.Where(x => x.id == selected).Any() && selected != "#")
                            {
                                tbl_Permission permission = _context.tbl_Permission.Where(x => x.PermissionId == selected).FirstOrDefault();

                                if (!_context.tbl_RolePermission.Where(x => x.RoleId == tbl_Role.RoleId).Any(x => x.PermissionId == selected))
                                {
                                    var model = new tbl_RolePermission
                                    {
                                        PermissionId = selected,
                                        RoleId = tbl_Role.RoleId,
                                    };
                                    _context.tbl_RolePermission.Add(model);
                                }
                            }
                        }

                        _context.SaveChanges();
                        transaction.Commit();

                        //Serialize to JSON string.                
                        // Alert("Data Saved Sucessfully!!!", NotificationType.success);
                        return RedirectToAction("ViewRoles");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        //   Exception(ex);
                        //  Alert("Their is something went wrong!!!", NotificationType.error);
                        return RedirectToAction("ViewRoles");
                    }
                }
            }
        }

        [CustomAuthorization]
        public ActionResult Edit(string id)
        {
            using (var _context = new Entities())
            {
                RolesViewModel vm = new RolesViewModel()
                { tbl_Role = _context.tbl_Role.Where(x => x.RoleId == id).SingleOrDefault() };
                List<TreeViewNode> nodesMaster = new List<TreeViewNode>();
                var data = _context.tbl_RolePermission.Where(x => x.RoleId == id).ToList();
                foreach (tbl_Permission p in _context.tbl_Permission)
                {
                    State checkState = new State();
                    checkState.selected = false;
                    foreach (var item in data)
                    {

                        if (p.PermissionId == item.PermissionId)
                        {    // Set Check State.  
                            var isParent = 0;
                            foreach (tbl_Permission per in _context.tbl_Permission)
                            {
                                if (p.PermissionId == per.ParentId)
                                {
                                    isParent = 1;
                                }

                            }

                            if (isParent == 0)
                            {
                                checkState.selected = true;
                            }

                        }
                    }
                    if (p.ParentId == "0") { p.ParentId = "#"; }
                    nodesMaster.Add(new TreeViewNode { id = p.PermissionId, parent = p.ParentId, text = p.DisplayName, state = checkState });
                }
                //Serialize to string.
                vm.PermissionsList = (new JavaScriptSerializer()).Serialize(nodesMaster);

                return PartialView("_EditRole", vm);
            }
        }

        [HttpPost]
        [CustomAuthorization]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, tbl_Role tbl_Role, string selectedItems)
        {
            using (var _context = new Entities())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    List<TreeViewNode> parentItems = new List<TreeViewNode>();
                    var vm = new RolesViewModel();
                    try
                    {
                        if (id != null)
                        {
                            _context.tbl_RolePermission.RemoveRange(_context.tbl_RolePermission.Where(x => x.RoleId == id));
                            _context.SaveChanges();
                        }
                        if (selectedItems != "[]")
                        {
                            List<TreeViewNode> items = (new JavaScriptSerializer()).Deserialize<List<TreeViewNode>>(selectedItems);
                            List<string> newListOfParent = new List<string>();
                            foreach (var temp in items.Select(x => x.parents))
                            {
                                newListOfParent = newListOfParent.Concat(temp).ToList();
                            }
                            newListOfParent = newListOfParent.Distinct().ToList();

                            foreach (var selected in items)
                            {
                                if (selected.parent == "#")
                                {
                                    selected.parent = "0";
                                }

                                if (!_context.tbl_RolePermission.Where(x => x.RoleId == tbl_Role.RoleId).Any(x => x.PermissionId == selected.id))
                                {
                                    var model = new tbl_RolePermission
                                    {
                                        PermissionId = selected.id,
                                        RoleId = tbl_Role.RoleId,
                                    };
                                    _context.tbl_RolePermission.Add(model);
                                }
                            }

                            foreach (var selected in newListOfParent)
                            {
                                if (!items.Where(x => x.id == selected).Any() && selected != "#")
                                {
                                    tbl_Permission permission = _context.tbl_Permission.Where(x => x.PermissionId == selected).FirstOrDefault();

                                    if (!_context.tbl_RolePermission.Where(x => x.RoleId == tbl_Role.RoleId).Any(x => x.PermissionId == selected))
                                    {
                                        var model = new tbl_RolePermission
                                        {
                                            PermissionId = selected,
                                            RoleId = tbl_Role.RoleId,
                                        };
                                        _context.tbl_RolePermission.Add(model);
                                    }
                                }
                            }

                            _context.SaveChanges();


                        }


                        //Serialize to JSON string.                
                        // Alert("Data Saved Sucessfully!!!", NotificationType.success);
                        transaction.Commit();
                        return RedirectToAction("ViewRoles");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        //   Exception(ex);
                        //  Alert("Their is something went wrong!!!", NotificationType.error);
                        return RedirectToAction("ViewRoles");
                    }
                }
            }
        }

        [CustomAuthorization]
        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorization]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        #endregion roles

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #region user

        [CustomAuthorization]
        public ActionResult UserIndex()
        {
            using (var _context = new Entities())
            {
                UserRoleViewModel vm = new UserRoleViewModel()
                {
                    tbl_User_list = _context.tbl_User.Where(x=>x.IsDeleted != 1).ToList(),

                };
                return View(vm);
            }

        }

        [CustomAuthorization]
        public ActionResult CreateUser()
        {
            using (var _context = new Entities())
            {
                UserRoleViewModel vm = new UserRoleViewModel()
                {

                    _tbl_Unit = _context.tbl_Unit.ToList(),
                    _tbl_Role = _context.tbl_Role.ToList(),
                };
                return PartialView("_CreateUser", vm);
            }
        }

        [HttpPost]
        [CustomAuthorization]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(tbl_User tbl_User, tbl_Role tbl_Role)
        {
            using (var _context = new Entities())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        string password = string.Empty;
                        if (tbl_User.Pno != null)
                        {
                            password = "Abc@" + tbl_User.Pno.Trim();

                        }
                        if (!ModelState.IsValid)
                        {
                            UserRoleViewModel vm = new UserRoleViewModel()
                            {
                                _tbl_Role = _context.tbl_Role.ToList(),
                                _tbl_Unit = _context.tbl_Unit.ToList(),
                            };
                            return PartialView("_CreateUser", vm);
                        }

                        if (tbl_User.Pno != null)
                        {
                            tbl_User.UserId = tbl_User.Pno;
                            tbl_User.Password = password;

                            _context.tbl_User.Add(tbl_User);
                            _context.SaveChanges();

                            if (tbl_Role.RoleId != null)
                            {
                                var obj = new tbl_UserRole()
                                {
                                    UserId = tbl_User.UserId,
                                    RoleId = tbl_Role.RoleId,
                                    IsDefault = 1,
                                    IsActive = 1,
                                };
                                _context.tbl_UserRole.Add(obj);
                                _context.SaveChanges();
                            }


                        }


                        #region User Injection

                        var UserName = tbl_User.UserId;
                        var Password = password;


                        var objNewAdminUser = new ApplicationUser { Id= tbl_User.UserId,  UserName = tbl_User.UserId, Email = tbl_User.UserId };
                        var AdminUserCreateResult = UserManager.Create(objNewAdminUser, Password);
                        if (AdminUserCreateResult.Succeeded)
                        {

                            transaction.Commit();
                        }
                        else
                        {

                            transaction.Rollback();
                        }

                        #endregion

                        return RedirectToAction("UserIndex");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        //   Exception(ex);
                        // Alert("Their is something went wrong!!!", NotificationType.error);
                        return RedirectToAction("UserIndex");
                    }
                }
            }
        }

        [HttpGet]
        [CheckUserSession]
        public ActionResult GetRoles()
        {
            using (var _context = new Entities())
            {
                var roles = _context.tbl_Role.Where(x=>x.IsDeleted != 1).Select(x => new RolesModel
                {
                    RoleId = x.RoleId,
                    Name = x.Name,

                }).ToList();

                return new JsonResult() { Data = new { roles = roles }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

        [CustomAuthorization]
        public ActionResult EditUser(string id)
        {
            using (var _context = new Entities())
            {
                UserRoleViewModel vm = new UserRoleViewModel();
                var unit = _context.tbl_Unit.ToList();
                var allRoles = _context.tbl_Role.Select(x => new RolesModel
                {
                    RoleId = x.RoleId,
                    Name = x.Name,
                    IsDefault = x.IsDefault,
                }).ToList();

                //  List<RolesModel> roles = new List<RolesModel>();
                var result = _context.tbl_User.Where(x => x.UserId == id).FirstOrDefault();
                if (result.UserId != null)
                {
                    var userRole = _context.tbl_UserRole.Where(x => x.UserId == id).Select(x => new UserRoleModel
                    {
                        RoleId = x.RoleId,
                        UserId = x.UserId,
                        IsDefault = x.IsDefault
                    }).ToList();
                    vm._tbl_Unit = unit;
                    vm.RolesModel_list = allRoles;
                    vm.tbl_User = result;
                    vm.userRoleModel_list = userRole;

                }
                return PartialView("_EditUser", vm);
            }
        }

        [HttpPost]
        [CustomAuthorization]
        public ActionResult EditUser(string id, tbl_User tbl_User, string selectedItems)
        {
            using (var _context = new Entities())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {

                        if (!ModelState.IsValid)
                        {
                            UserRoleViewModel vm = new UserRoleViewModel()
                            {
                                RolesModel_list = _context.tbl_Role.Select(x => new RolesModel
                                {
                                    RoleId = x.RoleId,
                                    Name = x.Name,
                                }).ToList(),
                                _tbl_Unit = _context.tbl_Unit.ToList(),
                            };
                            return PartialView("_CreateUser", vm);
                        }
                        List<UserRoleModel> items = (new JavaScriptSerializer()).Deserialize<List<UserRoleModel>>(selectedItems);


                        if (tbl_User.Pno != null)
                        {
                            string password = string.Empty;

                            using (var newCOntext = new Entities())
                            {
                                var previousUser = newCOntext.tbl_User.FirstOrDefault(x => x.UserId == tbl_User.Pno);
                                password = previousUser.Password;
                            }

                            string pwd = password;
                            tbl_User.UserId = tbl_User.Pno;
                            tbl_User.Password = pwd;

                            _context.Entry(tbl_User).State = EntityState.Modified;
                            _context.SaveChanges();
                            _context.tbl_UserRole.RemoveRange(_context.tbl_UserRole.Where(x => x.UserId == id));
                            _context.SaveChanges();
                            if (items[0].RoleId != "")
                            {
                                foreach (var data in items)
                                {
                                    if (data != null)
                                    {
                                        var obj = new tbl_UserRole()
                                        {
                                            UserId = tbl_User.UserId,
                                            RoleId = data.RoleId,
                                            IsDefault = data.IsDefault
                                        };
                                        _context.tbl_UserRole.Add(obj);
                                    }
                                }
                                _context.SaveChanges();
                            }


                            #region User Injection

                            var UserName = tbl_User.UserId;

                            ApplicationUser result = UserManager.FindById(UserName);
                            // Was a password sent across?
                            if (result != null && !string.IsNullOrEmpty(tbl_User.Password))
                            {
                                // Remove current password
                                var removePassword = UserManager.RemovePassword(result.Id);
                                if (removePassword.Succeeded)
                                {
                                    // Add new password
                                    var AddPassword =
                                        UserManager.AddPassword(
                                            result.Id,
                                            tbl_User.Password
                                            );


                                    if (AddPassword.Succeeded)
                                    {
                                        transaction.Commit();
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        throw new Exception(AddPassword.Errors.FirstOrDefault());
                                    }
                                }
                                else
                                {
                                    transaction.Rollback();
                                    throw new Exception("Some error occured.");
                                }
                            }
                            else
                            {
                                transaction.Commit();
                            }

                            #endregion

                        }
                        return RedirectToAction("UserIndex");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return RedirectToAction("UserIndex");
                    }
                }
            }
        }

        [CustomAuthorization]
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView("_Delete");
        }

 
        [HttpPost]
        [CustomAuthorization]
 
        public ActionResult DeleteUser(string id, FormCollection collection)
        {
            using (var _context = new Entities())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }

                        var result = _context.tbl_User.Where(x => x.UserId == id).FirstOrDefault();
                        if (result != null)
                        {
 
                            result.IsDeleted = 1;

                            _context.tbl_User.Attach(result);
                            _context.Entry(result).Property(x => x.IsDeleted).IsModified = true;
                       
                            _context.SaveChanges();

                            #region User Injection 

                            ApplicationUser user = UserManager.FindById(id);
                            UserManager.Delete(user);

                            #endregion

                            transaction.Commit();
                        }
                        // Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                        return RedirectToAction("UserIndex");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        //  Exception(ex);
                        //   Alert("Their is something went wrong!!!", NotificationType.error);
                        return RedirectToAction("UserIndex");
                    }
                }
            }
        }


        [CheckUserSession]
        public ActionResult ChangePassword()
        {
            ViewBag.Msg = "";
            ChangePwdVM vm = new ChangePwdVM() { UserId = Session[SessionKeys.UserId]?.ToString() };
            return PartialView("_ChangePassword", vm);
        }

        [HttpPost]
        [CheckUserSession]
        public ActionResult ChangePassword(ChangePwdVM changePwd)
        {
            string actionName = "ChangePassword";

            var type = "error";
            string msg = string.Empty;

            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            try
            {
                if (ModelState.IsValid)
                {
                    #region User Injection

                    var UserName = changePwd.UserId;
                    var Password = changePwd.NewPassword;

                    if (changePwd.NewPassword == changePwd.ConfirmPassword)
                    {
                        using (var context = new Entities())
                        {
                            var user = context.tbl_User.FirstOrDefault(x => x.UserId == changePwd.UserId && x.IsDeleted != 1);
                            if (user != null)
                            {
                                if (user.Password == changePwd.OldPassword)
                                {
                                    var AdminUserCreateResult = UserManager.ChangePassword(changePwd.UserId, changePwd.OldPassword, changePwd.NewPassword);
                                    if (AdminUserCreateResult.Succeeded)
                                    {
                                        user.Password = changePwd.NewPassword;
                                        user.PasswordChangeDateTime = DateTime.Now;

                                        context.Entry(user).State = EntityState.Modified;
                                        context.SaveChanges();

                                        type = "success";
                                        msg = "Password changed successfully.";
                                    }
                                    else
                                    {
                                        msg = "Something went wrong.";
                                    }
                                }
                                else
                                {
                                    msg = "Incorrect password.";
                                }
                            }
                            else
                            {
                                msg = "User not found.";
                            }
                        }
                    }
                    else
                    {
                        msg = "New and old password mismatch.";
                    }
                    #endregion
                }
                else
                {
                    msg = "Please input valid data.";
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
                msg = "Some error occurred";
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            ViewBag.Msg = msg;

            if (type == "success")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return PartialView("_ChangePassword", changePwd);
            }
        }


        [HttpPost]
        [CustomAuthorization]
        public ActionResult ResetPassword(string userId)
        {
            string actionName = "ResetPassword";

            var type = "error";
            string msg = string.Empty;

            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            try
            {
                if (!String.IsNullOrWhiteSpace(userId))
                {
                    using (var context = new Entities())
                    {
                        var user = context.tbl_User.FirstOrDefault(x => x.UserId == userId && x.IsDeleted != 1);
                        if (user != null)
                        {
                            string newPassword = "Abc@" + user.Pno.Trim();

                            var AdminUserCreateResult = UserManager.ChangePassword(user.UserId, user.Password,newPassword);
                            if (AdminUserCreateResult.Succeeded)
                            {
                                user.Password = newPassword;
                                user.PasswordChangeDateTime = DateTime.Now;

                                context.Entry(user).State = EntityState.Modified;
                                context.SaveChanges();

                                type = "success";
                                msg = "Password reset successfully";

                                _logger.Log(LogLevel.Trace, actionName + " :: Pwd reset successfully for user id : " + user.UserId );

                            }
                            else
                            {
                                msg = "Unable to reset password";
                                _logger.Log(LogLevel.Trace, actionName + " :: Reset Pwd Failed for user : "  + user.UserId + "  :: Errors : " + AdminUserCreateResult.Errors.ToList());
                            }

                        }
                        else
                        {
                            msg = "User not found.";
                        }
                    }
                }
                else
                {
                    msg = "User id is empty";
                }
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
                msg = "Some error occurred";
            }


            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            return Json(new
            {
                msg = msg,
                type = type
            });

        }

        [CheckUserSession]
        private List<tbl_Role> GetActiveRoles()
        {
            using (_context = new Entities())
            {
                return _context.tbl_Role.Where(x => x.IsDeleted != 1).ToList();
            }
        }

        #endregion user
    }

}
