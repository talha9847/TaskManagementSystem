namespace MyApp.Namespace;

public interface ITaskInterface
{
    Task<List<Tasks>> GetTasksByEmpIdAsync(int empId);
    Task<Tasks> GetTaskByIdAsync(int taskId);
    Task DeleteTaskAsync(int taskId);
    Task UpdateTaskAsync(Tasks task);
    Task CreateTaskAsync(Tasks task);
}