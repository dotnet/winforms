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
    public async Task CSharp_ValueTypesAndNestedReturns_NoDiagnostic()
    {
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource("Example.cs",
                """
                using System;
                class Example
                {
                    public DateTime Value => new DateTime(2020, 1, 1);
                    public Func<object> Factory => static () => new object();
                    public object Cached
                    {
                        get
                        {
                            object Create() { return new object(); }
                            return this;
                        }
                    }
                }
                """));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task VisualBasic_ValueTypesAndNestedReturns_NoDiagnostic()
    {
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource("Example.vb",
                """
                Imports System
                Class Example
                    Public ReadOnly Property Value As DateTime
                        Get
                            Return New DateTime(2020, 1, 1)
                        End Get
                    End Property
                    Public ReadOnly Property Factory As Func(Of Object)
                        Get
                            Return Function() New Object()
                        End Get
                    End Property
                End Class
                """));

        await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CSharp_DirectObjectCreation_ReportsWarning()
    {
        const string source =
            """
            class Example
            {
                private readonly object _cached = new();

                public object {|WFO2013:ExpressionBodied|} => new object();
                public object {|WFO2013:Parenthesized|} => (new object());
                public object {|WFO2013:ConditionalPath|}
                {
                    get
                    {
                        if (System.DateTime.Now.Ticks > 0) return new object();
                        return _cached;
                    }
                }

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
            Imports System
            Class Example
                Private ReadOnly _cached As Object = New Object()

                Public ReadOnly Property {|WFO2013:Value|} As Object
                    Get
                        Return New Object()
                    End Get
                End Property

                Public ReadOnly Property {|WFO2013:Parenthesized|} As Object
                    Get
                        Return (New Object())
                    End Get
                End Property

                Public ReadOnly Property {|WFO2013:ConditionalPath|} As Object
                    Get
                        If DateTime.Now.Ticks > 0 Then Return New Object()
                        Return _cached
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

    [Fact]
    public async Task CSharp_UserDefinedConversion_NoDiagnostic()
    {
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource("Example.cs",
                """
                class Result { }
                class Source
                {
                    private static readonly Result s_cached = new Result();
                    public static implicit operator Result(Source source) => s_cached;
                }
                class Example { public Result Value => new Source(); }
                """));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Theory]
    [InlineData("Example.Designer.cs", "")]
    [InlineData("Example.cs", "// <auto-generated/>\r\n")]
    public async Task CSharp_GeneratedSource_NoDiagnostic(string path, string header)
    {
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource(path, header + "class Example { public object Value => new object(); }"));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }
}
