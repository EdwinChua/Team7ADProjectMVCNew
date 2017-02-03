﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Team7ADProjectMVC.Services.UtilityService;

namespace Team7ADProjectMVC.Models
{
    public class UtilityService : IUtilityService
    {
        public DateTime GetDateTimeFromPicker(string date)
        {
            List<String> datesplit = date.Split('/').ToList<String>();
            DateTime selected = new DateTime(Int32.Parse((datesplit[2])), Int32.Parse((datesplit[1])), Int32.Parse((datesplit[0])));
            return selected;
        }

        public void SendEmail(List<string> sendToEmailAddress, string emailSubject, string emailBody, List<string> ccToEmailAddress = null)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("StationeryStore@lu.edu.sg");
            foreach (string emailAdd in sendToEmailAddress)
            {
                mail.To.Add(new MailAddress(emailAdd));
            }
            if (ccToEmailAddress != null)
            {
                foreach (string emailAdd in ccToEmailAddress)
                {
                    mail.CC.Add(new MailAddress(emailAdd));
                }
            }
            SmtpClient client = GetSmtpClient();
            mail.Subject = emailSubject;
            mail.Body = emailBody;
            client.Send(mail);
        }

        private SmtpClient GetSmtpClient()
        {
            SmtpClient client = new SmtpClient();
            
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;           
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.UseDefaultCredentials = false;
            client.Host = "Smtp.Gmail.com";
            client.Port = 587;
            client.Credentials = new NetworkCredential("youngmountain7@gmail.com", "password!!");
            //client.Host = "lynx.class.iss.nus.edu.sg";
            //client.Port = 25;
            return client;
        }
    }
}