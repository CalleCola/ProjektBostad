// EmailSender.cs
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp-relay.brevo.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("bostadconfirmation@outlook.com", "wv8mGXNajs1RCr37"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage("bostadconfirmation@outlook.com", email)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
