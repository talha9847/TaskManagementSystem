using FluentValidation;

namespace MyApp.Namespace
{
    public class RegisterVMValidator : AbstractValidator<User>
    {
        public RegisterVMValidator()
        {
            // Name validation (Not empty and length check)
            RuleFor(x => x.c_Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");

            // Email validation (Not empty and format check)
            RuleFor(x => x.c_Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            // Password validation (Not empty and length check)
            RuleFor(x => x.c_Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            // Confirm password validation (Not empty and match check)
            RuleFor(x => x.c_ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(x => x.c_Password).WithMessage("Password and Confirm Password must match");

            // Image file validation (Check file extension)
            RuleFor(x => x.ProfilePicture)
                .NotEmpty().WithMessage("Profile image is required")
                .Must(file => file != null && (file.ContentType == "image/jpeg" || file.ContentType == "image/png"))
                .WithMessage("Profile image must be in JPEG or PNG format");
        }
    }
}
