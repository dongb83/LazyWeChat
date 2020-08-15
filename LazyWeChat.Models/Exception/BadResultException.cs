using LazyWeChat.Utility;
using Newtonsoft.Json;

namespace LazyWeChat.Models.Exception
{
    public class BadResultException : System.Exception
    {
        int _errcode;
        string _message;

        public BadResultException(dynamic obj)
        {
            if (UtilRepository.IsPropertyExist(obj, "errcode") &&
                UtilRepository.IsPropertyExist(obj, "errmsg"))
            {
                if (!int.TryParse(obj.errcode.ToString(), out _errcode))
                {
                    _message = $"invlid errcode:{obj.errcode}";
                }
                else
                {
                    _message = $"wechat API return unsuccessful result:errcode('{_errcode}'),errmsg('{obj.errmsg}')";
                }
            }
            else
            {
                _message = $"invalid error message format:{JsonConvert.SerializeObject(obj)}";
            }
        }

        public override string Message => _message;
    }
}
