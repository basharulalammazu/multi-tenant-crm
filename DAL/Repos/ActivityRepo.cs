using DAL.EF;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DAL.Repos
{
    public class ActivityRepo : Repository<Activity>, IActivityRepo
    {
        private readonly CrmSaaSDbContext db;
        public ActivityRepo(CrmSaaSDbContext db) : base(db)
        {
            this.db = db;
        }

        public List<EF.Models.Activity> FindByContact(Guid contactId, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Activities
                    .Include(a => a.Contact)
                    .Where(a => a.ContactId == contactId)
                    .OrderByDescending(a => a.DueAt) // Changed from ScheduledAt to DueAt
                    .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error finding activities by contact: {ex.Message}";
                Debug.WriteLine(msg);
                return new List<EF.Models.Activity>();
            }
        }

        public List<EF.Models.Activity> FindByCreatedBy(string email, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Activities.Include(a => a.Contact)
                                        .Include(a => a.Deal)
                                        .Where(a => a.CreatedBy == email)
                                        .OrderByDescending(a => a.DueAt)
                                        .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error finding activities by creator: {ex.Message}";
                Debug.WriteLine(msg);
                return null;
            }
        }
        public List<EF.Models.Activity> FindByDateRange(DateTime from, DateTime to, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Activities.Include(a => a.Contact)
                                    .Include(a => a.Deal)
                                    .Where(a => a.DueAt >= from && a.DueAt <= to)
                                    .OrderBy(a => a.DueAt)
                                    .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error finding activities by date range: {ex.Message}";
                Debug.WriteLine(msg);
                return null;
            }
        }

        public List<EF.Models.Activity> FindByDeal(Guid dealId, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Activities.Include(a => a.Deal)
                                    .Where(a => a.DealId == dealId)
                                    .OrderByDescending(a => a.DueAt)
                                    .ToList();
            }
            catch(Exception ex)
            {
                msg = $"Error finding activities by deal: {ex.Message}";
                Debug.WriteLine(msg);
                return new List<EF.Models.Activity>();
            }
        }

        public List<EF.Models.Activity> FindByType(ActivityType type, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Activities.Include(a => a.Contact)
                            .Include(a => a.Deal)
                            .Where(a => a.Type == type)
                            .OrderByDescending(a => a.DueAt)
                            .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error finding activities by type: {ex.Message}";
                Debug.WriteLine(msg);
                return new List<EF.Models.Activity>();
            }
        }

        public EF.Models.Activity? FindFull(Guid id, out string msg)
        {
            try
            {
                msg= "";
                return db.Activities.Include(a => a.Contact)
                                    .Include(a => a.Deal)
                                       .ThenInclude(d => d!.Owner)
                                    .FirstOrDefault(a => a.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error finding full activity: {ex.Message}";
                Debug.WriteLine(msg);
                return null;
            }
        }

        public EF.Models.Activity? FindWithContact(Guid id, out string msg)
        {
            try
            {
                msg = "";
                return db.Activities.Include(a => a.Contact).FirstOrDefault(a => a.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error finding activity with contact: {ex.Message}";
                Debug.WriteLine(msg);
                return null;
            }
        }

        public EF.Models.Activity? FindWithDeal(Guid id, out string msg)
        {
            try
            {
                msg = "";
                return db.Activities.Include(a => a.Deal).FirstOrDefault(a => a.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error finding activity with deal: {ex.Message}";
                Debug.WriteLine(msg);
                return null;
            }
        }

        public bool SoftDelete(Guid id, out string msg)
        {
            try
            {
                var activity = db.Activities.Find(id);
                if (activity == null)
                {
                    msg = "Activity not found.";
                    return false;
                }

                activity.IsDeleted = true;
                activity.DeletedAt = DateTime.UtcNow;
                activity.UpdatedAt = DateTime.UtcNow;

                msg = string.Empty;
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = $"Error soft deleting activity: {ex.Message}";
                Debug.WriteLine(msg);
                return false;
            }
        }
    }
}
