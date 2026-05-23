using DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface INotificationRepo
    {
        public Notification? FindWithUser(Guid id, out string msg);
        public List<Notification> FindByUser(Guid userId, out string msg);
        public List<Notification> FindUnreadByUser(Guid userId, out string msg);
        public List<Notification> FindAllWithUser(out string msg);
        public int CountUnreadByUser(Guid userId, out string msg);
        public bool MarkRead(Guid id, out string msg);
        public bool MarkAllReadForUser(Guid userId, out string msg);
        public bool SoftDelete(Guid id, out string msg);
    }
}
