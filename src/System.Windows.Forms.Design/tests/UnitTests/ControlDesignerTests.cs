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
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.Null(controlDesigner.GetAccessibleObjectField());
        }

        [Fact]
        public void BehaviorServiceProperty()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.Null(controlDesigner.GetBehaviorServiceProperty());
        }

        [Fact]
        public void AccessibilityObjectField()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.NotNull(controlDesigner.AccessibilityObject);
        }

        [Fact]
        public void EnableDragRectProperty()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.False(controlDesigner.GetEnableDragRectProperty());
        }
        
        [Fact]
        public void ParticipatesWithSnapLinesProperty()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.True(controlDesigner.ParticipatesWithSnapLines);
        }

        [Fact]
        public void AutoResizeHandlesProperty()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.True(controlDesigner.AutoResizeHandles = true);
            Assert.True(controlDesigner.AutoResizeHandles);
        }

        [Fact]
        public void SelectionRulesProperty()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.Equal(SelectionRules.Visible ,controlDesigner.SelectionRules);
        }

        [Fact]
        public void InheritanceAttributeProperty()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            Assert.NotNull(controlDesigner.GetInheritanceAttributeProperty());
        }

        [Fact]
        public void NumberOfInternalControlDesignersTest()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.Equal(0, controlDesigner.NumberOfInternalControlDesigners());
        }

        [Fact]
        public void BaseWndProcTest()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
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
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            Assert.True(controlDesigner.CanBeParentedTo(new ParentControlDesigner()));
        }

        public static TheoryData BoolData => CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(BoolData))]
        public void EnableDragDropTest(bool val)
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
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
            MockControlDesigner controlDesigner = new MockControlDesigner();
            Assert.False(controlDesigner.GetHitTestMethod(new Drawing.Point()));
        }

        [Fact]
        public void HookChildControlsTest()
        {
            MockControlDesigner controlDesigner = new MockControlDesigner();
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
            MockControlDesigner controlDesigner = new MockControlDesigner();
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
            MockControlDesigner controlDesigner = new MockControlDesigner();
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
            MockControlDesigner controlDesigner = new MockControlDesigner();
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
            MockControlDesigner controlDesigner = new MockControlDesigner();
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
            MockControlDesigner controlDesigner = new MockControlDesigner();
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
