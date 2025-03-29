// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms;

public partial class KeysConverterTests
{
    // This test modifies global culture settings (Thread.CurrentUICulture and SR.Culture).
    // As a result, it must run sequentially to avoid conflicts with other tests that may
    // rely on or alter these settings. Running it in a sequential collection ensures consistent results.
    [Collection("Sequential")]
    public class LocalizationTests
    {
        [Theory]
        [InlineData("fr-FR", "toStringNone", Keys.None)]
        [InlineData("zh-CN", "toStringNone", Keys.None)]
        [InlineData("nb-NO", "toStringNone", Keys.None)]
        [InlineData("de-DE", "toStringEnd", Keys.End)]
        public void ConvertFrom_ShouldConvertKeys_Localization(string cultureName, string resourceKey, Keys expectedKey)
        {
            // Record original culture.
            CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;
            CultureInfo originalSRCulture = SR.Culture;

            CultureInfo targetCulture = CultureInfo.GetCultureInfo(cultureName);

            try
            {
                // Use the SR resource key to retrieve the localized string corresponding to SR.Culture.
                SR.Culture = targetCulture;
                string localizedString = SR.GetResourceString(resourceKey);

                KeysConverter converter = new();
                var resultFromSpecificCulture = (Keys?)converter.ConvertFrom(context: null, targetCulture, localizedString);

                Assert.Equal(expectedKey, resultFromSpecificCulture);

                Thread.CurrentThread.CurrentUICulture = targetCulture;

                // When the culture is empty, the 'localizedString' is converted
                // to the corresponding key value based on CurrentUICulture.
                var resultFromUICulture = (Keys?)converter.ConvertFrom(context: null, culture: null, localizedString);
                Assert.Equal(expectedKey, resultFromUICulture);
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = originalUICulture;
                SR.Culture = originalSRCulture;
            }
        }

        [Theory]
        [InlineData("fr-FR", Keys.None, "toStringNone")]
        [InlineData("de-DE", Keys.End, "toStringEnd")]
        public void ConvertToString_ShouldConvertKeys_Localization(string cultureName, Keys key, string resourceKey)
        {
            CultureInfo originalSRCulture = SR.Culture;
            try
            {
                CultureInfo targetCulture = CultureInfo.GetCultureInfo(cultureName);

                KeysConverter converter = new();
                string? result = converter.ConvertToString(null, targetCulture, key);

                SR.Culture = targetCulture;
                Assert.Equal(SR.GetResourceString(resourceKey), result);
            }
            finally
            {
                SR.Culture = originalSRCulture;
            }
        }
    }
}
