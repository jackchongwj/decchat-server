using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using ChatroomB_Backend.Models;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client.Events;
using System.Threading.Channels;
using ChatroomB_Backend.DTO;

namespace ChatroomB_Backend.Service
{
    public class RabbitMQServices
    {
        private readonly string RabbitMQConnection;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string queueName = "MessagePortal";

        public RabbitMQServices(IConfiguration configuration)
        {
            //RabbitMQConnection = configuration.GetConnectionString("RabbitMQConnectionString");
            RabbitMQConnection = configuration.GetSection("RabbitMQConnection")["RabbitMQConnectionString"]
                         ?? throw new InvalidOperationException("Connection string not found.");

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri(RabbitMQConnection);
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        }

        public void PublishMessage(FileMessage fileMessage)
        {
            try
            {
                byte[] serializedMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(fileMessage));
                
                _channel.BasicPublish(exchange: "", routingKey: queueName, body: serializedMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing message: {ex}");
            }
        }
        public void PublishEditMessage(editMessage editMsg)
        {
            try
            {
                byte[] serializedMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(editMsg));

                _channel.BasicPublish(exchange: "", routingKey: queueName, body: serializedMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing message: {ex}");
            }
        }

        public void ConsumeMessage(Func<string, Task> onMessageReceived)
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            // Register event handler for Received
            consumer.Received += (model, ea) =>
            {
                // Define actions when a msg is received
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                // Invoke the callback
                onMessageReceived?.Invoke(message);
            };

            _channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);
        }

    }
}
