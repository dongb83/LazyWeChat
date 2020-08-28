using LazyWeChat.Abstract.QY;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.QY
{
    public static partial class CONSTANT
    {
        public const string CREATEDEPTURL = "https://qyapi.weixin.qq.com/cgi-bin/department/create?access_token={0}";

        public const string UPDATEDEPTURL = "https://qyapi.weixin.qq.com/cgi-bin/department/update?access_token={0}";

        public const string DELETEDEPTURL = "https://qyapi.weixin.qq.com/cgi-bin/department/delete?access_token={0}&id={1}";

        public const string GETDEPTSURL = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}";

        public const string CREATETAGURL = "https://qyapi.weixin.qq.com/cgi-bin/tag/create?access_token={0}";

        public const string UPDATETAGURL = "https://qyapi.weixin.qq.com/cgi-bin/tag/update?access_token={0}";

        public const string DELETETAGURL = "https://qyapi.weixin.qq.com/cgi-bin/tag/delete?access_token={0}&tagid={1}";

        public const string GETTAGUSERSURL = "https://qyapi.weixin.qq.com/cgi-bin/tag/get?access_token={0}&tagid={1}";

        public const string ADDUSERSFORTAGURL = "https://qyapi.weixin.qq.com/cgi-bin/tag/addtagusers?access_token={0}";

        public const string DeleteUSERSFORTAGURL = "https://qyapi.weixin.qq.com/cgi-bin/tag/deltagusers?access_token={0}";

        public const string GETTAGSURL = "https://qyapi.weixin.qq.com/cgi-bin/tag/list?access_token={0}";
    }

    public partial class LazyQYContact : ILazyQYContact
    {
        #region Department
        public virtual async Task<dynamic> CreateDeptAsync(string name, string name_en, int parentid, int order, int? id)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.name = name;
            requestObject.name_en = name_en;
            requestObject.parentid = parentid;
            requestObject.order = order;
            if (id != null)
                requestObject.id = id.Value;

            return await SendContactRequest(requestObject, CONSTANT.CREATEDEPTURL, HttpMethod.Post, "id");
        }

        public virtual async Task<dynamic> UpdateDeptAsync(int id, string name, string name_en, int parentid, int order)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.id = id;
            requestObject.name = name;
            requestObject.name_en = name_en;
            requestObject.parentid = parentid;
            requestObject.order = order;

            return await SendContactRequest(requestObject, CONSTANT.UPDATEDEPTURL, HttpMethod.Post);
        }

        public virtual async Task<dynamic> DeleteDeptAsync(int id)
        {
            var access_token = await _lazyQYBasic.GetContactAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.DELETEDEPTURL, access_token, id);

            return await _httpRepository.GetParseValidateAsync(requestUrl);
        }

        public virtual async Task<dynamic> GetDeptsAsync(int? id)
        {
            var access_token = await _lazyQYBasic.GetContactAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.GETDEPTSURL, access_token);
            if (id != null)
                requestUrl = string.Concat(requestUrl, $"&id={id}");

            return await _httpRepository.GetParseValidateAsync(requestUrl, "department");
        }
        #endregion

        #region Tags
        public virtual async Task<dynamic> CreateTagAsync(int? tagid, string tagname)
        {
            dynamic requestObject = new ExpandoObject();
            if (tagid != null)
                requestObject.tagid = tagid;
            requestObject.tagname = tagname;

            return await SendContactRequest(requestObject, CONSTANT.CREATETAGURL, HttpMethod.Post, "tagid");
        }

        public virtual async Task<dynamic> UpdateTagAsync(int tagid, string tagname)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.tagid = tagid;
            requestObject.tagname = tagname;

            return await SendContactRequest(requestObject, CONSTANT.UPDATETAGURL, HttpMethod.Post);
        }

        public virtual async Task<dynamic> DeleteTagAsync(int tagid)
        {
            var access_token = await _lazyQYBasic.GetContactAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.DELETETAGURL, access_token, tagid);

            return await _httpRepository.GetParseValidateAsync(requestUrl);
        }

        public virtual async Task<dynamic> GetTagUsersAsync(int tagid)
        {
            var access_token = await _lazyQYBasic.GetContactAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.GETTAGUSERSURL, access_token, tagid);

            return await _httpRepository.GetParseValidateAsync(requestUrl, "userlist");
        }

        public virtual async Task<dynamic> AddUsersforTagAsync(int tagid, params string[] userlist) => await SendContactRequest(new { tagid = tagid, userlist = userlist }, CONSTANT.ADDUSERSFORTAGURL, HttpMethod.Post);

        public virtual async Task<dynamic> AddUsersforTagAsync(int tagid, params int[] partylist) => await SendContactRequest(new { tagid = tagid, userlist = partylist }, CONSTANT.ADDUSERSFORTAGURL, HttpMethod.Post);

        public virtual async Task<dynamic> DeleteUsersforTagAsync(int tagid, params string[] userlist) => await SendContactRequest(new { tagid = tagid, userlist = userlist }, CONSTANT.DeleteUSERSFORTAGURL, HttpMethod.Post);

        public virtual async Task<dynamic> DeleteUsersforTagAsync(int tagid, params int[] partylist) => await SendContactRequest(new { tagid = tagid, partylist = partylist }, CONSTANT.DeleteUSERSFORTAGURL, HttpMethod.Post);

        public virtual async Task<dynamic> GetTagsAsync()
        {
            var access_token = await _lazyQYBasic.GetContactAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.GETTAGSURL, access_token);

            return await _httpRepository.GetParseValidateAsync(requestUrl, "taglist");
        }
        #endregion
    }
}
