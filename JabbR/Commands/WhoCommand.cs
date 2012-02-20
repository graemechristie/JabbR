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
    [CommandMetadata(Name = "who", Usage = "Type /who to show a list of all users, /who [name] to show specific information about that user", Weight = 10.0f)]
    public class WhoCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        [ImportingConstructor]
        public WhoCommand(INotificationService notificationService, IJabbrRepository repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            if (parts.Length == 1)
            {
                _notificationService.ListUsers();
                return;
            }

            var name = ChatService.NormalizeUserName(parts[1]);

            ChatUser user = _repository.GetUserByName(name);

            if (user == null)
            {
                throw new InvalidOperationException(String.Format("We didn't find anyone with the username {0}", name));
            }

            _notificationService.ShowUserInfo(user);
        }
    }
}

