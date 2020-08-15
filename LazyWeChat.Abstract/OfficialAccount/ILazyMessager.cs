using LazyWeChat.Models.OfficialAccount;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.OfficialAccount
{
    public interface ILazyMessager
    {
        Task<dynamic> SendKFMessage(WeChatKFTextMessage text);

        Task<dynamic> SendKFMessage(WeChatKFImageMessage image);

        Task<dynamic> SendKFMessage(WeChatKFVoiceMessage voice);

        Task<dynamic> SendKFMessage(WeChatKFVideoMessage video);

        Task<dynamic> SendKFMessage(WeChatKFMusicMessage music);

        Task<dynamic> SendKFMessage(WeChatKFExternalNewsMessage externalNews);

        Task<dynamic> SendKFMessage(WeChatKFNewsMessage news);

        Task<dynamic> SendKFMessage(WeChatKFMenuMessage menu);

        Task<dynamic> SetIndustryAsync(string industry1, string industry2);

        Task<dynamic> GetIndustryAsync();

        Task<dynamic> GetTemplateIDAsync(string template_short_name);

        Task<dynamic> GetTemplateListAsync();

        Task<dynamic> DeleteTemplateAsync(string template_id);

        Task<dynamic> SendTemplateMessageAsync(string touser, string template_id, string url, params (string, string, string)[] data);

        Task<dynamic> SendTemplateMessageAsync(string touser, string template_id, string appid, string pagepath, params (string, string, string)[] data);
    }
}
