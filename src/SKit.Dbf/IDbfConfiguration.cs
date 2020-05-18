using System;
using System.Collections.Generic;
using System.Text;

namespace SKit.Dbf
{
    public interface IDbfConfiguration
    {
        Encoding GetEncoding(bool winCoding);

        byte GetLanguageDriverCode(bool winCoding);

        byte GetSignature();
    }
}
