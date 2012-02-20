using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabbR.Services;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using JabbR.Models;

namespace JabbR.Commands
{
    public class CommandManager
    {
        private readonly string _clientId;
        private readonly string _userAgent;
        private readonly string _userId;
        private readonly string _roomName;
        private readonly INotificationService _notificationService;
        private readonly IChatService _chatService;
        private readonly IJabbrRepository _repository;

        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<Lazy<ICommand, ICommandMetaData>> _commands { get; set; } 

        public CommandManager(string clientId,
                              string userId,
                              string roomName,
                              IChatService service,
                              IJabbrRepository repository,
                              INotificationService notificationService)
            : this(clientId, null, userId, roomName, service, repository, notificationService)
        {
        }

        public CommandManager(string clientId,
                              string userAgent,
                              string userId,
                              string roomName,
                              IChatService service,
                              IJabbrRepository repository,
                              INotificationService notificationService)
        {
            _clientId = clientId;
            _userAgent = userAgent;
            _userId = userId;
            _roomName = roomName;
            _chatService = service;
            _repository = repository;
            _notificationService = notificationService;

            LoadCommands();
        }


        private void LoadCommands() 
        {

            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(catalog);

            // Add our injected dependancies to MEF's Compoisition Container. 
            // Bit of a hack as we are kind of bridging Ninject and MEF
            container.ComposeExportedValue<INotificationService>(_notificationService);
            container.ComposeExportedValue<IJabbrRepository>(_repository);
            container.ComposeExportedValue<IChatService>(_chatService);

            var batch = new CompositionBatch();
            batch.AddPart(this);
            container.Compose(batch);    
        }

        public bool TryHandleCommand(string command)
        {
            command = command.Trim();
            if (!command.StartsWith("/"))
            {
                return false;
            }

            string[] parts = command.Substring(1).Split(' ');
            string commandName = parts[0];

            return TryHandleCommand(commandName, parts);
        }

        public bool TryHandleCommand(string commandName, string[] parts)
        {
            commandName = commandName.Trim();
            if (commandName.StartsWith("/"))
            {
                return false;
            }

            ICommand command;
            try
            {
                command = _commands.First(lc => lc.Metadata.Name == commandName).Value;
            }
            catch (InvalidOperationException) // Command was not found in the collection
            {
                throw new InvalidOperationException(String.Format("'{0}' is not a valid command.", commandName));
            }

            command.Handle(parts, _userId, _roomName, _clientId, _userAgent);

            return true;
        }

        public IEnumerable<ICommandMetaData> GetAllCommandMetaData()
        {
            return _commands.Select(ic => ic.Metadata);
        }
    }
}