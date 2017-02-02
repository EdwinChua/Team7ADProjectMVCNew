using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team7ADProjectMVC;

namespace Team7ADProjectMVC.TestControllers
{
    public class DisbursementListsController : Controller
    {
        private ProjectEntities db = new ProjectEntities();

        // GET: DisbursementLists
        public ActionResult Index()
        {
            var disbursementLists = db.DisbursementLists.Include(d => d.CollectionPoint).Include(d => d.Department).Include(d => d.Retrieval);
            return View(disbursementLists.ToList());
        }

        // GET: DisbursementLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisbursementList disbursementList = db.DisbursementLists.Find(id);
            if (disbursementList == null)
            {
                return HttpNotFound();
            }
            return View(disbursementList);
        }

        // GET: DisbursementLists/Create
        public ActionResult Create()
        {
            ViewBag.CollectionPointId = new SelectList(db.CollectionPoints, "CollectionPointId", "PlaceName");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode");
            ViewBag.RetrievalId = new SelectList(db.Retrievals, "RetrievalId", "RetrievalId");
            return View();
        }

        // POST: DisbursementLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DisbursementListId,RetrievalId,DepartmentId,OrderedDate,CollectionPointId,Status,DeliveryDate")] DisbursementList disbursementList)
        {
            if (ModelState.IsValid)
            {
                db.DisbursementLists.Add(disbursementList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CollectionPointId = new SelectList(db.CollectionPoints, "CollectionPointId", "PlaceName", disbursementList.CollectionPointId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", disbursementList.DepartmentId);
            ViewBag.RetrievalId = new SelectList(db.Retrievals, "RetrievalId", "RetrievalId", disbursementList.RetrievalId);
            return View(disbursementList);
        }

        // GET: DisbursementLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisbursementList disbursementList = db.DisbursementLists.Find(id);
            if (disbursementList == null)
            {
                return HttpNotFound();
            }
            ViewBag.CollectionPointId = new SelectList(db.CollectionPoints, "CollectionPointId", "PlaceName", disbursementList.CollectionPointId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", disbursementList.DepartmentId);
            ViewBag.RetrievalId = new SelectList(db.Retrievals, "RetrievalId", "RetrievalId", disbursementList.RetrievalId);
            return View(disbursementList);
        }

        // POST: DisbursementLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DisbursementListId,RetrievalId,DepartmentId,OrderedDate,CollectionPointId,Status,DeliveryDate")] DisbursementList disbursementList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(disbursementList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CollectionPointId = new SelectList(db.CollectionPoints, "CollectionPointId", "PlaceName", disbursementList.CollectionPointId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", disbursementList.DepartmentId);
            ViewBag.RetrievalId = new SelectList(db.Retrievals, "RetrievalId", "RetrievalId", disbursementList.RetrievalId);
            return View(disbursementList);
        }

        // GET: DisbursementLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisbursementList disbursementList = db.DisbursementLists.Find(id);
            if (disbursementList == null)
            {
                return HttpNotFound();
            }
            return View(disbursementList);
        }

        // POST: DisbursementLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DisbursementList disbursementList = db.DisbursementLists.Find(id);
            db.DisbursementLists.Remove(disbursementList);
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
