using System;
using System.Collections.Generic;
using System.Text;

using DotNetDBF;

namespace SKit.Dbf.Tests
{
    public class DbfExampleRecord
    {
        [DbfField(DbType = NativeDbType.Numeric, Length = 6, Precision = 0)]
        public int Id { get; set; }

        [DbfField(DbType = NativeDbType.Date)]
        public DateTime BeginDate { get; set; }

        [DbfField(DbType = NativeDbType.Logical)]
        public bool Published { get; set; }

        [DbfField(DbType = NativeDbType.Char, Length = 100)]
        public string Name { get; set; }

        [DbfField(DbType = NativeDbType.Numeric, Length = 6, Precision = 2)]
        public float Cost { get; set; }
    }
}
