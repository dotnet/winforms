// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Core.BinaryFormat;

namespace FormatTests.FormattedObject;

public class SystemDrawingTests : Common.SystemDrawingTests<FormattedObjectSerializer>
{
    [Fact]
    public void Point_Parse()
    {
        BinaryFormattedObject format = new(Serialize(new Point()));

        ClassWithMembersAndTypes classInfo = (ClassWithMembersAndTypes)format.RootRecord;
        classInfo.ObjectId.Should().Be(1);
        classInfo.Name.Should().Be("System.Drawing.Point");
        classInfo.MemberNames.Should().BeEquivalentTo(["x", "y"]);
        classInfo.MemberValues.Should().BeEquivalentTo(new object[] { 0, 0 });
        classInfo.MemberTypeInfo.Should().BeEquivalentTo(new MemberTypeInfo[]
        {
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.Primitive, PrimitiveType.Int32)
        });

        format[classInfo.LibraryId].Should().BeOfType<BinaryLibrary>()
            .Which.LibraryName.Should().Be("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
    }

    [Fact]
    public void Size_Parse()
    {
        BinaryFormattedObject format = new(Serialize(new Size()));

        ClassWithMembersAndTypes classInfo = (ClassWithMembersAndTypes)format.RootRecord;
        classInfo.ObjectId.Should().Be(1);
        classInfo.Name.Should().Be("System.Drawing.Size");
        classInfo.MemberNames.Should().BeEquivalentTo(["width", "height"]);
        classInfo.MemberValues.Should().BeEquivalentTo(new object[] { 0, 0 });
        classInfo.MemberTypeInfo.Should().BeEquivalentTo(new MemberTypeInfo[]
        {
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.Primitive, PrimitiveType.Int32)
        });

        format[classInfo.LibraryId].Should().BeOfType<BinaryLibrary>()
            .Which.LibraryName.Should().Be("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
    }

    [Fact]
    public void Rectangle_Parse()
    {
        BinaryFormattedObject format = new(Serialize(new Rectangle()));

        ClassWithMembersAndTypes classInfo = (ClassWithMembersAndTypes)format.RootRecord;
        classInfo.ObjectId.Should().Be(1);
        classInfo.Name.Should().Be("System.Drawing.Rectangle");
        classInfo.MemberNames.Should().BeEquivalentTo(["x", "y", "width", "height"]);
        classInfo.MemberValues.Should().BeEquivalentTo(new object[] { 0, 0, 0, 0 });
        classInfo.MemberTypeInfo.Should().BeEquivalentTo(new MemberTypeInfo[]
        {
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.Primitive, PrimitiveType.Int32)
        });
    }

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
    public void SizeF_Parse()
    {
        BinaryFormattedObject format = new(Serialize(new SizeF()));

        ClassWithMembersAndTypes classInfo = (ClassWithMembersAndTypes)format.RootRecord;
        classInfo.ObjectId.Should().Be(1);
        classInfo.Name.Should().Be("System.Drawing.SizeF");
        classInfo.MemberNames.Should().BeEquivalentTo(["width", "height"]);
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
        new Point(),
        new Size(),
        new Rectangle(),
        new PointF(),
        new SizeF(),
        new RectangleF(),
        Color.Red,
        new Color()
    };
}
