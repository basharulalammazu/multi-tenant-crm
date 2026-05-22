using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.EF.Models
{
    public class Lead : BaseEntity
    {
        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string Email { get; set; } = string.Empty;

        [StringLength(30)]
        [Column(TypeName = "VARCHAR(30)")]
        public string? Phone { get; set; }

        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string? Company { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string? Source { get; set; }
        public LeadStatus Status { get; set; }
        [ForeignKey("AssignedToUser")]
        public Guid? AssignedToUserId { get; set; }
        [StringLength(1000)]
        [Column(TypeName = "VARCHAR(1000)")]
        public string? Notes { get; set; }
        public DateTime? LastContactedAt { get; set; }
        public DateTime? ConvertedAt { get; set; }


        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual AppUser? AssignedToUser { get; set; }
        public virtual Contact? Contact { get; set; }
    }
}
