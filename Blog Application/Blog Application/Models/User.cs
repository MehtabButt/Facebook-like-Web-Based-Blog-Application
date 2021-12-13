using System.ComponentModel.DataAnnotations;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Blog_Application.Models
{
    public class User
    {
        //profile pic loaction of user just create an account
        public static string defaultProfilePicSrc = "Default.jpg";

        //first name of user
        [Required(ErrorMessage = "Please enter first name.")]
        public string FirstName { get; set; }

        //last name of user
        [Required(ErrorMessage = "Please enter last name.")]
        public string LastName { get; set; }

        //username of user
        [Required(ErrorMessage = "Please enter a username.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Username cannot have white spaces")]
        public string Username { get; set; }

        //email of user
        [Required(ErrorMessage = "Please enter email adress.")]
        [EmailAddress]
        public string Email { get; set; }

        //save location of profile pic of user
        public string ProfilePicSrc { get; set; }

    }
}
