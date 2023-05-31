using System;
using Fibonacci.Logic.Infrastructure;
using Fibonacci.Logic.Infrastructure.Interfaces;
using Fibonacci.Logic.Services;
using Fibonacci.Logic.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fibonacci.WebApi.Infrastructure
{
    public class Dependencies
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICacheService, CacheService>(_ => new CacheService(int.Parse(configuration["CacheLifetime"])));
            services.AddTransient<IAppMemoryGetter, AppMemoryGetter>();
            services.AddTransient<IMemoryUsageService, MemoryUsageService>();
            services.AddScoped<IFibonacciService, FibonacciService>();
            services.AddSingleton<ITwoThreadTaskScheduler, CircularTaskScheduler>(_ => new CircularTaskScheduler(2));
            services.AddSingleton<ICpuCoresThreadTaskScheduler, CircularTaskScheduler>(_ => new CircularTaskScheduler((byte)Math.Max(Environment.ProcessorCount, 2)));
        }
    }
}