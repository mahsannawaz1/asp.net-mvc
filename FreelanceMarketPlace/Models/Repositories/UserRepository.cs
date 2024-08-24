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
        public (bool isAuthenticated,string role) Login(Users user)
        {
            var result = (false, "");
            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                connect.Open();
                string LoginQuery = "SELECT UserPassword,Role FROM Users WHERE UserEmail = @email";
                using (SqlCommand cmd = new SqlCommand(LoginQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@email", user.UserEmail);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            string storedHash = reader["UserPassword"] as string;

                            string role = reader["Role"] as string;
                            Console.WriteLine(role);
                            if (storedHash != null)
                            {
                                // Verify the provided password
                                result = VerifyPassword(storedHash, user.UserPassword) ? (true,role) : result;
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
                        string RegisterQuery = "INSERT INTO [Users] (FirstName, LastName, UserEmail, UserPassword, Role) VALUES (@first_name, @last_name, @email, @password,@role); SELECT SCOPE_IDENTITY();";
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
                            cmd.Parameters.AddWithValue("@role", role ?? (object)DBNull.Value);
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

        public Users GetUserProfile(string email)
        {
            Users user = new Users();
            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                connect.Open();
                string LoginQuery = "SELECT * FROM Users WHERE UserEmail = @email";
                using (SqlCommand cmd = new SqlCommand(LoginQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                                user = new Users
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    UserEmail = reader["UserEmail"] as string,
                                    CNIC = reader.IsDBNull(reader.GetOrdinal("CNIC")) ? null : reader["CNIC"] as string,
                                    FirstName = reader["FirstName"] as string,
                                    LastName = reader["LastName"] as string,
                                    PaypalEmail = reader.IsDBNull(reader.GetOrdinal("PaypalEmail")) ? null : reader["PaypalEmail"] as string,
                                    Availability = reader.GetInt32(reader.GetOrdinal("Availability")) == 1,
                                    Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"] as string,
                                    AddressId = reader.IsDBNull(reader.GetOrdinal("AddressId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AddressId")),
                                    Role = reader["Role"] as string,
                                    CardId = reader.IsDBNull(reader.GetOrdinal("CardId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CardId")),
                                };
                            
                        }
                    }
                }
            }
            return user;
        }

        public Client GetClientProfile(int userId)
        {
            Client client = new Client();
            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                connect.Open();
                string LoginQuery = "SELECT * FROM Client WHERE UserId = @userId";
                using (SqlCommand cmd = new SqlCommand(LoginQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            client = new Client {
                                ClientId = reader.GetInt32(reader.GetOrdinal("ClientId")),
                                AmountSpent = reader["AmountSpent"] != DBNull.Value ? (decimal)reader["AmountSpent"] : 0,
                                JobCount = 0
                            };
                        }
                    }
                }
                if (client != null)
                {
                    Console.WriteLine("Coubnting");
                    
                    string jobsQuery = "SELECT COUNT(*) FROM Job WHERE ClientId = @clientId";
                    using (SqlCommand cmd = new SqlCommand(jobsQuery, connect))
                    {
                        cmd.Parameters.AddWithValue("@clientId", client.ClientId);

                        
                        client.JobCount = (int)cmd.ExecuteScalar();
                    }
                }
            }
            Console.WriteLine(client.ClientId);
            return client;
        }

        public Freelancer GetFreelancerProfile(int userId)
        {
            Freelancer freelancer = new Freelancer();
            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                connect.Open();
                string LoginQuery = "SELECT * FROM Freelancer WHERE UserId = @userId";
                using (SqlCommand cmd = new SqlCommand(LoginQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {

                            freelancer = new Freelancer
                            {
                                FreelancerId = (int)reader["FreelancerId"],
                                AmountReceived = reader["AmountReceived"] != DBNull.Value ? (decimal)reader["AmountReceived"] : default(decimal),
                                GithubLink = reader["GithubLink"] != DBNull.Value ? (string)reader["GithubLink"] : null,
                                LinkedInLink = reader["LinkedInLink"] != DBNull.Value ? (string)reader["LinkedInLink"] : null,
                                PerHourRate = reader["PerHourRate"] != DBNull.Value ? (int)reader["PerHourRate"] : default(int),
                                Title = reader["Title"] != DBNull.Value ? (string)reader["Title"] : null,
                                Intro = reader["Intro"] != DBNull.Value ? (string)reader["Intro"] : null,
                                ProposalCount = 0 // Assuming this is never null and can default to 0
                            };
                        }
                    }
                }
                if (freelancer != null)
                {

                    string jobsQuery = "SELECT COUNT(*) FROM Proposal WHERE FreelancerId = @freelancerId";
                    using (SqlCommand cmd = new SqlCommand(jobsQuery, connect))
                    {
                        cmd.Parameters.AddWithValue("@freelancerId", freelancer.FreelancerId);
                        freelancer.ProposalCount = (int)cmd.ExecuteScalar();
                    }
                }
            }
            return freelancer;
        }

        public void EditFreelancerProfile(Users user, Freelancer freelancer)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update Users table
                        string updateUserSql = @"
                    UPDATE Users
                    SET FirstName = @FirstName,
                        LastName = @LastName,
                        Availability = @Availability,
                        CNIC = @CNIC,
                        PaypalEmail = @PaypalEmail,
                        Phone = @Phone
                    WHERE UserId = @UserId";

                        using (SqlCommand command = new SqlCommand(updateUserSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UserId", user.UserId);
                            command.Parameters.AddWithValue("@FirstName", user.FirstName);
                            command.Parameters.AddWithValue("@LastName", user.LastName);
                            command.Parameters.AddWithValue("@Availability", user.Availability ? 1 : 0);
                            command.Parameters.AddWithValue("@CNIC", user.CNIC);
                            command.Parameters.AddWithValue("@PaypalEmail", user.PaypalEmail);
                            command.Parameters.AddWithValue("@Phone", user.Phone);
                            command.ExecuteNonQuery();
                        }

                        // Update Freelancer table
                        string updateFreelancerSql = @"
                    UPDATE Freelancer
                    SET Title = @Title,
                        Intro = @Intro,
                        GithubLink = @GithubLink,
                        LinkedInLink = @LinkedInLink,
                        PerHourRate = @PerHourRate
                    WHERE UserId = @UserId"; 

                        using (SqlCommand command = new SqlCommand(updateFreelancerSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UserId", user.UserId); 
                            command.Parameters.AddWithValue("@Title", freelancer.Title ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Intro", freelancer.Intro ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@GithubLink", freelancer.GithubLink ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@LinkedInLink", freelancer.LinkedInLink ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PerHourRate", freelancer.PerHourRate);

                            command.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();
                        Console.WriteLine("UPDATED");
                    }
                    catch
                    {
                        // Rollback the transaction if any error occurs
                        transaction.Rollback();
                        Console.WriteLine("ERROR");
                        throw;
                    }
                }
            }
        }

        public void EditClientProfile(Users user)
        {
           
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
               
                  
                    string updateUserSql = @"
                    UPDATE Users
                    SET FirstName = @FirstName,
                        LastName = @LastName,
                        Availability = @Availability,
                        CNIC = @CNIC,
                        PaypalEmail = @PaypalEmail,
                        Phone = @Phone
                    WHERE UserId = @UserId";

                        using (SqlCommand command = new SqlCommand(updateUserSql, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", user.UserId);
                            command.Parameters.AddWithValue("@FirstName", user.FirstName);
                            command.Parameters.AddWithValue("@LastName", user.LastName);
                            command.Parameters.AddWithValue("@Availability", user.Availability ? 1 : 0);
                            command.Parameters.AddWithValue("@CNIC", user.CNIC);
                            command.Parameters.AddWithValue("@PaypalEmail", user.PaypalEmail);
                            command.Parameters.AddWithValue("@Phone", user.Phone);
                            command.ExecuteNonQuery();
                            Console.WriteLine("Updated");
                        }   
                
            }
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
