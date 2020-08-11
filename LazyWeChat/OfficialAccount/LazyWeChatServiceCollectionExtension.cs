using LazyWeChat.Abstract;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Abstract.WeChatPay.V2;
using LazyWeChat.Models;
using Microsoft.Extensions.DependencyInjection;
using LazyWeChat;
using System;
using System.Collections.Generic;

namespace LazyWeChat.OfficialAccount
{
    public static partial class LazyWeChatServiceCollectionExtension
    {
        internal static readonly string LAZYWXSECTIONNAME = "LazyWeChatSettings";
        internal static readonly string ILAZYWECHATBASICIMPELEMENTATION = "LazyWeChatBasic";
        internal static readonly string ILAZYMESSAGERIMPELEMENTATION = "LazyMessager";
        internal static readonly string ILAZYMATERIALSIMPELEMENTATION = "LazyMaterials";

        /// <summary>
        /// 初始化LazyWeChat的基础服务
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns></returns>
        public static IServiceCollection AddLazyWeChat(this IServiceCollection services)
        {
            return services.AddLazyWeChat(null);
        }

        /// <summary>
        /// 初始化LazyWeChat的基础服务
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configure">
        /// 微信公众号的配置信息
        /// AppID/AppSecret
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddLazyWeChat(this IServiceCollection services, Action<LazyWeChatConfiguration> configure)
        {
            services.AddHttpClient();

            var items = new List<(Type, string, ServiceLifetime)>();
            items.Add((typeof(IHttpRepository), Constant.IHTTPREPOSITORYIMPELEMENTATION, ServiceLifetime.Transient));
            items.Add((typeof(ILazyWeChatBasic), ILAZYWECHATBASICIMPELEMENTATION, ServiceLifetime.Singleton));
            items.Add((typeof(ILazyMessager), ILAZYMESSAGERIMPELEMENTATION, ServiceLifetime.Scoped));
            items.Add((typeof(ILazyMaterials), ILAZYMATERIALSIMPELEMENTATION, ServiceLifetime.Transient));

            return services.RegisterServices(LAZYWXSECTIONNAME, configure, items);
        }
    }
}
