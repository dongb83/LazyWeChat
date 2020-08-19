using LazyWeChat.Abstract;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models;
using LazyWeChat.OfficialAccount;
using LazyWeChat.Utility;
using LazyWeChat.WeChatPay;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWeChat
{
    public static class LazyWeChatMiddlewareExtension
    {
        private readonly static string DEFAULTMESSAGEQUEUE = "DefaultMessageQueue";

        public static IApplicationBuilder UseLazyWeChat(this IApplicationBuilder app)
        {
            Action<WeChatMessager> _onMessageReceived = (message) => { };
            return app.UseLazyWeChat(_onMessageReceived);
        }

        public static IApplicationBuilder UseLazyWeChat(
            this IApplicationBuilder app,
            Action<WeChatMessager> onMessageReceived)
        {
            Type implementation = UtilRepository.GetImplementation(DEFAULTMESSAGEQUEUE);
            return app.UseMiddleware<LazyWeChatMiddleware>(onMessageReceived, implementation);
        }

        public static IApplicationBuilder UseLazyWeChat<T>(this IApplicationBuilder app) where T : IMessageQueue
        {
            Action<WeChatMessager> _onMessageReceived = (message) => { };
            return app.UseLazyWeChat<T>(_onMessageReceived);
        }

        public static IApplicationBuilder UseLazyWeChat<T>(
            this IApplicationBuilder app,
            Action<WeChatMessager> onMessageReceived) where T : IMessageQueue
        {
            return app.UseMiddleware<LazyWeChatMiddleware>(onMessageReceived, typeof(T));
        }

        public static IApplicationBuilder UseLazyNativePay(
            this IApplicationBuilder app,
            Func<string, (string, int)> _onGetProductInfo)
        {
            if (_onGetProductInfo == null)
                throw new ArgumentNullException(nameof(_onGetProductInfo));

            Type implementation = UtilRepository.GetImplementation(DEFAULTMESSAGEQUEUE);
            return app.UseMiddleware<NativeNotifyMiddleware>(_onGetProductInfo, implementation);
        }

        public static IApplicationBuilder UseLazyNativePay<T>(
            this IApplicationBuilder app,
            Func<string, (string, int)> _onGetProductInfo) where T : IMessageQueue
        {
            if (_onGetProductInfo == null)
                throw new ArgumentNullException(nameof(_onGetProductInfo));

            return app.UseMiddleware<NativeNotifyMiddleware>(_onGetProductInfo, typeof(T));
        }
    }
}
