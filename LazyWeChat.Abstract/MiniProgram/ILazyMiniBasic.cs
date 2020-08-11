using LazyWeChat.Models;
using LazyWeChat.Models.MiniProgram;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.MiniProgram
{
    public interface ILazyMiniBasic
    {
        Task<string> GetAccessTokenAsync();

        Task<dynamic> Code2SessionAsync(string js_code);

        Task<byte[]> GetTempMediaAsync(string media_id);

        Task<dynamic> SendKFMessage(MiniKFMessage message);

        Task<dynamic> SendUniformMessage(UniformMessage message);

        string Decrypt(string encryptedData, string iv, string sessionKey);

        Task<dynamic> GetTemplateList();

        Task<dynamic> SendSubscribeMessage(SubscribeMessage message);
    }
}
