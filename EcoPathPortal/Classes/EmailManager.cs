using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using EcoPathPortal.Models;

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
                        message.Subject = "Потвърдете вашата регистрация в Еко-Пътеки БГ";
                        message.Body = "<html><head><meta content=\"text/html; charset=utf-8\" /></head><body><p>"
                            + userName + ", </p><p>За да потвърдите вашият акаунт, моля натиснете този линк:</p>"
                            + "<p><a href=\"" + verifyUrl + "\" target=\"_blank\">" + verifyUrl
                            + "</a></p><div>Благодарим ви че се регистрирахте,</div><div>Екипът на еcopath.bg</div><p>Не отговаряйте на "
                            + "този e-mail. Линкът за потвърждаване е личен и не трябва да го споделяте.</p></body></html>";

                        message.IsBodyHtml = true;

                        client.EnableSsl = true;
                        client.Send(message);
                    };
                };
            }
        }
    }
}