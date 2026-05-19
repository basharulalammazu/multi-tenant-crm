using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.EF.Models
{
    public class AppUser : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }   
        public bool IsActive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        
        // Navigation Properties
        public Tenant Tenant { get; set; } = null!;
        public ICollection<Lead> AssignedLeads { get; set; } = new List<Lead>();
        public ICollection<Deal> OwnedDeals { get; set; } = new List<Deal>();
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        
        // Computed Property
        public string FullName => $"{FirstName} {LastName}";
    }
}
