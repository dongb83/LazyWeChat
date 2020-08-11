using System;
using System.Collections.Generic;
using System.Text;
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

        Task<dynamic> GetMaterialsAsync(string type, int offset, int count);

        Task<dynamic> GetMaterialsCountAsync();
    }
}
