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
    [CommandMetadata(Name = "close", Usage = "Type /close [room] - To close a room. Only works if you're an owner of that room.", Weight = 19.0f)]
    public class CloseCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public CloseCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            if (parts.Length < 2)
            {
                throw new InvalidOperationException("Which room do you want to close?");
            }

            roomName = parts[1];
            ChatRoom room = _repository.VerifyRoom(roomName);

            // Before I close the room, I need to grab a copy of -all- the users in that room.
            // Otherwise, I can't send any notifications to the room users, because they
            // have already been kicked.
            var users = room.Users.ToList();
            _chatService.CloseRoom(user, room);

            _notificationService.CloseRoom(users, room);
        }
    }
}

