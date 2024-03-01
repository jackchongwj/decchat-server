using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlobsController : ControllerBase
    {
        private readonly string _storageConnectionString;
        private readonly string _containerName;

        private readonly IBlobService _blobService;

        public BlobsController(IConfiguration configuration, IBlobService blobService)
        {
            _storageConnectionString = configuration.GetSection("AzureBlobStorage")["StorageConnectionString"]
                        ?? throw new InvalidOperationException("Storage connection string not found.");
            _containerName = configuration.GetSection("AzureBlobStorage")["ContainerName"]
                             ?? throw new InvalidOperationException("Container name not found.");

            _blobService = blobService;
        }
       
        [HttpGet]
        [Authorize]
        public IActionResult GetSasToken()
        {
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _containerName);

            // Set the permissions and expiry time for the SAS token
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _containerName,
                Resource = "c",
                ExpiresOn = DateTime.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Write | BlobContainerSasPermissions.Create | BlobContainerSasPermissions.Read);

            // Generate the SAS token
            string sasToken = container.GenerateSasUri(sasBuilder).Query;

            return Ok(sasToken);

        }

        [HttpPost]
        [Authorize]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided.");
            }

            Console.WriteLine(file);

            return Ok("File successfully uploaded");
        }
    }
}
