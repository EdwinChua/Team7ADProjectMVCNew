using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team7ADProjectMVC.Services.UtilityService
{
    public interface IUtilityService
    {
        DateTime GetDateTimeFromPicker(string date);
        void SendEmail(String sendTo, String emailSubject, String emailBody);
    }
}