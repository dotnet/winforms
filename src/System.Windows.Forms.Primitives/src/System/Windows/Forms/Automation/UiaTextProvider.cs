// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.Automation;

internal abstract unsafe class UiaTextProvider : ITextProvider.Interface, ITextProvider2.Interface, IManagedWrapper<ITextProvider2, ITextProvider>
{
    /// <summary>
    ///  The value of a width of an end of a text line as 2 px to a ScreenReader can show it.
    /// </summary>
    public const int EndOfLineWidth = 2;

    public abstract HRESULT GetSelection(SAFEARRAY** pRetVal);

    public abstract HRESULT GetVisibleRanges(SAFEARRAY** pRetVal);

    public abstract HRESULT RangeFromChild(IRawElementProviderSimple* childElement, ITextRangeProvider** pRetVal);

    public abstract HRESULT RangeFromPoint(UiaPoint point, ITextRangeProvider** pRetVal);

    public abstract ITextRangeProvider* DocumentRange { get; }

    public abstract SupportedTextSelection SupportedTextSelection { get; }

    public abstract Rectangle BoundingRectangle { get; }

    public abstract int FirstVisibleLine { get; }

    public abstract bool IsMultiline { get; }

    public abstract bool IsReadingRTL { get; }

    public abstract bool IsReadOnly { get; }

    public abstract bool IsScrollable { get; }

    public abstract int LinesPerPage { get; }

    public abstract int LinesCount { get; }

    public abstract LOGFONTW Logfont { get; }

    public abstract string Text { get; }

    public abstract int TextLength { get; }

    public abstract WINDOW_EX_STYLE WindowExStyle { get; }

    public abstract WINDOW_STYLE WindowStyle { get; }

    public abstract int GetLineFromCharIndex(int charIndex);

    public abstract int GetLineIndex(int line);

    public abstract Point GetPositionFromChar(int charIndex);

    public abstract Point GetPositionFromCharForUpperRightCorner(int startCharIndex, string text);

    public abstract void GetVisibleRangePoints(out int visibleStart, out int visibleEnd);

    public abstract bool LineScroll(int charactersHorizontal, int linesVertical);

    public abstract Point PointToScreen(Point pt);

    public abstract Rectangle RectangleToScreen(Rectangle rect);

    public abstract void SetSelection(int start, int end);

    public static WINDOW_EX_STYLE GetWindowExStyle(IHandle<HWND> hWnd) =>
        (WINDOW_EX_STYLE)PInvokeCore.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

    public static WINDOW_STYLE GetWindowStyle(IHandle<HWND> hWnd) =>
        (WINDOW_STYLE)PInvokeCore.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

    public static SafeArrayScope<double> RectListToDoubleArray(List<Rectangle> rectArray)
    {
        if (rectArray is null || rectArray.Count == 0)
        {
            return new(SAFEARRAY.CreateEmpty(VARENUM.VT_R8));
        }

        SafeArrayScope<double> result = new((uint)(rectArray.Count * 4));
        int scan = 0;
        for (int i = 0; i < rectArray.Count; i++)
        {
            result[scan++] = rectArray[i].X;
            result[scan++] = rectArray[i].Y;
            result[scan++] = rectArray[i].Width;
            result[scan++] = rectArray[i].Height;
        }

        return result;
    }

    /// <summary>
    ///  Bounding rectangles are represented by a VT_ARRAY of doubles in a native VARIANT
    ///  in accessibility interfaces. This method does the conversion. Accessibility will then convert it to an UiaRect.
    ///  https://learn.microsoft.com/windows/win32/api/uiautomationcore/nf-uiautomationcore-irawelementproviderfragment-get_boundingrectangle
    ///  https://learn.microsoft.com/windows/win32/api/uiautomationcore/ns-uiautomationcore-uiarect
    /// </summary>
    internal static SafeArrayScope<double> BoundingRectangleAsArray(Rectangle bounds)
    {
        SafeArrayScope<double> result = new(4);
        result[0] = bounds.X;
        result[1] = bounds.Y;
        result[2] = bounds.Width;
        result[3] = bounds.Height;
        return result;
    }

    /// <inheritdoc cref="BoundingRectangleAsArray(Rectangle)"/>
    internal static VARIANT BoundingRectangleAsVariant(Rectangle bounds)
        => (VARIANT)BoundingRectangleAsArray(bounds);

    public static int SendInput(ref INPUT input)
    {
        fixed (INPUT* i = &input)
        {
            return (int)PInvoke.SendInput(1, i, sizeof(INPUT));
        }
    }

    public static unsafe int SendKeyboardInputVK(VIRTUAL_KEY vk, bool press)
    {
        INPUT keyboardInput = default;

        keyboardInput.type = INPUT_TYPE.INPUT_KEYBOARD;
        keyboardInput.Anonymous.ki.wVk = vk;
        keyboardInput.Anonymous.ki.wScan = 0;
        keyboardInput.Anonymous.ki.dwFlags = press ? 0 : KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP;

        if (VirtualKeyUtilities.IsExtendedKey(vk))
        {
            keyboardInput.Anonymous.ki.dwFlags |= KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY;
        }

        keyboardInput.Anonymous.ki.time = 0;
        keyboardInput.Anonymous.ki.dwExtraInfo = UIntPtr.Zero;

        return SendInput(ref keyboardInput);
    }

    public abstract HRESULT RangeFromAnnotation(IRawElementProviderSimple* annotationElement, ITextRangeProvider** pRetVal);

    public abstract HRESULT GetCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal);
}
