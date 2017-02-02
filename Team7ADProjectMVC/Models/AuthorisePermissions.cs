using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Team7ADProjectMVC.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorisePermissions : AuthorizeAttribute
    {
        public string Permission { get; set; }

        //Called when access is denied
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //User isn't logged in
            //filterContext.Result = new RedirectResult("~/Auth/Unauthorised");       
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not authorised to view the page. Please click on the back button to continue using the application");
            }     
            else filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        }

        //Core authentication, called before each action
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (((Employee)httpContext.Session["User"]).Role != null )
            {
                Employee e = (Employee)httpContext.Session["User"];
                List<string> permissions = Permission.Split(',').ToList<string>();
                foreach (string p in permissions)
                {
                    bool authorised = (bool)e.Role.GetType().GetProperty(p).GetValue(e.Role);
                    if (authorised)
                    {
                        return authorised;
                    }
                }               
                
            }
            return false;
        }

    }
}