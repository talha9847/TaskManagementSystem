

using Microsoft.AspNetCore.Connections;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace MyApp.Namespace
{
    public class RegisterRepository : IRegisterInterface
    {
        private readonly string connectionString = "Server=cipg01;Port=5432;Database=intern042;User Id=postgres;Password=123456;";

        public RegisterRepository() { }

        public async Task<int> Add(User user)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    var query = "SELECT 1 FROM t_Employees WHERE c_Email = @email;";
                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", user.c_Email);

                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                        {
                            return 0;
                        }
                        else
                        {
                            var insertQuery = @"INSERT INTO t_Employees (c_name, c_email, c_password, c_profileimage) 
                                        VALUES (@username, @email, @password, @image);";

                            await using (var cm = new NpgsqlCommand(insertQuery, conn))
                            {
                                cm.Parameters.AddWithValue("@username", user.c_Name);
                                cm.Parameters.AddWithValue("@email", user.c_Email);
                                cm.Parameters.AddWithValue("@password", user.c_Password);
                                cm.Parameters.AddWithValue("@image", user.c_Image);
                                await cm.ExecuteNonQueryAsync();
                                return 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Register Failed, Error: " + ex.Message);
                return -1;
            }
        }

        public async Task<bool> CheckIfEmailExists(string email)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();

                var query = "SELECT COUNT(*) FROM t_Employees WHERE c_email = @Email";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    var result = (long)await cmd.ExecuteScalarAsync();

                    return result > 0;
                }
            }
        }




    }


}