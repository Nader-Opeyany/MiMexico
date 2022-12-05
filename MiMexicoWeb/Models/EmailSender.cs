using Microsoft.AspNetCore.Identity.UI.Services;

namespace MiMexicoWeb.Models
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Fake implementation to get rid of error.
            return Task.CompletedTask;
        }
    }
}
