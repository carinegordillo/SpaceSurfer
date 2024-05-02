using MailKit.Security;
using MimeKit;

namespace SS.Backend.Services.EmailService
{
    public class MailSender
    {
        public static async Task SendEmail(string email, string subject, string messageBody, DateTime? scheduledTime = null)
        {
            if (scheduledTime.HasValue && scheduledTime > DateTime.Now)
            {
                TimeSpan delay = scheduledTime.Value - DateTime.Now;
                await Task.Delay(delay);
            }
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

        public static async Task SendEmailWithAttachment(string email, string subject, string messageBody, string attachmentPath)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SpaceSurfers", "spacesurfers5@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var body = new TextPart("plain") { Text = messageBody };

            var attachment = new MimePart()
            {
                Content = new MimeContent(File.OpenRead(attachmentPath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(attachmentPath)
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.Add(attachment);

            message.Body = multipart;

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
