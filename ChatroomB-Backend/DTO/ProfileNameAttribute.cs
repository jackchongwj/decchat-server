using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ChatroomB_Backend.DTO
{
    public class ProfileNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var profileName = value as string ?? "";

            if (profileName.StartsWith(' '))
            {
                return new ValidationResult("Profile name cannot start with a space.");
            }

            if (profileName.Length > 15)
            {
                return new ValidationResult("Profile name cannot exceed 15 characters.");
            }

            if (!Regex.IsMatch(profileName, @"^[\p{L}\p{N}_-]*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled))
            {
                return new ValidationResult("Profile name contains invalid characters.");
            }

            return ValidationResult.Success;
        }
    }

}
