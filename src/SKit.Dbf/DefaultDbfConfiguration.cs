using System;
using System.Collections.Generic;
using System.Text;

namespace SKit.Dbf
{
    public class DefaultDbfConfiguration : IDbfConfiguration
    {
        public Encoding GetEncoding(bool winCoding)
        {
            return Encoding.GetEncoding(winCoding ? 1251 : 866);
        }

        public byte GetLanguageDriverCode(bool winCoding)
        {
            return (byte)(winCoding ? 0xC9 : 0x26);
        }

        public byte GetSignature()
        {
            return 0x3; // для dBASE III, dBASE IV, dBASE V, 4 для dBASE 7
        }
    }
}
