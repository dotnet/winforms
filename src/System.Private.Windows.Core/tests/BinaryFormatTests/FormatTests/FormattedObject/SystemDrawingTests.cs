// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.BinaryFormat;

namespace FormatTests.FormattedObject;

public class SystemDrawingTests : Common.SystemDrawingTests<FormattedObjectSerializer>
{
    [Fact]
    public void PointF_Parse()
    {
        BinaryFormattedObject format = new(Serialize(new PointF()));

        ClassWithMembersAndTypes classInfo = (ClassWithMembersAndTypes)format.RootRecord;
        classInfo.ObjectId.Should().Be(1);
        classInfo.Name.Should().Be("System.Drawing.PointF");
        classInfo.MemberNames.Should().BeEquivalentTo(["x", "y"]);
        classInfo.MemberValues.Should().BeEquivalentTo(new object[] { 0.0f, 0.0f });
        classInfo.MemberTypeInfo.Should().BeEquivalentTo(new MemberTypeInfo[]
        {
            new(BinaryType.Primitive, PrimitiveType.Single),
            new(BinaryType.Primitive, PrimitiveType.Single)
        });

        format[classInfo.LibraryId].Should().BeOfType<BinaryLibrary>()
            .Which.LibraryName.Should().Be("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
    }

    [Fact]
    public void RectangleF_Parse()
    {
        BinaryFormattedObject format = new(Serialize(new RectangleF()));

        ClassWithMembersAndTypes classInfo = (ClassWithMembersAndTypes)format.RootRecord;
        classInfo.ObjectId.Should().Be(1);
        classInfo.Name.Should().Be("System.Drawing.RectangleF");
        classInfo.MemberNames.Should().BeEquivalentTo(["x", "y", "width", "height"]);
        classInfo.MemberValues.Should().BeEquivalentTo(new object[] { 0.0f, 0.0f, 0.0f, 0.0f });
        classInfo.MemberTypeInfo.Should().BeEquivalentTo(new MemberTypeInfo[]
        {
            new(BinaryType.Primitive, PrimitiveType.Single),
            new(BinaryType.Primitive, PrimitiveType.Single),
            new(BinaryType.Primitive, PrimitiveType.Single),
            new(BinaryType.Primitive, PrimitiveType.Single)
        });
    }

    public static TheoryData<object> SystemDrawing_TestData => new()
    {
        new PointF(),
        new RectangleF()
    };
}
