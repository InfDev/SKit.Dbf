using System;
using System.Collections.Generic;
using System.Text;
using DotNetDBF;

namespace SKit.Dbf.Tests
{
    // Структура файла KPI (частичная, не включает некоторые поля для теста)
    public class DbfKpi
    {
        public int IND { get; set; }
        public string NAM { get; set; }
        public int MSP { get; set; }
        public int PRISN0 { get; set; }
        public int GOS { get; set; }
        public int NDOG { get; set; }
    }
}
