// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat.Tests;

public class StringRecordsCollectionTests
{
    [Fact]
    public void BasicFunctionality()
    {
        StringRecordsCollection collection = new();
        int id = 1;
        IRecord record = collection.GetStringRecord("Foo", ref id);
        id.Should().Be(2);
        record.Should().BeOfType<BinaryObjectString>();
        ((BinaryObjectString)record).ObjectId.Should().Be(1);

        record = collection.GetStringRecord("Foo", ref id);
        id.Should().Be(2);
        record.Should().BeOfType<MemberReference>();
        ((MemberReference)record).IdRef.Should().Be(1);

        record = collection.GetStringRecord("Bar", ref id);
        id.Should().Be(3);
        record.Should().BeOfType<BinaryObjectString>();
        ((BinaryObjectString)record).ObjectId.Should().Be(2);
    }
}
