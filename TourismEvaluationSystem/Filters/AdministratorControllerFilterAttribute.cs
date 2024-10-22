﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourismEvaluationSystem.Models;

namespace TourismEvaluationSystem.Filters
{
    public class TouristControllerFilterAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Tourists tourist = HttpContext.Current.Session["Tourist"] as Tourists;
            if (tourist == null)
            {
                HttpContext.Current.Response.Redirect("/Tourist/Tourist/TouristLogin");
            }
        }
    }
}