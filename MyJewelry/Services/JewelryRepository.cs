using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MyJewelry.Interfaces;
using MyJewelry.Models;
using Microsoft.AspNetCore.Mvc;
// using System.Threading.Tasks;

namespace MyJewelry.Services;

public class JewelryRepository : IJewelryRepository
{
    
    private readonly List<Jewelry> jewelries;
    private readonly string filePath;

    public JewelryRepository(IWebHostEnvironment webHost)
    {
        filePath = Path.Combine(webHost.ContentRootPath, "Data", "jewelries.json");
        using var jsonFile = File.OpenText(filePath);
        jewelries = JsonSerializer.Deserialize<List<Jewelry>>(jsonFile.ReadToEnd(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Jewelry>();
    }
    
    private void SaveChanges()
       => File.WriteAllText(filePath, JsonSerializer.Serialize(jewelries));

    public List<Jewelry> Get()=>jewelries;


    public Jewelry Get(int id) => jewelries.FirstOrDefault(p => p.Id == id);

    public int Create(Jewelry newJewelry)
    {
        newJewelry.Id = jewelries.Count == 0 ? 1 : jewelries.Max(p => p.Id) + 1;
        jewelries.Add(newJewelry);
        SaveChanges();
        return newJewelry.Id;
    }


    public bool Update(Jewelry newJewelry)
    {
        var index = jewelries.FindIndex(p => p.Id == newJewelry.Id);
        if (index == -1)
            return false;

        jewelries[index] = newJewelry;
        SaveChanges();
        return true;     
    }

    public bool Delete(int id)
    {
        var jewelry = Get(id);
        if (jewelry is null)
            return false;

        jewelries.Remove(jewelry);
        SaveChanges();
        return true;
    }

    public int Count => jewelries.Count;
}
