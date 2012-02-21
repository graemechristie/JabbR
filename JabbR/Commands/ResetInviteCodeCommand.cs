using System;
using System.ComponentModel.Composition;
using JabbR.Infrastructure;
using JabbR.Models;
using JabbR.Services;

namespace JabbR.Commands
{
    [CommandMetadata(
        Name = "resetinvitecode", 
        Usage = "Type /resetinvitecode - To reset the current invite code. " +
            "This will render the previous invite code invalid", 
        Weight=23.0f
    )]
    public class ResetInviteCodeCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public ResetInviteCodeCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);
            ChatRoom room = _repository.VerifyRoom(roomName);

            _chatService.SetInviteCode(user, room, RandomUtils.NextInviteCode());
            _notificationService.PostNotification(room, user, String.Format("Invite Code for this room: {0}", room.InviteCode));

        }
    }
}

