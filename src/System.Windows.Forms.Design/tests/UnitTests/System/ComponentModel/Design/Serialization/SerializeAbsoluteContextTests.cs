// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.ComponentModel.Design.Serialization.Tests
{
    public class SerializeAbsoluteContextTests
    {
        [Fact]
        public void SerializeAbsoluteContext_Ctor_Default()
        {
            var context = new SerializeAbsoluteContext();
            Assert.Null(context.Member);
        }

        public static IEnumerable<object[]> Ctor_MemberDescriptor_TestData()
        {
            yield return new object[] { null };

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(RootContext));
            yield return new object[] { properties[nameof(RootContext.Expression)] };
        }

        [Theory]
        [MemberData(nameof(Ctor_MemberDescriptor_TestData))]
        public void SerializeAbsoluteContext_Ctor_MemberDescriptor(MemberDescriptor member)
        {
            var context = new SerializeAbsoluteContext(member);
            Assert.Same(member, context.Member);
        }

        public static IEnumerable<object[]> ShouldSerialize_TestData()
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(RootContext));
            MemberDescriptor member1 = properties[nameof(RootContext.Expression)];
            MemberDescriptor member2 = properties[nameof(RootContext.Value)];

            yield return new object[] { new SerializeAbsoluteContext(null), null, true };
            yield return new object[] { new SerializeAbsoluteContext(null), member1, true };

            yield return new object[] { new SerializeAbsoluteContext(member1), null, false };
            yield return new object[] { new SerializeAbsoluteContext(member1), member1, true };
            yield return new object[] { new SerializeAbsoluteContext(member2), member2, true };
        }

        [Theory]
        [MemberData(nameof(ShouldSerialize_TestData))]
        public void SerializeAbsoluteContext_ShouldSerialize_Invoke_ReturnsExpected(SerializeAbsoluteContext context, MemberDescriptor member, bool expected)
        {
            Assert.Equal(expected, context.ShouldSerialize(member));
        }
    }
}
