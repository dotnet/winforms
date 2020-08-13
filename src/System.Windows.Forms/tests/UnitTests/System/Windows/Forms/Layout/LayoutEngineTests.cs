// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Layout.Tests
{
    public class LayoutEngineTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void LayoutEngine_InitLayout_ValidChild_Nop()
        {
            var engine = new SubLayoutEngine();
            engine.InitLayout(new ScrollableControl(), BoundsSpecified.All);
        }

        [WinFormsFact]
        public void LayoutEngine_InitLayout_InvalidChild_ThrowsNotSupportedException()
        {
            var engine = new SubLayoutEngine();
            Assert.Throws<NotSupportedException>(() => engine.InitLayout("child", BoundsSpecified.All));
        }

        [WinFormsFact]
        public void LayoutEngine_InitLayout_NullChild_ThrowsArgumentNullException()
        {
            var engine = new SubLayoutEngine();
            Assert.Throws<ArgumentNullException>("child", () => engine.InitLayout(null, BoundsSpecified.All));
        }

        [WinFormsFact]
        public void LayoutEngine_Layout_ValidContainer_ReturnsFalse()
        {
            var engine = new SubLayoutEngine();
            Assert.False(engine.Layout(new ScrollableControl(), new LayoutEventArgs(new Component(), "affectedProperty")));
        }

        [WinFormsFact]
        public void LayoutEngine_Layout_InvalidContainer_Nop()
        {
            var engine = new SubLayoutEngine();
            Assert.Throws<NotSupportedException>(() => engine.Layout("container", new LayoutEventArgs(new Component(), "affectedProperty")));
        }

        [WinFormsFact]
        public void LayoutEngine_Layout_NullContainer_ThrowsArgumentNullException()
        {
            var engine = new SubLayoutEngine();
            Assert.Throws<ArgumentNullException>("container", () => engine.Layout(null, new LayoutEventArgs(new Component(), "affectedProperty")));
        }

        private class SubLayoutEngine : LayoutEngine
        {
        }
    }
}
