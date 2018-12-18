using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinAppDriver.Server
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private CommandEnvironment _commandEnvironment;

        public Bootstrapper(CommandEnvironment commandEnvironment)
        {
            _commandEnvironment = commandEnvironment;
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            // Register other types here

            container.Register<CommandEnvironment>((x, options) => {
                return _commandEnvironment;
            });
        }
    }
}
