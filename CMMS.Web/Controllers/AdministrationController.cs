using ClassLibrary.Common;
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

namespace WebApplication.Controllers
{
    [Authorization]
    public class AdministrationController : Controller
    {
        #region roles
        private Entities _context = new Entities();
        // GET: Roles
        public ActionResult ViewRoles()
        {
            RolesViewModel vm = new RolesViewModel()
            {
                tbl_Role_list = _context.tbl_Role.ToList(),
            };

            return View(vm);
        }

        // GET: Roles/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            RolesViewModel vm = new RolesViewModel();

            List<TreeViewNode> nodesMaster = new List<TreeViewNode>();
            foreach (tbl_Permission p in _context.tbl_Permission)
            {
                //State checkState = new State();
                //if (p.PermissionId == "ViewRole")
                //{
                //    checkState.selected = true;
                //}
                if (p.ParentId == "0") { p.ParentId = "#"; }
                nodesMaster.Add(new TreeViewNode { id = p.PermissionId, parent = p.ParentId, text = p.DisplayName });
            }
            //Serialize to string.
            vm.PermissionsList = (new JavaScriptSerializer()).Serialize(nodesMaster);
            return PartialView("_CreateRole", vm);
        }

        // POST: Roles/Create
        [HttpPost]
        public ActionResult Create(tbl_Role tbl_Role, string selectedItems)
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

        // GET: Roles/Edit/5
        public ActionResult Edit(string id)
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

        // POST: Roles/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, tbl_Role tbl_Role, string selectedItems)
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

        // GET: Roles/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Roles/Delete/5
        [HttpPost]
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
        public ActionResult UserIndex()
        {
            UserRoleViewModel vm = new UserRoleViewModel()
            {
                tbl_User_list = _context.tbl_User.ToList(),

            };
            return View(vm);
        }

        [Authorization]
        public ActionResult CreateUser()
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

        [HttpGet]
        public ActionResult GetRoles()
        {
            var roles = _context.tbl_Role.Select(x => new RolesModel
            {
                RoleId = x.RoleId,
                Name = x.Name,

            }).ToList();

            return new JsonResult() { Data = new { roles = roles }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [HttpPost]
        public ActionResult CreateUser(tbl_User tbl_User, string selectedItems)
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
                        tbl_User.UserId = tbl_User.Pno;

                        _context.tbl_User.Add(tbl_User);
                        _context.SaveChanges();

                        foreach (var data in items)
                        {
                            if (data != null)
                            {
                                if (!String.IsNullOrWhiteSpace(data.RoleId))
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

                        }
                    }
                    _context.SaveChanges();

                    #region User Injection

                    var UserName = tbl_User.UserId;
                    var Password = tbl_User.Password.Trim();


                    var objNewAdminUser = new ApplicationUser { UserName = tbl_User.UserId,Email = tbl_User.UserId };
                    var AdminUserCreateResult = UserManager.Create(objNewAdminUser, Password);
                    if(AdminUserCreateResult.Succeeded)
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


        public ActionResult EditUser(string id)
        {
            UserRoleViewModel vm = new UserRoleViewModel();
            var unit = _context.tbl_Unit.ToList();
            var allRoles = _context.tbl_Role.Select(x => new RolesModel
            {
                RoleId = x.RoleId,
                Name = x.Name,
            }).ToList();

            //  List<RolesModel> roles = new List<RolesModel>();
            var result = _context.tbl_User.Where(x => x.UserId == id).FirstOrDefault();
            if (result.UserId != null)
            {
                var userRole = _context.tbl_UserRole.Where(x => x.UserId == id).Select(x => new UserRoleModel
                {
                    RoleId = x.RoleId,
                    UserId = x.UserId,
                }).ToList();
                vm._tbl_Unit = unit;
                vm.RolesModel_list = allRoles;
                vm.tbl_User = result;
                vm.userRoleModel_list = userRole;

            }
            return PartialView("_EditUser", vm);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        public ActionResult EditUser(string id, tbl_User tbl_User, string selectedItems)
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
                        tbl_User.UserId = tbl_User.Pno;

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

                        ApplicationUser result = UserManager.FindByEmail(UserName);
                        // Was a password sent across?
                        if (result!=null && !string.IsNullOrEmpty(tbl_User.Password))
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


                                if(AddPassword.Succeeded)
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


        // GET: Parts/Delete/id
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView("_Delete");
        }

        // POST: Parts/Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(string id, FormCollection collection)
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
                       
                        result.IsActive = 0;

                        _context.tbl_User.Attach(result);
                        _context.Entry(result).Property(x => x.IsActive).IsModified = true;

                        _context.SaveChanges();

                        #region User Injection 

                        ApplicationUser user = UserManager.FindByEmail(id);
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

        #endregion user
    }

}
