using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWeChat.Abstract
{
    public interface IQRGenerator
    {
        byte[] Generate(string content);
    }
}
