using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team7ADProjectMVC.Models;
using Team7ADProjectMVC.Services;
using Team7ADProjectMVC.Services.DepartmentService;

namespace Team7ADProjectMVC.TestControllers
{
    public class RepresentativeController : Controller
    {
        private IDisbursementService disbursementSvc;
        private IDepartmentService departmentSvc;
        private ProjectEntities db = new ProjectEntities();


        public RepresentativeController()
        {
            disbursementSvc = new DisbursementService();
            departmentSvc = new DepartmentService();
        }
        // GET: Representative

        //[AuthorisePermissions(Permission="")]

        public ActionResult Viewdisbursements()
        {

            var id = ((Employee)Session["user"]).DepartmentId;
            var disbursementlist = disbursementSvc.GetDisbursementByDeptId(id);
            TempData["list"] = disbursementlist;

            return View(disbursementlist);


        }
        public ActionResult Searchdisbursements(string date, string status)
        {

            var disbursementlist = (List<DisbursementList>)TempData.Peek("list");
            return View("Viewdisbursements", disbursementSvc.FindDisbursementsBySearch(disbursementlist,date, status));
        }
        public ActionResult ViewDisbursementDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisbursementList dl = disbursementSvc.GetDisbursementById(id);
            if (dl == null)
            {
                return HttpNotFound();
            }
            ViewBag.DisbursementList = disbursementSvc.GetDisbursementById(id);
            ViewBag.Cpname = disbursementSvc.findCpnameByDisburse(id);
            ViewBag.Cptime = disbursementSvc.findCptimeByDisburse(id);
            ViewBag.status = disbursementSvc.findDisbursenmentStatus(id);
            return View(disbursementSvc.GetdisbursementdetailById(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewDisbursementDetail(int id)
        {

            disbursementSvc.ConfirmDisbursement(id);
            return RedirectToAction("Viewdisbursements");
        }



        public ActionResult Edit()
        {

            int id = (int)((Employee)Session["user"]).DepartmentId;
            Department department = departmentSvc.FindDeptById(id);
            ViewBag.Message = db.CollectionPoints.ToList();
            return View("ChangeCollectionPoint", department);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit([Bind(Include = "DepartmentId,CollectionPointId")] Department department)
        {

            if (ModelState.IsValid)
            {

                var rid = Request.Form["radio"];

                departmentSvc.changeDeptCp(department, int.Parse(rid));


                return RedirectToAction("Viewdisbursements");
            }
            ViewBag.Message = db.CollectionPoints.ToList();
            return View(department);

        }


    }
}