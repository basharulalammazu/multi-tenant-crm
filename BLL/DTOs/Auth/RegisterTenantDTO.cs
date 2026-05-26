using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BLL.DTOs.Auth
{
    public class RegisterTenantDTO
    {
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(150,
            ErrorMessage = "Company name cannot exceed 150 characters.")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subdomain is required.")]
        [RegularExpression(
            @"^[a-z0-9-]+$",
            ErrorMessage = "Subdomain can contain only lowercase letters, numbers, and hyphens."
        )]
        [StringLength(50,
            ErrorMessage = "Subdomain cannot exceed 50 characters.")]
        public string Subdomain { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admin email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100,
            ErrorMessage = "Email cannot exceed 100 characters.")]
        public string AdminEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admin password is required.")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string AdminPassword { get; set; } = string.Empty;

        [StringLength(50,
            ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50,
            ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; }
    }
}
