using Microsoft.Extensions.DependencyInjection;

namespace Todo.Common
{
    public static class HostingExtensions
    {
        public static IServiceCollection AddSystemClock(this IServiceCollection svc) => svc.AddSingleton<IClock, SystemClock>();

        public static IServiceCollection AddFrozenClock(this IServiceCollection svc) => svc.AddSingleton<IClock, FrozenClock>();
    }
}
