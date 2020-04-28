// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    // NB: doesn't require thread affinity
    public class ToolStripItemDesignerAvailabilityAttributeTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void ToolStripItemDesignerAvailabilityAttribute_Ctor_Default()
        {
            var attribute = new ToolStripItemDesignerAvailabilityAttribute();
            Assert.Equal(ToolStripItemDesignerAvailability.None, attribute.ItemAdditionVisibility);
            Assert.True(attribute.IsDefaultAttribute());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemDesignerAvailability))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripItemDesignerAvailability))]
        public void ToolStripItemDesignerAvailabilityAttribute_Ctor_ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability visibility)
        {
            var attribute = new ToolStripItemDesignerAvailabilityAttribute(visibility);
            Assert.Equal(visibility, attribute.ItemAdditionVisibility);
        }

        [Fact]
        public void ToolStripItemDesignerAvailabilityAttribute_Default_Get_ReturnsExpected()
        {
            ToolStripItemDesignerAvailabilityAttribute attribute = ToolStripItemDesignerAvailabilityAttribute.Default;
            Assert.Same(attribute, ToolStripItemDesignerAvailabilityAttribute.Default);
            Assert.Equal(ToolStripItemDesignerAvailability.None, attribute.ItemAdditionVisibility);
            Assert.True(attribute.IsDefaultAttribute());
        }

        public static IEnumerable<object[]> IsDefaultAttribute_TestData()
        {
            yield return new object[] { new ToolStripItemDesignerAvailabilityAttribute(), true };
            yield return new object[] { new ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability.All), false };
        }

        [Theory]
        [MemberData(nameof(IsDefaultAttribute_TestData))]
        public void ToolStripItemDesignerAvailabilityAttribute_IsDefaultAttribute_Invoke_ReturnsExpected(ToolStripItemDesignerAvailabilityAttribute attribute, bool expected)
        {
            Assert.Equal(expected, attribute.IsDefaultAttribute());
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            var attribute = new ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability.All);
            yield return new object[] { attribute, attribute, true };
            yield return new object[] { attribute, new ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability.All), true };
            yield return new object[] { attribute, new ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability.None), false };

            yield return new object[] { attribute, new object(), false };
            yield return new object[] { attribute, null, false };
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public void ToolStripItemDesignerAvailabilityAttribute_Equals_Invoke_ReturnsExpected(ToolStripItemDesignerAvailabilityAttribute attribute, object obj, bool expected)
        {
            if (obj is ToolStripItemDesignerAvailability other)
            {
                Assert.Equal(expected, attribute.GetHashCode().Equals(other.GetHashCode()));
            }

            Assert.Equal(expected, attribute.Equals(obj));
        }
    }
}
