using LazyWeChat.Abstract;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models.OfficialAccount;
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

        public const string GETMATERIALSURL = "https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}";

        public const string GETMATERTIALCOUNTURL = "https://api.weixin.qq.com/cgi-bin/material/get_materialcount?access_token={0}";

        public const string GETMATERIALURL = "https://api.weixin.qq.com/cgi-bin/material/get_material?access_token";

        public const string GETTEMPMATERIALURL = "https://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";

        public const string UPLOADNEWSIMGURL = " https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token={0}";

        public const string UPLOADIMGURL = "https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token={0}";

        public const string CREATENEWSURL = "https://api.weixin.qq.com/cgi-bin/material/add_news?access_token={0}";

        public const string DELETEMATERIALURL = "https://api.weixin.qq.com/cgi-bin/material/del_material?access_token={0}";

        public const string EDITMATERIALURL = "https://api.weixin.qq.com/cgi-bin/material/update_news?access_token={0}";
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

        #region 新增素材
        public async Task<dynamic> UploadTempMaterialAsync(string fullFilePath, MediaType mediaType)
            => await RequestUploadAPI(fullFilePath, CONSTANT.UPLOADTEMPMATERIALURL, mediaType);

        public async Task<dynamic> UploadMaterialAsync(string fullFilePath, MediaType mediaType)
            => await RequestUploadAPI(fullFilePath, CONSTANT.UPLOADMATERIALURL, mediaType, "media");

        public async Task<dynamic> UploadMaterialAsync(string fullFilePath, string title, string introduction)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.title = title;
            requestObject.introduction = introduction;
            var json = JsonConvert.SerializeObject(requestObject);
            var res = await RequestUploadAPI(fullFilePath, CONSTANT.UPLOADMATERIALURL, MediaType.video, "media", json);
            return res;
        }

        public async Task<dynamic> UploadImgMaterialAsync(string fullImgFilePath)
        {
            var accessToken = await _lazyWeChatBasic.GetAccessTokenAsync();
            var url = string.Format(CONSTANT.UPLOADIMGURL, accessToken);
            var returnJson = await _httpRepository.UploadFile(url, fullImgFilePath);
            var returnObject = UtilRepository.ParseAPIResult(returnJson);
            _httpRepository.Validate(returnObject, "url");
            return returnObject;
        }

        public async Task<dynamic> CreateNewsMaterialAsync(List<ArticleModel> articles)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.articles = articles;

            var returnObject = await SendRequest(requestObject, CONSTANT.CREATENEWSURL, HttpMethod.Post, "media_id");
            return returnObject;
        }

        #region private

        private async Task<dynamic> RequestUploadAPI(
            string fullFilePath, string requestAPIUrl, MediaType mediaType, string formName = "file", string additionInfo = "")
        {
            string requestUrl = await ValidateMaterial(fullFilePath, requestAPIUrl, mediaType);
            var returnJson = await _httpRepository.UploadFile(requestUrl, fullFilePath, formName, additionInfo);
            var returnObject = UtilRepository.ParseAPIResult(returnJson);
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
        #endregion

        #endregion

        #region 获取素材
        public async Task<object> GetTempMaterialAsync(string media_id)
        {
            var access_token = await _lazyWeChatBasic.GetAccessTokenAsync();
            var url = string.Format(CONSTANT.GETTEMPMATERIALURL, access_token, media_id);
            var res = await _httpRepository.GetAsync(url);
            if (res.Contains("video_url"))
            {
                var returnObject = UtilRepository.ParseAPIResult(res);
                return returnObject;
            }
            else
            {
                byte[] byteArray = Encoding.Default.GetBytes(res);
                return byteArray;
            }
        }

        public async Task<dynamic> GetMaterialsAsync(string type, int offset, int count)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.type = type;
            requestObject.offset = offset;
            requestObject.count = count;

            var returnObject = await SendRequest(requestObject, CONSTANT.GETMATERIALSURL, HttpMethod.Post, "item");
            return returnObject;
        }

        public async Task<object> GetMaterialAsync(string media_id)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.media_id = media_id;

            var access_token = await _lazyWeChatBasic.GetAccessTokenAsync();
            var url = string.Format(CONSTANT.GETMATERIALURL, access_token, media_id);
            var json = JsonConvert.SerializeObject(requestObject);
            string res = await _httpRepository.PostAsync(url, json);
            if (res.Contains("down_url") || res.Contains("news_item"))
            {
                var returnObject = UtilRepository.ParseAPIResult(res);
                return returnObject;
            }
            else
            {
                byte[] byteArray = Encoding.Default.GetBytes(res);
                return byteArray;
            }
        }

        public async Task<dynamic> GetMaterialsCountAsync()
        {
            var returnObject = await _httpRepository.PostParseValidateAsync(CONSTANT.GETMATERTIALCOUNTURL, "",
                "voice_count", "video_count", "image_count", "news_count");
            return returnObject;
        }
        #endregion

        #region 修改、删除素材

        public async Task<dynamic> EditMaterialAsync(string media_id, int index, ArticleModel article)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.media_id = media_id;
            requestObject.index = index;
            requestObject.articles = article;

            var returnObject = await SendRequest(requestObject, CONSTANT.EDITMATERIALURL, HttpMethod.Post);
            return returnObject;
        }

        public async Task<dynamic> DeleteMaterialAsync(string media_id)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.media_id = media_id;

            var returnObject = await SendRequest(requestObject, CONSTANT.DELETEMATERIALURL, HttpMethod.Post);
            return returnObject;
        }
        #endregion
    }
}
