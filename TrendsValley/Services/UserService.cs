using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Cryptography;
using System.Text;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;

namespace TrendsValley.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ValidateUser(string username, string password)
        {

            var pass = HashPassword(password); // Hash the password before checking it

            var user = await _db.customers.FirstOrDefaultAsync(u => u.Username == username && u.Password == pass);

            if (user == null)
            {
                return false; // Invalid username or password
            }
            else
            {
                return true; // User found
            }
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert password to byte array
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Hash the password
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash to a base64 string
                return Convert.ToBase64String(hashBytes);
            }
        }

        public async Task<bool> RegisterUser(Models.Models.Customer obj, string role)
        {
            if (await _db.customers.AnyAsync(u => u.Username == obj.Username))
                return false; // User already exists

            var pass = HashPassword(obj.Password); // Hash the password before storing it

            obj.Password = pass; // Store the hashed password

            _db.customers.Add(obj);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _db.customers.AnyAsync(u => u.Username == username);
        }

        public async Task<Models.Models.Customer> GetUserByUsername(string username)
        {
            return await _db.customers.FirstOrDefaultAsync(u => u.Username == username);
        }

    }
}
