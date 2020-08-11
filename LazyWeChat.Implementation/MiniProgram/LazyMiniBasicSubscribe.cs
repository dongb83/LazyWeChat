using LazyWeChat.Abstract.MiniProgram;
using LazyWeChat.Models.MiniProgram;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.MiniProgram
{
    public static partial class CONSTANT
    {
        public const string GETTEMPLATELISTURL = "https://api.weixin.qq.com/wxaapi/newtmpl/gettemplate?access_token={0}";

        public const string SUBSCRIBEMESSAGEURL = "https://api.weixin.qq.com/cgi-bin/message/subscribe/send?access_token={0}";
    }


    public partial class LazyMiniBasic : ILazyMiniBasic
    {
        public async Task<dynamic> GetTemplateList()
        {
            var access_token = await GetAccessTokenAsync();
            var url = string.Format(CONSTANT.GETTEMPLATELISTURL, access_token);
            var res = await _httpRepository.GetParseAsync(url);
            return res;
        }

        public async Task<dynamic> SendSubscribeMessage(SubscribeMessage message)
        {
            if (message.miniprogram_state == null)
                message.miniprogram_state = MiniprogramState.formal.ToString();

            if (message.lang == null)
                message.lang = Language.zh_CN.ToString();

            var access_token = await GetAccessTokenAsync();
            var url = string.Format(CONSTANT.SUBSCRIBEMESSAGEURL, access_token);
            var requestContent = JsonConvert.SerializeObject(message);
            var res = await _httpRepository.PostParseAsync(url, requestContent);
            return res;
        }
    }
}
