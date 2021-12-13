using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Application.Models
{
    public class UserCredentials
    {
        //username of logging user
        [Required(ErrorMessage ="Please enter username.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Username cannot have white spaces")]
        public string Username { get; set; }

        //password of logging user
        [Required(ErrorMessage ="Please enter password.")]
        public string Password { get; set; }
    }
}
