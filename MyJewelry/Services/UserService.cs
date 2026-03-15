using System;
using System.Collections.Generic;
using System.Linq;
using MyJewelry.Interfaces;
using MyJewelry.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace MyJewelry.Services;

public class UserService : IUserService
{
    private readonly IUserRepository repository;
    private readonly int activeUserId;
    private readonly string activeUserName;


    public UserService(IUserRepository repository, IActiveUser activeUser)
    {
        this.repository = repository;
        var user = activeUser.ActiveUser;

        // במקום לזרוק שגיאה כאן, פשוט נשמור את הנתונים אם הם קיימים
        if (user != null)
        {
            this.activeUserId = user.Id;
            this.activeUserName = user.Name;
        }
        else
        {
            // ערכי ברירת מחדל למקרה שאין משתמש מחובר (למשל בלוגין)
            this.activeUserId = 0;
            this.activeUserName = "Unknown";
        }
    }


    public List<User> Get() => repository.Get();

    public User Get(int id) => repository.Get(id);


    public int Create(User newUser)
    {
        // newUser.Id = activeUserId;///
        return repository.Create(newUser);
        // BroadcastActivity("added", newUser);
    }


    public bool Update(User newUser)
    {
        var existing = repository.Get(newUser.Id);
        if (existing == null)
            return false;
        return repository.Update(newUser);
        // QueueActivityBroadcast("updated", newUser);

    }

    public bool Delete(int id)
    {
        var user = Get(id);
        if (user is null)
            return false;
        // if (user.Id != activeUserId&&activeUserName!="Esty"&&activeUserId!=1272)
        //     return false;
        return repository.Delete(id);
        // BroadcastActivity("deleted", user);
    }

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