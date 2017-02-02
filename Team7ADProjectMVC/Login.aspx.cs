using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Team7ADProjectMVC.Services.DepartmentService;

namespace Team7ADProjectMVC
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["ReturnUrl"] != null && IsPostBack)
            {
                string userId = Request.Form["Login1$UserName"];
                string userPw = Request.Form["Login1$Password"];
                bool authenticated=Membership.ValidateUser(userId, userPw);
                if (authenticated)
                {
                    FormsAuthentication.SetAuthCookie(userId, false);
                    Session["ReturnUrl"] = Request.QueryString["ReturnUrl"];
                    Response.Redirect("~/Auth");
                }
                              
            }


        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
        }

    }
}