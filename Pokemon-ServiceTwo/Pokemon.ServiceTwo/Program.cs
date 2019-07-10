using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Pokemon.DAL;
using System.Linq;
using Newtonsoft.Json;
using Pokemon.Models;
using System.Collections.Generic;
using Pokemon.DAL.Models;
using Pokemon.ServiceTwo.Battle;

class Program
{
    private static DummyDAL dal;
    private static BattleHandler battleHandler;

    public static void Main()
    {
        dal = new DummyDAL();
        battleHandler = new BattleHandler();

        Console.WriteLine("Consumer starting");

        // DO STUFF
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            RequestedHeartbeat = 60
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            var consumer = new EventingBasicConsumer(channel);

            SetupQueues(channel, consumer);

            consumer.Received += (model, ea) =>
            {
                var response = string.Empty;

                var body = ea.Body;
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {

                    response = Handle(ea);
                }
                catch (Exception e)
                {
                    Console.WriteLine(" [.] " + e.Message);
                    response = "";
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };



            Console.ReadLine();
        }
    }

    private static void SetupQueues(IModel channel, EventingBasicConsumer consumer)
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

        channel.QueueDeclare(queue: Constants.HEADERSLIST,
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

        channel.QueueBind(queue: Constants.HEADERSLIST,
                          exchange: Constants.HEADERSEXCHANGE,
                          routingKey: Constants.HEADERSLIST);

        channel.QueueBind(queue: Constants.HEADERSSEARCH,
                          exchange: Constants.HEADERSEXCHANGE,
                          routingKey: Constants.HEADERSSEARCH);

        channel.QueueBind(queue: Constants.BATTLE,
                          exchange: Constants.BATTLEEXCHANGE,
                          routingKey: Constants.BATTLE);

        channel.BasicConsume(queue: Constants.TYPESEARCH,
                            autoAck: false,
                            consumer: consumer);

        channel.BasicConsume(queue: Constants.TYPESSEARCH,
                             autoAck: false,
                             consumer: consumer);

        channel.BasicConsume(queue: Constants.LEGENDARYLIST,
                             autoAck: false,
                             consumer: consumer);

        channel.BasicConsume(queue: Constants.NAMESEARCH,
                             autoAck: false,
                             consumer: consumer);

        channel.BasicConsume(queue: Constants.HEADERSLIST,
                             autoAck: false,
                             consumer: consumer);

        channel.BasicConsume(queue: Constants.HEADERSSEARCH,
                             autoAck: false,
                             consumer: consumer);

        channel.BasicConsume(queue: Constants.BATTLE,
                             autoAck: false,
                             consumer: consumer);
    }

    private static string Handle(BasicDeliverEventArgs ea)
    {
        var message = Encoding.UTF8.GetString(ea.Body);
        var model = JsonConvert.DeserializeObject<SearchModel>(message);

        switch (ea.RoutingKey)
        {
            case Constants.TYPESEARCH:
                return JsonConvert.SerializeObject(dal.GetByType(model.Type1));
            case Constants.TYPESSEARCH:
                return JsonConvert.SerializeObject(dal.GetByTypes(model.Type1, model.Type2));
            case Constants.LEGENDARYLIST:
                return JsonConvert.SerializeObject(dal.GetAllLegendaries());
            case Constants.NAMESEARCH:
                return JsonConvert.SerializeObject(dal.GetAllByParam(model.Param));
            case Constants.HEADERSLIST:
                return JsonConvert.SerializeObject(dal.GetAllHeaders());
            case Constants.HEADERSSEARCH:
                return JsonConvert.SerializeObject(dal.GetByHeaderSearch(model.HeaderParams));
            case Constants.BATTLE:
                var (pokemonA, pokemonB) = dal.GetBattlePokemons(model.PokemonIDA, model.PokemonIDB);
                return battleHandler.HandleBattle(pokemonA, pokemonB);
            default:
                return string.Empty;
        }
    }

    
}