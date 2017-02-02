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
using Team7ADProjectMVC.Models.ListAllRequisitionService;
using Team7ADProjectMVC.Services.DepartmentService;

namespace Team7ADProjectMVC.TestControllers
{

    public class PersonModel
    {
        public string Name { get; set; }
        public string Depart { get; set; }
        public List<RoleModel> Roles { get; set; }
        public List<ItemModel> Items { get; set; }

        
    }
    public class ItemModel
    {
        public string Item { get; set; }
        public string Quantity { get; set; }
    }

    public class RoleModel
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
   

    public class DepartmentController : Controller
    {
        public class PersonModel
        {
            public string Name { get; set; }
            public string Depart { get; set; }
            public List<RoleModel> Roles { get; set; }
            public List<ItemModel> Items { get; set; }


        }
        public class ItemModel
        {
            public string Item { get; set; }
            public string Quantity { get; set; }
        }

        public class RoleModel
        {
            public string RoleName { get; set; }
            public string Description { get; set; }
        }


        private static PersonModel mododo;
        private IRequisitionService listsvc;
        private IDepartmentService depasvc;
        private IInventoryService invsvc;
        private static string gsearchString;
        PushNotification notify = new PushNotification(); 
       
        public DepartmentController()
        {         
            listsvc = new RequisitionService();
            depasvc = new DepartmentService();
            invsvc = new InventoryService();
            gsearchString = "";
            //mododo = new PersonModel();
        }
        
        public ActionResult Index(string sortOrder/*,int pages=4*/)
        {
            var requisitions = listsvc.ListAllRequisition();


            gsearchString = "";



            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            //ViewBag.NameSortParm = sortOrder == "Name_desc" ? "Name_desc" : "Name";
            ViewBag.emeSortParm = String.IsNullOrEmpty(sortOrder) ? "e_desc" : "";

            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.remeSortParm = String.IsNullOrEmpty(sortOrder) ? "s_desc" : "";

            if (Session["npg"] == null || Session["npg"].Equals("4"))
            {
                Session["npg"] = 4;

            }
            //else {


            //    Session["npg"] = pages;

            //}

            
            //Session["UserName"]="joe";
            if (Session["searchstr"] != null){
                gsearchString = Session["searchstr"].ToString();

            }
            



           





            var re = listsvc.ListAllRequisition().AsQueryable();
            if (sortOrder != null)
            {
                switch (sortOrder)
                {
                    case "Name_desc":
                        re = re.OrderByDescending(s => s.Employee.Department.DepartmentName);
                        break;
                    case "e_desc":
                        re = re.OrderBy(s => s.Employee.EmployeeName);
                        break;
                    case "Date":
                        re = re.OrderByDescending(s => s.ApprovedDate);
                        break;
                    default:
                        re = re.OrderBy(s => s.RequisitionStatus);
                        break;
                }


                if (!String.IsNullOrEmpty(gsearchString))
                {
                    var q = re.Where(s => s.Employee.EmployeeName.Contains(gsearchString)
                                           || s.ApprovedDate.ToString().Contains(gsearchString)
                                           || s.Employee.Department.DepartmentName.Contains(gsearchString));
                    requisitions = q.ToList();
                }
                else {

                    requisitions = re.ToList();

                }


               
            }
            //if (sortOrder != null)
            if (sortOrder == null) {

                if (!String.IsNullOrEmpty(gsearchString))
                {
                    var q = re.Where(s => s.Employee.EmployeeName.Contains(gsearchString)
                                           || s.ApprovedDate.ToString().Contains(gsearchString)
                                           || s.Employee.Department.DepartmentName.Contains(gsearchString));
                    requisitions = q.ToList();
                }
                else
                {

                    requisitions = re.ToList();

                }


            }


            Employee userName = (Employee)Session["User"];

            requisitions.RemoveAll(x => x.DepartmentId != userName.DepartmentId);


            //requisitions.RemoveAll(x => x.DepartmentId != 2);


            ViewBag.Cat = requisitions;
            //ViewBag.dapaName = requisitions.FirstOrDefault().Employee.Department.DepartmentName;
            return View(requisitions);
        }
        public ActionResult DepartmentEmployee()
        {
            var requisitions = listsvc.ListAllRequisition();

            Session["npg"] = 4;

            ViewBag.Cat = requisitions;
            ViewBag.dapaName = requisitions.First().Employee.Department.DepartmentName;
            return View(requisitions);
        }
        [HttpPost]
        public ActionResult Index(string searchString, string sortOrder/*, int pages = 4*/)
        {
            var requisitions = listsvc.ListAllRequisition();



            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            //ViewBag.NameSortParm = sortOrder == "Name_desc" ? "Name_desc" : "Name";
            ViewBag.emeSortParm = String.IsNullOrEmpty(sortOrder) ? "e_desc" : "";

            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.remeSortParm = String.IsNullOrEmpty(sortOrder) ? "s_desc" : "";





            if (!String.IsNullOrEmpty(searchString))
            {
                var q = listsvc.ListAllRequisition().AsQueryable().Where(s => s.Employee.EmployeeName.Contains(searchString)
                                       || s.ApprovedDate.ToString().Contains(searchString)
                                       || s.Employee.Department.DepartmentName.Contains(searchString));
                requisitions = q.ToList();


                Session["searchstr"] = searchString;
            }
            else {

                requisitions = requisitions.ToList();
                Session["searchstr"] = searchString;

            }
            ///* i*/nt currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            //var requisitions = db.Requisitions.ToList();

            //ViewBag.Cat = requisitions;
            //ViewBag.dapaName = requisitions.First().Employee.Department.DepartmentName;



            //String.IsNullOrEmpty(Session["npg"])


            if (Session["npg"] == null || Session["npg"].Equals("4"))
            {
                Session["npg"] = 4;

            }
            //else
            //{


            //    Session["npg"] = pages;

            //}







           

            // Convert sort order
            ViewBag.NameSort = sortOrder == "Name" ? "Name_desc" : "Name";
            var re = listsvc.ListAllRequisition().AsQueryable();
            if (sortOrder!=null) {
            switch (sortOrder)
            {
                case "Name_desc":
                    re = re.OrderByDescending(s => s.Employee.Department.DepartmentName);
                    break;
                case "e_desc":
                    re = re.OrderBy(s => s.Employee.EmployeeName);
                    break;
                case "Date":
                    re = re.OrderByDescending(s => s.ApprovedDate);
                    break;
                default:
                    re = re.OrderBy(s => s.RequisitionStatus);
                    break;
            }
                requisitions = re.ToList();
            }

            Employee userName = (Employee)Session["User"];

            requisitions.RemoveAll(x => x.DepartmentId != userName.DepartmentId);


            ViewBag.Cat = requisitions;
           
            //ViewBag.dapaName = requisitions.FirstOrDefault().Employee.Department.DepartmentName;


            return View(requisitions);
        }
        [HttpPost]
        public ActionResult DepartmentEmployee(string searchString)
        {
            var requisitions = listsvc.ListAllRequisition();



         
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";





            if (!String.IsNullOrEmpty(searchString))
            {
                var q = listsvc.ListAllRequisition().AsQueryable().Where(s => s.Employee.EmployeeName.Contains(searchString)
                                       || s.ApprovedDate.ToString().Contains(searchString)
                                       || s.Employee.Department.DepartmentName.Contains(searchString));
                requisitions = q.ToList();
            }
            ///* i*/nt currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            //var requisitions = db.Requisitions.ToList();

            //ViewBag.Cat = requisitions;
            //ViewBag.dapaName = requisitions.First().Employee.Department.DepartmentName;














            string userName = Session["UserName"].ToString();

        

            //requisitions = re.ToList();


            ViewBag.Cat = requisitions;
            ViewBag.dapaName = requisitions.First().Employee.Department.DepartmentName;


            return View(requisitions);
        }
        public ActionResult Search(int id)
        {
            //var inventories = inventorySvc.GetInventoryListByCategory(id);
            //var categories = inventorySvc.GetAllCategories();
            //ViewBag.Cat = categories.ToList();
            var requisitions = listsvc.ListAllRequisition();

            ViewBag.Cat = requisitions;
            return View("Index");
        }
        public ActionResult Viewdd()
        {
            //var inventories = inventorySvc.GetInventoryListByCategory(id);
            //var categories = inventorySvc.GetAllCategories();
            //ViewBag.Cat = categories.ToList();
            var requisitions = listsvc.ListAllRequisition();

            ViewBag.Cat = requisitions;
            return View(requisitions);
        }

        // GET: TESTRequisitions/Create
        public ActionResult MakeRequisition()
        {
            ViewBag.EmployeeId = new SelectList(depasvc.GetAllEmployees(), "EmployeeId", "EmployeeName");

            List<RequisitionDetail> relis = listsvc.GetAllRequisitionDetails().Take(3).ToList();


            ViewBag.MembershipList = invsvc.GetAllInventory();

            

            ViewBag.clips = invsvc.GetAllInventory().Where(x => x.Category.CategoryName == "Clips").ToList();






            DateTime d = DateTime.Today;
            ViewBag.time = d.ToShortDateString();
            ViewBag.rel = relis;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeRequisition([Bind(Include = "RequisitionId,EmployeeId,DepartmentId,ApprovedBy,ApprovedDate,OrderedDate,RequisitionStatus")] Requisition requisition, PersonModel model)
        {
            ViewBag.EmployeeId = new SelectList(depasvc.GetAllEmployees(), "EmployeeId", "EmployeeName", requisition.EmployeeId);

            Employee userName = (Employee)Session["User"];


            Requisition req = new Requisition();

            var count = listsvc.ListAllRequisition();
            //var count = db.Requisitions.ToList();
            int idd = count.Count() + 1;

           

           listsvc.UpdateRequisition(requisition,req,idd,userName.EmployeeId, userName.DepartmentId);


           
            ///fake
            //requisition.RequisitionId = idd;
            //req.RequisitionStatus = "Pending";
            //req.EmployeeId = 1;
            //req.DepartmentId = 2;
            //req.OrderedDate = DateTime.Today;
           
          
               
           
          



            List<RequisitionDetail> redlis = new List<RequisitionDetail>();

            //RequisitionDetail  r= model.Items[1];
            req.RequisitionDetails = redlis;


            foreach (ItemModel i in mododo.Items)
            {




                RequisitionDetail rd = new RequisitionDetail();



                rd.Quantity = Int32.Parse(i.Quantity);

                string desc = i.Item;

                string itemno = invsvc.FindItemIdByName(desc);

                //rd.ItemNo = "C002";


                rd.ItemNo = itemno;




                rd.OutstandingQuantity = Int32.Parse(i.Quantity);
                rd.RequisitionId = idd;

                redlis.Add(rd);

                //db.RequisitionDetails.Add(rd);
                //db.SaveChanges();

                req.RequisitionDetails.Add(rd);
            }

           

            //List<RequisitionDetail> relis = db.RequisitionDetails.Take(3).ToList();
            if (ModelState.IsValid)
            {

                listsvc.CreateRequisition(req);
                //ViewBag.rel = relis;
                return RedirectToAction("Index");
            }

            

            notify.NotificationForHeadOnCreate(userName.EmployeeId.ToString());




            return View(requisition);
            //var requisitions = db.Requisitions.ToList();
            //ViewBag.Cat = requisitions;


            ////List<RequisitionDetail> relis = db.RequisitionDetails.ToList();

            ////ViewBag.rel = relis;



            //return View(requisitions);
        }

        // GET: TESTRequisitions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requisition requisition = listsvc.FindById(id);
            if (requisition == null)
            {
                return HttpNotFound();
            }
            return View(requisition);
        }

        [HttpPost]
        public ActionResult AddUser(PersonModel model)
        {
            Requisition requisition = new Requisition();

            mododo = new PersonModel();

            //    model.Items[0];
            if (model != null)
            {
                mododo = model;

                return Json("Success");
            }
            else
            {
                return Json("An Error Has occoured");
            }

        }

       

        public ActionResult ViewRequisitionDetails(int? id)
        {
            

            Requisition requisition = listsvc.FindById(id);
            ViewBag.re = requisition;
          

        
            List<RequisitionDetail> relis = listsvc.GetAllRequisitionDetails().Where(u => u.RequisitionId == id).ToList();

            

            ViewBag.rel = relis;

            return View(requisition);
        }
        public ActionResult HtmlPage1() {

            return View();
        }
    }
}


