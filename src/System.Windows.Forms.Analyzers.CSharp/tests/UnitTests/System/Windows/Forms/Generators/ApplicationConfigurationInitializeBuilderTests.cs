// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Windows.Forms.Analyzers;
using VerifyXunit;
using Xunit;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.Generators.Tests;

[UsesVerify]
public partial class ApplicationConfigurationInitializeBuilderTests
{
    private static readonly string[] s_locales =
    [
        "ar-SA",
        "en-US",
        "es-ES",
        "fr-FR",
        "hi-IN",
        "ja-JP",
        "ru-RU",
        "tr-TR",
        "zh-CN"
    ];

    [Theory]
    [InlineData(null, "default_top_level")]
    [InlineData("", "default_top_level")]
    [InlineData(" ", "default_top_level")]
    [InlineData("\t", "default_top_level")]
    [InlineData("MyProject", "default_boilerplate")]
    public void ApplicationConfigurationInitializeBuilder_GenerateInitialize_can_handle_namespace(string? ns, string expectedFileName)
    {
        string expected = File.ReadAllText($@"System\Windows\Forms\Generators\MockData\{GetType().Name}.{expectedFileName}.cs");

        string output = ApplicationConfigurationInitializeBuilder.GenerateInitialize(ns,
            new ApplicationConfig(
                EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                DefaultFont: null,
                HighDpiMode: PropertyDefaultValue.DpiMode,
                UseCompatibleTextRendering: PropertyDefaultValue.UseCompatibleTextRendering
            ));

        Assert.Equal(expected, output);
    }

    public static IEnumerable<object[]> GenerateInitializeData()
    {
        foreach (string cultureName in s_locales)
        {
            CultureInfo culture = new(cultureName);

            // EnableVisualStyles: false, true
            yield return new object[]
            {
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: false,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: PropertyDefaultValue.UseCompatibleTextRendering
                ),
                "EnableVisualStyles=false"
            };
            yield return new object[]
            {
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: true,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: PropertyDefaultValue.UseCompatibleTextRendering
                ),
                "EnableVisualStyles=true"
            };

            // UseCompatibleTextRendering: false, true
            yield return new object[]
            {
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: false
                ),
                "UseCompTextRendering=false"
            };
            yield return new object[]
            {
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: true
                ),
                "UseCompTextRendering=true"
            };

            // DefaultFont: null, FontDescriptor
            yield return new object[]
            {
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: false
                ),
                "DefaultFont=null"
            };
            yield return new object[]
            {
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: new FontDescriptor(string.Empty, 12, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Millimeter).ToString(),
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: true
                ),
                "DefaultFont=default"
            };
            yield return new object[]
            {
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: new FontDescriptor("Tahoma", 12, FontStyle.Regular, GraphicsUnit.Point).ToString(),
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: true
                ),
                "DefaultFont=Tahoma"
            };
            yield return new object[]
            {
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: new FontDescriptor("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point).ToString(),
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: true
                ),
                "DefaultFont=SansSerif"
            };
        }
    }

    [Theory]
    [MemberData(nameof(GenerateInitializeData))]
    public Task ApplicationConfigurationInitializeBuilder_GenerateInitialize(CultureInfo culture, /* ApplicationConfig */object config, string testName)
    {
        Thread.CurrentThread.CurrentCulture = culture;

        string output = ApplicationConfigurationInitializeBuilder.GenerateInitialize(null, (ApplicationConfig)config);

        // Compare all locale tests against the same files - we expect the produced output to be the same
        return Verifier.Verify(output)
            .UseMethodName("GenerateInitialize")
            .UseTextForParameters(testName)
            .DisableRequireUniquePrefix();
    }
}
