using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Team7ADProjectMVC.Services.UtilityService;

namespace Team7ADProjectMVC.Models.UtilityService
{
    public class UtilityService : IUtilityService
    {
        public DateTime GetDateTimeFromPicker(string date)
        {
            List<String> datesplit = date.Split('/').ToList<String>();
            DateTime selected = new DateTime(Int32.Parse((datesplit[2])), Int32.Parse((datesplit[1])), Int32.Parse((datesplit[0])));
            return selected;
        }

        public void SendEmail(String sendTo, String emailSubject, String emailBody)
        {
            MailMessage mail = new MailMessage("StationeryStore@lu.edu.sg", sendTo);
            SmtpClient client = GetSmtpClient();
            mail.Subject = emailSubject;
            mail.Body = emailBody;
            client.Send(mail);
        }

        private SmtpClient GetSmtpClient()
        {
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "lynx.class.iss.nus.edu.sg";
            return client;
        }
    }
}