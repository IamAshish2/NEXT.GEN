using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NEXT.GEN.Models;
using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        // override the default identity users table
        override 
        public DbSet<User> Users { get; set; }
        public DbSet<ProfilePicture> ProfilePictures { get; set; }
        public DbSet<Friendships> Friends { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMembers> GroupMembers { get; set; }
        public DbSet<CreatePost> Posts { get; set; }
        public DbSet<Likes> Likes { get; set; } 
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Dislike> Dislikes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Friendships>()
                .HasOne(f => f.User)
                .WithMany(f => f.UserFriendships)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Friendships>()
            //    .HasOne(f => f.Friend)
            //    .WithMany(f => f.FriendsFriendships)
            //    .HasForeignKey(f => f.UserName)
            //    .OnDelete(DeleteBehavior.Restrict);

            // the unique key for the groupMembers table
            modelBuilder.Entity<GroupMembers>().HasKey(f => f.GroupMemberId);

            // one group can have multiple members
            modelBuilder.Entity<GroupMembers>()
                .HasOne(g => g.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(g => g.GroupName)
                //.OnDelete(DeleteBehavior.Restrict);
                .OnDelete(DeleteBehavior.Cascade);

            // one user can join multiple groups
            modelBuilder.Entity<GroupMembers>()
                .HasOne(g => g.User)
                .WithMany(g => g.GroupMember)
                .HasForeignKey(g => g.MemberId)
                .OnDelete(DeleteBehavior.NoAction);
                //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMembers>()
              .HasIndex(u => new { u.GroupMemberId, u.GroupName }).IsUnique();

            // 
            modelBuilder.Entity<Comment>().HasKey(c => c.CommentId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(c => c.Comment)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Likes>().HasKey(l => l.LikeId);

            modelBuilder.Entity<Likes>()
               .HasOne(c => c.User)
               .WithMany(c => c.Likes)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Likes>()
                .HasOne(c => c.Post)
                .WithMany(c => c.Likes)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Dislike>().HasKey(l => l.DislikeId);
            modelBuilder.Entity<Dislike>()
               .HasOne(c => c.User)
               .WithMany(c => c.Dislikes)
               .HasForeignKey(c => c.UserName)
               .OnDelete(DeleteBehavior.NoAction);

           modelBuilder.Entity<Dislike>()
                .HasOne(c => c.Post)
                .WithMany(c => c.Dislikes)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CreatePost>().HasKey(p => p.PostId);
            modelBuilder.Entity<CreatePost>()
               .HasOne(p => p.User)
               .WithMany(u => u.Posts)
               .HasForeignKey(p => p.CreatorId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
