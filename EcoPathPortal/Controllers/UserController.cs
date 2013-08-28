using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcoPathPortal.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        [HttpGet]
        public ActionResult Profile()
        {
            if (Request.IsAuthenticated)
            {
                Models.User model = new Models.User(User.Identity.Name);
                return View(model);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Profile(Models.User _user)
        {
            if (!_user.IsPasswordValid())
                ModelState.AddModelError("invalidPassword", "Въвели сте грешна парола.");

            if (ModelState.IsValid)
            {
                _user.Save();
                return RedirectToAction("Index", "Home");
            }

            return View(_user);
        }
    }
}
