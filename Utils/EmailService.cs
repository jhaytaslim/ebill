using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using ebill.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ebill.Utils
{
    public interface IEmailService
    {
        //Task SendEmailAsync(string email, string subject, string message);

        Task<string> SendEmail(MailVM model);
    }
    public class EmailService : IEmailService
    {
        public EmailService(IConfiguration configuration, ILoggerFactory log, IEmailSettings emailSettings)
        {
            Configuration = configuration;
            _log = log.CreateLogger<EmailService>();
            _emailSettings = emailSettings;
        }
        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    return Execute(Configuration["SendGridKey"], subject, message, email);
        //}
        //private readonly IHostingEnvironment _env;

        //public EmailSender(
        //    IOptions<EmailSettings> emailSettings,
        //    IHostingEnvironment env)
        //{
        //    _emailSettings = emailSettings.Value;
        //    _env = env;
        //}

        //public async Task SendEmailAsync(string email, string subject, string message)
        //{
        //    try
        //    {
        //        var mimeMessage = new MimeMessage();

        //        mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));

        //        mimeMessage.To.Add(new MailboxAddress(email));

        //        mimeMessage.Subject = subject;

        //        mimeMessage.Body = new TextPart("html")
        //        {
        //            Text = message
        //        };

        //        //using (var client = new SmtpClient())
        //        //{
        //        //    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
        //        //    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

        //        //    if (_env.IsDevelopment())
        //        //    {
        //        //        // The third parameter is useSSL (true if the client should make an SSL-wrapped
        //        //        // connection to the server; otherwise, false).
        //        //        await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, true);
        //        //    }
        //        //    else
        //        //    {
        //        //        await client.ConnectAsync(_emailSettings.MailServer);
        //        //    }

        //        //    // Note: only needed if the SMTP server requires authentication
        //        //    await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);

        //        //    await client.SendAsync(mimeMessage);

        //        //    await client.DisconnectAsync(true);
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        // TODO: handle exception
        //        throw new InvalidOperationException(ex.Message);
        //    }
        //}

        public IConfiguration Configuration { get; }
        public ILogger _log { get; }
        public IEmailSettings _emailSettings { get; }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            //var client = new SendGridClient(apiKey);
            //var msg = new SendGridMessage()
            //{
            //    From = new EmailAddress("apptest@courtevillegroup.com", "Samiat from School Management Solution"),
            //    Subject = subject,
            //    PlainTextContent = message,
            //    HtmlContent = message
            //};
            //msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            //msg.TrackingSettings = new TrackingSettings
            //{
            //    ClickTracking = new ClickTracking { Enable = true }
            //};

            return Task.CompletedTask;//client.SendEmailAsync(msg);
        }


        public async Task<string> SendEmail(MailVM model)
        {
            try
            {
                var message = new MimeMessage();
                //message.To.Add(new MailboxAddress(model.Recipient));
                //message.To.Add(new MailboxAddress("jhaytaslim@gmail.com"));

                message.From.Add(new MailboxAddress(_emailSettings.Sender));

                message.Subject = model.Title;
                if (model.Copy != null)
                    message.Cc.AddRange(model.Copy.Select(c => { return new MailboxAddress(c); }));
                //We will say we are sending HTML. But there are options for plaintext etc. 
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = model.Message
                };

                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (var emailClient = new SmtpClient())
                {
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect(_emailSettings.MailServer, _emailSettings.MailPort, false);

                    //Remove any OAuth functionality as we won't be using it. 
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    await emailClient.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);

                    await emailClient.SendAsync(message);

                    emailClient.Disconnect(true);

                    return "Email Sent Successfully!";
                }
            }
            catch (Exception e)
            {
                _log.LogError(e.Message + e.StackTrace);
                return e.Message;
            }
        }
    }

    // return Task.CompletedTask;
    //}

    public class EmailSettings : IEmailSettings
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
        public string Recipient { get; set; }
    }

    public interface IEmailSettings
    {
        string MailServer { get; set; }
        int MailPort { get; set; }
        string SenderName { get; set; }
        string Sender { get; set; }
        string Password { get; set; }
        string Recipient { get; set; }
    }

}
