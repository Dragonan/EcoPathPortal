using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Configuration;

namespace EcoPathPortal.Models
{
    public class Register
    {
        EcoPathDBEntities _context = new EcoPathDBEntities();

        [Required(ErrorMessage = "Трябва да въведете потребителско име")]
        [Display(Name = "Потребителско име")]
        [StringLength(25, ErrorMessage="{0} трябва да е между {2} и {1} символа", MinimumLength=5)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Трябва да въведете e-mail")]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress, ErrorMessage="Трябва да въведете валиден e-mail адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Трябва да въведете парола")]
        [Display(Name = "Парола")]
        [DataType(DataType.Password, ErrorMessage = "Паролата не трябва да съдържа невалидни символи")]
        [StringLength(25, ErrorMessage = "{0} трябва да е между {2} и {1} символа", MinimumLength = 5)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Трябва да потвърдите паролата")]
        [Display(Name = "Повторете Паролата")]
        [Compare("Password", ErrorMessage="Паролите не съвпадат")]
        public string ComparePassword { get; set; }

        /// <summary>
        /// Checks if the user name is already taken
        /// </summary>
        /// <returns>True if it is taken, False if it is free</returns>
        public bool NameExists()
        {
            var result = from ua in _context.User_Accounts
                         where ua.Username == UserName
                         select ua;

            return result.Any();
        }

        /// <summary>
        /// Checks if the email is already taken
        /// </summary>
        /// <returns>True if it is taken, False if it is free</returns>
        public bool EmailExists()
        {
            var result = from ua in _context.User_Accounts
                         where ua.Email == Email
                         select ua;

            return result.Any();
        }

        /// <summary>
        /// Adds the new user into the database
        /// </summary>
        public void Create()
        {
            User_Account newUser = User_Account.CreateUser_Account(Guid.NewGuid(), UserName,
                Helpers.SHA1.Encode(Password), DateTime.Now, Email, false);

            _context.AddToUser_Accounts(newUser);
            _context.SaveChanges();

            User_Info newUserInfo = User_Info.CreateUser_Info(newUser.Id);

            _context.AddToUser_Info(newUserInfo);
            _context.SaveChanges();
        }
    }
}