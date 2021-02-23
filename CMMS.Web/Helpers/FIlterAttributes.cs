using CMMS.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace WebApplication.Helpers
{
    public class ValidateAjaxAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                return;

            var modelState = filterContext.Controller.ViewData.ModelState;
            if (!modelState.IsValid)
            {
                var errorModel =
                        from x in modelState.Keys
                        where modelState[x].Errors.Count > 0
                        select new
                        {
                            key = x,
                            errors = modelState[x].Errors.
                                                          Select(y => y.ErrorMessage).
                                                          ToArray()
                        };
                filterContext.Result = new JsonResult()
                {
                    Data = errorModel
                };
                filterContext.HttpContext.Response.StatusCode =
                                                      (int)HttpStatusCode.BadRequest;
            }
        }
    }


    public class ValidateLoadActions : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool result = false;
            try
            {
        
                var rolePermission = ((SessionHelper)filterContext.HttpContext.Session[SessionKeys.SessionHelperInstance]).CurrentRolePermissions;
                if (rolePermission != null)
                {

                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {

                        var requestParts = filterContext.HttpContext.Request.UrlReferrer.LocalPath.Split('?')[0]?.Split('/');

                        string url = "/" + (!String.IsNullOrWhiteSpace(requestParts[0]) ? requestParts[0] : requestParts[1]) + "/" +
                                           (!String.IsNullOrWhiteSpace(requestParts[0]) ? requestParts[1] : requestParts[2]);

                        if (rolePermission.Any(x => !String.IsNullOrWhiteSpace(x.URL) && x.URL.Contains(url)))
                        {
                            result = true;
                        }
                    }
                    else
                    {

                        var requestParts = filterContext.HttpContext.Request.UrlReferrer.LocalPath.Split('?')[0]?.Split('/');

                        // Previous was a ajax Request
                        if (requestParts.Length > 3)
                        {
                            string url = "/" + (!String.IsNullOrWhiteSpace(requestParts[0]) ? requestParts[0] : requestParts[1]) + "/" +
                                        (!String.IsNullOrWhiteSpace(requestParts[0]) ? requestParts[1] : requestParts[2]);

                            if (rolePermission.Any(x => !String.IsNullOrWhiteSpace(x.URL) && x.URL.Contains(url)))
                            {
                                result = true;
                            }
                        }
                        else
                        {

                            if (rolePermission.Any(x => x.URL == filterContext.HttpContext.Request.RawUrl.Split('?')[0]))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            if(!result)
            {
                filterContext.HttpContext.Response.StatusCode =
                                      (int)HttpStatusCode.BadRequest;
            }
        }


    }
}