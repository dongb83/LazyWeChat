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
        /// <summary>
        /// 新增临时素材
        /// </summary>
        /// <param name="fullFilePath">素材完整路径</param>
        /// <param name="mediaType">素材类型</param>
        /// <returns></returns>
        Task<dynamic> UploadTempMaterialAsync(string fullFilePath, MediaType mediaType);

        /// <summary>
        /// 上传永久图文素材
        /// </summary>
        /// <param name="articles">图文素材内容</param>
        /// <returns></returns>
        Task<dynamic> CreateNewsMaterialAsync(List<ArticleModel> articles);

        /// <summary>
        /// 上传永久素材(image, voice, thumb)
        /// </summary>
        /// <param name="fullFilePath">素材完整路径</param>
        /// <param name="mediaType">素材类型</param>
        /// <returns></returns>
        Task<dynamic> UploadMaterialAsync(string fullFilePath, MediaType mediaType);

        /// <summary>
        /// 上传图文消息内的图片获取URL
        /// </summary>
        /// <param name="fullImgFilePath">图片完整路径</param>
        /// <returns></returns>
        Task<dynamic> UploadImgMaterialAsync(string fullImgFilePath);

        /// <summary>
        /// 上传永久素材(video)
        /// </summary>
        /// <param name="fullFilePath">视频完整路径</param>
        /// <param name="title">视频素材的标题</param>
        /// <param name="introduction">视频素材的描述</param>
        /// <returns></returns>
        Task<dynamic> UploadMaterialAsync(string fullFilePath, string title, string introduction);

        /// <summary>
        /// 获取临时素材
        /// </summary>
        /// <param name="media_id">media_id</param>
        /// <returns></returns>
        Task<object> GetTempMaterialAsync(string media_id);

        /// <summary>
        /// 获取永久素材
        /// </summary>
        /// <param name="media_id">media_id</param>
        /// <returns></returns>
        Task<object> GetMaterialAsync(string media_id);

        /// <summary>
        /// 获取永久素材的列表
        /// </summary>
        /// <param name="type">素材的类型</param>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 返回</param>
        /// <param name="count">返回素材的数量，取值在1到20之间</param>
        /// <returns></returns>
        Task<dynamic> GetMaterialsAsync(string type, int offset, int count);

        /// <summary>
        /// 获取永久素材的总数
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetMaterialsCountAsync();

        /// <summary>
        /// 修改永久图文素材
        /// </summary>
        /// <param name="media_id">要修改的图文消息的id</param>
        /// <param name="index">要更新的文章在图文消息中的位置(多图文消息时，此字段才有意义),第一篇为0</param>
        /// <param name="article">图文消息内容</param>
        /// <returns></returns>
        Task<dynamic> EditMaterialAsync(string media_id, int index, ArticleModel article);

        /// <summary>
        /// 删除素材
        /// </summary>
        /// <param name="media_id">media_id</param>
        /// <returns></returns>
        Task<dynamic> DeleteMaterialAsync(string media_id);
    }
}
