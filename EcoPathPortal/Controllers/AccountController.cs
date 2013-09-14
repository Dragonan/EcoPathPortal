using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Recaptcha;
using System.Text.RegularExpressions;
using EcoPathPortal.Models;

namespace EcoPathPortal.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [RecaptchaControlMvc.CaptchaValidator]
        public ActionResult Login(Models.Login _login, bool captchaValid, string captchaErrorMessage)
        {
            if (Convert.ToInt32(Session["FailedLogins"]) > 3 && !captchaValid)
            {
                ModelState.AddModelError("captcha", captchaErrorMessage);
            }

            if (ModelState.IsValid)
            {
                User_Account user = _login.IsValid();
                if (user != null)
                {
                    if (!user.Confirmed)
                    {
                        ModelState.AddModelError("userError", "Този потребителски акаунт не е потвърден. Изпратено е ново съобщение за потвърждение на посочения e-mail.");
                        EmailManager.SendConfirmationEmail(_login.UserName);
                        return View(_login);
                    }
                    FormsAuthentication.SetAuthCookie(_login.UserName, _login.RememberMe);
                    Session.Contents.Remove("FailedLogins");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("userError", "Въвели сте грешни данни!");
                }
            }

            if (Session["FailedLogins"] != null)
                Session["FailedLogins"] = Convert.ToInt32(Session["FailedLogins"]) + 1;
            else Session["FailedLogins"] = 1;

            return View(_login);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [RecaptchaControlMvc.CaptchaValidator]
        public ActionResult Register(Models.Register _register, bool captchaValid, string captchaErrorMessage)
        {
            if (!captchaValid)
            {
                ModelState.AddModelError("captcha", captchaErrorMessage);
            }

            if (_register.NameExists())
            {
                ModelState.AddModelError("userNameExists", "Това име се използва от друг потребител.");
            }

            if (_register.EmailExists())
            {
                ModelState.AddModelError("emailExists", "Този e-mail се използва от друг потребител.");
            }

            if (ModelState.IsValid)
            {
                _register.Create();
                EmailManager.SendConfirmationEmail(_register.UserName);
                return RedirectToAction("RegSuccessful", "Account");
            }
            return View(_register);
        }

        public ActionResult RegSuccessful()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Verify(string id)
        {
            if (string.IsNullOrEmpty(id) || (!Regex.IsMatch(id, @"[0-9a-f]{8}\-([0-9a-f]{4}\-){3}[0-9a-f]{12}")))
            {
                ViewBag.Msg = "Грешен линк";
                return View();
            }
            else
            {
                EcoPathDBEntities _context = new EcoPathDBEntities();
                User_Account user = (from ua in _context.User_Accounts
                                     where ua.Id == new Guid(id)
                                     select ua).FirstOrDefault();
                if (user != null)
                {
                    if (!user.Confirmed)
                    {
                        user.Confirmed = true;
                        _context.SaveChanges();
                        ViewBag.Msg = "Вие успешно потвърдихте Вашият потребителски акаунт! Ще бъдете прехвърлени на началната страница след 5 секунди.";
                        FormsAuthentication.SetAuthCookie(user.Username, false);
                        return View();
                    }
                    else
                    {
                        FormsAuthentication.SignOut();
                        ViewBag.Msg = "Този потребителски акаунт е вече потвърден";
                        return View();
                    }
                }
                else
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Msg = "Не съществува потребител с този код.";
                    return View();
                }
            }
        }
    }
}
