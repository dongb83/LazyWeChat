using LazyWeChat.Abstract;
using LazyWeChat.Abstract.QY;
using LazyWeChat.Models;
using LazyWeChat.Models.QY;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.QY
{
    public static partial class CONSTANT
    {
        public const string ADDNEWMEMBERURL = "https://qyapi.weixin.qq.com/cgi-bin/user/create?access_token={0}";

        public const string UPDATENEWMEMBERURL = "https://qyapi.weixin.qq.com/cgi-bin/user/update?access_token={0}";

        public const string GETMEMBERURL = "https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}";

        public const string DELETEMEMBERURL = "https://qyapi.weixin.qq.com/cgi-bin/user/delete?access_token={0}&userid={1}";

        public const string BATCHDELETEMEMBERURL = "https://qyapi.weixin.qq.com/cgi-bin/user/batchdelete?access_token={0}";
    }

    public partial class LazyQYContact : ILazyQYContact
    {
        private readonly IOptions<LazyWeChatConfiguration> _options;
        private readonly ILazyQYBasic _lazyQYBasic;
        private readonly IHttpRepository _httpRepository;
        private readonly ILogger<LazyQYContact> _logger;
        private string _accessToken;
        private DateTime _expireAccessToken;
        private string _contactAccessToken;
        private DateTime _contactExpireAccessToken;

        public LazyQYContact(
            ILazyQYBasic lazyQYBasic,
            IOptions<LazyWeChatConfiguration> options,
            IHttpRepository httpRepository,
            ILogger<LazyQYContact> logger)
        {
            _options = options;
            _logger = logger;
            _httpRepository = httpRepository;
            _lazyQYBasic = lazyQYBasic;
        }

        async Task<dynamic> SendContactRequest(dynamic requestObject, string requestUrl, HttpMethod method, params string[] validationNames)
        {
            var accessToken = await _lazyQYBasic.GetContactAccessTokenAsync();
            return await _httpRepository.SendRequestAsync(requestObject, requestUrl, method, accessToken, validationNames);
        }

        public virtual async Task<dynamic> CreateMemberAsync(MemberModel member) => await SendContactRequest(member.ToDynamic(), CONSTANT.ADDNEWMEMBERURL, HttpMethod.Post);

        public virtual async Task<dynamic> UpdateMemberAsync(MemberModel member) => await SendContactRequest(member.ToDynamic(), CONSTANT.UPDATENEWMEMBERURL, HttpMethod.Post);

        public virtual async Task<dynamic> DeleteMemberAsync(string userid)
        {
            var access_token = await _lazyQYBasic.GetContactAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.DELETEMEMBERURL, access_token, userid);
            return await _httpRepository.GetParseAsync(requestUrl);
        }

        public virtual async Task<dynamic> GetMemberAsync(string userid)
        {
            var access_token = await _lazyQYBasic.GetContactAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.GETMEMBERURL, access_token, userid);
            return await _httpRepository.GetParseAsync(requestUrl);
        }

        public virtual async Task<dynamic> BatchDeleteMemberAsync(params string[] useridlist) => await SendContactRequest(new { useridlist = useridlist }, CONSTANT.BATCHDELETEMEMBERURL, HttpMethod.Post);
    }
}
