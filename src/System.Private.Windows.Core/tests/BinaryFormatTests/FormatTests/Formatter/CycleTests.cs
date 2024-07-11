// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace FormatTests.Formatter;

public class CycleTests : Common.CycleTests<BinaryFormatterSerializer>
{
    public override void BackPointerToISerializableClass()
    {
        Action action = () => base.BackPointerToISerializableClass();
        action.Should().Throw<SerializationException>();
    }
}
