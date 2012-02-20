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
    [CommandMetadata(Name = "allow", Usage = "Type /allow [user] [room] - To give a user permission to a private room. Only works if you're an owner of that room.", Weight = 20.0f)]
    public class AllowCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public AllowCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            if (parts.Length == 1)
            {
                throw new InvalidOperationException("Who do you want to allow?");
            }

            string targetUserName = parts[1];

            ChatUser targetUser = _repository.VerifyUser(targetUserName);

            if (parts.Length == 2)
            {
                throw new InvalidOperationException("Which room?");
            }

            roomName = parts[2];
            ChatRoom targetRoom = _repository.VerifyRoom(roomName);

            _chatService.AllowUser(user, targetUser, targetRoom);

            _notificationService.AllowUser(targetUser, targetRoom);

            _repository.CommitChanges();
        }
    }
}

