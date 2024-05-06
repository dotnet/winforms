﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;
using System.Windows.Forms.BinaryFormat;

namespace FormatTests.FormattedObject;

public class RecordMapTests
{
    private class Record : IRecord
    {
        public Id Id => 1;

        void IBinaryWriteable.Write(BinaryWriter writer) { }
    }

    [Fact]
    public void RecordMap_CannotAddSameIndexTwice()
    {
        RecordMap map = new();
        Action action = () => map.AddRecord(new Record());
        action();
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RecordMap_GetMissingThrowsKeyNotFound()
    {
        RecordMap map = new();
        Func<object> func = () => map[(Id)0];
        func.Should().Throw<KeyNotFoundException>();
    }
}
