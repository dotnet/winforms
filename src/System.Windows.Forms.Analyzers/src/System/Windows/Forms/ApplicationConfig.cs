// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Analyzers
{
    internal partial class ApplicationConfig
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

        public bool EnableVisualStyles { get; set; }
        public FontDescriptor? DefaultFont { get; set; }
        public HighDpiMode HighDpiMode { get; set; }
        public bool UseCompatibleTextRendering { get; set; }
    }
}
