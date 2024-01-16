using ChatroomB_Backend.Data;
using ChatroomB_Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ChatroomB_Backend.Repository
{
    public class UsersRepo : IUserRepo
    {
        private readonly ChatroomB_BackendContext _context;

        public UsersRepo(ChatroomB_BackendContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Users>> GetByName(string profileName)
        {
            SqlParameter param = new SqlParameter("@profileName", profileName);

            List<Users> result = await Task.Run(() => _context.Users
                .FromSqlRaw(@"exec GetUserByProfileName @profileName", param)
                .ToListAsync());

            return result;
        }
    }
}
