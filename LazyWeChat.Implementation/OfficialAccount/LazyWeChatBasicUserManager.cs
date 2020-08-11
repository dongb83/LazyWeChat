using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.OfficialAccount
{
    public static partial class CONSTANT
    {
        public const string CREATETAGURL = "https://api.weixin.qq.com/cgi-bin/tags/create?access_token={0}";

        public const string EDITTAGURL = "https://api.weixin.qq.com/cgi-bin/tags/update?access_token={0}";

        public const string DELETETAGURL = "https://api.weixin.qq.com/cgi-bin/tags/delete?access_token={0}";

        public const string GETTAGSURL = "https://api.weixin.qq.com/cgi-bin/tags/get?access_token={0}";

        public const string GETTAGUSERSURL = "https://api.weixin.qq.com/cgi-bin/user/tag/get?access_token={0}";

        public const string SETTAGFORUSERSURL = "https://api.weixin.qq.com/cgi-bin/tags/members/batchtagging?access_token={0}";

        public const string REMOVETAGFROMUSERSURL = "https://api.weixin.qq.com/cgi-bin/tags/members/batchuntagging?access_token={0}";

        public const string GETUSERTAGSURL = "https://api.weixin.qq.com/cgi-bin/tags/getidlist?access_token={0}";

        public const string SETUSERCOMMENTSURL = "https://api.weixin.qq.com/cgi-bin/user/info/updateremark?access_token={0}";

        public const string GETUSERDETAILSURL = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang={2}";

        public const string GETUSERLISTURL = "https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}";

        public const string GETBLACKLISTURL = "https://api.weixin.qq.com/cgi-bin/tags/members/getblacklist?access_token={0}";

        public const string BATCHBLACKLISTURL = "https://api.weixin.qq.com/cgi-bin/tags/members/batchunblacklist?access_token={0}";


        public const string BATCHUNBLACKLISTURL = "https://api.weixin.qq.com/cgi-bin/tags/members/batchunblacklist?access_token={0}"; 
    }

    public partial class LazyWeChatBasic : ILazyWeChatBasic
    {
        public async Task<dynamic> CreateTagAsync(string tagName)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.tag = new ExpandoObject();
            requestObject.tag.name = tagName;

            var returnObject = await SendRequest(requestObject, CONSTANT.CREATETAGURL, HttpMethod.Post, "tag");
            return returnObject;
        }

        public async Task<dynamic> GetTagsAsync()
        {
            var returnObject = await SendRequest(null, CONSTANT.GETTAGSURL, HttpMethod.Get, "tags");
            return returnObject;
        }

        public async Task<dynamic> EditTagAsync(string tagId, string tagName)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.tag = new ExpandoObject();
            requestObject.tag.id = tagId;
            requestObject.tag.name = tagName;

            var returnObject = await SendRequest(requestObject, CONSTANT.EDITTAGURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> DeleteTagAsync(string tagId)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.tag = new ExpandoObject();
            requestObject.tag.id = tagId;

            var returnObject = await SendRequest(requestObject, CONSTANT.DELETETAGURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> GetTagUsersAsync(string tagId, string next_openid)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.tagid = tagId;
            requestObject.next_openid = next_openid;

            var returnObject = await SendRequest(requestObject, CONSTANT.GETTAGUSERSURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> SetTagforUsersAsync(string tagId, params string[] openids)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid_list = openids;
            requestObject.tagid = tagId;

            var returnObject = await SendRequest(requestObject, CONSTANT.SETTAGFORUSERSURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> RemoveTagforUsersAsync(string tagId, params string[] openids)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid_list = openids;
            requestObject.tagid = tagId;

            var returnObject = await SendRequest(requestObject, CONSTANT.REMOVETAGFROMUSERSURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> GetUserTagsAsync(string openid)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid = openid;

            var returnObject = await SendRequest(requestObject, CONSTANT.GETUSERTAGSURL, HttpMethod.Post, "tagid_list");
            return returnObject;
        }

        public async Task<dynamic> SetCommentsforUsersAsync(string remark, string openid)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid = openid;
            requestObject.remark = remark;

            var returnObject = await SendRequest(requestObject, CONSTANT.SETUSERCOMMENTSURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> GetUserDetailsAsync(string openid, string lang)
        {
            var access_token = await GetAccessTokenAsync();
            string requestUrl = string.Format(CONSTANT.GETUSERDETAILSURL, access_token, openid, lang);

            var returnObject = await _httpRepository.GetParseValidateAsync(requestUrl, "nickname");
            return returnObject;
        }

        public async Task<dynamic> GetUserListAsync(string next_openid)
        {
            var access_token = await GetAccessTokenAsync();
            string requestUrl = string.Format(CONSTANT.GETUSERLISTURL, access_token, next_openid);

            var returnObject = await _httpRepository.GetParseValidateAsync(requestUrl, "data");
            return returnObject;
        }

        public async Task<dynamic> GetBlacklistAsync(string begin_openid)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.begin_openid = begin_openid;

            var returnObject = await SendRequest(requestObject, CONSTANT.GETBLACKLISTURL, HttpMethod.Post, "total");
            return returnObject;
        }

        public async Task<dynamic> SetBlackUsersAsync(params string[] openid_list)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid_list = openid_list;

            var returnObject = await SendRequest(requestObject, CONSTANT.BATCHBLACKLISTURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> CancelBlackUsersAsync(params string[] openid_list)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid_list = openid_list;

            var returnObject = await SendRequest(requestObject, CONSTANT.BATCHUNBLACKLISTURL, HttpMethod.Post);
            return returnObject;
        }
    }
}
