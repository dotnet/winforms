// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DomainUpDownTests
    {
        [Fact]
        public void DomainUpDown_Constructor()
        {
            var underTest = GetNewDomainUpDown();
            
            Assert.NotNull(underTest);
            Assert.False(underTest.AllowDrop);
            Assert.False(underTest.Wrap);
            Assert.Equal(string.Empty, underTest.Text);
        }

        [Theory]
        [InlineData(0, 0, "foo1")]
        [InlineData(1, 1, "foo2")]
        [InlineData(3, 3, "Cowman")]
        public void DomainUpDown_SelectedIndexGetSet(int indexToSet, int indexAfterSet, string value)
        {
            var underTest = GetNewDomainUpDown();

            underTest.SelectedIndex = indexToSet;
            Assert.Equal(indexAfterSet, underTest.SelectedIndex);
            Assert.Equal(value, underTest.SelectedItem);
        }
        
        [Fact]
        public void DomainUpDown_SelectedIndex_ArgumentOutOfRangeException()
        {
            var underTest = GetNewDomainUpDown();
            var index = 3100;
            var paramName = "SelectedIndex";
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => underTest.SelectedIndex = index);
            Assert.Equal(paramName, ex.ParamName);
            Assert.Contains($"Value of '{index}' is not valid for '{paramName}'.\r\nParameter name: {paramName}", ex.Message);
        }

        [Fact]
        public void DomainUpDown_Sorted_SelectedIndexGetSet()
        {
            var underTest = GetNewDomainUpDown(true);

            underTest.SelectedIndex = 3;
            Assert.Equal(3, underTest.SelectedIndex);
            Assert.Equal("foo3", underTest.SelectedItem);
        }

        [Theory]
        [InlineData("cow", 0, 3)]
        [InlineData("cow", 4, 3)]
        [InlineData("foo", 0, 0)]
        [InlineData("foo", 3, 4)]
        [InlineData("foo", 4, 4)]
        [InlineData("foo", 5, 0)]
        [InlineData("foo", 100, 0)]
        [InlineData("foo", -1, 4)]
        [InlineData("foo", -100, 4)]
        [InlineData("foo2", 0, 1)]
        [InlineData("foo5", 0, -1)]
        [InlineData("foo5", 4, -1)]
        [InlineData("", 0, -1)]
        public void DomainUpDown_MatchIndex(string search, int start, int index)
        {
            var underTest = GetNewDomainUpDown();
            var expected = index;
            var actual = underTest.MatchIndex(search, false, start);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DomainUpDown_MatchIndex_NullReferenceException()
        {
            var underTest = GetNewDomainUpDown();
            Assert.Throws<NullReferenceException>(() => underTest.MatchIndex(null, false, 0));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(4, 3)]
        public void DomainUpDown_UpButton(int index, int newIndex)
        {
            var underTest = GetNewDomainUpDown();
            underTest.SelectedIndex = index;
            underTest.UpButton();
            var expected = newIndex;
            var actual = underTest.SelectedIndex;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(4, 4)]
        public void DomainUpDown_DownButton(int index, int newIndex)
        {
            var underTest = GetNewDomainUpDown();
            underTest.SelectedIndex = index;
            underTest.DownButton();
            var expected = newIndex;
            var actual = underTest.SelectedIndex;
            Assert.Equal(expected, actual);
        }

        private DomainUpDown GetNewDomainUpDown(bool sorted = false) {
            var domainUpDown = new DomainUpDown();
            domainUpDown.Sorted = sorted;
            var items = domainUpDown.Items;
            Assert.NotNull(items);
            items.Add("foo1");
            items.Add("foo2");
            items.Add("foo3");
            items.Add("Cowman");
            items.Add("foo4");

            return domainUpDown;
        }
    }
}
