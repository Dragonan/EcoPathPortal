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

        [Required(ErrorMessage = "You must enter an username")]
        [Display(Name = "User name")]
        [StringLength(25, ErrorMessage="The {0} must be between {2} and {1} characters long", MinimumLength=5)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "You must enter an email address")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage="Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must enter a password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(25, ErrorMessage = "The {0} must be between {2} and {1} characters long", MinimumLength = 5)]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must confirm the password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage="The passwords must match")]
        public string ComparePassword { get; set; }

        /// <summary>
        /// Checks if the user name is already taken
        /// </summary>
        /// <param name="_username">User name</param>
        /// <returns>True if it is taken, False if it is free</returns>
        public bool NameExists(String _username)
        {
            var result = from ua in _context.User_Accounts
                         where ua.Username == _username
                         select ua;

            return result.Any();
        }

        /// <summary>
        /// Checks if the email is already taken
        /// </summary>
        /// <param name="_email">User email</param>
        /// <returns>True if it is taken, False if it is free</returns>
        public bool EmailExists(String _email)
        {
            var result = from ua in _context.User_Accounts
                         where ua.Email == _email
                         select ua;

            return result.Any();
        }

        /// <summary>
        /// Adds the new user into the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <param name="_email">User email</param>
        public void Create(String _username, String _password, String _email)
        {
            User_Account newUser = User_Account.CreateUser_Account(Guid.NewGuid(), _username,
                Helpers.SHA1.Encode(_password), DateTime.Now, _email, false);

            _context.AddToUser_Accounts(newUser);
            _context.SaveChanges();
        }
    }
}