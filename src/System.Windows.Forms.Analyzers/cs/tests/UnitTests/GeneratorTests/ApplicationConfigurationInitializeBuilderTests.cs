// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Windows.Forms.CSharp.Generators.ApplicationConfiguration;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.Analyzers.Tests;

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
        string expected = File.ReadAllText($@"Generators\MockData\{GetType().Name}.{expectedFileName}.cs");

        string output = ApplicationConfigurationInitializeBuilder.GenerateInitialize(ns,
            new ApplicationConfig(
                EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                DefaultFont: null,
                HighDpiMode: PropertyDefaultValue.DpiMode,
                UseCompatibleTextRendering: PropertyDefaultValue.UseCompatibleTextRendering
            ));

        Assert.Equal(expected, output);
    }

    public static TheoryData<CultureInfo, object, string> GenerateInitializeData()
    {
        TheoryData<CultureInfo, object, string> testData = new();

        foreach (string cultureName in s_locales)
        {
            CultureInfo culture = new(cultureName);

            // EnableVisualStyles: false, true
            testData.Add(
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: false,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: PropertyDefaultValue.UseCompatibleTextRendering
                ),
                "EnableVisualStyles=false"
            );

            testData.Add(
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: true,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: PropertyDefaultValue.UseCompatibleTextRendering
                ),
                "EnableVisualStyles=true"
            );

            // UseCompatibleTextRendering: false, true
            testData.Add(
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: false
                ),
                "UseCompTextRendering=false"
            );

            testData.Add(
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: true
                ),
                "UseCompTextRendering=true"
            );

            // DefaultFont: null, FontDescriptor
            testData.Add(
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: null,
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: false
                ),
                "DefaultFont=null"
            );

            testData.Add(
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: new FontDescriptor(string.Empty, 12, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Millimeter).ToString(),
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: true
                ),
                "DefaultFont=default"
            );

            testData.Add(
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: new FontDescriptor("Tahoma", 12, FontStyle.Regular, GraphicsUnit.Point).ToString(),
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: true
                ),
                "DefaultFont=Tahoma"
            );

            testData.Add(
                culture,
                new ApplicationConfig(
                    EnableVisualStyles: PropertyDefaultValue.EnableVisualStyles,
                    DefaultFont: new FontDescriptor("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point).ToString(),
                    HighDpiMode: PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering: true
                ),
                "DefaultFont=SansSerif"
            );
        }

        return testData;
    }

    [Theory]
    [MemberData(nameof(GenerateInitializeData))]
    public Task ApplicationConfigurationInitializeBuilder_GenerateInitialize(CultureInfo culture, /* ApplicationConfig */object config, string testName)
    {
        Thread.CurrentThread.CurrentCulture = culture;

        string output = ApplicationConfigurationInitializeBuilder.GenerateInitialize(null, (ApplicationConfig)config);

        // Compare all locale tests against the same files - we expect the produced output to be the same
        return Verify(output)
            .UseMethodName("GenerateInitialize")
            .UseTextForParameters(testName)
            .DisableRequireUniquePrefix();
    }
}
