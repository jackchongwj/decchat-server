using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatroomB_Backend.Models;

namespace ChatroomB_Backend.Data
{
    public class ChatroomB_BackendContext : DbContext
    {
        public ChatroomB_BackendContext (DbContextOptions<ChatroomB_BackendContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; } = default!;
        public DbSet<Friends> Friends { get; set; } = default!;
        public DbSet<RefreshToken> RefreshToken { get; set; } = default!;
        public DbSet<ChatRooms> ChatRooms { get; set; } = default!;
        public DbSet<UserChatRooms> UserChatRooms { get; set; } = default!;
        public DbSet<Messages> Messages { get; set; } = default!;

    }
}
