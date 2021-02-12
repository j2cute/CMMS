using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Helpers;
using static ClassLibrary.Common.Enums;

namespace WebApplication.Controllers
{
    [CustomAuthorization]
    public class BaseController : Controller
    {
        private WebAppDbContext db = new WebAppDbContext();
        private tbl_ErrorLogs ex = new tbl_ErrorLogs();
        // GET: Base
        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal('" + notificationType.ToString().ToUpper() + "', '" + message + "','" + notificationType + "')" + "</script>";
            TempData["notification"] = msg;
        }

        /// <summary>
        /// Sets the information for the system notification.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="notifyType">The type of notification to display to the user: Success, Error or Warning.</param>
        public void Message(string message, NotificationType notifyType)
        {
            TempData["Notification2"] = message;

            switch (notifyType)
            {
                case NotificationType.success:
                    TempData["NotificationCSS"] = "alert-box success";
                    break;
                case NotificationType.error:
                    TempData["NotificationCSS"] = "alert-box errors";
                    break;
                case NotificationType.warning:
                    TempData["NotificationCSS"] = "alert-box warning";
                    break;

                case NotificationType.info:
                    TempData["NotificationCSS"] = "alert-box notice";
                    break;
            }
        }

        // Save Exception log in database
        public void Exception(Exception errorLogs)
        {
            ex.Message = errorLogs.Message?.ToString();
            ex.StackTrace = errorLogs.StackTrace?.ToString();
            ex.ExceptionType = errorLogs.GetType()?.Name?.ToString();
            ex.LogTime = DateTime.Now;
            db.tbl_ErrorLogs.Add(ex);
            db.SaveChanges();
        }
    }
}