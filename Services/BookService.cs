using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    public class BookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Add a new book
        public async Task<(bool Success, string Message)> AddBook(string title, string author, string bookCode)
        {
            try
            {
                var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.BookCode == bookCode);
                if (existingBook != null)
                {
                    return (false, "A book with this code already exists!");
                }

                var book = new Book
                {
                    Title = title,
                    Author = author,
                    BookCode = bookCode,
                    IsAvailable = true
                };

                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();
                return (true, "Book added successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding book: {ex.Message}");
            }
        }

        // Remove a book
        public async Task<(bool Success, string Message)> RemoveBook(string bookCode)
        {
            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(b => b.BookCode == bookCode);
                if (book == null)
                {
                    return (false, "Book not found!");
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return (true, "Book removed successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error removing book: {ex.Message}");
            }
        }

        // Get all books
        public async Task<List<Book>> GetAllBooks()
        {
            try
            {
                return await _context.Books.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving books: {ex.Message}");
            }
        }
    }
}