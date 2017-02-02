using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team7ADProjectMVC.Models;
using Team7ADProjectMVC.Models.UtilityService;
using Team7ADProjectMVC.Services;
using Team7ADProjectMVC.Services.DepartmentService;
using Team7ADProjectMVC.Services.SupplierService;
using Team7ADProjectMVC.Services.UtilityService;

namespace Team7ADProjectMVC.Controllers
{
    public class StorePOController : Controller
    {
        private IInventoryService inventorySvc;
        private IDisbursementService disbursementSvc;
        private IDepartmentService deptSvc;
        private ISupplierAndPurchaseOrderService supplierAndPOSvc;
        private IUtilityService utilSvc;
        
        public StorePOController()
        {
            inventorySvc = new InventoryService();
            disbursementSvc = new DisbursementService();
            deptSvc = new DepartmentService();
            supplierAndPOSvc = new SupplierAndPurchaseOrderService();
            utilSvc = new UtilityService();
        }
        // seq diagram done
        [AuthorisePermissions(Permission = "MakePurchaseOrder")]
        public ActionResult GeneratePO()
        {
            List<Inventory> itemsToResupply = supplierAndPOSvc.GetAllItemsToResupply();
            return View(itemsToResupply);
        }
        // seq diagram done
        [AuthorisePermissions(Permission = "MakePurchaseOrder")]
        public ActionResult GeneratePurchaseOrders(string[] itemNo, int[] supplier, int?[] orderQuantity)
        {
            Employee currentEmployee = (Employee)Session["User"];
            supplierAndPOSvc.GeneratePurchaseOrders(currentEmployee,itemNo, supplier, orderQuantity);
            List<Inventory> itemsToResupply = supplierAndPOSvc.GetAllItemsToResupply();
            return RedirectToAction("PurchaseOrderSummary");
        }
        // seq diagram done
        //[AuthorisePermissions(Permission = "MakePurchaseOrder")] both
        public ActionResult PurchaseOrderSummary()
        {
            List<PurchaseOrder> poList = supplierAndPOSvc.GetAllPOOrderByApproval();
            
            return View(poList);
        }
        // seq diagram done
        [AuthorisePermissions(Permission = "MakePurchaseOrder,ApprovePurchaseOrder")]
        public ActionResult SearchPurchaseOrderSummary(string orderStatus, string dateOrderedString, string dateApprovedString)
        {
            DateTime? dateOrdered = null;
            DateTime? dateApproved = null;
            int resultCount = 0;
            if (dateOrderedString != null && dateOrderedString.Count() > 1)
            {
                dateOrdered = utilSvc.GetDateTimeFromPicker(dateOrderedString);
            }
            if (dateApprovedString != null && dateApprovedString.Count() > 1)
            {
                dateApproved = utilSvc.GetDateTimeFromPicker(dateApprovedString);
            }

            List <PurchaseOrder> poList = supplierAndPOSvc.SearchPurchaseOrders(orderStatus, dateOrdered, dateApproved, out resultCount);
            ViewBag.ResultCount = resultCount;
            return View("PurchaseOrderSummary", poList);
        }
        // seq diagram done
        [AuthorisePermissions(Permission = "MakePurchaseOrder,ApprovePurchaseOrder")]
        public ActionResult DeliveryDetails(int id)
        {
            List<DeliveryDetail> deliveryDetailsList = supplierAndPOSvc.GetDeliveryDetailsByDeliveryId(id);
            Delivery delivery = supplierAndPOSvc.FindDeliveryById(id);
            ViewBag.DeliveryDetailsList = deliveryDetailsList;
            return View("ViewReceiveOrder",delivery);
        }
        // seq diagram done
        [AuthorisePermissions(Permission = "MakePurchaseOrder,ApprovePurchaseOrder")]
        public ActionResult PurchaseOrder(int id, int? page)
        {
            PurchaseOrder purchaseOrder = supplierAndPOSvc.FindPOById(id);
            ViewBag.PurchaseOrder = purchaseOrder;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(purchaseOrder.PurchaseDetails.ToPagedList(pageNumber, pageSize));
        }
        // seq diagram done
        [AuthorisePermissions(Permission = "ApprovePurchaseOrder")]
        public ActionResult ApprovePO(int poNumber, string approve)
        {
            if(approve=="Approve")
            {
                approve = "Approved";
            } else
            {
                approve = "Rejected";
            }
            Employee currentEmployee = (Employee)Session["User"];
            supplierAndPOSvc.ApprovePurchaseOrder(currentEmployee, poNumber, approve);
            return RedirectToAction("PurchaseOrderSummary");
        }
        // seq diagram done
        [AuthorisePermissions(Permission = "InventoryManagement")]
        public ActionResult ListDeliveries()
        {
            List<Delivery> allDeliveries = supplierAndPOSvc.GetAllDeliveries();
            return View(allDeliveries);
        }
        // seq diagram done
        [AuthorisePermissions(Permission = "InventoryManagement")]
        public ActionResult AcceptDelivery(int deliveryId, string deliveryRefNo, string dateDelivered, int[] deliveryDetailId, string[] itemNo, int[] quantity, string[] remarks)
        {
            Employee currentEmployee = (Employee)Session["User"];
            supplierAndPOSvc.ReceiveDelivery(currentEmployee, deliveryId, deliveryRefNo, dateDelivered, deliveryDetailId, itemNo, quantity, remarks);

            return RedirectToAction("ListDeliveries");
        }

    }

}