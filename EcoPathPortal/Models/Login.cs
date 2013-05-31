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

        [Required(ErrorMessage="You must enter your username")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "You must enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>The user data if found, else null</returns>
        public User_Account IsValid(string _username, string _password)
        {
            _password = Helpers.SHA1.Encode(_password);
            var result = (from ua in _context.User_Accounts
                          where (ua.Username == _username) && (ua.Password == _password)
                          select ua).FirstOrDefault();
             
            return result;
        }
    }
}