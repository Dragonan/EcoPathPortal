using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcoPathPortal.Models;
using System.IO;

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
        public ActionResult Index(Models.EcoPathModel model, HttpPostedFileBase file)
        {
            var _context = new EcoPathDBEntities();

            var id = (from u in _context.User_Accounts
                      where u.Username == User.Identity.Name
                      select u.Id).FirstOrDefault();

            if (id != null)
            {
                if (!String.IsNullOrEmpty(model.newCommText))
                {
                    var newComment = new Comment
                    {
                        EcoPathId = model.entId,
                        UserId = id,
                        Text = model.newCommText,
                        Date = DateTime.Now
                    };

                    _context.Comments.AddObject(newComment);
                    _context.SaveChanges();
                }

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fileTitle = fileName.Substring(0, fileName.LastIndexOf('.'));
                    var folderPath = HttpRuntime.AppDomainAppPath + @"\Content\Images\" + model.entId;

                    var count = 1;
                    while (System.IO.File.Exists(Path.Combine(folderPath, fileName)))
                    {
                        var imageName = fileName.Substring(0, fileName.LastIndexOf('.'));
                        var imageType = fileName.Substring(fileName.LastIndexOf('.'));
                        fileName = imageName + "(" + count + ")" + imageType;
                    }
                    var path = Path.Combine(folderPath, fileName);
                    file.SaveAs(path);

                    var newImage = new Image
                    {
                        EcoPathId = 1,
                        UserId = id,
                        ImageName = fileName,
                        Title = fileTitle
                    };

                    _context.Images.AddObject(newImage);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index", new { id = model.entId });
        }

        //[HttpPost]
        //public ActionResult Index(HttpPostedFileBase file)
        //{
        //    var _context = new EcoPathDBEntities();

        //    var id = (from u in _context.User_Accounts
        //              where u.Username == User.Identity.Name
        //              select u.Id).FirstOrDefault();
        //    if (id != null)
        //    {
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            var fileName = Path.GetFileName(file.FileName);
        //            var path = Path.Combine(HttpRuntime.AppDomainAppPath + @"\Content\Images", fileName);
        //            file.SaveAs(path);

        //            var newImage = new Image
        //            {
        //                EcoPathId = 1,
        //                UserId = id,
        //                ImageName = fileName,
        //                Title = fileName.Substring(0, fileName.LastIndexOf('.'))
        //            };

        //            _context.Images.AddObject(newImage);
        //        }

        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("Index", new { id = 1 });
        //}
    }
}
