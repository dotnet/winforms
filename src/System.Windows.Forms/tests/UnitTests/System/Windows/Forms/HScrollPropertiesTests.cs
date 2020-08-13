// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class HScrollPropertiesTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_ScrollableControl_TestData()
        {
            yield return new object[] { new ScrollableControl() };
            yield return new object[] { null };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_ScrollableControl_TestData))]
        public void HScrollProperties_Ctor_Control(ScrollableControl container)
        {
            var properties = new SubHScrollProperties(container);
            Assert.Equal(container, properties.ParentControlEntry);
            Assert.True(properties.Enabled);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.False(properties.Visible);
        }

        private class SubHScrollProperties : HScrollProperties
        {
            public SubHScrollProperties(ScrollableControl container) : base(container)
            {
            }

            public ScrollableControl ParentControlEntry => ParentControl;
        }
    }
}
