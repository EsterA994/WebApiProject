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

    private readonly IJewelryRepository repository;
    private readonly int activeUserId;
    private readonly string activeUserName;


    public JewelryService(IJewelryRepository repository, IActiveUser activeUser)
    {
        this.repository = repository;
        var user = activeUser.ActiveUser;
        if (user is null)
            throw new System.InvalidOperationException("Active user is required");
        this.activeUserId = user.Id;
        this.activeUserName = user.Name;
    }


    public List<Jewelry> Get()
        => repository
            .Get()
            .Where(p => p.UserId == activeUserId)
            .ToList();

    public Jewelry Get(int id)
    {
        var jewelry = repository.Get(id);
        return jewelry?.UserId == activeUserId ? jewelry : null;
    }

    public int Create(Jewelry jewelry)
    {
        jewelry.UserId = activeUserId;
        return repository.Create(jewelry);
        // BroadcastActivity("added", jewelry);
    }


    public bool Update(Jewelry jewelry)
    {
        var existing = repository.Get(jewelry.Id);
        if (existing?.UserId != activeUserId)
            return false;

        jewelry.UserId = activeUserId;
        repository.Update(jewelry);
        // QueueActivityBroadcast(jewelry);
        return true;
    }

    public bool  Delete(int id)
    {
        var jewelry = Get(id);
        if (jewelry is null)
            return false;

        if (jewelry.UserId != activeUserId)
            return false;

        repository.Delete(id);
        return true;

    }

    public int Count => Get().Count;
}


public static class JewelryExtension
{
    public static void AddJewelryService(this IServiceCollection services)
    {
        services.AddSingleton<IJewelryRepository, JewelryRepository>();
        services.AddScoped<IJewelryService, JewelryService>();
    }
}