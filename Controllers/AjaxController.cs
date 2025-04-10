using MyApp.Namespace;
using Microsoft.AspNetCore.Mvc;


namespace MyApp.Namespace
{
    public class AjaxController : Controller
    {
        private readonly ITaskInterface _taskRepository;

        public AjaxController(ITaskInterface taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public IActionResult Index()
        {
            return View();
        }




        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            int? empId = HttpContext.Session.GetInt32("UserId"); // Make sure UserId is stored in session

            if (empId == null)
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            var tasks = await _taskRepository.GetTasksByEmpIdAsync(empId.Value);

            if (tasks == null || !tasks.Any())
            {
                return Json(new { success = false, message = "No tasks found." });
            }

            return Json(new { success = true, data = tasks });
        }

        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] Tasks task)
        {
            // Get User ID from session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            if (task == null)
            {
                return Json(new { success = false, message = "Invalid task data received." });
            }

            try
            {
                task.c_EmpId = userId.Value; // Assign User ID from session

                await _taskRepository.CreateTaskAsync(task); // Save task to database

                return Json(new { success = true, message = "Task added successfully!", data = task });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "Error adding task.", error = ex.Message });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
                return Json(new { success = false, message = "Task not found." });

            return Json(new { success = true, data = task });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromBody] Tasks task)
        {
            if (task == null)
            {
                Console.WriteLine("Received null task object.");
                return Json(new { success = false, message = "Invalid task data received." });
            }

            Console.WriteLine("Received UpdateTask request."); // Debugging log

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                Console.WriteLine("User not logged in.");
                return Json(new { success = false, message = "User is not logged in." });
            }

            task.c_EmpId = userId ?? 0; // Ensure a valid int value

            // Ensure null values are handled
            task.c_Status ??= "Pending"; // Default status
            // task.c_startDate ??= DateTime.UtcNow; // Default start date
            // task.c_endDate ??= DateTime.UtcNow.AddDays(7); // Default end date

            // Debug log
            Console.WriteLine($"Task ID: {task.c_TaskId}, Title: {task.c_Title}, " +
                $"Description: {task.c_Description}, Estimate Days: {task.c_EstimateDays}, " +
                $"Start Date: {task.c_startDate}, End Date: {task.c_endDate}, " +
                $"Status: {task.c_Status}, Employee ID: {task.c_EmpId}");

            try
            {
                await _taskRepository.UpdateTaskAsync(task);
                Console.WriteLine("Task updated successfully!");
                return Json(new { success = true, message = "Task updated successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating task: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the task." });
            }
        }







        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            try
            {
                if (taskId <= 0)
                {
                    return Json(new { success = false, message = "Invalid Task ID" });
                }

                await _taskRepository.DeleteTaskAsync(taskId); // Call repository method

                return Json(new { success = true, message = "Task deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }



    }
}
