using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Application.Models
{
    public class Post
    {
        //save location of profile pic of user who is posting this post
        public string UserProfilePicSrc { get; set; }

        //post id of post
        public int PostId { get; set; }


        //title of post
        [Required(ErrorMessage ="Please enter title.")]
        public string Title { get; set; }

        //content of post
        [Required(ErrorMessage ="Please enter some content.")]
        public string Content { get; set; }

        //name of user posting
        public string Name { get; set; }

        //username of user posting
        public string Username { get; set; }

        //date and time when posted
        public DateTime PostDate { get; set; }
    }
}
