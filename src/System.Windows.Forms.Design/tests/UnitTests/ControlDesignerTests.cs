// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.TestUtilities;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Design.Tests
{
    public class ControlDesignerTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void AccessibleObjectField()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.Null(controlDesigner.GetAccessibleObjectField());
        }

        [Fact]
        public void BehaviorServiceProperty()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.Null(controlDesigner.GetBehaviorServiceProperty());
        }

        [Fact]
        public void AccessibilityObjectField()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.NotNull(controlDesigner.AccessibilityObject);
        }

        [Fact]
        public void EnableDragRectProperty()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.False(controlDesigner.GetEnableDragRectProperty());
        }

        [Fact]
        public void ParticipatesWithSnapLinesProperty()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.True(controlDesigner.ParticipatesWithSnapLines);
        }

        [Fact]
        public void AutoResizeHandlesProperty()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.True(controlDesigner.AutoResizeHandles = true);
            Assert.True(controlDesigner.AutoResizeHandles);
        }

        [Fact]
        public void SelectionRulesProperty()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.Equal(SelectionRules.Visible, controlDesigner.SelectionRules);
        }

        [Fact]
        public void InheritanceAttributeProperty()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            using Button button = new Button();
            controlDesigner.Initialize(button);
            Assert.NotNull(controlDesigner.GetInheritanceAttributeProperty());
        }

        [Fact]
        public void NumberOfInternalControlDesignersTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.Equal(0, controlDesigner.NumberOfInternalControlDesigners());
        }

        [Fact]
        public void BaseWndProcTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Message m = default;
            controlDesigner.BaseWndProcMethod(ref m);
        }

        [Fact]
        public void CanBeParentedToTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            using Button button = new Button();
            controlDesigner.Initialize(button);
            Assert.True(controlDesigner.CanBeParentedTo(new ParentControlDesigner()));
        }

        public static TheoryData BoolData => CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(BoolData))]
        public void EnableDragDropTest(bool val)
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            controlDesigner.EnableDragDropMethod(val);
        }

        [Fact]
        public void GetHitTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            Assert.False(controlDesigner.GetHitTestMethod(new Drawing.Point()));
        }

        [Fact]
        public void HookChildControlsTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            using Button button = new Button();
            controlDesigner.Initialize(button);
            controlDesigner.HookChildControlsMethod(new Control());
        }

        [Fact]
        public void InitializeTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            using Button button = new Button();
            controlDesigner.Initialize(button);
        }

        [Fact]
        public void InitializeNewComponentTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            using Button button = new Button();
            controlDesigner.Initialize(button);
        }

        [Fact]
        public void OnSetComponentDefaultsTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            using Button button = new Button();
            controlDesigner.Initialize(button);
#pragma warning disable CS0618 // Type or member is obsolete
            controlDesigner.OnSetComponentDefaults();
#pragma warning restore CS0618
        }

        [Fact]
        public void OnContextMenuTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            controlDesigner.OnContextMenuMethod(0, 0);
        }

        [Fact]
        public void OnCreateHandleTest()
        {
            using TestControlDesigner controlDesigner = new TestControlDesigner();
            using Button button = new Button();
            controlDesigner.Initialize(button);
            controlDesigner.OnCreateHandleMethod();
        }

        [WinFormsFact]
        public void ControlDesigner_WndProc_InvokePaint_Success()
        {
            using var designer = new ControlDesigner();
            Message m = new Message
            {
                Msg = (int)User32.WM.PAINT
            };
            designer.TestAccessor().Dynamic.WndProc(ref m);
        }
    }
}
