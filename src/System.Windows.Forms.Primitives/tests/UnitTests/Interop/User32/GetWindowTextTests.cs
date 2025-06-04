// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Primitives.Tests.Interop.User32;

public class GetWindowTextTests
{
    [StaFact]
    public void GetWindowText_DoesNotTruncateText()
    {
        CallGetWindowText(useBeforeGetTextLengthCallback: false);
    }

    [StaFact]
    public void GetWindowText_DoesNotLoopInfinitely()
    {
        CallGetWindowText(useBeforeGetTextLengthCallback: true);
    }

    private void CallGetWindowText(bool useBeforeGetTextLengthCallback)
    {
        const string shortText = "A";

        // Use a long string that exceeds the initial buffer size (16).
        string longText = new('X', 50);

        ChangeWindowTextClass windowClass = new();
        windowClass.Register();
        HWND windowHandle = (HWND)windowClass.CreateWindow(shortText);

        windowClass.BeforeGetTextCallback = () => longText;
        if (useBeforeGetTextLengthCallback)
        {
            windowClass.BeforeGetTextLengthCallback = () => shortText;
        }

        string result = PInvokeCore.GetWindowText(windowHandle);
        PInvoke.DestroyWindow(windowHandle);

        Assert.Equal(longText, result);
    }

    private class ChangeWindowTextClass : WindowClass
    {
        public Func<string>? BeforeGetTextLengthCallback
        {
            get;
            set;
        }

        public Func<string>? BeforeGetTextCallback
        {
            get;
            set;
        }

        protected override LRESULT WNDPROC(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
        {
            switch (msg)
            {
                case PInvokeCore.WM_GETTEXTLENGTH:
                    string? text = BeforeGetTextLengthCallback?.Invoke();
                    if (text is not null)
                    {
                        PInvoke.SetWindowText(hWnd, text);
                    }

                    break;
                case PInvokeCore.WM_GETTEXT:
                    text = BeforeGetTextCallback?.Invoke();
                    if (text is not null)
                    {
                        PInvoke.SetWindowText(hWnd, text);
                    }

                    break;
            }

            return base.WNDPROC(hWnd, msg, wParam, lParam);
        }
    }
}
