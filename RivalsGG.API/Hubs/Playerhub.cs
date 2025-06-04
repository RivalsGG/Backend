using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace RivalsGG.API.Hubs
{
    public class Playerhub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "PlayerUpdates");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "PlayerUpdates");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
