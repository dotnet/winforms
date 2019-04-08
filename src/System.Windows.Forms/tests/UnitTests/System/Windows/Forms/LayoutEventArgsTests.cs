// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class LayoutEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_IComponent_String_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new Control(), "" };
            yield return new object[] { new SubComponent(), "affectedProperty" };
        }

        [Theory]
        [MemberData(nameof(Ctor_IComponent_String_TestData))]
        public void Ctor_IComponent_String(IComponent affectedComponent, string affectedProperty)
        {
            var e = new LayoutEventArgs(affectedComponent, affectedProperty);
            Assert.Equal(affectedComponent, e.AffectedComponent);
            Assert.Equal(affectedComponent as Control, e.AffectedControl);
            Assert.Equal(affectedProperty, e.AffectedProperty);
        }

        public static IEnumerable<object[]> Ctor_Control_String_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new Control(), "" };
            yield return new object[] { new Control(), "affectedProperty" };
        }

        [Theory]
        [MemberData(nameof(Ctor_Control_String_TestData))]
        public void Ctor_Control_String(Control affectedControl, string affectedProperty)
        {
            var e = new LayoutEventArgs(affectedControl, affectedProperty);
            Assert.Equal(affectedControl, e.AffectedComponent);
            Assert.Equal(affectedControl, e.AffectedControl);
            Assert.Equal(affectedProperty, e.AffectedProperty);
        }

        private class SubComponent : IComponent
        {
            public ISite Site { get; set; }

#pragma warning disable 0067
            public event EventHandler Disposed;
#pragma warning restore 0067

            public void Dispose() { }
        }
    }
}
