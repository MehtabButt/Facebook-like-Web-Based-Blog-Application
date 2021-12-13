using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Application.Models
{
    public class NewPasswordAttribute: ValidationAttribute
    {
        //new password added by the account holder logged in user
       private string NewPassword { get; set; }
       
        //intialize the new password
        public NewPasswordAttribute(string newPassword)
        {
            NewPassword = newPassword;
           
        }

        //if user want to add new password he must first validate his old password 
        //it validates that is user has add old password if he wants to update his password
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string SelectedNewPassword = (string)validationContext.ObjectType.GetProperty(NewPassword).GetValue(validationContext.ObjectInstance, null);
            if (!string.IsNullOrEmpty(SelectedNewPassword))
            {
                if (string.IsNullOrEmpty(value as string))
                {
                    return new ValidationResult("You need to verify your old password first.");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }



    }
}
