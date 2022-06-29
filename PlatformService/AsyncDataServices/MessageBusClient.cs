using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _chanel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]),
            };

            try
            {
                _connection = factory.CreateConnection();
                _chanel = _connection.CreateModel();
                _chanel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
                Console.WriteLine("--> Connected to the MessageBus");

            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {e.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection is open");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection is closed");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _chanel.BasicPublish(exchange: "trigger", routingKey:"", basicProperties: null, body: body);
            Console.WriteLine($"--> Sent {message}");
        }

        public void Dispose()
        {
            if (_chanel.IsOpen)
            {
                _chanel.Close();
                _connection.Close();
            }
        }

        private static void RabbitMQ_ConnectionShutDown(object? sender, ShutdownEventArgs eventArgs)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
}
