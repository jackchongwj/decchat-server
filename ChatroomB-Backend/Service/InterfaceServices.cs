using ChatroomB_Backend.Models;

namespace ChatroomB_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetByName(string profileName);                                                       //Get user by user profile name
    }
}
