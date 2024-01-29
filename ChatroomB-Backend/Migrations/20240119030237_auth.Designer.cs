﻿// <auto-generated />
using System;
using ChatroomB_Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChatroomB_Backend.Migrations
{
    [DbContext(typeof(ChatroomB_BackendContext))]
    [Migration("20240119030237_auth")]
    partial class auth
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChatroomB_Backend.Models.ChatRooms", b =>
                {
                    b.Property<int>("ChatRoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChatRoomId"));

                    b.Property<int>("InitiatedBy")
                        .HasColumnType("int");

                    b.Property<bool?>("IsDeleted")
                        .IsRequired()
                        .HasColumnType("bit");

                    b.Property<string>("RoomName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoomProfilePic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("RoomType")
                        .HasColumnType("bit");

                    b.HasKey("ChatRoomId");

                    b.ToTable("ChatRooms");
                });

            modelBuilder.Entity("ChatroomB_Backend.Models.Friends", b =>
                {
                    b.Property<int?>("RequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("RequestId"));

                    b.Property<int?>("ReceiverId")
                        .HasColumnType("int");

                    b.Property<int?>("SenderId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("RequestId");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("ChatroomB_Backend.Models.Messages", b =>
                {
                    b.Property<int?>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("MessageId"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("MessageType")
                        .HasColumnType("int");

                    b.Property<string>("ResourceUrl")
                        .HasColumnType("varchar(256)");

                    b.Property<DateTime?>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserChatRoomId")
                        .HasColumnType("int");

                    b.HasKey("MessageId");

                    b.HasIndex("UserChatRoomId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("ChatroomB_Backend.Models.RefreshToken", b =>
                {
                    b.Property<Guid>("TokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ExpiredDateTime")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("TokenHash")
                        .IsRequired()
                        .HasColumnType("varchar(256)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("TokenId");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshToken");
                });

            modelBuilder.Entity("ChatroomB_Backend.Models.UserChatRooms", b =>
                {
                    b.Property<int?>("UserChatRoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("UserChatRoomId"));

                    b.Property<int?>("ChatRoomId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserChatRoomId");

                    b.HasIndex("ChatRoomId");

                    b.HasIndex("UserId");

                    b.ToTable("UserChatRooms");
                });

            modelBuilder.Entity("ChatroomB_Backend.Models.Users", b =>
                {
                    b.Property<int?>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("UserId"));

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ProfileName")
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("varchar(256)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(15)");

                    b.HasKey("UserId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ChatroomB_Backend.Models.Friends", b =>
                {
                    b.HasOne("ChatroomB_Backend.Models.Users", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId");

                    b.HasOne("ChatroomB_Backend.Models.Users", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("ChatroomB_Backend.Models.Messages", b =>
                {
                    b.HasOne("ChatroomB_Backend.Models.UserChatRooms", "UserChatRooms")
                        .WithMany()
                        .HasForeignKey("UserChatRoomId");

                    b.Navigation("UserChatRooms");
                });

            modelBuilder.Entity("ChatroomB_Backend.Models.RefreshToken", b =>
                {
                    b.HasOne("ChatroomB_Backend.Models.Users", "Users")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("ChatroomB_Backend.Models.UserChatRooms", b =>
                {
                    b.HasOne("ChatroomB_Backend.Models.ChatRooms", "ChatRooms")
                        .WithMany()
                        .HasForeignKey("ChatRoomId");

                    b.HasOne("ChatroomB_Backend.Models.Users", "Users")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("ChatRooms");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
