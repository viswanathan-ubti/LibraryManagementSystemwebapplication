using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    public class BorrowService
    {
        private readonly LibraryDbContext _context;
        private const int FinePerDay = 10; // ₹10 fine per day after due date

        public BorrowService(LibraryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Borrow a book
        public async Task<(bool Success, string Message)> BorrowBook(string userCode, string bookCode)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.LibraryCode == userCode);
                var book = await _context.Books.FirstOrDefaultAsync(b => b.BookCode == bookCode);

                if (user == null)
                {
                    return (false, "User not found!");
                }

                if (book == null)
                {
                    return (false, "Book not found!");
                }

                if (!book.IsAvailable)
                {
                    return (false, "Book is already borrowed!");
                }

                var borrowedBook = new BorrowedBook
                {
                    UserId = user.Id,
                    BookId = book.Id,
                    BorrowDate = DateTime.Now
                };

                book.IsAvailable = false;
                await _context.BorrowedBooks.AddAsync(borrowedBook);
                await _context.SaveChangesAsync();
                return (true, "Book borrowed successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error borrowing book: {ex.Message}");
            }
        }

        // Return a book with fine calculation
        public async Task<(bool Success, string Message)> ReturnBook(string userCode, string bookCode)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.LibraryCode == userCode);
                var book = await _context.Books.FirstOrDefaultAsync(b => b.BookCode == bookCode);
                var borrowedBook = await _context.BorrowedBooks
                    .FirstOrDefaultAsync(bb => bb.UserId == user.Id && bb.BookId == book.Id && bb.ReturnDate == null);

                if (user == null)
                {
                    return (false, "User not found!");
                }

                if (book == null)
                {
                    return (false, "Book not found!");
                }

                if (borrowedBook == null)
                {
                    return (false, "This book was not borrowed by the user or has already been returned!");
                }

                borrowedBook.ReturnDate = DateTime.Now;
                book.IsAvailable = true;

                int daysLate = (DateTime.Now - borrowedBook.DueDate).Days;
                if (daysLate > 0)
                {
                    int fine = daysLate * FinePerDay;
                    await _context.SaveChangesAsync();
                    return (true, $"Book returned late! Fine Amount: ₹{fine}");
                }

                await _context.SaveChangesAsync();
                return (true, "Book returned successfully, no fine applied.");
            }
            catch (Exception ex)
            {
                return (false, $"Error returning book: {ex.Message}");
            }
        }
    }
}