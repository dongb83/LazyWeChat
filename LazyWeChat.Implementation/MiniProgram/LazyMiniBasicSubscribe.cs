using LazyWeChat.Abstract.MiniProgram;
using LazyWeChat.Models.MiniProgram;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.MiniProgram
{
    public static partial class CONSTANT
    {
        public const string ADDTEMPLATEURL = "https://api.weixin.qq.com/wxaapi/newtmpl/addtemplate?access_token={0}";

        public const string DELETETEMPLATEURL = "https://api.weixin.qq.com/wxaapi/newtmpl/deltemplate?access_token={0}";

        public const string GETPUBTEMPLATEKEYWORDSBYIDURL = "https://api.weixin.qq.com/wxaapi/newtmpl/getpubtemplatekeywords?access_token={0}";

        public const string GETCATEGORYURL = "https://api.weixin.qq.com/wxaapi/newtmpl/getcategory?access_token={0}";

        public const string GETPUBTEMPLATETITLELISTURL = "https://api.weixin.qq.com/wxaapi/newtmpl/getpubtemplatetitles?access_token={0}";

        public const string GETTEMPLATELISTURL = "https://api.weixin.qq.com/wxaapi/newtmpl/gettemplate?access_token={0}";

        public const string SUBSCRIBEMESSAGEURL = "https://api.weixin.qq.com/cgi-bin/message/subscribe/send?access_token={0}";
    }


    public partial class LazyMiniBasic : ILazyMiniBasic
    {
        async Task<dynamic> SendRequest(dynamic requestObject, string requestUrl, HttpMethod method, params string[] validationNames)
        {
            var accessToken = await _lazyWeChatBasic.GetAccessTokenAsync();
            return await _httpRepository.SendRequest(requestObject, requestUrl, method, accessToken, validationNames);
        }

        public async Task<dynamic> AddTemplateAsync(string tid, string sceneDesc, params string[] kidList) =>
            await SendRequest(new { tid = tid, sceneDesc = sceneDesc, kidList = kidList }, CONSTANT.ADDTEMPLATEURL, HttpMethod.Post);

        public async Task<dynamic> DeleteTemplateAsync(string priTmplId) => await SendRequest(new { priTmplId = priTmplId }, CONSTANT.DELETETEMPLATEURL, HttpMethod.Post);

        public async Task<dynamic> GetCategoryAsync() => await SendRequest("", CONSTANT.GETCATEGORYURL, HttpMethod.Get);

        public async Task<dynamic> GetPubTemplateKeyWordsByIdAsync(string tid) => await SendRequest(new { tid = tid }, CONSTANT.GETPUBTEMPLATETITLELISTURL, HttpMethod.Get);

        public async Task<dynamic> GetPubTemplateTitleListAsync(string ids, int start, int limit) => await SendRequest(new { ids = ids, start = start, limit = limit }, CONSTANT.GETPUBTEMPLATETITLELISTURL, HttpMethod.Get);

        public async Task<dynamic> GetTemplateListAsync() => await SendRequest("", CONSTANT.GETTEMPLATELISTURL, HttpMethod.Get);

        public async Task<dynamic> SendSubscribeMessageAsync(SubscribeMessage message)
        {
            if (message.miniprogram_state == null)
                message.miniprogram_state = MiniprogramState.formal.ToString();

            if (message.lang == null)
                message.lang = Language.zh_CN.ToString();

            var res = await SendRequest(message, CONSTANT.SUBSCRIBEMESSAGEURL, HttpMethod.Post);
            return res;
        }
    }
}
