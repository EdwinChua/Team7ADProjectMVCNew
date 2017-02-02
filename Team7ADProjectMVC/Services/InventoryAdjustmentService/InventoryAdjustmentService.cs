using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team7ADProjectMVC.Models.InventoryAdjustmentService
{
    public class InventoryAdjustmentService : IInventoryAdjustmentService
    {
        ProjectEntities db = new ProjectEntities();
        public string findRolebyUserID(int userid)
        {
            string role = db.Employees.Find(userid).Role.Name;
            return (role);
        }
        public List<Adjustment> findSupervisorAdjustmentList()
        {

            var adjustmentlist = (from x in db.Adjustments
                                  where x.Status == "Pending Approval"
                                  || x.Status == "Approved"
                                  || x.Status == "Rejected"
                                  orderby x.AdjustmentDate
                                  select x
                                   ).ToList();


            return (adjustmentlist);
        }
        public List<Adjustment> findManagerAdjustmentList()
        {
            var adjustmentlist = (from x in db.Adjustments
                                  where x.Status == "Pending final Approval"
                                  || x.Status == "Approved"
                                  || x.Status == "Rejected"
                                  orderby x.AdjustmentDate
                                  select x
                      ).ToList();

            return (adjustmentlist);
        }

        public List<Adjustment> FindAdjustmentBySearch(List<Adjustment> searchlist, string employee, string status, string date)
        {

            //int epyid = Int32.Parse(employee);
            //List<String> datesplit = date.Split('/').ToList<String>();
            //DateTime selectedate = new DateTime(Int32.Parse((datesplit[2])), Int32.Parse((datesplit[1])), Int32.Parse((datesplit[0])));
            if ((status == null || status == "") && (date == null || date == "") && (employee == null || employee == ""))
            {
                return searchlist;
            }
            else if ((date == null || date == "") && (employee == null || employee == ""))//only select status
            {
                var resultlist = (from x in searchlist
                                  where x.Status.Equals(status)
                                  orderby x.AdjustmentDate
                                  select x).ToList();
                return resultlist;
            }
            else if ((status == null || status == "") && (date == null || date == ""))//only select employee
            {
                int epyid = Int32.Parse(employee);
                var resultlist = (from x in searchlist
                                  where x.EmployeeId == epyid
                                  orderby x.AdjustmentDate
                                  select x).ToList();
                return resultlist;

            }
            else if ((status == null || status == "") && (employee == null || employee == ""))//only select date
            {
                List<String> datesplit = date.Split('/').ToList<String>();
                DateTime selectedate = new DateTime(Int32.Parse((datesplit[2])), Int32.Parse((datesplit[1])), Int32.Parse((datesplit[0])));
                var resultlist = (from x in searchlist
                                  where x.AdjustmentDate == selectedate
                                  orderby x.Status
                                  select x).ToList();
                return resultlist;
            }
            else if ((date == null || date == ""))//select employee and status
            {
                int epyid = Int32.Parse(employee);
                var resultlist = (from x in searchlist
                                  where x.EmployeeId == epyid
                                  && x.Status.Equals(status)
                                  orderby x.AdjustmentDate
                                  select x).ToList();
                return resultlist;
            }
            else if ((status == null || status == ""))//select emp and date
            {
                int epyid = Int32.Parse(employee);
                List<String> datesplit = date.Split('/').ToList<String>();
                DateTime selectedate = new DateTime(Int32.Parse((datesplit[2])), Int32.Parse((datesplit[1])), Int32.Parse((datesplit[0])));
                var resultlist = (from x in searchlist
                                  where x.AdjustmentDate == selectedate
                                  && x.EmployeeId == epyid
                                  orderby x.Status
                                  select x).ToList();
                return resultlist;
            }
            else if ((employee == null || employee == ""))//select date and status
            {
                List<String> datesplit = date.Split('/').ToList<String>();
                DateTime selectedate = new DateTime(Int32.Parse((datesplit[2])), Int32.Parse((datesplit[1])), Int32.Parse((datesplit[0])));
                var resultlist = (from x in searchlist
                                  where x.AdjustmentDate == selectedate
                                  && x.Status.Equals(status)
                                  orderby x.EmployeeId
                                  select x).ToList();
                return resultlist;
            }
            else
            {
                int epyid = Int32.Parse(employee);
                List<String> datesplit = date.Split('/').ToList<String>();
                DateTime selectedate = new DateTime(Int32.Parse((datesplit[2])), Int32.Parse((datesplit[1])), Int32.Parse((datesplit[0])));
                var resultlist = (from x in searchlist
                                  where x.AdjustmentDate == selectedate
                                  && x.Status.Equals(status)
                                  && x.EmployeeId == epyid
                                  orderby x.Status
                                  select x).ToList();
                return resultlist;
            }
        }
        public Adjustment findAdjustmentByID(int? id)
        {

            return (db.Adjustments.Find(id));
        }
        public List<AdjustmentDetail> findDetailByAdjustment(Adjustment adjust)
        {
            return (db.AdjustmentDetails.Where(x => x.AdjustmentId == adjust.AdjustmentId).ToList());
        }
        public string findAdjustmentStatus(int? id)
        {
            return (db.Adjustments.Find(id).Status);
        }
        public decimal? caculateTotal(List<AdjustmentDetail> adjdtlist)
        {
            decimal? total = 0;
            foreach (var item in adjdtlist)
            {
                var price = item.Quantity * item.Inventory.Price1;

                total += total + price;
            }
            return total;
        }
        public void ApproveBySupervisor(int? empid, int? adjid)
        {
            db.Adjustments.Find(adjid).SupervisorAuthorizedDate = DateTime.Today;
            db.Adjustments.Find(adjid).SupervisorId = empid;
            db.Adjustments.Find(adjid).Status = "Approved";
            db.SaveChanges();
        }
        public void RejecteBySupervisor(int? empid, int? adjid)
        {
            db.Adjustments.Find(adjid).SupervisorAuthorizedDate = DateTime.Today;
            db.Adjustments.Find(adjid).SupervisorId = empid;
            db.Adjustments.Find(adjid).Status = "Rejected";
            db.SaveChanges();
        }
        public void ApproveByManager(int? empid, int? adjid)
        {
            db.Adjustments.Find(adjid).HeadAuthorizedDate = DateTime.Today;
            db.Adjustments.Find(adjid).HeadId = empid;
            db.Adjustments.Find(adjid).Status = "Approved";
            db.SaveChanges();
        }
        public void RejectByManager(int? empid, int? adjid)
        {
            db.Adjustments.Find(adjid).HeadAuthorizedDate = DateTime.Today;
            db.Adjustments.Find(adjid).HeadId = empid;
            db.Adjustments.Find(adjid).Status = "Rejected";
            db.SaveChanges();
        }
        
        public void PendingBySupervisor(int? empid, int? adjid)
        {
            db.Adjustments.Find(adjid).SupervisorAuthorizedDate = DateTime.Today;
            db.Adjustments.Find(adjid).SupervisorId = empid;
            db.Adjustments.Find(adjid).Status = "Pending Final Approval";
            db.SaveChanges();
        }
        public void createAdjustment(Adjustment adjustment)
        {
            db.Adjustments.Add(adjustment);
            db.SaveChanges();
        }
    }
}
