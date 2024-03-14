using Azure.Storage;
using Azure.Storage.Blobs;
using ChatroomB_Backend.Repository;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace ChatroomB_Backend.Service
{
    public class BlobServices: IBlobService
    {
        private readonly IBlobRepo _blobRepo;
        TimeZoneInfo singaporeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");

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
            audioName = CheckFileNameLength(audioName);
            string newFileName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, singaporeTimeZone).ToString("dd-MM-yyyy h:mm:ss tt") + "-" + audioName;
            string blobUri = await _blobRepo.UploadAudios(audioByte, newFileName, folderpath);
            return blobUri;
        }

        public async Task<string> UploadDocuments(byte[] docByte, string docName)
        {
            string folderpath = "Messages/Documents";
            docName = CheckFileNameLength(docName);
            string newFileName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, singaporeTimeZone).ToString("dd-MM-yyyy h:mm:ss tt") + "-" + docName;
            string blobUri = await _blobRepo.UploadDocuments(docByte, newFileName, folderpath);
            string decodedUrl = WebUtility.UrlDecode(blobUri);
            return decodedUrl;
        }

        public async Task<string> UploadImageFiles(byte[] fileByte, string filename, int CaseImageFile)
        {
            string directory = "";
            filename = CheckFileNameLength(filename);
            string newFileName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, singaporeTimeZone).ToString("dd-MM-yyyy h:mm:ss tt") + "-" + Path.GetFileNameWithoutExtension(filename) + ".webp";
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
            vidName = CheckFileNameLength(vidName);
            string newFileName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, singaporeTimeZone).ToString("dd-MM-yyyy h:mm:ss tt") + "-" + vidName;
            string blobUri = await _blobRepo.UploadVideoFiles(vidByte, newFileName, folderpath);
            string decodedUrl = WebUtility.UrlDecode(blobUri);
            return decodedUrl;
        }

        private string CheckFileNameLength(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return filename; // or throw an exception or return an alternate value based on your requirements
            }

            string extension = Path.GetExtension(filename);
            string baseName = Path.GetFileNameWithoutExtension(filename);
            baseName = baseName.Length > 150 ? baseName.Substring(0, 150) : baseName;

            return baseName + extension;
        }
    }
}
