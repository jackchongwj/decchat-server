using Azure.Storage;
using Azure.Storage.Blobs;
using ChatroomB_Backend.Repository;
using Microsoft.Extensions.Configuration;

namespace ChatroomB_Backend.Service
{
    public class BlobServices
    {
        private readonly IBlobRepo _blobRepo;

        public BlobServices(IBlobRepo blobRepo)
        {
            _blobRepo = blobRepo;
        }

        public async Task<string> UploadImageFiles(string filepath, string folderPath)
        {
            string newFileName = Path.GetFileNameWithoutExtension(filepath) + "-" + DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + ".webp";
            string blobUri = await _blobRepo.UploadImageFiles(filepath, newFileName, folderPath);
            return blobUri;
        }

    }
}
