﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MinimalAPIConcepts.Context;

#nullable disable

namespace NEXT.GEN.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MinimalAPIConcepts.Models.User", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.PrimitiveCollection<string>("Addresses")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Course")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.PrimitiveCollection<string>("Socials")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("NEXT.GEN.Models.Friendships", b =>
                {
                    b.Property<int>("FriendshipId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FriendshipId"));

                    b.Property<string>("AnotherUserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FriendshipDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserName1")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FriendshipId");

                    b.HasIndex("UserName");

                    b.HasIndex("UserName1");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("NEXT.GEN.Models.Group", b =>
                {
                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CreatorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MemberCount")
                        .HasColumnType("int");

                    b.HasKey("GroupName");

                    b.HasIndex("CreatorName");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("NEXT.GEN.Models.GroupMembers", b =>
                {
                    b.Property<int>("GroupMemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupMemberId"));

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("JoinDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GroupMemberId");

                    b.HasIndex("GroupName");

                    b.HasIndex("UserName", "GroupName")
                        .IsUnique();

                    b.ToTable("GroupMembers");
                });

            modelBuilder.Entity("NEXT.GEN.Models.PostModel.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"));

                    b.Property<DateTime>("CommentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CommentText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserName");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("NEXT.GEN.Models.PostModel.CreatePost", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PostId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.PrimitiveCollection<string>("ImageUrls")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PostedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PostId");

                    b.HasIndex("UserName");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("NEXT.GEN.Models.PostModel.Dislike", b =>
                {
                    b.Property<int>("DislikeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DislikeId"));

                    b.Property<DateTime>("DislikeDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("DislikeId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserName");

                    b.ToTable("Dislikes");
                });

            modelBuilder.Entity("NEXT.GEN.Models.PostModel.Likes", b =>
                {
                    b.Property<int>("LikeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LikeId"));

                    b.Property<DateTime>("LikedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LikeId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserName");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("NEXT.GEN.Models.Friendships", b =>
                {
                    b.HasOne("MinimalAPIConcepts.Models.User", "Friend")
                        .WithMany("FriendsFriendships")
                        .HasForeignKey("UserName")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MinimalAPIConcepts.Models.User", "User")
                        .WithMany("UserFriendships")
                        .HasForeignKey("UserName1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Friend");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NEXT.GEN.Models.Group", b =>
                {
                    b.HasOne("MinimalAPIConcepts.Models.User", "User")
                        .WithMany("CreatedGroups")
                        .HasForeignKey("CreatorName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("NEXT.GEN.Models.GroupMembers", b =>
                {
                    b.HasOne("NEXT.GEN.Models.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupName")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MinimalAPIConcepts.Models.User", "User")
                        .WithMany("GroupMember")
                        .HasForeignKey("UserName")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NEXT.GEN.Models.PostModel.Comment", b =>
                {
                    b.HasOne("NEXT.GEN.Models.PostModel.CreatePost", "Post")
                        .WithMany("Comment")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MinimalAPIConcepts.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserName")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NEXT.GEN.Models.PostModel.CreatePost", b =>
                {
                    b.HasOne("MinimalAPIConcepts.Models.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserName")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("NEXT.GEN.Models.PostModel.Dislike", b =>
                {
                    b.HasOne("NEXT.GEN.Models.PostModel.CreatePost", "Post")
                        .WithMany("Dislikes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MinimalAPIConcepts.Models.User", "User")
                        .WithMany("Dislikes")
                        .HasForeignKey("UserName")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NEXT.GEN.Models.PostModel.Likes", b =>
                {
                    b.HasOne("NEXT.GEN.Models.PostModel.CreatePost", "Post")
                        .WithMany("Likes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MinimalAPIConcepts.Models.User", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserName")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MinimalAPIConcepts.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("CreatedGroups");

                    b.Navigation("Dislikes");

                    b.Navigation("FriendsFriendships");

                    b.Navigation("GroupMember");

                    b.Navigation("Likes");

                    b.Navigation("Posts");

                    b.Navigation("UserFriendships");
                });

            modelBuilder.Entity("NEXT.GEN.Models.Group", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("NEXT.GEN.Models.PostModel.CreatePost", b =>
                {
                    b.Navigation("Comment");

                    b.Navigation("Dislikes");

                    b.Navigation("Likes");
                });
#pragma warning restore 612, 618
        }
    }
}
