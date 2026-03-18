using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MyJewelry.Hubs;
using MyJewelry.Interfaces;
using MyJewelry.Models;
namespace MyJewelry.Services;

public class UserService(
    IUserRepository repository,
    IActiveUser activeUser,
    IHubContext<ActivityHub> hubContext
    ) : IUserService
{
   private readonly IHubContext<ActivityHub> hubContext = hubContext;
    private readonly IUserRepository repository = repository;
    private readonly IActiveUser activeUser = activeUser;

    public List<User> Get()
    {
        return repository.Get();
    }

    public User Get(int id)
    {
        if (this.activeUser.ActiveUser?.Id == id)
            return repository.Get(id);
        return null;
    }
    public int Create(User newUser)
    {
        //  if(this.activeUserId!=role.Id||this.activeUsweName!=role.Name)
        //           throw new Exeption("no authorization")
        int isCreate = repository.Create(newUser);
        // if (isCreate>0)
            //  BroadcastActivity("created", newUser);
            return isCreate;
    }

    public bool Update(User newUser)
    {
        // if (this.activeUserId != role.Id || this.activeUsweName != role.Name)
        //     throw new Exeption("no authorization")
        var existing = repository.Get(newUser.Id);
        if (existing == null)
            return false;
        bool isUpdate = repository.Update(newUser);
        // if (isUpdate)
        //     QueueActivityBroadcast("updated", newUser);
        return isUpdate;
    }

    public bool Delete(int id)
    {
        // if (this.activeUserId != role.Id || this.activeUsweName != role.Name)
        //     throw new Exception("no authorization")
        var user = repository.Get(id);
        if (user is null)
            return false;
        // if (user.Id != activeUserId&&activeUserName!="Esty"&&activeUserId!=1272)
        //     return false;
        bool flag = repository.Delete(id);
        // if (flag)
        //     BroadcastActivity("deleted", user);
        return flag;
    }

    // private void BroadcastActivity(string action, User user)
    // {
    //     // תיקון 2: שולפים את שם המשתמש בצורה תקינה
    //     var username = activeUser.ActiveUser?.Name ?? "Unknown";
    //     hubContext.Clients.All.SendAsync("ReceiveActivity", username, action, User.Name);
    // }

    // private void QueueActivityBroadcast(User user)
    // {
    //     // תיקון 2: שולפים את המזהה ושם המשתמש בצורה תקינה
    //     var userActive = activeUser.ActiveUser;
    //     var message = new JewelryUpdatedMessage
    //     {
    //         UserId = user?.Id ?? 0,
    //         userActive = user?.Name ?? "Unknown",
    //         username = user.Name,
    //         Timestamp = DateTime.UtcNow,
    //     };

    //     rabbitMqService.PublishJewelryUpdated(message).Wait();
    // }


    public int Count => Get().Count;
}

public static partial class UserServiceExtension
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
    }
}
