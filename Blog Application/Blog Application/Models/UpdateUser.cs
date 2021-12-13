using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Application.Models
{
    //used when admin want to update the user info 
    public class UpdateUser:User
    {
        //password of user whom admin want to update account
        [StringLength(20, ErrorMessage = "Password must be 8-20 character long", MinimumLength = 8)]
        public string Password { get; set; }

        //confirm password of user whom admin want update account
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        public string PasswordConfirm { get; set; }
    }
}
