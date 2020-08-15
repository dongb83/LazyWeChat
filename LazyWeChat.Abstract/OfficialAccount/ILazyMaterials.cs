using LazyWeChat.Models.OfficialAccount;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.OfficialAccount
{
    public enum MediaType
    {
        image,
        voice,
        video,
        thumb
    }

    public interface ILazyMaterials
    {
        Task<dynamic> UploadTempMaterialAsync(string fullFilePath, MediaType mediaType);

        Task<dynamic> UploadMaterialAsync(string fullFilePath, MediaType mediaType);

        Task<dynamic> UploadMaterialAsync(string fullFilePath, string title, string introduction);

        /// <summary>
        /// 如果返回的是视频消息素材，则内容如下：{ "video_url":DOWN_URL } 否则为二进制流
        /// </summary>
        /// <param name="media_id"></param>
        /// <returns></returns>
        Task<object> GetTempMaterialAsync(string media_id);

        /// <summary>
        /// 如果返回的是视频消息或者图文素材，则内容JSON 否则为二进制流
        /// </summary>
        /// <param name="media_id"></param>
        /// <returns></returns>
        Task<object> GetMaterialAsync(string media_id);

        Task<dynamic> UploadImgMaterialAsync(string fullImgFilePath);

        Task<dynamic> CreateNewsMaterialAsync(List<ArticleModel> articles);

        Task<dynamic> GetMaterialsAsync(string type, int offset, int count);

        Task<dynamic> GetMaterialsCountAsync();

        Task<dynamic> EditMaterialAsync(string media_id, int index, ArticleModel article);

        Task<dynamic> DeleteMaterialAsync(string media_id);
    }
}
