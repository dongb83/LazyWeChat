using LazyWeChat.Models.OfficialAccount;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.OfficialAccount
{
    public interface ILazyMessager
    {
        /// <summary>
        /// 获取客服列表
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetKFAccountsAsync();

        /// <summary>
        /// 添加客服账号
        /// </summary>
        /// <param name="kf_account">客服email, 例如test1@test</param>
        /// <param name="nickname">昵称</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task<dynamic> AddKFAccountAsync(string kf_account, string nickname, string password);

        /// <summary>
        /// 修改客服账号
        /// </summary>
        /// <param name="kf_account">客服email, 例如test1@test</param>
        /// <param name="nickname">昵称</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task<dynamic> EditKFAccountAsync(string kf_account, string nickname, string password);

        /// <summary>
        /// 删除客服账号
        /// </summary>
        /// <param name="kf_account">客服email</param>
        /// <returns></returns>
        Task<dynamic> DeleteKFAccountAsync(string kf_account);

        /// <summary>
        /// 设置客服帐号的头像
        /// </summary>
        /// <param name="kf_account">客服email, 例如test1@test</param>
        /// <param name="avatarFilePath">头像完整路径</param>
        /// <returns></returns>
        Task<dynamic> UploadKFAvatarAsync(string kf_account, string avatarFilePath);

        /// <summary>
        /// 文本客服消息
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFTextMessage text);

        /// <summary>
        /// 图片客服消息
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFImageMessage image);

        /// <summary>
        /// 语音客服消息
        /// </summary>
        /// <param name="voice"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFVoiceMessage voice);

        /// <summary>
        /// 视频客服消息
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFVideoMessage video);

        /// <summary>
        /// 音乐客服消息
        /// </summary>
        /// <param name="music"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFMusicMessage music);

        /// <summary>
        /// 外部图文客服消息
        /// </summary>
        /// <param name="externalNews"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFExternalNewsMessage externalNews);

        /// <summary>
        /// 图文客服消息
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFNewsMessage news);

        /// <summary>
        /// 菜单客服消息
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFMenuMessage menu);

        /// <summary>
        /// 小程序客服消息
        /// </summary>
        /// <param name="mini"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFMiniMessage mini);

        /// <summary>
        /// 卡券客服消息
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(WeChatKFCardMessage card);

        /// <summary>
        /// 设置所属行业
        /// </summary>
        /// <param name="industry1">公众号模板消息所属行业编号1</param>
        /// <param name="industry2">公众号模板消息所属行业编号2</param>
        /// <returns></returns>
        Task<dynamic> SetIndustryAsync(string industry1, string industry2);

        /// <summary>
        /// 获取设置的行业信息
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetIndustryAsync();

        /// <summary>
        /// 获得模板ID
        /// </summary>
        /// <param name="template_short_name">模板库中模板的编号，有“TM**”和“OPENTMTM**”等形式</param>
        /// <returns></returns>
        Task<dynamic> GetTemplateIDAsync(string template_short_name);

        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetTemplateListAsync();

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="template_id">公众帐号下模板消息ID</param>
        /// <returns></returns>
        Task<dynamic> DeleteTemplateAsync(string template_id);

        /// <summary>
        /// 发送模板消息 跳转至URL
        /// </summary>
        /// <param name="touser">需要发送消息的用户的openid</param>
        /// <param name="template_id">模板消息ID</param>
        /// <param name="url">点击Link后跳转的URL</param>
        /// <param name="data">模板消息内容,Item1为模板中的keyword, Item2为keyword对应的value, Item3为value对应的color(若使用默认可传递"")</param>
        /// <returns></returns>
        Task<dynamic> SendTemplateMessageAsync(string touser, string template_id, string url, params (string, string, string)[] data);

        /// <summary>
        /// 发送模板消息 跳转至小程序
        /// </summary>
        /// <param name="touser">需要发送消息的用户的openid</param>
        /// <param name="template_id">模板消息ID</param>
        /// <param name="appid">点击跳转的小程序appid</param>
        /// <param name="pagepath">跳转的小程序的页面路径</param>
        /// <param name="data">模板消息内容,Item1为模板中的keyword, Item2为keyword对应的value, Item3为value对应的color(若使用默认可传递"")</param>
        /// <returns></returns>
        Task<dynamic> SendTemplateMessageAsync(string touser, string template_id, string appid, string pagepath, params (string, string, string)[] data);
    }
}
