using System;
using System.ComponentModel.Composition;
using JabbR.Models;
using JabbR.Services;

namespace JabbR.Commands
{
    [CommandMetadata(
        Name = "addowner", 
        Usage = "Type /addowner [user] [room] - To add an owner a user as an owner to the specified room. " +
            "Only works if you're an owner of that room.", 
        Weight = 16.0f
    )]
    public class AddOwnerCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public AddOwnerCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
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
                throw new InvalidOperationException("Who do you want to make an owner?");
            }

            string targetUserName = parts[1];

            ChatUser targetUser = _repository.VerifyUser(targetUserName);

            if (parts.Length == 2)
            {
                throw new InvalidOperationException("Which room?");
            }

            roomName = parts[2];
            ChatRoom targetRoom = _repository.VerifyRoom(roomName);

            _chatService.AddOwner(user, targetUser, targetRoom);

            _notificationService.AddOwner(targetUser, targetRoom);

            _repository.CommitChanges();
        }
    }
}

