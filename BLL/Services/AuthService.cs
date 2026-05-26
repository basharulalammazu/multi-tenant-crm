using Azure.Core;
using BLL.Constants;
using BLL.DTOs;
using BLL.DTOs.Auth;
using DAL;
using DAL.EF.Models;
using DAL.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services
{
    public class AuthService
    {
        private readonly DataAccessFactory dataAccessFactory;
        private readonly IConfiguration configuration;

        public AuthService(DataAccessFactory factory, IConfiguration configuration)
        {
            dataAccessFactory = factory;
            this.configuration = configuration;
        }

        
        // Login
        public TokenResponseDTO Login(LoginDTO data, out string msg)
        {
            msg = string.Empty;

            ValidateLoginInput(data.Email, data.Password);

            var user = dataAccessFactory.AppUserRepoAccess().FindByEmail(data.Email, out msg);

           

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("This account has been deactivated");

            bool passwordMatched = BCrypt.Net.BCrypt.Verify(data.Password, user.PasswordHash);

            if (!passwordMatched)
                throw new UnauthorizedAccessException("Invalid email or password");

            var tokenResponse = GenerateTokenPair(user);

            bool response = dataAccessFactory.AppUserRepoAccess().UpdateRefreshToken(user.Id, tokenResponse.RefreshToken, DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays), out msg);

            if (!response)
                throw new InvalidOperationException( $"Failed to update refresh token: {msg}");

            return tokenResponse;
        }

        // Refresh Token
        public TokenResponseDTO RefreshToken(string refreshToken, out string msg)
        {
            msg = string.Empty;

            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token is required", nameof(refreshToken));

            var user = dataAccessFactory.AppUserRepoAccess().FindByRefreshToken(refreshToken, out msg);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            var tokenResponse = GenerateTokenPair(user);

            bool response = dataAccessFactory.AppUserRepoAccess().UpdateRefreshToken(user.Id, tokenResponse.RefreshToken, DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays),out msg);

            if (!response)
                throw new InvalidOperationException($"Failed to update refresh token: {msg}");

            return tokenResponse;
        }

        // Register Tenant
        /*
        public TokenResponseDTO RegisterTenant(RegisterTenantDTO model, out string msg)
        {
            msg = string.Empty;

            ValidateRegisterInput(model.CompanyName, model.Subdomain, model.AdminEmail, model.AdminPassword);

            bool subdomainExists = dataAccessFactory.TenantRepoAccess().FindBySubdomain(model.Subdomain, out _) != null;

            if (subdomainExists)
                throw new InvalidOperationException($"Subdomain '{model.Subdomain}' is already taken");

            bool emailExists = dataAccessFactory.AppUserRepoAccess().FindByEmail(model.AdminEmail, out _) != null;

            if (emailExists)
                throw new InvalidOperationException($"Email '{model.AdminEmail}' is already registered");

            var mapper = MapperConfig.GetMapper();
             mapper.Map<Tenant>(RegisterTenantDTO);

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = companyName,
                Subdomain = subdomain,
                Plan = PlanType.Free,
                IsActive = true,
                TrialEndsAt = DateTime.UtcNow.AddDays(AppConstants.DefaultTrialDays),
                CreatedAt = DateTime.UtcNow
            };
            
            bool tenantCreated = dataAccessFactory.TenantRepoAccess().Add(tenant, out msg);

            if (!tenantCreated)
                throw new InvalidOperationException(msg);

            mapper.Map<AppUser>

            var admin = new AppUser
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                Role = UserRole.TenantAdmin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            bool adminCreated = dataAccessFactory
                .AppUserRepoAccess()
                .Add(admin, out msg);

            if (!adminCreated)
                throw new InvalidOperationException(msg);

            var tokenResponse = GenerateTokenPair(admin);

            dataAccessFactory
                .AppUserRepoAccess()
                .UpdateRefreshToken(
                    admin.Id,
                    tokenResponse.RefreshToken,
                    DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays),
                    out _
                );
            
            return tokenResponse;
        }
        */

        // ─────────────────────────────────────────────────────────
        // Logout
        // ─────────────────────────────────────────────────────────

        public bool Logout(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException(
                    "UserId is required",
                    nameof(userId)
                );

            return dataAccessFactory
                .AppUserRepoAccess()
                .UpdateRefreshToken(userId, null, null, out _);
        }

        // ─────────────────────────────────────────────────────────
        // Get Current User
        // ─────────────────────────────────────────────────────────

        public UserDTO? GetCurrentUser(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException(
                    "UserId is required",
                    nameof(userId)
                );

            var user = dataAccessFactory
                .AppUserRepoAccess()
                .FindWithTenant(userId, out _);

            if (user == null)
                return null;

            var mapper = MapperConfig.GetMapper();

            return mapper.Map<UserDTO>(user);
        }

        // ─────────────────────────────────────────────────────────
        // Private Helper Methods
        // ─────────────────────────────────────────────────────────

        private void ValidateLoginInput(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email is required",
                    nameof(email)
                );

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(
                    "Password is required",
                    nameof(password)
                );
        }

        private void ValidateRegisterInput(
            string companyName,
            string subdomain,
            string adminEmail,
            string adminPassword)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException(
                    "Company name is required",
                    nameof(companyName)
                );

            if (string.IsNullOrWhiteSpace(subdomain))
                throw new ArgumentException(
                    "Subdomain is required",
                    nameof(subdomain)
                );

            if (string.IsNullOrWhiteSpace(adminEmail))
                throw new ArgumentException(
                    "Admin email is required",
                    nameof(adminEmail)
                );

            if (string.IsNullOrWhiteSpace(adminPassword))
                throw new ArgumentException(
                    "Admin password is required",
                    nameof(adminPassword)
                );
        }

        private TokenResponseDTO GenerateTokenPair(AppUser user)
        {
            var expiry =
                DateTime.UtcNow.AddMinutes(
                    AppConstants.AccessTokenExpiryMinutes
                );

            string jwtKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT Key is missing from configuration"
                );

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Id.ToString()
                ),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email
                ),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                ),

                new Claim(
                    ClaimNames.UserId,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimNames.TenantId,
                    user.TenantId.ToString()
                ),

                new Claim(
                    ClaimNames.Role,
                    user.Role.ToString()
                )
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            string accessToken =
                new JwtSecurityTokenHandler().WriteToken(token);

            string refreshToken =
                Convert.ToBase64String(
                    RandomNumberGenerator.GetBytes(64)
                );

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiry
            };
        }
    }
}