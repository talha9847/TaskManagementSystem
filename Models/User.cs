using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;

namespace MyApp.Namespace;
public class User
{
    public int c_EmpID { get; set; }

    public string c_Name { get; set; }

    public string c_Email { get; set; }

    public string c_Password { get; set; }
    [Compare("c_Password",ErrorMessage ="password does not match")]
    public string c_ConfirmPassword { get; set; }

    public string? c_Image { get; set; }

    public IFormFile ProfilePicture { get; set; }
}