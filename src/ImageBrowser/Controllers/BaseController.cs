using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageBrowser.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            filterContext.Result = View("Error", filterContext.Exception);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Settings.Singleton.IsEmpty)
            {
                filterContext.Result = RedirectToAction("Index", "Setup");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}