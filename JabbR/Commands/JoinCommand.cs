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
    [CommandMetadata(Name = "join", Usage = "Type /join [room] [inviteCode] - to join a channel of your choice. If it is private and you have an invite code, enter it after the room name", Weight = 3.0f)]
    public class JoinCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public JoinCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatservice)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatservice;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            if (parts.Length < 2)
            {
                throw new InvalidOperationException("Join which room?");
            }

            // Extract arguments
            roomName = parts[1];
            string inviteCode = null;
            if (parts.Length > 2)
            {
                inviteCode = parts[2];
            }

            // Locate the room
            ChatRoom room = _repository.VerifyRoom(roomName);

            if (!ChatService.IsUserInRoom(room, user))
            {

                _chatService.JoinRoom(user, room, inviteCode);

                _repository.CommitChanges();
            }
            
            _notificationService.JoinRoom(user, room);
            
        }
    }
}

