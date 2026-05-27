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
        public TokenResponseDTO RegisterTenant(RegisterTenantDTO model, out string msg)
        {
            msg = string.Empty;

            ValidateRegisterInput(model.Name, model.Subdomain, model.Email, model.PasswordHash);

            var subdomainExists = dataAccessFactory.TenantRepoAccess().FindBySubdomain(model.Subdomain, out msg);

            if (subdomainExists != null)
                throw new InvalidOperationException($"Subdomain '{model.Subdomain}' is already taken");

            bool emailExists = dataAccessFactory.AppUserRepoAccess().FindByEmail(model.Email, out msg) != null;

            if (emailExists)
                throw new InvalidOperationException($"Email '{model.Email}' is already registered");

            var mapper = MapperConfig.GetMapper();
            var tenantData = mapper.Map<Tenant>(model);


            var tenantCreated = dataAccessFactory.GetRepo<Tenant>().Add(tenantData, out msg);



            // Admin creation
            if (!tenantCreated)
                throw new InvalidOperationException(msg);

            model.TenantId = tenantData.Id;
            var adminData = mapper.Map<AppUser>(model);

            // Hash the password before saving
            adminData.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);

            bool adminCreated = dataAccessFactory.GetRepo<AppUser>().Add(adminData, out msg);

            if (!adminCreated)
                throw new InvalidOperationException(msg);

            var tokenResponse = GenerateTokenPair(adminData);

            bool response = dataAccessFactory.AppUserRepoAccess().UpdateRefreshToken(adminData.Id, tokenResponse.RefreshToken,DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays),out msg);
            
            if (!response)
                throw new InvalidOperationException(msg);

            return tokenResponse;
        }

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

        private void ValidateRegisterInput(string companyName, string subdomain, string adminEmail, string adminPassword)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("Company name is required", nameof(companyName));

            if (string.IsNullOrWhiteSpace(subdomain))
                throw new ArgumentException("Subdomain is required", nameof(subdomain));

            if (string.IsNullOrWhiteSpace(adminEmail))
                throw new ArgumentException("Admin email is required", nameof(adminEmail));

            if (string.IsNullOrWhiteSpace(adminPassword))
                throw new ArgumentException("Admin password is required", nameof(adminPassword));
        }

        private TokenResponseDTO GenerateTokenPair(AppUser user)
        {
            var expiry =DateTime.UtcNow.AddMinutes(AppConstants.AccessTokenExpiryMinutes);

            string jwtKey = configuration["Jwt:Key"]?? 
                                    throw new InvalidOperationException("JWT Key is missing from configuration");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

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
                Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiry
            };
        }
    }
}