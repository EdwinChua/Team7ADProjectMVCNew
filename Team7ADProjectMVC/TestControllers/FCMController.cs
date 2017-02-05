using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team7ADProjectMVC.Models;

namespace Team7ADProjectMVC.TestControllers
{
    //Not included in project
    public class FCMController : Controller
    {
        // GET: FCM
      
        public ActionResult Index()
        {
            return View();
        }

        public void Test(int? id)
        {
            PushNotification fcmPush = new PushNotification();

           if(id==1)
            fcmPush.CollectionPointChanged(2);//to clerk, change in location
       
           else
               if(id==2)
            fcmPush.NewRequisitonMade("3");//to clerk, new req made..


            else if(id==3)
                 fcmPush.RepAcceptRequisition("2");// to clerk, when rep accepts the req...

            else if(id==4)
                   fcmPush.CheckForStockReorder();//to clerk,when the stock run out.....

            else if (id == 5)
                       fcmPush.NotificationForHeadOnCreate("14");//to head, when new req come in for accepting..

            else if(id==6)// to rep. for confirming..
               {
                   List<String> myData = new List<string>();
                   myData.Add("ReceiveRequisition");
                   myData.Add("English Department");
                   myData.Add("34");
                   myData.Add("Stationert Store");

                   fcmPush.PushNotificationForRep("Accept Delivery", "Please Confirm Delivery", myData, 4);
                }
                  

        }

      

    }
}