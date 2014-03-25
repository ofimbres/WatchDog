using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WatchDog.WpfApp;
using WatchDog.WpfApp.Models;

namespace MvcApplication3.Hubs
{
    public class ChatHub : Hub
    {
        public void DoWork()
        {
            ServiceBus
                .Setup(ServiceBusUtilities.GetServiceBusCredentials())
                .Subscribe<ImageStreamMessage>(this.OnImageReceived);
        }

        void OnImageReceived(ImageStreamMessage message)
        {
            Clients.All.broadcastMessage(message.ImageData);
        }

        //public void Send(string name, string message)
        //{
        //    // Call the broadcastMessage method to update clients.
        //    Clients.All.broadcastMessage(name, message);
        //}
    }
}