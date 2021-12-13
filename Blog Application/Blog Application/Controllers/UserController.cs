using Microsoft.AspNetCore.Mvc;
using Blog_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Blog_Application.Controllers
{
    public class UserController : Controller
    {
        private UserServices userServices = new UserServices();
        private readonly IHostingEnvironment hostingEnvironment;

        //constructor
        public UserController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        //return true if the user is logged in 
        private bool CheckIfUserIsLoggedIn()
        {
            if (TempData["LoggedInUser"] != null)
            {
                TempData.Keep("LoggedInUser");
                return true;
            }
            else
            {
                TempData.Keep("LoggedInUser");
                return false;
            }
        }

        //return the detailed view of the given post
        public ActionResult ViewPost(int postId)
        {
            if (!CheckIfUserIsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            Post post = userServices.GetPost(postId);
            if (post != null)
            {
                post.UserProfilePicSrc = Path.Combine("~/images/User_Profile_Pics", post.UserProfilePicSrc);

                return View(post);
            }
            else
            {
                return View();
            }
        }

        //return the home page has all the posts of all users 
        public ViewResult Home()
        {
            List<Post> posts = userServices.GetAllPosts();
            if (posts != null)
            {
                foreach(Post post in posts)
                {
                    post.UserProfilePicSrc = Path.Combine("~/images/User_Profile_Pics", post.UserProfilePicSrc);
                }
                return View("Home", posts);
            }
            else
            {
                return View("Home");
            }
        }

        //return the view to create post and post it 
        [HttpGet]
        public ActionResult CreatePost()
        {
            if (!CheckIfUserIsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        //save post in database
        [HttpPost]
        public ViewResult CreatePost(Post post)
        {
            if (ModelState.IsValid)
            {
                if (TempData["LoggedInUser"] != null)
                {
                    post.Username = TempData["loggedInUser"] as string;
                    TempData.Keep("loggedInUser");
                }
                post.PostDate = DateTime.Now;
                string error = string.Empty;
                if (userServices.SavePost(post, error))
                {
                    return Home();
                    //return View("Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please enter correct info");
                return View();
            }
        }

        //return the view to update the post like title and content
        [HttpGet]
        public ActionResult UpdatePost(int postId)
        {
            if (!CheckIfUserIsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            Post post = userServices.GetPost(postId);
            if (post != null)
            {
                return View(post);
            }
            else
            {
                return View();
            }
        }

        //save the updated post
        [HttpPost]
        public ViewResult UpdatePost(Post post)
        {
            if (ModelState.IsValid)
            {
                string error = string.Empty;
                if (userServices.UpdatePost(post, error))
                {
                    return Home();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please enter correct info");
                return View();
            }
        }

        //remove the given post from the database
        public ActionResult DeletePost(int postId)
        {
            if (!CheckIfUserIsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            string error = string.Empty;
            if (userServices.DeletePost(postId, out error))
            {
                return Home();
            }
            else
            {
                ModelState.AddModelError(string.Empty, error);
                return ViewPost(postId);
            }
        }

        //return the view has all profile info of current logged in user
        [HttpGet]
        public ActionResult Profile()
        {
            if (!CheckIfUserIsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            SignInUser loggedInUser = null;
            if (TempData["loggedInUser"] != null)
            {
                loggedInUser = userServices.GetUser(TempData["loggedInUser"] as string);
                TempData.Keep("loggedInUser");
            }
            if (loggedInUser != null)
            {
                loggedInUser.ProfilePicSrc = Path.Combine("~/images/User_Profile_Pics", loggedInUser.ProfilePicSrc);

                return View(loggedInUser);
            }
            else
            {
                return View();
            }
        }

        //save the updated profile info
        [HttpPost]
        public ViewResult Profile(SignInUser user)
        {
            if (ModelState.IsValid)
            {
                if(!string.IsNullOrEmpty(user.NewPassword) && user.OldPassword !=user.PreviousPassword)
                {
                    ModelState.AddModelError(string.Empty, "Old password does not match to previous password");
                    return (ViewResult)Profile();
                }
                string uniqueImageFileName = null;
                if (user.ProfilePic != null)
                {
                    DeletePreviousProfilePic(user.Username);
                    string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images/User_Profile_Pics");
                    uniqueImageFileName = Guid.NewGuid().ToString() + "_" + user.ProfilePic.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueImageFileName);
                    user.ProfilePicSrc = uniqueImageFileName;
                    user.ProfilePic.CopyTo(new FileStream(filePath, FileMode.Create));
                }


                string error = string.Empty;
                
                if (userServices.UpdateUser(user, out error))
                {
                    return Home();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                    return (ViewResult)Profile();
                }
            }
            else
            {
                return (ViewResult)Profile();
            }
           
        }

        //delete the previous profile pic of user when new one is added 
        private void DeletePreviousProfilePic(string username)
        {
            string src = userServices.GetUserProfilePicSrc(username);
            if(src!= Blog_Application.Models.User.defaultProfilePicSrc)
            {
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images/User_Profile_Pics");
                string filePath = Path.Combine(uploadFolder, src);

                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        System.IO.File.Delete(filePath);

                    }
                    catch (Exception e) { }

                }

            }
        }

    }
}
