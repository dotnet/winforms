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
    [Theory]
    [InlineData("new object[1]")]
    [InlineData("new[] { new object() }")]
    [InlineData("new { Value = new object() }")]
    public async Task CSharp_ArrayAndAnonymousObjectCreation_ReportsWarning(string creation)
    {
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource("Example.cs",
                $$"""
                class Example
                {
                    public object {|WFO2000:ExpressionBodied|} => {{creation}};
                    public object {|WFO2000:Getter|}
                    {
                        get
                        {
                            if (System.DateTime.Now.Ticks > 0) return {{creation}};
                            return (object)({{creation}});
                        }
                    }
                }
                """));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Theory]
    [InlineData("New Object(0) {}")]
    [InlineData("{New Object()}")]
    [InlineData("New With {.Value = New Object()}")]
    public async Task VisualBasic_ArrayAndAnonymousObjectCreation_ReportsWarning(string creation)
    {
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource("Example.vb",
                $$"""
                Class Example
                    Public ReadOnly Property {|WFO2000:Value|} As Object
                        Get
                            If System.DateTime.Now.Ticks > 0 Then Return {{creation}}
                            Return CType(({{creation}}), Object)
                        End Get
                    End Property
                End Class
                """));

        await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Theory]
    [InlineData("new object[1]")]
    [InlineData("new[] { new object() }")]
    [InlineData("new { Value = new object() }")]
    public async Task CSharp_ArrayAndAnonymousObjectCreation_ExcludedContexts_NoDiagnostic(string creation)
    {
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource("Example.cs",
                $$"""
                class Example
                {
                    private readonly object _cached = {{creation}};
                    public object Cached => _cached;
                    public object Initialized { get; } = {{creation}};
                    public object this[int index] => {{creation}};
                    public System.Func<object> Factory => () => {{creation}};
                    public object LocalFunction
                    {
                        get
                        {
                            object Create() { return {{creation}}; }
                            return Create();
                        }
                    }
                    public object Create() => {{creation}};
                }
                """),
            new AnalyzerTestSource("Generated.Designer.cs",
                $$"""class Generated { public object Value => {{creation}}; }"""));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Theory]
    [InlineData("New Object(0) {}")]
    [InlineData("{New Object()}")]
    [InlineData("New With {.Value = New Object()}")]
    public async Task VisualBasic_ArrayAndAnonymousObjectCreation_ExcludedContexts_NoDiagnostic(string creation)
    {
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90,
            new AnalyzerTestSource("Example.vb",
                $$"""
                Class Example
                    Private ReadOnly _cached As Object = {{creation}}
                    Public ReadOnly Property Cached As Object
                        Get
                            Return _cached
                        End Get
                    End Property
                    Public ReadOnly Property Initialized As Object = {{creation}}
                    Default Public ReadOnly Property Item(index As Integer) As Object
                        Get
                            Return {{creation}}
                        End Get
                    End Property
                    Public ReadOnly Property Factory As System.Func(Of Object)
                        Get
                            Return Function() {{creation}}
                        End Get
                    End Property
                    Public Function Create() As Object
                        Return {{creation}}
                    End Function
                End Class
                """),
            new AnalyzerTestSource("Generated.Designer.vb",
                $$"""
                Class Generated
                    Public ReadOnly Property Value As Object
                        Get
                            Return {{creation}}
                        End Get
                    End Property
                End Class
                """));

        await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest<PropertyAllocatesNewInstanceAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

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

                public object {|WFO2000:ExpressionBodied|} => new object();
                public object {|WFO2000:Parenthesized|} => (new object());
                public object {|WFO2000:ConditionalPath|}
                {
                    get
                    {
                        if (System.DateTime.Now.Ticks > 0) return new object();
                        return _cached;
                    }
                }

                public object {|WFO2000:Getter|}
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

                Public ReadOnly Property {|WFO2000:Value|} As Object
                    Get
                        Return New Object()
                    End Get
                End Property

                Public ReadOnly Property {|WFO2000:Parenthesized|} As Object
                    Get
                        Return (New Object())
                    End Get
                End Property

                Public ReadOnly Property {|WFO2000:ConditionalPath|} As Object
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
