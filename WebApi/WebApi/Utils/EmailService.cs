using DAL.Models;
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

            EmailBody = EmailBody + link + "<br>";
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
    }
}





