using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BLL.DTOs.Auth
{
    public class TenantDTO
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Subdomain { get; set; } = string.Empty;
        public PlanType Plan { get; set; } = PlanType.Free;
        public bool IsActive { get; set; } = true;
        public DateTime? TrialEndsAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
