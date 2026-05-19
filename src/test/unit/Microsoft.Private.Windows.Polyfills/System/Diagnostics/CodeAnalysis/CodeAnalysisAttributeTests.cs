// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Diagnostics.CodeAnalysis.Tests;

public class CodeAnalysisAttributeTests
{
    [Fact]
    public void DynamicallyAccessedMembersAttribute_RoundTrips()
    {
        DynamicallyAccessedMembersAttribute attr = new(DynamicallyAccessedMemberTypes.PublicMethods);
        attr.MemberTypes.Should().Be(DynamicallyAccessedMemberTypes.PublicMethods);
    }

    [Fact]
    public void DynamicallyAccessedMembersAttribute_None()
    {
        DynamicallyAccessedMembersAttribute attr = new(DynamicallyAccessedMemberTypes.None);
        attr.MemberTypes.Should().Be(DynamicallyAccessedMemberTypes.None);
    }

    [Fact]
    public void DynamicallyAccessedMembersAttribute_All()
    {
        DynamicallyAccessedMembersAttribute attr = new(DynamicallyAccessedMemberTypes.All);
        attr.MemberTypes.Should().Be(DynamicallyAccessedMemberTypes.All);
    }

    [Fact]
    public void DynamicallyAccessedMembersAttribute_CombinedFlags()
    {
        DynamicallyAccessedMemberTypes combined = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods;
        DynamicallyAccessedMembersAttribute attr = new(combined);
        attr.MemberTypes.Should().HaveFlag(DynamicallyAccessedMemberTypes.PublicConstructors);
        attr.MemberTypes.Should().HaveFlag(DynamicallyAccessedMemberTypes.PublicMethods);
    }

    [Fact]
    public void DynamicallyAccessedMemberTypes_AllConstructors_IncludesBothPublicAndNonPublic()
    {
        DynamicallyAccessedMemberTypes allCtors = DynamicallyAccessedMemberTypes.AllConstructors;
        allCtors.Should().HaveFlag(DynamicallyAccessedMemberTypes.PublicConstructors);
        allCtors.Should().HaveFlag(DynamicallyAccessedMemberTypes.NonPublicConstructors);
    }

    [Fact]
    public void DynamicDependencyAttribute_StringSignature()
    {
        DynamicDependencyAttribute attr = new("Method");
        attr.MemberSignature.Should().Be("Method");
    }

    [Fact]
    public void DynamicDependencyAttribute_SignatureAndType()
    {
        DynamicDependencyAttribute attr = new("Method", typeof(string));
        attr.MemberSignature.Should().Be("Method");
        attr.Type.Should().Be(typeof(string));
    }

    [Fact]
    public void DynamicDependencyAttribute_SignatureTypeNameAssembly()
    {
        DynamicDependencyAttribute attr = new("Method", "System.String", "mscorlib");
        attr.MemberSignature.Should().Be("Method");
        attr.TypeName.Should().Be("System.String");
        attr.AssemblyName.Should().Be("mscorlib");
    }

    [Fact]
    public void DynamicDependencyAttribute_MemberTypesAndType()
    {
        DynamicDependencyAttribute attr = new(DynamicallyAccessedMemberTypes.PublicMethods, typeof(string));
        attr.MemberTypes.Should().Be(DynamicallyAccessedMemberTypes.PublicMethods);
        attr.Type.Should().Be(typeof(string));
    }

    [Fact]
    public void DynamicDependencyAttribute_MemberTypesTypeNameAssembly()
    {
        DynamicDependencyAttribute attr = new(DynamicallyAccessedMemberTypes.PublicFields, "System.String", "mscorlib");
        attr.MemberTypes.Should().Be(DynamicallyAccessedMemberTypes.PublicFields);
        attr.TypeName.Should().Be("System.String");
        attr.AssemblyName.Should().Be("mscorlib");
    }

    [Fact]
    public void DynamicDependencyAttribute_ConditionProperty()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        DynamicDependencyAttribute attr = new("Method") { Condition = "DEBUG" };
        attr.Condition.Should().Be("DEBUG");
#pragma warning restore CS0618
    }

    [Fact]
    public void RequiresUnreferencedCodeAttribute_Message()
    {
        RequiresUnreferencedCodeAttribute attr = new("Test message");
        attr.Message.Should().Be("Test message");
    }

    [Fact]
    public void RequiresUnreferencedCodeAttribute_UrlProperty()
    {
        RequiresUnreferencedCodeAttribute attr = new("Test") { Url = "https://example.com" };
        attr.Url.Should().Be("https://example.com");
    }

    [Fact]
    public void UnconditionalSuppressMessageAttribute_CategoryAndCheckId()
    {
        UnconditionalSuppressMessageAttribute attr = new("Usage", "CA2227");
        attr.Category.Should().Be("Usage");
        attr.CheckId.Should().Be("CA2227");
    }

    [Fact]
    public void UnconditionalSuppressMessageAttribute_AllProperties()
    {
        UnconditionalSuppressMessageAttribute attr = new("Usage", "CA2227")
        {
            Scope = "member",
            Target = "TestTarget",
            MessageId = "msg1",
            Justification = "Test justification"
        };

        attr.Scope.Should().Be("member");
        attr.Target.Should().Be("TestTarget");
        attr.MessageId.Should().Be("msg1");
        attr.Justification.Should().Be("Test justification");
    }
}
