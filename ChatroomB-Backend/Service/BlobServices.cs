﻿using Azure.Storage;
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

        public async Task<string> UploadDocuments(string filepath)
        {
            string folderpath = "";
            string newFileName = Path.GetFileNameWithoutExtension(filepath) + "-" + DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + ".webp";
            string blobUri = await _blobRepo.UploadDocuments(filepath, newFileName, folderpath);
            return blobUri;
        }

        public async Task<string> UploadImageFiles(byte[] fileByte, string filename, int CaseImageFile)
        {
            string directory = "";
            string newFileName = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "-" + filename;
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

        public async Task<string> UploadVideoFiles(string filepath)
        {
            string folderpath = "";
            string newFileName = Path.GetFileNameWithoutExtension(filepath) + "-" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + ".webp";
            string blobUri = await _blobRepo.UploadVideoFiles(filepath, newFileName, folderpath);
            return blobUri;
        }
    }
}
