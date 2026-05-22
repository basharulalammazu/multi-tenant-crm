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
        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column(TypeName = "VARCHAR(255)")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }   
        public bool IsActive { get; set; }
        [StringLength(500)]
        [Column(TypeName = "VARCHAR(500)")]
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
