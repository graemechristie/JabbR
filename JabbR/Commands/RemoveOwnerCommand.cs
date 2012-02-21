using System;
using System.ComponentModel.Composition;
using JabbR.Models;
using JabbR.Services;

namespace JabbR.Commands
{
    [CommandMetadata(
        Name = "removeowner", 
        Usage = "Type /removeowner [user] [room] - To remove an owner from the specified room. Only works if you're the creator of that room", 
        Weight = 17.0f
    )]
    public class RemoveOwnerCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public RemoveOwnerCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
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
                throw new InvalidOperationException("Which owner do you want to remove?");
            }

            string targetUserName = parts[1];

            ChatUser targetUser = _repository.VerifyUser(targetUserName);

            if (parts.Length == 2)
            {
                throw new InvalidOperationException("Which room?");
            }

            roomName = parts[2];
            ChatRoom targetRoom = _repository.VerifyRoom(roomName);

            _chatService.RemoveOwner(user, targetUser, targetRoom);

            _notificationService.RemoveOwner(targetUser, targetRoom);

            _repository.CommitChanges();
        }
    }
}

