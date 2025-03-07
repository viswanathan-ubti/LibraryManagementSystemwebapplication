using System;
using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Enums;

namespace LibraryManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, StringLength(50)]
        public string LibraryCode { get; set; }
        [Required]
        public DateTime SubscriptionEndDate { get; set; }
        [Required]
        public SubscriptionStatus Status { get; set; }
    }
}