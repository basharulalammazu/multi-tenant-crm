using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.EF.Models
{
    public class AuditLog
    {
        public long Id { get; set; }
        public Guid TenantId { get; set; }
        public EntityType EntityName { get; set; }  
        public Guid EntityId { get; set; }
        public AuditActionType Action { get; set; }  // Added/Modified/Deleted

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? OldValues { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? NewValues { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }

        [StringLength(45)]
        [Column(TypeName = "VARCHAR(45)")]
        public string? IpAddress { get; set; }
    }
}
