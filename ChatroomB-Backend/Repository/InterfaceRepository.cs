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
