using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract
{
    public interface IHttpRepository
    {
        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns>请求结果</returns>
        Task<string> GetAsync(string url);

        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="requestContent">Post的Body内容</param>
        /// <returns>请求结果</returns>
        Task<string> PostAsync(string url, string requestContent);

        /// <summary>
        /// 发送Get请求并将请求结果转换为dynamic类型
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns>请求结果(dynamic类型)</returns>
        Task<dynamic> GetParseAsync(string url);

        /// <summary>
        /// 发送Post请求并将请求结果转换为dynamic类型
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="requestContent"></param>
        /// <returns>请求结果(dynamic类型)</returns>
        Task<dynamic> PostParseAsync(string url, string requestContent);

        /// <summary>
        /// 发送Get请求,转换结果为dynamic并验证结果是否存在某些字段
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="validationName">要验证的字段</param>
        /// <returns>请求结果(dynamic类型)</returns>
        Task<dynamic> GetParseValidateAsync(string url, params string[] validationName);

        /// <summary>
        /// 发送Post请求,转换结果为dynamic并验证结果是否存在某些字段
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="requestContent">Post的Body内容</param>
        /// <param name="validationNames">要验证的字段</param>
        /// <returns>请求结果(dynamic类型)</returns>
        Task<dynamic> PostParseValidateAsync(string url, string requestContent, params string[] validationNames);

        /// <summary>
        /// 发送Http请求,转换结果为dynamic并验证结果是否存在某些字段
        /// </summary>
        /// <param name="requestObject">Body内容的dynamic格式</param>
        /// <param name="requestUrl">请求的URL</param>
        /// <param name="method">HttpMethod</param>
        /// <param name="accessToken">access_token</param>
        /// <param name="validationNames">要验证的字段</param>
        /// <returns></returns>
        Task<dynamic> SendRequestAsync(dynamic requestObject, string requestUrl, HttpMethod method, string accessToken, params string[] validationNames);

        /// <summary>
        /// 验证某个dynamic类型是否存在某些字段
        /// </summary>
        /// <param name="returnObj">被验证的dynamic实例</param>
        /// <param name="validationNames">要验证的字段</param>
        void Validate(dynamic returnObj, params string[] validationNames);

        /// <summary>
        /// 发送Post请求上传文件
        /// </summary>
        /// <param name="requestUrl">请求的URL</param>
        /// <param name="fileName">要上传的文件的完整路径</param>
        /// <returns>请求结果</returns>
        Task<string> UploadFileAsync(string requestUrl, string fileName);

        /// <summary>
        /// 发送Post请求上传文件
        /// </summary>
        /// <param name="requestUrl">请求的URL</param>
        /// <param name="fileName">要上传的文件的完整路径</param>
        /// <param name="formName">Post表单的name属性</param>
        /// <returns>请求结果</returns>
        Task<string> UploadFileAsync(string requestUrl, string fileName, string formName);

        /// <summary>
        /// 发送Post请求上传文件
        /// </summary>
        /// <param name="requestUrl">请求的URL</param>
        /// <param name="fileName">要上传的文件的完整路径</param>
        /// <param name="formName">Post表单的name属性</param>
        /// <param name="additionInfo">除了上传文件额外要Post给服务器的字符串</param>
        /// <returns>请求结果</returns>
        Task<string> UploadFileAsync(string requestUrl, string fileName, string formName, string additionInfo);
    }
}
