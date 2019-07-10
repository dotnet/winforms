// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DockingAttributeTests
    {
        [Fact]
        public void DockingAttribute_Ctor_Default()
        {
            var attribute = new DockingAttribute();
            Assert.Equal(DockingBehavior.Never, attribute.DockingBehavior);
            Assert.True(attribute.IsDefaultAttribute());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockingBehavior))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DockingBehavior))]
        public void DockingAttribute_Ctor_DockingBehavior(DockingBehavior dockingBehavior)
        {
            var attribute = new DockingAttribute(dockingBehavior);
            Assert.Equal(dockingBehavior, attribute.DockingBehavior);
            Assert.Equal(dockingBehavior == DockingBehavior.Never, attribute.IsDefaultAttribute());
        }

        [Fact]
        public void DockingAttribute_Default_Get_ReturnsExpected()
        {
            DockingAttribute attribute = DockingAttribute.Default;
            Assert.Same(attribute, DockingAttribute.Default);
            Assert.Equal(DockingBehavior.Never, attribute.DockingBehavior);
            Assert.True(attribute.IsDefaultAttribute());
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            var attribute = new DockingAttribute(DockingBehavior.Ask);
            yield return new object[] { attribute, attribute, true };
            yield return new object[] { attribute, new DockingAttribute(DockingBehavior.Ask), true };
            yield return new object[] { attribute, new DockingAttribute(DockingBehavior.Never), false };

            yield return new object[] { attribute, new object(), false };
            yield return new object[] { attribute, null, false };
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public void DockingAttribute_Equals_Invoke_ReturnsExpected(DockingAttribute attribute, object other, bool expected)
        {
            Assert.Equal(expected, attribute.Equals(other));
            if (other is DockingAttribute)
            {
                Assert.Equal(expected, attribute.GetHashCode().Equals(other.GetHashCode()));
            }
        }
    }
}
