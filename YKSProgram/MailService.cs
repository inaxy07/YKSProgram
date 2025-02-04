using System;
using MailKit.Net.Smtp;
using MimeKit;

namespace YKSProgram
{
    public class EmailService
    {
        private const string SenderEmail = "talanisa1905@gmail.com"; // Gönderen e-posta adresi
        private const string SenderPassword = "jluv taen ujcs aekt"; // Gönderen e-posta adresi şifresi
        private const string SmtpServer = "smtp.gmail.com"; // SMTP sunucusu
        private const int SmtpPort = 587; // SMTP portu

        public void SendEmail(string recipientEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YKS Hazırlık Programı", SenderEmail));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(SmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(SenderEmail, SenderPassword);

                    client.Send(message);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("E-posta gönderilemedi. Hata: " + ex.Message);
                }
            }
        }
    }
}
