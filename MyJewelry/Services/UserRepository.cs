using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MyJewelry.Models;
using MyJewelry.Interfaces;


namespace MyJewelry.Services;

public class UserRepository : IUserRepository
{
    
    private  List<User> list;
        private readonly string filePath;
        private readonly string filePath;


    public UserRepository()
    {
        filePath=filePath.Combine(Directory.GetCurrentDirectory(), "Data", "users.json");
            using var jsonFile = File.OpenText(filePath);
            list=JsonSerializer.Deserialize<List<User>>(jsonFile.ReadToEnd(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            }) ?? new List<User>();
        // {
        //     // new User { Id = 1, Name = "Sara Cohen", Age = 18, Gender = "male"},
        //     // new User { Id = 2, Name = "Tamer Levin", Age = 18, Gender = "male"},
        //     // new User { Id = 4, Name = "Dan Glik", Age = 18, Gender = "female"},
        //     // new User { Id = 3, Name = "Avigail Beno", Age = 18, Gender = "male"}
        // };

    }

    private void Save() => File.WriteAllText(filePath, JsonSerializer.Serialize(list));

    public List<User> Get()
    {
        return list;
    }

    private User find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);

    }

    public User Get(int id) => find(id);


    public User Create(User newUser)
    {
        var maxId = list.Max(p => p.Id);
        newUser.Id = maxId + 1;
        list.Add(newUser);
      Save();
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
        Save();

        return true;
    }

    public bool Delete(int id)
    {
        var user = find(id);
        if (user == null)
            return false;
        list.Remove(user);
        Save();

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