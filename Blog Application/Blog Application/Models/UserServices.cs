using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Application.Models
{
    public class UserServices
    {
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BolgAppDataBase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        //return all users in saved in data base
        internal List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM [User]";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    users.Add(new User
                    {
                        FirstName = dataReader[1] as string,
                        LastName = dataReader[2] as string,
                        Username = (dataReader[3] as string).Trim(),
                        Email = dataReader[4] as string,
                        ProfilePicSrc = dataReader[6] as string
                    });

                }
            }
            finally
            {
                con.Close();
            }
            return users;
        }

        //delete all the posts of specific user
        internal void DeleteAllPosts(string username)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM Post";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                string error = string.Empty;
                while (dataReader.Read())
                {
                    if ((dataReader[3] as string).Trim() == username)
                    {
                        DeletePost(Convert.ToInt32(dataReader[0]), out error);
                    }
                }
            }
            finally
            {
                con.Close();
            }
        }

        //delte account of given user
        internal void DeleteUser(string username)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {

                string query = "DELETE\n" +
                        "FROM [User]\n" +
                        "WHERE Username=@Username";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);

                int deletedRow = cmd.ExecuteNonQuery();
               
            }
            finally
            {
                con.Close();
            }
        }

        //return the post having given post id
        internal Post GetPost(int postId)
        {
            Post post = new Post();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM Post";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    if(Convert.ToInt32(dataReader[0]) == postId)
                    {
                        post.PostId = Convert.ToInt32(dataReader[0]);
                        post.Title = dataReader[1] as string;
                        post.Content = dataReader[2] as string;
                        post.Name = GetNameOfUser((dataReader[3] as string).Trim());
                        post.Username = (dataReader[3] as string).Trim();
                        post.PostDate = (DateTime)dataReader[4];
                        post.UserProfilePicSrc = GetUserProfilePicSrc((dataReader[3] as string).Trim());
                    }
                }
            }
            finally
            {
                con.Close();
            }
            return post;
        }

        //return all the posts saved in data base
        internal List<Post> GetAllPosts()
        {
            List<Post> posts = new List<Post>();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM Post";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    posts.Add(new Post
                    {
                        PostId=Convert.ToInt32(dataReader[0]),
                        Title = dataReader[1] as string,
                        Content = dataReader[2] as string,
                        Name = GetNameOfUser((dataReader[3] as string).Trim()),
                        Username= (dataReader[3] as string).Trim(),
                        PostDate = (DateTime)dataReader[4],
                        UserProfilePicSrc=GetUserProfilePicSrc((dataReader[3] as string).Trim())
                    });
                   
                }
            }
            finally
            {
                con.Close();
            }
            return posts;
        }

        //update the given post given
        internal bool UpdatePost(Post post, string error)
        {
            error = "some error occur";
            bool isUpdated = false;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                string query = "UPDATE Post\n" +
                    $"SET Title=@Title, Content=@Content, PostTime=@PostTime\n" +
                    $"WHERE Id=@PostId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Title", post.Title);
                cmd.Parameters.AddWithValue("@Content", post.Content);
                cmd.Parameters.AddWithValue("@PostTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@PostId", post.PostId);

                int updated = cmd.ExecuteNonQuery();

                if (updated >= 1)
                {
                    isUpdated = true;
                }
                else
                {
                    isUpdated = false;
                    error = "some error occur";
                }
            }
            catch (Exception)
            {
                isUpdated = false;
                error = "some error occur";
            }
            finally
            {
                con.Close();
            }
            return isUpdated;

        }

        //update the user profile info given
        internal bool UpdateUser(SignInUser user, out string error)
        {
            if(CheckIsEmailExistAlready(user, out error))
            {
                return false;
            }
            error = "some error occur";
            bool isUpdated = false;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                string query = string.Empty;
                if (!string.IsNullOrEmpty(user.NewPassword))
                {
                    query = "UPDATE [User]\n" +
                    $"SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Password=@Password, ImageSrc=@ImageSrc\n" +
                    $"WHERE Username=@Username";
                }
                else
                {
                    query = "UPDATE [User]\n" +
                    $"SET FirstName=@FirstName, LastName=@LastName, Email=@Email, ImageSrc=@ImageSrc\n" +
                    $"WHERE Username=@Username";
                }
                

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                if (!string.IsNullOrEmpty(user.NewPassword))
                {
                    cmd.Parameters.AddWithValue("@Password", user.NewPassword);
                }
                cmd.Parameters.AddWithValue("@ImageSrc", user.ProfilePicSrc);

                int updated = cmd.ExecuteNonQuery();

                if (updated >= 1)
                {
                    isUpdated = true;
                }
                else
                {
                    isUpdated = false;
                    error = "some error occur";
                }
            }
            catch (Exception e)
            {
                isUpdated = false;
                error = "some error occur";
            }
            finally
            {
                con.Close();
            }
            return isUpdated;
        }

        //return true if the email of iven user is already exist in data base
        private bool CheckIsEmailExistAlready(SignInUser user, out string error)
        {
            error = string.Empty;
            bool isAlreadyExist = false;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM [User]";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    if ((string)dataReader[3] != user.Username && (string)dataReader[4] == user.Email)
                    {
                        error = "Email already exists.";
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        isAlreadyExist = true;
                        break;
                    }
                }

            }
            catch(Exception e)
            {
                error = "some error occur";
                return true;
            }
            finally
            {
                con.Close();
            }
            return isAlreadyExist;
        }

        //return the instance of user has given username
        internal SignInUser GetUser(string username)
        {
            SignInUser user = null;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM [User]";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    if ((dataReader[3] as string).Trim() == username)
                    {
                        user = new SignInUser
                        {
                            FirstName = dataReader[1] as string,
                            LastName = dataReader[2] as string,
                            Username = dataReader[3] as string,
                            Email = dataReader[4] as string,
                            PreviousPassword = dataReader[5] as string,
                            ProfilePicSrc=dataReader[6] as string,
                        };
                    }
                }
            }
            finally
            {
                con.Close();
            }
            return user;
        }

        //delete the post has given post id
        internal bool DeletePost(int postId, out string error)
        {
            error = "some error occur";
            bool isDeleted = false;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {

                string query = "DELETE\n" +
                        "FROM Post\n" +
                        "WHERE Id=@PostId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PostId", postId);

                int deletedRow = cmd.ExecuteNonQuery();


                if (deletedRow >= 1)
                {
                    isDeleted = true;
                }
                else
                {
                    isDeleted = false;
                    error = "some error occur";
                }

            }
            catch (Exception)
            {
                isDeleted = false;
                error = "some error occur";
            }
            finally
            {
                con.Close();
            }
            return isDeleted;
        }

        //return the save location of profile pic of given user
        internal string GetUserProfilePicSrc(string username)
        {
            string profilePicSrc = string.Empty;

        SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM [User]";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    if ((string)dataReader[3] == username)
                    {
                        profilePicSrc = dataReader[6] as string;
                        break;
                    }
                }


            }
            finally
            {
                con.Close();
            }
            return profilePicSrc;
        }

        //return the full name of the asked user
        private string GetNameOfUser(string username)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM [User]";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    if ((string)dataReader[3] == username)
                    {
                        username = (string)dataReader[1];
                        username += " " + (string)dataReader[2];
                        break;
                    }
                }


            }
            finally
            {
                con.Close();
            }
            return username;
        }

        //save the given user in data base
        public bool AddUser(SignUpUser user, out string error)
        {
            if (CheckIsUsernameOrEmailExistAlready(user, out error))
            {
                return false;
            }

            error = "";
            bool isInserted = false;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                string query = "INSERT INTO\n" +
                        "[User](FirstName, LastName, Username, Email, Password, ImageSrc)\n" +
                        $"VALUES(@FirstName, @LastName, @Username, @Email, @Password, @ImageSrc)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@ImageSrc", User.defaultProfilePicSrc);

                int insertedRow = cmd.ExecuteNonQuery();


                if (insertedRow >= 1)
                {
                    isInserted = true;
                }
                else
                {
                    error = "some error occur";
                    isInserted = false;
                }

            }
            catch (Exception )
            {
                error = "some error occur";
                isInserted = false;
            }
            finally
            {
                con.Close();
            }
            return isInserted;
        }

        //save the post in data base
        internal bool SavePost(Post post, string error)
        {
            error = "some error occur.";
            bool isInserted = false;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                string query = "INSERT INTO\n" +
                        "Post(Title, Content, Name, PostTime)\n" +
                        $"VALUES(@Title, @Content, @Name, @PostTime)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Title", post.Title);
                cmd.Parameters.AddWithValue("@Content", post.Content);
                cmd.Parameters.AddWithValue("@Name", post.Username);
                cmd.Parameters.AddWithValue("@PostTime", post.PostDate);

                int insertedRow = cmd.ExecuteNonQuery();


                if (insertedRow >= 1)
                {
                    isInserted = true;
                }
                else
                {
                    error = "some error occur";
                    isInserted = false;
                }

            }
            catch (Exception )
            {
                error = "some error occur";
                isInserted = false;
            }
            finally
            {
                con.Close();
            }
            return isInserted;
        }

        //return true if user with given credentials exists in data base
        internal bool CheckCredentials(UserCredentials userCredentials, out string error)
        {
            error = "Incorrect username.";
            bool isUserExist = false;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM [User]";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    if ((string)dataReader[3]==userCredentials.Username)
                    {
                        if((string)dataReader[5] == userCredentials.Password)
                        {
                            isUserExist = true;
                            error = string.Empty;
                            break;
                        }
                        else
                        {
                            isUserExist = false;
                            error = "Incorrect password.";
                            break;
                        }
                        
                    }
                }


            }
            catch (Exception)
            {
                isUserExist = false;
                error = "some error occur";

            }
            finally
            {
                con.Close();
            }
            return isUserExist;
        }

        //return true if the email or username of given user already resides in data base
        private bool CheckIsUsernameOrEmailExistAlready(SignUpUser user, out string error)
        {
            error = string.Empty;
            bool isAlreadyExist = false;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                string query = "SELECT * FROM [User]";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    if ((string)dataReader[3]==user.Username)
                    {
                        error = "Username already exists.";
                    }
                    else if ((string)dataReader[4] == user.Email)
                    {
                        error = "Email already exists.";
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        isAlreadyExist = true;
                        break;
                    }
                }


            }
            catch (Exception)
            {
                isAlreadyExist = true;
                error = "some error occur";

            }
            finally
            {
                con.Close();
            }
            return isAlreadyExist;
        }
    
    
    }
}
