using DAL.EF;
using DAL.EF.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repos
{
    public class ContactNoteRepo : Repository<ContactNote>, IContactNoteRepo
    {
        private readonly CrmSaaSDbContext db;
        public ContactNoteRepo(CrmSaaSDbContext db) : base(db)
        {
            this.db = db;
        }

        public List<ContactNote> FindByContact(Guid contactId, out string msg)
        {
            try
            {
                msg = "Contact notes retrieved successfully.";
                return db.ContactNotes.Where(n => n.ContactId == contactId)
                                       .OrderByDescending(n => n.CreatedAt)
                                       .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving contact notes: {ex.Message}";
                return new List<ContactNote>();
            }

        }

        public List<ContactNote> FindByContactWithContact(Guid contactId, out string msg)
        {
            try
            {
                msg = "Contact notes with contact retrieved successfully.";
                return db.ContactNotes.Include(n => n.Contact)
                                       .Where(n => n.ContactId == contactId)
                                       .OrderByDescending(n => n.CreatedAt)
                                       .ToList();
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving contact notes with contact: {ex.Message}";
                return new List<ContactNote>();
            }
        }

        public ContactNote? FindWithContact(Guid id, out string msg)
        {
            try
            {
                msg = "Contact note with contact retrieved successfully.";
                return db.ContactNotes.Include(n => n.Contact)
                                        .FirstOrDefault(n => n.Id == id);
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving contact note with contact: {ex.Message}";
                return null;
            }
        }

        public bool SoftDelete(Guid id, out string msg)
        {
            try
            {
                var note = db.ContactNotes.Find(id);
                if (note == null)
                {
                    msg = "Contact note not found.";
                    return false;
                }


                note.IsDeleted = true;
                note.DeletedAt = DateTime.UtcNow;
                note.UpdatedAt = DateTime.UtcNow;

                msg = "Contact note soft deleted successfully.";
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = $"Error soft deleting contact note: {ex.Message}";
                return false;
            }
        }
    }
}
