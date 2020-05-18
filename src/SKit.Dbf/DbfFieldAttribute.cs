// Copyright (c) Alexander Shlyakhto (InfDev). All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// Created:  2019.08.18
// Modified: 2020.05.18

using DotNetDBF;
using System;

namespace SKit.Dbf
{
    public class DbfFieldAttribute : Attribute
    {
        public NativeDbType DbType { get; set; }
        public int Length { get; set; }
        public int Precision { get; set; }
        //public string Name { get; set; }

        public DbfFieldAttribute()
        {
            DbType = NativeDbType.Char;
            Length = 255;
        }

    }
}
