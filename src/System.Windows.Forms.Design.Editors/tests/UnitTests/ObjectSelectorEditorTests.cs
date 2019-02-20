// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing.Design;
using Xunit;

namespace System.Windows.Forms.Design.Editors.System.ComponentModel.Design.Tests
{
    public class ObjectSelectorEditorTests
    {

        [Fact]
        public void ObjectSelectorEditor_Constructor()
        {
            var underTest = GetNewObjectSelectorEditor();

            Assert.NotNull(underTest);
            Assert.True(underTest.EqualsToValue(null));
            Assert.False(underTest.SubObjectSelector);

            underTest = GetNewObjectSelectorEditor(true);
            Assert.True(underTest.SubObjectSelector);
        }

        [Fact]
        public void ObjectSelectorEditor_CurrentValue()
        {
            var underTest = GetNewObjectSelectorEditor();

            underTest.SetValue("some Value");
            Assert.True(underTest.EqualsToValue("some Value"));
            Assert.False(underTest.EqualsToValue("some other value"));
        }

        [Fact]
        public void ObjectSelectorEditor_GetEditStyle() {
            var underTest = GetNewObjectSelectorEditor();

            Assert.Equal(UITypeEditorEditStyle.DropDown, underTest.GetEditStyle(null));
        }

        [Fact]
        public void ObjectSelectorEditor_Selector()
        {
            var underTest = GetNewSelector();

            Assert.NotNull(underTest);
            underTest.AddNode("node", "value", null);
            Assert.Single(underTest.Nodes);
            Assert.True(underTest.SetSelection("value", null));
            Assert.False(underTest.SetSelection("other value", null));
            underTest.Clear();
            Assert.Empty(underTest.Nodes);
        }

        private ObjectSelectorEditor GetNewObjectSelectorEditor(bool subObjectSelector = false)
        {
            return subObjectSelector ? new TestObjectSelectorEditor(subObjectSelector) : new TestObjectSelectorEditor();
        }

        private class TestObjectSelectorEditor : ObjectSelectorEditor {
            public TestObjectSelectorEditor()
            {
            }
            public TestObjectSelectorEditor(bool subObjectSelector) : base(subObjectSelector)
            {
            }
        }

        private ObjectSelectorEditor.Selector GetNewSelector() {
            return new ObjectSelectorEditor.Selector(GetNewObjectSelectorEditor());
        }
    }
}
