using LazyWeChat.Abstract;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Abstract.QY;
using LazyWeChat.Models;
using LazyWeChat.Models.OfficialAccount;
using LazyWeChat.Models.QY;
using LazyWeChat.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.QY
{
    public static partial class CONSTANT
    {
        public const string CREATEMENUURL = "https://qyapi.weixin.qq.com/cgi-bin/menu/create?access_token={0}&agentid={1}";

        public const string GETCURRENTMENUURL = "https://qyapi.weixin.qq.com/cgi-bin/menu/get?access_token={0}&agentid={1}";

        public const string DELETEMENUURL = "https://qyapi.weixin.qq.com/cgi-bin/menu/delete?access_token={0}&agentid={1}";

        public const string UPLOADTEMPMATERIALURL = "https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}";

        public const string UPLOADIMGURL = "https://qyapi.weixin.qq.com/cgi-bin/media/uploadimg?access_token={0}";

        public const string GETTEMPMATERIALURL = "https://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";
    }

    public partial class LazyQYBasic : ILazyQYBasic
    {
        public virtual async Task<dynamic> CreateMenuAsync(MenuButton menuButton)
        {
            var access_token = await GetAccessTokenAsync();
            string requestUrl = string.Format(CONSTANT.CREATEMENUURL, access_token, _options.Value.AppID);
            var requestContent = menuButton.ToJson();
            var returnObject = await _httpRepository.PostParseValidateAsync(requestUrl, requestContent);
            return returnObject;
        }

        public virtual async Task<dynamic> GetCurrentMenuAsync()
        {
            var access_token = await GetAccessTokenAsync();
            string requestUrl = string.Format(CONSTANT.GETCURRENTMENUURL, access_token, _options.Value.AppID);
            var returnObject = await _httpRepository.GetParseAsync(requestUrl);
            return returnObject;
        }

        public virtual async Task<dynamic> DeleteMenuAsync()
        {
            var access_token = await GetAccessTokenAsync();
            string requestUrl = string.Format(CONSTANT.DELETEMENUURL, access_token, _options.Value.AppID);
            var returnObject = await _httpRepository.GetParseValidateAsync(requestUrl);
            return returnObject;
        }

        public virtual async Task<dynamic> UploadTempMaterialAsync(string fullFilePath, MediaType mediaType)
            => await RequestUploadAPI(fullFilePath, CONSTANT.UPLOADTEMPMATERIALURL, mediaType, "media");

        private async Task<dynamic> RequestUploadAPI(
            string fullFilePath, string requestAPIUrl, MediaType mediaType, string formName = "file", string additionInfo = "")
        {
            string requestUrl = await ValidateMaterial(fullFilePath, requestAPIUrl, mediaType);
            var returnJson = await _httpRepository.UploadFileAsync(requestUrl, fullFilePath, formName, additionInfo);
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

            var access_token = await GetAccessTokenAsync();
            var requestUrl = string.Format(url, access_token, mediaType.ToString());
            return requestUrl;
        }

        public virtual async Task<dynamic> UploadImgMaterialAsync(string fullImgFilePath)
        {
            var accessToken = await GetAccessTokenAsync();
            var url = string.Format(CONSTANT.UPLOADIMGURL, accessToken);
            var returnJson = await _httpRepository.UploadFileAsync(url, fullImgFilePath);
            var returnObject = UtilRepository.ParseAPIResult(returnJson);
            _httpRepository.Validate(returnObject, "url");
            return returnObject;
        }

        public virtual async Task<object> GetTempMaterialAsync(string media_id)
        {
            var access_token = await GetAccessTokenAsync();
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
    }
}
