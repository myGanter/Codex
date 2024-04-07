using Microsoft.Extensions.DependencyInjection;
using Codex.Dispatcher;

namespace Codex.AspNet.Infrastructure
{
    internal class MicrosoftDiAdapter : IDiAdapter
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrosoftDiAdapter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? Create(Type type) 
            => _serviceProvider.GetService(type);

        public TService? Create<TService>() where TService : class
            => _serviceProvider.GetService<TService>();
    }
}
