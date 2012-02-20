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
    [CommandMetadata(Name = "nick", Usage = "Type /nick [user] [password] to create a user or change your nickname. You can change your password with /nick [user] [oldpassword] [newpassword]", Weight = 2.0f)]
    public class NickCommand : ICommand
    {
        private readonly INotificationService _notificationService;
        private readonly IJabbrRepository _repository;
        private readonly IChatService _chatService;

        [ImportingConstructor]
        public NickCommand(INotificationService notificationService, IJabbrRepository repository, IChatService chatService)
        {
            _notificationService = notificationService;
            _repository = repository;
            _chatService = chatService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            if (parts.Length == 1)
            {
                throw new InvalidOperationException("No nick specified!");
            }

            string userName = parts[1];

            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new InvalidOperationException("No nick specified!");
            }

            string password = null;
            if (parts.Length > 2)
            {
                password = parts[2];
            }

            string newPassword = null;
            if (parts.Length > 3)
            {
                newPassword = parts[3];
            }

            // See if there is a current user
            ChatUser user = _repository.GetUserById(userId);

            if (user == null && String.IsNullOrEmpty(newPassword))
            {
                user = _repository.GetUserByName(userName);

                // There's a user with the name specified
                if (user != null)
                {
                    if (String.IsNullOrEmpty(password))
                    {
                        ChatService.ThrowPasswordIsRequired();
                    }
                    else
                    {
                        // If there's no user but there's a password then authenticate the user
                        _chatService.AuthenticateUser(userName, password);

                        // Add this client to the list of clients for this user
                        _chatService.AddClient(user, clientId, userAgent);

                        // Initialize the returning user
                        _notificationService.LogOn(user, clientId);
                    }
                }
                else
                {
                    // If there's no user add a new one
                    user = _chatService.AddUser(userName, clientId, userAgent, password);

                    // Notify the user that they're good to go!
                    _notificationService.OnUserCreated(user);
                }
            }
            else
            {
                if (String.IsNullOrEmpty(password))
                {
                    string oldUserName = user.Name;

                    // Change the user's name
                    _chatService.ChangeUserName(user, userName);

                    _notificationService.OnUserNameChanged(user, oldUserName, userName);
                }
                else
                {
                    // If the user specified a password, verify they own the nick
                    ChatUser targetUser = _repository.VerifyUser(userName);

                    // Make sure the current user and target user are the same
                    if (user != targetUser)
                    {
                        throw new InvalidOperationException("You can't set/change the password for a nickname you down own.");
                    }

                    if (String.IsNullOrEmpty(newPassword))
                    {
                        if (targetUser.HashedPassword == null)
                        {
                            _chatService.SetUserPassword(user, password);

                            _notificationService.SetPassword();
                        }
                        else
                        {
                            throw new InvalidOperationException("Use /nick [nickname] [oldpassword] [newpassword] to change and existing password.");
                        }
                    }
                    else
                    {
                        _chatService.ChangeUserPassword(user, password, newPassword);

                        _notificationService.ChangePassword();
                    }
                }
            }

            // Commit the changes
            _repository.CommitChanges();      
        }
    }
}