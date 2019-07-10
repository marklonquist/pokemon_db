using Microsoft.Extensions.Logging;
using Pokemon.ServiceOne.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokemon.ServiceOne.Interfaces
{
    public interface IRabbitMQHandler
    {
        string TypeSearch(SearchModel m);
        string TypesSearch(SearchModel m);
        string LegendaryList();
        string ParamSearch(SearchModel m);
        string HeadersList();
        string HeadersSearch(SearchModel m);
        string Battle(SearchModel m);

        void Close();
    }
}
