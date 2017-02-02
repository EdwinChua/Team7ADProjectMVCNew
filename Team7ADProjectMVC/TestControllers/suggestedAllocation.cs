using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team7ADProjectMVC.Models;

namespace Team7ADProjectMVC.TestControllers
{
    public class suggestedAllocation
    {
        ProjectEntities db = new ProjectEntities();


        public void PopulateRetrievalListItems()
        {
            System.Web.HttpContext.Current.Application.Lock();
            RetrievalList rList = (RetrievalList)System.Web.HttpContext.Current.Application["RetrievalList"];
            if (rList.itemsToRetrieve == null)
            {
                rList.itemsToRetrieve = new List<RetrievalListItems>();

                List<RetrievalListItems> unconsolidatedList = new List<RetrievalListItems>();

                //******************************

                List<RequisitionDetail> reqDetailList = db.RequisitionDetails.Where(x => rList.requisitionList.Contains(x.Requisition)).ToList();

                var q = from x in reqDetailList
                        group x by x.ItemNo into g
                        select new { ItemNo = g.Key, Quantity = g.Sum(y => y.OutstandingQuantity) };
                var itemswithOS = q.ToList();

                foreach (var x in itemswithOS)
                {
                    Inventory inv = db.Inventories.Find(x.ItemNo);
                    RetrievalListItems newItem = new RetrievalListItems();
                    newItem.itemNo = x.ItemNo;
                    newItem.requiredQuantity = (int)x.Quantity;
                    newItem.binNo = inv.BinNo;
                    newItem.description = inv.Description;
                    newItem.collectionStatus = false;
                    unconsolidatedList.Add(newItem);
                }
            }
        }


        public void AutoAllocateDisbursements()
        {
            System.Web.HttpContext.Current.Application.Lock();
            RetrievalList retrievalList = (RetrievalList)System.Web.HttpContext.Current.Application["RetrievalList"];


            List<Requisition> requisitionListFromRList = retrievalList.requisitionList;
            //********************************************

            List<RequisitionDetail> reqDetailList = db.RequisitionDetails.Where(x => requisitionListFromRList.Contains(x.Requisition)).ToList();
            IEnumerable<RequisitionDetail> uniqueDepts = reqDetailList.GroupBy(x => x.Requisition.DepartmentId).Select(group => group.First()).ToList();
            List<int> deptIDs = new List<int>();
            foreach (RequisitionDetail rd in uniqueDepts)
            {
                deptIDs.Add((int)rd.Requisition.DepartmentId);
            }

            DisbursementList[] disbursementLists = new DisbursementList[deptIDs.Count];
            for (int z = 0; z < deptIDs.Count; z++)
            {
                int deptId = deptIDs[z];
                Department d = db.Departments.Find(deptId);
                DisbursementList disbList = new DisbursementList();
                disbList.DepartmentId = deptId;
                disbList.CollectionPointId = d.CollectionPointId;
                //disbList.OrderedDate = requisition.OrderedDate; ???
                disbList.RetrievalId = retrievalList.retrievalId;
                disbList.Status = "Pending Delivery";
                disbList.DeliveryDate = DateTime.Today.AddDays(2); //TODO: Place logic for date later
                disbursementLists[z] = disbList;

            }

            foreach (RetrievalListItems rli in retrievalList.itemsToRetrieve)
            {
                int totalAvailableQty = rli.collectedQuantity;
                IEnumerable<RequisitionDetail> requestDetailsFromAllDeptsForItem = reqDetailList.Where(x => x.ItemNo == rli.itemNo).ToList();
                List<DisbursementDetail> generatedDDs = GenerateDisbursementDetailByItem(totalAvailableQty, requestDetailsFromAllDeptsForItem);
                DistributeDisbursementDetailsFromListToDepartmentPerItem(disbursementLists, generatedDDs);
            }

            foreach (DisbursementList dl in disbursementLists)
            {
                db.DisbursementLists.Add(dl);
            }
            db.SaveChanges();
        }

        private List<DisbursementDetail> GenerateDisbursementDetailByItem(int qtyToAllocate, IEnumerable<RequisitionDetail> list)
        {
            List<DisbursementDetail> disbursementDetailList = new List<DisbursementDetail>();

            foreach (RequisitionDetail rd in list)
            {
                while (qtyToAllocate > 0)
                {
                    if (qtyToAllocate >= rd.OutstandingQuantity)
                    {
                        DisbursementDetail dd = new DisbursementDetail();
                        //dd.RequisitionDetailId = rd.RequisitionDetailId; thats how it comes in
                        dd.PreparedQuantity = rd.OutstandingQuantity;
                        dd.DeliveredQuantity = rd.OutstandingQuantity;
                        dd.ItemNo = rd.ItemNo;

                        disbursementDetailList.Add(dd);
                    }
                    else
                    {
                        DisbursementDetail dd = new DisbursementDetail();
                        //dd.RequisitionDetailId = rd.RequisitionDetailId; thats how it comes in
                        dd.PreparedQuantity = qtyToAllocate;
                        dd.DeliveredQuantity = qtyToAllocate;
                        dd.ItemNo = rd.ItemNo;
                        disbursementDetailList.Add(dd);
                    }
                    Requisition requisition = db.Requisitions.Find(rd.RequisitionId);
                    requisition.RequisitionStatus = "Processing";
                    db.SaveChanges();
                }
            }

            return disbursementDetailList;
        }

        private void DistributeDisbursementDetailsFromListToDepartmentPerItem(DisbursementList[] disbursementLists, List<DisbursementDetail> DDList)
        {
            foreach (DisbursementDetail dd in DDList)
            {
                foreach (DisbursementList disbList in disbursementLists)
                {
                    if (dd.RequisitionDetailId.Requisition.Emp.DepId == disbList.DepartmentId)
                    {
                        disbList.DisbursementDetails.Add(dd);
                    }
                }
            }
        }
    }
}