using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabbR.Services;
using JabbR.Models;
using Ninject;
using JabbR.Infrastructure;
using System.ComponentModel.Composition;

namespace JabbR.Commands
{
    [Export(typeof(ICommand))]
    [CommandMetadata(Name = "msg", Usage = "Type /msg @nickname (message) to send a private message to nickname. @ is optional", Weight = 6.0f)]
    public class MsgCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        [ImportingConstructor]
        public MsgCommand(INotificationService notificationService, IJabbrRepository repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            if (_repository.Users.Count() == 1)
            {
                throw new InvalidOperationException("You're the only person in here...");
            }

            if (parts.Length < 2 || String.IsNullOrWhiteSpace(parts[1]))
            {
                throw new InvalidOperationException("Who are you trying send a private message to?");
            }
            var toUserName = parts[1];
            ChatUser toUser = _repository.VerifyUser(toUserName);

            if (toUser == user)
            {
                throw new InvalidOperationException("You can't private message yourself!");
            }

            string messageText = String.Join(" ", parts.Skip(2)).Trim();

            if (String.IsNullOrEmpty(messageText))
            {
                throw new InvalidOperationException(String.Format("What did you want to say to '{0}'.", toUser.Name));
            }

            HashSet<string> urls;
            var transform = new TextTransform(_repository);
            messageText = transform.Parse(messageText);

            messageText = TextTransform.TransformAndExtractUrls(messageText, out urls);

            _notificationService.SendPrivateMessage(user, toUser, messageText);
            
        }
    }
}

