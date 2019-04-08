// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class ControlDesignerTests
    {
        internal MockControlDesigner controlDesigner = new MockControlDesigner();

        [Fact]
        public void AccessibleObjectField()
        {
            Assert.Null(controlDesigner.GetAccessibleObjectField());
        }

        [Fact]
        public void BehaviorServiceProperty()
        {
            Assert.NotNull(controlDesigner.GetBehaviorServiceProperty());
        }

        [Fact]
        public void AssociatedComponentsProperty()
        {
            Assert.NotNull(controlDesigner.AssociatedComponents);
        }

        [Fact]
        public void AccessibilityObjectField()
        {
            Assert.NotNull(controlDesigner.AccessibilityObject);
        }

        [Fact]
        public void ControlProperty()
        {
            Assert.NotNull(controlDesigner.Control);
        }

        [Fact]
        public void EnableDragRectProperty()
        {
            Assert.False(controlDesigner.GetEnableDragRectProperty());
        }

        [Fact]
        public void ParentComponentProperty()
        {
            Assert.NotNull(controlDesigner.GetParentComponentProperty());
        }
        
        [Fact]
        public void ParticipatesWithSnapLinesProperty()
        {
            Assert.True(controlDesigner.ParticipatesWithSnapLines);
        }

        [Fact]
        public void AutoResizeHandlesProperty()
        {
            Assert.True(controlDesigner.AutoResizeHandles = true);
            Assert.True(controlDesigner.AutoResizeHandles);
        }

        [Fact]
        public void SelectionRulesProperty()
        {
            Assert.Equal(SelectionRules.Visible ,controlDesigner.SelectionRules);
        }

        [Fact]
        public void InheritanceAttributeProperty()
        {
            Assert.NotNull(controlDesigner.GetInheritanceAttributeProperty());
        }

        [Fact]
        public void NumberOfInternalControlDesignersTest()
        {
            Assert.Equal(0, controlDesigner.NumberOfInternalControlDesigners());
        }

        [Fact]
        public void InternalControlDesignerTest()
        {
            Assert.NotNull(controlDesigner.InternalControlDesigner(1));
        }

        [Fact]
        public void BaseWndProcTest()
        {
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
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            Assert.True(controlDesigner.CanBeParentedTo(new ParentControlDesigner()));
        }

        [Fact]
        public void DefWndProcTest()
        {
            Message m = default;
            try
            {
                controlDesigner.DefWndProcMethod(ref m);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DisplayErrorTest()
        {
            try
            {
                controlDesigner.DisplayErrorMethod(new Exception());
                controlDesigner.InitializeExistingComponent(null);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DisposeTest()
        {
            try
            {
                controlDesigner.DisposeMethod(true);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void EnableDesignModeTest()
        {
            Assert.True(controlDesigner.EnableDesignModeMethod(null, "fake"));
        }

        public static TheoryData BoolData => CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(BoolData))]
        public void EnableDragDropTest(bool val)
        {
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
            Assert.False(controlDesigner.GetHitTestMethod(new Drawing.Point()));
        }

        [Fact]
        public void HookChildControlsTest()
        {
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
            try
            {
                controlDesigner.Initialize(null);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void InitializeExistingComponentTest()
        {
            Assert.NotNull(controlDesigner);
            controlDesigner.Initialize(new Button());
            try
            {
                controlDesigner.InitializeExistingComponent(null);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void InitializeNewComponentTest()
        {
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
