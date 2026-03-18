using System;
using System.IO; // נוסף רק זה בשביל הכתיבה לקובץ
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MyJewelry.Hubs;
using MyJewelry.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyJewelry.Services;

public class JewelryUpdateWorker : BackgroundService
{
    private readonly IHubContext<ActivityHub> hubContext;
    private IConnection connection;
    private IChannel channel;
    private const string QueueName = "jewelry-updates";

    public JewelryUpdateWorker(IHubContext<ActivityHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<JewelryUpdatedMessage>(json);

            // --- כאן הכתיבה ללוג שביקשת ---
            string logEntry = $"[{DateTime.Now}] User {message.Username} updated jewelry: {message.JewelryName}{Environment.NewLine}";
            Console.WriteLine($">>>>> RECEIVED MESSAGE FOR: {message.JewelryName} <<<<<");
            await File.AppendAllTextAsync("Logs.txt", logEntry);
            // ------------------------------

            // HEAVY OPERATIONS HAPPEN HERE
            Thread.Sleep(5000);

            // Broadcast to SignalR after heavy work completes
            await hubContext.Clients.All.SendAsync(
                "ReceiveActivity",
                message.Username,
                "updated",
                message.JewelryName,
                stoppingToken
            );

            // Acknowledge message
            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );

        // Keep the worker running
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // if (channel != null)
        await channel.CloseAsync();
        // if (connection != null)
        await connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}


// using System;
// using System.Text;
// using System.Text.Json;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.SignalR;
// using Microsoft.Extensions.Hosting;
// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;

// namespace MyJewelry.Services
// {
//     public class JewelryUpdateWorker : BackgroundService
//     {
//         private readonly IHubContext<ActivityHub> hubContext;
//         private IConnection connection;
//         private IChannel channel;
//         private const string QueueName = "jewelry-updates";

//         public JewelryUpdateWorker(IHubContext<ActivityHub> hubContext)
//         {
//             this.hubContext = hubContext;
//         }

//         protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//         {
//             var factory = new ConnectionFactory() { HostName = "localhost" };
//             connection = await factory.CreateConnectionAsync();
//             channel = await connection.CreateChannelAsync();

//             await channel.QueueDeclareAsync(
//                 queue: QueueName,
//                 durable: true,
//                 exclusive: false,
//                 autoDelete: false,
//                 arguments: null
//             );

//             var consumer = new AsyncEventingBasicConsumer(channel);
//             consumer.ReceivedAsync += async (model, ea) =>
//             {
//                 var body = ea.Body.ToArray();
//                 var json = Encoding.UTF8.GetString(body);
//                 var message = JsonSerializer.Deserialize<JewelryUpdatedMessage>(json);

// // כתיבה לקובץ לוג - זה החלק הכי חשוב במטלה!
//                 string logEntry =
//                     $"[{DateTime.Now}] User {message.Username} updated jewelry: {message.JewelryName}{Environment.NewLine}";
//                 await File.AppendAllTextAsync("Logs.txt", logEntry);

// // שימוש ב-SignalR כדי לעדכן את המסך בזמן אמת
//                 await hubContext.Clients.All.SendAsync(
//                     "ReceiveActivity",
//                     message.Username,
//                     "updated",
//                     message.JewelryName

//                 var body = ea.Body.ToArray();
//                 var json = Encoding.UTF8.GetString(body);
//                 var message = JsonSerializer.Deserialize<JewelryUpdatedMessage>(json);

//                 // HEAVY OPERATIONS HAPPEN HERE (not in HTTP request thread!)
//                 Thread.Sleep(5000);  // Simulate invoice generation, analytics, etc.
//                 //await Task.Delay(5000);

//                 // Broadcast to SignalR after heavy work completes
//                 await hubContext.Clients.All.SendAsync(
//                     "ReceiveActivity",
//                     message.Username,
//                     "updated",
//                     message.JewelryName,
//                     stoppingToken);

//                 // Acknowledge message
//                 await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
//             };

//             await channel.BasicConsumeAsync(
//                 queue: QueueName,
//                 autoAck: false,  // Manual acknowledgment for reliability
//                 consumer: consumer);

//             // Keep the worker running
//             await Task.Delay(Timeout.Infinite, stoppingToken);
//         }

//         public override async Task StopAsync(CancellationToken cancellationToken)
//         {
//             await channel?.CloseAsync();
//             await connection?.CloseAsync();
//             await base.StopAsync(cancellationToken);
//         }
//     }
// }
