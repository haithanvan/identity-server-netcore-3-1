using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nmb.Shared.Hangfire
{
    public class CustomJobActivator: JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomJobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            var serviceScope = _serviceProvider.CreateScope();
            return new CustomJobActivatorScope(serviceScope);
        }
    }

    internal class CustomJobActivatorScope : JobActivatorScope
    {
        private readonly IServiceScope _serviceScope;

        public CustomJobActivatorScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
        }

        public override object Resolve(Type type)
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(_serviceScope.ServiceProvider, type);
        }

        public override void DisposeScope()
        {
            _serviceScope.Dispose();
        }
    }
}
