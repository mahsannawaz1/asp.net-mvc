using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using System;
using System.Security.Cryptography;
namespace FreelanceMarketPlace.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public bool Login(Users user)
        {
            bool result = false;
            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                connect.Open();
                string LoginQuery = "SELECT UserPassword FROM Users WHERE UserEmail = @email";
                using (SqlCommand cmd = new SqlCommand(LoginQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@email", user.UserEmail);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            string storedHash = reader["UserPassword"] as string;
                            if (storedHash != null)
                            {
                                // Verify the provided password
                                result = VerifyPassword(storedHash, user.UserPassword);
                            }
                          
                        } 
                    }
                }
            }
            return result;

        }

        public bool SignUp(Users user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object is null");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentException("Role is required", nameof(role));
            }

            try
            {
                using (SqlConnection connect = new SqlConnection(ConnectionString))
                {
                    connect.Open();
                    SqlTransaction transaction = connect.BeginTransaction();
                    try
                    {
                        // Insert into User table
                        string RegisterQuery = "INSERT INTO [Users] (FirstName, LastName, UserEmail, UserPassword) VALUES (@first_name, @last_name, @email, @password); SELECT SCOPE_IDENTITY();";
                        int userId = 0;

                        using (SqlCommand cmd = new SqlCommand(RegisterQuery, connect, transaction))
                        {
                            // Hash the password
                            var hashedPassword = HashPassword(user.UserPassword);

                            // Prevent SQL injection
                            cmd.Parameters.AddWithValue("@first_name", user.FirstName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@last_name", user.LastName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@email", user.UserEmail ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@password", hashedPassword ?? (object)DBNull.Value);

                            var result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                userId = Convert.ToInt32(result);
                            }
                            else
                            {
                                throw new Exception("Failed to retrieve the user ID.");
                            }
                        }

                        // Insert into Freelancer or Client table based on role
                        if (role == "freelancer")
                        {
                            Console.WriteLine(userId);
                            string InsertFreelancerQuery = "INSERT INTO Freelancer (UserId) VALUES (@user_id);";
                            using (SqlCommand cmd = new SqlCommand(InsertFreelancerQuery, connect, transaction))
                            {
                                cmd.Parameters.AddWithValue("@user_id", userId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (role == "client")
                        {
                            string InsertClientQuery = "INSERT INTO Client (UserId) VALUES (@user_id);";
                            using (SqlCommand cmd = new SqlCommand(InsertClientQuery, connect, transaction))
                            {
                                cmd.Parameters.AddWithValue("@user_id", userId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Commit transaction
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        // Rollback transaction in case of an error
                        transaction.Rollback();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Console.WriteLine(ex);
                return false;
            }
        }


        public Users Profile(Users user)
        {
            return new Users();
        }

        private string HashPassword(string password)
        {
            // Generate a salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            // Combine salt and hashed password
            return Convert.ToBase64String(salt) + ":" + hashed;
        }

        private bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2)
            {
                throw new FormatException("Invalid password format.");
            }

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            // Hash the provided password using the same salt
            var providedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: providedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            return providedHash == storedHash;
        }


    }
}
