using FluentEmail.Core;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Services.Repository
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }
        public async Task<bool> sendEmail(string toEmail, string body, string subject)
        {
            var response = await _fluentEmail.To(toEmail).Subject(subject).Body(body).SendAsync();

            if(response.Successful)
            {
                return true;
            }

            return false;
        }
    }
}
