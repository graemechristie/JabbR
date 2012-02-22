using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using JabbR.Commands;
using JabbR.Services;
using Ninject.Parameters;
using Ninject;
using SignalR.Hubs;

namespace JabbR.App_Start
{
    public class CommandModule : NinjectModule
    {
        // We manually list command types here to avoid the overhead of reflection
        // But we can do it either way (see below)
        private static Type[] _commandTypes = { typeof(HelpCommand),
                                        typeof(AfkCommand),
                                        typeof(AddOwnerCommand),
                                        typeof(AllowCommand),
                                        typeof(CloseCommand),
                                        typeof(CreateCommand),
                                        typeof(FlagCommand),
                                        typeof(GravatarCommand),
                                        typeof(InviteCodeCommand),
                                        typeof(JoinCommand),
                                        typeof(KickCommand),
                                        typeof(LeaveCommand),
                                        typeof(ListCommand),
                                        typeof(LockCommand),
                                        typeof(LogoutCommand),
                                        typeof(MeCommand),
                                        typeof(MsgCommand),
                                        typeof(NickCommand),
                                        typeof(NoteCommand),
                                        typeof(NudgeCommand),
                                        typeof(OpenCommand),
                                        typeof(RemoveOwnerCommand),
                                        typeof(ResetInviteCodeCommand),
                                        typeof(RoomsCommand),
                                        typeof(TopicCommand),
                                        typeof(UnallowCommand),
                                        typeof(WhereCommand),
                                        typeof(WhoCommand)
                                      };

        public override void Load()
        {
            // Use this to autoload commands using reflection
            // We won't worry about this for now, and just use a list of known commands.
            //var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
            //   .Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && (t.GetCustomAttributes(typeof(CommandInfoAttribute), true).Count()) != 0);
            
            foreach (var ct in _commandTypes)
            {
                var cma=ct.GetCustomAttributes(typeof(CommandMetadataAttribute), true).First() as CommandMetadataAttribute;

                Bind<ICommand>()
                    .To(ct)
                    .InRequestScope()
                    .Named(cma.Name)
                    .WithMetadata("CommandMetadata", new CommandMetadata { Name=cma.Name, Usage=cma.Usage, Weight=cma.Weight });
            }

            // Ad Hoc Factory method to get command based on string passed into constructor
            Bind<Func<string, INotificationService, ICommand>>()
                .ToMethod(
                    context =>
                        (command, notificationService) => context.Kernel.Get<ICommand>(command, new ConstructorArgument("notificationService", notificationService)));

            // Ad Hoc method to get all commands ... basically just used to construct help text
            Bind<Func<IEnumerable<CommandMetadata>>>()
                .ToMethod(
                    context =>
                        () => Kernel
                            .GetBindings(typeof(ICommand))
                            .Select(b => b.Metadata.Get<CommandMetadata>("CommandMetadata")));
        }
    }
}