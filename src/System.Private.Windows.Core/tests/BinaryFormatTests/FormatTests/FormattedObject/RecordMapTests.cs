// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.BinaryFormat.Serializer;

namespace FormatTests.FormattedObject;

public class RecordMapTests
{
    private class Record : IWritableRecord
    {
        public Id Id => 1;

        void IWritableRecord.Write(BinaryWriter writer) { }
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
