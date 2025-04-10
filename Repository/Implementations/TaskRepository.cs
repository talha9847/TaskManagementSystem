using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using Npgsql;
namespace MyApp.Namespace;
public class TaskRepository : ITaskInterface
{
    private readonly string connectionString = "Server=cipg01;Port=5432;Database=intern042;User Id=postgres;Password=123456;";

    private readonly NotificationService _notification;
    public TaskRepository(NotificationService notification){
        _notification=new NotificationService();
    }



    public async Task<List<Tasks>> GetTasksByEmpIdAsync(int empId)
    {
        List<Tasks> tasks = new List<Tasks>();

        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new NpgsqlCommand("SELECT * FROM t_tasks WHERE c_EmpId = @EmpId", connection);
            command.Parameters.AddWithValue("@EmpId", empId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    tasks.Add(new Tasks
                    {
                        c_TaskId = reader.GetInt32(reader.GetOrdinal("c_TaskId")),
                        c_Title = reader.GetString(reader.GetOrdinal("c_Title")),
                        c_Description = reader.GetString(reader.GetOrdinal("c_Description")),
                        c_EstimateDays = reader.GetInt32(reader.GetOrdinal("c_EstimatedDays")),
                        c_startDate = reader.GetDateTime(reader.GetOrdinal("c_startDate")),
                        c_endDate = reader.GetDateTime(reader.GetOrdinal("c_endDate")),
                        c_Status = reader.GetString(reader.GetOrdinal("c_Status")),
                        c_EmpId = reader.GetInt32(reader.GetOrdinal("c_EmpId"))
                    });
                }
            }
        }
        return tasks;
    }




    public async Task<Tasks> GetTaskByIdAsync(int taskId)
    {
        Tasks task = null;

        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new NpgsqlCommand("SELECT * FROM t_tasks WHERE c_TaskId = @TaskId", connection);
            command.Parameters.AddWithValue("@TaskId", taskId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    task = new Tasks
                    {
                        c_TaskId = reader.GetInt32(reader.GetOrdinal("c_TaskId")),
                        c_Title = reader.GetString(reader.GetOrdinal("c_Title")),
                        c_Description = reader.GetString(reader.GetOrdinal("c_Description")),
                        c_EstimateDays = reader.GetInt32(reader.GetOrdinal("c_EstimatedDays")),
                        c_startDate = reader.GetDateTime(reader.GetOrdinal("c_startDate")),
                        c_endDate = reader.GetDateTime(reader.GetOrdinal("c_endDate")),
                        c_Status = reader.GetString(reader.GetOrdinal("c_Status")),
                        c_EmpId = reader.GetInt32(reader.GetOrdinal("c_EmpId"))
                    };

                    if (!reader.IsDBNull(reader.GetOrdinal("c_endDate")))
                    {
                        task.c_endDate = reader.GetDateTime(reader.GetOrdinal("c_endDate"));
                    }
                }
            }
        }
        return task;
    }




    public async Task CreateTaskAsync(Tasks task)
    {
        try
        {


            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                System.Console.WriteLine("Inseting is workinggggg....");
                var command = new NpgsqlCommand("INSERT INTO t_tasks (c_Title, c_Description, c_EstimatedDays, c_startDate,c_endDate, c_Status, c_EmpId) VALUES (@Title, @Description, @EstimateDays, @StartDate,@endDate, @Status, @EmpId)", connection);
                command.Parameters.AddWithValue("@Title", task.c_Title);
                command.Parameters.AddWithValue("@Description", task.c_Description);
                command.Parameters.AddWithValue("@EstimateDays", task.c_EstimateDays);
                command.Parameters.AddWithValue("@StartDate", task.c_startDate);
                command.Parameters.AddWithValue("@endDate", task.c_endDate);
                command.Parameters.AddWithValue("@Status", task.c_Status);
                command.Parameters.AddWithValue("@EmpId", task.c_EmpId);

                await command.ExecuteNonQueryAsync();

                _notification.HandleTaskEvent("new taskk","ByUser",$"{task.c_Title} is added");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting task: {ex.Message}");
            throw;
        }
    }




    public async Task UpdateTaskAsync(Tasks task)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var checkCommand = new NpgsqlCommand("SELECT COUNT(*) FROM t_tasks WHERE c_TaskId = @TaskId", connection);
            checkCommand.Parameters.AddWithValue("@TaskId", task.c_TaskId);

            var taskExists = (long)await checkCommand.ExecuteScalarAsync() > 0;

            if (!taskExists)
            {
                Console.WriteLine($"Task with ID {task.c_TaskId} does not exist.");
                return;
            }

            var checkEmpCommand = new NpgsqlCommand("SELECT COUNT(*) FROM t_employees WHERE c_EmpId = @EmpId", connection);
            checkEmpCommand.Parameters.AddWithValue("@EmpId", task.c_EmpId);

            var empExists = (long)await checkEmpCommand.ExecuteScalarAsync() > 0;

            if (!empExists)
            {
                Console.WriteLine($"Employee with ID {task.c_EmpId} does not exist.");
                return;
            }

            var command = new NpgsqlCommand("UPDATE t_tasks SET c_Title = @Title, c_Description = @Description, c_EstimatedDays = @EstimateDays, c_startDate = @StartDate, c_endDate = @EndDate, c_Status = @Status, c_EmpId = @EmpId WHERE c_TaskId = @TaskId", connection);

            command.Parameters.AddWithValue("@TaskId", task.c_TaskId);
            command.Parameters.AddWithValue("@Title", task.c_Title);
            command.Parameters.AddWithValue("@Description", task.c_Description);
            command.Parameters.AddWithValue("@EstimateDays", task.c_EstimateDays);
            command.Parameters.AddWithValue("@StartDate", task.c_startDate);
            command.Parameters.AddWithValue("@EndDate", task.c_endDate);
            command.Parameters.AddWithValue("@Status", task.c_Status);
            command.Parameters.AddWithValue("@EmpId", task.c_EmpId);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"Rows affected: {rowsAffected}");
        }
    }




    public async Task DeleteTaskAsync(int taskId)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var command = new NpgsqlCommand("DELETE FROM t_tasks WHERE c_TaskId = @TaskId", connection);
            command.Parameters.AddWithValue("@TaskId", taskId);

            await command.ExecuteNonQueryAsync();
        }
    }


}

