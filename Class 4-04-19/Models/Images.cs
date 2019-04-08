using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web; 

namespace Class_4_04_19.Models
{
    public class Images
    {
        public string Password { get; set; }
        public string FileName { get; set; }
        public int id { get; set; }
        public string text { get; set; }
        public int Count;                                 
    }

    public class ViewModel
    {
        public Images Image { get; set; }
        public bool IncorrectPassword { get; set; }
        public string Password { get; set; }
    }   

    public class Users
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int id { get; set; }
        public string Password { get; set; }
    }

    public class ViewModel2
    {
        public int id { get; set; }         
        public bool IsAuthenticated { get; set; }
    }

    public class PassWordManager
    {
        private string _connectionString;

        public PassWordManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddImage(Images i, string password, int userid)
        {                     
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into images values (@filename,@password,@count,@userid) select SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@filename", i.FileName);
            cmd.Parameters.AddWithValue("@password", i.Password);
            cmd.Parameters.AddWithValue("@count", i.Count);
            cmd.Parameters.AddWithValue("@userid",userid);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public void AddUser(Users user)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hash;
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into users values(@name, @email, @password)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@password", user.Password);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public Users GetEmail(string email)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from users where email=@email";
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Users users = new Users();
            if (!reader.Read())
            {
                return null;
            }

            {
                users.Name = (string)reader["Name"];
                users.Email = (string)reader["email"];
                users.Password = (string)reader["Password"];
                users.id = (int)reader["id"];  
            }
            return users;
        }
          
        public Users Login(string email, string password)
        {
            Users user = GetEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isValid)
            {
                return null;
            }

            return user;
        }         

        public Images GetImage(int id, string hi)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from images where id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Images images = new Images();
            while (reader.Read())
            {
                images.FileName = (string)reader["FileName"];
                images.Password = (string)reader["Password"];
                images.id = (int)reader["id"];
                images.text = hi;
                images.Count = (int)reader["count"];
            }
            return images;
        }

        public int Count(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select count from images where id= @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count;
        }

        public int AddToCount(int count, int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            count++;
            cmd.CommandText = "update images set count=@count where id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@count", count);
            conn.Open();
            cmd.ExecuteNonQuery();
            return count;
        }

        public IEnumerable<Images> MyAccount(int userid)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from images where userid= @id";
            cmd.Parameters.AddWithValue("@id", userid);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Images>images = new List<Images>();
            while (reader.Read())
            {   
                images.Add(new Images
                {
                    FileName = (string)reader["FileName"],
                    Password = (string)reader["Password"],
                    Count = (int)reader["count"]
                });   
            }
            return images;
        }
    }
}