using System;
using System.IO;
using System.Net;
using System.Text;

namespace LazyWeChat.Utility
{
    internal class HttpRequestRepository
    {
        #region 发送 POST Request
        /// <summary>
        /// 以 POST 形式请求数据
        /// </summary>
        /// <param name="RequestPara"></param>
        /// <param name="Url"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string PostData(string RequestPara, string Url)
        {
            WebRequest hr = HttpWebRequest.Create(Url);

            byte[] buf = System.Text.Encoding.GetEncoding("utf-8").GetBytes(RequestPara);
            string contentType = "application/x-www-form-urlencoded";
            hr.ContentType = contentType;
            hr.ContentLength = buf.Length;
            hr.Method = "POST";

            System.IO.Stream RequestStream = hr.GetRequestStream();
            RequestStream.Write(buf, 0, buf.Length);
            RequestStream.Close();

            System.Net.WebResponse response = hr.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            string ReturnVal = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return ReturnVal;
        }

        public static string PostData(string RequestPara, string Url, string certPath, string certPwd)
        {
            System.Security.Cryptography.X509Certificates.X509Certificate cer = new System.Security.Cryptography.X509Certificates.X509Certificate(certPath, certPwd);
            HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(Url);
            webrequest.ClientCertificates.Add(cer);
            webrequest.Method = "POST";
            using (Stream writeStream = webrequest.GetRequestStream())
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(RequestPara);
                writeStream.Write(bytes, 0, bytes.Length);
            }
            HttpWebResponse webreponse = (HttpWebResponse)webrequest.GetResponse();
            Stream stream = webreponse.GetResponseStream();
            string resp = string.Empty;
            using (StreamReader reader = new StreamReader(stream))
            {
                resp = reader.ReadToEnd();
            }
            return resp;
        }

        //这个方法是两个URL第一个url是条到微信的，第二个是本地图片路径
        public static string HttpUploadFile(string url, string path, string formName = "file", string additionInfo = "")
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            int pos = path.LastIndexOf("\\");
            string fileName = path.Substring(pos + 1);

            StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"" + formName + "\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] bArr = new byte[fs.Length];
            fs.Read(bArr, 0, bArr.Length);
            fs.Close();

            Stream postStream = request.GetRequestStream();
            postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(bArr, 0, bArr.Length);
            //postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            byte[] bSplit = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            postStream.Write(bSplit, 0, bSplit.Length);

            if (!string.IsNullOrEmpty(additionInfo))
            {
                byte[] additionBytes = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"description\";\r\n\r\n");
                postStream.Write(additionBytes, 0, additionBytes.Length);
                byte[] additionContentBytes = Encoding.UTF8.GetBytes(additionInfo);
                postStream.Write(additionContentBytes, 0, additionContentBytes.Length);
                postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            }

            postStream.Close();

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream instream = response.GetResponseStream();
            StreamReader sr = new StreamReader(instream, Encoding.UTF8);
            string content = sr.ReadToEnd();
            return content;
        }
        #endregion

        #region 发送 GET Request
        /// <summary>
        /// 以 GET 形式获取数据
        /// </summary>
        /// <param name="RequestPara"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string GetData(string Url)
        {
            WebRequest hr = HttpWebRequest.Create(Url);

            hr.Method = "GET";

            System.Net.WebResponse response = hr.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            string ReturnVal = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return ReturnVal;
        }

        /// <summary>
        /// 以 GET 形式获取数据
        /// </summary>
        /// <param name="RequestPara"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string GetData(string RequestPara, string Url)
        {
            RequestPara = RequestPara.IndexOf('?') > -1 ? (RequestPara) : ("?" + RequestPara);

            WebRequest hr = HttpWebRequest.Create(Url + RequestPara);

            byte[] buf = System.Text.Encoding.GetEncoding("utf-8").GetBytes(RequestPara);
            hr.Method = "GET";

            System.Net.WebResponse response = hr.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            string ReturnVal = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return ReturnVal;
        }

        public static Stream GetStream(string RequestPara, string Url)
        {
            RequestPara = RequestPara.IndexOf('?') > -1 ? (RequestPara) : ("?" + RequestPara);

            WebRequest hr = HttpWebRequest.Create(Url + RequestPara);

            byte[] buf = System.Text.Encoding.GetEncoding("utf-8").GetBytes(RequestPara);
            hr.Method = "GET";

            System.Net.WebResponse response = hr.GetResponse();

            return response.GetResponseStream();
        }
        #endregion
    }
}
