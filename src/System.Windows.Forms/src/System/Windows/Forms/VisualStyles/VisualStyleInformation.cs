// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope = "namespace", Target = "System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles
{

    using System;
    using System.Text;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;
    using System.Diagnostics.CodeAnalysis;


    /// <summary>
    ///    <para> 
    ///         Provides information about the current visual style. 
    ///         NOTE: 
    ///         1) These properties (except SupportByOS, which is always meaningful) are meaningful only 
    ///            if visual styles are supported and have currently been applied by the user.
    ///         2) A subset of these use VisualStyleRenderer objects, so they are
    ///            not meaningful unless VisualStyleRenderer.IsSupported is true.
    ///   </para>
    /// </summary>
    public static class VisualStyleInformation
    {

        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer visualStyleRenderer = null;

        /// <summary>
        ///    <para>
        ///       Used to find whether visual styles are supported by the current OS. Same as 
        ///       using the OSFeature class to see if themes are supported.
        ///    </para>
        /// </summary>
        public static bool IsSupportedByOS
        {
            get
            {
                return (OSFeature.Feature.IsPresent(OSFeature.Themes));
            }
        }

        /// <summary>
        ///    <para> 
        ///     Returns true if a visual style has currently been applied by the user, else false.
        ///    </para>
        /// </summary>
        public static bool IsEnabledByUser
        {
            get
            {
                if (!IsSupportedByOS)
                {
                    return false;
                }

                return (SafeNativeMethods.IsAppThemed());
            }
        }

        internal static string ThemeFilename
        {
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder filename = new StringBuilder(512);
                    SafeNativeMethods.GetCurrentThemeName(filename, filename.Capacity, null, 0, null, 0);
                    return (filename.ToString());
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    The current visual style's color scheme name.
        /// </summary>
        public static string ColorScheme
        {
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder colorScheme = new StringBuilder(512);
                    SafeNativeMethods.GetCurrentThemeName(null, 0, colorScheme, colorScheme.Capacity, null, 0);
                    return (colorScheme.ToString());
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    The current visual style's size name.
        /// </summary>
        public static string Size
        {
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder size = new StringBuilder(512);
                    SafeNativeMethods.GetCurrentThemeName(null, 0, null, 0, size, size.Capacity);
                    return (size.ToString());
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    The current visual style's display name.
        /// </summary>
        public static string DisplayName
        {
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder name = new StringBuilder(512);
                    SafeNativeMethods.GetThemeDocumentationProperty(ThemeFilename, SafeNativeMethods.VisualStyleDocProperty.DisplayName, name, name.Capacity);
                    return name.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    The current visual style's company.
        /// </summary>
        public static string Company
        {
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder company = new StringBuilder(512);
                    SafeNativeMethods.GetThemeDocumentationProperty(ThemeFilename, SafeNativeMethods.VisualStyleDocProperty.Company, company, company.Capacity);
                    return company.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    The name of the current visual style's author.
        /// </summary>
        public static string Author
        {
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder author = new StringBuilder(512);
                    SafeNativeMethods.GetThemeDocumentationProperty(ThemeFilename, SafeNativeMethods.VisualStyleDocProperty.Author, author, author.Capacity);
                    return author.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    The current visual style's copyright information.
        /// </summary>
        public static string Copyright
        {
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder copyright = new StringBuilder(512);
                    SafeNativeMethods.GetThemeDocumentationProperty(ThemeFilename, SafeNativeMethods.VisualStyleDocProperty.Copyright, copyright, copyright.Capacity);
                    return copyright.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    The current visual style's url.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public static string Url
        {
            [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings")]
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder url = new StringBuilder(512);
                    SafeNativeMethods.GetThemeDocumentationProperty(ThemeFilename, SafeNativeMethods.VisualStyleDocProperty.Url, url, url.Capacity);
                    return url.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    The current visual style's version.
        /// </summary>
        public static string Version
        {
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder version = new StringBuilder(512);
                    SafeNativeMethods.GetThemeDocumentationProperty(ThemeFilename, SafeNativeMethods.VisualStyleDocProperty.Version, version, version.Capacity);
                    return version.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    The current visual style's description.
        /// </summary>
        public static string Description
        {
            get
            {
                if (IsEnabledByUser)
                {
                    StringBuilder description = new StringBuilder(512);
                    SafeNativeMethods.GetThemeDocumentationProperty(ThemeFilename, SafeNativeMethods.VisualStyleDocProperty.Description, description, description.Capacity);
                    return description.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///    Returns true if the current theme supports flat menus, else false.
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
        ///    The minimum color depth supported by the current visual style.
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
        ///    Border Color that Windows renders for controls like TextBox and ComboBox.
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
        ///    This is the color buttons and tab pages are highlighted with when they are moused over on themed OS.
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
