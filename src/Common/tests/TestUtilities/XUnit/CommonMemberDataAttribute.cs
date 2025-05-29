// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit.v3;

namespace Xunit;

/// <summary>
///  A custom MemberData attribute that is specialized for the CommonTestHelper type.
///  Useful to remove the need to suffix all attributes with "MemberType = ..."
///  We cannot inherit from <see cref="MemberDataAttribute"/> as it is sealed, so we have to reimplement
///  ConvertDataItem inheriting from MemberDataAttributeBase.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class CommonMemberDataAttribute : MemberDataAttributeBase
{
    public CommonMemberDataAttribute(Type memberType, string memberName = "TheoryData")
        : this(memberType, memberName, Array.Empty<object>()) { }

    public CommonMemberDataAttribute(Type memberType, string memberName, params object?[] parameters)
        : base(memberName, parameters)
    {
        MemberType = memberType;
    }
}
