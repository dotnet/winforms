// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.VisualStyles.Tests;

// NB: doesn't require thread affinity
public class VisualStyleRendererTests
{
    public static IEnumerable<object[]> Ctor_VisualStyleElement_TestData()
    {
        yield return new object[] { VisualStyleElement.Button.PushButton.Hot };
        yield return new object[] { VisualStyleElement.Button.PushButton.Normal };
        yield return new object[] { VisualStyleElement.Button.RadioButton.CheckedHot };
        yield return new object[] { VisualStyleElement.Button.RadioButton.CheckedNormal };
        yield return new object[] { VisualStyleElement.ComboBox.DropDownButton.Hot };
        yield return new object[] { VisualStyleElement.ComboBox.DropDownButton.Normal };
        yield return new object[] { VisualStyleElement.CreateElement("BUTTON", 0, int.MinValue) };
        yield return new object[] { VisualStyleElement.CreateElement("BUTTON", 0, int.MaxValue) };
    }

    [Theory]
    [MemberData(nameof(Ctor_VisualStyleElement_TestData))]
    public void VisualStyleRenderer_Ctor_String_Int_Int(VisualStyleElement element)
    {
        VisualStyleRenderer renderer = new(element.ClassName, element.Part, element.State);
        Assert.Equal(element.ClassName, renderer.Class);
        Assert.Equal(element.Part, renderer.Part);
        Assert.Equal(element.State, renderer.State);
        Assert.Equal(0, renderer.LastHResult);
        Assert.NotEqual(IntPtr.Zero, renderer.Handle);
    }

    [Fact]
    public void VisualStyleRenderer_Ctor_NullClassName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("className", () => new VisualStyleRenderer(null, 0, 0));
    }

    public static IEnumerable<object[]> Ctor_InvalidElement_TestData()
    {
        yield return new object[] { VisualStyleElement.CreateElement("", 0, 0) };
        yield return new object[] { VisualStyleElement.CreateElement("NoSuchClassName", 0, 0) };
        yield return new object[] { VisualStyleElement.CreateElement("BUTTON", -1, 0) };
        yield return new object[] { VisualStyleElement.CreateElement("BUTTON", int.MinValue, 0) };
        yield return new object[] { VisualStyleElement.CreateElement("BUTTON", int.MaxValue, 0) };
    }

    [Theory]
    [MemberData(nameof(Ctor_InvalidElement_TestData))]
    public void VisualStyleRenderer_Ctor_InvalidClassNamePartOrState_ThrowsArgumentException(VisualStyleElement element)
    {
        Assert.Throws<ArgumentException>(() => new VisualStyleRenderer(element.ClassName, element.Part, element.State));
    }

    [Theory]
    [MemberData(nameof(Ctor_VisualStyleElement_TestData))]
    public void VisualStyleRenderer_Ctor_VisualStyleElement(VisualStyleElement element)
    {
        VisualStyleRenderer renderer = new(element);
        Assert.Equal(element.ClassName, renderer.Class);
        Assert.Equal(element.Part, renderer.Part);
        Assert.Equal(element.State, renderer.State);
        Assert.Equal(0, renderer.LastHResult);
        Assert.NotEqual(IntPtr.Zero, renderer.Handle);
    }

    [Fact]
    public void VisualStyleRenderer_Ctor_NullElement_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => new VisualStyleRenderer(null));
    }

    [Theory]
    [MemberData(nameof(Ctor_InvalidElement_TestData))]
    public void VisualStyleRenderer_Ctor_InvalidElement_ThrowsArgumentException(VisualStyleElement element)
    {
        Assert.Throws<ArgumentException>(() => new VisualStyleRenderer(element));
    }

    [Fact]
    public void VisualStyleRenderer_IsSupported_Get_ReturnsExpected()
    {
        bool result = VisualStyleRenderer.IsSupported;
        Assert.True(result);
        Assert.Equal(result, VisualStyleRenderer.IsSupported);
    }

    public static IEnumerable<object[]> IsElementDefined_TestData()
    {
        yield return new object[] { VisualStyleElement.Button.PushButton.Hot, true };
        yield return new object[] { VisualStyleElement.Button.PushButton.Normal, true };
        yield return new object[] { VisualStyleElement.Button.RadioButton.CheckedHot, true };
        yield return new object[] { VisualStyleElement.Button.RadioButton.CheckedNormal, true };
        yield return new object[] { VisualStyleElement.ComboBox.DropDownButton.Hot, true };
        yield return new object[] { VisualStyleElement.ComboBox.DropDownButton.Normal, true };

        yield return new object[] { VisualStyleElement.CreateElement(string.Empty, 0, 0), false };
        yield return new object[] { VisualStyleElement.CreateElement("NoSuchName", 0, 0), false };
    }

    [Theory]
    [MemberData(nameof(IsElementDefined_TestData))]
    public void VisualStyleRenderer_IsElementDefined_Invoke_ReturnsExpected(VisualStyleElement element, bool expected)
    {
        Assert.Equal(expected, VisualStyleRenderer.IsElementDefined(element));
    }

    [Fact]
    public void VisualStyleRenderer_IsElementDefined_NullElement_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("element", () => VisualStyleRenderer.IsElementDefined(null));
    }

    public static IEnumerable<object[]> DrawBackground_IDeviceContext_Rectangle_TestData()
    {
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 0, 4) };
        yield return new object[] { new Rectangle(0, 0, -1, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 0) };
        yield return new object[] { new Rectangle(0, 0, 3, -1) };
        yield return new object[] { new Rectangle(0, 0, 0, 0) };
        yield return new object[] { new Rectangle(0, 0, -1, -1) };
        yield return new object[] { new Rectangle(-1, -2, 3, 4) };
    }

    [Theory]
    [MemberData(nameof(DrawBackground_IDeviceContext_Rectangle_TestData))]
    public void VisualStyleRenderer_DrawBackground_InvokeIDeviceContextRectangle_Success(Rectangle bounds)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        renderer.DrawBackground(graphics, bounds);
        Assert.Equal(0, renderer.LastHResult);
    }

    public static IEnumerable<object[]> DrawBackground_IDeviceContext_Rectangle_Rectangle_TestData()
    {
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 4), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 0, 4), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, -1, 4), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 0), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, -1), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 0, 0), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, -1, -1), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(-1, -2, 3, 4), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(0, 0, 0, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(0, 0, -1, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(0, 0, 3, 0) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(0, 0, 3, -1) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(0, 0, 0, 0) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(0, 0, -1, -1) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(-1, -2, 3, 4) };
    }

    [Theory]
    [MemberData(nameof(DrawBackground_IDeviceContext_Rectangle_Rectangle_TestData))]
    public void VisualStyleRenderer_DrawBackground_InvokeIDeviceContextRectangleRectangle_Success(Rectangle bounds, Rectangle clipBounds)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        renderer.DrawBackground(graphics, bounds, clipBounds);
        Assert.Equal(0, renderer.LastHResult);
    }

    [Fact]
    public void VisualStyleRenderer_DrawBackground_NullDc_ThrowsArgumentNullException()
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawBackground(null, new Rectangle(1, 2, 3, 4)));
        Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawBackground(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4)));
    }

    public static IEnumerable<object[]> DrawEdge_TestData()
    {
        yield return new object[] { new Rectangle(1, 2, 3, 4), Edges.Left, EdgeStyle.Raised, EdgeEffects.None };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Edges.Top, EdgeStyle.Bump, EdgeEffects.Mono };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Edges.Left | Edges.Top | Edges.Right | Edges.Bottom | Edges.Diagonal, EdgeStyle.Sunken, EdgeEffects.FillInterior | EdgeEffects.Flat | EdgeEffects.Soft | EdgeEffects.Mono };
        yield return new object[] { new Rectangle(0, 0, 3, 4), Edges.Left, EdgeStyle.Raised, EdgeEffects.None };
        yield return new object[] { new Rectangle(0, 0, 0, 4), Edges.Left, EdgeStyle.Raised, EdgeEffects.None };
        yield return new object[] { new Rectangle(0, 0, -1, 4), Edges.Left, EdgeStyle.Raised, EdgeEffects.None };
        yield return new object[] { new Rectangle(0, 0, 3, 0), Edges.Left, EdgeStyle.Raised, EdgeEffects.None };
        yield return new object[] { new Rectangle(0, 0, 3, -1), Edges.Left, EdgeStyle.Raised, EdgeEffects.None };
        yield return new object[] { new Rectangle(0, 0, 0, 0), Edges.Left, EdgeStyle.Raised, EdgeEffects.None };
        yield return new object[] { new Rectangle(0, 0, -1, -1), Edges.Left, EdgeStyle.Raised, EdgeEffects.None };
        yield return new object[] { new Rectangle(-1, -2, 3, 4), Edges.Left, EdgeStyle.Raised, EdgeEffects.None };
    }

    [Theory]
    [MemberData(nameof(DrawEdge_TestData))]
    public void VisualStyleRenderer_DrawEdge_Invoke_Success(Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle result = renderer.DrawEdge(graphics, bounds, edges, style, effects);
        Assert.Equal(0, renderer.LastHResult);
    }

    [Fact]
    public void VisualStyleRenderer_DrawEdge_NullDc_ThrowsArgumentNullException()
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawEdge(null, new Rectangle(1, 2, 3, 4), Edges.Top, EdgeStyle.Bump, EdgeEffects.FillInterior));
    }

    [Theory]
    [InvalidEnumData<Edges>]
    public void VisualStyleRenderer_DrawEdge_InvalidEdges_ThrowsInvalidEnumArgumentException(Edges edges)
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Assert.Throws<InvalidEnumArgumentException>("edges", () => renderer.DrawEdge(graphics, new Rectangle(1, 2, 3, 4), edges, EdgeStyle.Bump, EdgeEffects.FillInterior));
    }

    [Theory]
    [InvalidEnumData<EdgeStyle>]
    public void VisualStyleRenderer_DrawEdge_InvalidStyle_ThrowsInvalidEnumArgumentException(EdgeStyle style)
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Assert.Throws<InvalidEnumArgumentException>("style", () => renderer.DrawEdge(graphics, new Rectangle(1, 2, 3, 4), Edges.Bottom, style, EdgeEffects.FillInterior));
    }

    [Theory]
    [InvalidEnumData<EdgeEffects>]
    public void VisualStyleRenderer_DrawEdge_InvalidEffects_ThrowsInvalidEnumArgumentException(EdgeEffects effects)
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Assert.Throws<InvalidEnumArgumentException>("effects", () => renderer.DrawEdge(graphics, new Rectangle(1, 2, 3, 4), Edges.Bottom, EdgeStyle.Bump, effects));
    }

    public static IEnumerable<object[]> DrawImage_IDeviceContext_Rectangle_TestData()
    {
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 0, 4) };
        yield return new object[] { new Rectangle(0, 0, -1, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 0) };
        yield return new object[] { new Rectangle(0, 0, 3, -1) };
        yield return new object[] { new Rectangle(0, 0, 0, 0) };
        yield return new object[] { new Rectangle(0, 0, -1, -1) };
        yield return new object[] { new Rectangle(-1, -2, 3, 4) };
    }

    [Theory]
    [MemberData(nameof(DrawBackground_IDeviceContext_Rectangle_TestData))]
    public void VisualStyleRenderer_DrawImage_InvokeIDeviceContextRectangleImage_Success(Rectangle bounds)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using Bitmap image = new(10, 10);
        renderer.DrawImage(graphics, bounds, image);
        Assert.Equal(0, renderer.LastHResult);
    }

    [Theory]
    [MemberData(nameof(DrawBackground_IDeviceContext_Rectangle_TestData))]
    public void VisualStyleRenderer_DrawImage_InvokeIDeviceContextRectangleImageListInt_Success(Rectangle bounds)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        renderer.DrawImage(graphics, bounds, imageList, 0);
        Assert.Equal(0, renderer.LastHResult);
    }

    [Fact]
    public void VisualStyleRenderer_DrawImage_NullG_ThrowsArgumentNullException()
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        Assert.Throws<ArgumentNullException>("g", () => renderer.DrawImage(null, new Rectangle(1, 2, 3, 4), image));
        Assert.Throws<ArgumentNullException>("g", () => renderer.DrawImage(null, new Rectangle(1, 2, 3, 4), imageList, 0));
    }

    [Fact]
    public void VisualStyleRenderer_DrawImage_NullImage_ThrowsArgumentNullException()
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap image = new(10, 10);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Assert.Throws<ArgumentNullException>("image", () => renderer.DrawImage(graphics, new Rectangle(1, 2, 3, 4), null));
    }

    [Fact]
    public void VisualStyleRenderer_DrawImage_NullImageList_ThrowsArgumentNullException()
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap image = new(10, 10);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Assert.Throws<ArgumentNullException>("imageList", () => renderer.DrawImage(graphics, new Rectangle(1, 2, 3, 4), null, 0));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void VisualStyleRenderer_DrawImage_InvalidImageIndex_ThrowsArgumentNullException(int imageIndex)
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        Assert.Throws<ArgumentOutOfRangeException>("imageIndex", () => renderer.DrawImage(graphics, new Rectangle(1, 2, 3, 4), imageList, imageIndex));
    }

    public static IEnumerable<object[]> DrawParentBackground_TestData()
    {
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 0, 4) };
        yield return new object[] { new Rectangle(0, 0, -1, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 0) };
        yield return new object[] { new Rectangle(0, 0, 3, -1) };
        yield return new object[] { new Rectangle(0, 0, 0, 0) };
        yield return new object[] { new Rectangle(0, 0, -1, -1) };
        yield return new object[] { new Rectangle(-1, -2, 3, 4) };
    }

    [Theory]
    [MemberData(nameof(DrawParentBackground_TestData))]
    public void VisualStyleRenderer_DrawParentBackgroundInvokeIDeviceContextRectangleChildWithoutHandle_Success(Rectangle bounds)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using Control childControl = new();
        renderer.DrawParentBackground(graphics, bounds, childControl);
        Assert.False(childControl.IsHandleCreated);
        Assert.Equal(0, renderer.LastHResult);
    }

    public static IEnumerable<object[]> DrawParentBackground_WithHandle_TestData()
    {
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(0, 0, 0, 4) };
        yield return new object[] { new Rectangle(0, 0, -1, 4) };
        yield return new object[] { new Rectangle(0, 0, 3, 0) };
        yield return new object[] { new Rectangle(0, 0, 3, -1) };
        yield return new object[] { new Rectangle(0, 0, 0, 0) };
        yield return new object[] { new Rectangle(0, 0, -1, -1) };
        yield return new object[] { new Rectangle(-1, -2, 3, 4) };
    }

    [Theory]
    [MemberData(nameof(DrawParentBackground_WithHandle_TestData))]
    public void VisualStyleRenderer_DrawParentBackground_InvokeIDeviceContextRectangleChildWithHandle_Success(Rectangle bounds)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using Control childControl = new();
        Assert.NotEqual(IntPtr.Zero, childControl.Handle);
        renderer.DrawParentBackground(graphics, bounds, childControl);
        Assert.True(childControl.IsHandleCreated);
        Assert.Equal(0, renderer.LastHResult);
    }

    [Fact]
    public void VisualStyleRenderer_DrawParentBackground_NullDc_ThrowsArgumentNullException()
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Control childControl = new();
        Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawParentBackground(null, new Rectangle(1, 2, 3, 4), childControl));
    }

    public static IEnumerable<object[]> DrawText_IDeviceContext_Rectangle_String_TestData()
    {
        foreach (string textToDraw in new string[] { null, string.Empty, "text" })
        {
            yield return new object[] { new Rectangle(1, 2, 3, 4), textToDraw };
            yield return new object[] { new Rectangle(0, 0, 3, 4), textToDraw };
            yield return new object[] { new Rectangle(0, 0, 0, 4), textToDraw };
            yield return new object[] { new Rectangle(0, 0, -1, 4), textToDraw };
            yield return new object[] { new Rectangle(0, 0, 3, 0), textToDraw };
            yield return new object[] { new Rectangle(0, 0, 3, -1), textToDraw };
            yield return new object[] { new Rectangle(0, 0, 0, 0), textToDraw };
            yield return new object[] { new Rectangle(0, 0, -1, -1), textToDraw };
            yield return new object[] { new Rectangle(-1, -2, 3, 4), textToDraw };
        }
    }

    [Theory]
    [MemberData(nameof(DrawText_IDeviceContext_Rectangle_String_TestData))]
    public void VisualStyleRenderer_DrawText_InvokeIDeviceContextRectangleString_Success(Rectangle bounds, string textToDraw)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        renderer.DrawText(graphics, bounds, textToDraw);
        Assert.Equal(0, renderer.LastHResult);
    }

    public static IEnumerable<object[]> DrawText_IDeviceContext_Rectangle_String_Bool_TestData()
    {
        foreach (string textToDraw in new string[] { null, string.Empty, "text" })
        {
            foreach (bool drawDisabled in new bool[] { true, false })
            {
                yield return new object[] { new Rectangle(1, 2, 3, 4), textToDraw, drawDisabled };
                yield return new object[] { new Rectangle(0, 0, 3, 4), textToDraw, drawDisabled };
                yield return new object[] { new Rectangle(0, 0, 0, 4), textToDraw, drawDisabled };
                yield return new object[] { new Rectangle(0, 0, -1, 4), textToDraw, drawDisabled };
                yield return new object[] { new Rectangle(0, 0, 3, 0), textToDraw, drawDisabled };
                yield return new object[] { new Rectangle(0, 0, 3, -1), textToDraw, drawDisabled };
                yield return new object[] { new Rectangle(0, 0, 0, 0), textToDraw, drawDisabled };
                yield return new object[] { new Rectangle(0, 0, -1, -1), textToDraw, drawDisabled };
                yield return new object[] { new Rectangle(-1, -2, 3, 4), textToDraw, drawDisabled };
            }
        }
    }

    [Theory]
    [MemberData(nameof(DrawText_IDeviceContext_Rectangle_String_Bool_TestData))]
    public void VisualStyleRenderer_DrawText_InvokeIDeviceContextRectangleStringBool_Success(Rectangle bounds, string textToDraw, bool drawDisabled)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        renderer.DrawText(graphics, bounds, textToDraw, drawDisabled);
        Assert.Equal(0, renderer.LastHResult);
    }

    public static IEnumerable<object[]> DrawText_IDeviceContext_Rectangle_String_Bool_TextFormatFlags_TestData()
    {
        foreach (string textToDraw in new string[] { null, string.Empty, "text" })
        {
            foreach (bool drawDisabled in new bool[] { true, false })
            {
                yield return new object[] { new Rectangle(1, 2, 3, 4), textToDraw, drawDisabled, TextFormatFlags.Default };
                yield return new object[] { new Rectangle(0, 0, 3, 4), textToDraw, drawDisabled, TextFormatFlags.VerticalCenter };
                yield return new object[] { new Rectangle(0, 0, 0, 4), textToDraw, drawDisabled, TextFormatFlags.Default };
                yield return new object[] { new Rectangle(0, 0, -1, 4), textToDraw, drawDisabled, TextFormatFlags.Default };
                yield return new object[] { new Rectangle(0, 0, 3, 0), textToDraw, drawDisabled, TextFormatFlags.Default };
                yield return new object[] { new Rectangle(0, 0, 3, -1), textToDraw, drawDisabled, TextFormatFlags.Default };
                yield return new object[] { new Rectangle(0, 0, 0, 0), textToDraw, drawDisabled, TextFormatFlags.Default };
                yield return new object[] { new Rectangle(0, 0, -1, -1), textToDraw, drawDisabled, TextFormatFlags.Default };
                yield return new object[] { new Rectangle(-1, -2, 3, 4), textToDraw, drawDisabled, TextFormatFlags.Default };
            }
        }
    }

    [Theory]
    [MemberData(nameof(DrawText_IDeviceContext_Rectangle_String_Bool_TextFormatFlags_TestData))]
    public void VisualStyleRenderer_DrawText_InvokeIDeviceContextRectangleStringBoolTextFormatFlags_Success(Rectangle bounds, string textToDraw, bool drawDisabled, TextFormatFlags flags)
    {
        // Don't verify anything, just make sure the interop call succeeds.
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        renderer.DrawText(graphics, bounds, textToDraw, drawDisabled, flags);
        Assert.Equal(0, renderer.LastHResult);
    }

    [Fact]
    public void VisualStyleRenderer_DrawText_NullDc_ThrowsArgumentNullException()
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);
        using Bitmap image = new(10, 10);
        Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawText(null, new Rectangle(1, 2, 3, 4), "text"));
        Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawText(null, new Rectangle(1, 2, 3, 4), "text", true));
        Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawText(null, new Rectangle(1, 2, 3, 4), "text", false));
        Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawText(null, new Rectangle(1, 2, 3, 4), "text", true, TextFormatFlags.Default));
        Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawText(null, new Rectangle(1, 2, 3, 4), "text", false, TextFormatFlags.Default));
    }

    [Fact]
    public void VisualStyleRenderer_GetMargins()
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Normal);

        using Form form = new();
        using Graphics graphics = form.CreateGraphics();

        // GetMargins should not throw an exception.
        // See https://github.com/dotnet/winforms/issues/526.
        renderer.GetMargins(graphics, MarginProperty.SizingMargins);
    }

    [Theory]
    [MemberData(nameof(Ctor_VisualStyleElement_TestData))]
    public void VisualStyleRenderer_SetParameters_InvokeStringIntInt_Success(VisualStyleElement element)
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Hot);
        renderer.SetParameters(element.ClassName, element.Part, element.State);
        Assert.Equal(element.ClassName, renderer.Class);
        Assert.Equal(element.Part, renderer.Part);
        Assert.Equal(element.State, renderer.State);
        Assert.Equal(0, renderer.LastHResult);
        Assert.NotEqual(IntPtr.Zero, renderer.Handle);
    }

    [Theory]
    [MemberData(nameof(Ctor_InvalidElement_TestData))]
    public void VisualStyleRenderer_SetParameters_InvalidClassNamePartState_ThrowsArgumentException(VisualStyleElement element)
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Hot);
        Assert.Throws<ArgumentException>(() => renderer.SetParameters(element.ClassName, element.Part, element.State));
    }

    [Theory]
    [MemberData(nameof(Ctor_VisualStyleElement_TestData))]
    public void VisualStyleRenderer_SetParameters_InvokeVisualStyleElement_Success(VisualStyleElement element)
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Hot);
        renderer.SetParameters(element);
        Assert.Equal(element.ClassName, renderer.Class);
        Assert.Equal(element.Part, renderer.Part);
        Assert.Equal(element.State, renderer.State);
        Assert.Equal(0, renderer.LastHResult);
        Assert.NotEqual(IntPtr.Zero, renderer.Handle);
    }

    [Fact]
    public void VisualStyleRenderer_SetParameters_NullElement_ThrowsArgumentNullException()
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Hot);
        Assert.Throws<ArgumentNullException>("element", () => renderer.SetParameters(null));
    }

    [Theory]
    [MemberData(nameof(Ctor_InvalidElement_TestData))]
    public void VisualStyleRenderer_SetParameters_InvalidElement_ThrowsArgumentException(VisualStyleElement element)
    {
        VisualStyleRenderer renderer = new(VisualStyleElement.Button.PushButton.Hot);
        Assert.Throws<ArgumentException>(() => renderer.SetParameters(element));
    }

    [Fact]
    public void VisualStyleRenderer_IsBackgroundPartiallyTransparent_Invoke_ReturnsExpected()
    {
        VisualStyleRenderer renderer = new("BUTTON", 0, 0);
        Assert.False(renderer.IsBackgroundPartiallyTransparent());
    }

    [Fact]
    public void VisualStyleRenderer_GetFont_for_TextFont()
    {
        VisualStyleRenderer renderer = new("TEXTSTYLE", 1, 0);
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Font font = renderer.GetFont(graphics, FontProperty.TextFont);

        Assert.NotNull(font);
    }

    [Theory]
    [InvalidEnumData<FontProperty>]
    public void VisualStyleRenderer_GetFont_for_InvalidFontProperty(FontProperty value)
    {
        VisualStyleRenderer renderer = new("TEXTSTYLE", 1, 0);
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);

        Assert.Throws<InvalidEnumArgumentException>("prop", () => renderer.GetFont(graphics, value));
    }
}
