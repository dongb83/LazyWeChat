using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.OfficialAccount
{
    public static partial class CONSTANT
    {
        public const string SETINDUSTRYURL = "https://api.weixin.qq.com/cgi-bin/template/api_set_industry?access_token={0}";

        public const string GETINDUSTRYURL = "https://api.weixin.qq.com/cgi-bin/template/get_industry?access_token={0}";

        public const string GETTEMPLATEIDURL = "https://api.weixin.qq.com/cgi-bin/template/api_add_template?access_token={0}";

        public const string GETTEMPLATELISTURL = "https://api.weixin.qq.com/cgi-bin/template/get_all_private_template?access_token={0}";

        public const string DELETETEMPLATEURL = "https://api.weixin.qq.com/cgi-bin/template/del_private_template?access_token={0}";

        public const string SENDTEMPLATEMESSAGEURL = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
    }

    public partial class LazyMessager : ILazyMessager
    {
        async Task<dynamic> SendRequest(dynamic requestObject, string requestUrl, HttpMethod method, params string[] validationNames)
        {
            var accessToken = await _lazyWeChatBasic.GetAccessTokenAsync();
            return await _httpRepository.SendRequest(requestObject, requestUrl, method, accessToken, validationNames);
        }

        public async Task<dynamic> SetIndustryAsync(string industry1, string industry2)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.industry_id1 = industry1;
            requestObject.industry_id2 = industry2;

            var returnObject = await SendRequest(requestObject, CONSTANT.SETINDUSTRYURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> GetIndustryAsync()
        {
            var access_token = await _lazyWeChatBasic.GetAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.GETINDUSTRYURL, access_token);
            var returnObject = await _httpRepository.GetParseAsync(requestUrl);
            return returnObject;
        }

        public async Task<dynamic> GetTemplateIDAsync(string template_short_name)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.template_id_short = template_short_name;

            var returnObject = await SendRequest(requestObject, CONSTANT.GETTEMPLATEIDURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> GetTemplateListAsync()
        {
            var returnObject = await SendRequest(null, CONSTANT.GETTEMPLATELISTURL, HttpMethod.Get, "template_list");
            return returnObject;
        }

        public async Task<dynamic> DeleteTemplateAsync(string template_id)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.template_id = template_id;

            var returnObject = await SendRequest(requestObject, CONSTANT.DELETETEMPLATEURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> SendTemplateMessageAsync(string touser, string template_id, string url, params (string, string, string)[] data)
        {
            dynamic requestObject = GenerateTemplateModel(touser, template_id, data);
            requestObject.url = url;

            var returnObject = await SendRequest(requestObject, CONSTANT.SENDTEMPLATEMESSAGEURL, HttpMethod.Post, "msgid");
            return returnObject;
        }

        public async Task<dynamic> SendTemplateMessageAsync(string touser, string template_id, string appid, string pagepath, params (string, string, string)[] data)
        {
            dynamic requestObject = GenerateTemplateModel(touser, template_id, data);
            requestObject.miniprogram = new ExpandoObject();
            requestObject.miniprogram.appid = appid;
            requestObject.miniprogram.page = pagepath;

            var returnObject = await SendRequest(requestObject, CONSTANT.SENDTEMPLATEMESSAGEURL, HttpMethod.Post, "msgid");
            return returnObject;
        }

        private dynamic GenerateTemplateModel(string touser, string template_id, (string, string, string)[] data)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.touser = touser;
            requestObject.template_id = template_id;
            requestObject.data = new ExpandoObject();
            SortedDictionary<string, dynamic> dict = new SortedDictionary<string, dynamic>();
            for (int i = 0; i < data.Length; i++)
            {
                dynamic item = new ExpandoObject();
                item.value = data[i].Item2;
                if (!string.IsNullOrEmpty(data[i].Item3))
                    item.color = data[i].Item3;
                dict.Add(data[i].Item1, item);
            }
            var json = JsonConvert.SerializeObject(dict);
            requestObject.data = UtilRepository.ParseAPIResult(json);
            return requestObject;
        }
    }
}
