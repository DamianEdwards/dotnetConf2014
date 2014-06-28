using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Server
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        public override Task OnConnected()
        {
            Clients.All.userJoined(Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : "anonymous");

            return Task.FromResult(0);
        }

        public void Send(string message)
        {
            Clients.All.newMessage(message);
        }
    }
}