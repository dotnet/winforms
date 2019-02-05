// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Layout.Tests
{
    public class LayoutEngineTests
    {
        [Fact]
        public void LayoutEngine_InitLayout_ValidChild_Nop()
        {
            var engine = new SubLayoutEngine();
            engine.InitLayout(new ScrollableControl(), BoundsSpecified.All);
        }

        [Fact]
        public void LayoutEngine_InitLayout_InvalidChild_Nop()
        {
            var engine = new SubLayoutEngine();
            engine.InitLayout("child", BoundsSpecified.All);
        }

        [Fact]
        public void LayoutEngine_InitLayout_NullChild_ThrowsNullReferenceException()
        {
            var engine = new SubLayoutEngine();
            Assert.Throws<NullReferenceException>(() => engine.InitLayout(null, BoundsSpecified.All));
        }

        [Fact]
        public void LayoutEngine_Layout_ValidContainer_ReturnsFalse()
        {
            var engine = new SubLayoutEngine();
            Assert.False(engine.Layout(new ScrollableControl(), new LayoutEventArgs(new Component(), "affectedProperty")));
        }

        [Fact]
        public void LayoutEngine_Layout_InvalidContainer_Nop()
        {
            var engine = new SubLayoutEngine();
            engine.Layout("container", new LayoutEventArgs(new Component(), "affectedProperty"));
        }

        [Fact]
        public void LayoutEngine_Layout_NullContainer_ThrowsNullReferenceException()
        {
            var engine = new SubLayoutEngine();
            Assert.Throws<NullReferenceException>(() => engine.Layout(null, new LayoutEventArgs(new Component(), "affectedProperty")));
        }

        private class SubLayoutEngine : LayoutEngine
        {
        }
    }
}
