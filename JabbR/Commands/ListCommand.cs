using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabbR.Services;
using JabbR.Models;
using Ninject;

namespace JabbR.Commands
{
    [CommandInfo(Name = "list", Usage = "Type /list (room) to show a list of users in the room", Weight = 11.0f)]
    public class ListCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        public ListCommand(INotificationService notificationService, IJabbrRepository repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            if (parts.Length < 2)
            {
                throw new InvalidOperationException("List users in which room?");
            }

            roomName = parts[1];
            ChatRoom room = _repository.VerifyRoom(roomName);

            var names = room.Users.Online().Select(s => s.Name);

            _notificationService.ListUsers(room, names);
        }
    }
}

