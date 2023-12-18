using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Team22.Web.Contexts;
using Team22.Web.Enums;
using Team22.Web.Helpers;
using Team22.Web.Interfaces;

namespace Team22.Web.Services;

public class EmailService : IEmailService
{
    private readonly Team22Context _context;
    private readonly ILogger<EmailService> _logger;
    public AuthMessageSenderOptions Options { get; set; }

    public EmailService(Team22Context context, IOptions<AuthMessageSenderOptions> options, ILogger<EmailService> logger)
    {
        _context = context;
        Options = options.Value;
        _logger = logger;
    }

    private class EmailModel
    {
        public string Subject { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string PlainContent { get; set; } = null!;
        public string? HtmlContent { get; set; }
    }
    
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(Options.ApiKey))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(Options.ApiKey, subject, message, toEmail);
    }

    private async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("jspencershaw@gmail.com"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        var dummy = response.StatusCode;
        var dummy2 = response.Headers;
        _logger.LogInformation(response.IsSuccessStatusCode
            ? $"Email to {toEmail} queued successfully!"
            : $"Failure Email to {toEmail}");
    }

    private async Task<Response> SendEmail(EmailModel model)
    {
        var apiKey = Environment.GetEnvironmentVariable("SendGrid API Key", EnvironmentVariableTarget.User);
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("jkriste@clemson.edu", "DriveTime");
        var to = new EmailAddress(model.UserEmail, model.UserName);
        var msg = MailHelper.CreateSingleEmail(from, to, model.Subject, model.PlainContent, model.HtmlContent);
        return await client.SendEmailAsync(msg);
    }

    #region Verify Email Service

    public class VerifyEmailQuery
    {
        public int UserId { get; set; }
    }

    public async Task<QueryStatus> SendVerifyEmail(VerifyEmailQuery request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user is null)
        {
            return QueryStatus.NotFound;
        }

        var verification = user.Verification;

        if (verification.Status != VerificationStatus.EmailPending)
        {
            return QueryStatus.Conflict;
        }

        var response = await SendEmail(new EmailModel
        {
            Subject = "DriveTime - Verify Email Address",
            UserEmail = user.Email,
            UserName = $"{user.FirstName} {user.LastName}",
            PlainContent = $"Hello {user.Email},\n\tThank you for signing up for DriveTime. Below is a link to" +
                           $"verify your email.\nhttps://localhost:5011/verify/{user.Id}/{verification.Secret}"
        });

        verification.Status = VerificationStatus.EmailSent;
        await _context.SaveChangesAsync();
        return response.IsSuccessStatusCode ? QueryStatus.NoContent : QueryStatus.Invalid;
    }
    
    #endregion
}