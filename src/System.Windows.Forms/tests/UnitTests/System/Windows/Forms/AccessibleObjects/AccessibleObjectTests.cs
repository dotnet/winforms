// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using Accessibility;
using Moq;
using Windows.Win32.System.Variant;
using UIA = Windows.Win32.UI.Accessibility;
using UIA_PROPERTY_ID = Windows.Win32.UI.Accessibility.UIA_PROPERTY_ID;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public partial class AccessibleObjectTests
{
    [WinFormsFact]
    public void AccessibleObject_Ctor_Default()
    {
        AccessibleObject accessibleObject = new();
        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Null(accessibleObject.Name);
        Assert.Null(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.None, accessibleObject.Role);
        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.Empty(accessibleObject.Value);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void AccessibleObject_Name_Set_GetReturnsNull(string value)
    {
        AccessibleObject accessibleObject = new()
        {
            Name = value
        };
        Assert.Null(accessibleObject.Name);

        // Set same.
        accessibleObject.Name = value;
        Assert.Null(accessibleObject.Name);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void AccessibleObject_Value_Set_GetReturnsEmpty(string value)
    {
        AccessibleObject accessibleObject = new()
        {
            Value = value
        };
        Assert.Empty(accessibleObject.Value);

        // Set same.
        accessibleObject.Value = value;
        Assert.Empty(accessibleObject.Value);
    }

    [WinFormsFact]
    public void AccessibleObject_DoDefaultAction_InvokeDefault_Nop()
    {
        AccessibleObject accessibleObject = new();
        accessibleObject.DoDefaultAction();
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void AccessibleObject_GetChild_InvokeDefault_ReturnsNull(int index)
    {
        AccessibleObject accessibleObject = new();
        Assert.Null(accessibleObject.GetChild(index));
    }

    [WinFormsFact]
    public void AccessibleObject_GetChildCount_InvokeDefault_ReturnsMinusOne()
    {
        AccessibleObject accessibleObject = new();
        Assert.Equal(-1, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void AccessibleObject_GetFocused_InvokeDefault_ReturnsNull()
    {
        AccessibleObject accessibleObject = new();
        Assert.Null(accessibleObject.GetFocused());
    }

    [WinFormsFact]
    public void AccessibleObject_GetFocused_InvokeDefaultWithNoChildren_ReturnsExpected()
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(0);
        mockAccessibleObject
            .Setup(a => a.State)
            .CallBase();
        mockAccessibleObject
            .Setup(a => a.GetFocused())
            .CallBase();
        Assert.Null(mockAccessibleObject.Object.GetFocused());
    }

    [WinFormsFact]
    public void AccessibleObject_GetFocused_InvokeDefaultWithNoChildrenFocused_ReturnsExpected()
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(0);
        mockAccessibleObject
            .Setup(a => a.State)
            .Returns(AccessibleStates.Focused);
        mockAccessibleObject
            .Setup(a => a.GetFocused())
            .CallBase();
        Assert.Same(mockAccessibleObject.Object, mockAccessibleObject.Object.GetFocused());
    }

    [WinFormsFact]
    public void AccessibleObject_GetFocused_InvokeDefaultWithNoFocusedChildren_ReturnsExpected()
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(new AccessibleObject());
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(2);
            mockAccessibleObject
                .Setup(a => a.State)
                .CallBase();
            mockAccessibleObject
                .Setup(a => a.GetFocused())
                .CallBase();
            Assert.Null(mockAccessibleObject.Object.GetFocused());
        }
    }

    [WinFormsFact]
    public void AccessibleObject_GetFocused_InvokeDefaultWithFocusedChildren_ReturnsExpected()
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.State)
                .Returns(AccessibleStates.Focused);
            Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.State)
                .Returns(AccessibleStates.Focused);

            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(mockAccessibleObjectChild1.Object);
            mockAccessibleObject
                .Setup(a => a.GetChild(2))
                .Returns(mockAccessibleObjectChild2.Object);
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(3);
            mockAccessibleObject
                .Setup(a => a.State)
                .CallBase();
            mockAccessibleObject
                .Setup(a => a.GetFocused())
                .CallBase();
            Assert.Same(mockAccessibleObjectChild1.Object, mockAccessibleObject.Object.GetFocused());
        }
    }

    [WinFormsFact]
    public void AccessibleObject_GetHelpTopic_InvokeDefault_ReturnsMinusOne()
    {
        AccessibleObject accessibleObject = new();
        Assert.Equal(-1, accessibleObject.GetHelpTopic(out string fileName));
        Assert.Null(fileName);
    }

    [WinFormsFact]
    public void AccessibleObject_GetSelected_InvokeDefault_ReturnsNull()
    {
        AccessibleObject accessibleObject = new();
        Assert.Null(accessibleObject.GetSelected());
    }

    [WinFormsFact]
    public void AccessibleObject_GetSelected_InvokeDefaultWithNoChildren_ReturnsExpected()
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(0);
        mockAccessibleObject
            .Setup(a => a.State)
            .CallBase();
        mockAccessibleObject
            .Setup(a => a.GetSelected())
            .CallBase();
        Assert.Null(mockAccessibleObject.Object.GetSelected());
    }

    [WinFormsFact]
    public void AccessibleObject_GetSelected_InvokeDefaultWithNoChildrenFocused_ReturnsExpected()
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(0);
        mockAccessibleObject
            .Setup(a => a.State)
            .Returns(AccessibleStates.Selected);
        mockAccessibleObject
            .Setup(a => a.GetSelected())
            .CallBase();
        Assert.Same(mockAccessibleObject.Object, mockAccessibleObject.Object.GetSelected());
    }

    [WinFormsFact]
    public void AccessibleObject_GetSelected_InvokeDefaultWithNoFocusedChildren_ReturnsExpected()
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(new AccessibleObject());
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(2);
            mockAccessibleObject
                .Setup(a => a.State)
                .CallBase();
            mockAccessibleObject
                .Setup(a => a.GetSelected())
                .CallBase();
            Assert.Null(mockAccessibleObject.Object.GetSelected());
        }
    }

    [WinFormsFact]
    public void AccessibleObject_GetSelected_InvokeDefaultWithFocusedChildren_ReturnsExpected()
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.State)
                .Returns(AccessibleStates.Selected);
            Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.State)
                .Returns(AccessibleStates.Selected);

            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(mockAccessibleObjectChild1.Object);
            mockAccessibleObject
                .Setup(a => a.GetChild(2))
                .Returns(mockAccessibleObjectChild2.Object);
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(3);
            mockAccessibleObject
                .Setup(a => a.State)
                .CallBase();
            mockAccessibleObject
                .Setup(a => a.GetSelected())
                .CallBase();
            Assert.Same(mockAccessibleObjectChild1.Object, mockAccessibleObject.Object.GetSelected());
        }
    }

    [WinFormsTheory]
    [InlineData(-1, -2)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void AccessibleObject_HitTest_InvokeDefault_ReturnsNull(int x, int y)
    {
        AccessibleObject accessibleObject = new();
        Assert.Null(accessibleObject.HitTest(x, y));
    }

    [WinFormsTheory]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 5)]
    public void AccessibleObject_HitTest_InvokeDefaultWithNoChildrenBoundsValid_ReturnsExpected(int x, int y)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Bounds)
            .Returns(new Rectangle(1, 2, 3, 4));
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .CallBase();
        mockAccessibleObject
            .Setup(a => a.HitTest(x, y))
            .CallBase();
        Assert.Same(mockAccessibleObject.Object, mockAccessibleObject.Object.HitTest(x, y));
    }

    [WinFormsTheory]
    [InlineData(-1, -2)]
    [InlineData(0, 0)]
    [InlineData(4, 2)]
    [InlineData(1, 6)]
    [InlineData(4, 5)]
    public void AccessibleObject_HitTest_InvokeDefaultWithNoChildrenBoundsInvalid_ReturnsNull(int x, int y)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Bounds)
            .Returns(new Rectangle(1, 2, 3, 4));
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .CallBase();
        mockAccessibleObject
            .Setup(a => a.HitTest(x, y))
            .CallBase();
        Assert.Null(mockAccessibleObject.Object.HitTest(x, y));
    }

    [WinFormsTheory]
    [InlineData(-1, -2)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void AccessibleObject_HitTest_InvokeDefaultWithNoBoundsChildren_ReturnsExpected(int x, int y)
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(new AccessibleObject());
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(2);
            mockAccessibleObject
                .Setup(a => a.HitTest(x, y))
                .CallBase();
            Assert.Same(mockAccessibleObject.Object, mockAccessibleObject.Object.HitTest(x, y));
        }
    }

    [WinFormsTheory]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 5)]
    public void AccessibleObject_HitTest_InvokeDefaultWithBoundsChildren_ReturnsExpected(int x, int y)
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Bounds)
                .Returns(new Rectangle(1, 2, 3, 4));
            Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.Bounds)
                .Returns(new Rectangle(1, 2, 3, 4));

            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(mockAccessibleObjectChild1.Object);
            mockAccessibleObject
                .Setup(a => a.GetChild(2))
                .Returns(mockAccessibleObjectChild2.Object);
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(3);
            mockAccessibleObject
                .Setup(a => a.HitTest(x, y))
                .CallBase();
            Assert.Same(mockAccessibleObjectChild1.Object, mockAccessibleObject.Object.HitTest(x, y));
        }
    }

    [WinFormsTheory]
    [EnumData<AccessibleNavigation>]
    [InvalidEnumData<AccessibleNavigation>]
    public void AccessibleObject_Navigate_InvokeDefault_ReturnsNull(AccessibleNavigation navdir)
    {
        AccessibleObject accessibleObject = new();
        Assert.Null(accessibleObject.Navigate(navdir));
    }

    public static IEnumerable<object[]> Navigate_Child_TestData()
    {
        yield return new object[] { 0, null };
        yield return new object[] { 0, new AccessibleObject() };
        yield return new object[] { 1, null };
        yield return new object[] { 1, new AccessibleObject() };
        yield return new object[] { 2, null };
        yield return new object[] { 2, new AccessibleObject() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Navigate_Child_TestData))]
    public void AccessibleObject_Navigate_InvokeFirstChild_ReturnsNull(int childCount, AccessibleObject result)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(childCount);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns(result)
            .Verifiable();
        mockAccessibleObject
            .Setup(a => a.Navigate(AccessibleNavigation.FirstChild))
            .CallBase();
        Assert.Same(result, mockAccessibleObject.Object.Navigate(AccessibleNavigation.FirstChild));
        mockAccessibleObject.Verify(a => a.GetChild(0), Times.Once());
    }

    [WinFormsTheory]
    [MemberData(nameof(Navigate_Child_TestData))]
    public void AccessibleObject_Navigate_InvokeLastChild_ReturnsNull(int childCount, AccessibleObject result)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(childCount);
        mockAccessibleObject
            .Setup(a => a.GetChild(childCount - 1))
            .Returns(result)
            .Verifiable();
        mockAccessibleObject
            .Setup(a => a.Navigate(AccessibleNavigation.LastChild))
            .CallBase();
        Assert.Same(result, mockAccessibleObject.Object.Navigate(AccessibleNavigation.LastChild));
        mockAccessibleObject.Verify(a => a.GetChild(childCount - 1), Times.Once());
    }

    public static IEnumerable<object[]> Navigate_WithParent_TestData()
    {
        foreach (int childCount in new int[] { 0, 1, 2 })
        {
            foreach (int parentChildCount in new int[] { -1, 0, 1 })
            {
                yield return new object[] { childCount, parentChildCount, AccessibleNavigation.Previous };
                yield return new object[] { childCount, parentChildCount, AccessibleNavigation.Up };
                yield return new object[] { childCount, parentChildCount, AccessibleNavigation.Left };
                yield return new object[] { childCount, parentChildCount, AccessibleNavigation.Next };
                yield return new object[] { childCount, parentChildCount, AccessibleNavigation.Down };
                yield return new object[] { childCount, parentChildCount, AccessibleNavigation.Right };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Navigate_WithParent_TestData))]
    public void AccessibleObject_Navigate_InvokeWithParent_ReturnsNull(int childCount, int parentChildCount, AccessibleNavigation navdir)
    {
        Mock<AccessibleObject> mockParentAccessibleObject = new(MockBehavior.Strict);
        mockParentAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(parentChildCount)
            .Verifiable();

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(childCount);
        mockAccessibleObject
            .Setup(a => a.Parent)
            .Returns(mockParentAccessibleObject.Object)
            .Verifiable();
        mockAccessibleObject
            .Setup(a => a.Navigate(navdir))
            .CallBase();
        Assert.Null(mockAccessibleObject.Object.Navigate(navdir));
        mockAccessibleObject.Verify(a => a.Parent, Times.Once());
        mockParentAccessibleObject.Verify(a => a.GetChildCount(), Times.Once());
    }

    [WinFormsTheory]
    [InlineData(AccessibleNavigation.Previous)]
    [InlineData(AccessibleNavigation.Up)]
    [InlineData(AccessibleNavigation.Left)]
    [InlineData(AccessibleNavigation.Next)]
    [InlineData(AccessibleNavigation.Down)]
    [InlineData(AccessibleNavigation.Right)]
    public void AccessibleObject_Navigate_InvokeWithChildrenWithoutParent_ReturnsNull(AccessibleNavigation navdir)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(0);
        mockAccessibleObject
            .Setup(a => a.Parent)
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.Navigate(navdir))
            .CallBase();

        Assert.Null(mockAccessibleObject.Object.Navigate(navdir));
    }

    [WinFormsTheory]
    [EnumData<AccessibleSelection>]
    [InvalidEnumData<AccessibleSelection>]
    public void AccessibleObject_Navigate_InvokeDefault_Nop(AccessibleSelection flags)
    {
        AccessibleObject accessibleObject = new();
        accessibleObject.Select(flags);
    }

    [WinFormsFact]
    public void AccessibleObject_Navigate_FormChildren()
    {
        using Form form = new();
        using Control first = new();
        using Control second = new();
        form.Controls.Add(first);
        form.Controls.Add(second);
        form.Show();

        AccessibleObject firstChild = form.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild);
        Assert.NotNull(firstChild);
        Assert.Equal((AccessibleObject)first.TestAccessor().Dynamic.NcAccessibilityObject, firstChild);

        AccessibleObject next = firstChild.Navigate(AccessibleNavigation.Next);
        Assert.NotNull(next);

        AccessibleObject previous = next.Navigate(AccessibleNavigation.Previous);
        Assert.NotNull(previous);
        Assert.Same(firstChild, previous);
    }

    [WinFormsTheory]
    [InlineData(AccessibleNavigation.FirstChild, true, false)]
    [InlineData(AccessibleNavigation.LastChild, true, false)]
    [InlineData(AccessibleNavigation.Next, false, false)]
    [InlineData(AccessibleNavigation.Previous, true, true)]
    [InlineData(AccessibleNavigation.Right, false, false)]
    [InlineData(AccessibleNavigation.Up, true, true)]
    public void AccessibleObject_Navigate_FromForm_OneChild(AccessibleNavigation direction, bool returnsObject, bool isSystemAccessible)
    {
        using Form form = new();
        using Control control = new();
        form.Controls.Add(control);
        form.Show();

        AccessibleObject target = form.AccessibilityObject.Navigate(direction);

        Assert.Equal(returnsObject, target is not null);
        if (target is not null)
        {
            Assert.Equal(isSystemAccessible, (bool)target.TestAccessor().Dynamic._isSystemWrapper);
        }
    }

    [WinFormsTheory]
    [InlineData(AccessibleNavigation.FirstChild, false)]
    [InlineData(AccessibleNavigation.LastChild, false)]
    [InlineData(AccessibleNavigation.Next, false)]
    [InlineData(AccessibleNavigation.Previous, true)]
    [InlineData(AccessibleNavigation.Right, false)]
    [InlineData(AccessibleNavigation.Up, true)]
    public void AccessibleObject_Navigate_FromForm_NoChildren(AccessibleNavigation direction, bool returnsObject)
    {
        using Form form = new();
        form.Show();
        AccessibleObject target = form.AccessibilityObject.Navigate(direction);
        Assert.Equal(returnsObject, target is not null);
        if (target is not null)
        {
            Assert.True((bool)target.TestAccessor().Dynamic._isSystemWrapper);
        }
    }

    public static IEnumerable<object[]> UseStdAccessibleObjects_IntPtr_InvalidHandle_TestData()
    {
        yield return new object[] { IntPtr.Zero };
        yield return new object[] { (IntPtr)1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(UseStdAccessibleObjects_IntPtr_InvalidHandle_TestData))]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrInvalidHandle_Success(IntPtr handle)
    {
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(handle);
        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Null(accessibleObject.Name);
        Assert.Null(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.None, accessibleObject.Role);
        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.Empty(accessibleObject.Value);
    }

    [WinFormsFact]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrControlHandle_Success()
    {
        using Control control = new();
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(control.Handle);
        Assert.Equal(0, accessibleObject.Bounds.Width);
        Assert.Equal(0, accessibleObject.Bounds.Height);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Null(accessibleObject.Name);
        Assert.NotNull(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.Client, accessibleObject.Role);
        Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
        Assert.Null(accessibleObject.Value);
    }

    [WinFormsFact]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrLabelHandle_Success()
    {
        using Label control = new()
        {
            Text = "Text"
        };
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(control.Handle);
        Assert.Equal(accessibleObject.Bounds, accessibleObject.Bounds);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Equal("Text", accessibleObject.Name);
        Assert.NotNull(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.Graphic, accessibleObject.Role);
        Assert.Equal(AccessibleStates.ReadOnly, accessibleObject.State);
        Assert.Null(accessibleObject.Value);
    }

    public static IEnumerable<object[]> UseStdAccessibleObjects_IntPtr_Int_InvalidHandle_TestData()
    {
        yield return new object[] { IntPtr.Zero, 0 };
        yield return new object[] { (IntPtr)1, 0 };
        yield return new object[] { IntPtr.Zero, unchecked((int)0xFFFFFFFC) };
        yield return new object[] { (IntPtr)1, unchecked((int)0xFFFFFFFC) };
    }

    [WinFormsTheory]
    [MemberData(nameof(UseStdAccessibleObjects_IntPtr_Int_InvalidHandle_TestData))]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrIntInvalidHandle_Success(IntPtr handle, int objid)
    {
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(handle, objid);
        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Null(accessibleObject.Name);
        Assert.Null(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.None, accessibleObject.Role);
        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.Empty(accessibleObject.Value);
    }

    [WinFormsFact]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrIntControlHandleInvalidId_Success()
    {
        using Control control = new();
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(control.Handle, 250);
        Assert.Equal(0, accessibleObject.Bounds.Width);
        Assert.Equal(0, accessibleObject.Bounds.Height);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Null(accessibleObject.Name);
        Assert.Null(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.None, accessibleObject.Role);
        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.Empty(accessibleObject.Value);
    }

    [WinFormsFact]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrIntControlHandleWindowId_Success()
    {
        using Control control = new();
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(control.Handle, 0);
        Assert.Equal(0, accessibleObject.Bounds.Width);
        Assert.Equal(0, accessibleObject.Bounds.Height);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Null(accessibleObject.Name);
        Assert.NotNull(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.Window, accessibleObject.Role);
        Assert.Equal(AccessibleStates.Invisible | AccessibleStates.Offscreen | AccessibleStates.Focusable, accessibleObject.State);
        Assert.Null(accessibleObject.Value);
    }

    [WinFormsFact]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrIntControlHandleClientId_Success()
    {
        using Control control = new();
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(control.Handle, unchecked((int)0xFFFFFFFC));
        Assert.Equal(0, accessibleObject.Bounds.Width);
        Assert.Equal(0, accessibleObject.Bounds.Height);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Null(accessibleObject.Name);
        Assert.NotNull(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.Client, accessibleObject.Role);
        Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
        Assert.Null(accessibleObject.Value);
    }

    [WinFormsFact]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrIntLabelHandleInvalidId_Success()
    {
        using Label control = new()
        {
            Text = "Text"
        };
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(control.Handle, 250);
        Assert.Equal(accessibleObject.Bounds, accessibleObject.Bounds);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Null(accessibleObject.Name);
        Assert.Null(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.None, accessibleObject.Role);
        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.Empty(accessibleObject.Value);
    }

    [WinFormsFact]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrIntLabelHandleClientId_Success()
    {
        using Label control = new()
        {
            Text = "Text"
        };
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(control.Handle, unchecked((int)0xFFFFFFFC));
        Assert.Equal(accessibleObject.Bounds, accessibleObject.Bounds);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Equal("Text", accessibleObject.Name);
        Assert.NotNull(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.Graphic, accessibleObject.Role);
        Assert.Equal(AccessibleStates.ReadOnly, accessibleObject.State);
        Assert.Null(accessibleObject.Value);
    }

    [WinFormsFact]
    public void AccessibleObject_UseStdAccessibleObjects_InvokeIntPtrIntLabelHandleWindowId_Success()
    {
        using Label control = new()
        {
            Text = "Text"
        };
        SubAccessibleObject accessibleObject = new();
        accessibleObject.UseStdAccessibleObjects(control.Handle, 0);
        Assert.Equal(accessibleObject.Bounds, accessibleObject.Bounds);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Description);
        Assert.Null(accessibleObject.Help);
        Assert.Null(accessibleObject.KeyboardShortcut);
        Assert.Equal("Text", accessibleObject.Name);
        Assert.NotNull(accessibleObject.Parent);
        Assert.Equal(AccessibleRole.Window, accessibleObject.Role);
        Assert.Equal(AccessibleStates.Invisible | AccessibleStates.Offscreen | AccessibleStates.Focusable, accessibleObject.State);
        Assert.Null(accessibleObject.Value);
    }

    [WinFormsTheory]
    [InlineData(-2, -2)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public void AccessibleObject_IAccessibleAccChildCount_InvokeDefault_ReturnsExpected(int childCount, int expectedChildCount)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(childCount)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(expectedChildCount, iAccessible.accChildCount);
        mockAccessibleObject.Verify(a => a.GetChildCount(), Times.Once());
    }

    public static IEnumerable<object[]> SelfVarChild_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { 0 };
        yield return new object[] { unchecked((int)0x80020004) };
        yield return new object[] { "abc" };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelfVarChild_TestData))]
    public void AccessibleObject_IAccessibleAccDoDefaultAction_InvokeDefaultSelf_Success(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.DoDefaultAction())
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.accDoDefaultAction(varChild);
        mockAccessibleObject.Verify(a => a.DoDefaultAction(), Times.Once());
    }

    [WinFormsTheory]
    [InlineData(2, 1, 0)]
    [InlineData(3, 0, 1)]
    public void AccessibleObject_IAccessibleAccDoDefaultAction_InvokeDefaultChild_Success(object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.DoDefaultAction())
            .Verifiable();
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.DoDefaultAction())
            .Verifiable();

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.accDoDefaultAction(varChild);
        mockAccessibleObjectChild1.Verify(a => a.DoDefaultAction(), Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.DoDefaultAction(), Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleAccDoDefaultAction_InvokeDefaultNoSuchChild_Success(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.accDoDefaultAction(varChild);
    }

    public static IEnumerable<object[]> accFocus_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new AccessibleObject() };
    }

    [WinFormsTheory]
    [MemberData(nameof(accFocus_TestData))]
    public void AccessibleObject_IAccessibleGet_accFocus_InvokeDefault_ReturnsExpected(AccessibleObject result)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetFocused())
            .Returns(result)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Same(result, iAccessible.accFocus);
        mockAccessibleObject.Verify(a => a.GetFocused(), Times.Once());
    }

    [WinFormsFact]
    public void AccessibleObject_IAccessibleAccFocus_InvokeDefaultSelf_ReturnsExpected()
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetFocused())
            .Returns(mockAccessibleObject.Object)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(0, iAccessible.accFocus);
        mockAccessibleObject.Verify(a => a.GetFocused(), Times.Once());
    }

    [WinFormsTheory]
    [InlineData(-1, -2)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void AccessibleObject_IAccessibleAccHitTest_InvokeDefault_ReturnsNull(int x, int y)
    {
        AccessibleObject accessibleObject = new();
        IAccessible iAccessible = accessibleObject;
        Assert.Null(iAccessible.accHitTest(x, y));
    }

    [WinFormsTheory]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 5)]
    public void AccessibleObject_IAccessibleAccHitTest_InvokeDefaultWithNoChildrenBoundsValid_ReturnsExpected(int x, int y)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Bounds)
            .Returns(new Rectangle(1, 2, 3, 4));
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .CallBase();
        mockAccessibleObject
            .Setup(a => a.HitTest(x, y))
            .CallBase();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(0, iAccessible.accHitTest(x, y));
    }

    [WinFormsTheory]
    [InlineData(-1, -2)]
    [InlineData(0, 0)]
    [InlineData(4, 2)]
    [InlineData(1, 6)]
    [InlineData(4, 5)]
    public void AccessibleObject_IAccessibleAccHitTest_InvokeDefaultWithNoChildrenBoundsInvalid_ReturnsNull(int x, int y)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Bounds)
            .Returns(new Rectangle(1, 2, 3, 4));
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .CallBase();
        mockAccessibleObject
            .Setup(a => a.HitTest(x, y))
            .CallBase();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.accHitTest(x, y));
    }

    [WinFormsTheory]
    [InlineData(-1, -2)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void AccessibleObject_IAccessibleAccHitTest_InvokeDefaultWithNoBoundsChildren_ReturnsExpected(int x, int y)
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(new AccessibleObject());
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(2);
            mockAccessibleObject
                .Setup(a => a.HitTest(x, y))
                .CallBase();
            IAccessible iAccessible = mockAccessibleObject.Object;
            Assert.Equal(0, iAccessible.accHitTest(x, y));
        }
    }

    [WinFormsTheory]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 5)]
    public void AccessibleObject_IAccessibleAccHitTest_InvokeDefaultWithBoundsChildren_ReturnsExpected(int x, int y)
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Bounds)
                .Returns(new Rectangle(1, 2, 3, 4));
            Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.Bounds)
                .Returns(new Rectangle(1, 2, 3, 4));

            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(mockAccessibleObjectChild1.Object);
            mockAccessibleObject
                .Setup(a => a.GetChild(2))
                .Returns(mockAccessibleObjectChild2.Object);
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(3);
            mockAccessibleObject
                .Setup(a => a.HitTest(x, y))
                .CallBase();
            IAccessible iAccessible = mockAccessibleObject.Object;
            Assert.Same(mockAccessibleObjectChild1.Object, iAccessible.accHitTest(x, y));
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SelfVarChild_TestData))]
    public void AccessibleObject_IAccessibleAccLocation_InvokeDefaultSelf_Success(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Bounds)
            .Returns(new Rectangle(1, 2, 3, 4))
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, varChild);
        Assert.Equal(1, pxLeft);
        Assert.Equal(2, pyTop);
        Assert.Equal(3, pcxWidth);
        Assert.Equal(4, pcyHeight);
        mockAccessibleObject.Verify(a => a.Bounds, Times.Once());
    }

    [WinFormsTheory]
    [InlineData(2, 1, 0)]
    [InlineData(3, 0, 1)]
    public void AccessibleObject_IAccessibleAccLocation_InvokeDefaultChild_Success(object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.Bounds)
            .Returns(new Rectangle(1, 2, 3, 4))
            .Verifiable();
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.Bounds)
            .Returns(new Rectangle(1, 2, 3, 4))
            .Verifiable();

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, varChild);
        Assert.Equal(1, pxLeft);
        Assert.Equal(2, pyTop);
        Assert.Equal(3, pcxWidth);
        Assert.Equal(4, pcyHeight);
        mockAccessibleObjectChild1.Verify(a => a.Bounds, Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.Bounds, Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleAccLocation_InvokeDefaultNoSuchChild_Success(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, varChild);
        Assert.Equal(0, pxLeft);
        Assert.Equal(0, pyTop);
        Assert.Equal(0, pcxWidth);
        Assert.Equal(0, pcyHeight);
    }

    [WinFormsTheory]
    [InlineData((int)AccessibleNavigation.Right, 0)]
    [InlineData((int)AccessibleNavigation.Right, unchecked((int)0x80020004))]
    [InlineData((int)AccessibleNavigation.Right, "abc")]
    [InlineData((int)AccessibleNavigation.Right, null)]
    public void AccessibleObject_IAccessibleAccNavigate_InvokeDefaultSelf_Success(int navDir, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Navigate((AccessibleNavigation)navDir))
            .Returns(mockAccessibleObject.Object)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(0, iAccessible.accNavigate(navDir, varChild));
        mockAccessibleObject.Verify(a => a.Navigate((AccessibleNavigation)navDir), Times.Once());
    }

    [WinFormsTheory]
    [InlineData((int)AccessibleNavigation.Right, 2, 1, 0)]
    [InlineData((int)AccessibleNavigation.Right, 3, 0, 1)]
    public void AccessibleObject_IAccessibleAccNavigate_InvokeDefaultChild_Success(int navDir, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.Navigate((AccessibleNavigation)navDir))
            .Returns(mockAccessibleObjectChild1.Object)
            .Verifiable();
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.Navigate((AccessibleNavigation)navDir))
            .Returns(mockAccessibleObjectChild2.Object)
            .Verifiable();

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.NotEqual(0, iAccessible.accNavigate(navDir, varChild));
        mockAccessibleObjectChild1.Verify(a => a.Navigate((AccessibleNavigation)navDir), Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.Navigate((AccessibleNavigation)navDir), Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData((int)AccessibleNavigation.Right, 2, 1, 0)]
    [InlineData((int)AccessibleNavigation.Right, 3, 0, 1)]
    public void AccessibleObject_IAccessibleAccNavigate_InvokeDefaultChildSelf_Success(int navDir, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.Navigate((AccessibleNavigation)navDir))
            .Returns(mockAccessibleObject.Object)
            .Verifiable();
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.Navigate((AccessibleNavigation)navDir))
            .Returns(mockAccessibleObject.Object)
            .Verifiable();

        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(0, iAccessible.accNavigate(navDir, varChild));
        mockAccessibleObjectChild1.Verify(a => a.Navigate((AccessibleNavigation)navDir), Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.Navigate((AccessibleNavigation)navDir), Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData((int)AccessibleNavigation.Right, -1)]
    [InlineData((int)AccessibleNavigation.Right, 4)]
    public void AccessibleObject_IAccessibleAccNavigate_InvokeDefaultNoSuchChild_Success(int navDir, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.accNavigate(navDir, varChild));
    }

    public static IEnumerable<object[]> accParent_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new AccessibleObject() };
    }

    [WinFormsTheory]
    [MemberData(nameof(accParent_TestData))]
    public void AccessibleObject_IAccessibleAccParent_InvokeDefault_ReturnsExpected(AccessibleObject result)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Parent)
            .Returns(result)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Same(result, iAccessible.accParent);
        mockAccessibleObject.Verify(a => a.Parent, Times.Once());
    }

    [WinFormsFact]
    public void AccessibleObject_IAccessibleAccParent_InvokeDefaultSelf_ReturnsExpected()
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.Parent)
                .Returns(mockAccessibleObject.Object)
                .Verifiable();
            IAccessible iAccessible = mockAccessibleObject.Object;
            Assert.Null(iAccessible.accParent);
            mockAccessibleObject.Verify(a => a.Parent, Times.Once());
        }
    }

    [WinFormsTheory]
    [InlineData((int)AccessibleSelection.AddSelection, 0)]
    [InlineData((int)AccessibleSelection.AddSelection, unchecked((int)0x80020004))]
    [InlineData((int)AccessibleSelection.AddSelection, "abc")]
    [InlineData((int)AccessibleSelection.AddSelection, null)]
    public void AccessibleObject_IAccessibleAccSelect_InvokeDefaultSelf_Success(int flagsSelect, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Select((AccessibleSelection)flagsSelect))
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.accSelect(flagsSelect, varChild);
        mockAccessibleObject.Verify(a => a.Select((AccessibleSelection)flagsSelect), Times.Once());
    }

    [WinFormsTheory]
    [InlineData((int)AccessibleSelection.AddSelection, 2, 1, 0)]
    [InlineData((int)AccessibleSelection.AddSelection, 3, 0, 1)]
    public void AccessibleObject_IAccessibleAccSelect_InvokeDefaultChild_Success(int flagsSelect, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.Select((AccessibleSelection)flagsSelect))
            .Verifiable();
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.Select((AccessibleSelection)flagsSelect))
            .Verifiable();

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.accSelect(flagsSelect, varChild);
        mockAccessibleObjectChild1.Verify(a => a.Select((AccessibleSelection)flagsSelect), Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.Select((AccessibleSelection)flagsSelect), Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData((int)AccessibleSelection.AddSelection, -1)]
    [InlineData((int)AccessibleSelection.AddSelection, 4)]
    public void AccessibleObject_IAccessibleAccSelect_InvokeDefaultNoSuchChild_Success(int flagsSelect, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.accSelect(flagsSelect, varChild);
    }

    public static IEnumerable<object[]> accSelection_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new AccessibleObject() };
    }

    [WinFormsTheory]
    [MemberData(nameof(accSelection_TestData))]
    public void AccessibleObject_IAccessibleGet_accSelection_InvokeDefault_ReturnsExpected(AccessibleObject result)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetSelected())
            .Returns(result)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Same(result, iAccessible.accSelection);
        mockAccessibleObject.Verify(a => a.GetSelected(), Times.Once());
    }

    [WinFormsFact]
    public void AccessibleObject_IAccessibleAccSelection_InvokeDefaultSelf_ReturnsExpected()
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetSelected())
            .Returns(mockAccessibleObject.Object)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(0, iAccessible.accSelection);
        mockAccessibleObject.Verify(a => a.GetSelected(), Times.Once());
    }

    [WinFormsTheory]
    [MemberData(nameof(SelfVarChild_TestData))]
    public void AccessibleObject_IAccessibleGet_AccChild_InvokeDefaultSelf_ReturnsExpected(object varChild)
    {
        AccessibleObject accessibleObject = new();
        IAccessible iAccessible = accessibleObject;
        Assert.Same(iAccessible, iAccessible.get_accChild(varChild));
    }

    [WinFormsFact]
    public void AccessibleObject_IAccessibleGet_accChild_InvokeDefaultChild_Success()
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accChild(1));
        Assert.Same(mockAccessibleObjectChild1.Object, iAccessible.get_accChild(2));
        Assert.Same(mockAccessibleObjectChild2.Object, iAccessible.get_accChild(3));
    }

    [WinFormsTheory]
    [InlineData(2)]
    [InlineData(3)]
    public void AccessibleObject_IAccessibleGet_accChild_InvokeDefaultChildSelf_ReturnsExpected(object varChild)
    {
        using (new NoAssertContext())
        {
            Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
            Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

            Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(mockAccessibleObject.Object);
            mockAccessibleObject
                .Setup(a => a.GetChild(2))
                .Returns(mockAccessibleObject.Object);
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(3);

            IAccessible iAccessible = mockAccessibleObject.Object;
            Assert.Null(iAccessible.get_accChild(varChild));
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accChild_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accChild(varChild));
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData(null, unchecked((int)0x80020004))]
    [InlineData(null, "abc")]
    [InlineData(null, null)]
    [InlineData("", 0)]
    [InlineData("", unchecked((int)0x80020004))]
    [InlineData("", "abc")]
    [InlineData("", null)]
    [InlineData("value", 0)]
    [InlineData("value", unchecked((int)0x80020004))]
    [InlineData("value", "abc")]
    [InlineData("value", null)]
    public void AccessibleObject_IAccessibleGet_accDefaultAction_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.DefaultAction)
            .Returns(result)
            .Verifiable();
        mockAccessibleObject
            .Setup(a => a.CanGetDefaultActionInternal)
            .Returns(false);
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accDefaultAction(varChild));
        mockAccessibleObject.Verify(a => a.DefaultAction, Times.Once());
    }

    [WinFormsTheory]
    [InlineData(null, 2, 1, 0)]
    [InlineData("", 2, 1, 0)]
    [InlineData("value", 2, 1, 0)]
    [InlineData(null, 3, 0, 1)]
    [InlineData("", 3, 0, 1)]
    [InlineData("value", 3, 0, 1)]
    public void AccessibleObject_IAccessibleGet_accDefaultAction_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.DefaultAction)
            .Returns(result)
            .Verifiable();
        mockAccessibleObjectChild1
            .Setup(a => a.CanGetDefaultActionInternal)
            .Returns(false);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.DefaultAction)
            .Returns(result)
            .Verifiable();
        mockAccessibleObjectChild2
            .Setup(a => a.CanGetDefaultActionInternal)
            .Returns(false);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accDefaultAction(varChild));
        mockAccessibleObjectChild1.Verify(a => a.DefaultAction, Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.DefaultAction, Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accDefaultAction_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accDefaultAction(varChild));
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData(null, unchecked((int)0x80020004))]
    [InlineData(null, "abc")]
    [InlineData(null, null)]
    [InlineData("", 0)]
    [InlineData("", unchecked((int)0x80020004))]
    [InlineData("", "abc")]
    [InlineData("", null)]
    [InlineData("value", 0)]
    [InlineData("value", unchecked((int)0x80020004))]
    [InlineData("value", "abc")]
    [InlineData("value", null)]
    public void AccessibleObject_IAccessibleGet_accDescription_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Description)
            .Returns(result)
            .Verifiable();
        mockAccessibleObject
            .Setup(a => a.CanGetDescriptionInternal)
            .Returns(false);
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accDescription(varChild));
        mockAccessibleObject.Verify(a => a.Description, Times.Once());
    }

    [WinFormsTheory]
    [InlineData(null, 2, 1, 0)]
    [InlineData("", 2, 1, 0)]
    [InlineData("value", 2, 1, 0)]
    [InlineData(null, 3, 0, 1)]
    [InlineData("", 3, 0, 1)]
    [InlineData("value", 3, 0, 1)]
    public void AccessibleObject_IAccessibleGet_accDescription_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.Description)
            .Returns(result)
            .Verifiable();
        mockAccessibleObjectChild1
            .Setup(a => a.CanGetDescriptionInternal)
            .Returns(false);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.Description)
            .Returns(result)
            .Verifiable();
        mockAccessibleObjectChild2
            .Setup(a => a.CanGetDescriptionInternal)
            .Returns(false);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);
        mockAccessibleObject
            .Setup(a => a.CanGetDescriptionInternal)
            .Returns(false);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accDescription(varChild));
        mockAccessibleObjectChild1.Verify(a => a.Description, Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.Description, Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accDescription_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accDescription(varChild));
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData(null, unchecked((int)0x80020004))]
    [InlineData(null, "abc")]
    [InlineData(null, null)]
    [InlineData("", 0)]
    [InlineData("", unchecked((int)0x80020004))]
    [InlineData("", "abc")]
    [InlineData("", null)]
    [InlineData("value", 0)]
    [InlineData("value", unchecked((int)0x80020004))]
    [InlineData("value", "abc")]
    [InlineData("value", null)]
    public void AccessibleObject_IAccessibleGet_accHelp_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Help)
            .Returns(result)
            .Verifiable();
        mockAccessibleObject
            .Setup(a => a.CanGetHelpInternal)
            .Returns(false);
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accHelp(varChild));
        mockAccessibleObject.Verify(a => a.Help, Times.Once());
    }

    [WinFormsTheory]
    [InlineData(null, 2, 1, 0)]
    [InlineData("", 2, 1, 0)]
    [InlineData("value", 2, 1, 0)]
    [InlineData(null, 3, 0, 1)]
    [InlineData("", 3, 0, 1)]
    [InlineData("value", 3, 0, 1)]
    public void AccessibleObject_IAccessibleGet_accHelp_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.Help)
            .Returns(result)
            .Verifiable();
        mockAccessibleObjectChild1
            .Setup(a => a.CanGetHelpInternal)
            .Returns(false);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.Help)
            .Returns(result)
            .Verifiable();
        mockAccessibleObjectChild2
            .Setup(a => a.CanGetHelpInternal)
            .Returns(false);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);
        mockAccessibleObject
            .Setup(a => a.CanGetHelpInternal)
            .Returns(false);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accHelp(varChild));
        mockAccessibleObjectChild1.Verify(a => a.Help, Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.Help, Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accHelp_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accHelp(varChild));
    }

    private delegate void HelpTopicDelegate(out string pszHelpFile);

    [WinFormsTheory]
    [InlineData(-1, null, 0)]
    [InlineData(0, null, unchecked((int)0x80020004))]
    [InlineData(1, null, "abc")]
    [InlineData(2, null, null)]
    [InlineData(-1, "", 0)]
    [InlineData(0, "", unchecked((int)0x80020004))]
    [InlineData(1, "", "abc")]
    [InlineData(2, "", null)]
    [InlineData(-1, "value", 0)]
    [InlineData(0, "value", unchecked((int)0x80020004))]
    [InlineData(1, "value", "abc")]
    [InlineData(2, "value", null)]
    public void AccessibleObject_IAccessibleGet_accHelpTopic_InvokeDefaultSelf_ReturnsExpected(int result, string stringResult, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        string dummy;
        HelpTopicDelegate handler = (out string pszHelpFile) =>
        {
            pszHelpFile = stringResult;
        };
        mockAccessibleObject
            .Setup(a => a.GetHelpTopic(out dummy))
            .Callback(handler)
            .Returns(result);
        mockAccessibleObject
            .Setup(a => a.CanGetHelpTopicInternal)
            .Returns(false);
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accHelpTopic(out string pszHelpFile, varChild));
        Assert.Equal(stringResult, pszHelpFile);
    }

    [WinFormsTheory]
    [InlineData(-1, null, 2)]
    [InlineData(0, "", 2)]
    [InlineData(1, "value", 2)]
    [InlineData(-1, null, 3)]
    [InlineData(0, "", 3)]
    [InlineData(1, "value", 3)]
    public void AccessibleObject_IAccessibleGet_accHelpTopic_InvokeDefaultChild_ReturnsExpected(int result, string stringResult, object varChild)
    {
        string dummy;
        HelpTopicDelegate handler = (out string pszHelpFile) =>
        {
            pszHelpFile = stringResult;
        };
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.GetHelpTopic(out dummy))
            .Callback(handler)
            .Returns(result);
        mockAccessibleObjectChild1
            .Setup(a => a.CanGetHelpTopicInternal)
            .Returns(false);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.GetHelpTopic(out dummy))
            .Callback(handler)
            .Returns(result);
        mockAccessibleObjectChild2
            .Setup(a => a.CanGetHelpTopicInternal)
            .Returns(false);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);
        mockAccessibleObject
            .Setup(a => a.CanGetHelpTopicInternal)
            .Returns(false);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accHelpTopic(out string pszHelpFile, varChild));
        Assert.Equal(stringResult, pszHelpFile);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accHelpTopic_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(-1, iAccessible.get_accHelpTopic(out string pszHelpFile, varChild));
        Assert.Null(pszHelpFile);
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData(null, unchecked((int)0x80020004))]
    [InlineData(null, "abc")]
    [InlineData(null, null)]
    [InlineData("", 0)]
    [InlineData("", unchecked((int)0x80020004))]
    [InlineData("", "abc")]
    [InlineData("", null)]
    [InlineData("value", 0)]
    [InlineData("value", unchecked((int)0x80020004))]
    [InlineData("value", "abc")]
    [InlineData("value", null)]
    public void AccessibleObject_IAccessibleGet_accKeyboardShortcut_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new()
        {
            CallBase = true
        };
        mockAccessibleObject
            .Setup(a => a.KeyboardShortcut)
            .Returns(result)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accKeyboardShortcut(varChild));
        mockAccessibleObject.Verify(a => a.KeyboardShortcut, Times.Once());
    }

    [WinFormsTheory]
    [InlineData(null, 2, 1, 0)]
    [InlineData("", 2, 1, 0)]
    [InlineData("value", 2, 1, 0)]
    [InlineData(null, 3, 0, 1)]
    [InlineData("", 3, 0, 1)]
    [InlineData("value", 3, 0, 1)]
    public void AccessibleObject_IAccessibleGet_accKeyboardShortcut_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new()
        {
            CallBase = true
        };
        mockAccessibleObjectChild1
            .Setup(a => a.KeyboardShortcut)
            .Returns(result)
            .Verifiable();
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new()
        {
            CallBase = true
        };
        mockAccessibleObjectChild2
            .Setup(a => a.KeyboardShortcut)
            .Returns(result)
            .Verifiable();

        Mock<AccessibleObject> mockAccessibleObject = new()
        {
            CallBase = true
        };
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accKeyboardShortcut(varChild));
        mockAccessibleObjectChild1.Verify(a => a.KeyboardShortcut, Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.KeyboardShortcut, Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accKeyboardShortcut_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new()
        {
            CallBase = true
        };
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accKeyboardShortcut(varChild));
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData(null, unchecked((int)0x80020004))]
    [InlineData(null, "abc")]
    [InlineData(null, null)]
    [InlineData("", 0)]
    [InlineData("", unchecked((int)0x80020004))]
    [InlineData("", "abc")]
    [InlineData("", null)]
    [InlineData("value", 0)]
    [InlineData("value", unchecked((int)0x80020004))]
    [InlineData("value", "abc")]
    [InlineData("value", null)]
    public void AccessibleObject_IAccessibleGet_accName_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new()
        {
            CallBase = true
        };
        mockAccessibleObject
            .Setup(a => a.Name)
            .Returns(result)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accName(varChild));
        mockAccessibleObject.Verify(a => a.Name, Times.Once());
    }

    [WinFormsTheory]
    [InlineData(null, 2, 1, 0)]
    [InlineData("", 2, 1, 0)]
    [InlineData("value", 2, 1, 0)]
    [InlineData(null, 3, 0, 1)]
    [InlineData("", 3, 0, 1)]
    [InlineData("value", 3, 0, 1)]
    public void AccessibleObject_IAccessibleGet_accName_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new()
        {
            CallBase = true
        };
        mockAccessibleObjectChild1
            .Setup(a => a.Name)
            .Returns(result)
            .Verifiable();
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new()
        {
            CallBase = true
        };
        mockAccessibleObjectChild2
            .Setup(a => a.Name)
            .Returns(result)
            .Verifiable();

        Mock<AccessibleObject> mockAccessibleObject = new()
        {
            CallBase = true
        };
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accName(varChild));
        mockAccessibleObjectChild1.Verify(a => a.Name, Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.Name, Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accName_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new()
        {
            CallBase = true
        };
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accName(varChild));
    }

    [WinFormsTheory]
    [InlineData(AccessibleRole.None, 0)]
    [InlineData(AccessibleRole.None, unchecked((int)0x80020004))]
    [InlineData(AccessibleRole.None, "abc")]
    [InlineData(AccessibleRole.None, null)]
    [InlineData(AccessibleRole.Default, 0)]
    [InlineData(AccessibleRole.Default, unchecked((int)0x80020004))]
    [InlineData(AccessibleRole.Default, "abc")]
    [InlineData(AccessibleRole.Default, null)]
    [InlineData(AccessibleRole.Sound, 0)]
    [InlineData(AccessibleRole.Sound, unchecked((int)0x80020004))]
    [InlineData(AccessibleRole.Sound, "abc")]
    [InlineData(AccessibleRole.Sound, null)]
    public void AccessibleObject_IAccessibleGet_accRole_InvokeDefaultSelf_ReturnsExpected(AccessibleRole result, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Role)
            .Returns(result)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal((int)result, iAccessible.get_accRole(varChild));
        mockAccessibleObject.Verify(a => a.Role, Times.Once());
    }

    [WinFormsFact]
    public void AccessibleObject_IAccessibleGet_accRole_InvokeDefaultSelfNotAClientObject_ReturnsExpected()
    {
        using Control control = new();
        control.CreateControl();

        AccessibleObject accessibleObject = control.TestAccessor().Dynamic.NcAccessibilityObject;

        IAccessible iAccessible = accessibleObject;
        Assert.Equal((int)AccessibleRole.Window, iAccessible.get_accRole((int)PInvoke.CHILDID_SELF));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(AccessibleRole.None, 2, 1, 0)]
    [InlineData(AccessibleRole.Default, 2, 1, 0)]
    [InlineData(AccessibleRole.Sound, 2, 1, 0)]
    [InlineData(AccessibleRole.None, 3, 0, 1)]
    [InlineData(AccessibleRole.Default, 3, 0, 1)]
    [InlineData(AccessibleRole.Sound, 3, 0, 1)]
    public void AccessibleObject_IAccessibleGet_accRole_InvokeDefaultChild_ReturnsExpected(AccessibleRole result, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.Role)
            .Returns(result)
            .Verifiable();
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.Role)
            .Returns(result)
            .Verifiable();

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal((int)result, iAccessible.get_accRole(varChild));
        mockAccessibleObjectChild1.Verify(a => a.Role, Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.Role, Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accRole_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accRole(varChild));
    }

    [WinFormsTheory]
    [InlineData(AccessibleStates.None, 0)]
    [InlineData(AccessibleStates.None, unchecked((int)0x80020004))]
    [InlineData(AccessibleStates.None, "abc")]
    [InlineData(AccessibleStates.None, null)]
    [InlineData(AccessibleStates.Mixed, 0)]
    [InlineData(AccessibleStates.Mixed, unchecked((int)0x80020004))]
    [InlineData(AccessibleStates.Mixed, "abc")]
    [InlineData(AccessibleStates.Mixed, null)]
    public void AccessibleObject_IAccessibleGet_accState_InvokeDefaultSelf_ReturnsExpected(AccessibleStates result, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.State)
            .Returns(result)
            .Verifiable();
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal((int)result, iAccessible.get_accState(varChild));
        mockAccessibleObject.Verify(a => a.State, Times.Once());
    }

    [WinFormsTheory]
    [InlineData(AccessibleStates.None, 2, 1, 0)]
    [InlineData(AccessibleStates.Mixed, 2, 1, 0)]
    [InlineData(AccessibleStates.None, 3, 0, 1)]
    [InlineData(AccessibleStates.Mixed, 3, 0, 1)]
    public void AccessibleObject_IAccessibleGet_accState_InvokeDefaultChild_ReturnsExpected(AccessibleStates result, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.State)
            .Returns(result)
            .Verifiable();
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.State)
            .Returns(result)
            .Verifiable();

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal((int)result, iAccessible.get_accState(varChild));
        mockAccessibleObjectChild1.Verify(a => a.State, Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.State, Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accState_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accState(varChild));
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData(null, unchecked((int)0x80020004))]
    [InlineData(null, "abc")]
    [InlineData(null, null)]
    [InlineData("", 0)]
    [InlineData("", unchecked((int)0x80020004))]
    [InlineData("", "abc")]
    [InlineData("", null)]
    [InlineData("value", 0)]
    [InlineData("value", unchecked((int)0x80020004))]
    [InlineData("value", "abc")]
    [InlineData("value", null)]
    public void AccessibleObject_IAccessibleGet_accValue_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.Value)
            .Returns(result)
            .Verifiable();
        mockAccessibleObject
            .Setup(a => a.CanGetValueInternal)
            .Returns(false);
        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accValue(varChild));
        mockAccessibleObject.Verify(a => a.Value, Times.Once());
    }

    [WinFormsTheory]
    [InlineData(null, 2, 1, 0)]
    [InlineData("", 2, 1, 0)]
    [InlineData("value", 2, 1, 0)]
    [InlineData(null, 3, 0, 1)]
    [InlineData("", 3, 0, 1)]
    [InlineData("value", 3, 0, 1)]
    public void AccessibleObject_IAccessibleGet_accValue_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        mockAccessibleObjectChild1
            .Setup(a => a.Value)
            .Returns(result)
            .Verifiable();
        mockAccessibleObjectChild1
            .Setup(a => a.CanGetValueInternal)
            .Returns(false);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);
        mockAccessibleObjectChild2
            .Setup(a => a.Value)
            .Returns(result)
            .Verifiable();
        mockAccessibleObjectChild2
            .Setup(a => a.CanGetValueInternal)
            .Returns(false);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);
        mockAccessibleObject
            .Setup(a => a.CanGetValueInternal)
            .Returns(false);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Equal(result, iAccessible.get_accValue(varChild));
        mockAccessibleObjectChild1.Verify(a => a.Value, Times.Exactly(child1CallCount));
        mockAccessibleObjectChild2.Verify(a => a.Value, Times.Exactly(child2CallCount));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    public void AccessibleObject_IAccessibleGet_accValue_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
    {
        Mock<AccessibleObject> mockAccessibleObjectChild1 = new(MockBehavior.Strict);
        Mock<AccessibleObject> mockAccessibleObjectChild2 = new(MockBehavior.Strict);

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(mockAccessibleObjectChild1.Object);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(mockAccessibleObjectChild2.Object);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        Assert.Null(iAccessible.get_accValue(varChild));
    }

    [WinFormsTheory]
    [InlineData(0, null)]
    [InlineData(unchecked((int)0x80020004), null)]
    [InlineData("abc", null)]
    [InlineData(null, null)]
    [InlineData(0, "")]
    [InlineData(unchecked((int)0x80020004), "")]
    [InlineData("abc", "")]
    [InlineData(null, "")]
    [InlineData(0, "value")]
    [InlineData(unchecked((int)0x80020004), "value")]
    [InlineData("abc", "value")]
    [InlineData(null, "value")]
    public void AccessibleObject_IAccessibleSet_accName_InvokeDefaultSelf_ReturnsExpected(object varChild, string value)
    {
        AccessibleObject accessibleObject = new();
        IAccessible iAccessible = accessibleObject;
        iAccessible.set_accName(varChild, value);
        Assert.Null(iAccessible.get_accName(varChild));
        Assert.Null(accessibleObject.Name);
    }

    [WinFormsTheory]
    [InlineData(2, null)]
    [InlineData(2, "")]
    [InlineData(2, "value")]
    [InlineData(3, null)]
    [InlineData(3, "")]
    [InlineData(3, "value")]
    public void AccessibleObject_IAccessibleSet_accName_InvokeDefaultChild_ReturnsExpected(object varChild, string value)
    {
        AccessibleObject childAccessibleObject1 = new();
        AccessibleObject childAccessibleObject2 = new();

        Mock<AccessibleObject> mockAccessibleObject = new()
        {
            CallBase = true
        };
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(childAccessibleObject1);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(childAccessibleObject2);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.set_accName(varChild, value);
        Assert.Null(iAccessible.get_accName(varChild));
        Assert.Null(childAccessibleObject1.Name);
        Assert.Null(childAccessibleObject2.Name);
    }

    [WinFormsTheory]
    [InlineData(-1, null)]
    [InlineData(-1, "")]
    [InlineData(-1, "value")]
    [InlineData(4, null)]
    [InlineData(4, "")]
    [InlineData(4, "value")]
    public void AccessibleObject_IAccessibleSet_accName_InvokeDefaultNoSuchChild_ReturnsNull(object varChild, string value)
    {
        AccessibleObject childAccessibleObject1 = new();
        AccessibleObject childAccessibleObject2 = new();

        Mock<AccessibleObject> mockAccessibleObject = new()
        {
            CallBase = true
        };
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(childAccessibleObject1);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(childAccessibleObject2);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.set_accName(varChild, value);
        Assert.Null(iAccessible.get_accName(varChild));
        Assert.Null(childAccessibleObject1.Name);
        Assert.Null(childAccessibleObject2.Name);
    }

    [WinFormsTheory]
    [InlineData(0, null)]
    [InlineData(unchecked((int)0x80020004), null)]
    [InlineData("abc", null)]
    [InlineData(null, null)]
    [InlineData(0, "")]
    [InlineData(unchecked((int)0x80020004), "")]
    [InlineData("abc", "")]
    [InlineData(null, "")]
    [InlineData(0, "value")]
    [InlineData(unchecked((int)0x80020004), "value")]
    [InlineData("abc", "value")]
    [InlineData(null, "value")]
    public void AccessibleObject_IAccessibleSet_accValue_InvokeDefaultSelf_ReturnsExpected(object varChild, string value)
    {
        AccessibleObject accessibleObject = new();
        IAccessible iAccessible = accessibleObject;
        iAccessible.set_accValue(varChild, value);
        Assert.Empty(iAccessible.get_accValue(varChild));
        Assert.Empty(accessibleObject.Value);
    }

    [WinFormsTheory]
    [InlineData(2, null)]
    [InlineData(2, "")]
    [InlineData(2, "value")]
    [InlineData(3, null)]
    [InlineData(3, "")]
    [InlineData(3, "value")]
    public void AccessibleObject_IAccessibleSet_accValue_InvokeDefaultChild_ReturnsExpected(object varChild, string value)
    {
        AccessibleObject childAccessibleObject1 = new();
        AccessibleObject childAccessibleObject2 = new();

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(childAccessibleObject1);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(childAccessibleObject2);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.set_accValue(varChild, value);
        Assert.Empty(iAccessible.get_accValue(varChild));
        Assert.Empty(childAccessibleObject1.Value);
        Assert.Empty(childAccessibleObject2.Value);
    }

    [WinFormsTheory]
    [InlineData(-1, null)]
    [InlineData(-1, "")]
    [InlineData(-1, "value")]
    [InlineData(4, null)]
    [InlineData(4, "")]
    [InlineData(4, "value")]
    public void AccessibleObject_IAccessibleSet_accValue_InvokeDefaultNoSuchChild_ReturnsNull(object varChild, string value)
    {
        AccessibleObject childAccessibleObject1 = new();
        AccessibleObject childAccessibleObject2 = new();

        Mock<AccessibleObject> mockAccessibleObject = new(MockBehavior.Strict);
        mockAccessibleObject
            .Setup(a => a.GetChild(0))
            .Returns((AccessibleObject)null);
        mockAccessibleObject
            .Setup(a => a.GetChild(1))
            .Returns(childAccessibleObject1);
        mockAccessibleObject
            .Setup(a => a.GetChild(2))
            .Returns(childAccessibleObject2);
        mockAccessibleObject
            .Setup(a => a.GetChildCount())
            .Returns(3);

        IAccessible iAccessible = mockAccessibleObject.Object;
        iAccessible.set_accValue(varChild, value);
        Assert.Null(iAccessible.get_accValue(varChild));
        Assert.Empty(childAccessibleObject1.Value);
        Assert.Empty(childAccessibleObject2.Value);
    }

    [DllImport("Oleacc.dll")]
    internal static extern unsafe HRESULT AccessibleObjectFromPoint(
        Point ptScreen,
        [MarshalAs(UnmanagedType.Interface)]
        out object ppacc,
        out object pvarChild);

    [WinFormsFact(Skip = "This test needs to be run manually as it depends on the form being unobstructed.")]
    public unsafe void TestAccessibleObjectFromPoint_Button()
    {
        using Form form = new();
        using Button button = new Button
        {
            Text = "MSAA Button"
        };

        form.Controls.Add(button);
        form.Show();
        var bounds = button.Bounds;
        Point point = button.Location;
        point.Offset(bounds.Width / 2, bounds.Height / 2);
        point = button.PointToScreen(point);
        var result = AccessibleObjectFromPoint(
            point,
            out object ppacc,
            out object varItem);

        Assert.Equal(HRESULT.S_OK, result);
        Assert.NotNull(ppacc);
        Assert.True(varItem is int);

        IAccessible accessible = ppacc as IAccessible;
        Assert.NotNull(accessible);
        Assert.Equal("MSAA Button", accessible.accName);
        Assert.NotEqual(0, ((int)accessible.accState & 0x100000));    // STATE_SYSTEM_FOCUSABLE
        Assert.Equal(0x2b, accessible.accRole);                     // ROLE_SYSTEM_PUSHBUTTON
        Assert.Equal("Press", accessible.accDefaultAction);
    }

    [WinFormsFact(Skip = "This test needs to be run manually as it depends on the form being unobstructed.")]
    public unsafe void TestAccessibleObjectFromPoint_ComboBox()
    {
        using Form form = new();
        using ComboBox comboBox = new();
        comboBox.Items.Add("Item One");
        comboBox.Items.Add("Item Two");

        form.Controls.Add(comboBox);
        form.Show();
        var bounds = comboBox.Bounds;
        Point point = comboBox.Location;
        point.Offset(bounds.Width / 2, bounds.Height / 2);
        point = comboBox.PointToScreen(point);
        var result = AccessibleObjectFromPoint(
            point,
            out object ppacc,
            out object varItem);

        Assert.Equal(HRESULT.S_OK, result);
        Assert.NotNull(ppacc);
        Assert.True(varItem is int);

        IAccessible accessible = ppacc as IAccessible;
        Assert.NotNull(accessible);
        Assert.Null(accessible.accName);
        Assert.NotEqual(0, ((int)accessible.accState & 0x100000));    // STATE_SYSTEM_FOCUSABLE
        Assert.Equal(0x2a, accessible.accRole);                     // ROLE_SYSTEM_TEXT
        Assert.Null(accessible.accDefaultAction);

        var parent = accessible.accParent as IAccessible;
        Assert.Equal(0x09, parent.accRole);                         // ROLE_SYSTEM_WINDOW
        Assert.Equal(7, parent.accChildCount);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ControlTypePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)]
    public void AccessibleObject_GetPropertyValue_ReturnsNull_IfExpected(int propertyId)
    {
        AccessibleObject accessibleObject = new();

        Assert.Equal(VARIANT.Empty, accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId));
    }

    public static IEnumerable<object[]> AccessibleObject_RuntimeId_IsOverridden_TestData()
    {
        Assembly assembly = typeof(AccessibleObject).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            // ComboBox.ChildAccessibleObject is more like an abstract class, so we should check its direct inheritors instead of it
            if (type.BaseType != typeof(AccessibleObject) && type.BaseType != typeof(ComboBox.ChildAccessibleObject))
            {
                continue;
            }

            if (type == typeof(ComboBox.ChildAccessibleObject))
            {
                continue;
            }

            yield return new object[] { type };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AccessibleObject_RuntimeId_IsOverridden_TestData))]
    public void AccessibleObject_RuntimeId_IsOverridden(Type type)
    {
        PropertyInfo runtimeIdProperty = type.GetProperty(nameof(AccessibleObject.RuntimeId), BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.Equal(type, runtimeIdProperty.DeclaringType);
    }

    [WinFormsFact]
    public unsafe void AccessibleObject_GetIAccessiblePair_Invoke_ReturnsExpected()
    {
        const int expectedIdChild = (int)PInvoke.CHILDID_SELF;

        AccessibleObject accessibleObject = new();

        // Use some number different from the expected to ensure that the value is changed
        int idChild = unchecked((int)0xdeadbeef);
        Assert.NotEqual(expectedIdChild, idChild);

        using ComScope<UIA.IAccessible> accessible = new(null);
        HRESULT result = ((UIA.IAccessibleEx.Interface)accessibleObject).GetIAccessiblePair(accessible, &idChild);
        using ComScope<UIA.IAccessible> expected = new(ComHelpers.TryGetComPointer<UIA.IAccessible>(accessibleObject));

        Assert.Equal(HRESULT.S_OK, result);
        Assert.Equal((nint)expected.Value, (nint)accessible.Value);
        Assert.Equal(expectedIdChild, idChild);
    }

    [WinFormsFact]
    public unsafe void AccessibleObject_GetIAccessiblePair_InvokeWithInvalidArgument_ReturnsError()
    {
        AccessibleObject accessibleObject = new();

        using ComScope<UIA.IAccessible> accessible = new(null);
        HRESULT result = ((UIA.IAccessibleEx.Interface)accessibleObject).GetIAccessiblePair(accessible, pidChild: null);

        Assert.Equal(HRESULT.E_POINTER, result);
        Assert.True(accessible.IsNull);
    }

    [WinFormsFact]
    public void AccessibleObject_SystemWrapper_RuntimeId_IsValid()
    {
        AccessibleObject accessibleObject =
            (AccessibleObject)Activator.CreateInstance(typeof(AccessibleObject), BindingFlags.NonPublic | BindingFlags.Instance, null, [null], null);

        Assert.NotEmpty(accessibleObject.TestAccessor().Dynamic.RuntimeId);
    }

    private class SubAccessibleObject : AccessibleObject
    {
        public new void UseStdAccessibleObjects(IntPtr handle) => base.UseStdAccessibleObjects(handle);

        public new void UseStdAccessibleObjects(IntPtr handle, int objid) => base.UseStdAccessibleObjects(handle, objid);
    }
}
