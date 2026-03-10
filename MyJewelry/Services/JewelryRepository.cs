using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MyJewelry.Models;
using MyJewelry.Interfaces;


namespace MyJewelry.Services;

public class JewelryRepository : IJewelryService
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

    public Jewelry find(int id)
    {
        return jewelries.FirstOrDefault(p => p.Id == id);

    }

    public Jewelry Get(int id) => find(id);


    public Jewelry Create(Jewelry newJewelry)
    {
        newJewelry.Id = list.Max(p => p.Id)+1;
        jewelries.Add(newJewelry);
        SaveChanges();
        return newJewelry;
    }


    public bool Update(int id, Jewelry newJewelry)
    {
        var index = jewelries.FindIndex(p => p.Id == id);
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

}
