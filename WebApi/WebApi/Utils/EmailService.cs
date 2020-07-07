﻿using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApi.Settings;

namespace WebApi.Utils
{
    public class EmailService
    {
        private readonly AppSettings appSettings;

        public EmailService(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }
        public void SendAcknoledgePOEmail(string companyName,string supplierName,string contactPersonName,string link,string pono)
        {            
            string EmailBody = "";
            string EmailIDList = "";
            string EmailSubject="PO generated: " + pono;            
            

            EmailBody = "Hi " + contactPersonName + "<br>";

            EmailBody = EmailBody + "A new PO has been raised. Or there is a change in existing PO " + "<br>";

            EmailBody = EmailBody + "Kindly acknowledge the PO by clicking on Acknowledge PO button from following link." + "<br>";

            EmailBody = EmailBody + "<div>  <a href = " + link + " style = \"background-color:#31b59f;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:18px;font-weight:bold;line-height:65px;text-align:center;text-decoration:none;width:250px;min-width:250px;font-size:20px\" target = \"_blank\"  > Acknowledge PO </ a > </div>";

            //EmailBody = EmailBody + link + "<br>";
            EmailBody = EmailBody + "<br>" + "Warm Regards,";
            EmailBody = EmailBody + "<br>" + "customer Care";

            EmailBody = EmailBody + "<br>" + companyName;

            EmailIDList = appSettings.To;
            string[] stringArray = EmailIDList.Split(',');


            SendEmail(EmailIDList, EmailSubject, EmailBody);
        }

        public void SendContentEmail(string companyName,string subject,  string no)
        {
            string EmailBody = "";
            string EmailIDList = "";
            string EmailSubject = subject + no;

            EmailBody = subject + no + "<br>";
            //EmailBody = EmailBody + link + "<br>";
            EmailBody = EmailBody + "<br>" + "Warm Regards,";
            EmailBody = EmailBody + "<br>" + "customer Care";

            EmailBody = EmailBody + "<br>" + companyName;

            EmailIDList = appSettings.To;
            string[] stringArray = EmailIDList.Split(',');


            SendEmail(EmailIDList, EmailSubject, EmailBody);
        }

        public void SendNotifyAcknoledgePOEmail(string companyName, string supplierName, string pono)
        {
            string EmailBody = "";
            string EmailIDList = "";
            string EmailSubject = "PO Acknowledged: " + pono;


            EmailBody = "Hi " +  "<br>";

            EmailBody = EmailBody + "A Following PO has been Acknowledged. " + "<br>";

            EmailBody = EmailBody + "PO No : " +pono + "<br>";
            
            EmailBody = EmailBody + "<br>" + "Warm Regards,";
            EmailBody = EmailBody + "<br>" + "customer Care";

            EmailBody = EmailBody + "<br>" + companyName;

            EmailIDList = appSettings.To;
            string[] stringArray = EmailIDList.Split(',');


            SendEmail(EmailIDList, EmailSubject, EmailBody);
        }

        private void SendEmail(string EmailIDList,string EmailSubject, string EmailBody)
        {
            try
            {
                using (MailMessage msg = new MailMessage())
                {
                    msg.From = new MailAddress(appSettings.UserName);
                    //msg.To.Add(new MailAddress(EmailIDList));
                    msg.To.Add(EmailIDList);

                    msg.IsBodyHtml = true;                    
                    msg.Subject = EmailSubject;
                    msg.Body = EmailBody;

                    using (SmtpClient client = new SmtpClient())
                    {
                        //Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.Host = appSettings.Host;
                        client.Port = Convert.ToInt32(appSettings.Port);
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(appSettings.UserName, appSettings.Password);
                        client.UseDefaultCredentials = false;
                        client.Credentials = credentials;
                        client.Send(msg);
                    }
                }
            }
            catch (SmtpFailedRecipientException ex)
            {

            }
        }

        public void SendUserDetailViaEmail(User user)
        {
            string EmailBody = "";            
            string EmailSubject = "User detail ";


            EmailBody = "Hi " + user.FirstName + " " + user.LastName + "<br>";

            EmailBody = EmailBody + "Here is your account detail " + "<br>";
            

            EmailBody = EmailBody + "Username: " + user.UserName + "<br>";
            EmailBody = EmailBody + "Password: " + user.Password + "<br>";
            EmailBody = EmailBody + "First name: " + user.FirstName + "<br>";
            EmailBody = EmailBody + "Last name: " + user.LastName + "<br>";

            EmailBody = EmailBody + "<br>" + "Warm Regards,";
            EmailBody = EmailBody + "<br>" + "customer Care";

            SendEmail(user.Email, EmailSubject, EmailBody);
        }

        public void SendPOSEmail(string companyName, string contactPersonName, string link, string packingSlipNo, string EmailIDList)
        {
            string divStart = "<div>";
            string divEnd = "</div>";

            string EmailBody = "";
            string EmailSubject = "POS ready to download: " + packingSlipNo;


            EmailBody = "Hi " + contactPersonName + "<br>";

            EmailBody = EmailBody + "A POS has been uploaded. " + "<br>";

            EmailBody = EmailBody + "Kindly download the POS by clicking on the following link." + "<br>";

            //EmailBody = EmailBody + "< a href = " + link + " download > Download POS </ a >";
            //EmailBody = EmailBody + "<div> style = \"text-align:center;margin:35px auto 50px;font-size:30px\">" +
            EmailBody = EmailBody + "<div>  <a href = " + link + " style = \"background-color:#31b59f;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:18px;font-weight:bold;line-height:65px;text-align:center;text-decoration:none;width:250px;min-width:250px;font-size:20px\" target = \"_blank\"  > Download POS </ a > </div>";

                          //EmailBody = EmailBody + link + "<br>";
            EmailBody = EmailBody + divStart +  "<br>" + "Warm Regards,";
            EmailBody = EmailBody + "<br>" + "customer Care";

            EmailBody = EmailBody + "<br>" + companyName + divEnd;            

            //EmailIDList = appSettings.To;
            string[] stringArray = EmailIDList.Split(',');


            SendEmail(EmailIDList, EmailSubject, EmailBody);
        }
    }   
}





