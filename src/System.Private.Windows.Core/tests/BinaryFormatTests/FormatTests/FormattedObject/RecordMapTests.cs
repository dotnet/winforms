// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.BinaryFormat;

namespace FormatTests.FormattedObject;

public class RecordMapTests
{
    private class Record : IRecord
    {
        void IBinaryWriteable.Write(BinaryWriter writer) { }
    }

    [Fact]
    public void RecordMap_CannotAddSameIndexTwice()
    {
        RecordMap map = new();
        Action action = () => map[1] = new Record();
        action();
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RecordMap_GetMissingThrowsKeyNotFound()
    {
        RecordMap map = new();
        Func<object> func = () => map[0];
        func.Should().Throw<KeyNotFoundException>();
    }
}
