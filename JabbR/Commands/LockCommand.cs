﻿using System;
using System.ComponentModel.Composition;
using JabbR.Models;
using JabbR.Services;

namespace JabbR.Commands
{
    [CommandMetadata(
        Name = "lock", 
        Usage = "Type /lock [room] - To make a room private. Only works if you're the creator of that room.", 
        Weight = 18.0f
    )]
    public class LockCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        private readonly IChatService _chatService;

        [ImportingConstructor]
        public LockCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            if (parts.Length < 2)
            {
                throw new InvalidOperationException("Which room do you want to lock?");
            }

            roomName = parts[1];
            ChatRoom room = _repository.VerifyRoom(roomName);

            _chatService.LockRoom(user, room);

            _notificationService.LockRoom(user, room);
        }
    }
}

