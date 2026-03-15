
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MyJewelry.Interfaces;
using MyJewelry.Models;

namespace MyJewelry.Services;

public class JewelryService : IJewelryService
{
    private readonly IJewelryRepository repository;
    private readonly IActiveUser _activeUserService; // זה המשתנה שהיה חסר

    public JewelryService(IJewelryRepository repository, IActiveUser activeUser)
    {
        this.repository = repository;
        this._activeUserService = activeUser;
    }

    public List<Jewelry> Get()
    {
        // שליפה דינמית של ה-ID בכל קריאה
        var userId = _activeUserService.ActiveUser?.Id ?? 0;
        return repository.Get().Where(p => p.UserId == userId).ToList();
    }

    public Jewelry? Get(int id)
    {
        var userId = _activeUserService.ActiveUser?.Id ?? 0;
        var jewelry = repository.Get(id);
        return jewelry?.UserId == userId ? jewelry : null;
    }

    public int Create(Jewelry jewelry)
    {
        jewelry.UserId = _activeUserService.ActiveUser?.Id ?? 0;
        return repository.Create(jewelry);
    }

    public bool Update(Jewelry jewelry)
    {
        var userId = _activeUserService.ActiveUser?.Id ?? 0;
        var existing = repository.Get(jewelry.Id);
        if (existing?.UserId != userId)
            return false;

        jewelry.UserId = userId;
        return repository.Update(jewelry);
    }

    public bool Delete(int id)
    {
        var userId = _activeUserService.ActiveUser?.Id ?? 0;
        var jewelry = repository.Get(id);
        if (jewelry is null || jewelry.UserId != userId)
            return false;

        return repository.Delete(id);
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
