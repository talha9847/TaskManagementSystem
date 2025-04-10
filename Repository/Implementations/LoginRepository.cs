using Npgsql;
namespace MyApp.Namespace;

public class LoginRepository : ILoginInterface
{
    private readonly string connectionString = "Server=cipg01;Port=5432;Database=intern042;User Id=postgres;Password=123456;";

    public LoginRepository()
    {


    }
    public User GetUserByEmail(string email, string password)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            var query = "SELECT * FROM t_employees WHERE c_email=@identifier and c_password=@password";
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@identifier", email);
                cmd.Parameters.AddWithValue("@password", password);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            c_EmpID = reader.GetInt32(0),
                            c_Name = reader.GetString(1),
                            c_Email = reader.GetString(2),
                            c_Password = reader.GetString(3),
                            c_Image = reader.GetString(4)
                        };
                    }
                }
            }
        }
        return null;
    }

    public async Task<int> GetAdmin(string email, string password)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            var query = "SELECT * FROM t_admin WHERE c_email=@identifier and c_password=@password";
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@identifier", email);
                cmd.Parameters.AddWithValue("@password", password);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return 1;
                    }
                }
            }
        }
        return -1;
    }

}