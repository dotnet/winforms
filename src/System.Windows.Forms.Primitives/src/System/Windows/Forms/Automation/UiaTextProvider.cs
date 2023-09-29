﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.Automation;

internal abstract unsafe class UiaTextProvider : ITextProvider.Interface
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

    public static WINDOW_EX_STYLE GetWindowExStyle(IHandle<HWND> hWnd) => (WINDOW_EX_STYLE)PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

    public static WINDOW_STYLE GetWindowStyle(IHandle<HWND> hWnd) => (WINDOW_STYLE)PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

    public static SAFEARRAY* RectListToDoubleArray(List<Rectangle> rectArray)
    {
        if (rectArray is null || rectArray.Count == 0)
        {
            return SAFEARRAY.Empty(VARENUM.VT_R8);
        }

        SAFEARRAYBOUND saBound = new()
        {
            cElements = (uint)(rectArray.Count * 4),
            lLbound = 0
        };

        SAFEARRAY* result = PInvoke.SafeArrayCreate(VARENUM.VT_R8, 1, &saBound);

        double[] doubles = new double[rectArray.Count * 4];
        int scan = 0;

        for (int i = 0; i < rectArray.Count; i++)
        {
            double put = rectArray[i].X;
            PInvoke.SafeArrayPutElement(result, &scan, &put).ThrowOnFailure();
            scan++;
            put = rectArray[i].Y;
            PInvoke.SafeArrayPutElement(result, &scan, &put).ThrowOnFailure();
            scan++;
            put = rectArray[i].Width;
            PInvoke.SafeArrayPutElement(result, &scan, &put).ThrowOnFailure();
            scan++;
            put = rectArray[i].Height;
            PInvoke.SafeArrayPutElement(result, &scan, &put).ThrowOnFailure();
            scan++;
        }

        return result;
    }

    /// <summary>
    ///  Bounding rectangles are represented by a VT_ARRAY of doubles in a native VARIANT
    ///  in accessibility interfaces. This method does the conversion. Accessibility will then convert it to an UiaRect.
    ///  https://learn.microsoft.com/windows/win32/api/uiautomationcore/nf-uiautomationcore-irawelementproviderfragment-get_boundingrectangle
    ///  https://learn.microsoft.com/windows/win32/api/uiautomationcore/ns-uiautomationcore-uiarect
    /// </summary>
    internal static VARIANT BoundingRectangleAsArray(Rectangle bounds)
    {
        SAFEARRAYBOUND saBound = new()
        {
            cElements = 4,
            lLbound = 0
        };

        SAFEARRAY* result = PInvoke.SafeArrayCreate(VARENUM.VT_R8, 1, &saBound);
        int i = 0;
        double put = bounds.X;
        PInvoke.SafeArrayPutElement(result, &i, &put).ThrowOnFailure();
        i = 1;
        put = bounds.Y;
        PInvoke.SafeArrayPutElement(result, &i, &put).ThrowOnFailure();
        i = 2;
        put = bounds.Width;
        PInvoke.SafeArrayPutElement(result, &i, &put).ThrowOnFailure();
        i = 3;
        put = bounds.Height;
        PInvoke.SafeArrayPutElement(result, &i, &put).ThrowOnFailure();

        return new VARIANT()
        {
            vt = VARENUM.VT_ARRAY,
            data = new() { parray = result }
        };
    }

    public int SendInput(int inputs, ref INPUT input, int size)
    {
        Span<INPUT> currentInput = stackalloc INPUT[1];
        currentInput[0] = input;

        return (int)PInvoke.SendInput(currentInput, size);
    }

    public unsafe int SendKeyboardInputVK(VIRTUAL_KEY vk, bool press)
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

        return SendInput(1, ref keyboardInput, sizeof(INPUT));
    }
}
