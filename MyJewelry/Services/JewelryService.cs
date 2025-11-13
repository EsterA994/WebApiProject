using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MyJewelry.Models;
using MyJewelry.Interfaces;


namespace MyJewelry.Services;

public class JewelryService : IJewelryService
{
    
    private List<Jewelry> list;


    public JewelryService()
    {
        list = new List<Jewelry>
        {
            new Jewelry { Id = 1, Name = "pearl necklace", Type = "Pearl", Category = "chain", Price = 5000},
            new Jewelry { Id = 2, Name = "earrings", Type = "Gold", Category = "earrings", Price = 1000},
            new Jewelry { Id = 3, Name = "bracelet", Type = "Silver", Category = "bracelet", Price = 1200},
            new Jewelry { Id = 3, Name = "dimond ring", Type = "Rhodium", Category = "ring", Price = 6500}
        };
    }


    public List<Jewelry> Get()
    {
        return list;
    }

    public Jewelry find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);

    }

    public Jewelry Get(int id) => find(id);


    public Jewelry Create(Jewelry newJewelry)
    {
        var maxId = list.Max(p => p.Id);
        newJewelry.Id = maxId + 1;
        list.Add(newJewelry);
        return newJewelry;
    }


    public bool Update(int id, Jewelry newJewelry)
    {
        var jewelry = find(id);
        if (jewelry == null)
            return false;
        if (jewelry.Id != newJewelry.Id)
            return false;

        var index = list.IndexOf(jewelry);
        list[index] = newJewelry;

        return true;
    }

    public bool Delete(int id)
    {
        var jewelry = find(id);
        if (jewelry == null)
            return false;
        list.Remove(jewelry);
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