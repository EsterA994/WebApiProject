using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MyJewelry.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    public async Task BroadcastActivity(string username, string action, string jewelryName)
    {
        await Clients.All.SendAsync("ReceiveActivity", username, action, jewelryName);
    }
}
