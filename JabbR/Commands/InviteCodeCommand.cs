using System;
using System.ComponentModel.Composition;
using JabbR.Infrastructure;
using JabbR.Models;
using JabbR.Services;

namespace JabbR.Commands
{
    [CommandMetadata(
        Name = "invitecode", 
        Usage = "Type /invitecode - To show the current invite code", 
        Weight=22.0f
    )]
    public class InviteCodeCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public InviteCodeCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);
            ChatRoom room = _repository.VerifyRoom(roomName);

            if (String.IsNullOrEmpty(room.InviteCode))
            {
                _chatService.SetInviteCode(user, room, RandomUtils.NextInviteCode());
            }
            _notificationService.PostNotification(room, user, String.Format("Invite Code for this room: {0}", room.InviteCode));

        }
    }
}

