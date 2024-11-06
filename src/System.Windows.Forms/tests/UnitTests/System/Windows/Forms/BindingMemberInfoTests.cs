// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class BindingMemberInfoTests
{
    [Fact]
    public void Ctor_Default()
    {
        BindingMemberInfo info = default;
        Assert.Empty(info.BindingPath);
        Assert.Empty(info.BindingField);
        Assert.Empty(info.BindingMember);
    }

    [Theory]
    [InlineData(null, "", "", "")]
    [InlineData("", "", "", "")]
    [InlineData("Some", "", "Some", "Some")]
    [InlineData("Some.More.Other", "Some.More", "Other", "Some.More.Other")]
    [InlineData("Some.Other", "Some", "Other", "Some.Other")]
    [InlineData("Some.", "Some", "", "Some.")]
    [InlineData(".Other", "", "Other", "Other")]
    [InlineData(".", "", "", "")]
    public void Ctor_String(string dataMember, string expectedPath, string expectedField, string expectedMember)
    {
        BindingMemberInfo info = new(dataMember);
        Assert.Equal(expectedPath, info.BindingPath);
        Assert.Equal(expectedField, info.BindingField);
        Assert.Equal(expectedMember, info.BindingMember);
    }

    public static IEnumerable<object[]> Equals_TestData()
    {
        yield return new object[] { new BindingMemberInfo("Some.Other"), new BindingMemberInfo("Some.Other"), true };
        yield return new object[] { new BindingMemberInfo("Some.Other"), new BindingMemberInfo("some.other"), true };
        yield return new object[] { new BindingMemberInfo("Some.Other"), new BindingMemberInfo("Some2.Other"), false };
        yield return new object[] { new BindingMemberInfo("Some.Other"), new BindingMemberInfo("Some.Other2"), false };
        yield return new object[] { new BindingMemberInfo("Some.Other"), new BindingMemberInfo("Some"), false };
        yield return new object[] { new BindingMemberInfo("Some.Other"), default(BindingMemberInfo), false };

        yield return new object[] { new BindingMemberInfo("Some"), new BindingMemberInfo("Some"), true };
        yield return new object[] { new BindingMemberInfo("Some"), new BindingMemberInfo("Some2"), false };
        yield return new object[] { new BindingMemberInfo("Some"), new BindingMemberInfo("Some.Other"), false };
        yield return new object[] { new BindingMemberInfo("Some.Other"), default(BindingMemberInfo), false };

        yield return new object[] { default(BindingMemberInfo), default(BindingMemberInfo), true };
        yield return new object[] { default(BindingMemberInfo), new BindingMemberInfo(""), true };
        yield return new object[] { default(BindingMemberInfo), new BindingMemberInfo("Some.Other"), false };

        yield return new object[] { new BindingMemberInfo("Some.Other"), new(), false };
        yield return new object[] { new BindingMemberInfo("Some.Other"), null, false };
    }

    [Theory]
    [MemberData(nameof(Equals_TestData))]
    public void Equals_Invoke_ReturnsExpected(BindingMemberInfo info, object other, bool expected)
    {
        if (other is BindingMemberInfo otherInfo)
        {
            Assert.Equal(expected, info == otherInfo);
            Assert.Equal(!expected, info != otherInfo);
        }

        Assert.Equal(expected, info.Equals(other));
    }

    public static IEnumerable<object[]> GetHashCode_TestData()
    {
        yield return new object[] { default(BindingMemberInfo) };
        yield return new object[] { new BindingMemberInfo("Some") };
        yield return new object[] { new BindingMemberInfo("Some.Other") };
    }

    [Theory]
    [MemberData(nameof(GetHashCode_TestData))]
    public void GetHashCode_Invoke_ReturnsExpected(BindingMemberInfo info)
    {
        Assert.Equal(info.GetHashCode(), info.GetHashCode());
    }
}
