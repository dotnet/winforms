// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.PrintPreviewControl;
using IScrollProvider = Windows.Win32.UI.Accessibility.IScrollProvider;

namespace System.Windows.Forms.Tests;

public class PrintPreviewControl_PrintPreviewControlAccessibilityObjectTests
{
    [WinFormsFact]
    public void PrintPreviewControlAccessibilityObject_Ctor_Default()
    {
        using PrintPreviewControl control = new();

        AccessibleObject accessibleObject = control.AccessibilityObject;

        Assert.NotNull(accessibleObject);
        Assert.Equal(AccessibleRole.None, accessibleObject.Role);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibilityObject_Ctor_NullControl_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentNullException>(() => new PrintPreviewControlAccessibleObject(null));
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibilityObject_ChildCount_ReturnsExpected()
    {
        using SubPrintPreviewControl control = new();

        AccessibleObject accessibleObject = control.AccessibilityObject;

        Assert.Equal(0, accessibleObject.GetChildCount());
        Assert.False(control.IsHandleCreated);

        control.MakeOnlyVerticalScrollBarVisible();
        Assert.Equal(1, accessibleObject.GetChildCount());

        control.MakeOnlyHorizontalScrollBarVisible();
        Assert.Equal(1, accessibleObject.GetChildCount());

        control.MakeBothScrollBarsVisible();
        Assert.Equal(2, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibilityObject_GetChild_ReturnsExpected()
    {
        using SubPrintPreviewControl control = new();

        AccessibleObject accessibleObject = control.AccessibilityObject;
        AccessibleObject verticalScrollBarAO = control.VerticalScrollBar.AccessibilityObject;
        AccessibleObject horizontalScrollBarAO = control.HorizontalScrollBar.AccessibilityObject;

        Assert.Null(accessibleObject.GetChild(0));
        Assert.False(control.IsHandleCreated);

        control.MakeOnlyVerticalScrollBarVisible();
        Assert.Equal(verticalScrollBarAO, accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));

        control.MakeOnlyHorizontalScrollBarVisible();
        Assert.Equal(horizontalScrollBarAO, accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));

        control.MakeBothScrollBarsVisible();
        Assert.Equal(verticalScrollBarAO, accessibleObject.GetChild(0));
        Assert.Equal(horizontalScrollBarAO, accessibleObject.GetChild(1));
        Assert.Null(accessibleObject.GetChild(2));

        control.MakeNoScrollBarsVisible();
        Assert.Null(accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibilityObject_FragmentNavigate_Children_ReturnsExpected()
    {
        using SubPrintPreviewControl control = new();

        AccessibleObject accessibleObject = control.AccessibilityObject;
        AccessibleObject verticalScrollBarAO = control.VerticalScrollBar.AccessibilityObject;
        AccessibleObject horizontalScrollBarAO = control.HorizontalScrollBar.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);

        control.MakeOnlyVerticalScrollBarVisible();
        Assert.Equal(verticalScrollBarAO, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(verticalScrollBarAO, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        control.MakeOnlyHorizontalScrollBarVisible();
        Assert.Equal(horizontalScrollBarAO, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(horizontalScrollBarAO, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        control.MakeBothScrollBarsVisible();
        Assert.Equal(verticalScrollBarAO, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(horizontalScrollBarAO, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        control.MakeNoScrollBarsVisible();
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibilityObject_FragmentNavigate_ReturnsConsistent()
    {
        using SubPrintPreviewControl control = new();

        AccessibleObject accessibleObject = control.AccessibilityObject;
        AccessibleObject verticalScrollBarAO = control.VerticalScrollBar.AccessibilityObject;
        AccessibleObject horizontalScrollBarAO = control.HorizontalScrollBar.AccessibilityObject;

        Assert.Null(verticalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(verticalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        control.MakeOnlyVerticalScrollBarVisible();
        Assert.Null(verticalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(verticalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        control.MakeOnlyHorizontalScrollBarVisible();
        Assert.Null(verticalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(verticalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        control.MakeBothScrollBarsVisible();
        Assert.Equal(horizontalScrollBarAO, verticalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(verticalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(verticalScrollBarAO, horizontalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(accessibleObject, verticalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject, horizontalScrollBarAO.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibilityObject_ScrollInfo_ReturnsConsistent()
    {
        using SubPrintPreviewControl control = new();

        var accessibleObject = (PrintPreviewControlAccessibleObject)control.AccessibilityObject;
        IScrollProvider.Interface scrollProvider = accessibleObject;
        Assert.False(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ScrollPatternId));

        control.MakeOnlyVerticalScrollBarVisible();
        Assert.True(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ScrollPatternId));

        Assert.False((bool)scrollProvider.HorizontallyScrollable);
        Assert.Equal(0, (int)scrollProvider.HorizontalScrollPercent);
        Assert.Equal(100, (int)scrollProvider.HorizontalViewSize);

        Assert.True((bool)scrollProvider.VerticallyScrollable);
        Assert.InRange((int)scrollProvider.VerticalScrollPercent, 0, 100);
        AssertExtensions.GreaterThanOrEqualTo((int)scrollProvider.VerticalViewSize, 0);

        control.MakeNoScrollBarsVisible();
        Assert.False(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ScrollPatternId));

        control.MakeOnlyHorizontalScrollBarVisible();
        Assert.True(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ScrollPatternId));

        Assert.False((bool)scrollProvider.VerticallyScrollable);
        Assert.Equal(0, (int)scrollProvider.VerticalScrollPercent);
        Assert.Equal(100, (int)scrollProvider.VerticalViewSize);

        Assert.True((bool)scrollProvider.HorizontallyScrollable);
        Assert.InRange((int)scrollProvider.HorizontalScrollPercent, 0, 100);
        AssertExtensions.GreaterThanOrEqualTo((int)scrollProvider.HorizontalViewSize, 0);
    }

    private class SubPrintPreviewControl : PrintPreviewControl
    {
        public SubPrintPreviewControl()
        {
            Size = new(100, 100);
        }

        public new ScrollBar VerticalScrollBar
            => this.TestAccessor().Dynamic._vScrollBar;

        public new ScrollBar HorizontalScrollBar
            => this.TestAccessor().Dynamic._hScrollBar;

        public void MakeNoScrollBarsVisible()
        {
            if (!Created)
            {
                CreateControl();
            }

            SetVirtualSizeNoInvalidate(new Size(50, 50));
        }

        public void MakeOnlyVerticalScrollBarVisible()
        {
            if (!Created)
            {
                CreateControl();
            }

            SetVirtualSizeNoInvalidate(new Size(50, 200));
        }

        public void MakeOnlyHorizontalScrollBarVisible()
        {
            if (!Created)
            {
                CreateControl();
            }

            SetVirtualSizeNoInvalidate(new Size(200, 50));
        }

        public void MakeBothScrollBarsVisible()
        {
            if (!Created)
            {
                CreateControl();
            }

            SetVirtualSizeNoInvalidate(new Size(200, 200));
        }
    }
}
