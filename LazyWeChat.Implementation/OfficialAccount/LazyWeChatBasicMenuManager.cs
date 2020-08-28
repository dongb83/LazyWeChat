using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models.OfficialAccount;
using System.Net.Http;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.OfficialAccount
{
    public static partial class CONSTANT
    {
        public const string CREATEMENUURL = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}";

        public const string GETCURRENTMENUURL = "https://api.weixin.qq.com/cgi-bin/get_current_selfmenu_info?access_token={0}";

        public const string DELETEMENUURL = "https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}";
    }

    public partial class LazyWeChatBasic : ILazyWeChatBasic
    {
        public virtual async Task<dynamic> CreateMenuAsync(MenuButton menuButton)
        {
            var access_token = await GetAccessTokenAsync();
            string requestUrl = string.Format(CONSTANT.CREATEMENUURL, access_token);
            var requestContent = menuButton.ToJson();
            var returnObject = await _httpRepository.PostParseValidateAsync(requestUrl, requestContent);
            return returnObject;
        }

        public virtual async Task<dynamic> GetCurrentMenuAsync()
        {
            var returnObject = await SendRequest(null, CONSTANT.GETCURRENTMENUURL, HttpMethod.Get, "is_menu_open");
            return returnObject;
        }

        public virtual async Task<dynamic> DeleteMenuAsync()
        {
            var returnObject = await SendRequest(null, CONSTANT.DELETEMENUURL, HttpMethod.Get);
            return returnObject;
        }
    }
}
