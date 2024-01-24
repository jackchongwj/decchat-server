using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;


namespace ChatroomB_Backend.Repository
{
    public interface IUserRepo
    {
        Task<IEnumerable<Users>> GetByName(string profileName);                                                         //Get user by user profile name
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
    }   

    public interface IFriendRepo
    {
        Task<int> AddFriends(Friends friends);                                                                                 // add new friend 

    }
}
