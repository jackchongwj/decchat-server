using Azure;
using Azure.Core;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChatroomB_Backend.Models;

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
            _storageConnectionString = configuration.GetSection("AzureBlobStorage")["StorageConnectionString"]
                         ?? throw new InvalidOperationException("Storage connection string not found.");
            _containerName = configuration.GetSection("AzureBlobStorage")["ContainerName"]
                         ?? throw new InvalidOperationException("Container name not found.");

            _blobServiceClient = new BlobServiceClient(_storageConnectionString);
            client = _blobServiceClient.GetBlobContainerClient(_containerName);
        }

        public async Task<List<string>> ListBlobs()
        {
            List<string> blobList = new List<string>();

            await foreach(var blobItem in client.GetBlobsAsync())
            {
                blobList.Add(blobItem.Name);
            }
            return blobList;
        }

        //public async Task<BlobObjects> RetrieveResouceFile(string url)
        //{
        //    string? filename = new Uri(url).Segments.LastOrDefault();

        //    try
        //    {
        //        BlobClient blobClient = client.GetBlobClient(filename);
        //        if(await blobClient.ExistsAsync())
        //        {
        //            BlobDownloadResult content = await blobClient.DownloadContentAsync();
        //            Stream downloadedData = content.Content.ToStream();

        //            return new BlobObjects { BlobContent = downloadedData};
        //        }
        //        else
        //        {
        //            throw new FileNotFoundException($"Blob with filename '{filename}' not found.");
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine("Exception caught: " + ex);
        //        throw;
        //    }
        //}

        public async Task<string> UploadImageFiles(string filePath, string filename, string folderPath)
        {
            // example folderPath : "images/folder1"
            string blobName = folderPath.TrimEnd('/') + '/' + filename;
            BlobClient blobClient = client.GetBlobClient(blobName);

            // Convert image to WebP
            using (Image image = await Image.LoadAsync(filePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await image.SaveAsWebpAsync(ms);
                    ms.Position = 0; // Reset the memory stream position after writing.

                    // Upload the WebP image
                    await blobClient.UploadAsync(ms);
                }
            }
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadVideoFiles(string filepath, string filename, string folderPath)
        {
            // example folderPath : "images/folder1"
            string blobName = folderPath.TrimEnd('/') + '/' + filename;
            BlobClient blobClient = client.GetBlobClient(blobName);

            // Upload the video file
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                await blobClient.UploadAsync(fs);
            }

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadDocuments(string filepath, string filename, string folderPath)
        {
            // example folderPath : "images/folder1"
            string blobName = folderPath.TrimEnd('/') + '/' + filename;
            BlobClient blobClient = client.GetBlobClient(blobName);

            // Upload documents
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                await blobClient.UploadAsync(fs);
            }

            return blobClient.Uri.AbsoluteUri;
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
            catch(Exception)
            {
                throw;
            }
        }
    }
}
