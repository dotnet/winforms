// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Globalization;
using System.Windows.Forms.CSharp.Analyzers.Diagnostics;
using System.Windows.Forms.CSharp.Generators.ApplicationConfiguration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit.Abstractions;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.Analyzers.Tests;

public partial class ProjectFileReaderTests
{
    private static readonly char s_separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
    private static readonly dynamic s_static = typeof(ProjectFileReader).TestAccessor().Dynamic;
    private readonly ITestOutputHelper _output;

    private static bool TryReadBool(AnalyzerConfigOptionsProvider configOptions, string propertyName, bool defaultValue, out bool value, out Diagnostic? diagnostic)
        => (bool)s_static.TryReadBool(configOptions, propertyName, defaultValue, out value, out diagnostic);

    private static bool TryReadFont(AnalyzerConfigOptionsProvider configOptions, out FontDescriptor? font, out Diagnostic? diagnostic)
        => (bool)s_static.TryReadFont(configOptions, out font, out diagnostic);

    private static bool TryReadHighDpiMode(AnalyzerConfigOptionsProvider configOptions, out HighDpiMode highDpiMode, out Diagnostic? diagnostic)
        => (bool)s_static.TryReadHighDpiMode(configOptions, out highDpiMode, out diagnostic);

    public ProjectFileReaderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData("a")]
    [InlineData("@")]
    [InlineData("yes")]
    [InlineData("no")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    public void ProjectFileReader_TryReadBool_value_invalid(string value)
    {
        Dictionary<string, string> properties = new()
        {
            { $"build_property.myvalue", value }
        };
        CompilerAnalyzerConfigOptions configOptions = new(properties.ToImmutableDictionary());
        CompilerAnalyzerConfigOptionsProvider provider = new(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, configOptions);

        bool result = TryReadBool(provider, "myvalue", defaultValue: false, out bool returnedValue, out Diagnostic? diagnostic);

        Assert.False(result);
        Assert.False(returnedValue);
        Assert.NotNull(diagnostic);
        Assert.Equal(CSharpDiagnosticDescriptors.s_propertyCantBeSetToValue, diagnostic!.Descriptor);
        _output.WriteLine(diagnostic.ToString());
    }

    [Theory]
    [InlineData("", true)] // default value
    [InlineData("false", false)]
    [InlineData("true", true)]
    [InlineData("False", false)]
    [InlineData("True", true)]
    [InlineData("FALSE", false)]
    [InlineData("TRUE", true)]
    public void ProjectFileReader_TryReadBool_value_valid(string value, bool expected)
    {
        Dictionary<string, string> properties = new()
        {
            { $"build_property.myvalue", value }
        };
        CompilerAnalyzerConfigOptions configOptions = new(properties.ToImmutableDictionary());
        CompilerAnalyzerConfigOptionsProvider provider = new(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, configOptions);

        bool result = TryReadBool(provider, "myvalue", defaultValue: true, out bool returnedValue, out Diagnostic? diagnostic);

        Assert.True(result);
        Assert.Equal(expected, returnedValue);
        Assert.Null(diagnostic);
    }

    public static TheoryData<string> ExceptionFontConverterData()
       => new()
       {
           // ArgumentException
           { $"Courier New{s_separator} 11 px{s_separator} type=Bold{s_separator} Italic" },
           { $"Courier New{s_separator} {s_separator} Style=Bold" },
           { $"Courier New{s_separator} 11{s_separator} Style=" },
           { $"Courier New{s_separator} 11{s_separator} Style=RandomEnum" },
           { $"Arial{s_separator} 10{s_separator} style=bold{s_separator}" },
           { $"Arial{s_separator} 10{s_separator} style=null" },
           { $"Arial{s_separator} 10{s_separator} style=abc#" },
           { $"Arial{s_separator} 10{s_separator} style=##" },
           { $"Arial{s_separator} 10display{s_separator} style=bold" },
           { $"Arial{s_separator} 10style{s_separator} style=bold" },

           // InvalidEnumArgumentException
           { $"Arial{s_separator} 10{s_separator} style=56" },
           { $"Arial{s_separator} 10{s_separator} style=-1" },
       };

    [Theory]
    [MemberData(nameof(ExceptionFontConverterData))]
    public void ProjectFileReader_TryReadFont_value_invalid(string font)
    {
        Dictionary<string, string> properties = new()
        {
            { $"build_property.{PropertyNameCSharp.DefaultFont}", font }
        };
        CompilerAnalyzerConfigOptions configOptions = new(properties.ToImmutableDictionary());
        CompilerAnalyzerConfigOptionsProvider provider = new(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, configOptions);

        bool result = TryReadFont(provider, out FontDescriptor? returnedValue, out Diagnostic? diagnostic);

        Assert.False(result);
        Assert.Null(returnedValue);
        Assert.NotNull(diagnostic);
        Assert.Equal(CSharpDiagnosticDescriptors.s_propertyCantBeSetToValueWithReason, diagnostic!.Descriptor);
        _output.WriteLine(diagnostic.ToString());
    }

    public static TheoryData<string> TestConvertFormData()
        => new()
        {
            { "Courier New" },
            { $"Arial{s_separator} 11px" },
            { $"Courier New{s_separator} style=Bold" },
            { $"Courier New{s_separator} 11 px{s_separator} style=Regular, Italic" },
            { $"arIAL{s_separator} 10{s_separator} style=bold" },
            { $"Arial{s_separator}" },
            { $"{s_separator} 10{s_separator} style=bold" },

            // NOTE: in .NET runtime these tests will result in FontName='', but the implementation relies on GDI+, which we don't have...
            { $"11px{s_separator} Style=Bold" },
            { $"11px" },
            { $"Style=Bold" },
        };

    [Theory]
    [MemberData(nameof(TestConvertFormData))]
    public void ProjectFileReader_TryReadFont_value_valid(string font)
    {
        Dictionary<string, string> properties = new()
        {
            { $"build_property.{PropertyNameCSharp.DefaultFont}", font }
        };
        CompilerAnalyzerConfigOptions configOptions = new(properties.ToImmutableDictionary());
        CompilerAnalyzerConfigOptionsProvider provider = new(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, configOptions);

        bool result = TryReadFont(provider, out _, out Diagnostic? diagnostic);

        Assert.True(result);
        Assert.Null(diagnostic);
    }

    [Theory]
    [InlineData("@")]
    [InlineData("yes")]
    [InlineData("no")]
    [InlineData("System")]
    [InlineData("10")]
    [InlineData("-1")]
    public void ProjectFileReader_TryReadHighDpiMode_value_invalid(string value)
    {
        Dictionary<string, string> properties = new()
        {
            { $"build_property.{PropertyNameCSharp.HighDpiMode}", value }
        };
        CompilerAnalyzerConfigOptions configOptions = new(properties.ToImmutableDictionary());
        CompilerAnalyzerConfigOptionsProvider provider = new(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, configOptions);

        bool result = TryReadHighDpiMode(provider, out HighDpiMode returnedValue, out Diagnostic? diagnostic);

        Assert.False(result);
        Assert.Equal(PropertyDefaultValue.DpiMode, returnedValue);
        Assert.NotNull(diagnostic);
        Assert.Equal(CSharpDiagnosticDescriptors.s_propertyCantBeSetToValue, diagnostic!.Descriptor);
        _output.WriteLine(diagnostic.ToString());
    }

    [Theory]
    [EnumData<HighDpiMode>]
    public void ProjectFileReader_TryReadHighDpiMode_value_valid(HighDpiMode value)
    {
        Dictionary<string, string> properties = new()
        {
            { $"build_property.{PropertyNameCSharp.HighDpiMode}", value.ToString() }
        };
        CompilerAnalyzerConfigOptions configOptions = new(properties.ToImmutableDictionary());
        CompilerAnalyzerConfigOptionsProvider provider = new(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, configOptions);

        bool result = TryReadHighDpiMode(provider, out HighDpiMode returnedValue, out Diagnostic? diagnostic);

        Assert.True(result);
        Assert.Equal(value, returnedValue);
        Assert.Null(diagnostic);
    }

    [Theory]
    [EnumData<HighDpiMode>]
    public void ProjectFileReader_TryReadHighDpiMode_value_asint_valid(HighDpiMode value)
    {
        Dictionary<string, string> properties = new()
        {
            { $"build_property.{PropertyNameCSharp.HighDpiMode}", ((int)value).ToString() }
        };
        CompilerAnalyzerConfigOptions configOptions = new(properties.ToImmutableDictionary());
        CompilerAnalyzerConfigOptionsProvider provider = new(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, configOptions);

        bool result = TryReadHighDpiMode(provider, out HighDpiMode returnedValue, out Diagnostic? diagnostic);

        Assert.True(result);
        Assert.Equal(value, returnedValue);
        Assert.Null(diagnostic);
    }
}
