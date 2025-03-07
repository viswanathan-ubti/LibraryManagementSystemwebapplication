using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, StringLength(50)]
        public string AdminCode { get; set; }
        [Required, StringLength(255)]
        public string Password { get; set; }
    }
}