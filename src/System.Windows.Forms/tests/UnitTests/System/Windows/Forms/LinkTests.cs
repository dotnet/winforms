// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class LinkTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void Link_Ctor_Default()
        {
            var link = new LinkLabel.Link();
            Assert.Null(link.Description);
            Assert.True(link.Enabled);
            Assert.Equal(0, link.Length);
            Assert.Null(link.LinkData);
            Assert.Empty(link.Name);
            Assert.Equal(0, link.Start);
            Assert.Null(link.Tag);
            Assert.False(link.Visited);
        }

        [Theory]
        [InlineData(-2, -3, -3)]
        [InlineData(-2, -1, 0)]
        [InlineData(-1, 0, 0)]
        [InlineData(0, 0, 0)]
        [InlineData(1, 2, 2)]
        public void Link_Ctor_Int_Int(int start, int length, int expectedLength)
        {
            var link = new LinkLabel.Link(start, length);
            Assert.Null(link.Description);
            Assert.True(link.Enabled);
            Assert.Equal(expectedLength, link.Length);
            Assert.Null(link.LinkData);
            Assert.Empty(link.Name);
            Assert.Equal(start, link.Start);
            Assert.Null(link.Tag);
            Assert.False(link.Visited);
        }

        [Theory]
        [InlineData(-2, -3, null, -3)]
        [InlineData(-2, -1, "", 0)]
        [InlineData(-1, 0, "linkData", 0)]
        [InlineData(0, 0, "linkData", 0)]
        [InlineData(1, 2, "linkData", 2)]
        public void Link_Ctor_Int_Int_Object(int start, int length, object linkData, int expectedLength)
        {
            var link = new LinkLabel.Link(start, length, linkData);
            Assert.Null(link.Description);
            Assert.True(link.Enabled);
            Assert.Equal(expectedLength, link.Length);
            Assert.Same(linkData, link.LinkData);
            Assert.Empty(link.Name);
            Assert.Equal(start, link.Start);
            Assert.Null(link.Tag);
            Assert.False(link.Visited);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Link_Description_SetWithoutOwner_GetReturnsExpected(string value)
        {
            var link = new LinkLabel.Link
            {
                Description = value
            };
            Assert.Same(value, link.Description);

            // Set same.
            link.Description = value;
            Assert.Same(value, link.Description);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Link_Enabled_SetWithoutOwner_GetReturnsExpected(bool value)
        {
            var link = new LinkLabel.Link
            {
                Enabled = value
            };
            Assert.Equal(value, link.Enabled);

            // Set same.
            link.Enabled = value;
            Assert.Equal(value, link.Enabled);

            // Set opposite.
            link.Enabled = !value;
            Assert.Equal(!value, link.Enabled);
        }

        [Theory]
        [InlineData(-2, -2)]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        public void Link_Length_SetWithoutOwner_GetReturnsExpected(int value, int expected)
        {
            var link = new LinkLabel.Link
            {
                Length = value
            };
            Assert.Equal(expected, link.Length);

            // Set same.
            link.Length = value;
            Assert.Equal(expected, link.Length);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Link_LinkData_SetWithoutOwner_GetReturnsExpected(string value)
        {
            var link = new LinkLabel.Link
            {
                LinkData = value
            };
            Assert.Same(value, link.LinkData);

            // Set same.
            link.LinkData = value;
            Assert.Same(value, link.LinkData);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Link_Name_SetWithoutOwner_GetReturnsExpected(string value, string expected)
        {
            var link = new LinkLabel.Link
            {
                Name = value
            };
            Assert.Equal(expected, link.Name);

            // Set same.
            link.LinkData = value;
            Assert.Equal(expected, link.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Link_Start_SetWithoutOwner_GetReturnsExpected(int value)
        {
            var link = new LinkLabel.Link
            {
                Start = value
            };
            Assert.Equal(value, link.Start);

            // Set same.
            link.Start = value;
            Assert.Equal(value, link.Start);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Link_Tag_SetWithoutOwner_GetReturnsExpected(string value)
        {
            var link = new LinkLabel.Link
            {
                Tag = value
            };
            Assert.Same(value, link.Tag);

            // Set same.
            link.Tag = value;
            Assert.Same(value, link.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Link_Visited_SetWithoutOwner_GetReturnsExpected(bool value)
        {
            var link = new LinkLabel.Link
            {
                Visited = value
            };
            Assert.Equal(value, link.Visited);

            // Set same.
            link.Visited = value;
            Assert.Equal(value, link.Visited);

            // Set opposite.
            link.Visited = !value;
            Assert.Equal(!value, link.Visited);
        }
    }
}
