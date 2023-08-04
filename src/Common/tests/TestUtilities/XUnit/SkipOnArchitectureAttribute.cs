// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit.Sdk;

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
[TraitDiscoverer("System.SkipOnArchitectureDiscoverer", "System.Windows.Forms.TestUtilities")]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class SkipOnArchitectureAttribute : Attribute, ITraitAttribute
{
    public SkipOnArchitectureAttribute(TestArchitectures testArchitectures, string reason) { }
}
