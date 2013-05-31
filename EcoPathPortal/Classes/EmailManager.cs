using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;

namespace EcoPathPortal
{
    public class EmailManager
    {
        private const string EmailFrom = "noreply@ecopath.bg";
        public static void SendConfirmationEmail(string userName)
        {
            EcoPathDBEntities _context = new EcoPathDBEntities();
            var user = (from ua in _context.User_Accounts
                        where ua.Username == userName
                        select ua).FirstOrDefault();

            if (user != null)
            {
                var confirmationGuid = user.Id.ToString();
                var verifyUrl =
                    HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/Verify/" + confirmationGuid;

                using (var client = new SmtpClient())
                {
                    using (var message = new MailMessage(EmailFrom, user.Email))
                    {
                        message.Subject = "Please Verify your Account";
                        message.Body = "<html><head><meta content=\"text/html; charset=utf-8\" /></head><body><p>Dear "
                            + userName + ", </p><p>To verify your account, please click the following link:</p>"
                            + "<p><a href=\"" + verifyUrl + "\" target=\"_blank\">" + verifyUrl
                            + "</a></p><div>Best regards,</div><div>The EcoPath.bg Team</div><p>Do not forward "
                            + "this email. The verify link is private.</p></body></html>";

                        message.IsBodyHtml = true;

                        client.EnableSsl = true;
                        client.Send(message);
                    };
                };
            }
        }
    }
}