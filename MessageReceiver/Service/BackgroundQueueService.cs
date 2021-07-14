using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace MessageReceiver.Service
{
    public class BackgroundQueueService : BackgroundService
    {

        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger logger;

        /// <summary>
        /// IServiceProvider: The interface contract that is used to subscibe the service registered in Depednency Injection Container for the ASP.NET Ciore apps
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="service"></param>
        public BackgroundQueueService(ILoggerFactory loggerFactory, IServiceProvider service)
        {
            logger = loggerFactory.CreateLogger<BackgroundQueueService>();
            InitializaRabbitMqSubscription();

        }

        /// <summary>
        /// Connect to RabbitMQ
        /// </summary>
        private void InitializaRabbitMqSubscription()
        {
            var factory = new ConnectionFactory()
            {
              //  Uri = new Uri("amqp://guest:guest@localhost:5672")
              // RabbieMQ in Container
              Uri = new Uri("amqp://guest:guest@emessage-rabbitmq:5672")
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // start subscribing to the RabbitMQ using the Exchange
            _channel.ExchangeDeclare("message.exchange",ExchangeType.Topic);
            // define the pattern of Queue from which the Message will be read
            _channel.QueueDeclare("message.queue", durable:false, exclusive:false, autoDelete:false, arguments:null);
            // use the queue for the subscription to the current application
            // Parameters
            // 1. Queue Name
            // 2. Exchange
            // 3. Routing Key for Pattern
            // 4. Any other Arguments
            _channel.QueueBind("message.queue", "message.exchange", "message.queue.*",null);
            // define a pre-fetch size and pre-fetch count for receiving data from queue
            
            _channel.BasicQos(0,1,false);

            // events subscription
            _connection.ConnectionShutdown += _connection_ConnectionShutdown;
        }
        /// <summary>
        ///  write the cleanup code when the connection to RabbitMQ is shut down
        ///  e.g. delete queuen or disconnect from Host contaiinng the RabbiqMQ 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute the BAckground service and should perform pre-designated operations for the background service
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            // Create a COnsumer object that will read data from RabbitMQ
            // using the channel
            var consumer = new EventingBasicConsumer(_channel);
            // start receiving messages
            consumer.Received += (ch, message) =>
            {
                // receive the message body
                var content = Encoding.UTF8.GetString(message.Body.ToArray());
                // handle message
                HandleReceiverMessaged(content);
                // acknowledge that messages are received from RabbitMQ
                _channel.BasicAck(message.DeliveryTag,false);
            };

            // detach the consumer from the RabbitMQ
            consumer.Shutdown += Consumer_Shutdown;
            consumer.Registered += Consumer_Registered;
            consumer.Unregistered += Consumer_Unregistered;
            consumer.ConsumerCancelled += Consumer_ConsumerCancelled;

            // signal the message consuming Process
            // this will start executing all the above events
            _channel.BasicConsume("message.queue",false, consumer);
            // complete the task
            return Task.CompletedTask;
             
        }

        private void Consumer_ConsumerCancelled(object sender, ConsumerEventArgs e)
        {
           
        }

        private void Consumer_Unregistered(object sender, ConsumerEventArgs e)
        {
            
        }

        private void Consumer_Registered(object sender, ConsumerEventArgs e)
        {
            
        }

        private void Consumer_Shutdown(object sender, ShutdownEventArgs e)
        {
             
        }

        /// <summary>
        /// clear all allocated objects
        /// </summary>
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        private void HandleReceiverMessaged(string content)
        {
            // process the message received from the RabbitMQ
            Debug.WriteLine($"Message Received from RabbitMQ is = {content}");
            // here you may wrtite code to save data in database
        }
    }
}
