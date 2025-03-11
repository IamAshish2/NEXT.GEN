
using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Models;
using NEXT.GEN.Models;
using NEXT.GEN.Models.PostModel;
namespace MinimalAPIConcepts.Context
{
    public class ApplicationDbContext:DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Friendships> Friends { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMembers> GroupMembers { get; set; }
        public DbSet<Post> Posts { get; set; }

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

            modelBuilder.Entity<Friendships>()
                .HasOne(f => f.Friend)
                .WithMany(f => f.FriendsFriendships)
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            // the unique key for the groupMembers table
            modelBuilder.Entity<GroupMembers>().HasKey(f => f.GroupMemberId);

            // one group can have multiple members
            modelBuilder.Entity<GroupMembers>()
                .HasOne(g => g.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(g => g.GroupName)
                .OnDelete(DeleteBehavior.Restrict);

            // one user can join multiple groups
            modelBuilder.Entity<GroupMembers>()
                .HasOne(g => g.User)
                .WithMany(g => g.GroupMember)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GroupMembers>()
              .HasIndex(u => new { u.UserId, u.GroupName }).IsUnique();

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
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.NoAction);

           modelBuilder.Entity<Dislike>()
                .HasOne(c => c.Post)
                .WithMany(c => c.Dislikes)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Post>().HasKey(p => p.PostId);
            modelBuilder.Entity<Post>()
               .HasOne(p => p.User)
               .WithMany(u => u.Posts)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
