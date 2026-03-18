using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyJewelry.Interfaces;
using MyJewelry.Models;

namespace MyJewelry.Services;

public class UserRepository : IUserRepository
{
    private List<User> list;
    private readonly string filePath;

    public UserRepository(IWebHostEnvironment webHost)
    {
        filePath = Path.Combine(webHost.ContentRootPath, "Data", "users.json");
        using var jsonFile = File.OpenText(filePath);
        list =
            JsonSerializer.Deserialize<List<User>>(
                jsonFile.ReadToEnd(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<User>();
    }

    private void Save() => File.WriteAllText(filePath, JsonSerializer.Serialize(list));

    public List<User> Get()
    {
        // מחזיר את הרשימה, ואם היא ריקה - מחזיר רשימה חדשה במקום null
        return list ?? new List<User>();
    }

    public User Get(int id) => list.FirstOrDefault(p => p.Id == id);

    public int Create(User newUser)
    {
        newUser.Id = list.Count == 0 ? 1 : list.Max(p => p.Id) + 1;
        list.Add(newUser);
        Save();
        return newUser.Id;
    }

    public bool Update(User newUser)
    {
        var user = Get(newUser.Id);
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
        var user = Get(id);
        if (user == null)
            return false;
        list.Remove(user);
        Save();
        return true;
    }
}
