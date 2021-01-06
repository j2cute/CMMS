using CMMS.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace WebApplication.Helpers
{
    public class CustomAuthenticationFilter : IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            try
            {
                if (filterContext.HttpContext.Session[SessionKeys.UserId] != null)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(filterContext.HttpContext.Session[SessionKeys.UserId])))
                    {
                        filterContext.Result = new HttpUnauthorizedResult();
                    }
                }
            }
            catch
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