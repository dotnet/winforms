// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Windows.Forms.BinaryFormat;
using FormatTests.Common;

namespace FormatTests.FormattedObject;

public class ExceptionTests : SerializationTest<FormattedObjectSerializer>
{
    [Fact]
    public void NotSupportedException_Parse()
    {
        BinaryFormattedObject format = new(Serialize(new NotSupportedException()));
        var systemClass = (SystemClassWithMembersAndTypes)format.RootRecord;
        systemClass.Name.Should().Be(typeof(NotSupportedException).FullName);
        systemClass.MemberNames.Should().BeEquivalentTo(
        [
            "ClassName",
            "Message",
            "Data",
            "InnerException",
            "HelpURL",
            "StackTraceString",
            "RemoteStackTraceString",
            "RemoteStackIndex",
            "ExceptionMethod",
            "HResult",
            "Source",
            "WatsonBuckets"
        ]);

        systemClass.MemberTypeInfo.Should().BeEquivalentTo(new (BinaryType, object?)[]
        {
            (BinaryType.String, null),
            (BinaryType.String, null),
            (BinaryType.SystemClass, typeof(IDictionary).FullName),
            (BinaryType.SystemClass, typeof(Exception).FullName),
            (BinaryType.String, null),
            (BinaryType.String, null),
            (BinaryType.String, null),
            (BinaryType.Primitive, PrimitiveType.Int32),
            (BinaryType.String, null),
            (BinaryType.Primitive, PrimitiveType.Int32),
            (BinaryType.String, null),
            (BinaryType.PrimitiveArray, PrimitiveType.Byte)
        });

        systemClass.MemberValues.Should().BeEquivalentTo(new object?[]
        {
            new BinaryObjectString(2, "System.NotSupportedException"),
            new BinaryObjectString(3, "Specified method is not supported."),
            null,
            null,
            null,
            null,
            null,
            0,
            null,
            unchecked((int)0x80131515),
            null,
            null
        });
    }
}
