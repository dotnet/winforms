// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class GiveFeedbackEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(DragDropEffects.None, true)]
        [InlineData((DragDropEffects)(DragDropEffects.None - 1), false)]
        public void Ctor_DragDropEffects_Bool(DragDropEffects effect, bool useDefaultCursors)
        {
            var e = new GiveFeedbackEventArgs(effect, useDefaultCursors);
            Assert.Equal(effect, e.Effect);
            Assert.Equal(useDefaultCursors, e.UseDefaultCursors);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UseDefaultCursors_Set_GetReturnsExpected(bool value)
        {
            var e = new GiveFeedbackEventArgs(DragDropEffects.None, false)
            {
                UseDefaultCursors = value
            };
            Assert.Equal(value, e.UseDefaultCursors);
        }
    }
}
