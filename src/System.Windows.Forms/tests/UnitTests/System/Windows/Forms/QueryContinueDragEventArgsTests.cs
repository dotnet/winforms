// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class QueryContinueDragEventArgsTests
    {
        [Theory]
        [InlineData(-1, false, (DragAction)(DragAction.Continue - 1))]
        [InlineData(0, true, DragAction.Drop)]
        public void Ctor_Int_Bool_DragAction(int keyState, bool escapePressed, DragAction action)
        {
            var e = new QueryContinueDragEventArgs(keyState, escapePressed, action);
            Assert.Equal(keyState, e.KeyState);
            Assert.Equal(escapePressed, e.EscapePressed);
            Assert.Equal(action, e.Action);
        }

        [Theory]
        [InlineData((DragAction)(DragAction.Continue - 1))]
        [InlineData(DragAction.Drop)]
        public void Action_Set_GetReturnsExpected(DragAction value)
        {
            var e = new QueryContinueDragEventArgs(1, false, DragAction.Continue)
            {
                Action = value
            };
            Assert.Equal(value, e.Action);
        }
    }
}
