// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms.Layout;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    [Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads (instantiated via GUID)
    public class ControlControlCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ControlCollection_Ctor_Control()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.Empty(collection);
            Assert.False(collection.IsReadOnly);
            Assert.Same(owner, collection.Owner);
        }

        [WinFormsFact]
        public void ControlCollection_Ctor_NullOwner_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("owner", () => new Control.ControlCollection(null));
        }

        [WinFormsFact]
        public void ControlCollection_Add_ControlNewCollection_Success()
        {
            using var owner = new Control();
            using var control1 = new Control();
            using var control2 = new Control();
            var collection = new Control.ControlCollection(owner);
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(collection.Cast<Control>().Last(), e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            int layoutCallCount1 = 0;
            control1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            control2.Layout += (sender, e) => layoutCallCount2++;

            try
            {
                collection.Add(control1);
                Assert.Same(control1, Assert.Single(collection));
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Empty(owner.Controls);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);

                // Add another.
                collection.Add(control2);
                Assert.Equal(new Control[] { control1, control2 }, collection.Cast<Control>());
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(owner, control2.Parent);
                Assert.Equal(1, control2.TabIndex);
                Assert.Empty(owner.Controls);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);
                Assert.False(control2.IsHandleCreated);

                // Add existing.
                Assert.Throws<ArgumentException>(null, () => collection.Add(control2));
                Assert.Equal(new Control[] { control1, control2 }, collection.Cast<Control>());
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(owner, control2.Parent);
                Assert.Equal(1, control2.TabIndex);
                Assert.Empty(owner.Controls);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);
                Assert.False(control2.IsHandleCreated);

                // Add null.
                collection.Add(null);
                Assert.Equal(new Control[] { control1, control2 }, collection.Cast<Control>());
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Add_ControlExistingCollection_Success()
        {
            using var owner = new Control();
            using var control1 = new Control();
            using var control2 = new Control();
            Control.ControlCollection collection = owner.Controls;
            int parentLayoutCallCount = 0;
            string affectedProperty = null;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(collection.Cast<Control>().Last(), e.AffectedControl);
                Assert.Equal(affectedProperty, e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            int layoutCallCount1 = 0;
            control1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            control2.Layout += (sender, e) => layoutCallCount2++;

            try
            {
                affectedProperty = "Parent";
                collection.Add(control1);
                Assert.Same(control1, Assert.Single(collection));
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(control1, Assert.Single(owner.Controls));
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);

                // Add another.
                collection.Add(control2);
                Assert.Equal(new Control[] { control1, control2 }, collection.Cast<Control>());
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(owner, control2.Parent);
                Assert.Equal(1, control2.TabIndex);
                Assert.Equal(new Control[] { control1, control2 }, owner.Controls.Cast<Control>());
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);
                Assert.False(control2.IsHandleCreated);

                // Add existing.
                affectedProperty = "ChildIndex";
                collection.Add(control1);
                Assert.Equal(new Control[] { control2, control1 }, collection.Cast<Control>());
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(owner, control2.Parent);
                Assert.Equal(1, control2.TabIndex);
                Assert.Equal(new Control[] { control2, control1}, owner.Controls.Cast<Control>());
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(3, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);
                Assert.False(control2.IsHandleCreated);

                // Add null.
                collection.Add(null);
                Assert.Equal(new Control[] { control2, control1 }, collection.Cast<Control>());
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Add_ControlHasParent_Success()
        {
            using var owner1 = new Control();
            using var owner2 = new Control();
            using var control = new Control();
            Control.ControlCollection collection1 = owner1.Controls;
            Control.ControlCollection collection2 = owner2.Controls;
            int layoutCallCount = 0;
            string affectedProperty = "Parent";
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner2, sender);
                Assert.Same(collection2.Cast<Control>().Last(), e.AffectedControl);
                Assert.Equal(affectedProperty, e.AffectedProperty);
                layoutCallCount++;
            }
            owner2.Layout += parentHandler;

            try
            {
                collection1.Add(control);
                collection2.Add(control);
                Assert.Same(control, Assert.Single(collection2));
                Assert.Empty(collection1);
                Assert.Same(owner2, control.Parent);
                Assert.Equal(0, control.TabIndex);
                Assert.Same(control, Assert.Single(owner2.Controls));
                Assert.Equal(1, layoutCallCount);
                Assert.False(owner2.IsHandleCreated);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                owner2.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Add_ControlWithCustomLayoutEngine_Success()
        {
            using var owner = new Control();
            Control.ControlCollection collection = owner.Controls;
            int layoutCallCount = 0;
            string affectedProperty = null;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(collection.Cast<Control>().Last(), e.AffectedControl);
                Assert.Equal(affectedProperty, e.AffectedProperty);
                layoutCallCount++;
            }
            owner.Layout += parentHandler;

            using var control = new CustomLayoutEngineControl();
            var mockLayoutEngine = new Mock<LayoutEngine>(MockBehavior.Strict);
            mockLayoutEngine
                .Setup(e => e.InitLayout(control, BoundsSpecified.All))
                .Verifiable();
            control.SetLayoutEngine(mockLayoutEngine.Object);

            try
            {
                affectedProperty = "Parent";
                collection.Add(control);
                Assert.Same(control, Assert.Single(collection));
                Assert.Same(owner, control.Parent);
                Assert.Equal(0, control.TabIndex);
                Assert.Same(control, Assert.Single(owner.Controls));
                Assert.Equal(1, layoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control.IsHandleCreated);
                mockLayoutEngine.Verify(e => e.InitLayout(control, BoundsSpecified.All), Times.Once());

                // Add existing.
                affectedProperty = "ChildIndex";
                collection.Add(control);
                Assert.Same(control, Assert.Single(collection));
                Assert.Same(owner, control.Parent);
                Assert.Equal(0, control.TabIndex);
                Assert.Same(control, Assert.Single(owner.Controls));
                Assert.Equal(2, layoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control.IsHandleCreated);
                mockLayoutEngine.Verify(e => e.InitLayout(control, BoundsSpecified.All), Times.Once());
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Add_TabIndex_Success()
        {
            using var owner = new SubControl();
            Control.ControlCollection collection = owner.Controls;
            using var control1 = new SubControl
            {
                TabIndex = 1
            };
            int tabIndexChangedCallCount1 = 0;
            control1.TabIndexChanged += (sender, e) => tabIndexChangedCallCount1++;

            collection.Add(control1);
            Assert.Equal(new Control[] { control1 }, collection.Cast<Control>());
            Assert.Same(owner, control1.Parent);
            Assert.Equal(1, control1.TabIndex);
            Assert.Equal(0, tabIndexChangedCallCount1);

            // Add another.
            using var control2 = new SubControl();
            int tabIndexChangedCallCount2 = 0;
            control2.TabIndexChanged += (sender, e) => tabIndexChangedCallCount2++;
            collection.Add(control2);
            Assert.Equal(new Control[] { control1, control2 }, collection.Cast<Control>());
            Assert.Same(owner, control1.Parent);
            Assert.Equal(1, control1.TabIndex);
            Assert.Equal(0, tabIndexChangedCallCount1);
            Assert.Same(owner, control2.Parent);
            Assert.Equal(2, control2.TabIndex);
            Assert.Equal(0, tabIndexChangedCallCount2);

            // Add another.
            control1.TabIndex = 10;
            using var control3 = new SubControl();
            int tabIndexChangedCallCount3 = 0;
            control3.TabIndexChanged += (sender, e) => tabIndexChangedCallCount3++;
            collection.Add(control3);
            Assert.Equal(new Control[] { control1, control2, control3 }, collection.Cast<Control>());
            Assert.Same(owner, control1.Parent);
            Assert.Equal(10, control1.TabIndex);
            Assert.Equal(1, tabIndexChangedCallCount1);
            Assert.Same(owner, control2.Parent);
            Assert.Equal(2, control2.TabIndex);
            Assert.Equal(0, tabIndexChangedCallCount2);
            Assert.Same(owner, control3.Parent);
            Assert.Equal(11, control3.TabIndex);
            Assert.Equal(0, tabIndexChangedCallCount3);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ControlCollection_Add_InvokeValueWithoutHandleOwnerWithHandle_CreatedValueIfVisible(bool visible)
        {
            using var owner = new Control();
            using var control = new Control
            {
                Visible = visible
            };
            Control.ControlCollection collection = owner.Controls;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int parentInvalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

            collection.Add(control);
            Assert.Same(control, Assert.Single(collection));
            Assert.Same(owner, control.Parent);
            Assert.Equal(visible, control.Visible);
            Assert.Equal(visible, control.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ControlCollection_Add_InvokeValueWithHandleOwnerWithoutHandle_Success(bool visible)
        {
            using var owner = new Control();
            using var control = new Control
            {
                Visible = visible
            };
            Control.ControlCollection collection = owner.Controls;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            collection.Add(control);
            Assert.Same(control, Assert.Single(collection));
            Assert.Same(owner, control.Parent);
            Assert.Equal(visible, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ControlCollection_Add_InvokeValueWithHandleOwnerWithHandleControlNewCollection_Success(bool visible)
        {
            using var owner = new Control();
            using var control = new Control
            {
                Visible = visible
            };
            var collection = new Control.ControlCollection(owner);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int parentInvalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            owner.HandleCreated += (sender, e) => parentCreatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Throws<ArgumentException>(null, () => collection.Add(control));
            Assert.Same(control, Assert.Single(collection));
            Assert.Same(owner, control.Parent);
            Assert.Equal(visible, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ControlCollection_Add_InvokeValueWithHandleOwnerWithHandleControlExistingCollection_Success(bool visible)
        {
            using var owner = new Control();
            using var control = new Control
            {
                Visible = visible
            };
            Control.ControlCollection collection = owner.Controls;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int parentInvalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            owner.HandleCreated += (sender, e) => parentCreatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            collection.Add(control);
            Assert.Equal(owner.Handle, User32.GetParent(control.Handle));
            Assert.Same(control, Assert.Single(collection));
            Assert.Same(owner, control.Parent);
            Assert.Equal(visible, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }

        [WinFormsFact]
        public void ControlCollection_Add_InvokeWithHandler_CallsControlAdded()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            Control.ControlCollection collection = owner.Controls;
            int parentChangedCallCount = 0;
            int callCount = 0;
            int parentLayoutCallCount = 0;
            child1.ParentChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                parentChangedCallCount++;
            };
            child2.ParentChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                parentChangedCallCount++;
            };
            object affectedControl = null;
            string affectedProperty = null;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(affectedControl, e.AffectedControl);
                Assert.Equal(affectedProperty, e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            void handler(object sender, ControlEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.Control);
                Assert.Equal(parentChangedCallCount - 1, callCount);
                Assert.Equal(parentLayoutCallCount - 1, callCount);
                callCount++;
            }

            try
            {
                // Call with handler.
                owner.ControlAdded += handler;
                affectedControl = child1;
                affectedProperty = "Parent";
                collection.Add(child1);
                Assert.Same(owner, child1.Parent);
                Assert.Equal(1, parentChangedCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.Equal(1, callCount);

                // Call again.
                affectedProperty = "ChildIndex";
                collection.Add(child1);
                Assert.Same(owner, child1.Parent);
                Assert.Equal(1, parentChangedCallCount);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.Equal(1, callCount);

                // Remove handler.
                owner.ControlAdded -= handler;
                affectedControl = child2;
                affectedProperty = "Parent";
                collection.Add(child2);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(2, parentChangedCallCount);
                Assert.Equal(3, parentLayoutCallCount);
                Assert.Equal(1, callCount);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Add_InvokeWithNonOverridenProperties_DoesNotCallPropertyHandlers()
        {
            using var owner = new Control();
            using var control = new Control();
            var collection = new Control.ControlCollection(owner);

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            control.ParentChanged += (sender, e) => parentChangedCallCount++;
            control.EnabledChanged += (sender, e) => enabledChangedCallCount++;
            control.VisibleChanged += (sender, e) => visibleChangedCallCount++;
            control.FontChanged += (sender, e) => fontChangedCallCount++;
            control.ForeColorChanged += (sender, e) => foreColorChangedCallCount++;
            control.BackColorChanged += (sender, e) => backColorChangedCallCount++;
            control.RightToLeftChanged += (sender, e) => rightToLeftChangedCallCount++;
            control.BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            collection.Add(control);
            Assert.Same(owner, control.Parent);
            Assert.Equal(1, parentChangedCallCount);
            Assert.True(control.Enabled);
            Assert.Equal(0, enabledChangedCallCount);
            Assert.True(control.Visible);
            Assert.Equal(0, visibleChangedCallCount);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(0, fontChangedCallCount);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(0, rightToLeftChangedCallCount);
            Assert.Null(control.BindingContext);
            Assert.Equal(0, bindingContextChangedCallCount);
        }

        [WinFormsFact]
        public void ControlCollection_Add_InvokeWithNonOverridenPropertiesWithHandle_DoesNotCallPropertyHandlers()
        {
            using var owner = new Control();
            using var control = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            control.ParentChanged += (sender, e) => parentChangedCallCount++;
            control.EnabledChanged += (sender, e) => enabledChangedCallCount++;
            control.VisibleChanged += (sender, e) => visibleChangedCallCount++;
            control.FontChanged += (sender, e) => fontChangedCallCount++;
            control.ForeColorChanged += (sender, e) => foreColorChangedCallCount++;
            control.BackColorChanged += (sender, e) => backColorChangedCallCount++;
            control.RightToLeftChanged += (sender, e) => rightToLeftChangedCallCount++;
            control.BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            collection.Add(control);
            Assert.Same(owner, control.Parent);
            Assert.Equal(1, parentChangedCallCount);
            Assert.True(control.Enabled);
            Assert.Equal(0, enabledChangedCallCount);
            Assert.True(control.Visible);
            Assert.Equal(0, visibleChangedCallCount);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(0, fontChangedCallCount);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(0, rightToLeftChangedCallCount);
            Assert.Null(control.BindingContext);
            Assert.Equal(1, bindingContextChangedCallCount);
        }

        public static IEnumerable<object[]> Add_OverridenProperties_TestData()
        {
            var parentContext = new BindingContext();
            var childContext = new BindingContext();
            yield return new object[] { parentContext, childContext, childContext };
            yield return new object[] { parentContext, null, parentContext };
            yield return new object[] { null, childContext, childContext };
            yield return new object[] { null, null, null };
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_OverridenProperties_TestData))]
        public void ControlCollection_Add_InvokeWithOverridenProperties_CallsPropertyHandlers(BindingContext parentBindingContext, BindingContext bindingContext, BindingContext expectedBindingContext)
        {
            using var owner = new Control();
            using var control = new Control();
            var collection = new Control.ControlCollection(owner);

            owner.Enabled = false;
            Assert.True(control.Enabled);
            owner.Visible = false;
            Assert.True(control.Visible);
            var font = new Font("Arial", 8.25f);
            owner.Font = font;
            Assert.Equal(Control.DefaultFont, control.Font);
            owner.ForeColor = Color.Red;
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            owner.BackColor = Color.Blue;
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            owner.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            owner.BindingContext = parentBindingContext;
            control.BindingContext = bindingContext;
            Assert.Same(bindingContext, control.BindingContext);

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            void parentChangedHandler(object sender, EventArgs e)
            {
                Assert.Equal(enabledChangedCallCount, parentChangedCallCount);
                Assert.Equal(visibleChangedCallCount, parentChangedCallCount);
                Assert.Equal(fontChangedCallCount, parentChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, parentChangedCallCount);
                Assert.Equal(backColorChangedCallCount, parentChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, parentChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, parentChangedCallCount);
                parentChangedCallCount++;
            }
            control.ParentChanged += parentChangedHandler;
            control.EnabledChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, enabledChangedCallCount);
                Assert.Equal(visibleChangedCallCount, enabledChangedCallCount);
                Assert.Equal(fontChangedCallCount, enabledChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, enabledChangedCallCount);
                Assert.Equal(backColorChangedCallCount, enabledChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, enabledChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, enabledChangedCallCount);
                enabledChangedCallCount++;
            };
            control.VisibleChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, visibleChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, visibleChangedCallCount);
                Assert.Equal(fontChangedCallCount, visibleChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, visibleChangedCallCount);
                Assert.Equal(backColorChangedCallCount, visibleChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, visibleChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, visibleChangedCallCount);
                visibleChangedCallCount++;
            };
            control.FontChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(visibleChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, fontChangedCallCount);
                Assert.Equal(backColorChangedCallCount, fontChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, fontChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, fontChangedCallCount);
                fontChangedCallCount++;
            };
            control.ForeColorChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(visibleChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(backColorChangedCallCount, foreColorChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, foreColorChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, foreColorChangedCallCount);
                foreColorChangedCallCount++;
            };
            control.BackColorChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(visibleChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(foreColorChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, backColorChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, backColorChangedCallCount);
                backColorChangedCallCount++;
            };
            control.RightToLeftChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(visibleChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(foreColorChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(backColorChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, rightToLeftChangedCallCount);
                rightToLeftChangedCallCount++;
            };
            control.BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            try
            {
                collection.Add(control);
                Assert.Same(owner, control.Parent);
                Assert.Equal(1, parentChangedCallCount);
                Assert.False(control.Enabled);
                Assert.Equal(1, enabledChangedCallCount);
                Assert.False(control.Visible);
                Assert.Equal(1, visibleChangedCallCount);
                Assert.Same(font, control.Font);
                Assert.Equal(1, fontChangedCallCount);
                Assert.Equal(Color.Red, control.ForeColor);
                Assert.Equal(1, foreColorChangedCallCount);
                Assert.Equal(Color.Blue, control.BackColor);
                Assert.Equal(1, backColorChangedCallCount);
                Assert.Equal(RightToLeft.Yes, control.RightToLeft);
                Assert.Equal(1, rightToLeftChangedCallCount);
                Assert.Same(expectedBindingContext, control.BindingContext);
                Assert.Equal(0, bindingContextChangedCallCount);
            }
            finally
            {
                control.ParentChanged -= parentChangedHandler;
            }
        }

        public static IEnumerable<object[]> Add_OverridenPropertiesWithHandle_TestData()
        {
            var parentContext = new BindingContext();
            var childContext = new BindingContext();
            yield return new object[] { parentContext, childContext, childContext, 0 };
            yield return new object[] { parentContext, null, parentContext, 1 };
            yield return new object[] { null, childContext, childContext, 0 };
            yield return new object[] { null, null, null, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_OverridenPropertiesWithHandle_TestData))]
        public void ControlCollection_Add_InvokeWithOverridenPropertiesWithHandle_CallsPropertyHandlers(BindingContext parentBindingContext, BindingContext bindingContext, BindingContext expectedBindingContext, int expectedBindingContextChangedCallCount)
        {
            using var owner = new Control();
            using var control = new Control();
            var collection = new Control.ControlCollection(owner);

            owner.Enabled = false;
            Assert.True(control.Enabled);
            owner.Visible = false;
            Assert.True(control.Visible);
            var font = new Font("Arial", 8.25f);
            owner.Font = font;
            Assert.Equal(Control.DefaultFont, control.Font);
            owner.ForeColor = Color.Red;
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            owner.BackColor = Color.Blue;
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            owner.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            owner.BindingContext = parentBindingContext;
            control.BindingContext = bindingContext;
            Assert.Same(bindingContext, control.BindingContext);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            void parentChangedHandler(object sender, EventArgs e)
            {
                Assert.Equal(enabledChangedCallCount, parentChangedCallCount);
                Assert.Equal(visibleChangedCallCount, parentChangedCallCount);
                Assert.Equal(fontChangedCallCount, parentChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, parentChangedCallCount);
                Assert.Equal(backColorChangedCallCount, parentChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, parentChangedCallCount);
                parentChangedCallCount++;
            }
            control.ParentChanged += parentChangedHandler;
            control.EnabledChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, enabledChangedCallCount);
                Assert.Equal(visibleChangedCallCount, enabledChangedCallCount);
                Assert.Equal(fontChangedCallCount, enabledChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, enabledChangedCallCount);
                Assert.Equal(backColorChangedCallCount, enabledChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, enabledChangedCallCount);
                enabledChangedCallCount++;
            };
            control.VisibleChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, visibleChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, visibleChangedCallCount);
                Assert.Equal(fontChangedCallCount, visibleChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, visibleChangedCallCount);
                Assert.Equal(backColorChangedCallCount, visibleChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, visibleChangedCallCount);
                visibleChangedCallCount++;
            };
            control.FontChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(visibleChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, fontChangedCallCount);
                Assert.Equal(backColorChangedCallCount, fontChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, fontChangedCallCount);
                fontChangedCallCount++;
            };
            control.ForeColorChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(visibleChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(backColorChangedCallCount, foreColorChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, foreColorChangedCallCount);
                foreColorChangedCallCount++;
            };
            control.BackColorChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(visibleChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(foreColorChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, backColorChangedCallCount);
                backColorChangedCallCount++;
            };
            control.RightToLeftChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(visibleChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(foreColorChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(backColorChangedCallCount - 1, rightToLeftChangedCallCount);
                rightToLeftChangedCallCount++;
            };
            control.BindingContextChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, bindingContextChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, bindingContextChangedCallCount);
                Assert.Equal(visibleChangedCallCount - 1, bindingContextChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, bindingContextChangedCallCount);
                Assert.Equal(foreColorChangedCallCount - 1, bindingContextChangedCallCount);
                Assert.Equal(backColorChangedCallCount - 1, bindingContextChangedCallCount);
                bindingContextChangedCallCount++;
            };

            try
            {
                collection.Add(control);
                Assert.Same(owner, control.Parent);
                Assert.Equal(1, parentChangedCallCount);
                Assert.False(control.Enabled);
                Assert.Equal(1, enabledChangedCallCount);
                Assert.False(control.Visible);
                Assert.Equal(1, visibleChangedCallCount);
                Assert.Same(font, control.Font);
                Assert.Equal(1, fontChangedCallCount);
                Assert.Equal(Color.Red, control.ForeColor);
                Assert.Equal(1, foreColorChangedCallCount);
                Assert.Equal(Color.Blue, control.BackColor);
                Assert.Equal(1, backColorChangedCallCount);
                Assert.Equal(RightToLeft.Yes, control.RightToLeft);
                Assert.Equal(1, rightToLeftChangedCallCount);
                Assert.Same(expectedBindingContext, control.BindingContext);
                Assert.Equal(expectedBindingContextChangedCallCount, bindingContextChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(4, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(1, createdCallCount);
            }
            finally
            {
                control.ParentChanged -= parentChangedHandler;
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_OverridenProperties_TestData))]
        public void ControlCollection_Add_InvokeWithOverridenPropertiesAxHost_DoesNotCallPropertyHandlers(BindingContext parentBindingContext, BindingContext bindingContext, BindingContext expectedBindingContext)
        {
            using var owner = new Control();
            using var control = new SubAxHost("8856f961-340a-11d0-a96b-00c04fd705a2");
            var collection = new Control.ControlCollection(owner);

            owner.Enabled = false;
            Assert.True(control.Enabled);
            owner.Visible = false;
            Assert.True(control.Visible);
            var font = new Font("Arial", 8.25f);
            owner.Font = font;
            Assert.Equal(Control.DefaultFont, control.Font);
            owner.ForeColor = Color.Red;
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            owner.BackColor = Color.Blue;
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            owner.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.No, ((Control)control).RightToLeft);
            owner.BindingContext = parentBindingContext;
            control.BindingContext = bindingContext;
            Assert.Same(bindingContext, control.BindingContext);

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            void parentChangedHandler(object sender, EventArgs e)
            {
                parentChangedCallCount++;
            }
            ((Control)control).ParentChanged += parentChangedHandler;
            ((Control)control).EnabledChanged += (sender, e) => enabledChangedCallCount++;
            ((Control)control).VisibleChanged += (sender, e) => visibleChangedCallCount++;
            ((Control)control).FontChanged += (sender, e) => fontChangedCallCount++;
            ((Control)control).ForeColorChanged += (sender, e) => foreColorChangedCallCount++;
            ((Control)control).BackColorChanged += (sender, e) => backColorChangedCallCount++;
            ((Control)control).RightToLeftChanged += (sender, e) => rightToLeftChangedCallCount++;
            ((Control)control).BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            collection.Add(control);
            try
            {
                Assert.Same(owner, control.Parent);
                Assert.Equal(1, parentChangedCallCount);
                Assert.False(control.Enabled);
                Assert.Equal(0, enabledChangedCallCount);
                Assert.False(control.Visible);
                Assert.Equal(0, visibleChangedCallCount);
                Assert.Same(font, control.Font);
                Assert.Equal(0, fontChangedCallCount);
                Assert.Equal(Color.Red, control.ForeColor);
                Assert.Equal(0, foreColorChangedCallCount);
                Assert.Equal(Color.Blue, control.BackColor);
                Assert.Equal(0, backColorChangedCallCount);
                Assert.Equal(RightToLeft.Yes, ((Control)control).RightToLeft);
                Assert.Equal(0, rightToLeftChangedCallCount);
                Assert.Same(expectedBindingContext, control.BindingContext);
                Assert.Equal(0, bindingContextChangedCallCount);
            }
            finally
            {
                control.ParentChanged -= parentChangedHandler;
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_OverridenPropertiesWithHandle_TestData))]
        public void ControlCollection_Add_InvokeWithOverridenPropertiesAxHostWithHandle_CallSPropertyHandlers(BindingContext parentBindingContext, BindingContext bindingContext, BindingContext expectedBindingContext, int expectedBindingContextChangedCallCount)
        {
            using var owner = new Control();
            using var control = new SubAxHost("8856f961-340a-11d0-a96b-00c04fd705a2");
            var collection = new Control.ControlCollection(owner);

            owner.Enabled = false;
            Assert.True(control.Enabled);
            owner.Visible = false;
            Assert.True(control.Visible);
            var font = new Font("Arial", 8.25f);
            owner.Font = font;
            Assert.Equal(Control.DefaultFont, control.Font);
            owner.ForeColor = Color.Red;
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            owner.BackColor = Color.Blue;
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            owner.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.No, ((Control)control).RightToLeft);
            owner.BindingContext = parentBindingContext;
            control.BindingContext = bindingContext;
            Assert.Same(bindingContext, control.BindingContext);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            void parentChangedHandler(object sender, EventArgs e)
            {
                parentChangedCallCount++;
            }
            ((Control)control).ParentChanged += parentChangedHandler;
            ((Control)control).EnabledChanged += (sender, e) => enabledChangedCallCount++;
            ((Control)control).VisibleChanged += (sender, e) => visibleChangedCallCount++;
            ((Control)control).FontChanged += (sender, e) => fontChangedCallCount++;
            ((Control)control).ForeColorChanged += (sender, e) => foreColorChangedCallCount++;
            ((Control)control).BackColorChanged += (sender, e) => backColorChangedCallCount++;
            ((Control)control).RightToLeftChanged += (sender, e) => rightToLeftChangedCallCount++;
            ((Control)control).BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            try
            {
                collection.Add(control);
                Assert.Same(owner, control.Parent);
                Assert.Equal(1, parentChangedCallCount);
                Assert.False(control.Enabled);
                Assert.Equal(1, enabledChangedCallCount);
                Assert.False(control.Visible);
                Assert.Equal(1, visibleChangedCallCount);
                Assert.Same(font, control.Font);
                Assert.Equal(1, fontChangedCallCount);
                Assert.Equal(Color.Red, control.ForeColor);
                Assert.Equal(1, foreColorChangedCallCount);
                Assert.Equal(Color.Blue, control.BackColor);
                Assert.Equal(1, backColorChangedCallCount);
                Assert.Equal(RightToLeft.Yes, ((Control)control).RightToLeft);
                Assert.Equal(1, rightToLeftChangedCallCount);
                Assert.Same(expectedBindingContext, control.BindingContext);
                Assert.Equal(expectedBindingContextChangedCallCount, bindingContextChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(6, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
            }
            finally
            {
                control.ParentChanged -= parentChangedHandler;
            }
        }

        private class SubAxHost : AxHost
        {
            public SubAxHost(string clsid) : base(clsid)
            {
            }

            public SubAxHost(string clsid, int flags) : base(clsid, flags)
            {
            }
        }

        [WinFormsFact]
        public void ControlCollection_Add_TopLevelValue_ThrowsArgumentException()
        {
            using var owner = new Control();
            using var control = new SubControl();
            control.SetTopLevel(true);
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection.Add(control));
        }

        [Fact] // cross-thread access
        public void ControlCollection_Add_DifferentThreadValueOwner_ThrowsArgumentException()
        {
            Control owner = null;
            var thread = new Thread(() =>
            {
                owner = new Control();
                Assert.NotEqual(IntPtr.Zero, owner.Handle);
            });
            thread.Start();
            thread.Join();

            using var control = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection.Add(control));
        }

        [Fact] // cross-thread access
        public void ControlCollection_Add_DifferentThreadValueControl_ThrowsArgumentException()
        {
            Control control = null;
            var thread = new Thread(() =>
            {
                control = new Control();
                Assert.NotEqual(IntPtr.Zero, control.Handle);
            });
            thread.Start();
            thread.Join();

            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection.Add(control));
        }

        [WinFormsFact]
        public void ControlCollection_Add_SameAsOwner_ThrowsArgumentException()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection.Add(owner));
        }

        [WinFormsFact]
        public void ControlCollection_Add_OwnerParent_ThrowsArgumentException()
        {
            using var parent = new Control();
            using var owner = new Control
            {
                Parent = parent
            };
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection.Add(parent));
        }

        [WinFormsFact]
        public void ControlCollection_Add_OwnerGrandParent_ThrowsArgumentException()
        {
            using var grandparent = new Control();
            using var parent = new Control
            {
                Parent = grandparent
            };
            using var owner = new Control
            {
                Parent = parent
            };
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection.Add(grandparent));
        }

        [WinFormsFact]
        public void ControlCollection_AddRange_Invoke_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            Control.ControlCollection collection = owner.Controls;
            int parentLayoutCallCount = 0;
            object affectedControl = null;
            string affectedProperty = null;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(affectedControl, e.AffectedControl);
                Assert.Equal(affectedProperty, e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            int controlAddedCallCount = 0;
            owner.ControlAdded += (sender, e) => controlAddedCallCount++;

            try
            {
                affectedControl = child3;
                affectedProperty = "Parent";
                collection.AddRange(new Control[] { child1, child2, null, child3 });
                Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());
                Assert.Same(owner, child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Same(owner, child3.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.Equal(3, controlAddedCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                affectedControl = child1;
                affectedProperty = "ChildIndex";
                collection.AddRange(new Control[] { child1, child2, null, child3 });
                Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());
                Assert.Same(owner, child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Same(owner, child3.Parent);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.Equal(3, controlAddedCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                // Add empty.
                collection.AddRange(Array.Empty<Control>());
                Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());
                Assert.Same(owner, child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Same(owner, child3.Parent);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.Equal(3, controlAddedCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_AddRange_NullControls_ThrowsArgumentNullException()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentNullException>("controls", () => collection.AddRange(null));
        }

        [WinFormsFact]
        public void ControlCollection_Clear_Invoke_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);
            int parentLayoutCallCount = 0;
            owner.Layout += (sender, e) =>
            {
                Assert.Same(owner, sender);
                Assert.Same(child3, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            int controlRemovedCallCount = 0;
            owner.ControlRemoved += (sender, e) => controlRemovedCallCount++;

            collection.Clear();
            Assert.Empty(collection);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);
        }

        [WinFormsFact]
        public void ControlCollection_Clone_Invoke_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            var sourceCollection = new Control.ControlCollection(owner);
            ICloneable iClonable = sourceCollection;
            sourceCollection.Add(child1);
            sourceCollection.Add(child2);
            sourceCollection.Add(child3);
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                parentLayoutCallCount++;
            };
            owner.Layout += parentHandler;

            try
            {
                var collection = Assert.IsType<Control.ControlCollection>(iClonable.Clone());
                Assert.NotSame(sourceCollection, collection);
                Assert.Equal(new Control[] { child1, child2, child3 }, sourceCollection.Cast<Control>());
                Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());
                Assert.False(collection.IsReadOnly);
                Assert.Same(owner, collection.Owner);
                Assert.Same(owner, child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(0, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Contains_Invoke_ReturnsExpected()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            using var child1 = new Control();
            using var child2 = new Control();
            collection.Add(child1);
            collection.Add(child2);

            Assert.True(collection.Contains(child1));
            Assert.True(collection.Contains(child2));
            Assert.False(collection.Contains(new Control()));
            Assert.False(collection.Contains(null));
        }

        [WinFormsTheory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("name1", true)]
        [InlineData("NAME1", true)]
        [InlineData("name2", true)]
        [InlineData("NoSuchName", false)]
        [InlineData("abcd", false)]
        [InlineData("abcde", false)]
        [InlineData("abcdef", false)]
        public void ControlCollection_ContainsKey_Invoke_ReturnsExpected(string key, bool expected)
        {
            using var owner = new Control();
            using var child1 = new Control
            {
                Name = "name1"
            };
            using var child2 = new Control
            {
                Name = "name2"
            };
            using var child3 = new Control
            {
                Name = "name2"
            };
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            Assert.Equal(expected, collection.ContainsKey(key));

            // Call again.
            Assert.Equal(expected, collection.ContainsKey(key));
            Assert.Equal(-1, collection.IndexOfKey("NoSuchKey"));
        }

        [WinFormsTheory]
        [InlineData("name2")]
        [InlineData("NAME2")]
        public void ControlCollection_Find_InvokeKeyExists_ReturnsExpected(string key)
        {
            using var owner = new Control();
            using var child1 = new Control
            {
                Name = "name1"
            };
            using var child2 = new Control
            {
                Name = "name2"
            };
            using var child3 = new Control
            {
                Name = "name2"
            };
            using var grandchild1 = new Control
            {
                Name = "name1"
            };
            using var grandchild2 = new Control
            {
                Name = "name2"
            };
            using var grandchild3 = new Control
            {
                Name = "name2"
            };
            child3.Controls.Add(grandchild1);
            child3.Controls.Add(grandchild2);
            child3.Controls.Add(grandchild3);
            Control.ControlCollection collection = owner.Controls;
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            // Search all children.
            Assert.Equal(new Control[] { child2, child3, grandchild2, grandchild3 }, collection.Find(key, searchAllChildren: true));

            // Call again.
            Assert.Equal(new Control[] { child2, child3, grandchild2, grandchild3 }, collection.Find(key, searchAllChildren: true));

            // Don't search all children.
            Assert.Equal(new Control[] { child2, child3 }, collection.Find(key, searchAllChildren: false));

            // Call again.
            Assert.Equal(new Control[] { child2, child3 }, collection.Find(key, searchAllChildren: false));
        }

        [WinFormsTheory]
        [InlineData("NoSuchName")]
        [InlineData("abcd")]
        [InlineData("abcde")]
        [InlineData("abcdef")]
        public void ControlCollection_Find_InvokeNoSuchKey_ReturnsEmpty(string key)
        {
            using var owner = new Control();
            using var child1 = new Control
            {
                Name = "name1"
            };
            using var child2 = new Control
            {
                Name = "name2"
            };
            using var child3 = new Control
            {
                Name = "name2"
            };
            Control.ControlCollection collection = owner.Controls;
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            Assert.Empty(collection.Find(key, searchAllChildren: true));
            Assert.Empty(collection.Find(key, searchAllChildren: false));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void ControlCollection_Find_NullOrEmptyKey_ThrowsArgumentNullException(string key)
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentNullException>("key", () => collection.Find(key, searchAllChildren: true));
            Assert.Throws<ArgumentNullException>("key", () => collection.Find(key, searchAllChildren: false));
        }

        [WinFormsFact]
        public void ControlCollection_GetChildIndex_InvokeControl_ReturnsExpected()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            using var child1 = new Control();
            using var child2 = new Control();
            collection.Add(child1);
            collection.Add(child2);

            Assert.Equal(0, collection.GetChildIndex(child1));
            Assert.Equal(1, collection.GetChildIndex(child2));
        }

        [WinFormsFact]
        public void ControlCollection_GetChildIndex_InvokeControlBoolThrowException_ReturnsExpected()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            using var child1 = new Control();
            using var child2 = new Control();
            collection.Add(child1);
            collection.Add(child2);

            Assert.Equal(0, collection.GetChildIndex(child1, throwException: true));
            Assert.Equal(1, collection.GetChildIndex(child2, throwException: true));
        }

        [WinFormsFact]
        public void ControlCollection_GetChildIndex_InvokeControlBoolNoSuchControl_ReturnsExpected()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            using var child1 = new Control();
            using var child2 = new Control();
            collection.Add(child1);
            collection.Add(child2);

            Assert.Equal(0, collection.GetChildIndex(child1, throwException: false));
            Assert.Equal(1, collection.GetChildIndex(child2, throwException: false));
            Assert.Equal(-1, collection.GetChildIndex(new Control(), throwException: false));
            Assert.Equal(-1, collection.GetChildIndex(null, throwException: false));
        }

        public static IEnumerable<object[]> GetChildIndex_NoSuchControl_TestData()
        {
            yield return new object[] { new Control() };
            yield return new object[] { null };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetChildIndex_NoSuchControl_TestData))]
        public void ControlCollection_GetChildIndex_NoSuchControl_ThrowsArgumentException(Control child)
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            using var child1 = new Control();
            using var child2 = new Control();
            collection.Add(child1);
            collection.Add(child2);

            Assert.Throws<ArgumentException>(null, () => collection.GetChildIndex(child));
            Assert.Throws<ArgumentException>(null, () => collection.GetChildIndex(child));
        }

        [WinFormsFact]
        public void ControlCollection_GetEnumerator_InvokeEmpty_ReturnsExpected()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            IEnumerator enumerator = collection.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                Assert.Null(enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Null(enumerator.Current);

                // Call again.
                Assert.False(enumerator.MoveNext());
                Assert.Null(enumerator.Current);

                enumerator.Reset();
            }
        }

        [WinFormsFact]
        public void ControlCollection_GetEnumerator_InvokeNotEmpty_ReturnsExpected()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            IEnumerator enumerator = collection.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                Assert.Null(enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(child1, enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(child2, enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(child3, enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Same(child3, enumerator.Current);

                // Call again.
                Assert.False(enumerator.MoveNext());
                Assert.Same(child3, enumerator.Current);

                enumerator.Reset();
            }
        }

        [WinFormsFact]
        public void ControlCollection_GetEnumerator_AddDuringEnumeration_ReturnsExpected()
        {
            using var owner = new Control();
            using var child = new Control();
            var collection = new Control.ControlCollection(owner);
            IEnumerator enumerator = collection.GetEnumerator();
            collection.Add(child);
            for (int i = 0; i < 2; i++)
            {
                Assert.Null(enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Null(enumerator.Current);

                // Call again.
                Assert.False(enumerator.MoveNext());
                Assert.Null(enumerator.Current);

                enumerator.Reset();
            }
        }

        [WinFormsFact]
        public void ControlCollection_GetEnumerator_InvokeRemoveBeforeEnumeration_ReturnsExpected()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            IEnumerator enumerator = collection.GetEnumerator();
            collection.Remove(child1);

            Assert.Null(enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(child2, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(child3, enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Same(child3, enumerator.Current);

            // Call again.
            Assert.False(enumerator.MoveNext());
            Assert.Same(child3, enumerator.Current);
        }

        [WinFormsFact]
        public void ControlCollection_GetEnumerator_InvokeRemoveAtEndOfEnumeration_ReturnsExpected()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            IEnumerator enumerator = collection.GetEnumerator();
            Assert.Null(enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(child1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(child2, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(child3, enumerator.Current);

            collection.Remove(child1);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<ArgumentOutOfRangeException>("index", () => enumerator.Current);

            // Call again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<ArgumentOutOfRangeException>("index", () => enumerator.Current);

            collection.Add(child1);
            Assert.Same(child1, enumerator.Current);
        }

        [WinFormsFact]
        public void ControlCollection_IndexOf_Invoke_ReturnsExpected()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            using var child1 = new Control();
            using var child2 = new Control();
            collection.Add(child1);
            collection.Add(child2);

            Assert.Equal(0, collection.IndexOf(child1));
            Assert.Equal(1, collection.IndexOf(child2));
            Assert.Equal(-1, collection.IndexOf(new Control()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [WinFormsTheory]
        [InlineData(null, -1)]
        [InlineData("", -1)]
        [InlineData("name1", 0)]
        [InlineData("NAME1", 0)]
        [InlineData("name2", 1)]
        [InlineData("NoSuchName", -1)]
        [InlineData("abcd", -1)]
        [InlineData("abcde", -1)]
        [InlineData("abcdef", -1)]
        public void ControlCollection_IndexOfKey_Invoke_ReturnsExpected(string key, int expected)
        {
            using var owner = new Control();
            using var child1 = new Control
            {
                Name = "name1"
            };
            using var child2 = new Control
            {
                Name = "name2"
            };
            using var child3 = new Control
            {
                Name = "name2"
            };
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            Assert.Equal(expected, collection.IndexOfKey(key));

            // Call again.
            Assert.Equal(expected, collection.IndexOfKey(key));
            Assert.Equal(-1, collection.IndexOfKey("NoSuchKey"));
        }

        [WinFormsTheory]
        [InlineData("name1", 0)]
        [InlineData("NAME1", 0)]
        [InlineData("name2", 1)]
        public void ControlCollection_Item_GetStringValidKey_ReturnsExpected(string key, int expectedIndex)
        {
            using var owner = new Control();
            using var child1 = new Control
            {
                Name = "name1"
            };
            using var child2 = new Control
            {
                Name = "name2"
            };
            using var child3 = new Control
            {
                Name = "name2"
            };
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            Assert.Equal(collection[expectedIndex], collection[key]);

            // Call again.
            Assert.Equal(collection[expectedIndex], collection[key]);
            Assert.Null(collection["NoSuchKey"]);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NoSuchName")]
        [InlineData("abcd")]
        [InlineData("abcde")]
        [InlineData("abcdef")]
        public void ControlCollection_Item_GetStringNoSuchKey_ReturnsNull(string key)
        {
            using var owner = new Control();
            using var child1 = new Control
            {
                Name = "name1"
            };
            using var child2 = new Control
            {
                Name = "name2"
            };
            using var child3 = new Control
            {
                Name = "name2"
            };
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            Assert.Null(collection[key]);

            // Call again.
            Assert.Null(collection[key]);
            Assert.Null(collection["NoSuchKey"]);
        }

        [WinFormsFact]
        public void ControlCollection_Item_GetInt_ReturnsExpected()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            Assert.Same(child1, collection[0]);
            Assert.Same(child2, collection[1]);
            Assert.Same(child3, collection[2]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ControlCollection_Item_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ControlCollection_Item_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new Control();
            using var child = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsFact]
        public void ControlCollection_Remove_InvokeChildWithoutHandleOwnerWithoutHandle_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;

            try
            {
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);

                // Remove again.
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);

                // Remove null.
                collection.Remove(null);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Remove_InvokeContainerControlParent_Success()
        {
            using var owner = new ContainerControl();
            using var child1 = new Control();
            using var child2 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            owner.ActiveControl = child1;
            Assert.Same(child1, owner.ActiveControl);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;

            try
            {
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Null(owner.ActiveControl);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);

                // Remove again.
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Null(owner.ActiveControl);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);

                // Remove null.
                collection.Remove(null);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Null(owner.ActiveControl);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Remove_InvokeChildWithHandleOwnerWithoutHandle_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            Control.ControlCollection collection = owner.Controls;
            collection.Add(child1);
            collection.Add(child2);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, child1.Handle);
            int invalidatedCallCount = 0;
            child1.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            child1.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            child1.HandleCreated += (sender, e) => createdCallCount++;

            try
            {
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.False(child2.IsHandleCreated);

                // Remove again.
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.False(child2.IsHandleCreated);

                // Remove null.
                collection.Remove(null);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.False(child2.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Remove_InvokeChildWithHandleOwnerWithHandle_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            Control.ControlCollection collection = owner.Controls;
            collection.Add(child1);
            collection.Add(child2);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            child1.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            child1.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            child1.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(child2.IsHandleCreated);

                // Remove again.
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(child2.IsHandleCreated);

                // Remove null.
                collection.Remove(null);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(child2.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Remove_InvokeChildFromAnotherParent_Nop()
        {
            using var owner1 = new Control();
            using var owner2 = new Control();
            using var control1 = new Control();
            using var control2 = new Control();
            using var control3 = new Control();
            var collection1 = new Control.ControlCollection(owner1);
            var collection2 = new Control.ControlCollection(owner2);
            collection1.Add(control1);
            collection2.Add(control2);

            // Remove with parent.
            collection1.Remove(control2);
            Assert.Equal(new Control[] { control1 }, collection1.Cast<Control>());
            Assert.Equal(new Control[] { control2 }, collection2.Cast<Control>());
            Assert.Same(owner1, control1.Parent);
            Assert.Same(owner2, control2.Parent);

            // Remove without parent.
            collection1.Remove(control3);
            Assert.Equal(new Control[] { control1 }, collection1.Cast<Control>());
            Assert.Equal(new Control[] { control2 }, collection2.Cast<Control>());
            Assert.Same(owner1, control1.Parent);
            Assert.Same(owner2, control2.Parent);
        }

        [WinFormsFact]
        public void ControlCollection_Remove_InvokeWithNonOverridenProperties_DoesNotCallPropertyHandlers()
        {
            using var owner = new Control();
            using var control = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(control);

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            control.ParentChanged += (sender, e) => parentChangedCallCount++;
            control.EnabledChanged += (sender, e) => enabledChangedCallCount++;
            control.VisibleChanged += (sender, e) => visibleChangedCallCount++;
            control.FontChanged += (sender, e) => fontChangedCallCount++;
            control.ForeColorChanged += (sender, e) => foreColorChangedCallCount++;
            control.BackColorChanged += (sender, e) => backColorChangedCallCount++;
            control.RightToLeftChanged += (sender, e) => rightToLeftChangedCallCount++;
            control.BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            collection.Remove(control);
            Assert.Null(control.Parent);
            Assert.Equal(1, parentChangedCallCount);
            Assert.True(control.Enabled);
            Assert.Equal(0, enabledChangedCallCount);
            Assert.True(control.Visible);
            Assert.Equal(0, visibleChangedCallCount);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(0, fontChangedCallCount);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(0, rightToLeftChangedCallCount);
            Assert.Null(control.BindingContext);
            Assert.Equal(0, bindingContextChangedCallCount);
        }

        [WinFormsFact]
        public void ControlCollection_Remove_InvokeWithNonOverridenPropertiesWithHandle_DoesNotCallPropertyHandlers()
        {
            using var owner = new Control();
            using var control = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(control);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            control.ParentChanged += (sender, e) => parentChangedCallCount++;
            control.EnabledChanged += (sender, e) => enabledChangedCallCount++;
            control.VisibleChanged += (sender, e) => visibleChangedCallCount++;
            control.FontChanged += (sender, e) => fontChangedCallCount++;
            control.ForeColorChanged += (sender, e) => foreColorChangedCallCount++;
            control.BackColorChanged += (sender, e) => backColorChangedCallCount++;
            control.RightToLeftChanged += (sender, e) => rightToLeftChangedCallCount++;
            control.BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            collection.Remove(control);
            Assert.Null(control.Parent);
            Assert.Equal(1, parentChangedCallCount);
            Assert.True(control.Enabled);
            Assert.Equal(0, enabledChangedCallCount);
            Assert.True(control.Visible);
            Assert.Equal(2, visibleChangedCallCount);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(0, fontChangedCallCount);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(0, rightToLeftChangedCallCount);
            Assert.Null(control.BindingContext);
            Assert.Equal(1, bindingContextChangedCallCount);
        }

        public static IEnumerable<object[]> Remove_OverridenProperties_TestData()
        {
            var parentContext = new BindingContext();
            var childContext = new BindingContext();
            yield return new object[] { parentContext, childContext, childContext };
            yield return new object[] { parentContext, null, parentContext };
            yield return new object[] { null, childContext, childContext };
            yield return new object[] { null, null, null };
        }

        [WinFormsTheory]
        [MemberData(nameof(Remove_OverridenProperties_TestData))]
        public void ControlCollection_Remove_InvokeWithOverridenProperties_CallsPropertyHandlers(BindingContext parentBindingContext, BindingContext bindingContext, BindingContext expectedBindingContext)
        {
            using var owner = new Control();
            using var control = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(control);

            owner.Enabled = false;
            Assert.False(control.Enabled);
            owner.Visible = false;
            Assert.False(control.Visible);
            var font = new Font("Arial", 8.25f);
            owner.Font = font;
            Assert.Same(font, control.Font);
            owner.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            owner.BackColor = Color.Blue;
            Assert.Equal(Color.Blue, control.BackColor);
            owner.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            owner.BindingContext = parentBindingContext;
            control.BindingContext = bindingContext;
            Assert.Same(expectedBindingContext, control.BindingContext);

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            control.ParentChanged += (sender, e) =>
            {
                Assert.Equal(enabledChangedCallCount, parentChangedCallCount);
                Assert.Equal(visibleChangedCallCount, parentChangedCallCount);
                Assert.Equal(fontChangedCallCount, parentChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, parentChangedCallCount);
                Assert.Equal(backColorChangedCallCount, parentChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, parentChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, parentChangedCallCount);
                parentChangedCallCount++;
            };
            control.EnabledChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, enabledChangedCallCount);
                Assert.Equal(visibleChangedCallCount, enabledChangedCallCount);
                Assert.Equal(fontChangedCallCount, enabledChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, enabledChangedCallCount);
                Assert.Equal(backColorChangedCallCount, enabledChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, enabledChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, enabledChangedCallCount);
                enabledChangedCallCount++;
            };
            control.VisibleChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, visibleChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, visibleChangedCallCount);
                Assert.Equal(fontChangedCallCount, visibleChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, visibleChangedCallCount);
                Assert.Equal(backColorChangedCallCount, visibleChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, visibleChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, visibleChangedCallCount);
                visibleChangedCallCount++;
            };
            control.FontChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(visibleChangedCallCount, fontChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, fontChangedCallCount);
                Assert.Equal(backColorChangedCallCount, fontChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, fontChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, fontChangedCallCount);
                fontChangedCallCount++;
            };
            control.ForeColorChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(visibleChangedCallCount, foreColorChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(backColorChangedCallCount, foreColorChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, foreColorChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, foreColorChangedCallCount);
                foreColorChangedCallCount++;
            };
            control.BackColorChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(visibleChangedCallCount, backColorChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(foreColorChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, backColorChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, backColorChangedCallCount);
                backColorChangedCallCount++;
            };
            control.RightToLeftChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(visibleChangedCallCount, rightToLeftChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(foreColorChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(backColorChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(bindingContextChangedCallCount, rightToLeftChangedCallCount);
                rightToLeftChangedCallCount++;
            };
            control.BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            collection.Remove(control);
            Assert.Null(control.Parent);
            Assert.Equal(1, parentChangedCallCount);
            Assert.True(control.Enabled);
            Assert.Equal(1, enabledChangedCallCount);
            Assert.True(control.Visible);
            Assert.Equal(0, visibleChangedCallCount);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(1, fontChangedCallCount);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(1, foreColorChangedCallCount);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(1, backColorChangedCallCount);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(1, rightToLeftChangedCallCount);
            Assert.Same(bindingContext, control.BindingContext);
            Assert.Equal(0, bindingContextChangedCallCount);
        }

        public static IEnumerable<object[]> Remove_OverridenPropertiesWithHandle_TestData()
        {
            var parentContext = new BindingContext();
            var childContext = new BindingContext();
            yield return new object[] { parentContext, childContext, childContext, 0 };
            yield return new object[] { parentContext, null, parentContext, 1 };
            yield return new object[] { null, childContext, childContext, 0 };
            yield return new object[] { null, null, null, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Remove_OverridenPropertiesWithHandle_TestData))]
        public void ControlCollection_Remove_InvokeWithOverridenPropertiesWithHandle_CallsPropertyHandlers(BindingContext parentBindingContext, BindingContext bindingContext, BindingContext expectedBindingContext, int expectedBindingContextChangedCallCount)
        {
            using var owner = new Control();
            using var control = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(control);

            owner.Enabled = false;
            Assert.False(control.Enabled);
            owner.Visible = false;
            Assert.False(control.Visible);
            var font = new Font("Arial", 8.25f);
            owner.Font = font;
            Assert.Same(font, control.Font);
            owner.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            owner.BackColor = Color.Blue;
            Assert.Equal(Color.Blue, control.BackColor);
            owner.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            owner.BindingContext = parentBindingContext;
            control.BindingContext = bindingContext;
            Assert.Same(expectedBindingContext, control.BindingContext);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            void parentChangedHandler(object sender, EventArgs e)
            {
                Assert.Equal(enabledChangedCallCount, parentChangedCallCount);
                Assert.Equal(fontChangedCallCount, parentChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, parentChangedCallCount);
                Assert.Equal(backColorChangedCallCount, parentChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, parentChangedCallCount);
                parentChangedCallCount++;
            }
            control.ParentChanged += parentChangedHandler;
            control.EnabledChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, enabledChangedCallCount);
                Assert.Equal(fontChangedCallCount, enabledChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, enabledChangedCallCount);
                Assert.Equal(backColorChangedCallCount, enabledChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, enabledChangedCallCount);
                enabledChangedCallCount++;
            };
            control.VisibleChanged += (sender, e) => visibleChangedCallCount++;
            control.FontChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, fontChangedCallCount);
                Assert.Equal(foreColorChangedCallCount, fontChangedCallCount);
                Assert.Equal(backColorChangedCallCount, fontChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, fontChangedCallCount);
                fontChangedCallCount++;
            };
            control.ForeColorChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, foreColorChangedCallCount);
                Assert.Equal(backColorChangedCallCount, foreColorChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, foreColorChangedCallCount);
                foreColorChangedCallCount++;
            };
            control.BackColorChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(foreColorChangedCallCount - 1, backColorChangedCallCount);
                Assert.Equal(rightToLeftChangedCallCount, backColorChangedCallCount);
                backColorChangedCallCount++;
            };
            control.RightToLeftChanged += (sender, e) =>
            {
                Assert.Equal(parentChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(enabledChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(fontChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(foreColorChangedCallCount - 1, rightToLeftChangedCallCount);
                Assert.Equal(backColorChangedCallCount - 1, rightToLeftChangedCallCount);
                rightToLeftChangedCallCount++;
            };
            control.BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            try
            {
                collection.Remove(control);
                Assert.Null(control.Parent);
                Assert.Equal(1, parentChangedCallCount);
                Assert.True(control.Enabled);
                Assert.Equal(1, enabledChangedCallCount);
                Assert.True(control.Visible);
                Assert.Equal(2, visibleChangedCallCount);
                Assert.Equal(Control.DefaultFont, control.Font);
                Assert.Equal(1, fontChangedCallCount);
                Assert.Equal(Control.DefaultForeColor, control.ForeColor);
                Assert.Equal(1, foreColorChangedCallCount);
                Assert.Equal(Control.DefaultBackColor, control.BackColor);
                Assert.Equal(1, backColorChangedCallCount);
                Assert.Equal(RightToLeft.No, control.RightToLeft);
                Assert.Equal(1, rightToLeftChangedCallCount);
                Assert.Same(bindingContext, control.BindingContext);
                Assert.Equal(expectedBindingContextChangedCallCount * 2, bindingContextChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(4, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(1, createdCallCount);
            }
            finally
            {
                control.ParentChanged -= parentChangedHandler;
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Remove_OverridenProperties_TestData))]
        public void ControlCollection_Remove_InvokeWithOverridenPropertiesAxHost_DoesNotCallPropertyHandlers(BindingContext parentBindingContext, BindingContext bindingContext, BindingContext expectedBindingContext)
        {
            using var owner = new Control();
            using var control = new SubAxHost("8856f961-340a-11d0-a96b-00c04fd705a2");
            var collection = new Control.ControlCollection(owner);
            collection.Add(control);

            owner.Enabled = false;
            Assert.False(control.Enabled);
            owner.Visible = false;
            Assert.False(control.Visible);
            var font = new Font("Arial", 8.25f);
            owner.Font = font;
            Assert.Same(font, control.Font);
            owner.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            owner.BackColor = Color.Blue;
            Assert.Equal(Color.Blue, control.BackColor);
            owner.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, ((Control)control).RightToLeft);
            owner.BindingContext = parentBindingContext;
            control.BindingContext = bindingContext;
            Assert.Same(expectedBindingContext, control.BindingContext);

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            ((Control)control).ParentChanged += (sender, e) => parentChangedCallCount++;
            ((Control)control).EnabledChanged += (sender, e) => enabledChangedCallCount++;
            ((Control)control).VisibleChanged += (sender, e) => visibleChangedCallCount++;
            ((Control)control).FontChanged += (sender, e) => fontChangedCallCount++;
            ((Control)control).ForeColorChanged += (sender, e) => foreColorChangedCallCount++;
            ((Control)control).BackColorChanged += (sender, e) => backColorChangedCallCount++;
            ((Control)control).RightToLeftChanged += (sender, e) => rightToLeftChangedCallCount++;
            ((Control)control).BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            collection.Remove(control);
            Assert.Null(control.Parent);
            Assert.Equal(1, parentChangedCallCount);
            Assert.True(control.Enabled);
            Assert.Equal(0, enabledChangedCallCount);
            Assert.True(control.Visible);
            Assert.Equal(0, visibleChangedCallCount);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(0, fontChangedCallCount);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(RightToLeft.No, ((Control)control).RightToLeft);
            Assert.Equal(0, rightToLeftChangedCallCount);
            Assert.Same(bindingContext, control.BindingContext);
            Assert.Equal(0, bindingContextChangedCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Remove_OverridenPropertiesWithHandle_TestData))]
        public void ControlCollection_Remove_InvokeWithOverridenPropertiesAxHostWithHandle_CallSPropertyHandlers(BindingContext parentBindingContext, BindingContext bindingContext, BindingContext expectedBindingContext, int expectedBindingContextChangedCallCount)
        {
            using var owner = new Control();
            using var control = new SubAxHost("8856f961-340a-11d0-a96b-00c04fd705a2");
            var collection = new Control.ControlCollection(owner);
            collection.Add(control);

            owner.Enabled = false;
            Assert.False(control.Enabled);
            owner.Visible = false;
            Assert.False(control.Visible);
            var font = new Font("Arial", 8.25f);
            owner.Font = font;
            Assert.Same(font, control.Font);
            owner.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            owner.BackColor = Color.Blue;
            Assert.Equal(Color.Blue, control.BackColor);
            owner.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, ((Control)control).RightToLeft);
            owner.BindingContext = parentBindingContext;
            control.BindingContext = bindingContext;
            Assert.Same(expectedBindingContext, control.BindingContext);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int parentChangedCallCount = 0;
            int enabledChangedCallCount = 0;
            int visibleChangedCallCount = 0;
            int fontChangedCallCount = 0;
            int foreColorChangedCallCount = 0;
            int backColorChangedCallCount = 0;
            int rightToLeftChangedCallCount = 0;
            int bindingContextChangedCallCount = 0;
            void parentChangedHandler(object sender, EventArgs e)
            {
                parentChangedCallCount++;
            }
            ((Control)control).ParentChanged += parentChangedHandler;
            ((Control)control).EnabledChanged += (sender, e) => enabledChangedCallCount++;
            ((Control)control).VisibleChanged += (sender, e) => visibleChangedCallCount++;
            ((Control)control).FontChanged += (sender, e) => fontChangedCallCount++;
            ((Control)control).ForeColorChanged += (sender, e) => foreColorChangedCallCount++;
            ((Control)control).BackColorChanged += (sender, e) => backColorChangedCallCount++;
            ((Control)control).RightToLeftChanged += (sender, e) => rightToLeftChangedCallCount++;
            ((Control)control).BindingContextChanged += (sender, e) => bindingContextChangedCallCount++;

            try
            {
                collection.Remove(control);
                Assert.Null(control.Parent);
                Assert.Equal(1, parentChangedCallCount);
                Assert.True(control.Enabled);
                Assert.Equal(1, enabledChangedCallCount);
                Assert.True(control.Visible);
                Assert.Equal(2, visibleChangedCallCount);
                Assert.Equal(Control.DefaultFont, control.Font);
                Assert.Equal(1, fontChangedCallCount);
                Assert.Equal(Control.DefaultForeColor, control.ForeColor);
                Assert.Equal(1, foreColorChangedCallCount);
                Assert.Equal(Control.DefaultBackColor, control.BackColor);
                Assert.Equal(1, backColorChangedCallCount);
                Assert.Equal(RightToLeft.No, ((Control)control).RightToLeft);
                Assert.Equal(1, rightToLeftChangedCallCount);
                Assert.Same(bindingContext, control.BindingContext);
                Assert.Equal(expectedBindingContextChangedCallCount, bindingContextChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(7, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(1, createdCallCount);
            }
            finally
            {
                control.ParentChanged -= parentChangedHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Remove_InvokeWithHandler_CallsControlRemoved()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);

            int parentChangedCallCount = 0;
            int callCount = 0;
            int parentLayoutCallCount = 0;
            child1.ParentChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(callCount, parentChangedCallCount);
                Assert.Equal(parentLayoutCallCount, parentChangedCallCount);
                parentChangedCallCount++;
            };
            child2.ParentChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(callCount, parentChangedCallCount);
                Assert.Equal(parentLayoutCallCount, parentChangedCallCount);
                parentChangedCallCount++;
            };
            object affectedControl = null;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(affectedControl, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                Assert.Equal(parentChangedCallCount - 1, parentLayoutCallCount);
                Assert.Equal(callCount, parentLayoutCallCount);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            void handler(object sender, ControlEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.Control);
                Assert.Equal(parentChangedCallCount - 1, callCount);
                Assert.Equal(parentLayoutCallCount - 1, callCount);
                callCount++;
            }

            // Call with handler.
            owner.ControlRemoved += handler;
            affectedControl = child1;
            collection.Remove(child1);
            Assert.Null(child1.Parent);
            Assert.Equal(1, parentChangedCallCount);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.Equal(1, callCount);

            // Call again.
            collection.Remove(child1);
            Assert.Null(child1.Parent);
            Assert.Equal(1, parentChangedCallCount);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.Equal(1, callCount);

            // Remove handler.
            owner.ControlRemoved -= handler;
            affectedControl = child2;
            collection.Remove(child2);
            Assert.Null(child2.Parent);
            Assert.Equal(2, parentChangedCallCount);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ControlCollection_RemoveAt_Invoke_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            object affectedControl = null;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(affectedControl, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;

            try
            {
                affectedControl = child2;
                collection.RemoveAt(1);
                Assert.Equal(new Control[] { child1 }, collection.Cast<Control>());
                Assert.Same(owner, child1.Parent);
                Assert.Null(child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);

                // Remove again.
                affectedControl = child1;
                collection.RemoveAt(0);
                Assert.Empty(collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Null(child2.Parent);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ControlCollection_RemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ControlCollection_RemoveAtInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new Control();
            using var child = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [WinFormsTheory]
        [InlineData("name2")]
        [InlineData("NAME2")]
        public void ControlCollection_RemoveByKey_InvokeValidKey_ReturnsExpected(string key)
        {
            using var owner = new Control();
            using var child1 = new Control
            {
                Name = "name1"
            };
            using var child2 = new Control
            {
                Name = "name2"
            };
            using var child3 = new Control
            {
                Name = "name2"
            };
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            int layoutCallCount = 0;
            child2.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            object affectedControl = null;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(affectedControl, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;

            try
            {
                affectedControl = child2;
                collection.RemoveByKey(key);
                Assert.Equal(new Control[] { child1, child3 }, collection.Cast<Control>());
                Assert.Same(owner, child1.Parent);
                Assert.Null(child2.Parent);
                Assert.Same(owner, child3.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                // Remove again.
                affectedControl = child3;
                collection.RemoveByKey(key);
                Assert.Equal(new Control[] { child1 }, collection.Cast<Control>());
                Assert.Same(owner, child1.Parent);
                Assert.Null(child2.Parent);
                Assert.Null(child3.Parent);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NoSuchName")]
        [InlineData("abcd")]
        [InlineData("abcde")]
        [InlineData("abcdef")]
        public void ControlCollection_RemoveByKey_InvokeNoSuchKey_ReturnsNull(string key)
        {
            using var owner = new Control();
            using var child1 = new Control
            {
                Name = "name1"
            };
            using var child2 = new Control
            {
                Name = "name2"
            };
            using var child3 = new Control
            {
                Name = "name2"
            };
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            collection.RemoveByKey(key);
            Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());

            // Call again.
            collection.RemoveByKey(key);
            Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());
        }

        [WinFormsFact]
        public void ControlCollection_SetChildIndex_InvokeChildWithoutHandleOwnerWithoutHandle_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("ChildIndex", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;

            try
            {
                // Set middle.
                collection.SetChildIndex(child1, 1);
                Assert.Equal(new Control[] { child2, child1, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                // Set same.
                collection.SetChildIndex(child1, 1);
                Assert.Equal(new Control[] { child2, child1, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                // Set start.
                collection.SetChildIndex(child1, 0);
                Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        [InlineData(4, 2)]
        public void ControlCollection_SetChildIndex_InvokeLargeIndex_Success(int index, int expectedParentLayoutCallCount)
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("ChildIndex", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;

            try
            {
                // Set middle.
                collection.SetChildIndex(child1, index);
                Assert.Equal(new Control[] { child2, child3, child1 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                // Set same.
                collection.SetChildIndex(child1, index);
                Assert.Equal(new Control[] { child2, child3, child1 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_SetChildIndex_InvokeChildWithHandleOwnerWithoutHandle_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            Control.ControlCollection collection = owner.Controls;
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("ChildIndex", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, child1.Handle);
            int invalidatedCallCount = 0;
            child1.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            child1.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            child1.HandleCreated += (sender, e) => createdCallCount++;

            try
            {
                // Set middle.
                collection.SetChildIndex(child1, 1);
                Assert.Equal(new Control[] { child2, child1, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                // Set same.
                collection.SetChildIndex(child1, 1);
                Assert.Equal(new Control[] { child2, child1, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                // Set start.
                collection.SetChildIndex(child1, 0);
                Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_SetChildIndex_InvokeChildWithHandleOwnerWithHandle_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var child3 = new Control();
            Control.ControlCollection collection = owner.Controls;
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("ChildIndex", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            child1.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            child1.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            child1.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                // Set middle.
                collection.SetChildIndex(child1, 1);
                Assert.Equal(new Control[] { child2, child1, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(child2.IsHandleCreated);
                Assert.True(child3.IsHandleCreated);

                // Set same.
                collection.SetChildIndex(child1, 1);
                Assert.Equal(new Control[] { child2, child1, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(child2.IsHandleCreated);
                Assert.True(child3.IsHandleCreated);

                // Set start.
                collection.SetChildIndex(child1, 0);
                Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.True(child1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(child2.IsHandleCreated);
                Assert.True(child3.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_SetChildIndex_InvokeChildWithoutHandleOwnerWithHandle_Success()
        {
            using var owner = new Control();
            using var child1 = new SubControl();
            using var child2 = new Control();
            using var child3 = new Control();
            Control.ControlCollection collection = owner.Controls;
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("ChildIndex", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int parentInvalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                // Set middle.
                child1.DestroyHandle();
                collection.SetChildIndex(child1, 1);
                Assert.Equal(new Control[] { child2, child1, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.False(child1.IsHandleCreated);
                Assert.True(child2.IsHandleCreated);
                Assert.True(child3.IsHandleCreated);

                // Set same.
                collection.SetChildIndex(child1, 1);
                Assert.Equal(new Control[] { child2, child1, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.False(child1.IsHandleCreated);
                Assert.True(child2.IsHandleCreated);
                Assert.True(child3.IsHandleCreated);

                // Set start.
                collection.SetChildIndex(child1, 0);
                Assert.Equal(new Control[] { child1, child2, child3 }, collection.Cast<Control>());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.False(child1.IsHandleCreated);
                Assert.True(child2.IsHandleCreated);
                Assert.True(child3.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_SetChildIndex_NullChild_ThrowsArgumentNullException()
        {
            using var owner = new Control();
            var collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentNullException>("child", () => collection.SetChildIndex(null, 0));
        }

        [WinFormsFact]
        public void ControlCollection_SetChildIndex_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            using var owner = new Control();
            using var child = new Control();
            var collection = new Control.ControlCollection(owner);
            collection.Add(child);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.SetChildIndex(child, -2));
        }

        [WinFormsFact]
        public void ControlCollection_IListAdd_Invoke_Success()
        {
            using var owner = new Control();
            using var control1 = new Control();
            using var control2 = new Control();
            IList collection = owner.Controls;
            int parentLayoutCallCount = 0;
            string affectedProperty = null;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(collection.Cast<Control>().Last(), e.AffectedControl);
                Assert.Equal(affectedProperty, e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            int layoutCallCount1 = 0;
            control1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            control2.Layout += (sender, e) => layoutCallCount2++;

            try
            {
                affectedProperty = "Parent";
                Assert.Equal(0, collection.Add(control1));
                Assert.Same(control1, Assert.Single(collection));
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(control1, Assert.Single(owner.Controls));
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);

                // Add another.
                affectedProperty = "Parent";
                Assert.Equal(1, collection.Add(control2));
                Assert.Equal(new Control[] { control1, control2 }, collection.Cast<Control>());
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(owner, control2.Parent);
                Assert.Equal(1, control2.TabIndex);
                Assert.Equal(new Control[] { control1, control2 }, owner.Controls.Cast<Control>());
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);
                Assert.False(control2.IsHandleCreated);

                // Add existing.
                affectedProperty = "ChildIndex";
                Assert.Equal(1, collection.Add(control1));
                Assert.Equal(new Control[] { control2, control1 }, collection.Cast<Control>());
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(owner, control2.Parent);
                Assert.Equal(1, control2.TabIndex);
                Assert.Equal(new Control[] { control2, control1}, owner.Controls.Cast<Control>());
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(3, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);
                Assert.False(control2.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_IListAdd_NotControl_ThrowsArgumentException()
        {
            using var owner = new Control();
            IList collection = new Control.ControlCollection(owner);
            Assert.Throws<ArgumentException>("control", () => collection.Add(new object()));
            Assert.Throws<ArgumentException>("control", () => collection.Add(null));
        }

        [WinFormsFact]
        public void ControlCollection_IListRemove_Invoke_Success()
        {
            using var owner = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            IList collection = new Control.ControlCollection(owner);
            collection.Add(child1);
            collection.Add(child2);

            int layoutCallCount = 0;
            child1.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(child1, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;

            try
            {
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);

                // Remove again.
                collection.Remove(child1);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);

                // Remove null.
                collection.Remove(null);
                Assert.Equal(new Control[] { child2 }, collection.Cast<Control>());
                Assert.Null(child1.Parent);
                Assert.Same(owner, child2.Parent);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_IListRemove_NotControl_ThrowsArgumentException()
        {
            using var owner = new Control();
            IList collection = new Control.ControlCollection(owner);
            collection.Remove(new object());
            collection.Remove(null);
        }

        private class CustomLayoutEngineControl : Control
        {
            private LayoutEngine _layoutEngine;

            public CustomLayoutEngineControl()
            {
                _layoutEngine = new Control().LayoutEngine;
            }

            public void SetLayoutEngine(LayoutEngine layoutEngine)
            {
                _layoutEngine = layoutEngine;
            }

            public override LayoutEngine LayoutEngine => _layoutEngine;

            public new void InitLayout() => base.InitLayout();
        }

        private class SubControl : Control
        {
            public new void DestroyHandle() => base.DestroyHandle();

            public new void SetTopLevel(bool value) => base.SetTopLevel(value);
        }
    }
}
