using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract
{
    public interface IHttpRepository
    {
        Task<string> GetAsync(string url);

        Task<string> PostAsync(string url, string requestContent);

        Task<dynamic> GetParseAsync(string url);

        Task<dynamic> PostParseAsync(string url, string requestContent);

        Task<dynamic> GetParseValidateAsync(string url, params string[] validationName);

        Task<dynamic> PostParseValidateAsync(string url, string requestContent, params string[] validationNames);

        Task<dynamic> SendRequest(dynamic requestObject, string requestUrl, HttpMethod method, string accessToken, params string[] validationNames);

        void Validate(dynamic returnObj, params string[] validationNames);

        Task<string> UploadFile(string requestUrl, string fileName);

        Task<string> UploadFile(string requestUrl, string fileName, string formName);

        Task<string> UploadFile(string requestUrl, string fileName, string formName, string additionInfo);
    }
}
