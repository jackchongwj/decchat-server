using Azure;
using Azure.Core;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChatroomB_Backend.Models;
using Microsoft.AspNetCore.WebUtilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System.Reflection.Metadata;

namespace ChatroomB_Backend.Repository
{
    public class BlobsRepo : IBlobRepo
    {
        private readonly BlobServiceClient _blobServiceClient;
        private BlobContainerClient client;

        private readonly string _storageConnectionString;
        private readonly string _containerName;

        public BlobsRepo(IConfiguration configuration)
        {
            try
            {
                _storageConnectionString = configuration.GetSection("AzureBlobStorage")["StorageConnectionString"]
                                            ?? throw new InvalidOperationException("Storage connection string not found.");
                _containerName = configuration.GetSection("AzureBlobStorage")["ContainerName"]
                                ?? throw new InvalidOperationException("Container name not found.");

                _blobServiceClient = new BlobServiceClient(_storageConnectionString);
                client = _blobServiceClient.GetBlobContainerClient(_containerName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize BlobServiceClient", ex);
            }
        }

        public async Task<string> UploadImageFiles(byte[] imgByte, string filename, string folderPath)
        {
            try
            {
                // example folderPath : "images/folder1"
                string blobName = folderPath.TrimEnd('/') + '/' + filename;
                BlobClient blobClient = client.GetBlobClient(blobName);

                using (Image image = Image.Load(imgByte))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        WebpEncoder encoder = new WebpEncoder();
                        await image.SaveAsWebpAsync(ms, encoder);
                        ms.Position = 0; // Reset the memory stream position after writing.

                        // Upload the WebP image
                        await blobClient.UploadAsync(ms);
                    }
                }

                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to upload image file", ex);
            }
        }

        public async Task<string> UploadVideoFiles(byte[] vidByte, string filename, string folderPath)
        {
            try
            {
                // example folderPath : "images/folder1"
                string blobName = folderPath.TrimEnd('/') + '/' + filename;
                BlobClient blobClient = client.GetBlobClient(blobName);
                string encodedFilename = Uri.EscapeDataString(filename);

                // Upload the video file
                using (MemoryStream ms = new MemoryStream(vidByte))
                {
                    await blobClient.UploadAsync(ms, new BlobHttpHeaders
                    {
                        ContentType = "video/mp4",
                        //ContentDisposition = "inline; filename=\"" + blobName + "\""
                        ContentDisposition = $"inline; filename*=UTF-8''{encodedFilename}"
                    });
                }

                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to upload video file", ex);
            }
        }

        public async Task<string> UploadDocuments(byte[] docByte, string filename, string folderPath)
        {
            try
            {
                // example folderPath : "images/folder1"
                string blobName = folderPath.TrimEnd('/') + '/' + filename;
                BlobClient blobClient = client.GetBlobClient(blobName);

                // Upload documents
                using (MemoryStream ms = new MemoryStream(docByte))
                {
                    await blobClient.UploadAsync(ms);
                }

                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to upload document", ex);
            }
        }

        public async Task<string> UploadAudios(byte[] audioByte, string filename, string folderPath)
        {
            try
            {
                // example folderPath : "images/folder1"
                string blobName = folderPath.TrimEnd('/') + '/' + filename;
                BlobClient blobClient = client.GetBlobClient(blobName);

                // Upload audio file
                using (MemoryStream ms = new MemoryStream(audioByte))
                {
                    await blobClient.UploadAsync(ms);
                }

                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to upload audio file", ex);
            }
        }

        public async Task DeleteBlob(string blobUri)
        {
            try
            {
                Uri uri = new Uri(blobUri);
                string blobName = String.Join("", uri.Segments.Skip(2));
                BlobClient blobClient = client.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete blob", ex);
            }
        }
    }
}
