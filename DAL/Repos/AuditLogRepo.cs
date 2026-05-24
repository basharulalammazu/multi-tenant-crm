using DAL.EF;
using DAL.EF.Models;
using DAL.Enums;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace DAL.Repos
{
    public class AuditLogRepo : Repository<AuditLog>, IAuditLogRepo
    {
        private readonly CrmSaaSDbContext db;
        public AuditLogRepo(CrmSaaSDbContext db) : base(db)
        {
            this.db = db;
        }

        public bool Create(AuditLog obj, out string msg)
        {
            return Add(obj, out msg);
        }

        public AuditLog? Find(Guid id, out string msg)
        {
            try
            {
                msg  = string.Empty;
                var auditLog = GetById(id, out msg);

                if (auditLog == null)
                {
                    msg = "Audit log not found.";
                    return null;
                }

                return auditLog;
            }
            catch (Exception ex)
            {
                msg = $"Error finding audit log: {ex.Message}";
                return null;
            }
        }

        public List<AuditLog> Find(out string msg)
        {
            try
            {
                msg = string.Empty;
                return GetAll(out msg);
            }

            catch (Exception ex)
            {
                msg = $"Error retrieving audit logs: {ex.Message}";
                return new List<AuditLog>();
            }
        }

        public List<AuditLog> FindByDateRange(Guid tenantId, DateTime from, DateTime to, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.AuditLogs.Where(a => a.TenantId == tenantId 
                                        && a.OccurredAt >= from 
                                        && a.OccurredAt <= to)
                                    .OrderByDescending(a => a.OccurredAt)
                                    .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving audit logs by date range: {ex.Message}";
                return null;
            }
        }

        public List<AuditLog> FindByEntity(string entityName, Guid entityId, out string msg)
        {
            try
            {
                msg = string.Empty;
                if (!Enum.TryParse<EntityType>(entityName, out var entityType))
                {
                    msg = $"Invalid entity name: {entityName}";
                    return null;
                }

                return db.AuditLogs.Where(a => a.EntityName == entityType && a.EntityId == entityId)
                                    .OrderByDescending(a => a.OccurredAt)
                                    .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving audit logs for entity: {ex.Message}";
                return new List<AuditLog>();
            }
        }

        public List<AuditLog> FindByTenant(Guid tenantId, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.AuditLogs.Where(a => a.TenantId == tenantId).ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving audit logs for tenant: {ex.Message}";
                return new List<AuditLog>();
            }   
        }

        public List<AuditLog> FindByUser(string email, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.AuditLogs.Where(a => a.PerformedBy == email)
                                    .OrderByDescending(a => a.OccurredAt)
                                    .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving audit logs for user: {ex.Message}";
                return null;
            }
        }
    }
}
