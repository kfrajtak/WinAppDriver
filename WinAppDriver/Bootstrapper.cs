using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace WinAppDriver.Server
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            pipelines.OnError += (_, ex) =>
            {
                Logger.Error(ex, "Unexpected error has occurred.");
                return null;
            };
        }
    }
}
