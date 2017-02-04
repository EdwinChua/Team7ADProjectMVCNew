using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Team7ADProjectMVC.Models;
using Team7ADProjectMVC.Models.InventoryAdjustmentService;
using Team7ADProjectMVC.Models.UtilityService;
using Team7ADProjectMVC.Services.DepartmentService;

namespace Team7ADProjectMVC.Controllers
{
    public class AdjustmentsController : Controller
    {
        private IInventoryAdjustmentService ivadjustsvc;
        private IDepartmentService deptSvc;
        private IInventoryService invSvc;
        private UtilityService uSvc;
        
        public AdjustmentsController()
        {
            ivadjustsvc = new InventoryAdjustmentService();
            deptSvc = new DepartmentService();
            invSvc = new InventoryService();
            uSvc = new UtilityService();
        }

        // GET: Adjustments
        [AuthorisePermissions(Permission = "MakeAdjustment,ApproveAdjustment")]
        public ActionResult ViewAdjustment(int? page)
        {
            Employee user = (Employee)Session["user"];

            string role = ivadjustsvc.findRolebyUserID(user.EmployeeId);
            List<Employee> empList = deptSvc.GetEverySingleEmployeeInDepartment (user.DepartmentId);
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

        [AuthorisePermissions(Permission = "MakeAdjustment,ApproveAdjustment")]
        public ActionResult SearchAdjustment(string employee, string status, string date, int? page)
        {

            Employee user = ((Employee)Session["user"]);

            string role = ivadjustsvc.findRolebyUserID(user.EmployeeId);
            List<Employee> empList = deptSvc.GetEverySingleEmployeeInDepartment(user.DepartmentId);
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

            if (role == "Store Manager")
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

        [AuthorisePermissions(Permission = "MakeAdjustment,ApproveAdjustment")]
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

        [AuthorisePermissions(Permission = "ApproveAdjustment")]
        public ActionResult SupervisorApprove(int? id)
        {
            int empid = ((Employee)Session["user"]).EmployeeId;
            ivadjustsvc.ApproveBySupervisor(empid, id);

            return RedirectToAction("ViewAdjustment");
        }

        [AuthorisePermissions(Permission = "ApproveAdjustment")]
        public ActionResult SupervisorRejecct(int? id)
        {
            int empid = ((Employee)Session["user"]).EmployeeId;
            ivadjustsvc.RejecteBySupervisor(empid, id);
            return RedirectToAction("ViewAdjustment");
        }

        [AuthorisePermissions(Permission = "ApproveAdjustment")]
        public ActionResult SupervisorPending(int? id)
        {
            int empid = ((Employee)Session["user"]).EmployeeId;
            ivadjustsvc.PendingBySupervisor(empid, id);
            try //email to notify manager of approval
            {
                List<Employee> storeManagement = deptSvc.GetStoreManagerAndSupervisor();
                string emailBody = storeManagement.Where(x => x.RoleId == 6).First().EmployeeName + ", you have a new pending inventory adjustment for approval. Please go to http://" + Request.Url.Host + ":23130//Adjustments/ViewAdjustmentDetail/" + id + " to approve the adjustment.";
                uSvc.SendEmail(new List<string>(new string[] { storeManagement.Where(x => x.RoleId == 6).First().Email }), "New Inventory Adjustment Pending Approval", emailBody);
            }
            catch (Exception ex)
            {
            }

            return RedirectToAction("ViewAdjustment");
        }

        [AuthorisePermissions(Permission = "ApproveAdjustment")]
        public ActionResult ManagerApprove(int? id)
        {
            int empid = ((Employee)Session["user"]).EmployeeId;
            ivadjustsvc.ApproveByManager(empid, id);
            return RedirectToAction("ViewAdjustment");
        }

        [AuthorisePermissions(Permission = "ApproveAdjustment")]
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
        [AuthorisePermissions(Permission = "MakeAdjustment")]
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
        [AuthorisePermissions(Permission = "MakeAdjustment")]
        public ActionResult Create([Bind] Adjustment adjust)
        {
            if (ModelState.IsValid)
            {
                ivadjustsvc.createAdjustment(adjust);
                try //email to notify supervisor of approval
                {
                    List<Employee> storeManagement = deptSvc.GetStoreManagerAndSupervisor();
                    string emailBody = storeManagement.Where(x => x.RoleId == 5).First().EmployeeName + ", you have a new pending inventory adjustment for approval. Please go to http://" + Request.Url.Host + ":23130//Adjustments/ViewAdjustmentDetail/" + adjust.AdjustmentId + " to approve the adjustment.";
                    uSvc.SendEmail(new List<string>(new string[] { storeManagement.Where(x => x.RoleId == 5).First().Email }), "New Inventory Adjustment Pending Approval", emailBody);
                }
                catch (Exception ex)
                {
                }
                return RedirectToAction("Index");
            }
            
            ViewBag.ItemNo = new SelectList(invSvc.GetAllInventory(), "ItemNo", "Description");
            return View(adjust);
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [AuthorisePermissions(Permission = "MakeAdjustment")]
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

