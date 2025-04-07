using FluentEmail.Core;
using Microsoft.Extensions.Options;
using NEXT.GEN.Models;
using NEXT.GEN.Models.EmailModel;
using NEXT.GEN.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace NEXT.GEN.Services.Repository
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IFluentEmail fluentEmail, IOptions<SmtpSettings> smtpSettings)
        {
            _fluentEmail = fluentEmail;
            _smtpSettings = smtpSettings.Value;
        }
        public async Task<bool> sendEmail(string toEmail, string body, string subject)
        {
            var response = await _fluentEmail.To(toEmail).Subject(subject).Body(body,true).SendAsync();

            if (response.Successful)
            {
                return true;
            }

            return false;
        }
    }
}


//var smtpClient = new SmtpClient(_smtpSettings.Host)
//{
//    Port = _smtpSettings.Port,
//    EnableSsl = true,
//    Credentials = new NetworkCredential(_smtpSettings.FromEmail,_smtpSettings.Password)
//};

//var message = new MailMessage
//{
//    From = new MailAddress(_smtpSettings.FromEmail),
//    Subject = subject,
//    Body = body,
//    IsBodyHtml = true
//};
//message.To.Add(toEmail);
//await  smtpClient.SendMailAsync(message);

//return true;