using DAL.EF;
using DAL.EF.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repos
{
    public class NotificationRepo : Repository<Notification>, INotificationRepo
    {
        private readonly CrmSaaSDbContext db;
        public NotificationRepo(CrmSaaSDbContext db) : base(db)
        {
            this.db = db;
        }

        public int CountUnreadByUser(Guid userId, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Notifications.Count(n => n.RecipientId == userId && !n.IsRead);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return -1;
            }
        }

        public List<Notification> FindAllWithUser(out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Notifications
            .Include(n => n.User)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public List<Notification> FindByUser(Guid userId, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Notifications
                    .Where(n => n.RecipientId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public List<Notification> FindUnreadByUser(Guid userId, out string msg)
        {
            try
            {
                msg = string.Empty;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }

        }

        public Notification? FindWithUser(Guid id, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Notifications.Include(n => n.Recipient)
                                        .FirstOrDefault(n => n.Id == id);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public bool MarkAllReadForUser(Guid userId, out string msg)
        {
            try
            {
                msg = string.Empty;
                var unread = db.Notifications
                    .Where(n => n.RecipientId == userId && !n.IsRead)
                    .ToList();

                if (unread.Count == 0) return true;

                var now = DateTime.UtcNow;
                foreach (var n in unread)
                {
                    n.IsRead = true;
                    n.ReadAt = now;
                    n.UpdatedAt = now;
                }

                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        public bool MarkRead(Guid id, out string msg)
        {
            try
            {
                var notification = GetById(id, out msg);
                if (notification == null)
                {
                    msg = "Notification is not foud";
                    return false;
                }

                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                notification.UpdatedAt = DateTime.UtcNow;

                msg = string.Empty;
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        public bool SoftDelete(Guid id, out string msg)
        {
            try
            {
                var notification = db.Notifications.Find(id);
                if (notification == null)
                {
                    msg = "Notification not found";
                    return false;
                }

                notification.IsDeleted = true;
                notification.DeletedAt = DateTime.UtcNow;
                notification.UpdatedAt = DateTime.UtcNow;

                msg = string.Empty;
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
    }
}
