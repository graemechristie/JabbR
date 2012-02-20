using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabbR.Services;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace JabbR.Commands
{
    public class CommandManager
    {

        private readonly INotificationService _notificationService;

        [Import(typeof(ICommand))]
        private IEnumerable<ICommand> _commands;

        public CommandManager(INotificationService notificationService)
        {
            _notificationService = notificationService;
            LoadCommands();
        }

        private void LoadCommands()
        {
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(catalog);
            var batch = new CompositionBatch();
            batch.AddPart(this);
            container.Compose(batch);
        }

        public bool TryHandleCommand(string command, string clientId, string userAgent, string userId, string roomName)
        {
            command = command.Trim();
            if (!command.StartsWith("/"))
            {
                return false;
            }

            string[] parts = command.Substring(1).Split(' ');
            string commandName = parts[0];

            return TryHandleCommand(commandName, parts, clientId, userAgent, userId, roomName);
        }

        public bool TryHandleCommand(string commandName, string[] parts, string clientId, string userAgent, string userId, string roomName)
        {
            commandName = commandName.Trim();
            if (commandName.StartsWith("/"))
            {
                return false;
            }

            ICommand command = _commandFactory.Get(commandName, _notificationService);
            command.Handle(parts, userId, roomName, clientId, userAgent);

            return true;
        }
    }
}