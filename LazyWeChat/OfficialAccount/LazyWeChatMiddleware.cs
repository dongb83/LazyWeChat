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
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.OfficialAccount
{
    public class LazyWeChatMiddleware
    {
        private readonly RequestDelegate _next;
        private Action<WeChatMessager> _onMessageReceived;
        private IMessageQueue _messageQueue;
        private readonly ILogger<LazyWeChatMiddleware> _logger;
        private readonly IOptions<LazyWeChatConfiguration> _options;

        public LazyWeChatMiddleware(
            RequestDelegate next,
            ILogger<LazyWeChatMiddleware> logger,
            IOptions<LazyWeChatConfiguration> options,
            Action<WeChatMessager> onMessageReceived,
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
                bool validation = Validate(out string signature, out string timestamp, out string nonce, out string echostr);

                var info = "signature:{0},timestamp:{1},nonce:{2},echostr:{3} received from wechat at {4}";
                _logger.LogInformation(info, signature, timestamp, nonce, echostr, DateTime.Now);

                var weChatMessager = new WeChatMessager
                {
                    signature = signature,
                    timestamp = timestamp,
                    nonce = nonce,
                    echostr = echostr,
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
                #endregion

                info = "inputXml:'{0}' received from wechat at {1}";
                _logger.LogInformation(info, inputContent, DateTime.Now);

                var encrypted = false;

                if (!string.IsNullOrEmpty(inputContent))
                {
                    dynamic messageBody = new ExpandoObject();
                    messageBody = ParseMessage(inputContent, out MessageFormat format);
                    weChatMessager.format = format;

                    if (UtilRepository.IsPropertyExist(messageBody, "Encrypt"))
                    {
                        string decryptedMessage = Cryptography.AES_decrypt(messageBody.Encrypt, _options.Value.EncodingAESKey);
                        messageBody = ParseMessage(decryptedMessage, out _);
                        encrypted = true;
                    }

                    weChatMessager.messageBody = messageBody;

                    if (UtilRepository.IsPropertyExist(messageBody, "FromUserName"))
                        _onMessageReceived?.Invoke(weChatMessager);

                    var json = JsonConvert.SerializeObject(weChatMessager);
                    _ = _messageQueue.Push(json);

                    info = "json format of message:'{0}' has been pushed into queue at {1}";
                    _logger.LogInformation(info, json, DateTime.Now);
                }

                var message = weChatMessager.message;

                if (encrypted && message.ToLower() != "success")
                {
                    var encryptMessage = Cryptography.AES_encrypt(weChatMessager.message, _options.Value.EncodingAESKey, _options.Value.AppID);
                    var signatureResponse = generateSignature(timestamp, nonce, encryptMessage);
                    message = string.Format(MessageTemplateFactory.CreateInstance(weChatMessager.format == MessageFormat.Xml ? MessageType.Encrypt : MessageType.EncryptJson), encryptMessage, signatureResponse, timestamp, nonce);
                }

                info = "sent message:'{0}' has been logged at {1}";
                _logger.LogInformation(info, message, DateTime.Now);

                await context.Response.WriteAsync(message);
            }
            else
            {
                await _next(context);
            }

            bool Validate(out string signature, out string timestamp, out string nonce, out string echostr)
            {
                #region 微信在向监听程序发送数据的时候会同时发送signature,timestamp,nonce以及echostr供用户判断该请求是否来自微信端
                signature = string.IsNullOrEmpty(request.Query["signature"]) ? "" : request.Query["signature"].ToString();

                timestamp = string.IsNullOrEmpty(request.Query["timestamp"]) ? "" : request.Query["timestamp"].ToString();

                nonce = string.IsNullOrEmpty(request.Query["nonce"]) ? "" : request.Query["nonce"].ToString();

                echostr = string.IsNullOrEmpty(request.Query["echostr"]) ? "" : request.Query["echostr"].ToString();

                #endregion

                List<string> lstSort = new List<string> { _options.Value.Token, timestamp, nonce };
                lstSort.Sort();
                var sha1 = string.Join(string.Empty, lstSort).SHA1();

                var validation = (sha1.ToUpper() == signature.ToUpper());
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

            dynamic ParseMessage(string inputContent, out MessageFormat format)
            {
                inputContent = inputContent.Trim().Replace("\n", "");
                dynamic messageBody = new ExpandoObject();
                if (inputContent.StartsWith("<") && inputContent.EndsWith(">"))
                {
                    messageBody = UtilRepository.ParseAPIResult(JsonConvert.SerializeObject(inputContent.FromXml()));
                    format = MessageFormat.Xml;
                }
                else if (inputContent.StartsWith("{") && inputContent.EndsWith("}"))
                {
                    messageBody = UtilRepository.ParseAPIResult(inputContent);
                    format = MessageFormat.Json;
                }
                else
                    throw new ArgumentException(nameof(inputContent));
                return messageBody;
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
