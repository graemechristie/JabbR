using System.ComponentModel.Composition;
using JabbR.Services;

namespace JabbR.Commands
{
    [CommandMetadata(
        Name="help", 
        Usage="Type /help to show the list of commands", 
        Weight=1.0f
    )]
    public class HelpCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        [ImportingConstructor]
        public HelpCommand(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            _notificationService.ShowHelp();            
        }
    }
}