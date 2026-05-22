using DAL.EF;
using DAL.EF.Models;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Azure.Core.HttpHeader;

namespace DAL.Repos
{
    public class ContactRepo : Repository<Contact>, IContactRepo
    {
        private readonly CrmSaaSDbContext db;
        public ContactRepo(CrmSaaSDbContext db) : base(db)
        {
            this.db = db;
        }

        public List<Contact> FindAllWithDeals(out string msg)
        {
            try
            {
                msg = "Contacts with deals fetched successfully.";
                return db.Contacts
                        .Include(c => c.Deals)
                        .OrderBy(c => c.FullName)
                        .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching contacts with deals: {ex.Message}";
                return new List<Contact>();
            }
            finally
            {
                msg = "Contacts with deals fetched successfully.";
            }
        }

        public List<Contact> FindByCompany(string company, out string msg)
        {
            try
            {
                msg = "Contacts by company fetched successfully.";
                return db.Contacts.Where(c => c.CompanyName != null && c.CompanyName.Contains(company))
                                    .OrderBy(c => c.FullName)
                                    .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching contacts by company: {ex.Message}";
                return new List<Contact>();
            }
            finally
            {
                msg = "Contacts by company fetched successfully.";
            }
        }

        public List<Contact> FindByLead(Guid leadId, out string msg)
        {
            try
            {
                msg = "Contacts by lead fetched successfully.";
                return db.Contacts.Include(c => c.Lead)
                                    .Where(c => c.LeadId == leadId)
                                    .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching contacts by lead: {ex.Message}";
                return new List<Contact>();
            }
            finally
            {
                msg = "Contacts by lead fetched successfully.";
            }
        }

        public Contact? FindFull(Guid id, out string msg)
        {
            try
            {
                msg = "Contact with all details fetched successfully.";
                return  db.Contacts.Include(c => c.Lead)
                                    .Include(c => c.Deals)
                                        .ThenInclude(d => d.Owner)
                                    .Include(c => c.Notes)
                                    .Include(c => c.Activities)
                                    .FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error fetching contact with all details: {ex.Message}";
                return null;
            }
            finally
            {
                msg = "Contact with all details fetched successfully.";
            }
        }

        public Contact? FindWithActivities(Guid id, out string msg)
        {
            try
            {
                msg = "Contact with activities fetched successfully.";
                return db.Contacts.Include(c => c.Activities)
                                    .FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error fetching contact with activities: {ex.Message}";
                return null;
            }
            finally
            {
                msg = "Contact with activities fetched successfully.";
            }
        }

        public Contact? FindWithDeals(Guid id, out string msg)
        {
            try
            {
                msg = "Contact with deals fetched successfully.";
                return db.Contacts.Include(c => c.Deals)
                                    .FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error fetching contact with deals: {ex.Message}";
                return null;
            }
        }

        public Contact? FindWithNotes(Guid id, out string msg)
        {
            try
            {
                msg = "Contact with notes fetched successfully.";
                return db.Contacts.Include(c => c.Notes)
                                    .FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error fetching contact with notes: {ex.Message}";
                return null;
            }
        }

        public List<Contact> FindWithOpenDeals(out string msg)
        {
            try
            {
                msg = "Contacts with open deals fetched successfully.";
                return db.Contacts.Include(c => c.Deals)
                                   .Where(c => c.Deals.Any(d =>
                                       d.Stage != DealStage.Won &&
                                       d.Stage != DealStage.Lost))
                                   .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error fetching contacts with open deals: {ex.Message}";
                return new List<Contact>();
            }
            finally
            {
                msg = "Contacts with open deals fetched successfully.";
            }
        }

        public bool SoftDelete(Guid id, out string msg)
        {
            try
            {
                var contact = db.Contacts.Find(id);
                if (contact == null)
                {
                    msg = "Contact not found.";
                    return false;
                }
                   

                contact.IsDeleted = true;
                contact.DeletedAt = DateTime.UtcNow;
                contact.UpdatedAt = DateTime.UtcNow;

                msg = "Contact soft deleted successfully.";
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = $"Error soft deleting contact: {ex.Message}";
                return false;
            }
        }
    }
}
