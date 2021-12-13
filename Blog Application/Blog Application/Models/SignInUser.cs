using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Application.Models
{
    public class SignInUser : User
    {
        //new password of user has account
        [StringLength(20, ErrorMessage = "Password must be 8-20 character long", MinimumLength = 8)]
        public string NewPassword { get; set; }

        //old password of user has account
        [NewPassword("NewPassword")]
        public string OldPassword { get; set; }

        //current password that is saved of user has account 
        public string PreviousPassword { get; set; }

        //profile pic of user has account
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePic { get; set; }
    }
}
