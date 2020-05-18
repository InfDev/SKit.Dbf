using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

using SKit.Dbf;

namespace SKit.Dbf.Tests
{
    public class DbfTest
    {
        private string kpiPath => @"data\kpi1512.dbf";

        [Fact]
        public void GetKpi_contract194()
        {
            // Получить записи, принадлежащие договору 194 (должно быть 6 записей)
            var list = DbfHelper.Read<DbfKpi>(
                path: kpiPath,
                winCoding: false,
                filter: e => e.NDOG == 194,
                skip: 0,
                take: 10);
            Assert.Equal(6, list.Count);
        }

        //[Fact]
        //void MemoryLeakTest()
        //{
        //    var list = DbfHelper.Read<DbfKpi>(
        //                path: kpiPath,
        //                winCoding: false);

        //    var weakRef = new WeakReference(list);

        //    // Run an operation with leakyObject
        //    list = null;
                        
        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //    GC.Collect();
        //    Assert.False(weakRef.IsAlive);
        //}

        [Fact]
        public void GetKpi_EUREKA()
        {
            // Получить записи, принадлежащие договору 194 (должно быть 6 записей)
            var list = DbfHelper.Read<DbfKpi>(
                path: kpiPath,
                winCoding: false,
                filter: e => e.NAM == "ЭВРИКА");
            Assert.Equal(1, list.Count);
        }

        [Fact]
        public void WriteRead()
        {
            var items = new List<DbfExampleRecord>
            {
                new DbfExampleRecord
                {
                    Id = 1,
                    BeginDate = new DateTime(2019, 08, 19, 12, 58, 00),
                    Published = true,
                    Name = "Утюг",
                    Cost = 510,
                },
                new DbfExampleRecord
                {
                    Id = 2,
                    BeginDate = new DateTime(2019, 08, 19, 12, 58, 00),
                    Published = true,
                    Name = "Смартфон Xiaomi",
                    Cost = 6850,
                }
            };

            var path = @"data\example.dbf";
            if (File.Exists(path))
                File.Delete(path);
            DbfHelper.Write(path, items, true);
            var readedItems = DbfHelper.Read<DbfExampleRecord>(path, true);
            Assert.Equal(2, readedItems.Count);

            //using (var fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //    using (var writer = new DBFWriter(fileStream))
            //    {
            //        writer.CharEncoding = Encoding.GetEncoding(1251);
            //        writer.Fields = new List<DBFField> {
            //            new DBFField("Id", NativeDbType.Numeric, 9),
            //            new DBFField("Name", NativeDbType.Char, 100),
            //            new DBFField("Enabled", NativeDbType.Logical)
            //        }.ToArray();

            //        var recordArray = new object[][] {
            //            new object[] { 1, "First", true },
            //            new object[] { 2, "Second", true }
            //        };
            //        foreach (var record in recordArray)
            //        {
            //            writer.WriteRecord(record);
            //        }
            //    }
            //}

        }
    }
}
