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
        public void SendEmail(string companyName,string supplierName,string contactPersonName,string link,string pono)
        {
            //var fromAddress = new MailAddress("ychips02@gmail.com", "Yellow Chips");
            //var toAddress = new MailAddress("rits.kc@gmail.com", "Ritesh");
            //const string fromPassword = "Yellow&Chips1";
            //const string subject = "Test";
            //const string body = "Test Body";

            //var smtp = new SmtpClient
            //{
            //    Host = "smtp.gmail.com",
            //    Port = 587,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            //};
            //using (var message = new MailMessage(fromAddress, toAddress)
            //{
            //    Subject = subject,
            //    Body = body
            //})
            //{
            //    smtp.Send(message);
            //}
            string EmailBody = "";
            string EmailIDList = "";
            string EmailSubject="PO generated: " + pono;            
            

            EmailBody = "Hi " + contactPersonName + "<br>";

            EmailBody = EmailBody + "A new PO has been raised. " + "<br>";

            EmailBody = EmailBody + "Kindly acknowledge the PO by clicking on Acknowledge PO button from following link." + "<br>";

            EmailBody = EmailBody + link + "<br>";
            EmailBody = EmailBody + "<br>" + "Warm Regards,";
            EmailBody = EmailBody + "<br>" + "customer Care";

            EmailBody = EmailBody + "<br>" + companyName;

            EmailIDList = appSettings.To;
            string[] stringArray = EmailIDList.Split(',');


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
                //try
                //{
                //    Result = "Failed Recipients : ";
                //    for (int rec = 0; rec < ((System.Net.Mail.SmtpFailedRecipientsException)(ex)).InnerExceptions.Count(); rec++)
                //    {
                //        Result = Result + Environment.NewLine + ((System.Net.Mail.SmtpFailedRecipientsException)(ex)).InnerExceptions[rec].FailedRecipient;
                //    }
                //}
                //catch
                //{
                //    Result = "";

                //}
                //Result = ((System.Net.Mail.SmtpFailedRecipientException)(ex)).FailedRecipient;
            }

            catch (SmtpException ex)
            {
                //Result = ex.Message;
            }

            catch (Exception ex)
            {
                //Result = ex.Message;
            }           
        }
    }
}





