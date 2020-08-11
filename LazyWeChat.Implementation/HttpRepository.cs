using LazyWeChat.Abstract;
using LazyWeChat.Models.Exception;
using LazyWeChat.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation
{
    public class HttpRepository : IHttpRepository
    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpRepository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetAsync(string url)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    throw new BadHttpResponseException(url, response.StatusCode);
                }
            }
        }

        public async Task<string> PostAsync(string url, string requestContent)
        {
            using (var client = _clientFactory.CreateClient())
            {
                using (HttpContent httpContent = new StringContent(requestContent))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");//text/xml

                    var response = await client.PostAsync(url, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                    else
                    {
                        throw new BadHttpResponseException(url, response.StatusCode);
                    }
                }
            }
        }

        public async Task<dynamic> GetParseAsync(string url)
        {
            var content = await GetAsync(url);
            dynamic returnObject = new ExpandoObject();
            if (content.StartsWith("{") && content.EndsWith("}"))
                returnObject = UtilRepository.ParseAPIResult(content);
            else
                returnObject = content.FromXml().ToDynamic();

            return returnObject;
        }

        public async Task<dynamic> PostParseAsync(string url, string requestContent)
        {
            var content = await PostAsync(url, requestContent);
            dynamic returnObject = new ExpandoObject();
            if (content.StartsWith("{") && content.EndsWith("}"))
                returnObject = UtilRepository.ParseAPIResult(content);
            else
                returnObject = content.FromXml().ToDynamic();
            return returnObject;
        }

        public async Task<dynamic> GetParseValidateAsync(string url, params string[] validationNames)
        {
            var returnObject = await GetParseAsync(url);
            Validate(returnObject, validationNames);
            return returnObject;
        }

        public async Task<dynamic> PostParseValidateAsync(string url, string requestContent, params string[] validationNames)
        {
            var returnObject = await PostParseAsync(url, requestContent);
            Validate(returnObject, validationNames);
            return returnObject;
        }

        public void Validate(dynamic returnObj, params string[] validationNames)
        {
            if (validationNames.Length == 0)
            {
                var validate = false;
                if (UtilRepository.IsPropertyExist(returnObj, "errcode"))
                {
                    if (int.TryParse(returnObj.errcode, out int errcode))
                    {
                        validate = errcode == 0;
                    }
                }
                if (UtilRepository.IsPropertyExist(returnObj, "return_code"))
                {
                    validate = returnObj.return_code == "SUCCESS";
                }

                if (!validate)
                    throw new BadResultException(returnObj);
            }

            validationNames.ToList().ForEach(i =>
            {
                if (!UtilRepository.IsPropertyExist(returnObj, i))
                {
                    throw new BadResultException(returnObj);
                }
            });
        }

        public async Task<dynamic> SendRequest(dynamic requestObject, string requestUrl, HttpMethod method, string accessToken, params string[] validationNames)
        {
            var requestJson = requestObject == null ? "" : JsonConvert.SerializeObject(requestObject);
            var url = string.Format(requestUrl, accessToken);
            dynamic expandoObject = new ExpandoObject();
            if (method == HttpMethod.Post)
                expandoObject = await PostParseValidateAsync(url, requestJson, validationNames);
            else if (method == HttpMethod.Get)
                expandoObject = await GetParseValidateAsync(url, validationNames);
            else
                throw new Exception($"Request Method '{method}' is NOT supported");
            return expandoObject;
        }

        public async Task<string> UploadFile(string url, byte[] file, string fileName)
        {
            using (var client = _clientFactory.CreateClient())
            {
                using (MemoryStream memoryStream = new MemoryStream(file))
                {
                    using (var formData = new MultipartFormDataContent())
                    {
                        ByteArrayContent fileContent = new ByteArrayContent(file);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = fileName };

                        formData.Add(fileContent);

                        var response = await client.PostAsync(url, formData);
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            return result;
                        }
                        else
                        {
                            throw new BadHttpResponseException(url, response.StatusCode);
                        }
                    }
                }
            }
        }

        public async Task<string> UploadFile(string requestUrl, string fileName)
        {
            var result = await Task<string>.Run(() => HttpRequestRepository.HttpUploadFile(requestUrl, fileName));
            return result;
        }
    }
}
