using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using MessageSender.Models;
using RabbitMQ.Client;
namespace MessageSender.Infrastructure
{
    public class SenderService
    {
        public void PublishMessage(Employee employee)
        {
            // create a conneciton to RabbitMQ using the Connection Factory
            var factory = new ConnectionFactory()
            {
                              //protocol://username:password@container:port  
                // Uri = new Uri("amqp://guest:guest@localhost:5672")
                // use this for communication across containers
                Uri = new Uri("amqp://guest:guest@emessage-rabbitmq:5672")
            };

            // create a connection
            var connection = factory.CreateConnection();
            // create a channel
            var channel = connection.CreateModel();
            // define the max time for the message in the queue, this is known as
            // Time-To-Live
            var ttl = new Dictionary<string, object>
            {
                { "x-message-ttl", 60000 } // message will be live for 30 seconds
            };

            // define an exchange so that messages will be published
            channel.ExchangeDeclare("message.exchange", ExchangeType.Topic, arguments:ttl);

            // prepare the messsage to be send
            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(employee));
            // publish the message
            // Parameters
            // 1. The Exchane Name that will be used for Messaging
            // 2. Routing-Key, the queue that will be matched as pattern by the receiver to receive message
            // 3. Other Message Properties e.g. version, headers, etc
            // 4. The actual Message
            channel.BasicPublish("message.exchange", "message.queue.*", null, messageBody);

        }
    }
}
