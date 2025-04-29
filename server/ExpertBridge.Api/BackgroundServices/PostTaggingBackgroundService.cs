


namespace ExpertBridge.Api.BackgroundServices
{
    public class PostTaggingBackgroundService : BackgroundService
    {
        // BEWARE! This service will be registred as a singleton, so we need to
        // create our own scope of _services then use it to instanciate a service
        // like dbcontext and other.
        private readonly IServiceCollection _services;

        public PostTaggingBackgroundService(
            IServiceCollection services)
        {
            _services = services;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
