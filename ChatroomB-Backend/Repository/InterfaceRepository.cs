using ChatroomB_Backend.Models;


namespace ChatroomB_Backend.Repository
{
    public interface IUserRepo
    {
        Task<IEnumerable<Users>> GetByName(string profileName);                                                         //Get user by user profile name
    }   

    public interface IFriendRepo
    {
        Task<int> AddFriends(Friends friends);                                                                                 // add new friend 
        Task<IEnumerable<Users>> GetFriendList(int userId); //return friendllist
    }
}
