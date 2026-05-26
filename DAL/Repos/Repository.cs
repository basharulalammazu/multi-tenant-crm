using DAL.EF;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repos
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CrmSaaSDbContext db;
        private readonly DbSet<T> table;

        public Repository(CrmSaaSDbContext db)
        {
            this.db = db;
            table = db.Set<T>();
        }

        public bool Add(T entity, out string msg)
        {
            try
            {
                msg = string.Empty;

                table.Add(entity);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex) 
            {
                msg = ex.Message;
                return false;
            }

        }

        public bool Delete(Guid id, out string msg)
        {
            try
            {
                msg = string.Empty;
                var entry = GetById(id, out msg);
                if (entry == null)
                    return false;

                table.Remove(entry);
                return db.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                return false;
            }

        }

        public List<T> GetAll(out string msg)
        {
            try
            {
                msg = string.Empty;
                return table.ToList();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public T GetById(Guid id, out string msg)
        {
            try
            {
                var entity = table.Find(id);
                if (entity == null)
                {
                    msg = "Entity not found.";
                    return null;
                }

                msg = string.Empty;
                return entity;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public bool Update(T entity, out string msg)
        {
            try
            {
                msg = string.Empty;
                var entry = db.Entry(entity);
                var key = entry.Metadata.FindPrimaryKey();

                if (key == null || key.Properties.Count != 1)
                {
                    table.Update(entity);
                    return db.SaveChanges() > 0;
                }

                var keyProperty = key.Properties[0];
                var keyValue = entry.Property(keyProperty.Name).CurrentValue;
                var existing = table.Find(keyValue);

                if (existing == null)
                {
                    msg = "Entity not found for update.";
                    return false;
                }

                db.Entry(existing).CurrentValues.SetValues(entity);
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
