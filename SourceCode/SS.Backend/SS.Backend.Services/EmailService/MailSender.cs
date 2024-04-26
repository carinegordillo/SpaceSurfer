using MailKit.Security;
using MimeKit;

namespace SS.Backend.Services.EmailService
{
    public class MailSender
    {
        public static async Task SendEmail(string email, string subject, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SpaceSurfers", "spacesurfers5@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = messageBody };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("spacesurfers5@gmail.com", "ewnu jgif xtjj maks");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public static async Task SendConfirmEmail(string email, string filePath, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SpaceSurfers", "spacesurfers5@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "SpaceSurfer: Reservation Confirmation Email";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body,
                TextBody = "Hello! \nThank you for reserving a space at SpaceSurfer! Please click the link to see your reservation details and add it to your calendar. \n\nBest, \nPixelPals"
            };
            bodyBuilder.Attachments.Add(filePath);
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("spacesurfers5@gmail.com", "ewnu jgif xtjj maks");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
