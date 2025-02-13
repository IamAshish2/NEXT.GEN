﻿using System.ComponentModel.DataAnnotations;

namespace MinimalAPIConcepts.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty!;

    }
}
