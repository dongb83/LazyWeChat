using LazyWeChat.Abstract;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Utility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.OfficialAccount
{
    public static partial class CONSTANT
    {
        public const string UPLOADTEMPMATERIALURL = "https://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}";

        public const string UPLOADMATERIALURL = "https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}&type={1}";

        public const string GETMATERTIALSURL = "https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}";

        public const string GETMATERTIALCOUNTURL = "https://api.weixin.qq.com/cgi-bin/material/get_materialcount?access_token={0}";
    }

    public class LazyMaterials : ILazyMaterials
    {
        private readonly IHttpRepository _httpRepository;
        private readonly ILazyWeChatBasic _lazyWeChatBasic;
        private readonly ILogger<LazyMaterials> _logger;

        public LazyMaterials(IHttpRepository httpRepository,
            ILazyWeChatBasic lazyWeChatBasic,
            ILogger<LazyMaterials> logger)
        {
            _httpRepository = httpRepository;
            _lazyWeChatBasic = lazyWeChatBasic;
            _logger = logger;
        }

        async Task<dynamic> SendRequest(dynamic requestObject, string requestUrl, HttpMethod method, params string[] validationNames)
        {
            var accessToken = await _lazyWeChatBasic.GetAccessTokenAsync();
            return await _httpRepository.SendRequest(requestObject, requestUrl, method, accessToken, validationNames);
        }

        public async Task<dynamic> UploadTempMaterialAsync(string fullFilePath, MediaType mediaType)
            => await RequestUploadAPI(fullFilePath, CONSTANT.UPLOADTEMPMATERIALURL, mediaType);

        public async Task<dynamic> UploadMaterialAsync(string fullFilePath, MediaType mediaType)
            => await RequestUploadAPI(fullFilePath, CONSTANT.UPLOADMATERIALURL, mediaType);

        private async Task<dynamic> RequestUploadAPI(string fullFilePath, string requestAPIUrl, MediaType mediaType)
        {
            string requestUrl = await ValidateMaterial(fullFilePath, requestAPIUrl, mediaType);
            var returnObject = await _httpRepository.UploadFile(requestUrl, fullFilePath);
            return returnObject;
        }

        private async Task<string> ValidateMaterial(string fullFilePath, string url, MediaType mediaType)
        {
            var file = new FileInfo(fullFilePath);
            var extension = file.Extension.ToLower();
            byte[] buffur;
            using (FileStream fs = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
            {
                buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);
            }

            var fileSize = Math.Ceiling(file.Length / 1024.0);
            var exceptionExtension = false;
            var exceptionSize = false;
            switch (mediaType)
            {
                case MediaType.image:
                    if (!(extension == ".bmp" || extension == ".png" || extension == ".jpeg" || extension == ".jpg" || extension == ".gif"))
                        exceptionExtension = true;
                    else
                    {
                        if (fileSize > 10 * 1024)
                            exceptionSize = true;
                    }
                    break;
                case MediaType.voice:
                    if (!(extension == ".mp3" || extension == ".wma" || extension == ".wav" || extension == ".amr"))
                        exceptionExtension = true;
                    else
                    {
                        if (fileSize > 2 * 1024)
                            exceptionSize = true;
                    }
                    break;
                case MediaType.video:
                    if (!(extension == ".mp4"))
                        exceptionExtension = true;
                    else
                    {
                        if (fileSize > 10 * 1024)
                            exceptionSize = true;
                    }
                    break;
                case MediaType.thumb:
                    if (!(extension == ".jpg"))
                        exceptionExtension = true;
                    else
                    {
                        if (fileSize > 64)
                            exceptionSize = true;
                    }
                    break;
                default:
                    break;
            }
            if (exceptionExtension)
                throw new Exception("Invalid file extension");

            if (exceptionSize)
                throw new Exception("file size overflow");

            var access_token = await _lazyWeChatBasic.GetAccessTokenAsync();
            var requestUrl = string.Format(url, access_token, mediaType.ToString());
            return requestUrl;
        }

        public async Task<dynamic> GetMaterialsAsync(string type, int offset, int count)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.type = type;
            requestObject.offset = offset;
            requestObject.count = count;

            var returnObject = await SendRequest(requestObject, CONSTANT.GETMATERTIALSURL, HttpMethod.Post, "item");
            return returnObject;
        }

        public async Task<dynamic> GetMaterialsCountAsync()
        {
            var returnObject = await _httpRepository.PostParseValidateAsync(CONSTANT.GETMATERTIALCOUNTURL, "",
                "voice_count", "video_count", "image_count", "news_count");
            return returnObject;
        }
    }
}
