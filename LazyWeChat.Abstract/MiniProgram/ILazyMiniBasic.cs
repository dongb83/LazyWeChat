using LazyWeChat.Models;
using LazyWeChat.Models.MiniProgram;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.MiniProgram
{
    public enum ParameterType
    {
        member_count,
        room_limit,
        path,
        version_type,
    }

    public interface ILazyMiniBasic
    {
        /// <summary>
        /// 获取小程序全局唯一后台接口调用凭据(access_token)
        /// </summary>
        /// <returns></returns>
        Task<string> GetAccessTokenAsync();

        /// <summary>
        /// 获取openid,session_key和unionid
        /// </summary>
        /// <param name="js_code">通过 wx.login 接口获得临时登录凭证</param>
        /// <returns></returns>
        Task<dynamic> Code2SessionAsync(string js_code);

        /// <summary>
        /// 通过微信支付订单号获取unionid
        /// </summary>
        /// <param name="openid">openid</param>
        /// <param name="transaction_id">微信支付订单</param>
        /// <returns></returns>
        Task<dynamic> getPaidUnionIdwtTransactionIdAsync(string openid, string transaction_id);

        /// <summary>
        /// 通过微信支付商户订单号获取unionid
        /// </summary>
        /// <param name="openid">openid</param>
        /// <param name="out_trade_no">微信支付商户订单号</param>
        /// <returns></returns>
        Task<dynamic> getPaidUnionIdwtOutTradeNoAsync(string openid, string out_trade_no);

        /// <summary>
        /// 获取客服消息内的临时素材
        /// </summary>
        /// <param name="media_id">media_id</param>
        /// <returns></returns>
        Task<byte[]> GetTempMediaAsync(string media_id);

        /// <summary>
        /// 把媒体文件上传到微信服务器, 目前仅支持图片
        /// </summary>
        /// <param name="fullFilePath">素材完整路径(目前仅支持图片)</param>
        /// <returns></returns>
        Task<dynamic> UploadTempMediaAsync(string fullFilePath);

        /// <summary>
        /// 发送文本客服消息
        /// </summary>
        /// <param name="message">文本消息内容</param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(MiniKFTextMessage message);

        /// <summary>
        /// 发送图片客服消息
        /// </summary>
        /// <param name="message">图片消息内容</param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(MiniKFImageMessage message);

        /// <summary>
        /// 发送链接客服消息
        /// </summary>
        /// <param name="message">链接消息内容</param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(MiniKFLinkMessage message);

        /// <summary>
        /// 发送小程序客服消息
        /// </summary>
        /// <param name="message">小程序消息内容</param>
        /// <returns></returns>
        Task<dynamic> SendKFMessageAsync(MiniKFMiniMessage message);

        /// <summary>
        /// 发送统一服务消息(小程序发送公众号模板消息)
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        Task<dynamic> SendUniformMessageAsync(MPUniformMessage message);

        /// <summary>
        /// 根据微信小程序平台提供的解密算法解密数据
        /// </summary>
        /// <param name="encryptedData">加密数据</param>
        /// <param name="iv">初始向量</param>
        /// <param name="sessionKey">从服务端获取的SessionKey</param>
        /// <returns></returns>
        string Decrypt(string encryptedData, string iv, string sessionKey);

        /// <summary>
        /// 将模板并添加至帐号下的个人模板库
        /// </summary>
        /// <param name="tid">模板标题 id</param>
        /// <param name="sceneDesc">服务场景描述</param>
        /// <param name="kidList">开发者自行组合好的模板关键词列表</param>
        /// <returns></returns>
        Task<dynamic> AddTemplateAsync(string tid, string sceneDesc, params string[] kidList);

        /// <summary>
        /// 删除帐号下的个人模板
        /// </summary>
        /// <param name="priTmplId">要删除的模板id</param>
        /// <returns></returns>
        Task<dynamic> DeleteTemplateAsync(string priTmplId);

        /// <summary>
        /// 获取小程序账号的类目
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetCategoryAsync();

        //Task<dynamic> GetPubTemplateKeyWordsByIdAsync(string tid);

        //Task<dynamic> GetPubTemplateTitleListAsync(string ids, int start, int limit);

        /// <summary>
        /// 获取当前帐号下的个人模板列表
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetTemplateListAsync();

        /// <summary>
        /// 发送订阅消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        Task<dynamic> SendSubscribeMessageAsync(SubscribeMessage message);

        /// <summary>
        /// 创建动态消息需要的activity_id
        /// </summary>
        /// <returns></returns>
        Task<dynamic> CreateActivityIdAsync();

        /// <summary>
        /// 修改被分享的动态消息
        /// </summary>
        /// <param name="activity_id">动态消息的 ID</param>
        /// <param name="target_state">动态消息修改后的状态</param>
        /// <param name="parameters">模板中需要修改的参数列表</param>
        /// <returns></returns>
        Task<dynamic> SetUpdatableMsgAsync(string activity_id, int target_state, Dictionary<ParameterType, string> parameters);

        /// <summary>
        /// 生成小程序二维码
        /// </summary>
        /// <param name="path">扫码进入的小程序页面路径</param>
        /// <param name="width">二维码的宽度</param>
        /// <returns></returns>
        Task<byte[]> CreateQRCodeAsync(string path, int width);
    }
}
