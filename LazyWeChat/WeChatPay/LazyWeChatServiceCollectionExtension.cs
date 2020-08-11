using LazyWeChat.Abstract;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Abstract.WeChatPay.V2;
using LazyWeChat.Models;
using Microsoft.Extensions.DependencyInjection;
using LazyWeChat;
using System;
using System.Collections.Generic;

namespace LazyWeChat.WeChatPay
{
    public static partial class LazyPayServiceCollectionExtension
    {
        static readonly string LAZYWXSECTIONNAME = "LazyWeChatSettings";
        static readonly string ILAZYBASICPAYV2IMPELEMENTATION = "LazyBasicPayV2";

        /// <summary>
        /// 初始化LazyWeChat的基础服务
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns></returns>
        public static IServiceCollection AddLazyPay(this IServiceCollection services)
        {
            return services.AddLazyPay(null);
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
        public static IServiceCollection AddLazyPay(this IServiceCollection services, Action<LazyWeChatConfiguration> configure)
        {
            services.AddHttpClient();

            var items = new List<(Type, string, ServiceLifetime)>();
            items.Add((typeof(IHttpRepository), Constant.IHTTPREPOSITORYIMPELEMENTATION, ServiceLifetime.Singleton));
            items.Add((typeof(IQRGenerator), Constant.IQRGENERATORIMPELEMENTATION, ServiceLifetime.Transient));
            items.Add((typeof(ILazyBasicPayV2), ILAZYBASICPAYV2IMPELEMENTATION, ServiceLifetime.Transient));

            return services.RegisterServices(LAZYWXSECTIONNAME, configure, items);
        }
    }
}
