// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace FormatTests.FormattedObject;

public class ArrayTests : Common.ArrayTests<FormattedObjectSerializer>
{
    public override void Roundtrip_ArrayContainingArrayAtNonZeroLowerBound()
    {
        Action action = base.Roundtrip_ArrayContainingArrayAtNonZeroLowerBound;
        action.Should().Throw<NotSupportedException>();
    }
}
