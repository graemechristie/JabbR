using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using System.Reflection;
using JabbR.Commands;
using JabbR.Services;
using Ninject;
using Ninject.Parameters;

namespace JabbR.App_Start
{
    public class CommandModule : NinjectModule 
    {
        public override void Load()
        {
            var iCommands = Assembly.GetExecutingAssembly().GetTypes()
               .Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && (t.GetCustomAttributes(typeof(CommandInfoAttribute), true).Count()) != 0);

            
            foreach (var t in iCommands)
            {
                var attributeInfo = t.GetCustomAttributes(typeof(CommandInfoAttribute), true).First() as CommandInfoAttribute;
                Bind<ICommand>()
                    .To(t)
                    .InRequestScope()
                    .Named(attributeInfo.Name)
                    .WithMetadata("CommandInfo", new CommandInfo { Name = attributeInfo.Name, Usage = attributeInfo.Usage, Weight=attributeInfo.Weight });
            }


            Bind<ICommandFactory>()
                .To<JabbrCommandFactory>()
                .InSingletonScope();


            //// Ad Hoc Factory method to get command based on string passed into constructor
            //Bind<Func<string, INotificationService, ICommand>>()
            //    .ToMethod(
            //        context =>
            //            (command, notificationService) => context.Kernel.Get<ICommand>(command, new ConstructorArgument("notificationService", notificationService)));

            //// Ad Hoc method to get all commands ... basically just used to construct help text
            //Bind<Func<IEnumerable<CommandInfo>>>()
            //    .ToMethod(
            //        context =>
            //            () => Kernel
            //                .GetBindings(typeof(ICommand))
            //                .Select(b => b.Metadata.Get<CommandInfo>("CommandInfo")));

        }
    }
}