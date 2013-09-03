using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoPathPortal.Models
{
    public class EcoPathModel
    {
        EcoPathDBEntities _context = new EcoPathDBEntities();

        public EcoPath Entity { get; set; }
        public String PathCity { get; set; }
        public int entId { get; set; }
        public String newCommText { get; set; }

        public EcoPathModel()
        {
            Entity = new EcoPath();
        }

        public EcoPathModel(EcoPath ecoPath)
        {
            Entity = ecoPath;
            entId = ecoPath.Id;

            var loggedInUser = HttpContext.Current.User.Identity.Name;
            string selectedCity = null;
            if (loggedInUser != null)
            {
                selectedCity = (from u in _context.User_Info
                                where u.User_Accounts.Username == loggedInUser
                                select u.City1.EngName).FirstOrDefault();
            }
            if (String.IsNullOrEmpty(selectedCity)) PathCity = Entity.City.EngName;
            else PathCity = selectedCity;
        }

        public IList<System.Web.Mvc.SelectListItem> GetCities()
        {
            var Cities = new List<System.Web.Mvc.SelectListItem>();
            List<City> cities = _context.Cities.ToList();

            foreach (var item in cities)
            {
                var listItem = new System.Web.Mvc.SelectListItem();
                listItem.Text = item.Name;
                listItem.Value = item.EngName;

                if (PathCity == item.EngName)
                        listItem.Selected = true;

                Cities.Add(listItem);
            }

            return Cities;
        }

        public Dictionary<String, String> GetImages()
        {
            var images = (from i in _context.Images
                where i.EcoPathId == entId
                select i).ToList();
            var imagePaths = new Dictionary<String, String>();

            foreach (var item in images)
            {
                var localPath = "~/Content/Images/" + entId + "/" + item.ImageName;
                    
                if (!imagePaths.ContainsKey(localPath))
                    imagePaths.Add(localPath, item.Title);
            }

            return imagePaths;
        }
    }
}