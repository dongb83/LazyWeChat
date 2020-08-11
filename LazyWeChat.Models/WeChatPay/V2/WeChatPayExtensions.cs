using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace LazyWeChat.Models.WeChatPay.V2
{
    public static class WeChatPayExtensions
    {
        #region 微信支付相关的扩展方法
        /// <summary>
        /// 将有序的字典列表中的键值对转化成XML的
        /// </summary>
        /// <param name="m_values"></param>
        /// <returns></returns>
        public static string ToXml(this SortedDictionary<string, object> m_values)
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
                throw new NullReferenceException("数据字典长度为0！");

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                {
                    var errmsg = "数据字典内部(" + pair.Key + ")含有值为null的字段!";
                }

                var dataType = pair.Value.GetType();
                if (dataType == typeof(int)
                    || dataType == typeof(double)
                    || dataType == typeof(long)
                    || dataType == typeof(short)
                    || dataType == typeof(float)
                    || dataType == typeof(Int16)
                    || dataType == typeof(Int32)
                    || dataType == typeof(Int64)
                    || dataType == typeof(uint)
                    || dataType == typeof(UInt16)
                    || dataType == typeof(UInt32)
                    || dataType == typeof(UInt64)
                    || dataType == typeof(sbyte)
                    || dataType == typeof(Single))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (dataType == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                else
                {
                    //除了string和int类型不能含有其他数据类型
                    throw new System.Exception("数据字典中字段(" + pair.Key + ")数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }

        /// <summary>
        /// 将有序的字典列表中的键值对转化成key1=value1&key=value2&key3=value3...的格式
        /// </summary>
        /// <param name="m_values"></param>
        /// <returns></returns>
        public static string ToUrl(this SortedDictionary<string, object> m_values)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    var errmsg = "有序字典内部(" + pair.Key + ")含有值为null的字段!";
                    throw new System.Exception(errmsg);
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }

        public static bool IsSet(this SortedDictionary<string, object> m_values, string key)
        {
            m_values.TryGetValue(key, out object o);
            if (null != o)
                return true;
            else
                return false;
        }

        public static object GetValue(this SortedDictionary<string, object> m_values, string key)
        {
            m_values.TryGetValue(key, out object o);
            return o;
        }

        public static void SetValue(this SortedDictionary<string, object> m_values, string key, object value)
        {
            m_values[key] = value;
        }

        /// <summary>
        /// 生成签名，详见签名生成算法
        /// </summary>
        /// <param name="m_values">有序的字典列表</param>
        /// <param name="Key">商户支付密钥</param>
        /// <returns>签名, sign字段不参加签名</returns>
        public static string MakeSign(this SortedDictionary<string, object> m_values, string Key)
        {
            //转url格式
            string str = m_values.ToUrl();
            //在string后加入API KEY
            str += "&key=" + Key;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        public static bool CheckSign(this SortedDictionary<string, object> m_values, string Key, out string errmsg)
        {
            errmsg = "OK";
            //如果没有设置签名，则跳过检测
            if (!m_values.IsSet("sign"))
            {
                errmsg = "签名不存在!";
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (m_values.GetValue("sign") == null || m_values.GetValue("sign").ToString() == "")
            {
                errmsg = "签名存在但不合法!";
            }

            //获取接收到的签名
            string return_sign = m_values.GetValue("sign").ToString();

            //在本地计算新的签名
            string cal_sign = m_values.MakeSign(Key);

            if (cal_sign == return_sign)
            {
                return true;
            }
            else
            {
                errmsg = "签名验证错误!";
                return false;
            }
        }
        #endregion
    }
}
