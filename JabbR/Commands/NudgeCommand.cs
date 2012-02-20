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
    [CommandMetadata(Name = "nudge", Usage = "Type /nudge to send a nudge to the whole room, or \"/nudge @nickname\" to nudge a particular user. @ is optional.", Weight=13.0f)]
    public class NudgeCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        [ImportingConstructor]
        public NudgeCommand(INotificationService notificationService, IJabbrRepository repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            if (parts.Length == 2) // nudge [user]
            {
                nudgeUser(parts, user);
            }
            else // /nudge whole room
            {
                ChatRoom room = _repository.VerifyUserRoom(user, roomName);
                nudgeRoom(user, room);
            }
        }

        /// <summary>
        /// Nudges a user
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="user"></param>
        private void nudgeUser(string[] parts, ChatUser user)
        {
            if (_repository.Users.Count() == 1)
            {
                throw new InvalidOperationException("You're the only person in here...");
            }

            var toUserName = parts[1];

            ChatUser toUser = _repository.VerifyUser(toUserName);

            if (toUser == user)
            {
                throw new InvalidOperationException("You can't nudge yourself!");
            }

            string messageText = String.Format("{0} nudged you", user);

            var betweenNudges = TimeSpan.FromSeconds(60);
            if (toUser.LastNudged.HasValue && toUser.LastNudged > DateTime.Now.Subtract(betweenNudges))
            {
                throw new InvalidOperationException(String.Format("User can only be nudged once every {0} seconds", betweenNudges.TotalSeconds));
            }

            toUser.LastNudged = DateTime.Now;
            _repository.CommitChanges();

            _notificationService.NudgeUser(user, toUser);
        }

        /// <summary>
        /// Nudges Room
        /// </summary>
        /// <param name="parts">Command Line Parms</param>
        /// <param name="user">ChatUser doing the Nudging</param>
        /// <param name="room">Room to Nudge</param>
        private void nudgeRoom(ChatUser user, ChatRoom room)
        {

            var betweenNudges = TimeSpan.FromMinutes(1);
            if (room.LastNudged == null || room.LastNudged < DateTime.Now.Subtract(betweenNudges))
            {
                room.LastNudged = DateTime.Now;
                _repository.CommitChanges();

                _notificationService.NudgeRoom(room, user);
            }
            else
            {
                throw new InvalidOperationException(String.Format("Room can only be nudged once every {0} seconds", betweenNudges.TotalSeconds));
            }
        }
    }
}

