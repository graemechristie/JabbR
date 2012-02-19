using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JabbR.Services;

namespace JabbR.Commands
{
    public interface ICommandFactory
    {
        ICommand Get(string commandName, INotificationService notificationService);

        IEnumerable<CommandInfo> GetAllCommandInfo();
    }
}
