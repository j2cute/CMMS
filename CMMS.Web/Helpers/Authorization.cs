using CMMS.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace WebApplication.Helpers
{
    public class AuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                   || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                {
                    // Don't check for authorization as AllowAnonymous filter is applied to the action or controller  
                    return;
                }

                // Check for authorization  
                if (HttpContext.Current.Session[SessionKeys.UserId] == null)
                {
                    filterContext.Result = new RedirectResult("~/Account/Login");
                }
            }
            catch
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool result = false;
            try
            {
                var rolePermission = ((SessionHelper)httpContext.Session[SessionKeys.SessionHelperInstance]).RolePermissions;
                if (rolePermission != null)
                {
                    if (rolePermission.Any(x => x.URL == httpContext.Request.RawUrl.Split('?')[0]));
                    {
                        result = true;
                    }
                }
            }
            catch
            {

            }

            return result;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var httpContext = filterContext.HttpContext;
                var response = httpContext.Response;
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.SuppressFormsAuthenticationRedirect = true;
                response.End();
                base.HandleUnauthorizedRequest(filterContext);
            }
            else if (filterContext.HttpContext.Request.HttpMethod == "POST")
            {
                base.HandleUnauthorizedRequest(filterContext);
            }

            else
                base.HandleUnauthorizedRequest(filterContext);
        }
    }

}