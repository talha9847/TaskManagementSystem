using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MyApp.Namespace
{
    public class RegisterController : Controller
    {
        private readonly IRegisterInterface _registerRepository;
        private readonly string? connectionString;

        public RegisterController(IRegisterInterface registerRepository)
        {
            this._registerRepository = registerRepository;
        } 
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // var emailExists = await _registerRepository.CheckIfEmailExists(user.c_Email);
                // if (emailExists)
                // {
                //     TempData["message"] = "Email is already registered. Please choose another.";
                //     return View();
                // }

                if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
                {
                    try
                    {
                        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images");
                        Directory.CreateDirectory(uploadDir);

                        var fileName = user.c_Email + Path.GetExtension(user.ProfilePicture.FileName);
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await user.ProfilePicture.CopyToAsync(stream);
                        }

                        user.c_Image = fileName;
                    }
                    catch (Exception ex)
                    {
                        TempData["message"] = "File upload failed: " + ex.Message;
                        return RedirectToAction("Register");
                    }
                }

                var status = await _registerRepository.Add(user);

                if (status == 1)
                {
                    TempData["message"] = "User Registered Successfully";
                    return RedirectToAction("Index", "Home");
                }
                else if (status == 0)
                {
                    TempData["message"] = "User Already Registered";
                }
                else
                {
                    TempData["message"] = "An error occurred during registration.";
                }
            }

            return View();
        }

        // [HttpGet]
        // public IActionResult ValidateEmail(string email)
        // {
        //     if (string.IsNullOrEmpty(email))
        //     {
        //         return Json("Invalid email");
        //     }

        //     try
        //     {
        //         using (var conn = new NpgsqlConnection(connectionString))
        //         {
        //             conn.Open();

        //             var command = new NpgsqlCommand("SELECT COUNT(*) FROM t_Employees WHERE c_Email = @c_Email", conn);
        //             command.Parameters.AddWithValue("@Email", email);

        //             var result = (long)command.ExecuteScalar(); // Returns the count of rows matching the email

        //             if (result > 0)
        //             {
        //                 return Json("Email is already registered.");
        //             }

        //             return Json("Email is available.");
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json("An error occurred while validating the email.");
        //     }
        // }

    }
}

