﻿using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models.PostModel
{
    public class Likes
    {
        [Key]
        public int LikeId { get; set; }

        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public CreatePost Post { get; set; }

        public string UserName { get; set; }
        [ForeignKey("UserName")]
        public User User { get; set; }

        public DateTime LikedDate { get; set; } 
    }
}
