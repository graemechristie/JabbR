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
    [CommandMetadata(Name = "kick", Usage = "Type /kick [user] to kick a user from the room. Note, this is only valid for owners of the room.", Weight=14.0f)]
    public class KickCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public KickCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);
            ChatRoom room = _repository.VerifyRoom(roomName);

            if (parts.Length == 1)
            {
                throw new InvalidOperationException("Who are you trying to kick?");
            }

            if (room.Users.Count == 1)
            {
                throw new InvalidOperationException("You're the only person in here...");
            }

            string targetUserName = parts[1];

            ChatUser targetUser = _repository.VerifyUser(targetUserName);

            _chatService.KickUser(user, targetUser, room);

            _notificationService.KickUser(targetUser, room);

        }
    }
}

