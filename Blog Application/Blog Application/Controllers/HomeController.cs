using Microsoft.AspNetCore.Mvc;
using Blog_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Application.Controllers
{
    public class HomeController : Controller
    {
       //admin username and password
        private UserCredentials adminCredetials = new UserCredentials
        {
            Username = "admin",
            Password = "admin"
        };

        private UserServices userServices = new UserServices();

        //return the login view
        [HttpGet]
        public ViewResult Login()
        {
            return View();
        }

        //if the user has account let him enter the application
        [HttpPost]
        public ActionResult Login(UserCredentials userCredentials)
        {
            if (ModelState.IsValid)
            {
                if (userCredentials.Username == adminCredetials.Username && userCredentials.Password == adminCredetials.Password)
                {
                    TempData["loggedInAdmin"] = adminCredetials.Username;
                    return RedirectToAction("Home", "Admin");
                }
                else
                {
                    string error = string.Empty;
                    if (userServices.CheckCredentials(userCredentials, out error))
                    {
                        TempData["loggedInUser"] = userCredentials.Username;
                        return RedirectToAction("Home", "User");

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error);
                        return View();
                    }
                }
                
               
            }
            else
            {
                return View();
            }
        }

        //return the signup view where user can create account
        [HttpGet]
        public ViewResult SignUp()
        {
            return View();
        }

        //create the account of the user with given info
        [HttpPost]
        public ViewResult SignUp(SignUpUser user)
        {
            if (ModelState.IsValid)
            {
                string error = string.Empty;
                if(userServices.AddUser(user, out error))
                {
                    return View("Login");
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

        //let the user to leave the app
        public ViewResult LogOut()
        {
            TempData.Clear();
            return View("Login");
        }
    }
}
