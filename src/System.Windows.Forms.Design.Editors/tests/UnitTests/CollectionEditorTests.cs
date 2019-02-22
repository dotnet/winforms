// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace System.Windows.Forms.Design.Editors.System.ComponentModel.Design.Tests
{
    public class CollectionEditorTests
    {
        [Fact]
        public void CollectionEditor_Constructor()
        {
            var underTest = GetNewEditor();
            Assert.NotNull(underTest);
        }

        [Fact]
        public void CollectionEditor_Getters()
        {
            var underTest = GetNewEditor();
            Assert.Equal(typeof(string), underTest.GetCollectionType());
            Assert.True(underTest.CanSelectMultiple());
            Assert.True(underTest.CanRemove("some string"));
            Assert.True(underTest.CanRemove(1234));
            Assert.Equal("net.ComponentModel.CollectionEditor", underTest.GetHelpTopic());
            Assert.Equal("my string", underTest.GetItemDisplayText("my string"));
        }

        private TestCollectionEditor GetNewEditor()
        {
            return new TestCollectionEditor(typeof(string));
        }

        private class TestCollectionEditor : CollectionEditor
        {
            public TestCollectionEditor(Type type) : base(type)
            {
            }

            public Type GetCollectionType()
            {
                return base.CollectionType;
            }

            public bool CanSelectMultiple()
            {
                return base.CanSelectMultipleInstances();
            }

            public bool CanRemove(object value)
            {
                return base.CanRemoveInstance(value);
            }

            public string GetHelpTopic()
            {
                return base.HelpTopic;
            }

            public string GetItemDisplayText(object value)
            {
                return base.GetDisplayText(value);
            }
        }
    }
}
