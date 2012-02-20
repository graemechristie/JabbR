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
    [CommandMetadata(Name = "note", Usage = "Type /note - To set a note shown via a paperclip icon next to your name, with the message appearing when you hover over it.", Weight = 24.0f)]
    public class NoteCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        private readonly IJabbrRepository _repository;

        [ImportingConstructor]
        public NoteCommand(INotificationService notificationService, IJabbrRepository repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            ChatUser user = _repository.VerifyUserId(userId);

            // We need to determine if we're either
            // 1. Setting a new Note.
            // 2. Clearing the existing Note.
            // If we have no optional text, then we need to clear it. Otherwise, we're storing it.
            bool isNoteBeingCleared = parts.Length == 1;
            user.Note = isNoteBeingCleared ? null : String.Join(" ", parts.Skip(1)).Trim();

            ChatService.ValidateNote(user.Note);

            _notificationService.ChangeNote(user);

            _repository.CommitChanges();
            
        }
    }
}

