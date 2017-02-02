using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Team7ADProjectMVC.Models;
using Team7ADProjectMVC.Models.InventoryAdjustmentService;
using Team7ADProjectMVC.Services.DepartmentService;

namespace Team7ADProjectMVC.Controllers
{
    public class AdjustmentsController : Controller
    {
        private IInventoryAdjustmentService ivadjustsvc;
        private IDepartmentService deptSvc;
        private IInventoryService invSvc;
        
        public AdjustmentsController()
        {
            ivadjustsvc = new InventoryAdjustmentService();
            deptSvc = new DepartmentService();
            invSvc = new InventoryService();
        }

        // GET: Adjustments
        public ActionResult ViewAdjustment(int? page)
        {
            Employee user = (Employee)Session["user"];

            string role = ivadjustsvc.findRolebyUserID(user.EmployeeId);
            List<Employee> empList = deptSvc.GetAllEmployeebyDepId(user.DepartmentId);
            ViewBag.employee = new SelectList(empList, "EmployeeId", "EmployeeName");

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (role == "Store Supervisor")
            {
                List<SelectListItem> statuslist = new List<SelectListItem>()
                {
                    new SelectListItem {Text ="Pending Approval"},
                    new SelectListItem {Text ="Approved" },
                    new SelectListItem {Text ="Rejected" },
                };

                ViewBag.status = statuslist;
                var adjustmentlist = ivadjustsvc.findSupervisorAdjustmentList();
                TempData["list"] = adjustmentlist;
                return View(adjustmentlist.ToPagedList(pageNumber, pageSize));
            }

            if (role == "Store Manager")
            {
                List<SelectListItem> statuslist = new List<SelectListItem>()
                {
                    new SelectListItem {Text ="Pending Final Approval" },
                    new SelectListItem {Text ="Approved"},
                    new SelectListItem {Text ="Rejected"},
                };

                ViewBag.status = statuslist;
                var adjustmentlist = ivadjustsvc.findManagerAdjustmentList();
                TempData["list"] = adjustmentlist;
                return View(adjustmentlist.ToPagedList(pageNumber, pageSize));
            }



            return View();
        }

        public ActionResult SearchAdjustment(string employee, string status, string date, int? page)
        {

            Employee user = ((Employee)Session["user"]);

            string role = ivadjustsvc.findRolebyUserID(user.EmployeeId);
            List<Employee> empList = deptSvc.(user.DepartmentId);
            ViewBag.employee = new SelectList(empList, "EmployeeId", "EmployeeName");
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (role == "Store Supervisor")
            {
                //var adjustmentlist = ivadjustsvc.findSupervisorAdjustmentList();
                var adjustmentlist = (List<Adjustment>)TempData.Peek("list");

                var result = ivadjustsvc.FindAdjustmentBySearch(adjustmentlist, employee, status, date);


                List<SelectListItem> statuslist = new List<SelectListItem>()
                {
                    new SelectListItem {Text ="Pending Approval"},
                    new SelectListItem {Text ="Approved" },
                    new SelectListItem {Text ="Rejected" },
                };

                ViewBag.status = statuslist;
                return View("ViewAdjustment", result.ToPagedList(pageNumber,pageSize));
            }

            if (role == "Store Supervisor")
            {
                //var adjustmentlist = ivadjustsvc.findManagerAdjustmentList();
                var adjustmentlist = (List<Adjustment>)TempData.Peek("list");
                var result = ivadjustsvc.FindAdjustmentBySearch(adjustmentlist, employee, status, date);

                List<SelectListItem> statuslist = new List<SelectListItem>()
                {
                    new SelectListItem {Text ="Pending Final Approval" },
                    new SelectListItem {Text ="Approved"},
                    new SelectListItem {Text ="Rejected"},
                };

                ViewBag.status = statuslist;
                return View("ViewAdjustment", result.ToPagedList(pageNumber, pageSize));
            }

            return View();


        }

        public ActionResult ViewAdjustmentDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adjustment adjustment = ivadjustsvc.findAdjustmentByID(id);
            if (adjustment == null)
            {
                return HttpNotFound();
            }
            List<AdjustmentDetail> dtlist = ivadjustsvc.findDetailByAdjustment(adjustment);
            decimal? total = ivadjustsvc.caculateTotal(dtlist);
            ViewBag.Adjid = id;
            ViewBag.status = ivadjustsvc.findAdjustmentStatus(id);
            ViewBag.sum = total;
            return View(dtlist);
        }


        public ActionResult SupervisorApprove(int? id)
        {
            int empid = ((Employee)Session["user"]).EmployeeId;
            ivadjustsvc.ApproveBySupervisor(empid, id);

            return RedirectToAction("ViewAdjustment");
        }


        public ActionResult SupervisorRejecct(int? id)
        {
            int empid = ((Employee)Session["user"]).EmployeeId;
            ivadjustsvc.RejecteBySupervisor(empid, id);
            return RedirectToAction("ViewAdjustment");
        }

        public ActionResult SupervisorPending(int? id)
        {
            int empid = ((Employee)Session["user"]).EmployeeId;
            ivadjustsvc.PendingBySupervisor(empid, id);

            return RedirectToAction("ViewAdjustment");
        }

        public ActionResult ManagerApprove(int? id)
        {
            int empid = ((Employee)Session["user"]).EmployeeId;
            ivadjustsvc.ApproveByManager(empid, id);
            return RedirectToAction("ViewAdjustment");
        }

        public ActionResult ManagerRejecct(int? id)
        {
            int empid = ((Employee)Session["user"]).EmployeeId;
            ivadjustsvc.RejectByManager(empid, id);
            return RedirectToAction("ViewAdjustment");
        }


        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        [HttpGet]
        public ActionResult Create()
        {
            Employee currentEmployee = (Employee)Session["User"];
            var adjust = new Adjustment
            {
                AdjustmentDate = DateTime.Today,
                EmployeeId = currentEmployee.EmployeeId,
                Status = Convert.ToString("Pending Approval")
            };
            var adjustdetail = new AdjustmentDetail();
            ViewBag.ItemNo = new SelectList(invSvc.GetAllInventory(), "ItemNo", "Description");

            adjust.AdjustmentDetails.Add(adjustdetail);
            return View(adjust);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind] Adjustment adjust)
        {
            if (ModelState.IsValid)
            {
                ivadjustsvc.createAdjustment(adjust);

                return RedirectToAction("Index");
            }
            ViewBag.ItemNo = new SelectList(invSvc.GetAllInventory(), "ItemNo", "Description");
            return View(adjust);
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AddDetail()
        {
            Adjustment currentAdjustment = (Adjustment)Session["adjustment"];
            Session["adjustment"] = new Adjustment();
            var adjustdetail = new AdjustmentDetail();
            ViewBag.ItemNo = new SelectList(invSvc.GetAllInventory(), "ItemNo", "Description");
            currentAdjustment.AdjustmentDetails.Add(adjustdetail);
            return View(currentAdjustment);
        }

    }
}

