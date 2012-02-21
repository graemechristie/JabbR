using System;
using System.ComponentModel.Composition;
using JabbR.Models;
using JabbR.Services;

namespace JabbR.Commands
{
    [CommandMetadata(
        Name = "where", 
        Usage = "Type /where [name] to see the rooms that user is in", 
        Weight = 9.0f
    )]
    public class WhereCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        [ImportingConstructor]
        public WhereCommand(INotificationService notificationService, IJabbrRepository repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            if (parts.Length == 1)
            {
                throw new InvalidOperationException("Who are you trying to locate?");
            }

            ChatUser user = _repository.VerifyUser(parts[1]);
            _notificationService.ListRooms(user);
        }
    }
}

