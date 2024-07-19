// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace FormatTests.Formatter;

public class CorruptedTests : Common.CorruptedTests<BinaryFormatterSerializer>
{
    public override void ValueTypeReferencesSelf4()
    {
        Action action = base.ValueTypeReferencesSelf4;

        // "The ObjectManager found an invalid number of fixups."
        action.Should().Throw<SerializationException>();
    }

    public override void ValueTypeReferencesSelf5()
    {
        Action action = base.ValueTypeReferencesSelf5;

        // "The ObjectManager found an invalid number of fixups."
        action.Should().Throw<SerializationException>();
    }
}
