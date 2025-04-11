// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Analyzers;

internal partial record ApplicationConfig(
    bool EnableVisualStyles,
    string? DefaultFont,
    HighDpiMode HighDpiMode,
    bool UseCompatibleTextRendering)
{
    public static class PropertyNameCSharp
    {
        public const string EnableVisualStyles = "ApplicationVisualStyles";
        public const string DefaultFont = "ApplicationDefaultFont";
        public const string HighDpiMode = "ApplicationHighDpiMode";
        public const string UseCompatibleTextRendering = "ApplicationUseCompatibleTextRendering";
    }

    public static class PropertyNameVisualBasic
    {
        public const string EnableVisualStyles = "EnableVisualStyles";
        public const string HighDpiMode = "HighDpiMode";
    }

    public static class PropertyDefaultValue
    {
        public const bool EnableVisualStyles = true;
        public const float FontSize = 9f;
        public const HighDpiMode DpiMode = HighDpiMode.SystemAware;
        public const bool UseCompatibleTextRendering = false;
    }
}
