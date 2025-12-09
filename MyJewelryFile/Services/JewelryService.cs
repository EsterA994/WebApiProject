using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MyJewelry.Models;
using MyJewelry.Interfaces;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace MyJewelry.Services;

public class JewelryService : IJewelryService
{
    
    private List<Jewelry> Jewelries { get; }
    private string filePath;

    public JewelryService(IWebHostEnvironment webHost)
    {
        // list = new List<Jewelry>
        // {
        //     new Jewelry { Id = 1, Name = "pearl necklace", Type = "Pearl", Category = "chain", Price = 5000},
        //     new Jewelry { Id = 2, Name = "earrings", Type = "Gold", Category = "earrings", Price = 1000},
        //     new Jewelry { Id = 3, Name = "bracelet", Type = "Silver", Category = "bracelet", Price = 1200},
        //     new Jewelry { Id = 4, Name = "dimond ring", Type = "Rhodium", Category = "ring", Price = 6500}
        // };
        
        this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "Jewelry.json");
            using (var jsonFile = File.OpenText(filePath))
            {
                var content = jsonFile.ReadToEnd();
                Jewelries = JsonSerializer.Deserialize<List<Jewelry>>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
    }

    private void saveToFile()
    {
        var text = JsonSerializer.Serialize(Jewelries);
        File.WriteAllText(filePath, text);
    }


    public List<Jewelry> Get()
    {
        return Jewelries;
    }

    public Jewelry find(int id)
    {
        return Jewelries.FirstOrDefault(p => p.Id == id);

    }

    public Jewelry Get(int id) => find(id);


    public Jewelry Create(Jewelry newJewelry)
    {
        var maxId = Jewelries.Max(p => p.Id);
        newJewelry.Id = maxId + 1;
        Jewelries.Add(newJewelry);
        saveToFile();
        return newJewelry;
    }


    public bool Update(int id, Jewelry newJewelry)
    {
        var jewelry = find(id);
        if (jewelry == null)
            return false;
        if (jewelry.Id != newJewelry.Id)
            return false;

        var index = Jewelries.IndexOf(jewelry);
        Jewelries[index] = newJewelry;
        saveToFile();
        return true;
    }

    public bool Delete(int id)
    {
        var jewelry = find(id);
        if (jewelry == null)
            return false;
        Jewelries.Remove(jewelry);
        saveToFile();
        return true;
    }


}

    
public static class JewelryServiceExtension
{
    public static void addJewelryService(this IServiceCollection services)
    {
        services.AddSingleton<IJewelryService, JewelryService>();
    }
}