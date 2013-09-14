using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace EcoPathPortal.Models
{
    public class Login
    {
        EcoPathDBEntities _context = new EcoPathDBEntities();

        [Required(ErrorMessage="Трябва да въведете потребителско име")]
        [Display(Name = "Потребителско име")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Трябва да въведете парола")]
        [Display(Name = "Парола")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Паролата не трябва да съдържа невалидни символи")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомни ме")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <returns>The user data if found, else null</returns>
        public User_Account IsValid()
        {
            Password = Helpers.SHA1.Encode(Password);
            var result = (from ua in _context.User_Accounts
                          where (ua.Username == UserName) && (ua.Password == Password)
                          select ua).FirstOrDefault();
             
            return result;
        }
    }
}