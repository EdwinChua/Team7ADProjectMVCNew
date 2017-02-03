using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Team7ADProjectMVC.Models.ListAllRequisitionService
{
    public class RequisitionService : IRequisitionService
    {
        ProjectEntities db = new ProjectEntities();
        PushNotification notify = new PushNotification();


        public List<Requisition> ListAllRequisition()
        {

            return (db.Requisitions.ToList());
        }
        public List<Requisition> GetAllRequisition(int? depId)
        {
            var queryByStatus = from t in db.Requisitions 
                                  where t.RequisitionStatus == "Pending Approval" && t.DepartmentId == depId
                                 orderby t.RequisitionId ascending
                                  select t;
            return (queryByStatus.ToList());
 
        }
      
        public Requisition FindById(int? requisitionId)
        {
            return db.Requisitions.Find(requisitionId);
        }
        
        public void  UpdateApproveStatus(Requisition requisition, string comments)
        {

            requisition.RequisitionStatus = "Approved";
            requisition.Comment = comments;
            requisition.ApprovedDate = DateTime.Today.Date;
            
            db.Entry(requisition).State = EntityState.Modified;
            db.SaveChanges();

            string reqListId = requisition.RequisitionId.ToString();
            notify.NewRequisitonMade(reqListId);
        }
        public void UpdateRejectStatus(Requisition requisition, string comments)
        {

            requisition.Comment = comments;
            requisition.ApprovedDate = DateTime.Today.Date;
            requisition.RequisitionStatus = "Rejected";
            db.Entry(requisition).State = EntityState.Modified;
            db.SaveChanges();
        }
         public List<Requisition> getDataForPagination(string searchString)
        {
            var queryByStatus= db.Requisitions.Where(s => (s.Employee.EmployeeName.Contains(searchString)
                                       || s.OrderedDate.ToString().Contains(searchString)));
            return (queryByStatus.ToList());
        }

        public List<RequisitionDetail> GetAllRequisitionDetails(int dId, int rId)
        {
            var aList = from a in db.RequisitionDetails
                        where a.RequisitionId == rId
                        && a.Requisition.DepartmentId == dId
                        && a.Requisition.RequisitionStatus == "Pending Approval"
                        orderby a.Inventory.Description ascending
                        select a;
            return aList.ToList();
        }
        public List<RequisitionDetail> GetAllRequisitionDetails()
        {
            return db.RequisitionDetails.ToList();
        }

        public void CreateRequisition(Requisition r)
        {
            db.Requisitions.Add(r);
            db.SaveChanges();
        }

        public void UpdateRequisition(Requisition requisition, Requisition req, int idd, int eid, int? deid)
        {

            requisition.RequisitionId = idd;
            req.RequisitionStatus = "Pending Approval";
            req.EmployeeId = eid;
            req.DepartmentId = deid;
            req.OrderedDate = DateTime.Today;

        }

        public void CreateRequisition(Requisition requisition, int employeeId)
        {
            requisition.Employee = db.Employees.Find(employeeId);
            db.Requisitions.Add(requisition);
            db.SaveChanges();
        }
    }
}