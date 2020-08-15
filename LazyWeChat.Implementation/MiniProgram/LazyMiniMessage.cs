using LazyWeChat.Abstract.MiniProgram;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models;
using LazyWeChat.Models.MiniProgram;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.MiniProgram
{
    public partial class LazyMiniBasic : ILazyMiniBasic
    {
        public async Task<dynamic> SendKFMessage(MiniKFMessage message) =>
            await SendRequest(message, CONSTANT.SENDKFMESSAGEURL, HttpMethod.Post);

        public async Task<dynamic> SendUniformMessage(UniformMessage message) =>
            await SendRequest(message, CONSTANT.UNIFORMMESSAGEURL, HttpMethod.Post);

        public async Task<dynamic> UploadTempMediaAsync(string fullFilePath) => await _lazyMaterials.UploadTempMaterialAsync(fullFilePath, MediaType.image);

        public async Task<byte[]> GetTempMediaAsync(string media_id)
        {
            var res = await _lazyMaterials.GetTempMaterialAsync(media_id);
            byte[] byteArray = Encoding.Default.GetBytes(res.ToString());
            return byteArray;
        }
    }
}
