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
            Assert.Throws<NotImplementedException>(() => controlDesigner.GetBehaviorServiceProperty());
        }

        [Fact]
        public void AssociatedComponentsProperty()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.AssociatedComponents);
        }

        [Fact]
        public void AccessibilityObjectField()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.AccessibilityObject);
        }

        [Fact]
        public void ControlProperty()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.Control);
        }

        [Fact]
        public void EnableDragRectProperty()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.GetEnableDragRectProperty());
        }

        [Fact]
        public void ParentComponentProperty()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.GetParentComponentProperty());
        }
        
        [Fact]
        public void ParticipatesWithSnapLinesProperty()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.ParticipatesWithSnapLines);
        }

        [Fact]
        public void AutoResizeHandlesProperty()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.AutoResizeHandles = true);
            Assert.Throws<NotImplementedException>(() => controlDesigner.AutoResizeHandles);
        }

        [Fact]
        public void SelectionRulesProperty()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.SelectionRules);
        }

        [Fact]
        public void SnapLinesProperty()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.SnapLines);
        }

        [Fact]
        public void InheritanceAttributeProperty()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.GetInheritanceAttributeProperty());
        }

        [Fact]
        public void NumberOfInternalControlDesignersTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.NumberOfInternalControlDesigners());
        }

        [Fact]
        public void InternalControlDesignerTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.InternalControlDesigner(1));
        }

        [Fact]
        public void BaseWndProcTest()
        {
            Message m = default;
            Assert.Throws<NotImplementedException>(() => controlDesigner.BaseWndProcMethod(ref m));
        }

        [Fact]
        public void CanBeParentedToTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.CanBeParentedTo(new ParentControlDesigner()));
        }

        [Fact]
        public void DefWndProcTest()
        {
            Message m = default;
            Assert.Throws<NotImplementedException>(() => controlDesigner.DefWndProcMethod(ref m));
        }

        [Fact]
        public void DisplayErrorTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.DisplayErrorMethod(new Exception()));
        }
               
        /// <summary>
        /// Bool data
        /// </summary>
        public static TheoryData BoolData => CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(BoolData))]
        public void DisposeTest(bool val)
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.DisposeMethod(val));
        }

        [Fact]
        public void EnableDesignModeTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.EnableDesignModeMethod(null, "fake"));
        }

        [Theory]
        [MemberData(nameof(BoolData))]
        public void EnableDragDropTest(bool val)
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.EnableDragDropMethod(val));
        }

        /// <summary>
        /// Data for the GlyphSelectionType enum
        /// </summary>
        public static TheoryData<Behavior.GlyphSelectionType> GlyphSelectionTypeData =>
                        CommonTestHelper.GetEnumTheoryData<Behavior.GlyphSelectionType>();

        [Theory]
        [MemberData(nameof(GlyphSelectionTypeData))]
        public void GetControlGlyphTest(Behavior.GlyphSelectionType glyphSelectionType)
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.GetControlGlyphMethod(glyphSelectionType));
        }
        
        [Theory]
        [MemberData(nameof(GlyphSelectionTypeData))]
        public void GetGlyphsTest(Behavior.GlyphSelectionType glyphSelectionType)
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.GetGlyphs(glyphSelectionType));
        }

        [Fact]
        public void GetHitTest()
        {
            Assert.False(controlDesigner.GetHitTestMethod(new Drawing.Point()));
        }

        [Fact]
        public void HookChildControlsTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.HookChildControlsMethod(null));
        }

        [Fact]
        public void InitializeTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.Initialize(null));
        }

        [Fact]
        public void InitializeExistingComponentTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.InitializeExistingComponent(null));
        }

        [Fact]
        public void InitializeNewComponentTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.InitializeNewComponent(null));
        }

        [Fact]
        public void OnSetComponentDefaultsTest()
        {
#pragma warning disable 618
            Assert.Throws<NotImplementedException>(() => controlDesigner.OnSetComponentDefaults());
#pragma warning restore 618
        }

        [Fact]
        public void OnContextMenuTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.OnContextMenuMethod(0, 0));
        }

        [Fact]
        public void OnCreateHandleTest()
        {
            Assert.Throws<NotImplementedException>(() => controlDesigner.OnCreateHandleMethod());
        }
    }
}
