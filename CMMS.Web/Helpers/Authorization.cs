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
    public class CustomAuthorizationAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var rolePermission = (List<ClassLibrary.Models.PermissionViewModel>)httpContext.Session[Constants.SessionPermission];
            if (rolePermission.Any(x => x.URL == httpContext.Request.RawUrl.Split('?')[0]))
            {
                return true;
            }
            else
            {
                return false;
            }
           // return base.AuthorizeCore(httpContext);
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


    public class AuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter  
   {  
       public override void OnAuthorization(AuthorizationContext filterContext)  
       {  
           if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)  
               || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))  
           {  
               // Don't check for authorization as AllowAnonymous filter is applied to the action or controller  
               return;  
           }  
  
           // Check for authorization  
           if (HttpContext.Current.Session["UserName"] == null)  
           { 


               filterContext.Result = new RedirectResult("~/Default/Index");  
           }  
       }  
   }  


    public class CustomAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["UserName"])))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                //Redirecting the user to the Login View of Account Controller  
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                     { "controller", "Account" },
                     { "action", "Login" }
                });
            }
        }
    }
}