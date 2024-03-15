// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.DotNet.XUnitExtensions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit;

/// <summary>
///  This class discovers all of the tests and test classes that have
///  applied the <see cref="SkipOnArchitectureAttribute"/>.
/// </summary>
public class SkipOnArchitectureDiscoverer : ITraitDiscoverer
{
    /// <summary>
    ///  Gets the trait values from the Category attribute.
    /// </summary>
    /// <param name="traitAttribute">The trait attribute containing the trait values.</param>
    /// <returns>The trait values.</returns>
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        TestArchitectures testArchitectures = 0;

        if (traitAttribute.GetConstructorArguments().FirstOrDefault() is TestArchitectures architectures)
        {
            testArchitectures = architectures;
        }

        if ((testArchitectures.HasFlag(TestArchitectures.X86) && RuntimeInformation.ProcessArchitecture == Architecture.X86)
            || (testArchitectures.HasFlag(TestArchitectures.X64) && RuntimeInformation.ProcessArchitecture == Architecture.X64))
        {
            return new[] { new KeyValuePair<string, string>(XunitConstants.Category, XunitConstants.IgnoreForCI) };
        }

        return Array.Empty<KeyValuePair<string, string>>();
    }
}
