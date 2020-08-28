using LazyWeChat.Abstract;
using LazyWeChat.Abstract.QY;
using LazyWeChat.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWeChat.QY
{
    public static class LazyQYServiceCollectionExtension
    {
        internal static readonly string LAZYWXSECTIONNAME = "LazyWeChatSettings";
        internal static readonly string ILAZYQYBASICIMPELEMENTATION = "LazyQYBasic";
        internal static readonly string ILAZYQYCONTACTIMPELEMENTATION = "LazyQYContact";

        public static IServiceCollection AddLazyQY(this IServiceCollection services)
        {
            return services.AddLazyQY(null);
        }

        public static IServiceCollection AddLazyQY(this IServiceCollection services, Action<LazyWeChatConfiguration> configure)
        {
            services.AddHttpClient();

            var items = new List<(Type, string, ServiceLifetime)>();
            items.Add((typeof(IHttpRepository), Constant.IHTTPREPOSITORYIMPELEMENTATION, ServiceLifetime.Transient));
            items.Add((typeof(ILazyQYBasic), ILAZYQYBASICIMPELEMENTATION, ServiceLifetime.Singleton));
            items.Add((typeof(ILazyQYContact), ILAZYQYCONTACTIMPELEMENTATION, ServiceLifetime.Transient));

            return services.RegisterServices(LAZYWXSECTIONNAME, configure, items);
        }
    }
}
