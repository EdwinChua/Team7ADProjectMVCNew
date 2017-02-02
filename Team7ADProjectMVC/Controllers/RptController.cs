using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team7ADProjectMVC.Models;
using Team7ADProjectMVC.Models.ReportService;
using Team7ADProjectMVC.Services.DepartmentService;

namespace Team7ADProjectMVC.TestControllers
{
    public class RptController : Controller
    {
        private IReportService rptSvc = new ReportService();
        private IInventoryService invSvc = new InventoryService();
        private IDepartmentService  deptSvc= new DepartmentService();
        // GET: Rpt

        [AuthorisePermissions(Permission="ViewReports")]
        public ActionResult Index()
        {

            ViewBag.Departments = deptSvc.ListAllDepartments();
            ViewBag.Categories = invSvc.GetAllCategories();
            ViewBag.Months = rptSvc.GetMonthValues();
            ViewBag.Years = rptSvc.GetYearValues();

            return View("ItemDeptRpt");
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {

            List<YrMth> yrMthList = rptSvc.GetListOfYrMthFromUI(Request.Form["Year"], Request.Form["Month"], Request.Form["Year2"], Request.Form["Month2"], Request.Form["Year3"], Request.Form["Month3"]);
            List<string> depts = Request.Form["Departments"].Split(',').ToList<string>();
            String categorySelected = Request.Form["Categories"];
            DataView data = rptSvc.GetDataForDisbAnalysis(yrMthList, depts, categorySelected);
            Session["rptData"] = data;
            Session["rptPath"] = "~/Reports/CrystalReport1.rpt";
            return Redirect("ReportViewer.aspx");

        }

        // GET: Rpt/SupplierItem
        public ActionResult ItemSupplier()
        {
            ViewBag.Categories = invSvc.GetAllCategories();
            ViewBag.Months = rptSvc.GetMonthValues();
            ViewBag.Years = rptSvc.GetYearValues();

            return View("ItemSupplierRpt");
        }

        [HttpPost]
        public ActionResult ItemSupplier(FormCollection f)
        {
            String categorySelected = Request.Form["Categories"];
            List<YrMth> yrMthList = rptSvc.GetListOfYrMthFromUI(Request.Form["Year"], Request.Form["Month"], Request.Form["Year2"], Request.Form["Month2"], Request.Form["Year3"], Request.Form["Month3"]);

            DataView data = rptSvc.GetDataForSupplierAnalysis(yrMthList, categorySelected);
            Session["rptData"] = data;
            Session["rptPath"] = "~/Reports/CrystalReport2.rpt";
            return Redirect("/ReportViewer.aspx");

        }

        public ActionResult Stocklist()
        {

            DataView data = rptSvc.GetDataForStocklist();
            Session["rptData"] = data;
            Session["rptPath"] = "~/Reports/CrystalReport3.rpt";
            return Redirect("/ReportViewer.aspx");

        }
    }

  
}