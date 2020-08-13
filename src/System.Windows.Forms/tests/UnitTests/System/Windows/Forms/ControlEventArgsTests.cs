// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ControlEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Control_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Button() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Control_TestData))]
        public void Ctor_Control(Control control)
        {
            var e = new ControlEventArgs(control);
            Assert.Equal(control, e.Control);
        }
    }
}
