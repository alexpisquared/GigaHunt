﻿using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mail;
//using EASendMail; //add EASendMail namespace

namespace Emailing
{
  public class Office365_POC
  {
    public void Nogo_Jun2020() 
    {
      try
      {
        //ThreadPool.QueueUserWorkItem(t =>              {
        var client = new SmtpClient("smtp.office365.com", 587) { EnableSsl = true, Credentials = new System.Net.NetworkCredential("alex.pigida@outlook.com", "hello me") };
        var message = new MailMessage(new MailAddress("alex.pigida@outlook.com", string.Empty, System.Text.Encoding.UTF8), new MailAddress("pigida@gmail.com"))
        {
          Body = "This is your body message",
          BodyEncoding = System.Text.Encoding.UTF8,
          Subject = "Subject",
          SubjectEncoding = System.Text.Encoding.UTF8
        };

        //client.TargetName = "STARTTLS/smtp.office365.com";

        //client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);      // Set the method that is called back when the send operation ends.
        //client.SendAsync(message, message);      // The userState can be any object that allows your callback method to identify this send operation. For this example, I am passing the message itself

        client.Send(message);
        //});
      }
      catch (System.Exception ex)
      {
        Debug.WriteLine(ex);
      }
    }


    private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
      // Get the message we sent
      var msg = (MailMessage)e.UserState;

      if (e.Cancelled)
      {
        // prompt user with "send cancelled" message 
      }
      if (e.Error != null)
      {
        // prompt user with error message 
      }
      else
      {
        // prompt user with message sent!
        // as we have the message object we can also display who the message
        // was sent to etc 
      }

      // finally dispose of the message
      if (msg != null)
        msg.Dispose();
    }

    //static void Main__(string[] args)
    //{
    //  var oMail = new SmtpMail("TryIt");
    //  var oSmtp = new SmtpClient();

    //  // Your Offic 365 email address
    //  oMail.From = "myid@mydomain";

    //  // Set recipient email address
    //  oMail.To = "support@emailarchitect.net";

    //  // Set email subject
    //  oMail.Subject = "test email from office 365 account";

    //  // Set email body
    //  oMail.TextBody = "this is a test email sent from c# project.";

    //  // Your Office 365 SMTP server address,
    //  // You should get it from outlook web access.
    //  var oServer = new SmtpServer("smtp.office365.com")
    //  {

    //    // user authentication should use your
    //    // email address as the user name.
    //    User = "myid@mydomain",
    //    Password = "yourpassword",

    //    // Set 587 port
    //    Port = 587,

    //    // detect SSL/TLS connection automatically
    //    ConnectType = SmtpConnectType.ConnectSSLAuto
    //  };

    //  try
    //  {
    //    Trace.WriteLine("start to send email over SSL...");
    //    oSmtp.SendMail(oServer, oMail);
    //    Trace.WriteLine("email was sent successfully!");
    //  }
    //  catch (Exception ep)
    //  {
    //    Trace.WriteLine("failed to send email with the following error:");
    //    Trace.WriteLine(ep.Message);
    //  }
    //}
  }
}