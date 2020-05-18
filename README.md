# InfDev.Dbf

Helper for reading and writing dbf files without memo.  

By default supports DOS (866) and Windows (1251) encodings, but this can be changed by the definition of a new configuration according to IDbfConfiguration  

Used parser [DotNetDBF](https://github.com/ekonbenefits/dotnetdbf).  

Reading example: 

``` csharp
	using InfDef.Dbf;
	
	//...
	
	var list = DbfHelper.Read<DbfKpi>(
		path: kpiPath,
		winCoding: false,
		filter: e => e.NDOG == 194,
		skip: 0,
		take: 10);
```

For writing, the required properties must be marked with the DbfField attribute with the field descriptions in the database: DbType [, Length [, Precision]]. Only such properties will be stored in the database.

Writing example: 

``` csharp
	using System;
	using System.IO;
	using System.Collections.Generic;
	using DotNetDBF;
	using InfDef.Dbf;
	
	//...

    public class ExampleItem
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

    public class Test
	{
        public void WriteRead()
        {
            var items = new List<ExampleItem>
            {
                new ExampleItem
                {
                    Id = 1,
                    BeginDate = new DateTime(2019, 08, 19, 12, 58, 00),
                    Published = true,
                    Name = "Electric kettle",
                    Cost = 51,
                },
                new ExampleItem
                {
                    Id = 2,
                    BeginDate = new DateTime(2019, 08, 19, 12, 58, 00),
                    Published = true,
                    Name = "Xiaomi smartphone",
                    Cost = 685,
                }
            };

            var path = @"exampleItems.dbf";
            if (File.Exists(path))
                File.Delete(path);
            DbfHelper.Write(path, items, true);
            var readedItems = DbfHelper.Read<ExampleItem>(path, true);
		}
	}
```

You can use DotNetDBF directly to add records to the DBF file. If there is no file yet, a new one will be created.

``` csharp
	using System;
	using System.IO;
	using DotNetDBF;
	
	// ...
	
	using (var fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
	{
		using (var writer = new DBFWriter(fileStream))
		{
			writer.CharEncoding = Encoding.GetEncoding(1251);
			writer.Fields = new List<DBFField> {
				new DBFField("Id", NativeDbType.Numeric, 9),
				new DBFField("Name", NativeDbType.Char, 100),
				new DBFField("Enabled", NativeDbType.Logical)
			}.ToArray();

			var recordArray = new object[][] {
				new object[] { 1, "First", true },
				new object[] { 2, "Second", true }
			};
			foreach (var record in recordArray)
			{
				writer.WriteRecord(record);
			}
		}
	}
	
	// ...
```

