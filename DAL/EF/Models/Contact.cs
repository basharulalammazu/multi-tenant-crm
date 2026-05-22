using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace DAL.EF.Models
{
    public class Contact : BaseEntity
    {
        public Guid? LeadId { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string? Email { get; set; }

        [StringLength(30)]
        [Column(TypeName = "VARCHAR(30)")]
        public string? Phone { get; set; }

        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string? CompanyName { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string? JobTitle { get; set; }

        [StringLength(200)]
        [Column(TypeName = "VARCHAR(200)")]
        public string? Website { get; set; }

        [StringLength(1000)]
        [Column(TypeName = "VARCHAR(1000)")]
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;

        public virtual Lead? Lead { get; set; }

        public virtual ICollection<ContactNote> ContactNotes { get; set; } = new List<ContactNote>();

        public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();

        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
