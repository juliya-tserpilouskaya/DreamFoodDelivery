using DreamFoodDelivery.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic
{
    public class EmailSenderService : IEmailSenderService
    {
        const string FROM_EMAIL_ADDRESS = "mylt015@mail.ru";
        const string FROM_EMAIL_PASSWORD = "perisher1996";

        ////Make async
        //public static void SendMail(string toEmailAddress, string emailTitle, string emailMsgBody)
        //{
        //    MailAddress fromAddress = new MailAddress(FROM_EMAIL_ADDRESS);
        //    MailAddress toAddress = new MailAddress(toEmailAddress);
        //    MailMessage email = new MailMessage(fromAddress, toAddress);
        //    email.Subject = emailTitle;
        //    email.Body = emailMsgBody;
        //    email.IsBodyHtml = true;
        //    SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
        //    smtp.Credentials = new NetworkCredential(FROM_EMAIL_ADDRESS, FROM_EMAIL_PASSWORD);
        //    smtp.EnableSsl = true;
        //    try
        //    {
        //        smtp.Send(email);
        //    }
        //    catch (SmtpFailedRecipientsException ex)
        //    {
        //        for (int i = 0; i < ex.InnerExceptions.Length; i++)
        //        {
        //            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
        //            if (status == SmtpStatusCode.MailboxBusy ||
        //                status == SmtpStatusCode.MailboxUnavailable)
        //            {
        //                System.Threading.Thread.Sleep(5000);
        //                smtp.Send(email);
        //            }
        //        }
        //    }
        //    //catch (Exception ex)
        //    //{
        //    //    throw;
        //    //}
        //}

        public Result SendMailResult(string toEmailAddress, string emailTitle, string emailMsgBody)
        {
            MailAddress fromAddress = new MailAddress(FROM_EMAIL_ADDRESS);
            MailAddress toAddress = new MailAddress(toEmailAddress);
            MailMessage email = new MailMessage(fromAddress, toAddress);
            email.Subject = emailTitle;
            email.Body = emailMsgBody;
            email.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            smtp.Credentials = new NetworkCredential(FROM_EMAIL_ADDRESS, FROM_EMAIL_PASSWORD);
            smtp.EnableSsl = true;
            try
            {
                smtp.Send(email);
                return Result.Ok();
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        System.Threading.Thread.Sleep(5000);
                        smtp.Send(email);
                    }
                }
                return Result.Fail($"Cannot send email. {ex.Message}");
            }
        }
    }
}
