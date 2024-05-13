// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.BinaryFormat;
using FormatTests.Common;

namespace FormatTests.FormattedObject;

public class ExceptionTests : SerializationTest<FormattedObjectSerializer>
{
    [Fact]
    public void NotSupportedException_Parse()
    {
        System.Windows.Forms.BinaryFormat.BinaryFormattedObject format = new(Serialize(new NotSupportedException()));
        var systemClass = (ClassRecord)format.RootRecord;
        systemClass.TypeName.FullName.Should().Be(typeof(NotSupportedException).FullName);
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

        systemClass.GetString("ClassName").Should().Be("System.NotSupportedException");
        systemClass.GetString("Message").Should().Be("Specified method is not supported.");
        systemClass.GetObject("Data").Should().BeNull();
        systemClass.GetObject("InnerException").Should().BeNull();
        systemClass.GetObject("HelpURL").Should().BeNull();
        systemClass.GetObject("StackTraceString").Should().BeNull();
        systemClass.GetObject("RemoteStackTraceString").Should().BeNull();
        systemClass.GetInt32("RemoteStackIndex").Should().Be(0);
        systemClass.GetObject("ExceptionMethod").Should().BeNull();
        systemClass.GetInt32("HResult").Should().Be(unchecked((int)0x80131515));
        systemClass.GetObject("Source").Should().BeNull();
        systemClass.GetObject("WatsonBuckets").Should().BeNull();
    }
}
