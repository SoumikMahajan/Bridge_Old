using POMS.Controllers;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;

public class Email
{

    public Email()
    {
    }

    public void SendeMail1(string sFromEmail, string sToEmail, string sCCEmail, string sBCCEmail, string sHeader, string sMessage, string imagePath)
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(sToEmail);
            mail.From = new MailAddress("noreply@mysite.com");//sFromEmail.ToString());
            if (sCCEmail.ToString() != "")
            {
                mail.CC.Add(sCCEmail.ToString());
            }
            if (sBCCEmail.ToString() != "")
            {
                mail.Bcc.Add(sBCCEmail.ToString());
            }
            mail.Subject = sHeader;
            mail.Body = sMessage;
            mail.IsBodyHtml = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsBodyHtml"].ToString());

            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = false;
            smtp.Host = WebConfigurationManager.AppSettings["host"].ToString();
            smtp.Credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["userName"].ToString(), ConfigurationManager.AppSettings["password"].ToString()); //Or your Smtp Email ID and Password         
            mail.IsBodyHtml = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            smtp.EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
            smtp.Port = 587;
            mail.Priority = System.Net.Mail.MailPriority.High;
            mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
            smtp.Send(mail);
        }
        catch (Exception ex)
        {

            new LogWriter("Email exception :" + ex.Message.ToString());
        }


    }

    public void mailfunction(string sFromEmail, string sToEmail, string sCCEmail, string sBCCEmail, string sHeader, string sMessage, string imagePath)// HttpPostedFile file)
    {
        try
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(sFromEmail);
            msg.Subject = "Test";
            msg.To.Add(sToEmail);
            msg.IsBodyHtml = true;
            string MailBody = sMessage;
            msg.Body = MailBody;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail.theetsindia.com";
            smtp.Credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["userName"].ToString(), ConfigurationManager.AppSettings["password"].ToString()); //Or your Smtp Email ID and Password         
            smtp.Port = 26;
            smtp.Send(msg);
        }
        catch (Exception e)
        {
            new LogWriter("Email exception :" + e.Message.ToString());
        }
    }

    public void SendeMail(string sFromEmail, string sToEmail, string sCCEmail, string sBCCEmail, string sHeader, string sMessage, string imagePath)// HttpPostedFile file)
    {
        try
        {
            //MailMessage mail = new MailMessage();
            //mail.To.Add(sToEmail);
            //mail.From = new MailAddress("donotreply@thebridgebytcg.com");
            //mail.Subject = sHeader;
            //if (sCCEmail.ToString() != "")
            //{
            //    mail.CC.Add(sCCEmail.ToString());
            //}
            //if (sBCCEmail.ToString() != "")
            //{
            //    mail.Bcc.Add(sBCCEmail.ToString());
            //}
            //mail.Subject = sHeader;
            //mail.Body = sMessage;
            //mail.IsBodyHtml = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            //mail.IsBodyHtml = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            //mail.Priority = System.Net.Mail.MailPriority.High;
            //mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;

            //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            //smtpClient.Port = 25;
            //smtpClient.UseDefaultCredentials = true;
            //smtpClient.Send(mail);
            MailMessage mail = new MailMessage();
            mail.To.Add(sToEmail);
            mail.From = new MailAddress("developer@esolztech.com");
            mail.Subject = sHeader;
            if (sCCEmail.ToString() != "")
            {
                mail.CC.Add(sCCEmail.ToString());
            }
            if (sBCCEmail.ToString() != "")
            {
                mail.Bcc.Add(sBCCEmail.ToString());
            }
            mail.Subject = sHeader;
            mail.IsBodyHtml = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            mail.IsBodyHtml = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            mail.Priority = System.Net.Mail.MailPriority.High;
            mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;

            SmtpClient smtpClient = new SmtpClient("silver.specialservers.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("developer@esolztech.com", "4p2$Z2s1s");
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {

            new LogWriter("Email exception :" + ex.Message.ToString());
        }

    }

    public void SendeMail2(string sFromEmail, string sToEmail, string sCCEmail, string sBCCEmail, string sHeader, MailMessage mail, string imagePath, string Project_ID, string poid
        , string ActionName)// HttpPostedFile file)
    {
        try
        {
            //mail.To.Add(sToEmail);
            //mail.From = new MailAddress("system@thebridgebytcg.com");
            //mail.Subject = sHeader;
            //if (sCCEmail.ToString() != "")
            //{
            //    mail.CC.Add(sCCEmail.ToString());
            //}
            //if (sBCCEmail.ToString() != "")
            //{
            //    mail.Bcc.Add(sBCCEmail.ToString());
            //}
            //mail.Subject = sHeader;
            //mail.IsBodyHtml = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            //mail.IsBodyHtml = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            //mail.Priority = System.Net.Mail.MailPriority.High;
            //mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
            //SmtpClient smtpClient = new SmtpClient("localhost");
            //smtpClient.Port = 25;
            //smtpClient.UseDefaultCredentials = true;
            //new LogWriter("sToEmail :" + sToEmail);
            //smtpClient.Send(mail);
            //new LogWriter("sToEmail :" + sToEmail + " Success");
            //sCCEmail = "debankit.sarkar@esolzmail.com";

            mail.To.Add(sToEmail);
            mail.From = new MailAddress("developer@esolztech.com");
            mail.Subject = sHeader;
            if (sCCEmail.ToString() != "")
            {
                mail.CC.Add(sCCEmail.ToString());
            }
            if (sBCCEmail.ToString() != "")
            {
                mail.Bcc.Add(sBCCEmail.ToString());
            }
            mail.Subject = sHeader;
            mail.IsBodyHtml = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            mail.IsBodyHtml = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            mail.Priority = System.Net.Mail.MailPriority.High;
            mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;

            SmtpClient smtpClient = new SmtpClient("silver.specialservers.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("developer@esolztech.com", "4p2$Z2s1s");
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {

            new LogWriter("ProjectID : " + Project_ID + ",\r\n" + "   PurchasedOrderId : " + poid + ",\r\n" + "   CC Mail : " + sCCEmail + ",\r\n" + "   ActionName : " + ActionName + ",\r\n" + "   Email exception :" + ex.Message.ToString());
        }
    }

    public void SendeMailNew(string sFromEmail, string sToEmail, string sCCEmail, string sBCCEmail, string sHeader, MailMessage mail, string imagePath, string file, string body,string Project_ID,string poid
        ,string ActionName)// HttpPostedFile file)
    {
        //try
        //{
        //    mail.To.Add(sToEmail);
        //    mail.From = new MailAddress("system@thebridgebytcg.com");
        //    mail.Subject = sHeader;
        //    //if (file != "")
        //    //{
        //    //    //file = file.Replace("../..", "");
        //    //    mail.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath(file)));
        //    //}

        //    if (sCCEmail.ToString() != "")
        //    {
        //        mail.CC.Add(sCCEmail.ToString());
        //    }
        //    if (sBCCEmail.ToString() != "")
        //    {
        //        mail.Bcc.Add(sBCCEmail.ToString());
        //    }
        //    mail.Subject = sHeader;

        //    if (file != "")
        //    {
        //        for (int k = 0; k < file.Split('^').Length; k++)
        //        {
        //            mail.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath(file.Split('^')[k].ToString())));

        //        }


        //    }

        //    mail.Body = body;
        //    mail.IsBodyHtml = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsBodyHtml"].ToString());
        //    mail.IsBodyHtml = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBodyHtml"].ToString());
        //    mail.Priority = System.Net.Mail.MailPriority.High;
        //    mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
        //    SmtpClient smtpClient = new SmtpClient("localhost");
        //    smtpClient.Port = 25;
        //    smtpClient.UseDefaultCredentials = true;
        //    smtpClient.Send(mail);
        //}
        //catch (Exception ex)
        //{
        //    new LogWriter("Email exception :" + ex.Message.ToString());
        //}

        double totalAttachmentSizeKB = 0;

        try
        {
            mail.To.Add(sToEmail);
            mail.From = new MailAddress("developer@esolztech.com");
            mail.Subject = sHeader;
            long totalAttachmentSize = 0;            

            if (sCCEmail.ToString() != "")
            {
                mail.CC.Add(sCCEmail.ToString());
            }
            if (sBCCEmail.ToString() != "")
            {
                mail.Bcc.Add(sBCCEmail.ToString());
            }
            mail.Subject = sHeader;

            if (file != "")
            {
                for (int k = 0; k < file.Split('^').Length; k++)
                {
                    string fullPath = System.Web.HttpContext.Current.Server.MapPath(file.Split('^')[k].ToString());
                    Attachment attachment = new Attachment(fullPath);
                    mail.Attachments.Add(attachment);

                    FileInfo fileInfo = new FileInfo(fullPath);
                    totalAttachmentSize += fileInfo.Length;

                }


            }

            // Convert to kilobytes
            totalAttachmentSizeKB = totalAttachmentSize / 1024.0;

            mail.Body = body;
            mail.IsBodyHtml = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            mail.IsBodyHtml = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBodyHtml"].ToString());
            mail.Priority = System.Net.Mail.MailPriority.High;
            mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
            
            SmtpClient smtpClient = new SmtpClient("silver.specialservers.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;           
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("developer@esolztech.com", "4p2$Z2s1s");

            
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {
            new LogWriter("ProjectID : " + Project_ID + ",\r\n" + "   PurchasedOrderId : " + poid + ",\r\n" + "   CC Mail : " + sCCEmail + ",\r\n" + "   ActionName : " + ActionName + ",\r\n" + "   FileSizeInKB : " + totalAttachmentSizeKB + ",\r\n" + "   Email exception :" + ex.Message.ToString());
        }

    }

}
