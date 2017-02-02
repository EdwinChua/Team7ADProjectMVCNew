using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team7ADProjectMVC.Exceptions;
using Team7ADProjectMVC.Models;
using Team7ADProjectMVC.Models.ListAllRequisitionService;
using Team7ADProjectMVC.Models.UtilityService;
using Team7ADProjectMVC.Services.DepartmentService;
using Team7ADProjectMVC.Services.UtilityService;

namespace Team7ADProjectMVC.Controllers
{
    public class StationeryController : Controller
    {
        IInventoryService invService;
        IDepartmentService deptService;
        IRequisitionService reqService;
        IUtilityService uSvc;
        ProjectEntities db;
        public StationeryController()
        {
            invService = new InventoryService();
            db = new ProjectEntities();
            deptService = new DepartmentService();
            reqService = new RequisitionService();
            uSvc = new UtilityService();
        }
        // GET: Stationery
        public ActionResult DepartmentRequisitions(int? page, int? employeeId, string dateOrderedString)
        {
            Employee currentEmployee = (Employee)Session["User"];
            ViewBag.Employees = deptService.GetAllEmployeebyDepId(currentEmployee.DepartmentId);
            

            List<Requisition> resultList = reqService.GetAllRequisition(currentEmployee.DepartmentId);

            if(employeeId != null)
            {
                resultList.RemoveAll(x => x.EmployeeId != employeeId);
            }
            if (dateOrderedString != null && dateOrderedString.Length >1)
            {
                DateTime dateOrdered = uSvc.GetDateTimeFromPicker(dateOrderedString);
                resultList.RemoveAll(x => x.OrderedDate != dateOrdered);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(resultList.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult EmployeeRequisition()
        {
            Employee currentEmployee = (Employee)Session["User"];
            var requisition = new Requisition
            {
                OrderedDate = DateTime.Today,
                EmployeeId = currentEmployee.EmployeeId,
                DepartmentId = currentEmployee.DepartmentId,
                RequisitionStatus = Convert.ToString("Pending Approval")
            };
            var requisitionDetail = new RequisitionDetail();
            ViewBag.ItemNo = new SelectList(invService.GetAllInventory(), "ItemNo", "Description");

            requisition.RequisitionDetails.Add(requisitionDetail);
            return View(requisition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeRequisition([Bind] Requisition requisition)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var q = (from x in requisition.RequisitionDetails
                            orderby x.ItemNo
                            select x).ToList();

                    for (int i = 0; i< q.Count()-1; i++)
                    {
                        if(q[i].ItemNo == q[i+1].ItemNo)
                        {
                            throw new RequisitionCreationException("Please ensure the items are not duplicated.");
                        }

                    }

                    foreach (var item in requisition.RequisitionDetails)
                    {
                        item.OutstandingQuantity = item.Quantity;
                    }
                    db.Requisitions.Add(requisition);
                    db.SaveChanges();
                    return RedirectToAction("DepartmentRequisitions");
                }
            }
            catch (RequisitionCreationException e)
            {
                ViewBag.Error = e.Message.ToString();
            }


            ViewBag.ItemNo = new SelectList(invService.GetAllInventory(), "ItemNo", "Description");
            return View(requisition);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AddDetail()
        {
            Requisition currentRequisition = (Requisition)Session["requisition"];
            Session["requisition"] = new Requisition();
            var requisitionDetail = new RequisitionDetail();
            ViewBag.ItemNo = new SelectList(invService.GetAllInventory(), "ItemNo", "Description");
            currentRequisition.RequisitionDetails.Add(requisitionDetail);
            return View(currentRequisition);
        }

        public ActionResult Requisition(int id)
        {
            return View(reqService.FindById(id));
        }
    }
}