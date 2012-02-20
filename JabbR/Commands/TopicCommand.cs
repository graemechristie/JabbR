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
    [CommandMetadata(Name = "topic", Usage = "Type /topic [topic] to set the room topic. Type /topic to clear the room's topic.", Weight=27.0f)]
    public class TopicCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public TopicCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);
            ChatRoom room = _repository.VerifyRoom(roomName);

            string newTopic = String.Join(" ", parts.Skip(1)).Trim();
            ChatService.ValidateTopic(newTopic);
            newTopic = String.IsNullOrWhiteSpace(newTopic) ? null : newTopic;
            _chatService.ChangeTopic(user, room, newTopic);
            _notificationService.ChangeTopic(user, room);
        }
    }
}

