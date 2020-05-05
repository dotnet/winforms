// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class LinkLabelLinkClickedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_LinkLabelLink_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new LinkLabel.Link() };
        }

        [Theory]
        [MemberData(nameof(Ctor_LinkLabelLink_TestData))]
        public void Ctor_LinkLabelLink(LinkLabel.Link link)
        {
            var e = new LinkLabelLinkClickedEventArgs(link);
            Assert.Equal(link, e.Link);
            Assert.Equal(MouseButtons.Left, e.Button);
        }

        public static IEnumerable<object[]> Ctor_LinkLabelLink_MouseButtons_TestData()
        {
            yield return new object[] { null, (MouseButtons)1 };
            yield return new object[] { new LinkLabel.Link(), MouseButtons.Left };
            yield return new object[] { new LinkLabel.Link(), MouseButtons.None };
        }

        [Theory]
        [MemberData(nameof(Ctor_LinkLabelLink_MouseButtons_TestData))]
        public void Ctor_LinkLabelLink_MouseButtons(LinkLabel.Link link, MouseButtons button)
        {
            var e = new LinkLabelLinkClickedEventArgs(link, button);
            Assert.Equal(link, e.Link);
            Assert.Equal(button, e.Button);
        }
    }
}
