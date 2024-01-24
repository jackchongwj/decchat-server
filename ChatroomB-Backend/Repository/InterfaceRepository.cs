using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;


namespace ChatroomB_Backend.Repository
{
    public interface IUserRepo
    {
        Task<IEnumerable<UserSearch>> GetByName(string profileName, int userId);                                                    //Get user by user profile name and filter friend request
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                                 // Get All Friend request
        Task<Users> GetUserById(int userId);
        Task<int> UpdateUserProfile(Users userProfile);
        Task<int> DeleteUserProfile(int userId);
        Task<int> ChangePassword(int userId, string newPassword);
        Task<IEnumerable<Users>> GetByName(string profileName);                                                         //Get user by user profile name
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
    }   

    public interface IFriendRepo
    {
        Task<int> AddFriends(Friends friends);                                                                                 // Add new friend 
        Task<int> UpdateFriendRequest (FriendRequest request);                                              // update friend request
       
    }

    public interface IChatRoomRepo
    {
        Task<int> AddChatRoom(FriendRequest request);                                             // add new ChatRoom and user chat room with private user
    }

    public interface IMessageRepo
    {
        Task<int> AddMessages(Messages message);                                                                                 // add new friend 
    }

    public interface IBlobRepo
    {
        Task<string> UploadImageFiles(string filepath, string filename, string folderPath);
        Task<string> UploadVideoFiles(string filepath, string filename, string folderPath);
        Task<string> UploadDocuments(string filepath, string filename, string folderPath);
        Task<List<string>> ListBlobs();
        Task DeleteBlob(string blobUri);
    }
}
