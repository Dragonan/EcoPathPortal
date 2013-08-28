using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcoPathPortal.Models;

namespace EcoPathPortal.Controllers
{
    public class EcoPathController : Controller
    {
        //
        // GET: /EcoPath/
        
        [HttpGet]
        public ActionResult Index(string id = null)
        {
            var _context = new EcoPathDBEntities();
            int pathId;

            if (id == null) return RedirectToAction("Index", "Home");
            if (!Int32.TryParse(id, out pathId)) return RedirectToAction("Index", "Home");

            var ecoPath = (from e in _context.EcoPaths
                           where e.Id == pathId
                           select e).FirstOrDefault();

            if (ecoPath == null) return RedirectToAction("Index", "Home");

            var model = new EcoPathModel(ecoPath);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Models.EcoPathModel model)
        {
            var newComment = new Comment();
            var _context = new EcoPathDBEntities();

            newComment.EcoPathId = model.entId;
            var id = (from u in _context.User_Accounts
                      where u.Username == User.Identity.Name
                      select u.Id).FirstOrDefault();

            if (id != null)
            {
                newComment.UserId = id;
                newComment.Text = model.newCommText;
                newComment.Date = DateTime.Now;
                _context.Comments.AddObject(newComment);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", new { id = model.entId });
        }
    }
}
