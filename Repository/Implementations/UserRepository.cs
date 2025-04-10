namespace MyApp.Namespace
{
    using Npgsql;
    using System.Collections.Generic;

    public class UserRepository : IUserInterface
    {
        private readonly string _connectionString = "Server=cipg01;Port=5432;Database=intern042;User Id=postgres;Password=123456;";

        public UserRepository() { }

        public User GetEmpById(int userId)
        {
            User user = null;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM t_Employees WHERE c_empid = @UserId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                c_EmpID = reader.GetInt32(0),
                                c_Name = reader.GetString(1),
                                c_Email = reader.GetString(2),
                                c_Password = reader.GetString(3),
                                c_Image = reader.IsDBNull(4) ? null : reader.GetString(4)
                            };
                        }
                    }
                }

            }

            return user;
        }


        public void UpdateUser(User user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = "UPDATE t_Employees SET c_name = @Name, c_profileImage = @ProfileImage WHERE c_empid = @EmpId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmpId", user.c_EmpID);
                    command.Parameters.AddWithValue("@Name", user.c_Name);
                    // command.Parameters.AddWithValue("@Password", user.c_Password);
                    command.Parameters.AddWithValue("@ProfileImage", user.c_Image);

                    command.ExecuteNonQuery();
                }
            }


        }
        public async Task<bool> UpdatePasswordAsync(User user)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "UPDATE t_Employees SET c_password = @NewPassword WHERE c_empid = @EmpId";
                    System.Console.WriteLine("Password updating");
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmpId", user.c_EmpID);
                        command.Parameters.AddWithValue("@NewPassword", user.c_Password);

                        Console.WriteLine("Executing query...");
                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        Console.WriteLine($"Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password: {ex.Message}");
                return false;
            }
        }



        // public async Task<User> GetEmpByIdAsync(int loggedInEmpId)
        // {
        //     User user = null;
        //     try
        //     {
        //         using (var connection = new NpgsqlConnection(_connectionString))
        //         {
        //             await connection.OpenAsync();
        //             string query = "SELECT * FROM t_Employees WHERE c_empid = @UserId";

        //             using (var command = new NpgsqlCommand(query, connection))
        //             {
        //                 command.Parameters.AddWithValue("@UserId", loggedInEmpId);

        //                 using (var reader = await command.ExecuteReaderAsync())
        //                 {
        //                     if (await reader.ReadAsync())
        //                     {
        //                         user = new User
        //                         {
        //                             c_EmpID = reader.GetInt32(0),
        //                             c_Name = reader.GetString(1),
        //                             c_Email = reader.GetString(2),
        //                             c_Password = reader.GetString(3),
        //                             c_Image = reader.IsDBNull(4) ? null : reader.GetString(4)
        //                         };
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error fetching user: {ex.Message}");
        //     }

        //     return user;
        // }

    }
}