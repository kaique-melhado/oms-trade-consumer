using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace OmsTradeConsumer.Messaging.Configurations;

public class QueueConfiguration
{
    public ConnectionFactory Factory { get; set; }

    public QueueConfiguration(IConfiguration configuration)
    {
        Factory = new ConnectionFactory
        {
            HostName = configuration["QueueConnection:Hostname"],
            UserName = configuration["QueueConnection:Username"],
            Password = configuration["QueueConnection:Password"],
            Port = int.Parse(configuration["QueueConnection:Port"] ?? "5672")
        };
    }
}