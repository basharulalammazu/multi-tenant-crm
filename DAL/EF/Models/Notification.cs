using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace DAL.EF.Models
{
    public class Notification : BaseEntity
    {
        [ForeignKey("Recipient")]
        public Guid RecipientId { get; set; }
        public NotificationType Type { get; set; }  

        [Required]
        [StringLength(500)]
        [Column(TypeName = "VARCHAR(500)")]
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        [StringLength(500)]
        [Column(TypeName = "VARCHAR(500)")]
        public string? ActionUrl { get; set; }


        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual AppUser Recipient { get; set; } = null!;
    }
}
