using DAL.EF;
using DAL.EF.Models;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repos
{
    public class LeadRepo : Repository<Lead>, ILeadRepo
    {
        private readonly CrmSaaSDbContext db;
        public LeadRepo(CrmSaaSDbContext db) : base(db)
        {
            this.db = db;
        }

        public bool AssignToUser(Guid leadId, Guid userId, out string msg)
        {
            try
            {
                var lead = GetById(leadId, out msg);

                if (lead == null)
                {
                    msg = "Lead not found.";
                    return false;
                }

                lead.AssignedToUserId = userId;
                lead.Status = LeadStatus.Contacted;
                lead.UpdatedAt = DateTime.UtcNow;

                msg = "Lead assigned to user successfully.";
                return Update(lead, out msg);

            }

            catch (Exception ex)
            {
                msg = $"Error assigning lead to user: {ex.Message}";
                return false;
            }
        }

        public int CountByStatus(LeadStatus status, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Leads.Count(l => l.Status == status);
            }
            catch (Exception ex)
            {
                msg = $"Error counting leads by status: {ex.Message}";
                return -1;
            }
        }

        public List<Lead> FindAllWithAssignedUser(out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Leads.Include(l => l.AssignedTo).ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving leads with assigned users: {ex.Message}";
                return null;
            }
        }

        public List<Lead> FindByAssignedUser(Guid userId, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Leads.Include(l => l.AssignedTo)
                                .Where(l => l.AssignedToUserId == userId)
                                .OrderByDescending(l => l.CreatedAt)
                                .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving leads by assigned user: {ex.Message}";
                return null;
            }
        }

        public List<Lead> FindBySource(string source, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Leads.Where(l => l.Source == source)
                               .OrderByDescending(l => l.CreatedAt)
                               .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving leads by source: {ex.Message}";
                return null;
            }
        }

        public List<Lead> FindByStatus(LeadStatus status, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Leads.Include(l => l.AssignedTo)
                                .Where(l => l.Status == status)
                                .OrderByDescending(l => l.CreatedAt)
                                .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving leads by status: {ex.Message}";
                return null;
            }
        }

        public List<Lead> FindConverted(out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Leads.Include(l => l.Contact)
                                .Where(l => l.Status == LeadStatus.Converted && l.ConvertedAt != null)
                                .OrderByDescending(l => l.ConvertedAt)
                                .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving converted leads: {ex.Message}";
                return null;
            }
        }

        public List<Lead> FindStalled(int daysWithoutActivity, out string msg)
        {
            try
            {
                msg = string.Empty;
                var threshold = DateTime.UtcNow.AddDays(-daysWithoutActivity);
                return db.Leads.Include(l => l.AssignedTo)
                                .Where(l => l.Status == LeadStatus.Contacted
                                         && l.UpdatedAt != null
                                         && l.UpdatedAt < threshold)
                                .OrderBy(l => l.UpdatedAt)
                                .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving stalled leads: {ex.Message}";
                return null;
            }
        }

        public List<Lead> FindUnassigned(out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Leads
                        .Where(l => l.AssignedToUserId == null)
                        .OrderByDescending(l => l.CreatedAt)
                        .ToList();

            }
            catch (Exception ex)
            {
                msg = $"Error retrieving unassigned leads: {ex.Message}";
                return null;
            }
        }

        public Lead? FindWithAssignedUser(Guid id, out string msg)
        {
            throw new NotImplementedException();
        }

        public bool MarkConverted(Guid leadId, out string msg)
        {
            try
            {
                msg = string.Empty;
                var lead = GetById(leadId, out msg);
                if (lead == null) return false;

                lead.Status = LeadStatus.Converted;
                lead.ConvertedAt = DateTime.UtcNow;
                lead.UpdatedAt = DateTime.UtcNow;

                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = $"Error marking lead as converted: {ex.Message}";
                return false;
            }
        }

        public bool SoftDelete(Guid id, out string msg)
        {
            try
            {
                msg = string.Empty;

                var lead = GetById(id, out msg);
                if (lead == null)
                {
                     msg = "Lead not found.";
                        return false;
                }
                    

                lead.IsDeleted = true;
                lead.DeletedAt = DateTime.UtcNow;
                lead.UpdatedAt = DateTime.UtcNow;

                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = $"Error soft deleting lead: {ex.Message}";
                return false;
            }

        }
    }
}
