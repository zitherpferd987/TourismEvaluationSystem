using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourismEvaluationSystem.Models;

namespace TourismEvaluationSystem.Filters
{
    public class AdministratorControllerFilterAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Administrator admin = HttpContext.Current.Session["Admin"] as Administrator;
            if (admin == null)
            {
                HttpContext.Current.Response.Redirect("/Admin/Admin/AdminLogin");
            }
        }
    }
}