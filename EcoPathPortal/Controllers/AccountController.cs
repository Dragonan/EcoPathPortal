using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Recaptcha;
using System.Text.RegularExpressions;

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
        public ActionResult Login(Models.Login _login)
        {
            if (ModelState.IsValid)
            {
                User_Account user = _login.IsValid(_login.UserName, _login.Password);
                if (user != null)
                {
                    if (!user.Confirmed)
                    {
                        ModelState.AddModelError("userError", "User has not been confirmed. A new verification email has been sent.");
                        EmailManager.SendConfirmationEmail(_login.UserName);
                        return View(_login);
                    }
                    FormsAuthentication.SetAuthCookie(_login.UserName, _login.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("userError", "Login data is incorrect!");
                }
            }
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

            if (ModelState.IsValid)
            {
                bool valid = true;
                if(_register.NameExists(_register.UserName))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    valid = false;
                }
                if (_register.EmailExists(_register.Email))
                {
                    ModelState.AddModelError("Email", "Email is taken");
                    valid = false;
                }
                if (valid)
                {
                    _register.Create(_register.UserName, _register.Password, _register.Email);
                    EmailManager.SendConfirmationEmail(_register.UserName);
                    return RedirectToAction("RegSuccessful", "Account");
                }
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
                ViewBag.Msg = "Broken link error";
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
                        FormsAuthentication.SetAuthCookie(user.Username, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        FormsAuthentication.SignOut();
                        ViewBag.Msg = "Account Already Approved";
                        return View();
                    }
                }
                else
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Msg = "User id does not exist";
                    return View();
                }
            }
        } 
    }
}
