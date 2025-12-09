using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MyJewelry.Models;
using MyJewelry.Interfaces;


namespace MyJewelry.Services;

public class UserService : IUserService
{
    
    private List<User> list;


    public UserService()
    {
        list = new List<User>
        {
            new User { Id = 1, Name = "Sara Cohen", Age = 18, Gender = "male"},
            new User { Id = 2, Name = "Tamer Levin", Age = 18, Gender = "male"},
            new User { Id = 4, Name = "Dan Glik", Age = 18, Gender = "female"},
            new User { Id = 3, Name = "Avigail Beno", Age = 18, Gender = "male"}
        };
    }


    public List<User> Get()
    {
        return list;
    }

    public User find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);

    }

    public User Get(int id) => find(id);


    public User Create(User newUser)
    {
        var maxId = list.Max(p => p.Id);
        newUser.Id = maxId + 1;
        list.Add(newUser);
        return newUser;
    }


    public bool Update(int id, User newUser)
    {
        var user = find(id);
        if (user == null)
            return false;
        if (user.Id != newUser.Id)
            return false;

        var index = list.IndexOf(user);
        list[index] = newUser;

        return true;
    }

    public bool Delete(int id)
    {
        var user = find(id);
        if (user == null)
            return false;
        list.Remove(user);
        return true;
    }


}

    
public static class UserServiceExtension
{
    public static void addUserService(this IServiceCollection services)
    {
        services.AddSingleton<IUserService, UserService>();
    }
}