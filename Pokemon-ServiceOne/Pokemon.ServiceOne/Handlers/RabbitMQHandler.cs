using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pokemon.ServiceOne.Interfaces;
using Pokemon.ServiceOne.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.ServiceOne.Handlers
{
    public class RabbitMQHandler : IRabbitMQHandler
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private ConnectionFactory factory;
        private EventingBasicConsumer consumer;
        private IBasicProperties props;
        private BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private ILogger logger;

        public RabbitMQHandler(ConnectionFactory factory,
            ILogger<RabbitMQHandler> logger)
        {
            this.factory = factory;
            this.logger = logger;

            Setup();
        }

        private void Setup()
        {
            factory.HostName = "localhost";
            factory.Port = 5672;
            factory.RequestedHeartbeat = 60;

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            SetupQueues(channel);          

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
        }

        private static void SetupQueues(IModel channel)
        {
            var args = new Dictionary<string, object>
            {
                { "x-message-ttl", 5000 }
            };

            channel.ExchangeDeclare(exchange: Constants.TYPESEXCHANGE, type: "topic");
            channel.ExchangeDeclare(exchange: Constants.LEGENDARYEXCHANGE, type: "topic");
            channel.ExchangeDeclare(exchange: Constants.BASEEXCHANGE, type: "topic");
            channel.ExchangeDeclare(exchange: Constants.HEADERSEXCHANGE, type: "topic");
            channel.ExchangeDeclare(exchange: Constants.BATTLEEXCHANGE, type: "topic");

            channel.QueueDeclare(queue: Constants.TYPESEARCH,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: args);

            channel.QueueDeclare(queue: Constants.TYPESSEARCH,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: args);

            channel.QueueDeclare(queue: Constants.LEGENDARYLIST,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: args);

            channel.QueueDeclare(queue: Constants.NAMESEARCH,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: args);

            channel.QueueDeclare(queue: Constants.HEADERSSEARCH,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: args);

            channel.QueueDeclare(queue: Constants.BATTLE,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: args);

            channel.QueueBind(queue: Constants.TYPESEARCH,
                              exchange: Constants.TYPESEXCHANGE,
                              routingKey: Constants.TYPESEARCH);

            channel.QueueBind(queue: Constants.TYPESSEARCH,
                              exchange: Constants.TYPESEXCHANGE,
                              routingKey: Constants.TYPESSEARCH);

            channel.QueueBind(queue: Constants.LEGENDARYLIST,
                              exchange: Constants.LEGENDARYEXCHANGE,
                              routingKey: Constants.LEGENDARYLIST);

            channel.QueueBind(queue: Constants.NAMESEARCH,
                              exchange: Constants.BASEEXCHANGE,
                              routingKey: Constants.NAMESEARCH);

            channel.QueueBind(queue: Constants.HEADERSSEARCH,
                              exchange: Constants.HEADERSEXCHANGE,
                              routingKey: Constants.HEADERSSEARCH);

            channel.QueueBind(queue: Constants.BATTLE,
                              exchange: Constants.BATTLE,
                              routingKey: Constants.BATTLE);
        }

        public string TypeSearch(SearchModel m)
        {
            var messageBytes = m.AsBytes();
            channel.BasicPublish(
                exchange: Constants.TYPESEXCHANGE,
                routingKey: Constants.TYPESEARCH,
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public string TypesSearch(SearchModel m)
        {
            var messageBytes = m.AsBytes();
            channel.BasicPublish(
                exchange: Constants.TYPESEXCHANGE,
                routingKey: Constants.TYPESSEARCH,
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public string LegendaryList()
        {
            channel.BasicPublish(
                exchange: Constants.LEGENDARYEXCHANGE,
                routingKey: Constants.LEGENDARYLIST,
                basicProperties: props,
                body: null);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public string ParamSearch(SearchModel m)
        {
            var messageBytes = m.AsBytes();
            channel.BasicPublish(
                exchange: Constants.NAMESEARCH,
                routingKey: Constants.BASEEXCHANGE,
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public string HeadersList()
        {
            channel.BasicPublish(
                exchange: Constants.HEADERSEXCHANGE,
                routingKey: Constants.HEADERSLIST,
                basicProperties: props,
                body: null);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public string HeadersSearch(SearchModel m)
        {
            var messageBytes = m.AsBytes();
            channel.BasicPublish(
                exchange: Constants.HEADERSEXCHANGE,
                routingKey: Constants.HEADERSSEARCH,
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public string Battle(SearchModel m)
        {
            var messageBytes = m.AsBytes();
            channel.BasicPublish(
                exchange: Constants.BATTLEEXCHANGE,
                routingKey: Constants.BATTLE,
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
