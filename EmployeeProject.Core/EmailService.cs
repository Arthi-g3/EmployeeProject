using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;
using EmployeeProject.Model;

namespace EmployeeProject.Core
{
    public class EmailService:IEmailService

    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string to,string subject,string body)
        {
            using var message = new MailMessage();

            message.From = new MailAddress(
                _settings.SenderEmail,
                _settings.SenderName);

            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var smtp = new SmtpClient(
                _settings.SmtpServer,
                _settings.SmtpPort);

            await smtp.SendMailAsync(message);
        }
    }
}

