using LazyWeChat.Abstract;
using LazyWeChat.Models.Exception;
using LazyWeChat.Utility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation
{
    public class HttpRepository : IHttpRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HttpRepository> _logger;

        public HttpRepository(
            IHttpClientFactory clientFactory,
            ILogger<HttpRepository> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public virtual async Task<string> GetAsync(string url)
        {
            dynamic resultObject = new ExpandoObject();
            resultObject.requestID = UtilRepository.GenerateRandomCode();
            resultObject.method = "GET";
            resultObject.url = url;
            resultObject.createdAt = DateTime.Now.ToString();
            string json = "";

            using (var client = _clientFactory.CreateClient())
            {
                var response = await client.GetAsync(url);
                resultObject.statusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    resultObject.result = "success";
                    resultObject.resultContent = content;
                    json = JsonConvert.SerializeObject(resultObject);
                    _logger.LogInformation(json);
                    return content;
                }
                else
                {
                    resultObject.result = "fail";
                    resultObject.resultContent = "BadHttpResponseException";
                    json = JsonConvert.SerializeObject(resultObject);
                    _logger.LogInformation(json);
                    throw new BadHttpResponseException(url, response.StatusCode);
                }
            }
        }

        public virtual async Task<string> PostAsync(string url, string requestContent)
        {
            dynamic resultObject = new ExpandoObject();
            resultObject.requestID = UtilRepository.GenerateRandomCode();
            resultObject.method = "POST";
            resultObject.url = url;
            resultObject.requestContent = requestContent;
            resultObject.createdAt = DateTime.Now.ToString();
            string json = "";

            using (var client = _clientFactory.CreateClient())
            {
                using (HttpContent httpContent = new StringContent(requestContent))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await client.PostAsync(url, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        resultObject.result = "success";
                        resultObject.resultContent = content;
                        json = JsonConvert.SerializeObject(resultObject);
                        _logger.LogInformation(json);
                        return content;
                    }
                    else
                    {
                        resultObject.result = "fail";
                        resultObject.resultContent = "BadHttpResponseException";
                        json = JsonConvert.SerializeObject(resultObject);
                        _logger.LogInformation(json);
                        throw new BadHttpResponseException(url, response.StatusCode);
                    }
                }
            }
        }

        public virtual async Task<dynamic> GetParseAsync(string url) => Pasre(await GetAsync(url));

        public virtual async Task<dynamic> PostParseAsync(string url, string requestContent) => Pasre(await PostAsync(url, requestContent));

        private dynamic Pasre(string content)
        {
            dynamic returnObject = new ExpandoObject();
            if (content.StartsWith("{") && content.EndsWith("}"))
                returnObject = UtilRepository.ParseAPIResult(content);
            else
                returnObject = content.FromXml().ToDynamic();

            return returnObject;
        }

        public virtual async Task<dynamic> GetParseValidateAsync(string url, params string[] validationNames)
        {
            var returnObject = await GetParseAsync(url);
            Validate(returnObject, validationNames);
            return returnObject;
        }

        public virtual async Task<dynamic> PostParseValidateAsync(string url, string requestContent, params string[] validationNames)
        {
            var returnObject = await PostParseAsync(url, requestContent);
            Validate(returnObject, validationNames);
            return returnObject;
        }

        public virtual void Validate(dynamic returnObj, params string[] validationNames)
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

        public virtual async Task<dynamic> SendRequestAsync(dynamic requestObject, string requestUrl, HttpMethod method, string accessToken, params string[] validationNames)
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

        private async Task<string> UploadFile(string url, byte[] file, string fileName)
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

        public virtual async Task<string> UploadFileAsync(string requestUrl, string fileName) => await Task<string>.Run(() => HttpUploadFile(requestUrl, fileName));

        public virtual async Task<string> UploadFileAsync(string requestUrl, string fileName, string formName) => await Task<string>.Run(() => HttpUploadFile(requestUrl, fileName, formName));

        public virtual async Task<string> UploadFileAsync(string requestUrl, string fileName, string formName, string additionInfo) => await Task<string>.Run(() => HttpUploadFile(requestUrl, fileName, formName, additionInfo));

        string HttpUploadFile(string url, string path, string formName = "file", string additionInfo = "")
        {
            dynamic resultObject = new ExpandoObject();
            resultObject.requestID = UtilRepository.GenerateRandomCode();
            resultObject.method = "POST";
            resultObject.url = url;
            resultObject.requestContent = path;
            resultObject.createdAt = DateTime.Now.ToString();

            string content = "";

            try
            {
                #region Upload
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
                byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                int pos = path.LastIndexOf("\\");
                string fileName = path.Substring(pos + 1);

                StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"" + formName + "\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bArr = new byte[fs.Length];
                fs.Read(bArr, 0, bArr.Length);
                fs.Close();

                Stream postStream = request.GetRequestStream();
                postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                postStream.Write(bArr, 0, bArr.Length);
                byte[] bSplit = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
                postStream.Write(bSplit, 0, bSplit.Length);

                if (!string.IsNullOrEmpty(additionInfo))
                {
                    byte[] additionBytes = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"description\";\r\n\r\n");
                    postStream.Write(additionBytes, 0, additionBytes.Length);
                    byte[] additionContentBytes = Encoding.UTF8.GetBytes(additionInfo);
                    postStream.Write(additionContentBytes, 0, additionContentBytes.Length);
                    postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                }

                postStream.Close();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                content = sr.ReadToEnd();
                #endregion

                resultObject.result = "success";
                resultObject.resultContent = content;
                string json = JsonConvert.SerializeObject(resultObject);
                _logger.LogInformation(json);
            }
            catch (Exception ex)
            {
                resultObject.result = "fail";
                resultObject.resultContent = ex.Message;
                string json = JsonConvert.SerializeObject(resultObject);
                _logger.LogInformation(json);
                throw ex;
            }

            return content;
        }
    }
}
