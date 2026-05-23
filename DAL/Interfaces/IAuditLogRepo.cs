using DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IAuditLogRepo
    {
        public AuditLog? Find(Guid id, out string msg);
        public List<AuditLog> Find(out string msg);
        public List<AuditLog> FindByTenant(Guid tenantId, out string msg);
        public List<AuditLog> FindByEntity(string entityName, Guid entityId, out string msg);
        public List<AuditLog> FindByUser(string email, out string msg);
        public List<AuditLog> FindByDateRange(Guid tenantId, DateTime from, DateTime to, out string msg);
        public bool Create(AuditLog obj, out string msg);

    }
}
