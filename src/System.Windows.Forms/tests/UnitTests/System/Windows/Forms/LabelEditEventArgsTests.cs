// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class LabelEditEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void Ctor_Int(int item)
        {
            var e = new LabelEditEventArgs(item);
            Assert.Equal(item, e.Item);
            Assert.Null(e.Label);
            Assert.False(e.CancelEdit);
        }

        [Theory]
        [InlineData(-1, null)]
        [InlineData(0, "")]
        [InlineData(1, "label")]
        public void Ctor_Int_String(int item, string label)
        {
            var e = new LabelEditEventArgs(item, label);
            Assert.Equal(item, e.Item);
            Assert.Equal(label, e.Label);
            Assert.False(e.CancelEdit);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CancelEdit_Set_GetReturnsExpected(bool value)
        {
            var e = new LabelEditEventArgs(1)
            {
                CancelEdit = value
            };
            Assert.Equal(value, e.CancelEdit);
        }
    }
}
