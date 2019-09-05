// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.VisualStyles
{
    /// <summary>
    ///  Provides information about the current visual style.
    ///  
    ///  NOTE:
    ///  
    ///  1) These properties (except SupportByOS, which is always meaningful) are meaningful only
    ///  if visual styles are supported and have currently been applied by the user.
    ///  2) A subset of these use VisualStyleRenderer objects, so they are
    ///  not meaningful unless VisualStyleRenderer.IsSupported is true.
    /// </summary>
    public static class VisualStyleInformation
    {
        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer visualStyleRenderer = null;

        /// <summary>
        ///  Used to find whether visual styles are supported by the current OS. Same as
        ///  using the OSFeature class to see if themes are supported.
        ///  This is always supported on platforms that .NET Core supports.
        /// </summary>
        public static bool IsSupportedByOS => true;

        /// <summary>
        ///  Returns true if a visual style has currently been applied by the user, else false.
        /// </summary>
        public static bool IsEnabledByUser => SafeNativeMethods.IsAppThemed();

        internal static unsafe string ThemeFilename
        {
            get
            {
                if (IsEnabledByUser)
                {
                    Span<char> filename = stackalloc char[512];
                    fixed (char* pFilename = filename)
                    {
                        UxTheme.GetCurrentThemeName(pFilename, filename.Length, null, 0, null, 0);
                    }

                    return filename.SliceAtFirstNull().ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  The current visual style's color scheme name.
        /// </summary>
        public static unsafe string ColorScheme
        {
            get
            {
                if (IsEnabledByUser)
                {
                    Span<char> colorScheme = stackalloc char[512];
                    fixed (char* pColorScheme = colorScheme)
                    {
                        UxTheme.GetCurrentThemeName(null, 0, pColorScheme, colorScheme.Length, null, 0);
                    }

                    return colorScheme.SliceAtFirstNull().ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  The current visual style's size name.
        /// </summary>
        public static unsafe string Size
        {
            get
            {
                if (IsEnabledByUser)
                {
                    Span<char> size = stackalloc char[512];
                    fixed (char* pSize = size)
                    {
                        UxTheme.GetCurrentThemeName(null, 0, null, 0, pSize, size.Length);
                    }

                    return size.SliceAtFirstNull().ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  The current visual style's display name.
        /// </summary>
        public static unsafe string DisplayName
        {
            get
            {
                if (IsEnabledByUser)
                {
                    return UxTheme.GetThemeDocumentationProperty(ThemeFilename, UxTheme.VisualStyleDocProperty.DisplayName);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  The current visual style's company.
        /// </summary>
        public static string Company
        {
            get
            {
                if (IsEnabledByUser)
                {
                    return UxTheme.GetThemeDocumentationProperty(ThemeFilename, UxTheme.VisualStyleDocProperty.Company);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  The name of the current visual style's author.
        /// </summary>
        public static string Author
        {
            get
            {
                if (IsEnabledByUser)
                {
                    return UxTheme.GetThemeDocumentationProperty(ThemeFilename, UxTheme.VisualStyleDocProperty.Author);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  The current visual style's copyright information.
        /// </summary>
        public static string Copyright
        {
            get
            {
                if (IsEnabledByUser)
                {
                    return UxTheme.GetThemeDocumentationProperty(ThemeFilename, UxTheme.VisualStyleDocProperty.Copyright);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  The current visual style's url.
        /// </summary>
        public static string Url
        {
            get
            {
                if (IsEnabledByUser)
                {
                    return UxTheme.GetThemeDocumentationProperty(ThemeFilename, UxTheme.VisualStyleDocProperty.Url);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  The current visual style's version.
        /// </summary>
        public static string Version
        {
            get
            {
                if (IsEnabledByUser)
                {
                    return UxTheme.GetThemeDocumentationProperty(ThemeFilename, UxTheme.VisualStyleDocProperty.Version);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  The current visual style's description.
        /// </summary>
        public static string Description
        {
            get
            {
                if (IsEnabledByUser)
                {
                    return UxTheme.GetThemeDocumentationProperty(ThemeFilename, UxTheme.VisualStyleDocProperty.Description);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///  Returns true if the current theme supports flat menus, else false.
        /// </summary>
        public static bool SupportsFlatMenus
        {
            get
            {
                if (Application.RenderWithVisualStyles)
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);
                    }
                    else
                    {
                        visualStyleRenderer.SetParameters(VisualStyleElement.Window.Caption.Active);
                    }

                    return (SafeNativeMethods.GetThemeSysBool(new HandleRef(null, visualStyleRenderer.Handle), SafeNativeMethods.VisualStyleSystemProperty.SupportsFlatMenus));
                }

                return false;
            }
        }

        /// <summary>
        ///  The minimum color depth supported by the current visual style.
        /// </summary>
        public static int MinimumColorDepth
        {
            get
            {
                if (Application.RenderWithVisualStyles)
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);
                    }
                    else
                    {
                        visualStyleRenderer.SetParameters(VisualStyleElement.Window.Caption.Active);
                    }

                    int mcDepth = 0;

                    SafeNativeMethods.GetThemeSysInt(new HandleRef(null, visualStyleRenderer.Handle), SafeNativeMethods.VisualStyleSystemProperty.MinimumColorDepth, ref mcDepth);
                    return mcDepth;
                }

                return 0;
            }
        }

        /// <summary>
        ///  Border Color that Windows renders for controls like TextBox and ComboBox.
        /// </summary>
        public static Color TextControlBorder
        {
            get
            {
                if (Application.RenderWithVisualStyles)
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Normal);
                    }
                    else
                    {
                        visualStyleRenderer.SetParameters(VisualStyleElement.TextBox.TextEdit.Normal);
                    }
                    Color borderColor = visualStyleRenderer.GetColor(ColorProperty.BorderColor);
                    return borderColor;
                }

                return SystemColors.WindowFrame;
            }
        }

        /// <summary>
        ///  This is the color buttons and tab pages are highlighted with when they are moused over on themed OS.
        /// </summary>
        public static Color ControlHighlightHot
        {
            get
            {
                if (Application.RenderWithVisualStyles)
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);

                    }
                    else
                    {
                        visualStyleRenderer.SetParameters(VisualStyleElement.Button.PushButton.Normal);
                    }
                    Color accentColor = visualStyleRenderer.GetColor(ColorProperty.AccentColorHint);
                    return accentColor;
                }

                return SystemColors.ButtonHighlight;
            }
        }
    }
}
