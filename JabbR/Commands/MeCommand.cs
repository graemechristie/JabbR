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
    [CommandMetadata(Name = "me", Usage = "Type /me 'does anything'", Weight=5.0f)]
    public class MeCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        [ImportingConstructor]
        public MeCommand( INotificationService notificationService, IJabbrRepository repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            if (parts.Length < 2)
            {
                throw new InvalidOperationException("You what?");
            }

            var content = String.Join(" ", parts.Skip(1));

            ChatUser user = _repository.VerifyUserId(userId);
            ChatRoom room = _repository.VerifyUserRoom(user, roomName);

            _notificationService.OnSelfMessage(room, user, content);         
        }
    }
}