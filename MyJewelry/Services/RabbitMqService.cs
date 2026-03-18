// using System;
// using System.Text;
// using System.Text.Json;
// using System.Threading.Tasks;
// using Microsoft.Extensions.DependencyInjection;
// using MyJewelry.Models;
// using RabbitMQ.Client;

// namespace MyJewelry.Services
// {
//     public interface IRabbitMqService
//     {
//         Task PublishJewelryUpdated(JewelryUpdatedMessage message);
//     }

//     public class RabbitMqService : IRabbitMqService
//     {
//         private const string QueueName = "jewelry-updates";
//         private readonly ConnectionFactory _factory;

//         public RabbitMqService()
//         {
//             _factory = new ConnectionFactory() { HostName = "localhost" };
//         }

//         public async Task PublishJewelryUpdated(JewelryUpdatedMessage message)
//         {
//             // בגרסה 7+ פותחים חיבור וערוץ בצורה אסינכרונית בכל שליחה או שומרים אותם
//             using var connection = await _factory.CreateConnectionAsync();
//             using var channel = await connection.CreateChannelAsync();

//             await channel.QueueDeclareAsync(
//                 queue: QueueName,
//                 durable: true,
//                 exclusive: false,
//                 autoDelete: false,
//                 arguments: null
//             );

//             var json = JsonSerializer.Serialize(message);
//             var body = Encoding.UTF8.GetBytes(json);

//             await channel.BasicPublishAsync(exchange: string.Empty, routingKey: QueueName, body: body);
//         }
//     }

//     public static partial class MyJewelryExtensions
//     {
//         public static IServiceCollection AddRabbitMq(this IServiceCollection services)
//         {
//             services.AddSingleton<IRabbitMqService, RabbitMqService>();
//             return services;
//         }
//     }
// }

using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyJewelry.Models;
using RabbitMQ.Client;

namespace MyJewelry.Services;

public interface IRabbitMqService
{
    Task PublishJewelryUpdated(JewelryUpdatedMessage message);
}

public class RabbitMqService : IRabbitMqService, IDisposable
{
    private IConnection connection;
    private IChannel channel;
    private const string QueueName = "jewelry-updates";

    public RabbitMqService()
    {
        InitializeAsync().GetAwaiter().GetResult();
    }

    private async System.Threading.Tasks.Task InitializeAsync()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();

        // Declare queue (idempotent - creates if doesn't exist)
        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true, // Survives broker restart
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public async Task PublishJewelryUpdated(JewelryUpdatedMessage message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: QueueName,
            body: body);
    }

    public void Dispose()
    {
        channel?.CloseAsync().Wait();
        connection?.CloseAsync().Wait();
    }
}

public static partial class MyJewelryExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMqService, RabbitMqService>();
        services.AddHostedService<JewelryUpdateWorker>();
        return services;
    }
}

