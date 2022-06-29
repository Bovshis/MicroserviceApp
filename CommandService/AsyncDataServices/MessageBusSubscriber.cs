using System.Text;
using System.Xml.Serialization;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _chanel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]),
            };

            _connection = factory.CreateConnection();
            _chanel = _connection.CreateModel();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
            _chanel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _chanel.QueueDeclare().QueueName;
            _chanel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

            Console.WriteLine("--> Listening on the MessageBus");

        }

        public override void Dispose()
        {
            if (_chanel.IsOpen)
            {
                _chanel.Close();
                _connection.Close();
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_chanel);
            consumer.Received += (moduleHandle, ea) =>
            {
                Console.WriteLine("--> Event received");

                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(message);
            };

            _chanel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        private static void RabbitMQ_ConnectionShutDown(object? sender, ShutdownEventArgs eventArgs)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
}
