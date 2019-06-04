// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class ControlDesignerTests
    {
        [Fact]
        public void AccessibleObjectField()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.Null(controlDesigner.GetAccessibleObjectField());
        }

        [Fact]
        public void BehaviorServiceProperty()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.Null(controlDesigner.GetBehaviorServiceProperty());
        }

        [Fact]
        public void AccessibilityObjectField()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.NotNull(controlDesigner.AccessibilityObject);
        }

        [Fact]
        public void EnableDragRectProperty()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.False(controlDesigner.GetEnableDragRectProperty());
        }

        [Fact]
        public void ParticipatesWithSnapLinesProperty()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.True(controlDesigner.ParticipatesWithSnapLines);
        }

        [Fact]
        public void AutoResizeHandlesProperty()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.True(controlDesigner.AutoResizeHandles = true);
            Assert.True(controlDesigner.AutoResizeHandles);
        }

        [Fact]
        public void SelectionRulesProperty()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.Equal(SelectionRules.Visible, controlDesigner.SelectionRules);
        }

        [Fact]
        public void InheritanceAttributeProperty()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            Assert.NotNull(controlDesigner.GetInheritanceAttributeProperty());
        }

        [Fact]
        public void NumberOfInternalControlDesignersTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.Equal(0, controlDesigner.NumberOfInternalControlDesigners());
        }

        [Fact]
        public void BaseWndProcTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Message m = default;
            try
            {
                controlDesigner.BaseWndProcMethod(ref m);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void CanBeParentedToTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            Assert.True(controlDesigner.CanBeParentedTo(new ParentControlDesigner()));
        }

        public static TheoryData BoolData => CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(BoolData))]
        public void EnableDragDropTest(bool val)
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            try
            {
                controlDesigner.EnableDragDropMethod(val);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void GetHitTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.False(controlDesigner.GetHitTestMethod(new Drawing.Point()));
        }

        [Fact]
        public void HookChildControlsTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            try
            {

                controlDesigner.HookChildControlsMethod(new Control());
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void InitializeTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            try
            {
                controlDesigner.Initialize(new Button());
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void InitializeNewComponentTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            try
            {
                controlDesigner.InitializeNewComponent(null);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void OnSetComponentDefaultsTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
#pragma warning disable 618
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            try
            {
                controlDesigner.OnSetComponentDefaults();
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
#pragma warning restore 618
        }

        [Fact]
        public void OnContextMenuTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            try
            {
                controlDesigner.OnContextMenuMethod(0, 0);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void OnCreateHandleTest()
        {
            TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            try
            {
                controlDesigner.OnCreateHandleMethod();
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }
    }
}
