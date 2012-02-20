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
    [CommandMetadata(Name = "flag", Usage = "Type /flag [Iso 3366-2 Code] - To show a small flag which represents your nationality. Eg. /flag US for a USA flag. ISO Reference Chart: http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2 (Apologies to people with dual citizenship).", Weight = 26.0f)]
    public class FlagCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        [ImportingConstructor]
        public FlagCommand(INotificationService notificationService, IJabbrRepository repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            if (parts.Length <= 1)
            {
                // Clear the flag.
                user.Flag = null;
            }
            else
            {
                // Set the flag.
                string isoCode = String.Join(" ", parts[1]).ToLowerInvariant();
                ChatService.ValidateIsoCode(isoCode);
                user.Flag = isoCode;
            }

            _notificationService.ChangeFlag(user);

            _repository.CommitChanges();
            
        }
    }
}

