﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NEXT.GEN.Models
{
    public class GroupMembers
    {
        [Key,Required]
        public int GroupMemberId { get; set; }
        public DateTime JoinDate { get; set; }
        //public string UserName { get; set; }
        //[ForeignKey("UserName")]

        [Required, StringLength(450)]
        public string MemberId { get; set; }
        [ForeignKey("MemberId")]
        public User User { get; set; }
        //public int GroupId {  get; set; }
        // make groupName the foreign key 
        public string GroupName { get; set; }
        [ForeignKey("GroupName")]
        [JsonIgnore]
        public Group Group { get; set; }

        public bool HasJoined { get; set; }
    }
}
