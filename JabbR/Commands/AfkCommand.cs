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
    [CommandMetadata(Name = "afk", Usage = "Type /afk - (aka. Away From Keyboard). To set a temporary note shown via a paperclip icon next to your name, with the message appearing when you hover over it. This note will disappear when you first resume typing.", Weight = 25.0f)]
    public class AfkCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        [ImportingConstructor]
        public AfkCommand(INotificationService notificationService, IJabbrRepository repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            string message = String.Join(" ", parts.Skip(1)).Trim();

            ChatService.ValidateNote(message);

            user.AfkNote = String.IsNullOrWhiteSpace(message) ? null : message;
            user.IsAfk = true;

            _notificationService.ChangeNote(user);

            _repository.CommitChanges();
        }
    }
}

