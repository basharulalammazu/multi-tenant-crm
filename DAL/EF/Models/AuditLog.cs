using DAL.Enums;
using System;
using System.Collections.Generic;
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
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public string? IpAddress { get; set; }
    }
}
