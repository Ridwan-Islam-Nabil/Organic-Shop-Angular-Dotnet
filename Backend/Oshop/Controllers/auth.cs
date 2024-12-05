using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using Oshop.Models;

namespace Oshop.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> dbUser;

        public Auth(IConfiguration configuration)
        {
            _configuration = configuration;

            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("Oshop");
                dbUser = database.GetCollection<User>("users");
                Console.WriteLine("MongoDB connection established successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to MongoDB: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult login([FromBody] LoginRequest loginRequest)
        {
            var user = dbUser.Find(u => u.Username == loginRequest.Username).FirstOrDefault();

            if (user == null || user.Password != loginRequest.Password)
            {
                // If the user is not found or password is incorrect
                return Unauthorized(new { message = "Unauthorized" });
            }

            // Generate JWT token
            var token = GenerateToken(user);

            return Ok(new { message = "Login successful", token });
        }

        private string GenerateToken(User user)
        {
            var secretKey = _configuration["Keys:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set admin claim based on the user's IsAdmin field
            var isAdmin = user.IsAdmin == "1" ? "1" : "0";

            var claims = new[]
            {
                new Claim("name", user.Username), // Use the username from the MongoDB user
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique ID for the token
                new Claim("admin", isAdmin) // Add admin claim
            };
            // Set the expiration for the token (e.g., 1 hour)
            var expirationTime = DateTime.UtcNow.AddHours(12); // Token expiration time

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expirationTime, // Set expiration time
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string print()
        {
            return "Hi";
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        public class User
        {
            public ObjectId _id { get; set; } // MongoDB default ObjectId
            public string Username { get; set; }
            public string Password { get; set; }
            public string IsAdmin { get; set; } // Assuming IsAdmin is stored as a string ("1" or "0")
        }
    }
}
