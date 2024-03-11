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
            using (JsonDocument doc = JsonDocument.Parse(message))
            {
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("Message", out JsonElement inmessage))
                {
                    // If "Message" exists, try to get "MessageId" from within it
                    if (inmessage.TryGetProperty("MessageId", out JsonElement nestedMessageIdElement))
                    {
                        try
                        {
                            FileMessage? fm = JsonSerializer.Deserialize<FileMessage>(message);
                            if (fm != null)
                            {
                                if (fm.FileByte != null)
                                {
                                    await StoreMessageWithAttachment(fm);
                                }
                                else
                                {
                                    await StoreDatabase(fm);
                                }
                            }
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"JSON deserialization error: {ex.Message}");
                        }
                    }
                    else
                    {
                        // Handle the case where "MessageId" does not exist within "Message"
                        Console.WriteLine("MessageId within Message object not found.");
                        return;
                    }
                }
                else if (root.TryGetProperty("MessageId", out JsonElement directMessageIdElement))
                {
                    try
                    {
                        EditMessage? chatRoomMessage = JsonSerializer.Deserialize<EditMessage>(message);
                        if (chatRoomMessage != null)
                        {
                            await _messageService.EditMessage(chatRoomMessage);
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON deserialization error: {ex.Message}");
                    }
                }
                else
                {
                    // Handle the case where neither "Message" nor direct "MessageId" exists
                    Console.WriteLine("Neither Message object nor direct MessageId found.");
                    return;
                }
            }
        }

        private async Task StoreMessageWithAttachment(FileMessage fm)
        {
            if (fm.Message == null || fm.FileByte == null || fm.FileName == null)
            {
                // Handle the null case for fm.Message
                throw new InvalidOperationException("File message cannot be null.");
            }

            switch(fm.FileType)
            {
                case ("image"):
                    fm.Message.ResourceUrl = await StoreImageBlob(fm.FileByte, fm.FileName);
                    break;
                case ("video"):
                    fm.Message.ResourceUrl = await StoreVideoBlob(fm.FileByte, fm.FileName);
                    break;
                case ("audio"):
                    fm.Message.ResourceUrl = await StoreAudioBlob(fm.FileByte, fm.FileName);
                    break;
                default:
                    fm.Message.ResourceUrl = await StoreDocsBlob(fm.FileByte, fm.FileName);
                    break;
            }
            
            await StoreDatabase(fm);
        }

        private async Task<string> StoreImageBlob(byte[] FileByte, string filename)
        {
            return await _blobService.UploadImageFiles(FileByte, filename, 1);
        }

        private async Task<string> StoreVideoBlob(byte[] FileByte, string filename)
        {
            return await _blobService.UploadVideoFiles(FileByte, filename);
        }

        private async Task<string> StoreDocsBlob(byte[] FileByte, string filename)
        {
            return await _blobService.UploadDocuments(FileByte, filename);
        }

        private async Task<string> StoreAudioBlob(byte[] FileByte, string filename)
        {
            return await _blobService.UploadAudios(FileByte, filename);
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
