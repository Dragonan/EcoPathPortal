using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EcoPathPortal.Models
{
    public class User
    {
        EcoPathDBEntities _context = new EcoPathDBEntities();

        public Guid id { get; set; }

        [Display(Name = "Име")]
        [StringLength(50, ErrorMessage = "{0} трябва да e до {1} символа")]
        public String FirstName { get; set; }

        [Display(Name = "Презиме")]
        [StringLength(50, ErrorMessage = "{0} трябва да e до {1} символа")]
        public String MiddleName { get; set; }

        [Display(Name = "Фамилия")]
        [StringLength(50, ErrorMessage = "{0} трябва да e до {1} символа")]
        public String LastName { get; set; }

        [Display(Name = "Град")]
        public int? SelectedCity { get; set; }

        [Display(Name = "Възраст")]
        [Range(0,99, ErrorMessage = "Трябва да въведете число между 0 и 99")]
        public short? Age { get; set; }

        [Display(Name = "Пол")]
        public String Gender { get; set; }

        [Display(Name = "Сегашна Парола")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Паролата не трябва да съдържа невалидни символи")]
        [DataType(DataType.Password)]
        public String OldPassword { get; set; }

        [Display(Name = "Нова Парола")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Паролата не трябва да съдържа невалидни символи")]
        [StringLength(25, ErrorMessage = "{0} трябва да е между {2} и {1} символа", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public String NewPassword { get; set; }

        [Display(Name = "Потвърдете Новата Парола")]
        [Compare("NewPassword", ErrorMessage = "Паролите не съвпадат")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Паролата не трябва да съдържа невалидни символи")]
        [DataType(DataType.Password)]
        public String ComparePassword { get; set; }

        public User() { }

        public User(string name)
        {
            User_Info entity = (from ui in _context.User_Info
                          where ui.User_Accounts.Username == name
                          select ui).FirstOrDefault();
            
            if (entity != null)
            {
                id = entity.Id;
                FirstName = entity.FirstName;
                MiddleName = entity.MiddleName;
                LastName = entity.LastName;
                SelectedCity = entity.City;
                Age = entity.Age;
                Gender = entity.Gender;
            }
        }

        /// <summary>
        /// Creates and populates a list of Cities from the database
        /// </summary>
        /// <returns>The populated list of cities</returns>
        public IList<SelectListItem> GetCities()
        {
            var Cities = new List<SelectListItem>();
            List<City> cities = _context.Cities.ToList();
            Cities.Add(new SelectListItem
            {
                Text = "Изберете Град",
                Value = "0",
                Selected = true
            });
            foreach (var item in cities)
            {
                var listItem = new SelectListItem();
                listItem.Text = item.Name;
                listItem.Value = item.Id.ToString();

                if (SelectedCity != null && SelectedCity.Value == item.Id)
                {
                    Cities[0].Selected = false;
                    listItem.Selected = true;
                }

                Cities.Add(listItem);
            }

            return Cities;
        }

        /// <summary>
        /// Checks if the entered password is correct
        /// </summary>
        /// <returns>True if Password is correct, False if it's not</returns>
        public bool IsPasswordValid()
        {
            if (String.IsNullOrEmpty(OldPassword) && String.IsNullOrWhiteSpace(OldPassword))
                return true;

            var hashPass = Helpers.SHA1.Encode(OldPassword);
            return (from ua in _context.User_Accounts
                          where ua.Id == id && ua.Password == hashPass
                          select ua).Any();
        }

        /// <summary>
        /// Commits the changes to the User into the database
        /// </summary>
        public void Save()
        {
            var entity = (from ui in _context.User_Info
                          where ui.Id == id
                          select ui).FirstOrDefault();

            if (String.IsNullOrEmpty(FirstName) && String.IsNullOrWhiteSpace(FirstName))
                entity.FirstName = null;
            else entity.FirstName = FirstName;

            if (String.IsNullOrEmpty(MiddleName) && String.IsNullOrWhiteSpace(MiddleName))
                entity.MiddleName = null;
            else entity.MiddleName = MiddleName;

            if (String.IsNullOrEmpty(LastName) && String.IsNullOrWhiteSpace(LastName))
                entity.LastName = null;
            else entity.LastName = LastName;

            entity.City = SelectedCity;

            entity.Age = Age;

            if (Gender == "m" || Gender == "f")
                entity.Gender = Gender;

            if (!String.IsNullOrEmpty(OldPassword) && !String.IsNullOrWhiteSpace(OldPassword)
                && !String.IsNullOrEmpty(NewPassword))
                entity.User_Accounts.Password = NewPassword;

            _context.User_Info.ApplyCurrentValues(entity);
            _context.SaveChanges();
        }
    }
}