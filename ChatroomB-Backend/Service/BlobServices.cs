using Azure.Storage;
using Azure.Storage.Blobs;
using ChatroomB_Backend.Repository;
using Microsoft.Extensions.Configuration;

namespace ChatroomB_Backend.Service
{
    public class BlobServices: IBlobService
    {
        private readonly IBlobRepo _blobRepo;

        public BlobServices(IBlobRepo blobRepo)
        {
            _blobRepo = blobRepo;
        }

        public async Task DeleteBlob(string blobUri)
        {
            await _blobRepo.DeleteBlob(blobUri);
        }

        public async Task<string> UploadAudios(byte[] audioByte, string audioName)
        {
            string folderpath = "Messages/Audios";
            string newFileName = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "-" + audioName;
            string blobUri = await _blobRepo.UploadAudios(audioByte, newFileName, folderpath);
            return blobUri;
        }

        public async Task<string> UploadDocuments(byte[] docByte, string docName)
        {
            string folderpath = "Messages/Documents";
            string newFileName = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "-" + docName;
            string blobUri = await _blobRepo.UploadDocuments(docByte, newFileName, folderpath);
            return blobUri;
        }

        public async Task<string> UploadImageFiles(byte[] fileByte, string filename, int CaseImageFile)
        {
            string directory = "";
            string newFileName = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "-" + Path.GetFileNameWithoutExtension(filename) + ".webp";
            switch (CaseImageFile)
            {
                // Message Attached Image
                case 1:
                    directory = "Messages/Images";
                    break;
                // User Profile Picture
                case 2:
                    directory = "UserProfilePicture";
                    break;
                // Group Profile Picture
                case 3:
                    directory = "GroupProfilePicture";
                    break;
                default:
                    directory = "";
                    break;
            }

            string blobUri = await _blobRepo.UploadImageFiles(fileByte, newFileName, directory);

            return blobUri;
        }

        public async Task<string> UploadVideoFiles(byte[] vidByte, string vidName)
        {
            string folderpath = "Messages/Videos";
            string newFileName = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "-" + vidName;
            string blobUri = await _blobRepo.UploadVideoFiles(vidByte, newFileName, folderpath);
            return blobUri;
        }
    }
}
