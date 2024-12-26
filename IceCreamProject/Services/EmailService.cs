using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public class EmailService
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("Ice Cream App", "pnamanh98@gmail.com")); // Thay bằng email của bạn
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("plain")
        {
            Text = message
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync("pnamanh98@gmail.com", "rxss ekhj rjei saea"); // Thay thông tin thật
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}
