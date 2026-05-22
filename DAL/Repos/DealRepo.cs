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
    public class DealRepo : Repository<Deal>, IDealRepo
    {
        private readonly CrmSaaSDbContext db;
        public DealRepo(CrmSaaSDbContext db) : base(db)
        {
            this.db = db;
        }

        public bool ChangeStage(Guid dealId, DealStage newStage, out string msg)
        {
            try
            {
                var deal = db.Deals.Find(dealId);
                if (deal == null)
                {
                    msg = $"Deal with ID '{dealId}' not found.";
                    return false;
                }
                    

                deal.Stage = newStage;
                deal.UpdatedAt = DateTime.UtcNow;

                if (newStage is DealStage.Won or DealStage.Lost)
                    deal.ClosedAt = DateTime.UtcNow;

                msg = $"Deal stage changed to '{newStage}' successfully.";
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = $"Error changing deal stage: {ex.Message}";
                return false;
            }
        }

        public List<Deal> FindAllWithContact(out string msg)
        {
            try
            { 
                msg = "Deals with contacts fetched successfully.";
                return db.Deals.Include(d => d.Contact)
                                .OrderByDescending(d => d.CreatedAt)
                                .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching deals with contact: {ex.Message}";
                return null;
            }
        }

        public List<Deal> FindAllWithOwner(out string msg)
        {
            try 
                            {
                msg = "Deals with owners fetched successfully.";
                return db.Deals.Include(d => d.Owner)
                                .OrderByDescending(d => d.CreatedAt)
                                .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching deals with owner: {ex.Message}";
                return null;
            }
        }

        public List<Deal> FindByContact(Guid contactId, out string msg)
        {
            try
            {
                msg = $"Deals for contact ID '{contactId}' fetched successfully.";
                return db.Deals.Include(d => d.Owner)
                                .Where(d => d.ContactId == contactId)
                                .OrderByDescending(d => d.CreatedAt)
                                .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching deals by contact: {ex.Message}";
                return null;
            }
        }

        public List<Deal> FindByOwner(Guid ownerId, out string msg)
        {
            try
            {
                msg = $"Deals for owner ID '{ownerId}' fetched successfully.";
                return db.Deals.Include(d => d.Contact)
                                .Where(d => d.OwnerId == ownerId)
                                .OrderByDescending(d => d.CreatedAt)
                                .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching deals by owner: {ex.Message}";
                return null;
            }
        }

        public List<Deal> FindByStage(DealStage stage, out string msg)
        {
            try
            {
                msg = $"Deals in stage '{stage}' fetched successfully.";
                return db.Deals.Include(d => d.Contact)
                                .Include(d => d.Owner)
                                .Where(d => d.Stage == stage)
                                .OrderByDescending(d => d.Amount)
                                .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching deals by stage: {ex.Message}";
                return null;

            }
        }

        public List<Deal> FindExpired(out string msg)
        {
            try
            {
                msg = "Expired deals fetched successfully.";
                return db.Deals.Include(d => d.Owner)
                               .Where(d => d.ExpectedCloseAt < DateTime.UtcNow
                                        && d.Stage != DealStage.Won
                                        && d.Stage != DealStage.Lost
                                        && d.Stage != DealStage.Stale)
                               .OrderBy(d => d.ExpectedCloseAt)
                               .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching expired deals: {ex.Message}";
                return null;
            }
        }

        public Deal? FindFull(Guid id, out string msg)
        {
            try
            {
                msg = "Full deal fetched successfully.";
                return db.Deals.Include(d => d.Contact)
                                .Include(d => d.Owner)
                                .Include(d => d.Activities)
                                .FirstOrDefault(d => d.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error fetching full deal: {ex.Message}";
                return null;
            }
        }

        public Deal? FindWithActivities(Guid id, out string msg)
        {
            try
            {
                msg = "Deal with activities fetched successfully.";
                return db.Deals.Include(d => d.Activities).FirstOrDefault(d => d.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error fetching deal with activities: {ex.Message}";
                return null;
            }
        }

        public Deal? FindWithContact(Guid id, out string msg)
        {
            try
            {
                msg = "Deal with contact fetched successfully.";
                return db.Deals.Include(d => d.Contact).FirstOrDefault(d => d.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error fetching deal with contact: {ex.Message}";
                return null;
            }
        }

        public Deal? FindWithOwner(Guid id, out string msg)
        {
            try
            {
                msg = "Deal with owner fetched successfully.";
                return db.Deals.Include(d => d.Owner).FirstOrDefault(d => d.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error fetching deal with owner: {ex.Message}";
                return null;
            }
        }

        public List<Deal> FindWonThisMonth(out string msg)
        {
            try
            {
                msg = "Won deals this month fetched successfully.";
                var firstOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                return db.Deals
                    .Include(d => d.Owner)
                    .Where(d => d.Stage == DealStage.Won
                             && d.ClosedAt != null
                             && d.ClosedAt >= firstOfMonth)
                    .OrderByDescending(d => d.ClosedAt)
                    .ToList();

            }
            catch (Exception ex)
            {
                msg = $"Error fetching won deals this month: {ex.Message}";
                return null;
            }
        }

        public bool SoftDelete(Guid id, out string msg)
        {
            try
            {
                var deal = db.Deals.Find(id);
                if (deal == null)
                {
                    msg = $"Deal with ID '{id}' not found.";
                    return false;
                }
                    

                deal.IsDeleted = true;
                deal.DeletedAt = DateTime.UtcNow;
                deal.UpdatedAt = DateTime.UtcNow;

                msg = "Deal soft deleted successfully.";
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = $"Error soft deleting deal: {ex.Message}";
                return false;
            }
        }

        public decimal SumValueByStage(DealStage stage, out string msg)
        {
            try
            {
                msg = $"Total value of deals in stage '{stage}' calculated successfully.";
                return db.Deals.Where(d => d.Stage == stage).Sum(d => d.Amount);
            }
            catch (Exception ex)
            {
                msg = $"Error summing deal values by stage: {ex.Message}";
                return 0;
            }
        }
    }
}
