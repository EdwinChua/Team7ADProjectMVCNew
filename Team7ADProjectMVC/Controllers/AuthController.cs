using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Team7ADProjectMVC.Services.DepartmentService;

namespace Team7ADProjectMVC.Controllers
{
    public class AuthController : Controller
    {
        ProjectEntities db = new ProjectEntities();
        IDepartmentService deptSvc = new DepartmentService();
        // GET: Auth
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = Int32.Parse(User.Identity.Name);
                Employee e= db.Employees.Find(userId);
                Session["user"] = e;
                if (deptSvc.IsDelegate(e))
                {
                    deptSvc.SetDelegatePermissions(e);
                    Session["user"] = e;
                    return Redirect(Url.Content("~/Head/ApproveRequisition")); //If delegated, do not redirect to Make Requisition use case
                }

                


                switch (e.Role.Name)
                {
                    case "Store Clerk":
                    case "Store Representative":
                    case "Store Supervisor":
                        return Redirect(Url.Content("~/Store/ViewRequisitions"));
                    case "Department Head":
                    case "Store Manager":
                        return Redirect(Url.Content("~/Head/ApproveRequisition"));
                    case "Employee":
                    case "Representative":
                        return Redirect(Url.Content("~/Department/MakeRequisition"));
                    default:
                        return Redirect(Url.Content("~/Login.aspx"));
                }
            }
            
            else return Redirect(Url.Content("~/Login.aspx"));

        }

        public ActionResult Logout()
        {
            Session["user"] = new Employee();
            FormsAuthentication.SignOut();
            return Redirect(Url.Content("~/Login.aspx"));
        }
    }
}