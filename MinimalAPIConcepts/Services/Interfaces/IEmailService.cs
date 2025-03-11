namespace NEXT.GEN.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> sendEmail(string toEmail, string body , string subject);
    }
}
