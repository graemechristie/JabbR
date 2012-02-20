using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabbR.Services;
using JabbR.Models;
using Ninject;
using System.ComponentModel.Composition;

namespace JabbR.Commands
{
    [Export(typeof(ICommand))]
    [CommandMetadata(Name = "rooms", Usage = "Type /rooms to show the list of rooms", Weight=8.0f)]
    public class RoomsCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        [ImportingConstructor]
        public RoomsCommand(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            _notificationService.ShowRooms();
        }
    }
}

