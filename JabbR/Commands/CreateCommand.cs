using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabbR.Services;
using JabbR.Models;
using Ninject;

namespace JabbR.Commands
{
    [CommandInfo(Name = "create", Usage = "Type /create [room] to create a room", Weight = 4.0f)]
    public class CreateCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        public CreateCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatservice)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatservice;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUser(userId);

            if (parts.Length > 2)
            {
                throw new InvalidOperationException("Room name cannot contain spaces.");
            }

            if (parts.Length == 1)
            {
                throw new InvalidOperationException("No room specified.");
            }

            roomName = parts[1];
            if (String.IsNullOrWhiteSpace(roomName))
            {
                throw new InvalidOperationException("No room specified.");
            }

            ChatRoom room = _repository.GetRoomByName(roomName);

            if (room != null)
            {
                throw new InvalidOperationException(String.Format("The room '{0}' already exists{1}",
                    roomName,
                    room.Closed ? " but it's closed" : String.Empty));
            }

            // Create the room, then join it
            room = _chatService.AddRoom(user, roomName);
            
            _chatService.JoinRoom(user, room, null);

            _repository.CommitChanges();
            
            _notificationService.JoinRoom(user, room);
            
        }
    }
}

