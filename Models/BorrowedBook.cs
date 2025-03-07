using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class BorrowedBook
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; }
        [NotMapped]
        public DateTime DueDate => BorrowDate.AddDays(14);

    }
}