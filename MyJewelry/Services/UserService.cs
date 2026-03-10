using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MyJewelry.Models;
using MyJewelry.Interfaces;


namespace MyJewelry.Services;

public class UserService : IUserService
{
     private readonly IUserRepository repository;
    private List<User> list;
    private readonly int activeUserId;
    private readonly string activeUsername;


    public UserService(IUserRepository repository, IActiveUser activeUser)
    {
        this.repository = repository;
        var user = activeUser.ActiveUser;
       if (user is null)
         throw new System.InvalidOperationException("Active user is required");
        this.activeUserId = user.Id;
        this.activeUsername = user.Username;
    
        // list = new List<User>
        // {
        //     new User { Id = 1, Name = "Sara Cohen", Age = 18, Gender = "male"},
        //     new User { Id = 2, Name = "Tamer Levin", Age = 18, Gender = "male"},
        //     new User { Id = 4, Name = "Dan Glik", Age = 18, Gender = "female"},
        //     new User { Id = 3, Name = "Avigail Beno", Age = 18, Gender = "male"}
        // };
    }


    public List<User> Get()
    {
         => repository
                .Get()
                .Where(p => p.Id == activeUserId)
                .ToList();
    }

    public User find(int id)
    {
        return repository.find(id);

    }

    public User Get(int id) => find(id);


    public User Create(User newUser)
    {
       newUser.Id = activeUserId;
            repository.Create(newUser);
            BroadcastActivity("added", newUser);
    }


    public bool Update(int id, User newUser)
    {
        var existing = repository.Get(id);
        if (existing == null)
            return false;
            repository.Update(id, newUser);
            QueueActivityBroadcast("updated", newUser);
            return true;
    }

    public bool Delete(int id)
    {
        var user = Get(id);
        if (user is null)
            return false;
        if (user.UserId != activeUserId)
                return salse;
        repository.Delete(id);
        BroadcastActivity("deleted", user);

        return true;
    }
        public int Count => Get().Count;


}

    
public static class UserServiceExtension
{
    public static IServiceCollection addUserService(this IServiceCollection services)
    {
        services.AddSingleton<IUserService, UserRepository>();
       services.AddScoped<IUserService, UserService>();

        return services;
    }
}