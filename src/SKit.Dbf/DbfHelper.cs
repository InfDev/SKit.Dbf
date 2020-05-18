// Copyright (c) Alexander Shlyakhto (InfDev). All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// Created:  2019.05.26
// Modified: 2020.05.18

using DotNetDBF;

using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

namespace SKit.Dbf
{
    public class DbfHelper
    {
        private static DbfHelper _instance;

        private DbfHelper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static IDbfConfiguration Configuration { get; set; } = new DefaultDbfConfiguration();

        protected static DbfHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DbfHelper();
                return _instance;
            }
        }

        /// <summary>
        /// Reading from a DBF file
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="path">File path</param>
        /// <param name="winCoding">Encoding windows (1251), otherwise DOS (866)</param>
        /// <param name="filter">Data filtering function</param>
        /// <param name="skip">Skip records</param>
        /// <param name="take">Get the number of records</param>
        /// <returns></returns>
        public static IList<T> Read<T>(
            string path, 
            bool winCoding = true,
            Func<T, bool> filter = null, 
            int skip = 0, 
            int take = int.MaxValue) where T : class
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Файл не обнаружен", path);

            var instanse = Instance;
            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                var binaryReader = new BinaryReader(fileStream, Encoding.ASCII);
                var reader = new DBFReader(fileStream);
                reader.CharEncoding = Configuration.GetEncoding(winCoding);

                var type = typeof(T);
                var props = type.GetProperties()
                    .Where(p => p.CanWrite &&
                        Array.FindIndex(reader.Fields, f => f.Name.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)) >= 0)
                    .ToList();
                Dictionary<PropertyInfo, int> propIndexDictionary = new Dictionary<PropertyInfo, int>();
                foreach (var prop in props)
                {
                    var index = Array.FindIndex(reader.Fields, f => f.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (index >= 0)
                        propIndexDictionary.Add(prop, index);
                }

                var retval = new List<T>();
                var rowValues = reader.NextRecord();
                int filteredCount = 0;
                int addedCount = 0;
                while (rowValues != null)
                {
                    var newObj = (T)Activator.CreateInstance(typeof(T));
                    foreach(var propIndex in propIndexDictionary)
                    {
                        var fieldValue = rowValues[propIndex.Value];
                        if (fieldValue != null)
                        {
                            var propValue = Convert.ChangeType(fieldValue, propIndex.Key.PropertyType);
                            propIndex.Key.SetValue(newObj, propValue);
                        }
                        else
                        {
                            //var t = propIndex.Key.PropertyType;
                            //propIndex.Key.SetValue(newObj, default(propIndex.Key.PropertyType)));
                        }
                    }
                    
                    // Фильтрация
                    if (filter != null)
                    {
                        if (!filter(newObj))
                        {
                            rowValues = reader.NextRecord();
                            continue;
                        }
                    }
                    // Получить только указанный блок записей
                    filteredCount++;
                    if (filteredCount > skip)
                    {
                        if (addedCount >= take)
                            break;
                        retval.Add(newObj);
                        addedCount++;
                    }
                    rowValues = reader.NextRecord();
                }
                return retval;
            }
        }

        /// <summary>
        /// Writing to a DBF file
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="items"></param>
        /// <param name="winCoding">Encoding windows (1251), otherwise DOS (866)</param>
        /// <remarks>Only fields marked with the DbfField attribute are saved.</remarks>
        public static void Write<T>(string path, IEnumerable<T> items, bool winCoding = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Не указано имя файла", path);
            if (File.Exists(path))
                throw new Exception($"Файл '{path}' c таким имененем уже существует, запись невозвожна.");

            var instanse = Instance;
            var type = typeof(T);
            var props = type.GetProperties()
                .Where(p => p.CanRead)
                .ToList();
            var saveProps = new List<PropertyInfo>();
            var fields = new List<DBFField>();
            foreach (var prop in props)
            {
                var fieldAttr = prop.GetCustomAttributes<DbfFieldAttribute>(false).FirstOrDefault();
                if (fieldAttr == null)
                    continue;
                saveProps.Add(prop);
                if (fieldAttr.DbType == NativeDbType.Date || fieldAttr.DbType == NativeDbType.Logical || fieldAttr.DbType == NativeDbType.Memo)
                    fields.Add(new DBFField(prop.Name, fieldAttr.DbType));
                else
                    fields.Add(new DBFField(prop.Name, fieldAttr.DbType, fieldAttr.Length, fieldAttr.Precision));
            }
            if (fields.Count == 0)
                throw new Exception("У типа {type.Name} отсутствуют аттрибуты DbfField с описаниями полей в базе: DbType[, Length[, Precision]]");
            using (var fileStream = File.Create(path)) //Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (var writer = new DBFWriter(fileStream))
                {
                    writer.CharEncoding = Configuration.GetEncoding(winCoding);
                    writer.Fields = fields.ToArray();
                    foreach (var item in items)
                    {
                        var record = new object[saveProps.Count];
                        for (var i = 0; i < saveProps.Count; i++)
                            record[i] = saveProps[i].GetValue(item);
                        writer.WriteRecord(record);
                    }
                }
            }
        }
    }
}
// https://csharp.hotexamples.com/ru/examples/DotNetDBF/DBFWriter/-/php-dbfwriter-class-examples.html
// http://www.nudoq.org/#!/Projects/dotnetdbf
