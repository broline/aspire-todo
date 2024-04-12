using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Todo.Common
{
    public static class HostingExtensions
    {
        public static IServiceCollection AddSystemClock(this IServiceCollection svc) => svc.AddSingleton<IClock, SystemClock>();

        public static IServiceCollection AddFrozenClock(this IServiceCollection svc) => svc.Replace(ServiceDescriptor.Singleton<IClock, FrozenClock>());
    }
}
