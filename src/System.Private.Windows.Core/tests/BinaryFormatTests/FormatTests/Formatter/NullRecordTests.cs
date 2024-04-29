// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace FormatTests.Formatter;

public class NullRecordTests : Common.NullRecordTests<BinaryFormatterSerializer>
{
    public override void NullRecord_FollowingReferenceable()
    {
        // Not technically valid, the BinaryFormatter gets a null ref that it turns into SerializationException.
        Action action = base.NullRecord_FollowingReferenceable;
        action.Should().Throw<SerializationException>();
    }

    public override void NullRecord_BeforeReferenceable()
    {
        // Not technically valid, the BinaryFormatter gets a null ref that it turns into SerializationException.
        Action action = base.NullRecord_BeforeReferenceable;
        action.Should().Throw<SerializationException>();
    }

    public override void NullRecord_WrongLength_WithTuple(int nullCount)
    {
        Action action = () => base.NullRecord_WrongLength_WithTuple(nullCount);

        if (nullCount == 0)
        {
            action();
        }
        else
        {
            action.Should().Throw<SerializationException>();
        }
    }
}
