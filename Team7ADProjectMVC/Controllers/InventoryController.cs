using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team7ADProjectMVC;
using Team7ADProjectMVC.Models;

namespace Team7ADProjectMVC.Controllers
{
    public class InventoryController : Controller
    {
        private ProjectEntities db = new ProjectEntities();
        private InventoryService invSvc = new InventoryService();

        // GET: Inventory
        public ActionResult Index()
        {
            var inventories = db.Inventories.Include(i => i.Category).Include(i => i.Measurement).Include(i => i.Supplier).Include(i => i.Supplier1).Include(i => i.Supplier2);
            return View(inventories.ToList());
        }

        // GET: Inventory/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // GET: Inventory/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(invSvc.GetAllCategories(), "CategoryId", "CategoryName");
            ViewBag.MeasurementId = new SelectList(invSvc.GetAllMeasurements(), "MeasurementId", "UnitOfMeasurement");
            ViewBag.SupplierId1 = new SelectList(invSvc.GetAllSuppliers(), "SupplierId", "SupplierName");
            ViewBag.SupplierId2 = new SelectList(invSvc.GetAllSuppliers(), "SupplierId", "SupplierName");
            ViewBag.SupplierId3 = new SelectList(invSvc.GetAllSuppliers(), "SupplierId", "SupplierName");
            return View();
        }

        // POST: Inventory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemNo,CategoryId,Description,ReorderLevel,ReorderQuantity,MeasurementId,Quantity,HoldQuantity,SupplierId1,Price1,SupplierId2,Price2,SupplierId3,Price3,BinNo")] Inventory inventory)
        {
            inventory.ItemNo = invSvc.GetItemCode(inventory.Description);
            inventory.Quantity = 0;
            if (ModelState.IsValid)
            {
                db.Inventories.Add(inventory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(invSvc.GetAllCategories(), "CategoryId", "CategoryName");
            ViewBag.MeasurementId = new SelectList(invSvc.GetAllMeasurements(), "MeasurementId", "UnitOfMeasurement");
            ViewBag.SupplierId1 = new SelectList(invSvc.GetAllSuppliers(), "SupplierId", "SupplierName");
            ViewBag.SupplierId2 = new SelectList(invSvc.GetAllSuppliers(), "SupplierId", "SupplierName");
            ViewBag.SupplierId3 = new SelectList(invSvc.GetAllSuppliers(), "SupplierId", "SupplierName");
            return View(inventory);
        }

        // GET: Inventory/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(invSvc.GetAllCategories(), "CategoryId", "CategoryName");
            ViewBag.MeasurementId = new SelectList(invSvc.GetAllMeasurements(), "MeasurementId", "UnitOfMeasurement");
            ViewBag.SupplierId1 = new SelectList(invSvc.GetAllSuppliers(), "SupplierId", "SupplierName");
            ViewBag.SupplierId2 = new SelectList(invSvc.GetAllSuppliers(), "SupplierId", "SupplierName");
            ViewBag.SupplierId3 = new SelectList(invSvc.GetAllSuppliers(), "SupplierId", "SupplierName");
            return View(inventory);
        }

        // POST: Inventory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemNo,CategoryId,Description,ReorderLevel,ReorderQuantity,MeasurementId,Quantity,HoldQuantity,SupplierId1,Price1,SupplierId2,Price2,SupplierId3,Price3,BinNo")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", inventory.CategoryId);
            ViewBag.MeasurementId = new SelectList(db.Measurements, "MeasurementId", "UnitOfMeasurement", inventory.MeasurementId);
            ViewBag.SupplierId1 = new SelectList(db.Suppliers, "SupplierId", "SupplierCode", inventory.SupplierId1);
            ViewBag.SupplierId2 = new SelectList(db.Suppliers, "SupplierId", "SupplierCode", inventory.SupplierId2);
            ViewBag.SupplierId3 = new SelectList(db.Suppliers, "SupplierId", "SupplierCode", inventory.SupplierId3);
            return View(inventory);
        }

        // GET: Inventory/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Inventory inventory = db.Inventories.Find(id);
            db.Inventories.Remove(inventory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
