using ChatroomB_Backend.Models;
using RabbitMQ.Client;
using System.Configuration;
using System.Text.Json;
using System.Threading.Channels;

namespace ChatroomB_Backend.Service
{
    public class ApplicationServices
    {
        private readonly RabbitMQServices _rabbitMQService;
        private readonly IMessageService _messageService;
        private readonly BlobServices _blobService;

        public ApplicationServices(RabbitMQServices _rabbitMQSer, IMessageService messageService, BlobServices blobService)
        {
            _rabbitMQService = _rabbitMQSer;
            _messageService = messageService;
            _blobService = blobService;
            ConsumeQueueMessage();
        }

        public void ConsumeQueueMessage()
        {
            _rabbitMQService.ConsumeMessage(ProcessMessage);
        }

        private async Task ProcessMessage(string message)
        {
            try
            {
                Messages? messages = JsonSerializer.Deserialize<Messages>(message);
                if (messages != null)
                {
                    await (messages.ResourceUrl != null ? StoreMessageWithAttachment(messages) : StoreDatabase(messages));
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error during JSON deserialization: " + ex.Message);
            }

        }

        private async Task StoreMessageWithAttachment(Messages messages)
        {
            Console.WriteLine("StoreMessageWithAttachment: " + messages.Content);
            messages.ResourceUrl = await StoreImageBlob(messages);
            await StoreDatabase(messages);
        }

        private async Task<string> StoreImageBlob(Messages messages)
        {
            Console.WriteLine("StoreBlob: " + messages.Content);
            string resourceUrl = messages.ResourceUrl ?? throw new ArgumentException("ResourceUrl is null");
            string folderpath = "Messages/Images";

            return await _blobService.UploadImageFiles(resourceUrl, folderpath);
        }

        private async Task StoreDatabase(Messages messages)
        {
            Console.WriteLine("StoreDatabase: " + messages.Content);
            await _messageService.AddMessages(messages);
        }
    }
}
