// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Design.Tests;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
[Collection(nameof(CursorTestsCollection))]
public class CursorTests
{
    // Some controls have behavior that changes when the mouse is over them. To avoid verification issues, keep these
    // tests from running when other tests do and move the cursor back to 0,0 when they are done.
    //
    // See https://github.com/dotnet/winforms/pull/7031#issuecomment-1101339968 for an example of this.

    [CollectionDefinition(nameof(CursorTestsCollection), DisableParallelization = true)]
    public class CursorTestsCollection : ICollectionFixture<CursorTestsCollection.CursorTestsFixture>
    {
        public class CursorTestsFixture : IDisposable
        {
            public CursorTestsFixture() => Cursor.Position = default;

            public void Dispose() => Cursor.Position = default;
        }
    }

    [Fact]
    public void Cursor_Ctor_IntPtr()
    {
        Cursor sourceCursor = Cursors.AppStarting;
        using Cursor cursor = new(sourceCursor.Handle);
        Assert.Equal(sourceCursor.Handle, cursor.Handle);
        Assert.Equal(sourceCursor.HotSpot, cursor.HotSpot);
        Assert.Equal(sourceCursor.Size, cursor.Size);
        Assert.Null(cursor.Tag);
    }

    [Fact]
    public void Cursor_Ctor_IntPtr_Invalid()
    {
        using Cursor cursor = new(-1000);
        Assert.Equal(-1000, cursor.Handle);
        Assert.Equal(new Point(0, 0), cursor.HotSpot);
        Assert.True(cursor.Size == new Size(32, 32) || cursor.Size == new Size(64, 64));
        Assert.Null(cursor.Tag);
    }

    [Fact]
    public void Cursor_Ctor_ZeroHandle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>("handle", () => new Cursor(IntPtr.Zero));
    }

    public static IEnumerable<object[]> Ctor_ValidFile_TestData()
    {
        yield return new object[] { Path.Combine("bitmaps", "cursor.cur"), Point.Empty };
        yield return new object[] { Path.Combine("bitmaps", "10x16_one_entry_32bit.ico"), new Point(5, 8) };
    }

    [Theory]
    [MemberData(nameof(Ctor_ValidFile_TestData))]
    public void Cursor_Ctor_Stream(string fileName, Point expectedHotSpot)
    {
        using MemoryStream stream = new(File.ReadAllBytes(fileName));
        using Cursor cursor = new(stream);
        Assert.NotEqual(IntPtr.Zero, cursor.Handle);
        Assert.Equal(expectedHotSpot, cursor.HotSpot);
        Assert.True(cursor.Size == new Size(32, 32) || cursor.Size == new Size(64, 64));
        Assert.Null(cursor.Tag);
    }

    [Fact]
    public void Cursor_Ctor_Stream_NonStartPosition()
    {
        using MemoryStream stream = new(File.ReadAllBytes(Path.Combine("bitmaps", "cursor.cur")));
        stream.Position = 5;
        using Cursor cursor = new(stream);
        Assert.NotNull(cursor);
    }

    [Fact]
    public void Cursor_Ctor_EmptyStream_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>("stream", () => new Cursor(new MemoryStream()));
    }

    [Fact]
    public void Cursor_Ctor_NullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("stream", () => new Cursor((Stream)null));
    }

    public static IEnumerable<object[]> Ctor_InvalidFile_TestData()
    {
        yield return new object[] { Path.Combine("bitmaps", "nature24bits.jpg") };
        yield return new object[] { Path.Combine("bitmaps", "nature24bits.gif") };
        yield return new object[] { Path.Combine("bitmaps", "1bit.png") };
        yield return new object[] { Path.Combine("bitmaps", "almogaver24bits.bmp") };
        yield return new object[] { Path.Combine("bitmaps", "telescope_01.wmf") };
        yield return new object[] { Path.Combine("bitmaps", "milkmateya01.emf") };
        yield return new object[] { Path.Combine("bitmaps", "EmptyFile") };
    }

    [Theory]
    [MemberData(nameof(Ctor_InvalidFile_TestData))]
    public void Cursor_Ctor_StreamNotIcon_ThrowsArgumentException(string fileName)
    {
        using MemoryStream stream = new(File.ReadAllBytes(fileName));
        Assert.Throws<ArgumentException>("stream", () => new Cursor(stream));
    }

    [Theory]
    [MemberData(nameof(Ctor_ValidFile_TestData))]
    public void Cursor_Ctor_String(string fileName, Point expectedHotSpot)
    {
        using Cursor cursor = new(fileName);
        Assert.NotEqual(IntPtr.Zero, cursor.Handle);
        Assert.Equal(expectedHotSpot, cursor.HotSpot);
        Assert.True(cursor.Size == new Size(32, 32) || cursor.Size == new Size(64, 64));
        Assert.Null(cursor.Tag);
    }

    [Fact]
    public void Cursor_Ctor_NullFileName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("path", () => new Cursor((string)null));
    }

    [Theory]
    [InlineData("")]
    [InlineData("pa\0th")]
    public void Cursor_Ctor_InvalidFileName_ThrowsArgumentException(string fileName)
    {
        Assert.Throws<ArgumentException>("path", () => new Cursor(fileName));
    }

    [Fact]
    public void Cursor_Ctor_NoSuchFileName_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => new Cursor("NoSuchFile"));
    }

    [Theory]
    [MemberData(nameof(Ctor_InvalidFile_TestData))]
    public void Cursor_Ctor_FileNotIcon_ThrowsArgumentException(string fileName)
    {
        Assert.Throws<ArgumentException>("fileName", () => new Cursor(fileName));
    }

    [Fact]
    public void Cursor_Ctor_Type_String()
    {
        using Cursor cursor = new(typeof(PropertyTabTests), "CustomPropertyTab");
        Assert.NotEqual(IntPtr.Zero, cursor.Handle);
        Assert.Equal(new Point(5, 8), cursor.HotSpot);
        Assert.True(cursor.Size == new Size(32, 32) || cursor.Size == new Size(64, 64));
        Assert.Null(cursor.Tag);
    }

    [Fact]
    public void Cursor_Ctor_NullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("type", () => new Cursor((Type)null, "resource"));
    }

    [Theory]
    [InlineData(typeof(PropertyTabTests), "NoSuchType")]
    [InlineData(typeof(PropertyTabTests), "")]
    [InlineData(typeof(PropertyTabTests), null)]
    public void Cursor_Ctor_NoSuchResource_ThrowsArgumentNullException(Type type, string resource)
    {
        Assert.Throws<ArgumentNullException>("stream", () => new Cursor(type, resource));
    }

    [Fact]
    public void Cursor_Clip_Get_ReturnsExpected()
    {
        Rectangle clip = Cursor.Clip;

        // Cursor location can be negative when it's located on a secondary screen to the left of
        // or above the primary one.
        Assert.True(clip.Width >= 0);
        Assert.True(clip.Height >= 0);
    }

    [Fact]
    public void Cursor_Clip_Set_GetReturnsExpected()
    {
        DPI_AWARENESS_CONTEXT oldDpiAwarenessContext = DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT;
        Rectangle clip = Cursor.Clip;
        try
        {
            // The clipping area is always defined in physical pixels (disregarding DPI) while
            // the virtual screen area depends on the DPI awareness of the thread querying for it.
            // Cannot use DpiAwarenessScope because it rejects to change the DPI awareness.
            oldDpiAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

            // Set non-empty.
            Cursor.Clip = new Rectangle(1, 2, 3, 4);
            Assert.True(Cursor.Clip.X >= 0);
            Assert.True(Cursor.Clip.Y >= 0);
            Assert.True(Cursor.Clip.Width >= 0);
            Assert.True(Cursor.Clip.Height >= 0);

            Rectangle virtualScreen = SystemInformation.VirtualScreen;

            // Set empty.
            Cursor.Clip = new Rectangle(0, 0, 0, 0);
            Assert.Equal(virtualScreen, Cursor.Clip);

            // Set outside normal bounds.
            Cursor.Clip = Rectangle.Inflate(virtualScreen, 10, 10);
            Assert.Equal(virtualScreen, Cursor.Clip);
        }
        finally
        {
            if (oldDpiAwarenessContext != DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT)
                PInvoke.SetThreadDpiAwarenessContextInternal(oldDpiAwarenessContext);

            Cursor.Clip = clip;
        }
    }

    [Fact]
    public void Cursor_Current_Get_ReturnsExpected()
    {
        Cursor cursor = Cursor.Current;
        Assert.NotEqual(IntPtr.Zero, cursor.Handle);
        Point hotSpot = cursor.HotSpot;
        Assert.True(hotSpot.X >= 0 && hotSpot.X <= cursor.Size.Width);
        Assert.True(hotSpot.Y >= 0 && hotSpot.Y <= cursor.Size.Height);
        Assert.True(cursor.Size == new Size(32, 32) || cursor.Size == new Size(64, 64));
        Assert.Null(cursor.Tag);
        Assert.NotSame(cursor, Cursor.Current);
    }

    [Fact]
    public void Cursor_Current_Set_GetReturnsExpected()
    {
        Cursor current = Cursor.Current;
        try
        {
            // Set non-null.
            Cursor value = new(Cursors.AppStarting.Handle);
            Cursor.Current = value;
            Assert.Equal(value.Handle, Cursor.Current.Handle);

            // Set null.
            Cursor.Current = null;
            Assert.Null(Cursor.Current);
        }
        finally
        {
            Cursor.Current = current;
        }
    }

    [Fact]
    public void Cursor_Position_Get_ReturnsExpected()
    {
        Point position = Cursor.Position;
        Rectangle virtualScreen = SystemInformation.VirtualScreen;

        Assert.True(position.X >= virtualScreen.X);
        Assert.True(position.Y >= virtualScreen.Y);
        Assert.True(position.X <= virtualScreen.Right);
        Assert.True(position.Y <= virtualScreen.Bottom);
    }

    [Fact]
    public void Cursor_Position_Set_GetReturnsExpected()
    {
        Point position = Cursor.Position;
        try
        {
            Cursor.Position = new Point(1, 2);
            position = Cursor.Position;
            Assert.True(position.X >= 0);
            Assert.True(position.Y >= 0);

            Rectangle virtualScreen = SystemInformation.VirtualScreen;
            Cursor.Position = new Point(virtualScreen.X - 1, virtualScreen.Y - 1);
            position = Cursor.Position;
            Assert.True(position.X >= virtualScreen.X);
            Assert.True(position.Y >= virtualScreen.Y);
        }
        finally
        {
            Cursor.Position = position;
        }
    }

    [Fact]
    public void Cursor_Show_InvokeMultipleTimes_Success()
    {
        Cursor.Show();
        Cursor.Show();
    }

    [Fact]
    public void Cursor_Hide_InvokeMultipleTimes_Success()
    {
        Cursor.Hide();
        Cursor.Hide();
    }

    [Theory]
    [StringWithNullData]
    public void Cursor_Tag_Set_GetReturnsExpected(object value)
    {
        using Cursor cursor = new(2)
        {
            Tag = value
        };
        Assert.Same(value, cursor.Tag);

        // Set same.
        cursor.Tag = value;
        Assert.Same(value, cursor.Tag);
    }

    [Fact]
    public void Cursor_CopyHandle_Invoke_Success()
    {
        using Cursor sourceCursor = new(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico"));
        IntPtr handle = sourceCursor.CopyHandle();
        Assert.NotEqual(IntPtr.Zero, handle);
        Assert.NotEqual(sourceCursor.Handle, handle);

        using Cursor cursor = new(sourceCursor.Handle);
        Assert.Equal(sourceCursor.Handle, cursor.Handle);
        Assert.Equal(sourceCursor.HotSpot, cursor.HotSpot);
        Assert.Equal(sourceCursor.Size, cursor.Size);
        Assert.Null(cursor.Tag);
    }

    [Fact]
    public void Cursor_Dispose_InvokeOwned_Success()
    {
        Cursor cursor = new(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico"));
        cursor.Dispose();
        Assert.Throws<ObjectDisposedException>(() => cursor.Handle);
        Assert.Throws<ObjectDisposedException>(() => cursor.HotSpot);

        cursor.Dispose();
        Assert.Throws<ObjectDisposedException>(() => cursor.Handle);
        Assert.Throws<ObjectDisposedException>(() => cursor.HotSpot);
    }

    [Fact]
    public void Cursor_Dispose_InvokeNotOwned_Success()
    {
        Cursor cursor = new(2);
        cursor.Dispose();

        // Cursors not owned should not be disposed.
        Assert.NotEqual(IntPtr.Zero, cursor.Handle);
    }

    public static IEnumerable<object[]> Draw_TestData()
    {
        yield return new object[] { Rectangle.Empty };
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(-1, -2, 3, 4) };
        yield return new object[] { new Rectangle(-1, -2, -3, -4) };
        yield return new object[] { new Rectangle(0, 0, Cursors.Default.Size.Width, Cursors.Default.Size.Height) };
        yield return new object[] { new Rectangle(1, 0, Cursors.Default.Size.Width, Cursors.Default.Size.Height) };
        yield return new object[] { new Rectangle(0, 2, 3, Cursors.Default.Size.Height) };
        yield return new object[] { new Rectangle(0, 0, Cursors.Default.Size.Width, 4) };
    }

    [Theory]
    [MemberData(nameof(Draw_TestData))]
    public void Cursor_Draw_InvokeValidCursor_Success(Rectangle rectangle)
    {
        using Cursor cursor = new(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico"));
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        cursor.Draw(graphics, rectangle);
    }

    [Theory]
    [MemberData(nameof(Draw_TestData))]
    public void Cursor_Draw_InvokeInvalidCursor_Success(Rectangle rectangle)
    {
        using Cursor cursor = new(-1000);
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        cursor.Draw(graphics, rectangle);
    }

    [Fact]
    public void Cursor_Draw_NullGraphics_ThrowsArgumentNullException()
    {
        Cursor cursor = Cursors.AppStarting;
        Assert.Throws<ArgumentNullException>("graphics", () => cursor.Draw(null, new Rectangle(Point.Empty, cursor.Size)));
    }

    [Fact]
    public void Cursor_Draw_DisposedGraphics_ThrowsArgumentException()
    {
        Cursor cursor = Cursors.AppStarting;
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();
        Assert.Throws<ArgumentException>(() => cursor.Draw(graphics, new Rectangle(Point.Empty, cursor.Size)));
    }

    [Theory]
    [MemberData(nameof(Draw_TestData))]
    public void Cursor_DrawStretched_InvokeValidCursor_Success(Rectangle rectangle)
    {
        using Cursor cursor = new(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico"));
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        cursor.DrawStretched(graphics, rectangle);
    }

    [Theory]
    [MemberData(nameof(Draw_TestData))]
    public void Cursor_DrawStretched_InvokeInvalidCursor_Success(Rectangle rectangle)
    {
        using Cursor cursor = new(-1000);
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        cursor.DrawStretched(graphics, rectangle);
    }

    [Fact]
    public void Cursor_DrawStretched_NullGraphics_ThrowsArgumentNullException()
    {
        Cursor cursor = Cursors.AppStarting;
        Assert.Throws<ArgumentNullException>("graphics", () => cursor.DrawStretched(null, new Rectangle(Point.Empty, cursor.Size)));
    }

    [Fact]
    public void Cursor_DrawStretched_DisposedGraphics_ThrowsArgumentException()
    {
        Cursor cursor = Cursors.AppStarting;
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();
        Assert.Throws<ArgumentException>(() => cursor.DrawStretched(graphics, new Rectangle(Point.Empty, cursor.Size)));
    }

    public static IEnumerable<object[]> Equals_Object_TestData()
    {
        Cursor cursor = new(Cursors.AppStarting.Handle);
        yield return new object[] { cursor, cursor, true };
        yield return new object[] { cursor, new Cursor(Cursors.AppStarting.Handle), true };
        yield return new object[] { cursor, new Cursor(Cursors.Arrow.Handle), false };

        yield return new object[] { cursor, new(), false };
        yield return new object[] { cursor, null, false };
    }

    [Theory]
    [MemberData(nameof(Equals_Object_TestData))]
    public void Cursor_Equals_InvokeObject_ReturnsExpected(Cursor cursor, object obj, bool expected)
    {
        Assert.Equal(expected, cursor.Equals(obj));
        if (obj is Cursor)
        {
            Assert.Equal(expected, cursor.GetHashCode().Equals(obj.GetHashCode()));
        }
    }

    public static IEnumerable<object[]> Equals_Cursor_TestData()
    {
        Cursor cursor = new(2);
        yield return new object[] { cursor, cursor, true };
        yield return new object[] { cursor, new Cursor(2), true };
        yield return new object[] { cursor, new Cursor(1), false };

        yield return new object[] { null, null, true };
        yield return new object[] { null, cursor, false };
        yield return new object[] { cursor, null, false };
    }

    [Theory]
    [MemberData(nameof(Equals_Cursor_TestData))]
    public void Cursor_Equals_InvokeCursor_ReturnsExpected(Cursor cursor1, Cursor cursor2, bool expected)
    {
        Assert.Equal(expected, cursor1 == cursor2);
        Assert.Equal(!expected, cursor1 != cursor2);
    }

    [Fact]
    public void Cursor_ToString_KnownCursor_ReturnsExpected()
    {
        Cursor cursor = Cursors.AppStarting;
        Assert.Equal("[Cursor: AppStarting]", cursor.ToString());
    }

    [Fact]
    public void Cursor_ToString_CursorFromFile_ReturnsExpected()
    {
        using Cursor cursor = new(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico"));
        Assert.Equal("[Cursor: System.Windows.Forms.Cursor]", cursor.ToString());
    }

    [Fact]
    public void Cursor_ToString_InvalidCursor_DoesNotThrowFormatException()
    {
        using Cursor cursor = new(2);
        _ = cursor.ToString();
    }
}
