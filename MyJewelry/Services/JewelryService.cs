using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MyJewelry.Interfaces;
using MyJewelry.Models;
using Microsoft.AspNetCore.SignalR;
 using MyJewelry.Hubs; 
using MyJewelry.Services;
using  MyJewelry.Models;
namespace MyJewelry.Services;

public class JewelryService : IJewelryService
{
    private readonly IHubContext<ActivityHub> hubContext;
    private readonly IJewelryRepository repository;
    private readonly IRabbitMqService rabbitMqService;
    private readonly IActiveUser _activeUserService;

    // תיקון 1: הוספתי את IRabbitMqService לסוגריים כדי שלא יהיה קריסה!
    public JewelryService(IJewelryRepository repository, IActiveUser activeUser, IHubContext<ActivityHub> hubContext, IRabbitMqService rabbitMqService)
    {
        this.repository = repository;
        this._activeUserService = activeUser;
        this.hubContext = hubContext;
        this.rabbitMqService = rabbitMqService; 
    }

    public List<Jewelry> Get()
    {
        var userId = _activeUserService.ActiveUser?.Id ?? 0;
        return repository.Get().Where(p => p.UserId == userId).ToList();
    }

    public Jewelry Get(int id)
    {
        var userId = _activeUserService.ActiveUser?.Id ?? 0;
        var jewelry = repository.Get(id);
        return jewelry?.UserId == userId ? jewelry : null;
    }

    public int Create(Jewelry jewelry)
    {
        jewelry.UserId = _activeUserService.ActiveUser?.Id ?? 0;
        int isCreate = repository.Create(jewelry);
        if (    isCreate > 0)
            BroadcastActivity("created", jewelry);
        return isCreate;
    }

    public bool Update(Jewelry jewelry)
    {
        var userId = _activeUserService.ActiveUser?.Id ?? 0;
        var existing = repository.Get(jewelry.Id);
        if (existing?.UserId != userId)
            return false;

        jewelry.UserId = userId;
        bool isUpdate = repository.Update(jewelry);
        if (isUpdate)
            QueueActivityBroadcast(jewelry);
        return isUpdate;
    }

    public bool Delete(int id)
    {
        var userId = _activeUserService.ActiveUser?.Id ?? 0;
        var jewelry = repository.Get(id);
        if (jewelry is null || jewelry.UserId != userId)
            return false;

        bool isDelete = repository.Delete(id);
        if (isDelete)
            BroadcastActivity("deleted", jewelry);
        return isDelete;
    }

    private void BroadcastActivity(string action, Jewelry jewelry)
    {
        // תיקון 2: שולפים את שם המשתמש בצורה תקינה
        var username = _activeUserService.ActiveUser?.Name ?? "Unknown";
        hubContext.Clients.All.SendAsync("ReceiveActivity", username, action, jewelry.Name);
    }

    private void QueueActivityBroadcast(Jewelry jewelry)
    {
        // תיקון 2: שולפים את המזהה ושם המשתמש בצורה תקינה
        var user = _activeUserService.ActiveUser;
        var message = new JewelryUpdatedMassage
        {
            UserId = user?.Id ?? 0,
            Username = user?.Name ?? "Unknown",
            JewelryName = jewelry.Name,
            Timestamp = DateTime.UtcNow
        };

        rabbitMqService.PublishJewelryUpdated(message).Wait();
    }

    public int Count => Get().Count;
}

public static class JewelryExtension
{
    public static void AddJewelryService(this IServiceCollection services)
    {
        services.AddSingleton<IJewelryRepository, JewelryRepository>();
        services.AddScoped<IJewelryService, JewelryService>();
    }
}


// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.Extensions.DependencyInjection;
// using MyJewelry.Interfaces;
// using MyJewelry.Models;
// using Microsoft.AspNetCore.SignalR;
// using MyJewelry.Hubs;

// namespace MyJewelry.Services;

// public class JewelryService : IJewelryService
// {
//     private readonly IHubContext<ActivityHub> hubContext;
//     private readonly IJewelryRepository repository;
//     private readonly IRabbitMqService rabbitMqService;

//     private readonly IActiveUser _activeUserService; // זה המשתנה שהיה חסר

//     public JewelryService(IJewelryRepository repository, IActiveUser activeUser,IHubContext<ActivityHub> hubContext)
//     {
//         this.repository = repository;
//         this._activeUserService = activeUser;
//         this.rabbitMqService = rabbitMqService;
//         this.hubContext = hubContext;
//     }

//     public List<Jewelry> Get()
//     {
//         // שליפה דינמית של ה-ID בכל קריאה
//         var userId = _activeUserService.ActiveUser?.Id ?? 0;
//         return repository.Get().Where(p => p.UserId == userId).ToList();
//     }

//     public Jewelry Get(int id)
//     {
//         var userId = _activeUserService.ActiveUser?.Id ?? 0;
//         var jewelry = repository.Get(id);
//         return jewelry?.UserId == userId ? jewelry : null;
//     }

//     public int Create(Jewelry jewelry)
//     {
//         jewelry.UserId = _activeUserService.ActiveUser?.Id ?? 0;
//         var isCreate= repository.Create(jewelry);
//         if(isCreate)
//             BroadcastActivity("created", jewelry);
//           return isCreate;
//     }

//     public bool Update(Jewelry jewelry)
//     {
//         var userId = _activeUserService.ActiveUser?.Id ?? 0;
//         var existing = repository.Get(jewelry.Id);
//         if (existing?.UserId != userId)
//             return false;

//         jewelry.UserId = userId;
//         var isUpdate= repository.Update(jewelry);
//         if(isUpdate)
//             QueueActivityBroadcast(jewelry);
//             return isUpdate;

//     }

//     public bool Delete(int id)
//     {
//         var userId = _activeUserService.ActiveUser?.Id ?? 0;
//         var jewelry = repository.Get(id);
//         if (jewelry is null || jewelry.UserId != userId)
//             return false;

//         var isDelete= repository.Delete(id);
//         if(isDelete)
//          BroadcastActivity("deleted", jewelry);
//          return isDelete;
//     }
//      private void BroadcastActivity(string action,  Jewelry jewelry)
//         {
//             hubContext.Clients.All.SendAsync("ReceiveActivity", activeUsername, action, jewelry.Name);
//         }
//          private void QueueActivityBroadcast(Jewelry jewelry)
//         {
//             var message = new JewelryUpdatedMassage
//             {
//                 UserId = activeUserId,
//                 Username = activeUsername,
//                 JewelryName = jewelry.Name,
//                 Timestamp = DateTime.UtcNow
//             };

//             rabbitMqService.PublishJewelryUpdated(message).Wait();
//         }


//     public int Count => Get().Count;
// }

// public static class JewelryExtension
// {
//     public static void AddJewelryService(this IServiceCollection services)
//     {
//         services.AddSingleton<IJewelryRepository, JewelryRepository>();
//         services.AddScoped<IJewelryService, JewelryService>();
//     }
// }
