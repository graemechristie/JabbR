using System;
using System.ComponentModel.Composition;
using System.Linq;
using JabbR.Infrastructure;
using JabbR.Models;
using JabbR.Services;

namespace JabbR.Commands
{
    [CommandMetadata(
        Name = "gravatar", 
        Usage = "Type /gravatar [email] to set your gravatar", 
        Weight = 12.0f
    )]
    public class GravatarCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public GravatarCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatservice)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatservice;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            string email = String.Join(" ", parts.Skip(1));

            if (String.IsNullOrWhiteSpace(email))
            {
                throw new InvalidOperationException("Email was not specified!");
            }

            string hash = email.ToLowerInvariant().ToMD5();

            // Set user hash
            user.Hash = hash;

            _notificationService.ChangeGravatar(user);

            _repository.CommitChanges();
            
        }
    }
}

