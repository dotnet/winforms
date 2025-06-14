// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.DotNet.XUnitExtensions;
using Xunit.v3;

namespace Xunit;

/// <summary>
///  Apply this attribute to your test method or class to skip it on a certain architecture.
/// </summary>
/// <remarks>
///  <para>
///   This attribute isn't able to distinguish Arm64 from x64. To skip tests on Arm64 use
///   <see cref="TestArchitectures.X64"/>. See https://github.com/dotnet/winforms/issues/7013.
///  </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class SkipOnArchitectureAttribute : Attribute, ITraitAttribute
{
    private readonly TestArchitectures _testArchitectures;

    public SkipOnArchitectureAttribute(TestArchitectures testArchitectures, string reason)
    {
        _testArchitectures = testArchitectures;
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
    {
        if ((_testArchitectures.HasFlag(TestArchitectures.X86) && RuntimeInformation.ProcessArchitecture == Architecture.X86)
            || (_testArchitectures.HasFlag(TestArchitectures.X64) && RuntimeInformation.ProcessArchitecture == Architecture.X64))
        {
            return new[] { new KeyValuePair<string, string>(XunitConstants.Category, XunitConstants.IgnoreForCI) };
        }

        return Array.Empty<KeyValuePair<string, string>>();
    }
}
