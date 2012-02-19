using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using JabbR.Services;
using Ninject.Parameters;
using JabbR.Commands;

namespace JabbR.App_Start
{
    public class JabbrCommandFactory : ICommandFactory
    {
        private readonly IKernel _kernel;

        public JabbrCommandFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public ICommand Get(string commandName, INotificationService notificationService)
        {
            try
            {
                return _kernel.Get<ICommand>(commandName, new ConstructorArgument("notificationService", notificationService));
            }
            catch (ActivationException) 
            {
                throw new InvalidOperationException(String.Format("'{0}' is not a valid command.", commandName));
            }
        }

        public IEnumerable<CommandInfo> GetAllCommandInfo()
        {
            return _kernel.GetBindings(typeof(ICommand)).Select(b => b.Metadata.Get<CommandInfo>("CommandInfo")).OrderBy(ci=>ci.Weight);
        }
    }
}