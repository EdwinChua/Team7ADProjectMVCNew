using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team7ADProjectMVC.Exceptions;
using Team7ADProjectMVC.Models;
using Team7ADProjectMVC.Services;
using Team7ADProjectMVC.Services.DepartmentService;
using Team7ADProjectMVC.Services.SupplierService;

namespace Team7ADProjectMVC.TestControllers
{
    public class StoreController : Controller
    {
        private IInventoryService inventorySvc;
        private IDisbursementService disbursementSvc;
        private IDepartmentService deptSvc;
        private ISupplierAndPurchaseOrderService supplierAndPOSvc;

        public StoreController()
        {
            inventorySvc = new InventoryService();
            disbursementSvc = new DisbursementService();
            deptSvc = new DepartmentService();
            supplierAndPOSvc = new SupplierAndPurchaseOrderService();
        }

        //**************** INVENTORY ********************

        // GET: Store
        public ActionResult Index()
        {
            return View("Dashboard");
            //TODO: EDWIN - Create a nice dashboard or delete this
        }
        //Seq Diagram Done + Design Done
        public ActionResult Inventory(int? page, int? id) 
        {
            List<Inventory> inventories;
            try
            {
                inventories = inventorySvc.GetInventoryListByCategory((int)id);
            }
            catch (Exception e)
            {
                inventories = inventorySvc.GetAllInventory();
            }

            ViewBag.Cat = inventorySvc.GetAllCategories().ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View("ViewInventory",inventories.ToPagedList(pageNumber,pageSize));
        }
        //Seq Diagram Done  + Design Done
        public ActionResult InventoryItem(String id)
        {
            Inventory inventory = inventorySvc.FindInventoryItemById(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.inv = inventory;
            ViewBag.sCard = inventorySvc.GetStockCardFor(id);
            return View("ViewStockCard",inventory);
        }
        //Seq Diagram Done  + Design Done
        public ActionResult RetrievalList()
        {
            RetrievalList rList = inventorySvc.GetRetrievalList();
            ViewBag.RList = rList;
            return View("ViewRetrievalList");
        }
        //Seq Diagram Done  + Design Done
        public ActionResult MarkAsCollected(int collectedQuantity, string itemNo)
        {
            RetrievalList rList = inventorySvc.GetRetrievalList();
            inventorySvc.UpdateCollectionInfo(rList, collectedQuantity, itemNo);
            
            return RedirectToAction("RetrievalList");
        }
        //Seq Diagram Done  + Design Done
        public ActionResult New()
        {
            ViewBag.CategoryId = new SelectList(inventorySvc.GetAllCategories(), "CategoryId", "CategoryName");
            ViewBag.MeasurementId = new SelectList(inventorySvc.GetAllMeasurements(), "MeasurementId", "UnitOfMeasurement");
            ViewBag.SupplierId1 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode");
            ViewBag.SupplierId2 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode");
            ViewBag.SupplierId3 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode");
            return View("NewStockCard");
        }
        //Seq Diagram Done  + Design Done
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "ItemNo,CategoryId,Description,ReorderLevel,ReorderQuantity,MeasurementId,Quantity,HoldQuantity,SupplierId1,Price1,SupplierId2,Price2,SupplierId3,Price3,BinNo")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                if ((inventory.SupplierId1 != inventory.SupplierId2) && (inventory.SupplierId1 != inventory.SupplierId3) && (inventory.SupplierId2 != inventory.SupplierId3))
                {
                    inventory.ItemNo = inventorySvc.GetItemCode(inventory.Description);
                    inventorySvc.AddItem(inventory);
                    return RedirectToAction("Inventory");
                }
                else
                {
                    ViewBag.Error = "Please ensure that all three suppliers are different.";
                }
            }
            else
            {
                ViewBag.Error = "Please ensure that all three suppliers are different.";
            }

            ViewBag.CategoryId = new SelectList(inventorySvc.GetAllCategories(), "CategoryId", "CategoryName", inventory.CategoryId);
            ViewBag.MeasurementId = new SelectList(inventorySvc.GetAllMeasurements(), "MeasurementId", "UnitOfMeasurement", inventory.MeasurementId);
            ViewBag.SupplierId1 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode", inventory.SupplierId1);
            ViewBag.SupplierId2 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode", inventory.SupplierId2);
            ViewBag.SupplierId3 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode", inventory.SupplierId3);
            return View("NewStockCard");
        }
        //Seq Diagram Done  + Design Done
        // GET: Inventories/Edit/5
        public ActionResult Edit(string id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = inventorySvc.FindInventoryItemById(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(inventorySvc.GetAllCategories(), "CategoryId", "CategoryName", inventory.CategoryId);
            ViewBag.MeasurementId = new SelectList(inventorySvc.GetAllMeasurements(), "MeasurementId", "UnitOfMeasurement", inventory.MeasurementId);
            ViewBag.SupplierId1 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode", inventory.SupplierId1);
            ViewBag.SupplierId2 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode", inventory.SupplierId2);
            ViewBag.SupplierId3 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode", inventory.SupplierId3);
            ViewBag.inv = inventory;
            return View("UpdateStockCard",inventory);
        }
        //Seq Diagram Done  + Design Done
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemNo,CategoryId,Description,ReorderLevel,ReorderQuantity,MeasurementId,Quantity,HoldQuantity,SupplierId1,Price1,SupplierId2,Price2,SupplierId3,Price3,BinNo")] Inventory inventory) 
        {
            if (ModelState.IsValid)
            {
                inventorySvc.UpdateInventory(inventory);
                return RedirectToAction("Inventory");
            }
            ViewBag.CategoryId = new SelectList(inventorySvc.GetAllCategories(), "CategoryId", "CategoryName", inventory.CategoryId);
            ViewBag.MeasurementId = new SelectList(inventorySvc.GetAllMeasurements(), "MeasurementId", "UnitOfMeasurement", inventory.MeasurementId);
            ViewBag.SupplierId1 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode", inventory.SupplierId1);
            ViewBag.SupplierId2 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode", inventory.SupplierId2);
            ViewBag.SupplierId3 = new SelectList(inventorySvc.GetAllSuppliers(), "SupplierId", "SupplierCode", inventory.SupplierId3);
            ViewBag.inv = inventory;
            return View("UpdateStockCard",inventory);
        }
        //Seq Diagram Done + Design Done
        public ActionResult Search(int id, int? page) 
        {
            var inventories = inventorySvc.GetInventoryListByCategory(id);
            ViewBag.Cat = inventorySvc.GetAllCategories().ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View("ViewInventory", inventories.ToPagedList(pageNumber,pageSize));
        }

        //************** DISBURSEMENTS **************
        //Seq Diagram Done
        public ActionResult ViewDisbursements(int? page, int? id, String status)
        {
            List<DisbursementList> disbursementList;
            try
            {
                disbursementList = disbursementSvc.GetDisbursementsBySearchCriteria(id, status);
            }
            catch (Exception e)
            {
                disbursementList = disbursementSvc.GetAllDisbursements();
            }

            ViewBag.Departments = deptSvc.ListAllDepartments();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(disbursementList.ToPagedList(pageNumber, pageSize));
        }
        //Seq Diagram Done
        public ActionResult ViewDisbursement(int id)
        {
            DisbursementList dl = disbursementSvc.GetDisbursementById(id);
            ViewBag.disbursementListInfo = dl;
            ViewBag.Representative = deptSvc.FindEmployeeById((int)dl.Department.RepresentativeId);
            return View(dl);
        }
        //Seq Diagram Done
        public ActionResult SearchDisbursements(int? id, String status)
        { 
            ViewBag.Departments = deptSvc.ListAllDepartments();

            return View("ViewDisbursements", disbursementSvc.GetDisbursementsBySearchCriteria(id, status));
        }
        //Seq Diagram Done
        public ActionResult UpdateDisbursement(int disbursementListId, string[] itemNo, int[] originalPreparedQty, int[] adjustedQuantity, string[] remarks)
        {
            inventorySvc.UpdateDisbursementListDetails(disbursementListId, itemNo, originalPreparedQty, adjustedQuantity, remarks);
            return RedirectToAction("ViewDisbursements");
        }

        // ********************* MAINTAIN *******************
        //Seq Diagram Done
        public ActionResult SupplierList()
        {
            return View(supplierAndPOSvc.GetAllSuppliers());
        }
        //Seq Diagram Done
        public ActionResult Supplier(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = supplierAndPOSvc.FindSupplierById(id);
            List<Inventory> listOfItemsFromSupplier = supplierAndPOSvc.FindInventoryItemsBySupplier(id);
            ViewBag.SupplierItems = listOfItemsFromSupplier;
            ViewBag.SupplierId = supplier.SupplierId;
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }
        //Seq Diagram Done
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Supplier([Bind(Include = "SupplierId,SupplierCode,SupplierName,ContactName,PhNo,FaxNo,Address,GstRegistrationNo")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {

                supplierAndPOSvc.UpdateSupplier(supplier);
                return RedirectToAction("SupplierList");
            }
            return View("Supplier",supplier);
        }
        //Seq Diagram Done
        public ActionResult AddSupplier()
        {
            return View("Supplier");
        }
        //Seq Diagram Done
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSupplier([Bind(Include = "SupplierId,SupplierCode,SupplierName,ContactName,PhNo,FaxNo,Address,GstRegistrationNo")] Supplier supplier)
        {
                supplierAndPOSvc.AddNewSupplier(supplier);
                return RedirectToAction("SupplierList");
        }


        //****************** Outstanding Requisitions ***************
        //Seq Diagram Done  + Design Done
        public ActionResult ViewRequisitions()
        {
            RetrievalList rList = inventorySvc.GetRetrievalList();
            ViewBag.rList = rList;
            return View(inventorySvc.GetOutStandingRequisitions());
        }
        //Seq Diagram Done  + Design Done
        public ActionResult GenerateRetrievalList()
        {   
            inventorySvc.PopulateRetrievalList();
            inventorySvc.PopulateRetrievalListItems();
            return RedirectToAction("ViewRequisitions");
        }
        //Seq Diagram Done  + Design Done
        public ActionResult ClearRetrievalList()
        {
            inventorySvc.ClearRetrievalList();
            return RedirectToAction("ViewRequisitions");
        }
        //Seq Diagram Done
        public ActionResult DisburseItems()
        {
            inventorySvc.AutoAllocateDisbursementsByOrderOfRequisition();
            return RedirectToAction("ReallocateDisbursements");
        }
        //Seq Diagram Done
        public ActionResult ReallocateDisbursements()
        {
            List<DisbursementDetail> reallocationList = inventorySvc.GenerateListForManualAllocation();
            DisbursementListComparer comparer = new DisbursementListComparer(); //sort by item no
            reallocationList.Sort(comparer);
            int currentRetrievalListId = inventorySvc.GetLastRetrievalListId();
            List<Requisition> summedListByDepartment = inventorySvc.GetRequisitionsSummedByDept(currentRetrievalListId);
            ViewBag.MaxQuantityOfEachItem = summedListByDepartment;
            if (TempData["PrepQtyException"] != null)
            {
                ViewBag.PrepQtyException = TempData["PrepQtyException"].ToString();
            }  

            return View(reallocationList);
        }
        //Seq Diagram Done
        public ActionResult Reallocate(int[] departmentId, int[] preparedQuantity,int [] disbursementListId, int[] disbursementDetailId, string[] itemNo, int[] adjustedQuantity)
        {
            try
            {
                inventorySvc.ManuallyAllocateDisbursements(departmentId, preparedQuantity, adjustedQuantity, disbursementListId, disbursementDetailId, itemNo);
            }
            catch (InventoryAndDisbursementUpdateException e)
            {
                TempData["PrepQtyException"] = e;
            }

            return RedirectToAction("ReallocateDisbursements");
        }
    }
}