// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma warning disable CA1050 // Declare types in namespaces
public class ClassWithNoNamespace { }
#pragma warning restore CA1050

namespace System.Drawing.Tests
{
    public class bitmap_173x183_indexed_8bit { }

    public class Icon_toolboxBitmapAttributeTest { }

    public class ToolboxBitmapAttributeTests
    {
        private static Size s_defaultSize = new(16, 16);

        private static void AssertDefaultSize(Image image)
        {
            Assert.Equal(s_defaultSize, image.Size);
        }

        public static IEnumerable<object[]> Ctor_FileName_TestData()
        {
            yield return new object[] { null, new Size(0, 0) };
            yield return new object[] { Helpers.GetTestBitmapPath("bitmap_173x183_indexed_8bit.bmp"), new Size(173, 183) };
            yield return new object[] { Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico"), new Size(16, 16) };
            yield return new object[] { Helpers.GetTestBitmapPath("invalid.ico"), new Size(0, 0) };
        }

        [Theory]
        [MemberData(nameof(Ctor_FileName_TestData))]
        public void Ctor_FileName(string fileName, Size size)
        {
            ToolboxBitmapAttribute attribute = new(fileName);

            using Image image = attribute.GetImage(null);
            if (size == Size.Empty)
            {
                AssertDefaultSize(image);
            }
            else
            {
                Assert.Equal(size, image.Size);
            }
        }

        [Theory]
        [InlineData(null, -1, -1)]
        [InlineData(typeof(ClassWithNoNamespace), -1, -1)]
        [InlineData(typeof(bitmap_173x183_indexed_8bit), 173, 183)]
        [InlineData(typeof(ToolboxBitmapAttributeTests), -1, -1)]
        public void Ctor_Type(Type? type, int width, int height)
        {
            ToolboxBitmapAttribute attribute = new(type);
            using Image image = attribute.GetImage(type);
            if (width == -1 && height == -1)
            {
                AssertDefaultSize(image);
            }
            else
            {
                Assert.Equal(new Size(width, height), image.Size);
            }
        }

        [Theory]
        [InlineData(null, null, -1, -1)]
        [InlineData(null, "invalid.ico", -1, -1)]
        [InlineData(typeof(ClassWithNoNamespace), null, -1, -1)]
        [InlineData(typeof(ToolboxBitmapAttributeTests), "", -1, -1)]
        [InlineData(typeof(ToolboxBitmapAttributeTests), null, -1, -1)]
        [InlineData(typeof(ToolboxBitmapAttributeTests), "invalid.ico", -1, -1)]
        [InlineData(typeof(ToolboxBitmapAttributeTests), "48x48_multiple_entries_4bit", 16, 16)]
        [InlineData(typeof(ToolboxBitmapAttributeTests), "48x48_multiple_entries_4bit.ico", 16, 16)]
        [InlineData(typeof(ToolboxBitmapAttributeTests), "empty.file", -1, -1)]
        [InlineData(typeof(ToolboxBitmapAttributeTests), "bitmap_173x183_indexed_8bit", 173, 183)]
        [InlineData(typeof(ToolboxBitmapAttributeTests), "bitmap_173x183_indexed_8bit.bmp", 173, 183)]
        public void Ctor_Type_String(Type? type, string? fileName, int width, int height)
        {
            ToolboxBitmapAttribute attribute = new(type, fileName);

            using Image image = attribute.GetImage(type, fileName, false);
            if (width == -1 && height == -1)
            {
                AssertDefaultSize(image);
            }
            else
            {
                Assert.Equal(new Size(width, height), image.Size);
            }
        }

        [Theory]
        [InlineData("bitmap_173x183_indexed_8bit.bmp", 173, 183)]
        [InlineData("48x48_multiple_entries_4bit.ico", 16, 16)]
        public void GetImage_TypeFileNameBool_ReturnsExpected(string fileName, int width, int height)
        {
            ToolboxBitmapAttribute attribute = new((string)null);
            using (Image image = attribute.GetImage(typeof(ToolboxBitmapAttributeTests), fileName, large: true))
            {
                Assert.Equal(new Size(32, 32), image.Size);
            }

            using (Image image = attribute.GetImage(typeof(ToolboxBitmapAttributeTests), fileName, large: false))
            {
                Assert.Equal(new Size(width, height), image.Size);
            }
        }

        [Fact]
        public void GetImage_NullComponent_ReturnsNull()
        {
            ToolboxBitmapAttribute attribute = new((string)null);
            Assert.Null(attribute.GetImage((object)null));
            Assert.Null(attribute.GetImage((object)null, true));
        }

        [Fact]
        public void GetImage_Component_ReturnsExpected()
        {
            ToolboxBitmapAttribute attribute = new((string)null);

            using Image smallImage = attribute.GetImage(new bitmap_173x183_indexed_8bit(), large: false);
            Assert.Equal(new Size(173, 183), smallImage.Size);

            using Image largeImage = attribute.GetImage(new bitmap_173x183_indexed_8bit(), large: true);
            Assert.Equal(new Size(32, 32), largeImage.Size);
        }

        [Fact]
        public void GetImage_Default_ReturnsExpected()
        {
            ToolboxBitmapAttribute attribute = ToolboxBitmapAttribute.Default;

            using (Image image = attribute.GetImage(typeof(ToolboxBitmapAttributeTests), "bitmap_173x183_indexed_8bit", large: true))
            {
                Assert.Equal(new Size(32, 32), image.Size);
            }

            using (Image image = attribute.GetImage(typeof(ToolboxBitmapAttributeTests), "bitmap_173x183_indexed_8bit", large: false))
            {
                Assert.Equal(new Size(173, 183), image.Size);
            }
        }

        [Theory]
        [InlineData(typeof(Icon_toolboxBitmapAttributeTest), 256, 256)]
        public void GetImage_NoExtension(Type type, int width, int height)
        {
            ToolboxBitmapAttribute attribute = new(type);
            using Image image = attribute.GetImage(type);
            if (width == -1 && height == -1)
            {
                AssertDefaultSize(image);
            }
            else
            {
                Assert.Equal(new Size(width, height), image.Size);
            }
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            yield return new object[] { ToolboxBitmapAttribute.Default, ToolboxBitmapAttribute.Default, true };
            yield return new object[] { ToolboxBitmapAttribute.Default, new ToolboxBitmapAttribute(typeof(ToolboxBitmapAttribute), "bitmap_173x183_indexed_8bit"), true };

            yield return new object[] { ToolboxBitmapAttribute.Default, new(), false };
            yield return new object[] { ToolboxBitmapAttribute.Default, null, false };
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public void Equals_Other_ReturnsExpected(ToolboxBitmapAttribute attribute, object other, bool expected)
        {
            Assert.Equal(expected, attribute.Equals(other));
            Assert.Equal(attribute.GetHashCode(), attribute.GetHashCode());
        }
    }
}
