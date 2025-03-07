using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    public class AdminService
    {
        private readonly LibraryDbContext _context;

        public AdminService(LibraryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Register a new admin with hashed password
        public async Task<(bool Success, string Message)> RegisterAdmin(string name, string adminCode, string password)
        {
            try
            {
                var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminCode == adminCode);
                if (existingAdmin != null)
                {
                    return (false, "Admin with this code already exists!");
                }

                var admin = new Admin
                {
                    Name = name,
                    AdminCode = adminCode,
                    Password = BCrypt.Net.BCrypt.HashPassword(password) // Hash password
                };

                await _context.Admins.AddAsync(admin);
                await _context.SaveChangesAsync();
                return (true, "Admin registered successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error registering admin: {ex.Message}");
            }
        }

        // Validate admin login with password verification
        public async Task<(bool Success, string Message)> ValidateAdmin(string adminCode, string password)
        {
            try
            {
                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminCode == adminCode);
                if (admin == null || !BCrypt.Net.BCrypt.Verify(password, admin.Password))
                {
                    return (false, "Invalid Admin Code or Password!");
                }
                return (true, "Login successful!");
            }
            catch (Exception ex)
            {
                return (false, $"Error validating admin: {ex.Message}");
            }
        }

        // Renew a user subscription
        public async Task<(bool Success, string Message)> RenewUserSubscription(string libraryCode)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.LibraryCode == libraryCode);
                if (user == null)
                {
                    return (false, "User not found!");
                }

                user.SubscriptionEndDate = DateTime.Now.AddMonths(1);
                user.Status = Enums.SubscriptionStatus.Active;
                await _context.SaveChangesAsync();
                return (true, "User subscription renewed successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error renewing user subscription: {ex.Message}");
            }
        }
    }
}