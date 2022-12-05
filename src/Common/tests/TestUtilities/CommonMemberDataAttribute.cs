// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Windows.Forms.TestUtilities
{
    /// <summary>
    ///  A custom MemberData attribute that is specialized for the CommonTestHelper type.
    ///  Useful to remove the need to suffix all attributes with "MemberType = ..."
    ///  We cannot inherit from MemberDataAttribute as it is sealed, so we have to reimplement
    ///  ConvertDataItem inheriting from MemberDataAttributeBase.
    /// </summary>
    public sealed class CommonMemberDataAttribute : MemberDataAttributeBase
    {
        public CommonMemberDataAttribute(Type memberType, string memberName, params object[] parameters) : base(memberName, parameters)
        {
            MemberType = memberType;
        }

        protected override object[]? ConvertDataItem(MethodInfo testMethod, object item)
        {
            if (item is null)
            {
                return null;
            }

            if (item is not object[] array)
            {
                throw new ArgumentException($"Property {MemberName} on {MemberType ?? testMethod.DeclaringType} yielded an item that is not an object[]");
            }

            return array;
        }
    }
}
