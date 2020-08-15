using LazyWeChat.Abstract;
using LazyWeChat.Abstract.MiniProgram;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models;
using LazyWeChat.OfficialAccount;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWeChat.MiniProgram
{
    public static partial class LazyMiniProgramServiceCollectionExtension
    {
        internal static readonly string ILAZYMINIBASICIMPELEMENTATION = "LazyMiniBasic";

        public static IServiceCollection AddLazyMiniProgram(this IServiceCollection services)
        {
            return services.AddLazyMiniProgram(null);
        }

        public static IServiceCollection AddLazyMiniProgram(this IServiceCollection services, Action<LazyWeChatConfiguration> configure)
        {
            services.AddHttpClient();

            var items = new List<(Type, string, ServiceLifetime)>();
            items.Add((typeof(IHttpRepository), Constant.IHTTPREPOSITORYIMPELEMENTATION, ServiceLifetime.Transient));
            items.Add((typeof(ILazyWeChatBasic), LazyWeChatServiceCollectionExtension.ILAZYWECHATBASICIMPELEMENTATION, ServiceLifetime.Singleton));
            items.Add((typeof(ILazyMaterials), LazyWeChatServiceCollectionExtension.ILAZYMATERIALSIMPELEMENTATION, ServiceLifetime.Transient));
            items.Add((typeof(ILazyMiniBasic), ILAZYMINIBASICIMPELEMENTATION, ServiceLifetime.Transient));

            return services.RegisterServices(LazyWeChatServiceCollectionExtension.LAZYWXSECTIONNAME, configure, items);
        }
    }
}
