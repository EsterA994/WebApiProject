using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MyJewelry.Models;
using MyJewelry.Interfaces;
using Microsoft.Extensions.DependencyInjection;//


namespace MyJewelry.Services;

public class JewelryService : IJewelryService
{
    
    private readonly IJewelaryRepository repository;
    private readonly int activeUserId;
    private readonly string activeUserName;


    public JewelryService(IJewelaryRepository repository, IActiveUser activeUser)
    {
        this.repository = repository;
        var user = activeUser.ActiveUser;
        if(user is null)
        throw new System.InvalidOperationException("Active user is required");
        this.activeUserId = user.Id;
        this.activeUserName = user.Name;
    }


    public List<Jewelry> Get()
        => repository
            .GetAll()
            .Where(p => p.UserId == activeUserId)
            .ToList();

    public Jewelry find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);

    }

    public Jewelry Get(int id){ 
        var jewelry = repository.Get(id);
        return jewelry?.UserId == activeUserId ? jewelry : null;
    }

    public Jewelry Create(Jewelry jewelry)
    {
        jewelry.UserId = activeUserId;
        repository.Add(jewelry);
        BroadcastActivity("added", jewelry);
    }


    public bool Update(int id, Jewelry jewelry)
    {
        var existing = repository.Get(jewelry.Id);
            if (existing?.UserId != activeUserId)
                return false;

            jewelry.UserId = activeUserId;
            repository.Update(jewelry);
        // QueueActivityBroadcast(jewelry);
        return true;
    }

    public bool Delete(int id)
    {
        var jewelry = Get(id);
        if (jewelry is null)
            return false;

        if (jewelry.UserId != activeUserId)
                return false;

        repository.Delete(id);
        return true;
    }

    public int Count => GetAll().Count;
}

    
public static class JewelryExtension
{
    public static void addJewelryService(this IServiceCollection services)
    {
        services.AddSingleton<IJewelryRepository, JewelryRepository>();
        services.AddScoped<IJewelryService, JewelryService>();
    }
}