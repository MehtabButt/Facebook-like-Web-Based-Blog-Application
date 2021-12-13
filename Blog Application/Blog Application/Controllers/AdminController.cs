using Blog_Application.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Application.Controllers
{
    public class AdminController : Controller
    {
        UserServices userServices = new UserServices();

        //return true if admin is logged in otherwise returns false
        private bool CheckIfAdminIsLoggedIn()
        {
            if (TempData["loggedInAdmin"] !=null)
            {
                TempData.Keep("loggedInAdmin");
                return true;
            }
            else
            {
                TempData.Keep("loggedInAdmin");
                return false;
            }
        }

        //returns the home page of admin if logged in 
        //this view shows the details of all users have account
        public ActionResult Home()
        {
            if (!CheckIfAdminIsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            List<User> users = userServices.GetAllUsers();
            foreach(User user in users)
            {
                user.ProfilePicSrc = Path.Combine("~/images/User_Profile_Pics", user.ProfilePicSrc);

            }
            return View(users);

        }

        //remove the account of username specified if admin is logged in
        public ActionResult Delete(string username)
        {
            if (!CheckIfAdminIsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (!string.IsNullOrEmpty(username))
            {
                userServices.DeleteUser(username);
                userServices.DeleteAllPosts(username);
            }
            return RedirectToAction("Home");
        }

        //return the view create the user's account if admin is logged in
        [HttpGet]
        public ActionResult AddUser()
        {
            if (!CheckIfAdminIsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        //save the user in data base 
        [HttpPost]
        public ActionResult AddUser(SignUpUser addUser)
        {
            if (ModelState.IsValid)
            {
                string error = string.Empty;
                if(userServices.AddUser(addUser, out error))
                {
                    return RedirectToAction("Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        //return the view to update the info of user specified if admin is logged in
        [HttpGet]
        public ActionResult Update(string username)
        {
            if (!CheckIfAdminIsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            SignInUser user = userServices.GetUser(username);
            UpdateUser selectedUser = new UpdateUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                Password = user.PreviousPassword,
                PasswordConfirm = user.PreviousPassword,
                ProfilePicSrc = user.ProfilePicSrc
            };

            //selectedUser.ProfilePicSrc = Path.Combine("~/images/User_Profile_Pics", selectedUser.ProfilePicSrc);


            return View(selectedUser);
        }

        //update the info of given user 
        [HttpPost]
        public ActionResult Update(UpdateUser updatedUser)
        {
            if (ModelState.IsValid)
            {
                string error = string.Empty;
                SignInUser user = new SignInUser
                {
                    FirstName = updatedUser.FirstName,
                    LastName = updatedUser.LastName,
                    Username = updatedUser.Username,
                    Email = updatedUser.Email,
                    ProfilePicSrc = updatedUser.ProfilePicSrc,
                    NewPassword = updatedUser.Password
                };
                if(userServices.UpdateUser(user, out error))
                {
                    return RedirectToAction("Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

    }
}
