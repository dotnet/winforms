// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Tests warnings for properties that construct a new object on every access.
/// </summary>
public class PropertyAllocatesNewInstanceAnalyzerTests
{
    [Fact]
    public async Task CSharp_DirectObjectCreation_ReportsWarning()
    {
        const string source =
            """
            class Example
            {
                private readonly object _cached = new();

                public object {|WFO2013:ExpressionBodied|} => new object();

                public object {|WFO2013:Getter|}
                {
                    get
                    {
                        return new object();
                    }
                }

                public object Cached => _cached;

                public object Initialized { get; } = new object();

                public object Create() => new object();
            }
            """;

        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource("Example.cs", source));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(
            testCase).RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task VisualBasic_DirectObjectCreation_ReportsWarning()
    {
        const string source =
            """
            Class Example
                Private ReadOnly _cached As Object = New Object()

                Public ReadOnly Property {|WFO2013:Value|} As Object
                    Get
                        Return New Object()
                    End Get
                End Property

                Public ReadOnly Property Cached As Object
                    Get
                        Return _cached
                    End Get
                End Property

                Public ReadOnly Property Initialized As Object = New Object()

                Public Function Create() As Object
                    Return New Object()
                End Function
            End Class
            """;

        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource("Example.vb", source));

        await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(
            testCase).RunAsync(TestContext.Current.CancellationToken);
    }
}
