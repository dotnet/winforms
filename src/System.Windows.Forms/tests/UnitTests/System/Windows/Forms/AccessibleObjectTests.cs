// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Accessibility;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public partial class AccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void AccessibleObject_Ctor_Default()
        {
            var accessibleObject = new AccessibleObject();
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
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void AccessibleObject_Name_Set_GetReturnsNull(string value)
        {
            var accessibleObject = new AccessibleObject
            {
                Name = value
            };
            Assert.Null(accessibleObject.Name);

            // Set same.
            accessibleObject.Name = value;
            Assert.Null(accessibleObject.Name);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void AccessibleObject_Value_Set_GetReturnsEmpty(string value)
        {
            var accessibleObject = new AccessibleObject
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
            var accessibleObject = new AccessibleObject();
            accessibleObject.DoDefaultAction();
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void AccessibleObject_GetChild_InvokeDefault_ReturnsNull(int index)
        {
            var accessibleObject = new AccessibleObject();
            Assert.Null(accessibleObject.GetChild(index));
        }

        [WinFormsFact]
        public void AccessibleObject_GetChildCount_InvokeDefault_ReturnsMinusOne()
        {
            var accessibleObject = new AccessibleObject();
            Assert.Equal(-1, accessibleObject.GetChildCount());
        }

        [WinFormsFact]
        public void AccessibleObject_GetFocused_InvokeDefault_ReturnsNull()
        {
            var accessibleObject = new AccessibleObject();
            Assert.Null(accessibleObject.GetFocused());
        }

        [WinFormsFact]
        public void AccessibleObject_GetFocused_InvokeDefaultWithNoChildren_ReturnsExpected()
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
                var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
                mockAccessibleObjectChild1
                    .Setup(a => a.State)
                    .Returns(AccessibleStates.Focused);
                var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
                mockAccessibleObjectChild2
                    .Setup(a => a.State)
                    .Returns(AccessibleStates.Focused);

                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            var accessibleObject = new AccessibleObject();
            Assert.Equal(-1, accessibleObject.GetHelpTopic(out string fileName));
            Assert.Null(fileName);
        }

        [WinFormsFact]
        public void AccessibleObject_GetSelected_InvokeDefault_ReturnsNull()
        {
            var accessibleObject = new AccessibleObject();
            Assert.Null(accessibleObject.GetSelected());
        }

        [WinFormsFact]
        public void AccessibleObject_GetSelected_InvokeDefaultWithNoChildren_ReturnsExpected()
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
                var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
                mockAccessibleObjectChild1
                    .Setup(a => a.State)
                    .Returns(AccessibleStates.Selected);
                var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
                mockAccessibleObjectChild2
                    .Setup(a => a.State)
                    .Returns(AccessibleStates.Selected);

                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            var accessibleObject = new AccessibleObject();
            Assert.Null(accessibleObject.HitTest(x, y));
        }

        [WinFormsTheory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 5)]
        public void AccessibleObject_HitTest_InvokeDefaultWithNoChildrenBoundsValid_ReturnsExpected(int x, int y)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
                var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
                mockAccessibleObjectChild1
                    .Setup(a => a.Bounds)
                    .Returns(new Rectangle(1, 2, 3, 4));
                var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
                mockAccessibleObjectChild2
                    .Setup(a => a.Bounds)
                    .Returns(new Rectangle(1, 2, 3, 4));

                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AccessibleNavigation))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AccessibleNavigation))]
        public void AccessibleObject_Navigate_InvokeDefault_ReturnsNull(AccessibleNavigation navdir)
        {
            var accessibleObject = new AccessibleObject();
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
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            var mockParentAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockParentAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(parentChildCount)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            mockAccessibleObject.Verify(a => a.Parent, Times.Once());;
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
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AccessibleSelection))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AccessibleSelection))]
        public void AccessibleObject_Navigate_InvokeDefault_Nop(AccessibleSelection flags)
        {
            var accessibleObject = new AccessibleObject();
            accessibleObject.Select(flags);
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
            var accessibleObject = new SubAccessibleObject();
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
            using var control = new Control();
            var accessibleObject = new SubAccessibleObject();
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
            using var control = new Label
            {
                Text = "Text"
            };
            var accessibleObject = new SubAccessibleObject();
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
            var accessibleObject = new SubAccessibleObject();
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
            using var control = new Control();
            var accessibleObject = new SubAccessibleObject();
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
            using var control = new Control();
            var accessibleObject = new SubAccessibleObject();
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
            using var control = new Control();
            var accessibleObject = new SubAccessibleObject();
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
            using var control = new Label
            {
                Text = "Text"
            };
            var accessibleObject = new SubAccessibleObject();
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
            using var control = new Label
            {
                Text = "Text"
            };
            var accessibleObject = new SubAccessibleObject();
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
            using var control = new Label
            {
                Text = "Text"
            };
            var accessibleObject = new SubAccessibleObject();
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
        public void AccessibleObject_IAccessiblaccChildCount_InvokeDefault_ReturnsExpected(int childCount, int expectedChildCount)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccDoDefaultAction_InvokeDefaultSelf_Success(object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccDoDefaultAction_InvokeDefaultChild_Success(object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.DoDefaultAction())
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.DoDefaultAction())
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccDoDefaultAction_InvokeDefaultNoSuchChild_Success(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessiblget_accFocus_InvokeDefault_ReturnsExpected(AccessibleObject result)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetFocused())
                .Returns(result)
                .Verifiable();
            IAccessible iAccessible = mockAccessibleObject.Object;
            Assert.Same(result, iAccessible.accFocus);
            mockAccessibleObject.Verify(a => a.GetFocused(), Times.Once());
        }

        [WinFormsFact]
        public void AccessibleObject_IAccessiblaccFocus_InvokeDefaultSelf_ReturnsExpected()
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccHitTest_InvokeDefault_ReturnsNull(int x, int y)
        {
            var accessibleObject = new AccessibleObject();
            IAccessible iAccessible = accessibleObject;
            Assert.Null(iAccessible.accHitTest(x, y));
        }

        [WinFormsTheory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 5)]
        public void AccessibleObject_IAccessibleaccHitTest_InvokeDefaultWithNoChildrenBoundsValid_ReturnsExpected(int x, int y)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccHitTest_InvokeDefaultWithNoChildrenBoundsInvalid_ReturnsNull(int x, int y)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccHitTest_InvokeDefaultWithNoBoundsChildren_ReturnsExpected(int x, int y)
        {
            using (new NoAssertContext())
            {
                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccHitTest_InvokeDefaultWithBoundsChildren_ReturnsExpected(int x, int y)
        {
            using (new NoAssertContext())
            {
                var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
                mockAccessibleObjectChild1
                    .Setup(a => a.Bounds)
                    .Returns(new Rectangle(1, 2, 3, 4));
                var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
                mockAccessibleObjectChild2
                    .Setup(a => a.Bounds)
                    .Returns(new Rectangle(1, 2, 3, 4));

                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccLocation_InvokeDefaultSelf_Success(object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccLocation_InvokeDefaultChild_Success(object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Bounds)
                .Returns(new Rectangle(1, 2, 3, 4))
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.Bounds)
                .Returns(new Rectangle(1, 2, 3, 4))
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccLocation_InvokeDefaultNoSuchChild_Success(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccNavigate_InvokeDefaultSelf_Success(int navDir, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccNavigate_InvokeDefaultChild_Success(int navDir, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Navigate((AccessibleNavigation)navDir))
                .Returns(mockAccessibleObjectChild1.Object)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.Navigate((AccessibleNavigation)navDir))
                .Returns(mockAccessibleObjectChild2.Object)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccNavigate_InvokeDefaultChildSelf_Success(int navDir, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Navigate((AccessibleNavigation)navDir))
                .Returns(mockAccessibleObject.Object)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccNavigate_InvokeDefaultNoSuchChild_Success(int navDir, object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessiblaccParent_InvokeDefault_ReturnsExpected(AccessibleObject result)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.Parent)
                .Returns(result)
                .Verifiable();
            IAccessible iAccessible = mockAccessibleObject.Object;
            Assert.Same(result, iAccessible.accParent);
            mockAccessibleObject.Verify(a => a.Parent, Times.Once());
        }

        [WinFormsFact]
        public void AccessibleObject_IAccessiblaccParent_InvokeDefaultSelf_ReturnsExpected()
        {
            using (new NoAssertContext())
            {
                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccSelect_InvokeDefaultSelf_Success(int flagsSelect, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccSelect_InvokeDefaultChild_Success(int flagsSelect, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Select((AccessibleSelection)flagsSelect))
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.Select((AccessibleSelection)flagsSelect))
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleaccSelect_InvokeDefaultNoSuchChild_Success(int flagsSelect, object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessiblget_accSelection_InvokeDefault_ReturnsExpected(AccessibleObject result)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetSelected())
                .Returns(result)
                .Verifiable();
            IAccessible iAccessible = mockAccessibleObject.Object;
            Assert.Same(result, iAccessible.accSelection);
            mockAccessibleObject.Verify(a => a.GetSelected(), Times.Once());
        }

        [WinFormsFact]
        public void AccessibleObject_IAccessiblaccSelection_InvokeDefaultSelf_ReturnsExpected()
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_AccChild_InvokeDefaultSelf_ReturnsExpected(object varChild)
        {
            var accessibleObject = new AccessibleObject();
            IAccessible iAccessible = accessibleObject;
            Assert.Same(iAccessible, iAccessible.get_accChild(varChild));
        }

        [WinFormsFact]
        public void AccessibleObject_IAccessibleget_accChild_InvokeDefaultChild_Success()
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accChild_InvokeDefaultChildSelf_ReturnsExpected(object varChild)
        {
            using (new NoAssertContext())
            {
                var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
                var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

                var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accChild_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accDefaultAction_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.DefaultAction)
                .Returns(result)
                .Verifiable();
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
        public void AccessibleObject_IAccessibleget_accDefaultAction_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.DefaultAction)
                .Returns(result)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.DefaultAction)
                .Returns(result)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accDefaultAction_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accDescription_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.Description)
                .Returns(result)
                .Verifiable();
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
        public void AccessibleObject_IAccessibleget_accDescription_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Description)
                .Returns(result)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.Description)
                .Returns(result)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            Assert.Equal(result, iAccessible.get_accDescription(varChild));
            mockAccessibleObjectChild1.Verify(a => a.Description, Times.Exactly(child1CallCount));
            mockAccessibleObjectChild2.Verify(a => a.Description, Times.Exactly(child2CallCount));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(4)]
        public void AccessibleObject_IAccessibleget_accDescription_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accHelp_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.Help)
                .Returns(result)
                .Verifiable();
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
        public void AccessibleObject_IAccessibleget_accHelp_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Help)
                .Returns(result)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.Help)
                .Returns(result)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            Assert.Equal(result, iAccessible.get_accHelp(varChild));
            mockAccessibleObjectChild1.Verify(a => a.Help, Times.Exactly(child1CallCount));
            mockAccessibleObjectChild2.Verify(a => a.Help, Times.Exactly(child2CallCount));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(4)]
        public void AccessibleObject_IAccessibleget_accHelp_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accHelpTopic_InvokeDefaultSelf_ReturnsExpected(int result, string stringResult, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            string dummy;
            HelpTopicDelegate handler = (out string pszHelpFile) =>
            {
                pszHelpFile = stringResult;
            };
            mockAccessibleObject
                .Setup(a => a.GetHelpTopic(out dummy))
                .Callback(handler)
                .Returns(result);
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
        public void AccessibleObject_IAccessibleget_accHelpTopic_InvokeDefaultChild_ReturnsExpected(int result, string stringResult, object varChild)
        {
            string dummy;
            HelpTopicDelegate handler = (out string pszHelpFile) =>
            {
                pszHelpFile = stringResult;
            };
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.GetHelpTopic(out dummy))
                .Callback(handler)
                .Returns(result);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.GetHelpTopic(out dummy))
                .Callback(handler)
                .Returns(result);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            Assert.Equal(result, iAccessible.get_accHelpTopic(out string pszHelpFile, varChild));
            Assert.Equal(stringResult, pszHelpFile);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(4)]
        public void AccessibleObject_IAccessibleget_accHelpTopic_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accKeyboardShortcut_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>
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
        public void AccessibleObject_IAccessibleget_accKeyboardShortcut_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>
            {
                CallBase = true
            };
            mockAccessibleObjectChild1
                .Setup(a => a.KeyboardShortcut)
                .Returns(result)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>
            {
                CallBase = true
            };
            mockAccessibleObjectChild2
                .Setup(a => a.KeyboardShortcut)
                .Returns(result)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>
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
        public void AccessibleObject_IAccessibleget_accKeyboardShortcut_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>
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
        public void AccessibleObject_IAccessibleget_accName_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>
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
        public void AccessibleObject_IAccessibleget_accName_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>
            {
                CallBase = true
            };
            mockAccessibleObjectChild1
                .Setup(a => a.Name)
                .Returns(result)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>
            {
                CallBase = true
            };
            mockAccessibleObjectChild2
                .Setup(a => a.Name)
                .Returns(result)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>
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
        public void AccessibleObject_IAccessibleget_accName_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>
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
        public void AccessibleObject_IAccessibleget_accRole_InvokeDefaultSelf_ReturnsExpected(AccessibleRole result, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.Role)
                .Returns(result)
                .Verifiable();
            IAccessible iAccessible = mockAccessibleObject.Object;
            Assert.Equal((int)result, iAccessible.get_accRole(varChild));
            mockAccessibleObject.Verify(a => a.Role, Times.Once());
        }

        [WinFormsTheory]
        [InlineData(AccessibleRole.None, 2, 1, 0)]
        [InlineData(AccessibleRole.Default, 2, 1, 0)]
        [InlineData(AccessibleRole.Sound, 2, 1, 0)]
        [InlineData(AccessibleRole.None, 3, 0, 1)]
        [InlineData(AccessibleRole.Default, 3, 0, 1)]
        [InlineData(AccessibleRole.Sound, 3, 0, 1)]
        public void AccessibleObject_IAccessibleget_accRole_InvokeDefaultChild_ReturnsExpected(AccessibleRole result, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Role)
                .Returns(result)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.Role)
                .Returns(result)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accRole_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accState_InvokeDefaultSelf_ReturnsExpected(AccessibleStates result, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accState_InvokeDefaultChild_ReturnsExpected(AccessibleStates result, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.State)
                .Returns(result)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.State)
                .Returns(result)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accState_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleget_accValue_InvokeDefaultSelf_ReturnsExpected(string result, object varChild)
        {
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.Value)
                .Returns(result)
                .Verifiable();
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
        public void AccessibleObject_IAccessibleget_accValue_InvokeDefaultChild_ReturnsExpected(string result, object varChild, int child1CallCount, int child2CallCount)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild1
                .Setup(a => a.Value)
                .Returns(result)
                .Verifiable();
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObjectChild2
                .Setup(a => a.Value)
                .Returns(result)
                .Verifiable();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
            Assert.Equal(result, iAccessible.get_accValue(varChild));
            mockAccessibleObjectChild1.Verify(a => a.Value, Times.Exactly(child1CallCount));
            mockAccessibleObjectChild2.Verify(a => a.Value, Times.Exactly(child2CallCount));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(4)]
        public void AccessibleObject_IAccessibleget_accValue_InvokeDefaultNoSuchChild_ReturnsNull(object varChild)
        {
            var mockAccessibleObjectChild1 = new Mock<AccessibleObject>(MockBehavior.Strict);
            var mockAccessibleObjectChild2 = new Mock<AccessibleObject>(MockBehavior.Strict);

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
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
        public void AccessibleObject_IAccessibleset_accName_InvokeDefaultSelf_ReturnsExpected(object varChild, string value)
        {
            var accessibleObject = new AccessibleObject();
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
        public void AccessibleObject_IAccessibleset_accName_InvokeDefaultChild_ReturnsExpected(object varChild, string value)
        {
            var childAccesibleObject1 = new AccessibleObject();
            var childAccesibleObject2 = new AccessibleObject();

            var mockAccessibleObject = new Mock<AccessibleObject>
            {
                CallBase = true
            };
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(childAccesibleObject1);
            mockAccessibleObject
                .Setup(a => a.GetChild(2))
                .Returns(childAccesibleObject2);
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(3);

            IAccessible iAccessible = mockAccessibleObject.Object;
            iAccessible.set_accName(varChild, value);
            Assert.Null(iAccessible.get_accName(varChild));
            Assert.Null(childAccesibleObject1.Name);
            Assert.Null(childAccesibleObject2.Name);
        }

        [WinFormsTheory]
        [InlineData(-1, null)]
        [InlineData(-1, "")]
        [InlineData(-1, "value")]
        [InlineData(4, null)]
        [InlineData(4, "")]
        [InlineData(4, "value")]
        public void AccessibleObject_IAccessibleset_accName_InvokeDefaultNoSuchChild_ReturnsNull(object varChild, string value)
        {
            var childAccesibleObject1 = new AccessibleObject();
            var childAccesibleObject2 = new AccessibleObject();

            var mockAccessibleObject = new Mock<AccessibleObject>
            {
                CallBase = true
            };
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(childAccesibleObject1);
            mockAccessibleObject
                .Setup(a => a.GetChild(2))
                .Returns(childAccesibleObject2);
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(3);

            IAccessible iAccessible = mockAccessibleObject.Object;
            iAccessible.set_accName(varChild, value);
            Assert.Null(iAccessible.get_accName(varChild));
            Assert.Null(childAccesibleObject1.Name);
            Assert.Null(childAccesibleObject2.Name);
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
        public void AccessibleObject_IAccessibleset_accValue_InvokeDefaultSelf_ReturnsExpected(object varChild, string value)
        {
            var accessibleObject = new AccessibleObject();
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
        public void AccessibleObject_IAccessibleset_accValue_InvokeDefaultChild_ReturnsExpected(object varChild, string value)
        {
            var childAccesibleObject1 = new AccessibleObject();
            var childAccesibleObject2 = new AccessibleObject();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(childAccesibleObject1);
            mockAccessibleObject
                .Setup(a => a.GetChild(2))
                .Returns(childAccesibleObject2);
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(3);

            IAccessible iAccessible = mockAccessibleObject.Object;
            iAccessible.set_accValue(varChild, value);
            Assert.Empty(iAccessible.get_accValue(varChild));
            Assert.Empty(childAccesibleObject1.Value);
            Assert.Empty(childAccesibleObject2.Value);
        }

        [WinFormsTheory]
        [InlineData(-1, null)]
        [InlineData(-1, "")]
        [InlineData(-1, "value")]
        [InlineData(4, null)]
        [InlineData(4, "")]
        [InlineData(4, "value")]
        public void AccessibleObject_IAccessibleset_accValue_InvokeDefaultNoSuchChild_ReturnsNull(object varChild, string value)
        {
            var childAccesibleObject1 = new AccessibleObject();
            var childAccesibleObject2 = new AccessibleObject();

            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.GetChild(0))
                .Returns((AccessibleObject)null);
            mockAccessibleObject
                .Setup(a => a.GetChild(1))
                .Returns(childAccesibleObject1);
            mockAccessibleObject
                .Setup(a => a.GetChild(2))
                .Returns(childAccesibleObject2);
            mockAccessibleObject
                .Setup(a => a.GetChildCount())
                .Returns(3);

            IAccessible iAccessible = mockAccessibleObject.Object;
            iAccessible.set_accValue(varChild, value);
            Assert.Null(iAccessible.get_accValue(varChild));
            Assert.Empty(childAccesibleObject1.Value);
            Assert.Empty(childAccesibleObject2.Value);
        }

        [WinFormsFact]
        public void AccessibleObject_AsIAccessible_Invoke_DoesntReturnWrapper()
        {
            using Panel panel = new Panel();
            panel.CreateControl();
            using Button button = new Button();
            button.CreateControl();
            panel.Controls.Add(button);

            IAccessible iAccessible = panel.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild);
            var wrapper = ((AccessibleObject)iAccessible).TestAccessor().Dynamic.systemIAccessible;
            var child = iAccessible.get_accChild(0);

            Assert.NotSame(wrapper, child);
            Assert.Same(((AccessibleObject)iAccessible).GetSystemIAccessibleInternal(), child);
        }

        [WinFormsFact]
        public void AccessibleObject_GetSystemIAccessibleInternal_Invoke_DoesntReturnWrapper()
        {
            using Button button = new Button();
            button.CreateControl();
            var accessibleObject = button.AccessibilityObject;
            var wrapper = accessibleObject.TestAccessor().Dynamic.systemIAccessible;
            Assert.NotSame(wrapper, accessibleObject.GetSystemIAccessibleInternal());
        }

        [WinFormsFact]
        public void AccessibleObject_WrapIAccessible_Invoke_DoesntReturnWrapper()
        {
            using Button button = new Button();
            button.CreateControl();
            var accessibleObject = button.AccessibilityObject;
            var wrapper = accessibleObject.TestAccessor().Dynamic.systemIAccessible;
            var wrapIAccessibleResult = accessibleObject.TestAccessor().Dynamic.WrapIAccessible(accessibleObject.GetSystemIAccessibleInternal());

            Assert.Same(accessibleObject, wrapIAccessibleResult);
            Assert.NotSame(wrapper, accessibleObject.GetSystemIAccessibleInternal());
        }

        [DllImport("Oleacc.dll")]
        internal unsafe static extern HRESULT AccessibleObjectFromPoint(
            Point ptScreen,
            [MarshalAs(UnmanagedType.Interface)]
            out object ppacc,
            out object pvarChild
        );

        [WinFormsFact(Skip = "This test needs to be run manually as it depends on the form being unobstructed.")]
        public unsafe void TestAccessibleObjectFromPoint_Button()
        {
            using Form form = new Form();
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
            Assert.True(((int)accessible.accState & 0x100000) != 0);    // STATE_SYSTEM_FOCUSABLE
            Assert.Equal(0x2b, accessible.accRole);                     // ROLE_SYSTEM_PUSHBUTTON
            Assert.Equal("Press", accessible.accDefaultAction);
        }

        [WinFormsFact(Skip = "This test needs to be run manually as it depends on the form being unobstructed.")]
        public unsafe void TestAccessibleObjectFromPoint_ComboBox()
        {
            using Form form = new Form();
            using ComboBox comboBox = new ComboBox();
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
            Assert.True(((int)accessible.accState & 0x100000) != 0);    // STATE_SYSTEM_FOCUSABLE
            Assert.Equal(0x2a, accessible.accRole);                     // ROLE_SYSTEM_TEXT
            Assert.Null(accessible.accDefaultAction);

            var parent = accessible.accParent as IAccessible;
            Assert.Equal(0x09, parent.accRole);                         // ROLE_SYSTEM_WINDOW
            Assert.Equal(7, parent.accChildCount);
        }

        private class SubAccessibleObject : AccessibleObject
        {
            public new void UseStdAccessibleObjects(IntPtr handle) => base.UseStdAccessibleObjects(handle);

            public new void UseStdAccessibleObjects(IntPtr handle, int objid) => base.UseStdAccessibleObjects(handle, objid);
        }
    }
}
