using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabbR.Services;
using JabbR.Models;
using System.ComponentModel.Composition;

namespace JabbR.Commands
{
    [Export(typeof(ICommand))]
    [CommandMetadata(Name = "leave", Usage = "Type /leave to leave the current room. Type /leave [room name] to leave a specific room.", Weight=7.0f) ]
    public class LeaveCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public LeaveCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            if (parts.Length == 2) // leave [room]
            {
                roomName = parts[1];
            }

            ChatRoom room = _repository.VerifyRoom(roomName);

            _chatService.LeaveRoom(user, room);

            _notificationService.LeaveRoom(user, room);

            _repository.CommitChanges();
        }
    }
}

