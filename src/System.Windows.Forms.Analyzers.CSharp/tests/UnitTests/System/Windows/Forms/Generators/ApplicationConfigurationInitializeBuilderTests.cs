// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.Analyzers;
using VerifyXunit;
using Xunit;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.Generators.Tests
{
    [UsesVerify]
    public partial class ApplicationConfigurationInitializeBuilderTests
    {
        [Theory]
        [InlineData(null, "default_top_level")]
        [InlineData("", "default_top_level")]
        [InlineData(" ", "default_top_level")]
        [InlineData("\t", "default_top_level")]
        [InlineData("MyProject", "default_boilerplate")]
        public void ApplicationConfigurationInitializeBuilder_GenerateInitialize_can_handle_namespace(string ns, string expectedFileName)
        {
            string expected = File.ReadAllText($@"System\Windows\Forms\Generators\MockData\{GetType().Name}.{expectedFileName}.cs");

            string output = ApplicationConfigurationInitializeBuilder.GenerateInitialize(ns,
                new ApplicationConfig
                {
                    DefaultFont = null,
                    EnableVisualStyles = PropertyDefaultValue.EnableVisualStyles,
                    HighDpiMode = PropertyDefaultValue.DpiMode,
                    UseCompatibleTextRendering = PropertyDefaultValue.UseCompatibleTextRendering
                });

            Assert.Equal(expected, output);
        }

        internal static TheoryData<(object, string)> GenerateInitializeData()
             => new()
             {
                 // EnableVisualStyles: false, true
                 (new ApplicationConfig
                 {
                     DefaultFont = null,
                     EnableVisualStyles = false,
                     HighDpiMode = PropertyDefaultValue.DpiMode,
                     UseCompatibleTextRendering = PropertyDefaultValue.UseCompatibleTextRendering
                 }, "EnableVisualStyles=false"),
                 (new ApplicationConfig
                 {
                     DefaultFont = null,
                     EnableVisualStyles = true,
                     HighDpiMode = PropertyDefaultValue.DpiMode,
                     UseCompatibleTextRendering = PropertyDefaultValue.UseCompatibleTextRendering
                 }, "EnableVisualStyles=true"),

                 // UseCompatibleTextRendering: false, true
                 (new ApplicationConfig
                 {
                     DefaultFont = null,
                     EnableVisualStyles = PropertyDefaultValue.EnableVisualStyles,
                     HighDpiMode = PropertyDefaultValue.DpiMode,
                     UseCompatibleTextRendering = false
                 }, "UseCompTextRendering=false"),
                 (new ApplicationConfig
                 {
                     DefaultFont = null,
                     EnableVisualStyles = PropertyDefaultValue.EnableVisualStyles,
                     HighDpiMode = PropertyDefaultValue.DpiMode,
                     UseCompatibleTextRendering = true
                 }, "UseCompTextRendering=true"),

                 // DefaultFont: null, FontDescriptor
                 (new ApplicationConfig
                 {
                     DefaultFont = null,
                     EnableVisualStyles = PropertyDefaultValue.EnableVisualStyles,
                     HighDpiMode = PropertyDefaultValue.DpiMode,
                     UseCompatibleTextRendering = false
                 }, "DefaultFont=null"),
                 (new ApplicationConfig
                 {
                     DefaultFont = new FontDescriptor(string.Empty, 12, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Millimeter),
                     EnableVisualStyles = PropertyDefaultValue.EnableVisualStyles,
                     HighDpiMode = PropertyDefaultValue.DpiMode,
                     UseCompatibleTextRendering = true
                 }, "DefaultFont=default"),
                 (new ApplicationConfig
                 {
                     DefaultFont = new FontDescriptor("Tahoma", 12, FontStyle.Regular, GraphicsUnit.Point),
                     EnableVisualStyles = PropertyDefaultValue.EnableVisualStyles,
                     HighDpiMode = PropertyDefaultValue.DpiMode,
                     UseCompatibleTextRendering = true
                 }, "DefaultFont=Tahoma"),
             };

        [Theory]
        [MemberData(nameof(GenerateInitializeData))]
        public Task ApplicationConfigurationInitializeBuilder_GenerateInitialize((/* ApplicationConfig */object config, string testName) data)
        {
            string output = ApplicationConfigurationInitializeBuilder.GenerateInitialize(null, (ApplicationConfig)data.config);

            return Verifier.Verify(output)
                .UseMethodName("GenerateInitialize")
                .UseTextForParameters(data.testName);
        }
    }
}
