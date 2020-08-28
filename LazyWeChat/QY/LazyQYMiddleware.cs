using LazyWeChat.Abstract;
using LazyWeChat.Models;
using LazyWeChat.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LazyWeChat.QY
{
    public class LazyQYMiddleware
    {
        private readonly RequestDelegate _next;
        private Action<WeChatQYMessager> _onMessageReceived;
        private IMessageQueue _messageQueue;
        private readonly ILogger<LazyQYMiddleware> _logger;
        private readonly IOptions<LazyWeChatConfiguration> _options;

        public LazyQYMiddleware(
            RequestDelegate next,
            ILogger<LazyQYMiddleware> logger,
            IOptions<LazyWeChatConfiguration> options,
            Action<WeChatQYMessager> onMessageReceived,
            Type implementation)
        {
            _next = next;
            _logger = logger;
            _options = options;
            _onMessageReceived = onMessageReceived;
            _messageQueue = (IMessageQueue)Activator.CreateInstance(
                implementation,
                _options.Value.MQConnectionString);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            if (request.Path.Value.Contains(_options.Value.LazyWechatListener))
            {
                _logger.LogInformation($"LazyWechatListener{request.QueryString.ToString()}");

                string message = await GenerateReplyMessage(_options.Value.EncodingAESKey);

                await context.Response.WriteAsync(message);
            }
            else if (request.Path.Value.Contains(_options.Value.QYContactListener))
            {
                _logger.LogInformation($"QYContactListener{request.QueryString.ToString()}");

                string message = await GenerateReplyMessage(_options.Value.ContactEncodingAESKey);

                await context.Response.WriteAsync(message);
            }
            else
            {
                await _next(context);
            }

            bool Validate(out string msg_signature, out string timestamp, out string nonce, out string echostr)
            {
                #region msg_signature,timestamp,nonce以及echostr供用户判断该请求是否来自微信端
                msg_signature = string.IsNullOrEmpty(request.Query["msg_signature"]) ? "" : request.Query["msg_signature"].ToString();

                timestamp = string.IsNullOrEmpty(request.Query["timestamp"]) ? "" : request.Query["timestamp"].ToString();

                nonce = string.IsNullOrEmpty(request.Query["nonce"]) ? "" : request.Query["nonce"].ToString();

                echostr = string.IsNullOrEmpty(request.Query["echostr"]) ? "" : request.Query["echostr"].ToString();

                #endregion

                List<string> lstSort = new List<string> { _options.Value.Token, timestamp, nonce, echostr };
                lstSort.Sort();
                var sha1 = string.Join(string.Empty, lstSort).SHA1();

                var validation = (sha1.ToUpper() == msg_signature.ToUpper());
                return validation;
            }

            string generateSignature(string timestamp, string nonce, string encryptMessage)
            {
                ArrayList AL = new ArrayList();
                AL.Add(_options.Value.Token);
                AL.Add(timestamp);
                AL.Add(nonce);
                AL.Add(encryptMessage);
                AL.Sort(new DictionarySort());
                string raw = "";
                for (int i = 0; i < AL.Count; ++i)
                {
                    raw += AL[i];
                }

                SHA1 sha;
                ASCIIEncoding enc;
                string hash = "";
                sha = new SHA1CryptoServiceProvider();
                enc = new ASCIIEncoding();
                byte[] dataToHash = enc.GetBytes(raw);
                byte[] dataHashed = sha.ComputeHash(dataToHash);
                hash = BitConverter.ToString(dataHashed).Replace("-", "");
                hash = hash.ToLower();

                return hash;
            }

            dynamic ParseMessage(string inputContent)
            {
                inputContent = inputContent.Trim().Replace("\n", "");
                dynamic messageBody = new ExpandoObject();
                var dict = inputContent.FromXml();
                var json = JsonConvert.SerializeObject(dict);
                messageBody = UtilRepository.ParseAPIResult(json);
                return messageBody;
            }

            async Task<string> GenerateReplyMessage(string encodingAESKey)
            {
                bool validation = Validate(out string msg_signature, out string timestamp, out string nonce, out string echostr);

                var info = "msg_signature:{0},timestamp:{1},nonce:{2},echostr:{3} received from wechat at {4}";
                _logger.LogInformation(info, msg_signature, timestamp, nonce, echostr, DateTime.Now);

                var weChatQYMessager = new WeChatQYMessager
                {
                    msg_signature = msg_signature,
                    timestamp = timestamp,
                    nonce = nonce,
                    echostr = echostr,
                    encodingAESKey = encodingAESKey,
                    validation = validation,
                    type = _options.Value.Type,
                    method = context.Request.Method
                };

                #region 以stream的方式获取微信post到监听程序的数据:数据类型为XML
                var inputContent = "";
                using (StreamReader stream = new StreamReader(request.Body, Encoding.UTF8))
                {
                    inputContent = await stream.ReadToEndAsync();
                }
                info = "inputXml:'{0}' received from wechat at {1}";
                _logger.LogInformation(info, inputContent, DateTime.Now);
                #endregion

                if (!string.IsNullOrEmpty(inputContent))
                {
                    dynamic messageBody = new ExpandoObject();
                    messageBody = ParseMessage(inputContent);

                    if (UtilRepository.IsPropertyExist(messageBody, "Encrypt"))
                    {
                        string decryptedMessage = Cryptography.AES_decrypt(messageBody.Encrypt, encodingAESKey);
                        messageBody = ParseMessage(decryptedMessage);
                    }

                    weChatQYMessager.messageBody = messageBody;

                    if (UtilRepository.IsPropertyExist(messageBody, "FromUserName"))
                        _onMessageReceived?.Invoke(weChatQYMessager);

                    var json = JsonConvert.SerializeObject(weChatQYMessager);
                    _ = _messageQueue.Push(json);

                    info = "json format of message:'{0}' has been pushed into queue at {1}";
                    _logger.LogInformation(info, json, DateTime.Now);
                }

                var message = weChatQYMessager.message;

                if (weChatQYMessager.method.ToLower() == HttpMethods.Post.ToString().ToLower())
                {
                    if (message.ToLower() != "success")
                    {
                        var encryptMessage = Cryptography.AES_encrypt(weChatQYMessager.message, encodingAESKey, _options.Value.AppID);
                        var signatureResponse = generateSignature(timestamp, nonce, encryptMessage);
                        message = string.Format(MessageTemplateFactory.CreateInstance(weChatQYMessager.format == MessageFormat.Xml ? MessageType.Encrypt : MessageType.EncryptJson), encryptMessage, signatureResponse, timestamp, nonce);
                    }
                }

                info = "sent message:'{0}' has been logged at {1}";
                _logger.LogInformation(info, message, DateTime.Now);
                return message;
            }
        }
    }

    class DictionarySort : System.Collections.IComparer
    {
        public int Compare(object oLeft, object oRight)
        {
            string sLeft = oLeft as string;
            string sRight = oRight as string;
            int iLeftLength = sLeft.Length;
            int iRightLength = sRight.Length;
            int index = 0;
            while (index < iLeftLength && index < iRightLength)
            {
                if (sLeft[index] < sRight[index])
                    return -1;
                else if (sLeft[index] > sRight[index])
                    return 1;
                else
                    index++;
            }
            return iLeftLength - iRightLength;

        }
    }
}
