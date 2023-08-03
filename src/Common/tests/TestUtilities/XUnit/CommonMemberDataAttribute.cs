// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace Xunit;

/// <summary>
///  A custom MemberData attribute that is specialized for the CommonTestHelper type.
///  Useful to remove the need to suffix all attributes with "MemberType = ..."
///  We cannot inherit from <see cref="MemberDataAttribute"/> as it is sealed, so we have to reimplement
///  ConvertDataItem inheriting from MemberDataAttributeBase.
/// </summary>
public class CommonMemberDataAttribute : MemberDataAttributeBase
{
    public CommonMemberDataAttribute(Type memberType, string memberName = "TheoryData")
        : this(memberType, memberName, null) { }

    public CommonMemberDataAttribute(Type memberType, string memberName, params object[]? parameters)
        : base(memberName, parameters)
    {
        MemberType = memberType;
    }

    protected override object[]? ConvertDataItem(MethodInfo testMethod, object? item)
    {
        if (item is null)
        {
            return null;
        }

        if (item is not object[] array)
        {
            throw new ArgumentException($"Property {MemberName} on {MemberType ?? testMethod.DeclaringType} yielded an item that is not an object[], but {item.GetType().Name}");
        }

        return array;
    }
}
