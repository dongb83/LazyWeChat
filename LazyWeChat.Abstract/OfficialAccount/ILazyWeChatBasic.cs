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
        /// 获取公众号的全局唯一接口调用凭据 access_token
        /// </summary>
        /// <returns>access_token</returns>
        Task<string> GetAccessTokenAsync();

        /// <summary>
        /// 向用户发起请求授权, 并获取授权code
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="scope">
        /// 应用授权作用域
        /// </param>
        /// <returns>授权code</returns>
        string GetAuthorizationCode(HttpContext context, SCOPE scope);

        /// <summary>
        /// 微信开放平台获取授权code
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="openAppID">开发平台应用中的AppID</param>
        /// <returns>授权code</returns>
        string GetQRConnectCode(HttpContext context, string openAppID);

        /// <summary>
        /// 获取用来换取用户相关微信信息的网页授权access_token
        /// </summary>
        /// <param name="code">调用GetAuthorizationCode获得的code</param>
        /// <returns></returns>
        Task<dynamic> GetWebAccessTokenAsync(string code);

        /// <summary>
        /// 开放平台获取用来换取用户相关微信信息的网页授权access_token
        /// </summary>
        /// <param name="code">调用GetAuthorizationCode获得的code</param>
        /// <param name="openAppID">开发平台应用中的AppID</param>
        /// <param name="openAppSecret">开发平台应用中的AppSecret</param>
        /// <returns></returns>
        Task<dynamic> GetOpenWebAccessTokenAsync(string code, string openAppID, string openAppSecret);

        /// <summary>
        /// 用来检验授权凭证（access_token）是否有效
        /// </summary>
        /// <param name="access_token">网页授权接口调用凭证,注意:此access_token与基础支持的access_token不同</param>
        /// <param name="openid">用户的唯一标识</param>
        /// <returns></returns>
        Task<bool> ValidateWebAccessTokenAsync(string access_token, string openid);

        /// <summary>
        /// 通过access_token和openid拉取用户信息
        /// </summary>
        /// <param name="access_token">网页授权接口调用凭证,注意:此access_token与基础支持的access_token不同</param>
        /// <param name="openid">用户的唯一标识</param>
        /// <param name="lang">返回国家地区语言版本</param>
        /// <returns></returns>
        Task<dynamic> GetUserInfoAsync(string access_token, string openid, string lang);

        /// <summary>
        /// 获取微信callback IP地址
        /// </summary>
        /// <returns>IP list</returns>
        Task<List<string>> GetWeChatCallbackIPListAsync();

        /// <summary>
        /// 获取微信API接口 IP地址
        /// </summary>
        /// <returns>IP list</returns>
        Task<List<string>> GetWeChatAPIIPListAsync();

        /// <summary>
        /// 获取用于调用微信JS接口的临时票据
        /// </summary>
        /// <returns></returns>
        Task<string> GetJSTicketAsync();

        /// <summary>
        /// 用于生成wx.config的JS脚本
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="debug">debug模式</param>
        /// <param name="jsApiList">需要使用的JS接口列表</param>
        /// <returns></returns>
        Task<string> GenerateWXConfigScriptAsync(HttpContext context, bool debug, params string[] jsApiList);

        /// <summary>
        /// 用于生成wx.config的JS脚本
        /// </summary>
        /// <param name="requestUrl">当前页面URL</param>
        /// <param name="debug">debug模式</param>
        /// <param name="jsApiList">需要使用的JS接口列表</param>
        /// <returns></returns>
        Task<string> GenerateWXConfigScriptAsync(string requestUrl, bool debug, params string[] jsApiList);

        /// <summary>
        /// 为公众号中的用户创建标签
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
        /// <param name="tagId">需要编辑的tagId</param>
        /// <param name="tagName">修改后的tagName</param>
        /// <returns></returns>
        Task<dynamic> EditTagAsync(string tagId, string tagName);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagId">需要删除的tagId</param>
        /// <returns></returns>
        Task<dynamic> DeleteTagAsync(string tagId);

        /// <summary>
        /// 获取标签下粉丝列表
        /// </summary>
        /// <param name="tagId">tagId</param>
        /// <param name="next_openid">第一个拉取的OPENID，不填默认从头开始拉取</param>
        /// <returns></returns>
        Task<dynamic> GetTagUsersAsync(string tagId, string next_openid);

        /// <summary>
        /// 批量为用户打标签
        /// </summary>
        /// <param name="tagId">tagId</param>
        /// <param name="openids">要设置tag的用户openid数组</param>
        /// <returns></returns>
        Task<dynamic> SetTagforUsersAsync(string tagId, params string[] openids);

        /// <summary>
        /// 批量为用户取消标签
        /// </summary>
        /// <param name="tagId">tagId</param>
        /// <param name="openids">要设置tag的用户openid数组</param>
        /// <returns></returns>
        Task<dynamic> RemoveTagforUsersAsync(string tagId, params string[] openids);

        /// <summary>
        /// 获取用户身上的标签列表
        /// </summary>
        /// <param name="openid">openid</param>
        /// <returns></returns>
        Task<dynamic> GetUserTagsAsync(string openid);

        /// <summary>
        /// 指定用户设置备注名
        /// </summary>
        /// <param name="remark">remark</param>
        /// <param name="openid">openid</param>
        /// <returns></returns>
        Task<dynamic> SetCommentsforUsersAsync(string remark, string openid);

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="openid">openid</param>
        /// <param name="lang">语言</param>
        /// <returns></returns>
        Task<dynamic> GetUserDetailsAsync(string openid, string lang);

        /// <summary>
        /// 批量获取用户基本信息
        /// </summary>
        /// <param name="user_list">一个元组的列表,元组中的第一个元素为openid, 第二个元素为language, 指定该用户需要获取的语言信息</param>
        /// <returns></returns>
        Task<dynamic> GetUsersDetailsAsync(List<(string, string)> user_list);

        /// <summary>
        /// 获取帐号的关注者列表
        /// </summary>
        /// <param name="next_openid">第一个拉取的OPENID，不填默认从头开始拉取</param>
        /// <returns></returns>
        Task<dynamic> GetUserListAsync(string next_openid);

        /// <summary>
        /// 获取公众号的黑名单列表
        /// </summary>
        /// <param name="begin_openid">begin_openid 为空时，默认从开头拉取</param>
        /// <returns></returns>
        Task<dynamic> GetBlacklistAsync(string begin_openid);

        /// <summary>
        /// 拉黑用户
        /// </summary>
        /// <param name="openid_list">需要拉入黑名单的用户的openid，一次拉黑最多允许20个</param>
        /// <returns></returns>
        Task<dynamic> SetBlackUsersAsync(params string[] openid_list);

        /// <summary>
        /// 取消拉黑用户
        /// </summary>
        /// <param name="openid_list">需要取消拉入黑名单的用户的openid，一次拉黑最多允许20个</param>
        /// <returns></returns>
        Task<dynamic> CancelBlackUsersAsync(params string[] openid_list);

        /// <summary>
        /// 用来生成事件推送的二维码
        /// </summary>
        /// <typeparam name="T">场景类型string或者int</typeparam>
        /// <param name="tempOrNot">是否临时二维码</param>
        /// <param name="expire_seconds">临时二维码的过期时间</param>
        /// <param name="scene_value">场景值</param>
        /// <returns>二维码图片链接地址</returns>
        Task<string> GenerateQRCodeAsync<T>(bool tempOrNot, int expire_seconds, T scene_value);

        /// <summary>
        /// 将一条长链接转成短链接。
        /// </summary>
        /// <param name="long_url">原始URL</param>
        /// <returns>短连接</returns>
        Task<string> ShortUrlAsync(string long_url);

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="menuButton">菜单结构</param>
        /// <returns></returns>
        Task<dynamic> CreateMenuAsync(MenuButton menuButton);

        /// <summary>
        /// 获取当前菜单结构
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetCurrentMenuAsync();

        /// <summary>
        /// 删除当前菜单
        /// </summary>
        /// <returns></returns>
        Task<dynamic> DeleteMenuAsync();
    }
}
