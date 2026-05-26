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
    public class AppUserRepo : Repository<AppUser>, IAppUserRepo
    {
        private readonly CrmSaaSDbContext db;
        public AppUserRepo(CrmSaaSDbContext db) : base(db)
        {
            this.db = db;
        }

        public bool Deactivate(Guid userId, out string msg)
        {
            try
            {
                var user = GetById(userId, out msg);

                if (user == null)
                {
                    msg = $"User Not found";
                    return false;
                }

                user.IsActive = false;
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;

                return db.SaveChanges() > 0;

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        public List<AppUser> FindAllWithTenant(out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.AppUsers.Include(x => x.TenantId)
                                    .OrderBy(x => x.LastName)
                                    .ThenBy(u  => u.FirstName).ToList();    
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public AppUser? FindByEmail(string email, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.AppUsers.Include(x => x.Tenant).FirstOrDefault(x => x.Email == email);
                // return db.AppUsers.FirstOrDefault(x => x.Email == email);
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public AppUser? FindByRefreshToken(string refreshToken, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.AppUsers.FirstOrDefault(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
            }
            catch (Exception ex)
            { 
                msg = ex.Message; 
                return null; 
            }
        }

        public List<AppUser> FindByRole(UserRole role, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.AppUsers.Where(u => u.Role == role && u.IsActive)
                                    .OrderBy(u => u.LastName).ToList();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public List<AppUser> FindByTenant(Guid tenantId, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.AppUsers.Where(u => u.TenantId == tenantId && u.IsActive)
                                    .OrderBy(u => u.LastName)
                                    .ThenBy(u => u.FirstName).ToList();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public AppUser? FindWithTenant(Guid id, out string msg)
        {
            try
            {
                msg = String.Empty;
                return db.AppUsers.Include(u => u.Tenant).FirstOrDefault(u => u.Id == id);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public bool UpdateRefreshToken(Guid userId, string? token, DateTime? expiry, out string msg)
        {
            try
            {
                var user = GetById(userId, out msg);
                if (user == null)
                {
                    msg = $"No user found with this id: {userId}";
                    return false;
                }

                user.RefreshToken = token;
                user.RefreshTokenExpiry = expiry;

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
