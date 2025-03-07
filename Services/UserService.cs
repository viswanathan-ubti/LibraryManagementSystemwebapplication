using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    public class UserService
    {
        private readonly LibraryDbContext _context;

        public UserService(LibraryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Add a new user
        public async Task<(bool Success, string Message)> AddUser(string name, string libraryCode)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.LibraryCode == libraryCode);
                if (existingUser != null)
                {
                    return (false, "User with this Library Code already exists!");
                }

                var user = new User
                {
                    Name = name,
                    LibraryCode = libraryCode,
                    SubscriptionEndDate = DateTime.Now.AddMonths(1),
                    Status = SubscriptionStatus.Active
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return (true, "User registered successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding user: {ex.Message}");
            }
        }

        // Validate user login
        public async Task<(bool Success, string Message)> ValidateUser(string libraryCode)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.LibraryCode == libraryCode);
                if (user == null)
                {
                    return (false, "User not found!");
                }

                if (DateTime.Now > user.SubscriptionEndDate)
                {
                    user.Status = SubscriptionStatus.Expired;
                    await _context.SaveChangesAsync();
                    return (false, "Subscription has expired. Please renew.");
                }

                return (true, "Login successful!");
            }
            catch (Exception ex)
            {
                return (false, $"Error validating user: {ex.Message}");
            }
        }

        // Renew user subscription
        public async Task<(bool Success, string Message)> RenewSubscription(string libraryCode)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.LibraryCode == libraryCode);
                if (user == null)
                {
                    return (false, "User not found!");
                }

                user.SubscriptionEndDate = DateTime.Now.AddMonths(1);
                user.Status = SubscriptionStatus.Active;
                await _context.SaveChangesAsync();
                return (true, "Subscription renewed successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error renewing subscription: {ex.Message}");
            }
        }

        // Get user details
        public async Task<User> GetUserDetails(string libraryCode)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.LibraryCode == libraryCode);
                if (user == null)
                {
                    throw new Exception("User not found!");
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user details: {ex.Message}");
            }
        }

        // Remove a user
        public async Task<(bool Success, string Message)> RemoveUser(string libraryCode)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.LibraryCode == libraryCode);
                if (user == null)
                {
                    return (false, "User not found!");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return (true, "User removed successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error removing user: {ex.Message}");
            }
        }
        // Get all users 
        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all users: {ex.Message}");
            }
        }
    }
}