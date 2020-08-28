using LazyWeChat.Abstract.OfficialAccount;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
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

        public const string GETUSERSDETAILSURL = "https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token={0}";

        public const string GETUSERLISTURL = "https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}";

        public const string GETBLACKLISTURL = "https://api.weixin.qq.com/cgi-bin/tags/members/getblacklist?access_token={0}";

        public const string BATCHBLACKLISTURL = "https://api.weixin.qq.com/cgi-bin/tags/members/batchblacklist?access_token={0}";

        public const string BATCHUNBLACKLISTURL = "https://api.weixin.qq.com/cgi-bin/tags/members/batchunblacklist?access_token={0}";
    }

    public partial class LazyWeChatBasic : ILazyWeChatBasic
    {
        public virtual async Task<dynamic> CreateTagAsync(string tagName)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.tag = new ExpandoObject();
            requestObject.tag.name = tagName;

            var returnObject = await SendRequest(requestObject, CONSTANT.CREATETAGURL, HttpMethod.Post, "tag");
            return returnObject;
        }

        public virtual async Task<dynamic> GetTagsAsync()
        {
            var returnObject = await SendRequest(null, CONSTANT.GETTAGSURL, HttpMethod.Get, "tags");
            return returnObject;
        }

        public virtual async Task<dynamic> EditTagAsync(string tagId, string tagName)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.tag = new ExpandoObject();
            requestObject.tag.id = tagId;
            requestObject.tag.name = tagName;

            var returnObject = await SendRequest(requestObject, CONSTANT.EDITTAGURL, HttpMethod.Post);
            return returnObject;
        }

        public virtual async Task<dynamic> DeleteTagAsync(string tagId)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.tag = new ExpandoObject();
            requestObject.tag.id = tagId;

            var returnObject = await SendRequest(requestObject, CONSTANT.DELETETAGURL, HttpMethod.Post);
            return returnObject;
        }

        public virtual async Task<dynamic> GetTagUsersAsync(string tagId, string next_openid)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.tagid = tagId;
            requestObject.next_openid = next_openid;

            var returnObject = await SendRequest(requestObject, CONSTANT.GETTAGUSERSURL, HttpMethod.Post, "data");
            return returnObject;
        }

        public virtual async Task<dynamic> SetTagforUsersAsync(string tagId, params string[] openids)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid_list = openids;
            requestObject.tagid = tagId;

            var returnObject = await SendRequest(requestObject, CONSTANT.SETTAGFORUSERSURL, HttpMethod.Post);
            return returnObject;
        }

        public virtual async Task<dynamic> RemoveTagforUsersAsync(string tagId, params string[] openids)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid_list = openids;
            requestObject.tagid = tagId;

            var returnObject = await SendRequest(requestObject, CONSTANT.REMOVETAGFROMUSERSURL, HttpMethod.Post);
            return returnObject;
        }

        public virtual async Task<dynamic> GetUserTagsAsync(string openid)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid = openid;

            var returnObject = await SendRequest(requestObject, CONSTANT.GETUSERTAGSURL, HttpMethod.Post, "tagid_list");
            return returnObject;
        }

        public virtual async Task<dynamic> SetCommentsforUsersAsync(string remark, string openid)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid = openid;
            requestObject.remark = remark;

            var returnObject = await SendRequest(requestObject, CONSTANT.SETUSERCOMMENTSURL, HttpMethod.Post);
            return returnObject;
        }

        public virtual async Task<dynamic> GetUserDetailsAsync(string openid, string lang)
        {
            var access_token = await GetAccessTokenAsync();
            string requestUrl = string.Format(CONSTANT.GETUSERDETAILSURL, access_token, openid, lang);

            var returnObject = await _httpRepository.GetParseValidateAsync(requestUrl, "nickname");
            return returnObject;
        }

        public virtual async Task<dynamic> GetUsersDetailsAsync(List<(string, string)> user_list)
        {
            var access_token = await GetAccessTokenAsync();
            string requestUrl = string.Format(CONSTANT.GETUSERSDETAILSURL, access_token);
            dynamic requestObject = new ExpandoObject();
            var list = new List<dynamic>();
            user_list.ForEach(i =>
            {
                var item = new { openid = i.Item1, lang = i.Item2 };
                list.Add(item);
            });
            requestObject.user_list = list;
            var requestContent = JsonConvert.SerializeObject(requestObject);

            var returnObject = await _httpRepository.PostParseValidateAsync(requestUrl, requestContent, "user_info_list");
            return returnObject;
        }

        public virtual async Task<dynamic> GetUserListAsync(string next_openid)
        {
            var access_token = await GetAccessTokenAsync();
            string requestUrl = string.Format(CONSTANT.GETUSERLISTURL, access_token, next_openid);

            var returnObject = await _httpRepository.GetParseValidateAsync(requestUrl, "data");
            return returnObject;
        }

        public virtual async Task<dynamic> GetBlacklistAsync(string begin_openid)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.begin_openid = begin_openid;

            var returnObject = await SendRequest(requestObject, CONSTANT.GETBLACKLISTURL, HttpMethod.Post, "total");
            return returnObject;
        }

        public virtual async Task<dynamic> SetBlackUsersAsync(params string[] openid_list)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid_list = openid_list;

            var returnObject = await SendRequest(requestObject, CONSTANT.BATCHBLACKLISTURL, HttpMethod.Post);
            return returnObject;
        }

        public virtual async Task<dynamic> CancelBlackUsersAsync(params string[] openid_list)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.openid_list = openid_list;

            var returnObject = await SendRequest(requestObject, CONSTANT.BATCHUNBLACKLISTURL, HttpMethod.Post);
            return returnObject;
        }
    }
}
