﻿using System.ComponentModel.DataAnnotations;

namespace NEXT.GEN.Dtos.UserDto
{
    public class GetUserDto
    {
        public string? Id { get; set; }  
        public string? UserName {  get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? Course { get; set; }
        public string? Address { get; set; }
        public ICollection<string>? Socials { get; set; } = new List<string>();
        public ICollection<string>? Skills { get; set; } = new List<string>();

        // The user stats
        public UserStatsResponseDto? Stats { get; set; } = new UserStatsResponseDto();

    }
}
