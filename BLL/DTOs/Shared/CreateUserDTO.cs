using DAL.EF.Models;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTOs.Shared
{
    public class CreateUserDTO
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public Guid TenantId { get; set; }
    }

}
