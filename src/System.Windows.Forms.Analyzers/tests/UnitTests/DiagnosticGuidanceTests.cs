// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Xunit;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Verifies that emitted diagnostics contain their repair instructions, not just their help pages.
/// </summary>
public class DiagnosticGuidanceTests
{
    [Fact]
    public void Nameof_EmitsLiteralReplacement()
        => Assert.Equal(
            "CodeDOM cannot represent nameof in InitializeComponent; replace it with the serialized string literal",
            Diagnostic.Create(SharedDiagnosticDescriptors.s_unsupportedNameOfExpression, Location.None)
                .GetMessage(CultureInfo.InvariantCulture));

    [Fact]
    public void Allocation_DescribesReturnPath()
        => Assert.Equal(
            "Getter 'Items' directly returns a newly constructed reference instance on this path; cache it only if stable identity is intended, or use a factory method for intentional creation",
            Diagnostic.Create(SharedDiagnosticDescriptors.s_propertyAllocatesNewInstance, Location.None, "Items")
                .GetMessage(CultureInfo.InvariantCulture));

    [Fact]
    public void VisualBasicEvent_EmitsHandlesTarget()
        => Assert.Equal(
            "Use Handles Button1.Click on a named handler in the user partial instead of AddHandler in InitializeComponent; remove the redundant hookup and keep component members WithEvents",
            Diagnostic.Create(SharedDiagnosticDescriptors.s_designerAddHandler, Location.None, "Button1.Click")
                .GetMessage(CultureInfo.InvariantCulture));

    [Fact]
    public void VisualBasicLambda_EmitsLanguageSpecificRepairs()
        => Assert.Equal(
            "CodeDOM cannot represent lambdas in InitializeComponent; use a named method in the user partial, with Handles for designer member events or AddressOf for local component events",
            Diagnostic.Create(SharedDiagnosticDescriptors.s_visualBasicUnsupportedAnonymousFunction, Location.None)
                .GetMessage(CultureInfo.InvariantCulture));
}
