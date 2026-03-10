using MyJewelry.Interfaces;
using MyJewelry.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace MyJewelry.Services
{
    public class ActiveUserService : IActiveUser
    {
        public User ActiveUser { get; private set; }
        public ActiveUserService(IHttpContextAccessor context)
        {
            var userId = context?.HttpContext?.User?.FindFirst("Id");
            if (userId != null)
            {
                ActiveUser = new User
                {
                    Id = int.Parse(userId.Value),
                    Username = "test"
                };
            }
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
}