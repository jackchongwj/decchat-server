using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ChatroomB_Backend.DTO
{
    public class PasswordStrengthAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? password = value as string;
            List<string> errors = new List<string>();

            // Minimum length check
            if (password?.Length < 8)
            {
                errors.Add("Password must be at least 8 characters long.");
            }

            // Maximum length check
            if (password?.Length > 20)
            {
                errors.Add("Password must not exceed 20 characters.");
            }

            // Digit check
            if (!Regex.IsMatch(password!, @"\d"))
            {
                errors.Add("Password must contain at least one digit.");
            }

            // Uppercase letter check
            if (!Regex.IsMatch(password!, @"[A-Z]"))
            {
                errors.Add("Password must contain at least one uppercase letter.");
            }

            // Lowercase letter check
            if (!Regex.IsMatch(password!, @"[a-z]"))
            {
                errors.Add("Password must contain at least one lowercase letter.");
            }

            // Special character check
            if (!Regex.IsMatch(password!, @"[$@^!%*?&]"))
            {
                errors.Add("Password must contain at least one special character.");
            }

            if (errors.Count > 0)
            {
                return new ValidationResult(string.Join(" ", errors));
            }

            return ValidationResult.Success;
        }
    }

}
