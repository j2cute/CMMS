using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using ClassLibrary.Models;
using static ClassLibrary.Common.Enums;
using ILS.UserManagement.Models;
using CMMS.Web.Helper;
using WebApplication.Helpers;

namespace WebApplication.Controllers
{
    [Authorization]
    public class AdminController : BaseController
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private WebAppDbContext db;
        // Controllers

        #region Dashboard
        public ActionResult UnitSelection()
        {
            db = new WebAppDbContext();
            UnitSelectionViewModel vm = new UnitSelectionViewModel
            {
                _tbl_Unit = db.tbl_Unit.ToList(),
            };
            //var roles = db.tbl_Unit.Select(x => new 
            //{
            //    id = x.Id,
            //    Name = x.Name,

            //}).ToList();

            //DashboardViewModel dashboard = new DashboardViewModel();
            //List<PermissionViewModel> ListPermissionViewModel = new List<PermissionViewModel>();
            //dashboard._tbl_Unit = db.tbl_Unit.ToList();
            //string defaultRoleId = !String.IsNullOrWhiteSpace(inRoleId) ? inRoleId : string.Empty;
            //var loginUserId = Session[SessionKeys.UserId]?.ToString();
            //dashboard.datetime = DateTime.Now.ToString();


            //using (Entities _context = new Entities())
            //{
            //    if (string.IsNullOrWhiteSpace(defaultRoleId))
            //    {
            //        defaultRoleId = _context.tbl_UserRole.Where(x => x.UserId == loginUserId && x.IsDefault == 1)?.FirstOrDefault().RoleId;
            //    }
            //    else
            //    {
            //        defaultRoleId = _context.tbl_UserRole.Where(x => x.UserId == loginUserId && x.RoleId == inRoleId)?.FirstOrDefault().RoleId;
            //    }

            //    if (!String.IsNullOrWhiteSpace(defaultRoleId))
            //    {
            //        var permissions = _context.tbl_RolePermission.Where(x => x.RoleId == defaultRoleId).ToList();

            //        foreach (var item in permissions)
            //        {
            //            PermissionViewModel permissionViewModel = new PermissionViewModel()
            //            {
            //                PermissionId = item.PermissionId,
            //                DisplayName = item.tbl_Permission.DisplayName,
            //                Level = item.tbl_Permission.PermissionLevel.ToString(),
            //                ParentId = item.tbl_Permission.ParentId,
            //                URL = item.tbl_Permission.URL
            //            };

            //            ListPermissionViewModel.Add(permissionViewModel);
            //        }
            //    }
            //    else
            //    {
            //        // Show some error
            //    }
            //}
            //Session[SessionKeys.SessionHelperInstance] = new SessionHelper(loginUserId, defaultRoleId, ListPermissionViewModel);
            return PartialView("_UnitSelection",vm);

        }

        [HttpPost]
        public ActionResult Dashboard(string siteId, string inRoleId = null)
        {
            db = new WebAppDbContext();
            DashboardViewModel dashboard = new DashboardViewModel();
            List<PermissionViewModel> ListPermissionViewModel = new List<PermissionViewModel>();
            dashboard._tbl_Unit = db.tbl_Unit.ToList();
            string defaultRoleId = !String.IsNullOrWhiteSpace(inRoleId) ? inRoleId : string.Empty;
            var loginUserId = Session[SessionKeys.UserId]?.ToString();
            dashboard.datetime = DateTime.Now.ToString();
 
            using (Entities _context = new Entities())
            {
                if (string.IsNullOrWhiteSpace(defaultRoleId))
                {
                    defaultRoleId = _context.tbl_UserRole.Where(x => x.UserId == loginUserId && x.IsDefault == 1)?.FirstOrDefault().RoleId;
                }
                else
                {
                    defaultRoleId = _context.tbl_UserRole.Where(x => x.UserId == loginUserId && x.RoleId == inRoleId)?.FirstOrDefault().RoleId;
                }

                if (!String.IsNullOrWhiteSpace(defaultRoleId))
                {
                    var permissions = _context.tbl_RolePermission.Where(x => x.RoleId == defaultRoleId).ToList();

                    foreach (var item in permissions)
                    {
                        PermissionViewModel permissionViewModel = new PermissionViewModel()
                        {
                            PermissionId = item.PermissionId,
                            DisplayName = item.tbl_Permission.DisplayName,
                            Level = item.tbl_Permission.PermissionLevel.ToString(),
                            ParentId = item.tbl_Permission.ParentId,
                            URL = item.tbl_Permission.URL
                        };

                        ListPermissionViewModel.Add(permissionViewModel);
                    }
                }
                else
                {
                    // Show some error
                }
            }

            Session[SessionKeys.SessionHelperInstance] = new SessionHelper(siteId,loginUserId, defaultRoleId, ListPermissionViewModel);

            return View(dashboard);

        }

        #endregion

        #region UserCheck

        public JsonResult CheckUserName(string UserName)
        {
            using (WebAppDbContext db = new WebAppDbContext())
            {
                System.Threading.Thread.Sleep(200);
                var SearchData = db.AspNetUsers.Where(x => x.UserName == UserName).SingleOrDefault();
                if (SearchData != null)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }

            }
        }
        #endregion UserCheck


        #region public ActionResult Index(string searchStringUserNameOrEmail)
        public ActionResult Index()
        {
            try
            {
                using (WebAppDbContext db = new WebAppDbContext())
                {
                    var abc = db.AspNetUsers.ToList();
                    return View(abc);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                List<ExpandedUserDTO> col_UserDTO = new List<ExpandedUserDTO>();
                return View(col_UserDTO);
            }
        }
        #endregion

        // Users *****************************

        // GET: /Admin/Edit/Create 

        #region public ActionResult Create()
        public ActionResult Create()
        {
            ExpandedUserDTO objExpandedUserDTO = new ExpandedUserDTO();

            ViewBag.Roles = GetAllRolesAsSelectList();

            return View(objExpandedUserDTO);
        }
        #endregion

        // PUT: /Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult Create(ExpandedUserDTO paramExpandedUserDTO)
        public ActionResult Create(ExpandedUserDTO paramExpandedUserDTO)
        {
            try
            {
                if (paramExpandedUserDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                paramExpandedUserDTO.Email = "example@gmail.com";

                if (!ModelState.IsValid)
                {
                    //var errors = ModelState.Keys.Where(k => ModelState[k].Errors.Count > 0).Select(k => new
                    //{
                    //    propertyName = k,
                    //    errorMessage = ModelState[k].Errors[0].ErrorMessage
                    //});
                    ViewBag.Roles = GetAllRolesAsSelectList();
                    return View();
                }

                //var UserName = paramExpandedUserDTO.UserName.Trim();
                //var Password = paramExpandedUserDTO.Password.Trim();

                var UserName = paramExpandedUserDTO.UserName.ToLower();
                var Password = paramExpandedUserDTO.Password.Trim();


                var objNewAdminUser = new ApplicationUser { UserName = UserName, Email = UserName };
                var AdminUserCreateResult = UserManager.Create(objNewAdminUser, Password);

                if (AdminUserCreateResult.Succeeded == true)
                {
                    string strNewRole = Convert.ToString(Request.Form["Roles"]);

                    if (strNewRole != "0")
                    {
                        // Put user in role
                        UserManager.AddToRole(objNewAdminUser.Id, strNewRole);

                    }
                    Alert("User created successfully!!!", NotificationType.success);
                    return Redirect("~/Admin/Index");
                }


                else
                {
                    ViewBag.Roles = GetAllRolesAsSelectList();
                    ModelState.AddModelError(string.Empty,
                        "Error: Failed to create the user. Check requirements.");
                    return View(paramExpandedUserDTO);
                }
            }
            catch (Exception)
            {
                ViewBag.Roles = GetAllRolesAsSelectList();
                ModelState.AddModelError(string.Empty,
                    "Error: Failed to create the user. Check requirements.");
                return View("Create");
            }
        }
        #endregion

        // GET: /Admin/Edit/TestUser 
        #region public ActionResult EditUser(string UserName)
        public ActionResult EditUser(string UserName, string Email)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExpandedUserDTO objExpandedUserDTO = GetUser(UserName, Email);
            if (objExpandedUserDTO == null)
            {
                return HttpNotFound();
            }
            return View(objExpandedUserDTO);
        }
        #endregion

        // PUT: /Admin/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult EditUser(ExpandedUserDTO paramExpandedUserDTO)
        public ActionResult EditUser(ExpandedUserDTO paramExpandedUserDTO)
        {
            try
            {
                if (paramExpandedUserDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (paramExpandedUserDTO.Password == null)
                {
                    return View(paramExpandedUserDTO);
                }
                ExpandedUserDTO objExpandedUserDTO = UpdateDTOUser(paramExpandedUserDTO);

                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }
                Alert("Password updated successfully!!!", NotificationType.success);
                return Redirect("~/Admin/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(paramExpandedUserDTO.UserName, paramExpandedUserDTO.Email));
            }
        }
        #endregion

        // DELETE: /Admin/DeleteUser

        #region public ActionResult DeleteUser(string UserName)
        public ActionResult DeleteUser(string UserName, string Email)
        {
            try
            {
                if (UserName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (UserName.ToLower() == this.User.Identity.Name.ToLower())
                {
                    ModelState.AddModelError(
                        string.Empty, "Error: Cannot delete the current user");

                    Redirect("~/Admin/Index");
                }

                ExpandedUserDTO objExpandedUserDTO = GetUser(UserName, Email);

                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    DeleteUser(objExpandedUserDTO);
                    Alert("User deleted successfully!!!", NotificationType.success);
                }

                return Redirect("~/Admin/Index");
            }
            catch (Exception)
            {
                Alert("User cannot be deleted, because system using this information!!!", NotificationType.error);
                //ModelState.AddModelError(string.Empty, "Error: User can not be deleted, because system using this information" + ex);
                return Redirect("~/Admin/Index");
            }
        }
        #endregion

        // GET: /Admin/EditRoles/TestUser 

        #region ActionResult EditRoles(string UserName)
        public ActionResult EditRoles(string UserName, string Email)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserName = UserName.ToLower();

            // Check that we have an actual user
            ExpandedUserDTO objExpandedUserDTO = GetUser(UserName, Email);

            if (objExpandedUserDTO == null)
            {
                return HttpNotFound();
            }

            UserAndRolesDTO objUserAndRolesDTO =
                GetUserAndRoles(UserName);

            return View(objUserAndRolesDTO);
        }
        #endregion

        // PUT: /Admin/EditRoles/TestUser 

        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult EditRoles(UserAndRolesDTO paramUserAndRolesDTO)
        public ActionResult EditRoles(UserAndRolesDTO paramUserAndRolesDTO)
        {
            try
            {
                if (paramUserAndRolesDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string UserName = paramUserAndRolesDTO.UserName;
                string strNewRole = Convert.ToString(Request.Form["AddRole"]);

                if (strNewRole != "No Roles Found")
                {
                    // Go get the User
                    ApplicationUser user = UserManager.FindByName(UserName);

                    // Put user in role
                    UserManager.AddToRole(user.Id, strNewRole);
                }

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesDTO objUserAndRolesDTO =
                    GetUserAndRoles(UserName);

                return View(objUserAndRolesDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditRoles");
            }
        }
        #endregion

        // DELETE: /Admin/DeleteRole?UserName="TestUser&RoleName=Administrator

        #region public ActionResult DeleteRole(string UserName, string RoleName)
        public ActionResult DeleteRole(string UserName, string RoleName, string Email)
        {
            try
            {
                if ((UserName == null) || (RoleName == null))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserName = UserName.ToLower();
                Email = Email.ToLower();
                // Check that we have an actual user
                ExpandedUserDTO objExpandedUserDTO = GetUser(UserName, Email);

                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }

                if (UserName.ToLower() ==
                    this.User.Identity.Name.ToLower() && RoleName == "Admin")
                {
                    ModelState.AddModelError(string.Empty,
                        "Error: Cannot delete Admin Role for the current user");
                }

                // Go get the User
                ApplicationUser user = UserManager.FindByName(UserName);
                // Remove User from role
                UserManager.RemoveFromRoles(user.Id, RoleName);
                UserManager.Update(user);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                return RedirectToAction("EditRoles", new { Email = Email, UserName = UserName });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesDTO objUserAndRolesDTO =
                    GetUserAndRoles(UserName);

                return View("EditRoles", objUserAndRolesDTO);
            }
        }
        #endregion

        // Roles *****************************

        // GET: /Admin/ViewAllRoles

        #region public ActionResult ViewAllRoles()
        public ActionResult ViewAllRoles()
        {
            var roleManager =
                new RoleManager<IdentityRole>
                (
                    new RoleStore<IdentityRole>(new ApplicationDbContext())
                    );

            List<RoleDTO> colRoleDTO = (from objRole in roleManager.Roles
                                        select new RoleDTO
                                        {
                                            Id = objRole.Id,
                                            RoleName = objRole.Name
                                        }).ToList();

            return View(colRoleDTO);
        }
        #endregion

        // GET: /Admin/AddRole

        #region public ActionResult AddRole()
        public ActionResult AddRole()
        {
            RoleDTO objRoleDTO = new RoleDTO();

            return View(objRoleDTO);
        }
        #endregion

        // PUT: /Admin/AddRole

        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult AddRole(RoleDTO paramRoleDTO)
        public ActionResult AddRole(RoleDTO paramRoleDTO)
        {
            try
            {
                if (paramRoleDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (!ModelState.IsValid)
                {
                    return View(paramRoleDTO);
                }
                var RoleName = paramRoleDTO.RoleName.Trim();

                if (RoleName == "")
                {
                    throw new Exception("No Role Name");
                }

                // Create Role
                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext())
                        );

                if (!roleManager.RoleExists(RoleName))
                {
                    roleManager.Create(new IdentityRole(RoleName));
                }
                Alert("Role created successfully!!!", NotificationType.success);
                return Redirect("~/Admin/ViewAllRoles");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: Failed to create the role. Check requirements.");
                return View("AddRole");
            }
        }
        #endregion

        // DELETE: /Admin/DeleteUserRole?RoleName=TestRole

        #region public ActionResult DeleteUserRole(string RoleName)
        public ActionResult DeleteUserRole(string RoleName)
        {
            try
            {
                if (RoleName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (RoleName.ToLower() == "admin")
                {
                    throw new Exception(String.Format("Cannot delete {0} Role.", RoleName));
                }

                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));

                var UsersInRole = roleManager.FindByName(RoleName).Users.Count();
                if (UsersInRole > 0)
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role because it still has users.",
                            RoleName)
                            );

                }

                var objRoleToDelete = (from objRole in roleManager.Roles
                                       where objRole.Name == RoleName
                                       select objRole).FirstOrDefault();
                if (objRoleToDelete != null)
                {
                    roleManager.Delete(objRoleToDelete);
                }
                else
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role does not exist.",
                            RoleName)
                            );
                }

                List<RoleDTO> colRoleDTO = (from objRole in roleManager.Roles
                                            select new RoleDTO
                                            {
                                                Id = objRole.Id,
                                                RoleName = objRole.Name
                                            }).ToList();

                return View("ViewAllRoles", colRoleDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));

                List<RoleDTO> colRoleDTO = (from objRole in roleManager.Roles
                                            select new RoleDTO
                                            {
                                                Id = objRole.Id,
                                                RoleName = objRole.Name
                                            }).ToList();

                return View("ViewAllRoles", colRoleDTO);
            }
        }
        #endregion


        // Utility

        #region public ApplicationUserManager UserManager
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
        #endregion

        #region public ApplicationRoleManager RoleManager
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        #region private List<SelectListItem> GetAllRolesAsSelectList()
        private List<SelectListItem> GetAllRolesAsSelectList()
        {
            List<SelectListItem> SelectRoleListItems =
                new List<SelectListItem>();

            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new ApplicationDbContext()));

            var colRoleSelectList = roleManager.Roles.OrderBy(x => x.Name).ToList();

            SelectRoleListItems.Add(
                new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });

            foreach (var item in colRoleSelectList)
            {
                SelectRoleListItems.Add(
                    new SelectListItem
                    {
                        Text = item.Name.ToString(),
                        Value = item.Name.ToString()
                    });
            }

            return SelectRoleListItems;
        }
        #endregion

        #region private ExpandedUserDTO GetUser(string paramUserName)
        private ExpandedUserDTO GetUser(string paramUserName, string paramEmail)
        {
            ExpandedUserDTO objExpandedUserDTO = new ExpandedUserDTO();

            var result = UserManager.FindByName(paramEmail);

            // If we could not find the user, throw an exception
            if (result == null) throw new Exception("Could not find the User");

            objExpandedUserDTO.UserName = result.UserName;
            objExpandedUserDTO.Email = result.Email;
            //objExpandedUserDTO.LockoutEndDateUtc = result.LockoutEndDateUtc;
            //objExpandedUserDTO.AccessFailedCount = result.AccessFailedCount;
            //objExpandedUserDTO.PhoneNumber = result.PhoneNumber;

            return objExpandedUserDTO;
        }
        #endregion

        #region private ExpandedUserDTO UpdateDTOUser(ExpandedUserDTO objExpandedUserDTO)
        private ExpandedUserDTO UpdateDTOUser(ExpandedUserDTO paramExpandedUserDTO)
        {
            ApplicationUser result =
                UserManager.FindByName(paramExpandedUserDTO.Email);

            // If we could not find the user, throw an exception
            if (result == null)
            {
                throw new Exception("Could not find the User");
            }
            paramExpandedUserDTO.Email = paramExpandedUserDTO.UserName;
            result.UserName = paramExpandedUserDTO.UserName;
            result.Email = paramExpandedUserDTO.Email;

            // Lets check if the account needs to be unlocked
            if (UserManager.IsLockedOut(result.Id))
            {
                // Unlock user
                UserManager.ResetAccessFailedCountAsync(result.Id);
            }

            UserManager.Update(result);

            // Was a password sent across?
            if (!string.IsNullOrEmpty(paramExpandedUserDTO.Password))
            {
                // Remove current password
                var removePassword = UserManager.RemovePassword(result.Id);
                if (removePassword.Succeeded)
                {
                    // Add new password
                    var AddPassword =
                        UserManager.AddPassword(
                            result.Id,
                            paramExpandedUserDTO.Password
                            );

                    if (AddPassword.Errors.Count() > 0)
                    {
                        throw new Exception(AddPassword.Errors.FirstOrDefault());
                    }
                }
            }

            return paramExpandedUserDTO;
        }
        #endregion

        #region private void DeleteUser(ExpandedUserDTO paramExpandedUserDTO)
        private void DeleteUser(ExpandedUserDTO paramExpandedUserDTO)
        {
            ApplicationUser user =
                UserManager.FindByName(paramExpandedUserDTO.UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
            UserManager.Update(user);
            UserManager.Delete(user);
        }
        #endregion

        #region private UserAndRolesDTO GetUserAndRoles(string UserName)
        private UserAndRolesDTO GetUserAndRoles(string UserName)
        {
            // Go get the User
            ApplicationUser user = UserManager.FindByName(UserName);

            List<UserRoleDTO> colUserRoleDTO =
                (from objRole in UserManager.GetRoles(user.Id)
                 select new UserRoleDTO
                 {
                     RoleName = objRole,
                     UserName = UserName
                 }).ToList();

            if (colUserRoleDTO.Count() == 0)
            {
                colUserRoleDTO.Add(new UserRoleDTO { RoleName = "No Roles Found" });
            }

            ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

            // Create UserRolesAndPermissionsDTO
            UserAndRolesDTO objUserAndRolesDTO =
                new UserAndRolesDTO();
            objUserAndRolesDTO.UserName = UserName;
            objUserAndRolesDTO.colUserRoleDTO = colUserRoleDTO;
            return objUserAndRolesDTO;
        }
        #endregion

        #region private List<string> RolesUserIsNotIn(string UserName)
        private List<string> RolesUserIsNotIn(string UserName)
        {
            // Get roles the user is not in
            var colAllRoles = RoleManager.Roles.Select(x => x.Name).ToList();

            // Go get the roles for an individual
            ApplicationUser user = UserManager.FindByName(UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            var colRolesForUser = UserManager.GetRoles(user.Id).ToList();
            var colRolesUserInNotIn = (from objRole in colAllRoles
                                       where !colRolesForUser.Contains(objRole)
                                       select objRole).ToList();

            if (colRolesUserInNotIn.Count() == 0)
            {
                colRolesUserInNotIn.Add("No Roles Found");
            }

            return colRolesUserInNotIn;
        }
        #endregion

    }
}