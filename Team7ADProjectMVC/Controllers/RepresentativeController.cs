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
using PagedList;

namespace Team7ADProjectMVC.TestControllers
{
    public class RepresentativeController : Controller
    {
        private IDisbursementService disbursementSvc;
        private IDepartmentService departmentSvc;
       


        public RepresentativeController()
        {
            disbursementSvc = new DisbursementService();
            departmentSvc = new DepartmentService();
        }
        // GET: Representative

        [AuthorisePermissions(Permission="ConfirmDisbursement")]
        public ActionResult Viewdisbursements(int? page)
        {

            var id = ((Employee)Session["user"]).DepartmentId;
            var disbursementlist = disbursementSvc.GetDisbursementByDeptId(id);
            TempData["list"] = disbursementlist;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(disbursementlist.ToPagedList(pageNumber,pageSize));


        }

        [AuthorisePermissions(Permission = "ConfirmDisbursement")]
        public ActionResult Searchdisbursements(int? page,string date, string status)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var disbursementlist = (List<DisbursementList>)TempData.Peek("list");
            return View("Viewdisbursements", disbursementSvc.FindDisbursementsBySearch(disbursementlist,date, status).ToPagedList(pageNumber,pageSize));
        }

        [AuthorisePermissions(Permission = "ConfirmDisbursement")]
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
        [AuthorisePermissions(Permission = "ConfirmDisbursement")]
        public ActionResult ViewDisbursementDetail(int id)
        {

            disbursementSvc.ConfirmDisbursement(id);
            return RedirectToAction("Viewdisbursements");
        }


        [AuthorisePermissions(Permission = "ChangeCollectionPoint")]
        public ActionResult Edit()
        {

            int id = (int)((Employee)Session["user"]).DepartmentId;
            Department department = departmentSvc.FindDeptById(id);
            ViewBag.Message = departmentSvc .getAllCollectionPoint();
            return View("ChangeCollectionPoint", department);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorisePermissions(Permission = "ChangeCollectionPoint")]
        public ActionResult Edit([Bind(Include = "DepartmentId,CollectionPointId")] Department department)
        {

            if (ModelState.IsValid)
            {

                var rid = Request.Form["radio"];

                departmentSvc.changeDeptCp(department, int.Parse(rid));


                return RedirectToAction("Edit");
            }
            ViewBag.Message = departmentSvc .getAllCollectionPoint();
            return View(department);

        }


    }
}