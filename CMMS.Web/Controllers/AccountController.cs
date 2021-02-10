using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ClassLibrary.Models;
using CMMS.Web.Helper;
using WebApplication.Helpers;
using ILS.UserManagement.Models;
using System.Collections.Generic;
using NLog;

namespace WebApplication.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        private static Logger _logger;

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                _logger.Log(LogLevel.Trace, "Login attempted for : " + model.UserName);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                result = SignInStatus.Success;
               
                switch (result)
                {
                    case SignInStatus.Success:
                        Session[SessionKeys.UserId] = model.UserName;
                        return RedirectToAction("UnitSelection", "Admin");

                    case SignInStatus.Failure:
                        return View("Login","Account");

                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        //
        // POST: /Account/LogOff
 
        [Authorization]
        public ActionResult LogOff()
        {
            string actionName = "LogOff";
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");

                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                Session.Clear();
                Session.RemoveAll();
                Session.Abandon();

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();

                _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException.ToString());
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [Authorization]
        public ActionResult SwitchRole(string roleId = "")
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(roleId))
                {
                    LoadRole(roleId);
                }
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }

            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Dashboard", "Admin", new { inRoleId = roleId });
            return Json(new { Url = redirectUrl });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect(returnUrl);
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion


        // Add RoleManager
        #region public ApplicationRoleManager RoleManager
        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        private void LoadRole(string roleId)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(roleId))
                {
                    List<PermissionViewModel> ListPermissionViewModel = new List<PermissionViewModel>();

                    using (Entities _context = new Entities())
                    {
                        var loginId = Session[SessionKeys.UserId].ToString();
                        var currentRole = _context.tbl_UserRole.Where(x => x.UserId == loginId && x.RoleId == roleId)?.FirstOrDefault().RoleId;

                        if (!string.IsNullOrWhiteSpace(currentRole))
                        {
                            var permissions = _context.tbl_RolePermission.Where(x => x.RoleId == currentRole).ToList();

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

                            Session[SessionKeys.SessionHelperInstance] = ((SessionHelper)Session[SessionKeys.SessionHelperInstance]).UpdateFields(roleId, ListPermissionViewModel);
                        }
                        else
                        {
                            // Show some error
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
        }

        private void GetUserRole()
        {
            IEnumerable<tbl_Role> roles = new List<tbl_Role>();
            try
            {
                using (Entities _context = new Entities())
                {
                    roles =  _context.tbl_User.FirstOrDefault(x => x.UserId == Session[SessionKeys.UserId].ToString() && x.IsActive != 0)?
                                               .tbl_UserRole.Select(x => x.tbl_Role).Where(x=>x.IsDeleted != 1);

                    if(roles.Any())
                    {

                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error," Exception :: " + ex.ToString());
            }
        }
    }
}