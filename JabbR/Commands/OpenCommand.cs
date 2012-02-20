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
    [CommandMetadata(Name = "open", Usage = "Type /open [room] -  To open a room. Only works if you're the creator of that room.", Weight = 18.5f)]
    public class OpenCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public OpenCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
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
                throw new InvalidOperationException("Which room do you want to open?");
            }

            roomName = parts[1];
            ChatRoom room = _repository.VerifyRoom(roomName, mustBeOpen: false);

            _chatService.OpenRoom(user, room);

            // Automatically join user to newly opened room
            _chatService.JoinRoom(user, room, null);

            _repository.CommitChanges();
            
            _notificationService.JoinRoom(user, room);
        }
    }
}

