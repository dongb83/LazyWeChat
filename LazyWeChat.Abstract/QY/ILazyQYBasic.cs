using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models.OfficialAccount;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.QY
{
    public enum SCOPE
    {
        snsapi_base,
        snsapi_userinfo
    }

    public interface ILazyQYBasic
    {
        /// <summary>
        /// 获取访问用户身份
        /// </summary>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        Task<dynamic> GetUserInfoAsync(string code);

        /// <summary>
        /// 向用户发起请求授权, 并获取授权code
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>授权code</returns>
        string GetAuthorizationCode(HttpContext context);

        /// <summary>
        /// 企业微信API接口的调用凭据 
        /// </summary>
        /// <returns>access_token</returns>
        Task<string> GetAccessTokenAsync();

        /// <summary>
        /// 企业微信通讯录接口的调用凭据 
        /// </summary>
        /// <returns></returns>
        Task<string> GetContactAccessTokenAsync();

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="menuButton"></param>
        /// <returns></returns>
        Task<dynamic> CreateMenuAsync(MenuButton menuButton);

        /// <summary>
        /// 获取当前菜单
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetCurrentMenuAsync();

        /// <summary>
        /// 删除当前菜单
        /// </summary>
        /// <returns></returns>
        Task<dynamic> DeleteMenuAsync();

        /// <summary>
        /// 新增临时素材
        /// </summary>
        /// <param name="fullFilePath">素材完整路径</param>
        /// <param name="mediaType">素材类型</param>
        /// <returns></returns>
        Task<dynamic> UploadTempMaterialAsync(string fullFilePath, MediaType mediaType);
        
        /// <summary>
        /// 上传图文消息内的图片获取URL
        /// </summary>
        /// <param name="fullImgFilePath">图片完整路径</param>
        /// <returns></returns>
        Task<dynamic> UploadImgMaterialAsync(string fullImgFilePath);

        /// <summary>
        /// 获取临时素材
        /// </summary>
        /// <param name="media_id">media_id</param>
        /// <returns></returns>
        Task<object> GetTempMaterialAsync(string media_id);
    }
}
