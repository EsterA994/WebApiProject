using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyJewelry.Interfaces;
using MyJewelry.Models;

namespace MyJewelry.Services;

public class ActiveUserService : IActiveUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActiveUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // שימוש ב-Getter מבטיח שהשליפה תקרה רק אחרי שה-Middleware סיים
    public User ActiveUser => GetActiveUser();

    private User GetActiveUser()
    {
        var userClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("Id");
        if (userClaim == null) return null;

        return new User
        {
            Id = int.Parse(userClaim.Value),
            Name = _httpContextAccessor.HttpContext.User.FindFirst("Name")?.Value ?? ""
        };
    }
}



public static partial class MyJewelryExtensions
{
    public static IServiceCollection AddActiveUser(this IServiceCollection services)
    {
        services.AddScoped<IActiveUser, ActiveUserService>();
        return services;
    }
}
// // }
