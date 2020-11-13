﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Nmb.Shared.Initialization
{
    public static class HostExtension
    {
        public static IHost StartInitializationProcess(this IHost host, bool always = false)
        {
            using (var scope = host.Services.CreateScope())
            {
                foreach (var stage in scope.ServiceProvider.GetServices<IInitializationStage>().OrderBy(t => t.Order))
                {
                    stage.ExecuteAsync().Wait();
                }
            }
            return host;
        }
    }
}
