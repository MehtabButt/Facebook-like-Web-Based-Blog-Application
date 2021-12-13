using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Application.Models
{
    public class SignUpUser:User
    {
        //password of user want to create account
        [Required(ErrorMessage = "Please enter password.")]
        [StringLength(20, ErrorMessage = "Password must be 8-20 character long", MinimumLength = 8)]
        public string Password { get; set; }

        //confirm password of user want to create account
        //must be match to password
        [Required(ErrorMessage = "Please enter the password again.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        public string PasswordConfirm { get; set; }
    }
}
