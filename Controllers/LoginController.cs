using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace

{
    public class LoginController : Controller
    {
        // GET: LoginController
        private readonly ILoginInterface _loginRepository;
        private readonly RedisService _redisService;
        private readonly IUserInterface _userRepository;

        
        public LoginController(ILoginInterface loginRepository, IUserInterface userRepository, RedisService redisService)
        {
            this._loginRepository = loginRepository;
            this._userRepository = userRepository;
            this._redisService = redisService;
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Dashboard()
        {

            List<Notification> notifications = _redisService.GetNotifications();

            // Pass the notifications to the View
            return View(notifications);
        }
        [HttpPost]
        public ActionResult DismissNotification(string notificationId)
        {
            var redisService = new RedisService();
            redisService.RemoveNotification(notificationId);
            return Json(new { success = true });
        }

        public ActionResult AdminLogin()
        {

            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Login(string Email, string password)
        {
            var user = _loginRepository.GetUserByEmail(Email, password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.c_EmpID);
                TempData["LoginSuccess"] = "You have logged in successfully!";
                return RedirectToAction("Dashboard"); // Redirect to dashboard
            }
            else
            {
                TempData["LoginFailed"] = "Invalid email or password!";
                return RedirectToAction("Login"); // Redirect back to login page
            }
        }






        public IActionResult UserProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                var user = _userRepository.GetEmpById(userId.Value);

                if (user != null)
                {
                    return View(user);
                }
                else
                {
                    ViewBag.ErrorMessage = "User not found.";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult User()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                var user = _userRepository.GetEmpById(userId.Value);

                if (user != null)
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images", $"{user.c_Email}.jpg");
                    if (!System.IO.File.Exists(imagePath))
                    {
                        imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images", $"{user.c_Email}.png");
                        if (!System.IO.File.Exists(imagePath))
                        {
                            user.c_Image = null;
                        }
                        else
                        {
                            user.c_Image = $"/profile_images/{user.c_Email}.png";
                        }
                    }
                    else
                    {
                        user.c_Image = $"/profile_images/{user.c_Email}.jpg";
                    }

                    return View(user); // This will include the updated user data, including the image
                }
                else
                {
                    ViewBag.ErrorMessage = "User not found.";
                    return View("Error");
                }
            }

            return RedirectToAction("Login", "Login");
        }



        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                var user = _userRepository.GetEmpById(userId.Value);
                if (user != null)
                {
                    return View(user);
                }
                else
                {
                    ViewBag.ErrorMessage = "User not found.";
                    return View("Error");
                }
            }

            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public IActionResult UpdateProfile(User user, IFormFile profileImage)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                var existingUser = _userRepository.GetEmpById(userId.Value);
                if (existingUser != null)
                {
                    existingUser.c_Name = user.c_Name;
                    existingUser.c_Password = user.c_Password;

                    if (profileImage != null && profileImage.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(profileImage.FileName).ToLower();

                        if (fileExtension == ".jpg" || fileExtension == ".png")
                        {
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images", $"{existingUser.c_Email}{fileExtension}");

                            // Save the file to the specified path
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                profileImage.CopyTo(fileStream);
                            }

                            existingUser.c_Image = $"{existingUser.c_Email}{fileExtension}";
                        }
                        else
                        {
                            // Handle invalid file format
                            ViewBag.ErrorMessage = "Invalid image format. Only .jpg and .png are allowed.";
                            return View("Error");
                        }
                    }

                    _userRepository.UpdateUser(existingUser);

                    // Redirect to User page after the profile update
                    return RedirectToAction("User");
                }
                else
                {
                    ViewBag.ErrorMessage = "User not found.";
                    return View("Error");
                }
            }

            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public IActionResult EditPassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "New passwords do not match.");
                return View("EditPassword");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine("kfdlk " + userId);

            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var users = _userRepository.GetEmpById(userId.Value);
            Console.WriteLine(users);

            if (users == null)
            {
                System.Console.WriteLine("not null");
                ModelState.AddModelError("", "User not found.");
                return View("EditPassword");
            }


            users.c_Password = newPassword;
            System.Console.WriteLine("user found");
            bool isPasswordUpdated = await _userRepository.UpdatePasswordAsync(users);

            if (isPasswordUpdated)
            {
                TempData["SuccessMessage"] = "Password updated successfully!";
                return RedirectToAction("UserProfile");
            }

            ModelState.AddModelError("", "Failed to update password.");
            return View("EditPassword");
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            try
            {
                var admin = await _loginRepository.GetAdmin(email, password);
                if (admin == 1)
                {
                    return RedirectToAction("AdminDashboard");
                }
                else
                {
                    return View();
                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: " + ex.Message);
                return RedirectToAction("Login");
            }
        }




    }
}