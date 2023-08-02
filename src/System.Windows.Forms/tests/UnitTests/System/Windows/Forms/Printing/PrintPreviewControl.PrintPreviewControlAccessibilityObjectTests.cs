using System.Drawing;

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
        Assert.Throws<ArgumentNullException>(() => new PrintPreviewControl.PrintPreviewControlAccessibleObject(null));
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

        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
        Assert.False(control.IsHandleCreated);

        control.MakeOnlyVerticalScrollBarVisible();
        Assert.Equal(verticalScrollBarAO, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
        Assert.Equal(verticalScrollBarAO, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));

        control.MakeOnlyHorizontalScrollBarVisible();
        Assert.Equal(horizontalScrollBarAO, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
        Assert.Equal(horizontalScrollBarAO, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));

        control.MakeBothScrollBarsVisible();
        Assert.Equal(verticalScrollBarAO, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
        Assert.Equal(horizontalScrollBarAO, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));

        control.MakeNoScrollBarsVisible();
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibilityObject_FragmentNavigate_ReturnsConsistent()
    {
        using SubPrintPreviewControl control = new();

        AccessibleObject accessibleObject = control.AccessibilityObject;
        AccessibleObject verticalScrollBarAO = control.VerticalScrollBar.AccessibilityObject;
        AccessibleObject horizontalScrollBarAO = control.HorizontalScrollBar.AccessibilityObject;

        Assert.Null(verticalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(verticalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));

        control.MakeOnlyVerticalScrollBarVisible();
        Assert.Null(verticalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(verticalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));

        control.MakeOnlyHorizontalScrollBarVisible();
        Assert.Null(verticalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(verticalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));

        control.MakeBothScrollBarsVisible();
        Assert.Equal(horizontalScrollBarAO, verticalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(verticalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
        Assert.Null(horizontalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Equal(verticalScrollBarAO, horizontalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));

        Assert.Equal(accessibleObject, verticalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
        Assert.Equal(accessibleObject, horizontalScrollBarAO.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
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
