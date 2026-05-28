using BLL.DTOs;
using BLL.DTOs.Shared;
using DAL;
using DAL.EF.Models;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class UserService
    {
        private readonly DataAccessFactory factory;

        public UserService(DataAccessFactory factory)
        {
            this.factory = factory;
        }

        public UserDTO? GetById(Guid id, out string msg)
        {
            msg = string.Empty;

            try
            {
                if (id == Guid.Empty)
                {
                    msg = "Invalid user ID";
                    return null;
                }

                var user = factory.AppUserRepoAccess().FindWithTenant(id, out msg);

                if (!string.IsNullOrEmpty(msg))
                    return null;

                if (user == null)
                {
                    msg = "User not found";
                    return null;
                }

                var mapper = MapperConfig.GetMapper();
                return mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving user: {ex.Message}";
                return null;
            }
        }

        public List<UserDTO> GetAll(out string msg)
        {
            msg = string.Empty;

            try
            {
                var users = factory.GetRepo<AppUser>().GetAll(out msg);

                if (!string.IsNullOrEmpty(msg))
                    return new List<UserDTO>();

                if (users == null || users.Count == 0)
                {
                    msg = "No users found";
                    return new List<UserDTO>();
                }

                var mapper = MapperConfig.GetMapper();
                return mapper.Map<List<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving all users: {ex.Message}";
                return new List<UserDTO>();
            }
        }

        public List<UserDTO> GetUserByRole(UserRole role, out string msg)
        {
            msg = string.Empty;

            try
            {
                var users = factory.AppUserRepoAccess().FindByRole(role, out msg);

                if (!string.IsNullOrEmpty(msg))
                    return new List<UserDTO>();

                if (users == null || users.Count == 0)
                {
                    msg = "No users found for this role";
                    return new List<UserDTO>();
                }

                var mapper = MapperConfig.GetMapper();
                return mapper.Map<List<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving users by role: {ex.Message}";
                return new List<UserDTO>();
            }
        }

        public List<UserDTO> GetUserByTenant(Guid tenantId, out string msg)
        {
            msg = string.Empty;

            try
            {
                if (tenantId == Guid.Empty)
                {
                    msg = "Invalid tenant ID";
                    return new List<UserDTO>();
                }

                var users = factory.AppUserRepoAccess().FindByTenant(tenantId, out msg);

                if (!string.IsNullOrEmpty(msg))
                    return new List<UserDTO>();

                if (users == null || users.Count == 0)
                {
                    msg = "No users found for this tenant";
                    return new List<UserDTO>();
                }

                var mapper = MapperConfig.GetMapper();
                return mapper.Map<List<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                msg = $"Error retrieving users by tenant: {ex.Message}";
                return new List<UserDTO>();
            }
        }

        public bool RegisterUser(CreateUserDTO user, out string msg)
        {
            msg = string.Empty;

            try
            {
                // Validate input
                if (user == null)
                {
                    msg = "User data is required";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    msg = "Email is required";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    msg = "Password is required";
                    return false;
                }

                if (user.TenantId == Guid.Empty)
                {
                    msg = "Tenant ID is required";
                    return false;
                }

                // Validate tenant exists
                var validTenant = factory.GetRepo<Tenant>().GetById(user.TenantId, out msg);
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    msg = $"Error validating tenant: {msg}";
                    return false;
                }

                if (validTenant == null)
                {
                    msg = "Tenant not found";
                    return false;
                }

                // Check if email already exists
                var existingUser = factory.AppUserRepoAccess().FindByEmail(user.Email, out msg);
                if (existingUser != null)
                {
                    msg = "Email is already registered";
                    return false;
                }

                var mapper = MapperConfig.GetMapper();
                var data = mapper.Map<AppUser>(user);

                // Hash the password before saving
                data.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                // Set audit fields
                data.CreatedBy = "System";
                data.CreatedAt = DateTime.UtcNow;
                data.IsActive = true;

                bool result = factory.GetRepo<AppUser>().Add(data, out msg);

                if (!result)
                {
                    msg = string.IsNullOrWhiteSpace(msg) ? "Failed to create user" : msg;
                    return false;
                }

                msg = "User created successfully";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"Error registering user: {ex.Message}";
                return false;
            }
        }
    }
}