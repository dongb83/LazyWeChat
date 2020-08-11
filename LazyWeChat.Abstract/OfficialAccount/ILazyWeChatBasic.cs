using LazyWeChat.Models;
using LazyWeChat.Models.OfficialAccount;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.OfficialAccount
{
    public enum SCOPE
    {
        snsapi_base,
        snsapi_userinfo
    }

    public interface ILazyWeChatBasic
    {
        /// <summary>
        /// 用于获取公众号的全局唯一接口调用凭据
        /// https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/Get_access_token.html
        /// </summary>
        /// <returns></returns>
        Task<string> GetAccessTokenAsync();

        /// <summary>
        /// 获取用于换取用于身份认证的access token的code
        /// https://developers.weixin.qq.com/doc/offiaccount/OA_Web_Apps/Wechat_webpage_authorization.html
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="scope">
        /// 应用授权作用域，snsapi_base (不弹出授权页面，直接跳转，只能获取用户openid)
        /// snsapi_userinfo (弹出授权页面，可通过openid拿到昵称、性别、所在地。并且， 即使在未关注的情况下，只要用户授权，也能获取其信息 )
        /// </param>
        /// <returns></returns>
        string GetAuthorizationCode(HttpContext context, SCOPE scope);

        /// <summary>
        /// 用于换取网页授权access_token
        /// </summary>
        /// <param name="code">调用GetAuthorizationCode获得的code</param>
        /// <returns></returns>
        Task<dynamic> GetWebAccessTokenAsync(string code);

        /// <summary>
        /// 检验授权凭证（access_token）是否有效
        /// </summary>
        /// <param name="access_token">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同</param>
        /// <param name="openid">用户的唯一标识</param>
        /// <returns></returns>
        Task<bool> ValidateWebAccessTokenAsync(string access_token, string openid);

        /// <summary>
        /// 通过access_token和openid拉取用户信息
        /// </summary>
        /// <param name="access_token">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同</param>
        /// <param name="openid">用户的唯一标识</param>
        /// <param name="lang">返回国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语</param>
        /// <returns></returns>
        Task<dynamic> GetUserInfoAsync(string access_token, string openid, string lang);

        /// <summary>
        /// 获取微信callback IP地址
        /// callback IP即微信调用开发者服务器所使用的出口IP
        /// https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/Get_the_WeChat_server_IP_address.html
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetWeChatCallbackIPListAsync();

        /// <summary>
        /// 获取微信API接口 IP地址
        /// API接口IP即api.weixin.qq.com的解析地址，由开发者调用微信侧的接入IP
        /// https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/Get_the_WeChat_server_IP_address.html
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetWeChatAPIIPListAsync();

        /// <summary>
        /// 生成config接口注入权限验证配置所需要的JSTicket
        /// https://developers.weixin.qq.com/doc/offiaccount/OA_Web_Apps/JS-SDK.html#62
        /// </summary>
        /// <returns></returns>
        Task<string> GetJSTicketAsync();

        /// <summary>
        /// 所有需要使用JS-SDK的页面必须先注入配置信息，否则将无法调用,该方法用于生成配置信息
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="debug"></param>
        /// <param name="jsApiList"></param>
        /// <returns></returns>
        Task<string> GenerateWXConfigScriptAsync(HttpContext context, bool debug, params string[] jsApiList);

        /// <summary>
        /// 创建标签：一个公众号，最多可以创建100个标签。
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <returns></returns>
        Task<dynamic> CreateTagAsync(string tagName);

        /// <summary>
        /// 获取公众号已创建的标签
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetTagsAsync();

        /// <summary>
        /// 编辑标签
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        Task<dynamic> EditTagAsync(string tagId, string tagName);

        Task<dynamic> DeleteTagAsync(string tagId);

        Task<dynamic> SetTagforUsersAsync(string tagId, params string[] openids);

        Task<dynamic> SetCommentsforUsersAsync(string remark, string openid);

        Task<dynamic> GetUserDetailsAsync(string openid, string lang);

        Task<dynamic> GetUserListAsync(string next_openid);

        Task<dynamic> GetBlacklistAsync(string begin_openid);

        Task<dynamic> SetBlackUsersAsync(params string[] openid_list);

        Task<dynamic> CancelBlackUsersAsync(params string[] openid_list);
        /// <summary>
        /// 该方法返回带ticket的值的连接，若要在网页中显示，请参考如下例子
        /// <img src="https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQH27zoAAAAAAAAAASxodHRwOi8vd2VpeGluLnFxLmNvbS9xL3JqcFJmWGJsRlpsc0I4eHhseFNOAAIE64gXVwMEEA4AAA==" />
        /// 可以在中间件的onScanEvent中触发扫描改二维码的事件
        /// </summary>
        /// <typeparam name="T">场景类型string或者int</typeparam>
        /// <param name="tempOrNot">是否临时二维码</param>
        /// <param name="expire_seconds">临时二维码的过期时间</param>
        /// <param name="scene_value">场景值</param>
        /// <returns></returns>
        Task<string> GenerateQRCodeAsync<T>(bool tempOrNot, int expire_seconds, T scene_value) where T : struct;

        /// <summary>
        /// 将一条长链接转成短链接。
        /// 主要使用场景： 开发者用于生成二维码的原链接（商品、支付二维码等）太长导致扫码速度和成功率下降，将原长链接通过此接口转成短链接再生成二维码将大大提升扫码速度和成功率
        /// </summary>
        /// <param name="long_url"></param>
        /// <returns></returns>
        Task<string> ShortUrlAsync(string long_url);

        Task<dynamic> CreateMenuAsync(MenuButton menuButton);

        Task<dynamic> GetCurrentMenuAsync();

        Task<dynamic> DeleteMenuAsync();
    }
}
