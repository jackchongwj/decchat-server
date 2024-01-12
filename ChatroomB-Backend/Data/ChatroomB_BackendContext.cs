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

        public DbSet<ChatroomB_Backend.Models.Users> Users { get; set; } = default!;
    }
}
