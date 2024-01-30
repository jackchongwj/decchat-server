using ChatroomB_Backend.DTO;
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
        private readonly IBlobService _blobService;

        public ApplicationServices(RabbitMQServices _rabbitMQSer, IMessageService messageService, IBlobService blobService)
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
                FileMessage? fm = JsonSerializer.Deserialize<FileMessage>(message);
                if (fm != null)
                {
                    Console.WriteLine(fm);
                    if (fm.FileByte != null)
                    {
                        await StoreMessageWithAttachment(fm);
                    }
                    else
                    {
                        await StoreDatabase(fm);
                    }
                    //await Task.Delay(100);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error during JSON deserialization: " + ex.Message);
            }
        }


        private async Task StoreMessageWithAttachment(FileMessage fm)
        {
            if (fm.Message == null || fm.FileByte == null || fm.FileName == null)
            {
                // Handle the null case for fm.Message
                throw new InvalidOperationException("File message cannot be null.");
            }

            fm.Message.ResourceUrl = await StoreImageBlob(fm.FileByte, fm.FileName);
            await StoreDatabase(fm);
        }

        private async Task<string> StoreImageBlob(byte[] FileByte, string filename)
        {
            return await _blobService.UploadImageFiles(FileByte, filename, 1);
        }

        private async Task StoreDatabase(FileMessage fm)
        {
            if (fm.Message == null)
            {
                // Handle the null case for fm.Message
                throw new InvalidOperationException("Message cannot be null.");
            }

            await _messageService.AddMessages(fm.Message);
        }
    }
}
