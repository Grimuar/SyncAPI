using MailKit.Net.Smtp;
using MimeKit;

namespace DreamsSyncronizer.Common.Mail;

public static class EmailService
{
    public static async Task SendEmailAsync(string email, string subject, string body)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("erase",
                                                 "erase"));

        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = body
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 465, true);
            await client.AuthenticateAsync("erase", "erase");
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}