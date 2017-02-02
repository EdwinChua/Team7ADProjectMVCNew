using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team7ADProjectMVC.Models;

namespace Team7ADProjectMVC.Services.DepartmentService
{
    
    class DepartmentService : IDepartmentService
    {
        ProjectEntities db = new ProjectEntities();
        PushNotification notify = new PushNotification();

        public void UpdateEmployee(Employee e)
        {
            db.Entry(e).State = EntityState.Modified;
            db.SaveChanges();
        }

        public Department FindDeptById(int id)
        {
            return (db.Departments.Find(id));
        }

        public Employee FindEmployeeById(int id)
        {
            return (db.Employees.Find(id));
        }

        public List<Employee> GetAllEmployees()
        {
            return (db.Employees.ToList());
        }


        public List<Department> ListAllDepartments()
        {
            return (db.Departments.ToList());
        }

        public void changeDeptCp(Department department,int cpId)
        {
            
            int id = department.DepartmentId;
            db.Departments.Single(model => model.DepartmentId == id).CollectionPointId = cpId;
            db.SaveChanges();
            
            notify.CollectionPointChanged(id);
        }


        public void UpdateRequi(Requisition requisition,Requisition req, int idd,int eid,int? deid)
        {

            requisition.RequisitionId = idd;
            req.RequisitionStatus = "Pending Approval";
            req.EmployeeId = eid;
            req.DepartmentId = deid;
            req.OrderedDate = DateTime.Today;
           
        }
        //public void UpdateRequiDetail(RequisitionDetail rd, int iter, int idd, int eid, int deid)
        //{

        //    requisition.RequisitionId = idd;
        //    req.RequisitionStatus = "Pending Approval";
        //    req.EmployeeId = 1;
        //    req.DepartmentId = 2;
        //    req.OrderedDate = DateTime.Today;

        //}

        public string FinditemByName(string descibe) {

            string itemid = db.Inventories.Where(x => x.Description == descibe).FirstOrDefault().ItemNo.ToString();
            return itemid;
        }

        public List<RequisitionDetail> GetRequisitionDetailByDept(int dId, int rId)
        {
            var reqItem = from req in db.RequisitionDetails
                          where req.Requisition.DepartmentId == dId
                         && req.RequisitionId == rId
                          select req;
            return reqItem.ToList();
        }


        //Change Rep Methods
        public Employee GetCurrentRep(int? depId)
        {
            var queryBydepId = from t in db.Employees
                               where t.DepartmentId == depId
                               select t;
            var q2 = queryBydepId.ToList();
            foreach (var emp in q2)
            {
                if ((emp.RoleId == 4) || (emp.RoleId == 7))
                {
                    return emp;
                }
            }
            return null;

        }
        public List<Employee> GetAllEmployee(int? depId, int currentRepId)
        {
            var queryBydepId = from t in db.Employees
                               where t.DepartmentId == depId && t.EmployeeId != currentRepId && (t.RoleId != 2 && (t.RoleId != 6 && t.RoleId != 5))
                               orderby t.EmployeeId ascending
                               select t;
            return (queryBydepId.ToList());
        }
        public Employee GetEmpbyId(int? empIdforRep)
        {
            return db.Employees.Find(empIdforRep);
        }
        public void ChangeRep(Employee currentRep, Employee newRep)
        {

            if (currentRep.RoleId == 7)
            {
                currentRep.RoleId = 1;
                newRep.RoleId = 7;
                newRep.Department.RepresentativeId = newRep.EmployeeId;
                db.Entry(currentRep).State = EntityState.Modified;
                db.Entry(newRep).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                currentRep.RoleId = 3;
                newRep.RoleId = 4;
                newRep.Department.RepresentativeId = newRep.EmployeeId;
                db.Entry(currentRep).State = EntityState.Modified;
                db.Entry(newRep).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //Delegate Methods
        public Delegate getDelegatedEmployee(int? depId)
        {
            var queryBydepId = from t in db.Delegates
                               where t.Employee.DepartmentId == depId
                               select t;
            var q2 = queryBydepId.ToList();
            foreach (var xyz in q2)
            {
                if (xyz.EndDate.Equals(xyz.ActualEndDate) && (xyz.ActualEndDate > DateTime.Today || xyz.ActualEndDate.Equals(DateTime.Today)))
                {
                    return xyz;
                }
            }
            return null;
        }

        public List<Employee> GetAllEmployeebyDepId(int? depId)
        {
            var queryBydepId = from t in db.Employees
                               where t.DepartmentId == depId && (t.RoleId != 2 && (t.RoleId != 6 && t.RoleId != 5))
                               orderby t.EmployeeId ascending
                               select t;
            return (queryBydepId.ToList());
        }

        public List<Employee> GetEverySingleEmployeeInDepartment(int? depId)
        {
            var queryBydepId = from t in db.Employees
                               where t.DepartmentId == depId
                               orderby t.EmployeeId ascending
                               select t;
            return (queryBydepId.ToList());
        }

        public Employee FindById(int? empid)
        {
            return db.Employees.Find(empid);
        }
        public void manageDelegate(Employee e, DateTime startDate, DateTime endDate, int? depHeadId)
        {

            Delegate d = new Delegate();

            d.EmployeeId = e.EmployeeId;
            d.StartDate = startDate.Date;
            d.EndDate = endDate.Date;
            d.ActualEndDate = endDate.Date;
            d.ApprovedBy = depHeadId;//default dep head id
            d.ApprovedDate = DateTime.Today;
            db.Delegates.Add(d);
            db.SaveChanges();



            db.Entry(e).State = EntityState.Modified;
            db.SaveChanges();


        }
        public void updateDelegate(Delegate d, DateTime startDate, DateTime endDate, int? depHeadId)
        {
            d.StartDate = startDate.Date;
            d.EndDate = endDate.Date;
            d.ActualEndDate = endDate.Date;
            d.ApprovedBy = depHeadId;//default dep head id
            d.ApprovedDate = DateTime.Today.Date;
            db.Entry(d).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void TerminateDelegate(Delegate d)
        {
            d.ActualEndDate = DateTime.Today.AddDays(-1);
            db.Entry(d).State = EntityState.Modified;
            db.SaveChanges();
        }

        public List<Delegate> getDelegate()
        {

            var query = from t in db.Delegates
                        orderby t.DelegateId ascending
                        select t;


            return (query.ToList());
        }
        public Delegate FinddelegaterecordById(int? delegateId)
        {

            return db.Delegates.Find(delegateId);
        }

        public bool IsDelegate(Employee e)
        {
            if (e.RoleId != 6 && e.RoleId != 2)
            {
                Delegate approvedRecord = getDelegatedEmployee(e.DepartmentId);
                if (approvedRecord != null)
                {
                    if (e.EmployeeId == approvedRecord.EmployeeId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Employee SetDelegatePermissions(Employee e)
        {
            e.Role.ApproveRequisition = true;
            e.Role.ChangeCollectionPoint = true;
            e.Role.MakeRequisition = false;
            return e;
        }
    }
}
