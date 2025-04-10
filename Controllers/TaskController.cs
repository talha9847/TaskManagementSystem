using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    public class TaskController : Controller
    {
        private readonly ITaskInterface _taskRepository;

        public TaskController(ITaskInterface taskRepository)
        {
            _taskRepository = taskRepository;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                int? empId = HttpContext.Session.GetInt32("UserId");

                if (empId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var tasks = await _taskRepository.GetTasksByEmpIdAsync(empId.Value);

                return View(tasks);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tasks task)
        {
            int? empIdSession = HttpContext.Session.GetInt32("UserId");

            Console.WriteLine("EmpId" + empIdSession);  

            if (empIdSession != null)
            {
                task.c_EmpId = empIdSession.Value;
            }
            else
            {
                ModelState.AddModelError("", "Employee not logged in or session expired.");
                return RedirectToAction("Login", "Account");
            }

           

            if (ModelState.IsValid)
            {
                try
                {
                    await _taskRepository.CreateTaskAsync(task);
                    return RedirectToAction(nameof(Index));  
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting task: {ex.Message}");
                    ModelState.AddModelError("", "There was an error saving the task. Please try again.");
                    return View(task);
                }
            }

            return View(task);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            Console.WriteLine($"Edit GET: TaskId = {task.c_TaskId}");

            return View(task);
        }


     
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Tasks task)
        {
            Console.WriteLine($"Edit POST: Route ID = {id}, Form TaskId = {task.c_TaskId}, EmpId = {task.c_EmpId}");

            if (id != task.c_TaskId)
            {
                Console.WriteLine($"TaskId mismatch: Route ID = {id}, Form TaskId = {task.c_TaskId}");
                return NotFound();
            }



            if (ModelState.IsValid)
            {
                try
                {
                    await _taskRepository.UpdateTaskAsync(task);
                    return RedirectToAction("Index", "Task");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating task: {ex.Message}");
                    if (!await TaskExistsAsync(task.c_TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(task);
        }



        private async Task<bool> TaskExistsAsync(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            return task != null;
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            try
            {
                await _taskRepository.DeleteTaskAsync(id); 
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task: {ex.Message}");
                return View("Error"); 
            }
        }
    }

}


