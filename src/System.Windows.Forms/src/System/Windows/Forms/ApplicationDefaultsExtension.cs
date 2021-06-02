// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.Drawing;
using static System.Windows.Forms.Application;

namespace System.Windows.Forms
{
    public static class ApplicationDefaultsExtension
    {
        private const string DefaultFont = nameof(DefaultFont);
        private const string DefaultFontScaled = nameof(DefaultFontScaled);

        /// <summary>
        /// Scale the default font (if it is set) as per the Settings display text scale settings.
        /// </summary>
        internal static void ScaleFont(ApplicationDefaults defaults)
        {
            Font defaultFont, defaultFontScaled = null;
            defaultFont = defaults.GetValueOrDefault<Font>(DefaultFont);

            if (defaultFont is null || !OsVersion.IsWindows10_1507OrGreater)
            {
                return;
            }

            defaultFontScaled = defaults.GetValueOrDefault<Font>(DefaultFontScaled);
            float textScaleValue = DpiHelper.GetTextScaleFactor();

            if (defaultFontScaled is not null)
            {
                defaultFontScaled.Dispose();
                defaultFontScaled = null;
            }

            // Restore the text scale if it isn't the default value in the valid text scale factor value
            if (textScaleValue > 1.0f)
            {
                defaultFontScaled = new Font(defaultFont.FontFamily, defaultFont.Size * textScaleValue);
            }

            defaults.TryAddDefaultValue(DefaultFontScaled, defaultFontScaled);
        }

        internal static void ResetFont(this ApplicationDefaults defaults)
        {
            Font defaultFont, defaultFontScaled = null;

            defaultFont = defaults.GetValueOrDefault<Font>(DefaultFont);

            if (defaultFont is { })
            {
                defaultFont.Dispose();
                defaults.Remove(DefaultFont);

                defaultFontScaled = defaults.GetValueOrDefault<Font>(DefaultFontScaled);
                defaultFontScaled.Dispose();
                defaults.Remove(DefaultFontScaled);
            }
        }

        internal static Font GetFont(this ApplicationDefaults defaults)
            => defaults.GetValueOrDefault<Font>(DefaultFontScaled)
                    ?? defaults.GetValueOrDefault<Font>(DefaultFont);

        /// <summary>
        ///  Sets the default <see cref="Font"/> for process.
        /// </summary>
        /// <param name="font">The font to be used as a default across the application.</param>
        /// <exception cref="ArgumentNullException"><paramref name="font"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// You can only call this method before the first window is created by your Windows Forms application.
        /// </exception>
        /// <remarks>
        ///  <para>
        ///    The system text scale factor will be applied to the font, i.e. if the default font is set to "Calibri, 11f"
        ///    and the text scale factor is set to 150% the resulting default font will be set to "Calibri, 16.5f".
        ///  </para>
        ///  <para>
        ///    Users can adjust text scale with the Make text bigger slider on the Settings -> Ease of Access -> Vision/Display screen.
        ///  </para>
        /// </remarks>
        /// <seealso href="https://docs.microsoft.com/windows/uwp/design/input/text-scaling">Windows Text scaling</seealso>
        public static ApplicationDefaults SetFont(this ApplicationDefaults defaults, Font font)
        {
            Font defaultFont, defaultFontScaled = null;

            if (font is null)
                throw new ArgumentNullException(nameof(font));

            if (NativeWindow.AnyHandleCreated)
                throw new InvalidOperationException(string.Format(SR.Win32WindowAlreadyCreated, nameof(SetFont)));

            // If user made a prior call to this API with a different custom fonts, we want to clean it up.
            defaultFont = defaults.GetValueOrDefault<Font>(DefaultFont);

            if (defaultFont is not null)
            {
                defaultFont?.Dispose();
                defaultFont = null;

                defaultFontScaled = defaults.GetValueOrDefault<Font>(DefaultFontScaled);
                defaultFontScaled?.Dispose();
                defaultFontScaled = null;
            }

            if (font.IsSystemFont)
            {
                // The system font is managed the .NET runtime, and it is already scaled to the current text scale factor.
                // We need to clone it because our reference will no longer be scaled by the .NET runtime.
                defaultFont = (Font)font.Clone();
            }
            else
            {
                defaultFont = font;
                ScaleFont(defaults);
            }

            defaults.TryAddDefaultValue(DefaultFont, defaultFont);

            return defaults;
        }
    }
}
