using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Title { get; set; }
        [Required, StringLength(100)]
        public string Author { get; set; }
        [Required, StringLength(50)]
        public string BookCode { get; set; }
        [Required]
        public bool IsAvailable { get; set; } = true;
    }
}