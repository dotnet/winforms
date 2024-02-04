// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Globalization;

namespace System.Windows.Forms.Tests;

public class KeysConverterTests
{
    [Theory]
    [UseDefaultXunitCulture]
    [InlineData("Ctrl+Alt+Shift+A", Keys.Control | Keys.Alt | Keys.Shift | Keys.A)]
    [InlineData("Ctrl+Alt+Shift+F1", Keys.Control | Keys.Alt | Keys.Shift | Keys.F1)]
    [InlineData("Ctrl+Alt+D", Keys.Control | Keys.Alt | Keys.D)]
    [InlineData("Ctrl + N", Keys.Control | Keys.N)]
    [InlineData("G", Keys.G)]
    [InlineData("None", Keys.None)]
    public void ConvertFrom_ShouldConvertKeys(string input, Keys keys)
    {
        KeysConverter converter = new();
        var result = (Keys)converter.ConvertFrom(input);
        Assert.Equal(keys, result);
    }

    [Theory]
    [InlineData("fr-FR", "(aucun)", Keys.None)]
    [InlineData("nb-NO", "None", Keys.None)]
    [InlineData("de-DE", "Ende", Keys.End)]
    public void ConvertFrom_ShouldConvertKeys_Localization(string cultureName, string localizedKeyName, Keys expectedKey)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
        KeysConverter converter = new();

        // The 'localizedKeyName' is converted into the corresponding key value according to the specified culture.
        var resultFromSpecificCulture = (Keys?)converter.ConvertFrom(context: null, culture, localizedKeyName);
        Assert.Equal(expectedKey, resultFromSpecificCulture);

        // Record original UI culture.
        CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = culture;

            // When the culture is empty, the 'localizedKeyName' is converted to the corresponding key value based on CurrentUICulture.
            var resultFromUICulture = (Keys?)converter.ConvertFrom(context: null, culture: null, localizedKeyName);
            Assert.Equal(expectedKey, resultFromUICulture);
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }
    }

    [Theory]
    [InlineData(Keys.None, "(none)")]
    [InlineData(Keys.S, "S")]
    [InlineData(Keys.Control | Keys.C, "Ctrl+C")]
    [InlineData(Keys.Control | Keys.Add, "Ctrl+Add")]
    [InlineData(Keys.Control | Keys.Alt | Keys.D, "Ctrl+Alt+D")]
    [InlineData(Keys.Control | Keys.Alt | Keys.Shift | Keys.A, "Ctrl+Alt+Shift+A")]
    [InlineData(Keys.Control | Keys.Alt | Keys.Shift | Keys.F1, "Ctrl+Alt+Shift+F1")]
    [InlineData(Keys.F2 | Keys.Shift | Keys.Alt | Keys.Control, "Ctrl+Alt+Shift+F2")]
    public void ConvertToString_ShouldConvertKeys(Keys keys, string expectedResult)
    {
        KeysConverter converter = new();
        string result = converter.ConvertToString(null, CultureInfo.InvariantCulture, keys);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("fr-FR", Keys.None, "(aucun)")]
    [InlineData("de-DE", Keys.End, "Ende")]
    public void ConvertToString_ShouldConvertKeys_Localization(string cultureName, Keys key, string expectedLocalizedKeyName)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);

        KeysConverter converter = new();
        string result = converter.ConvertToString(null, culture, key);

        Assert.Equal(expectedLocalizedKeyName, result);
    }

    public static IEnumerable<object[]> ConvertToEnumArray_ShouldConvertKeys_TestData()
    {
        yield return new object[] { "fr-FR", Keys.None, new Enum[] { Keys.None } };
        yield return new object[] { "de-DE", Keys.S, new Enum[] { Keys.S } };
        yield return new object[] { "zh-CN", Keys.Control | Keys.C, new Enum[] { Keys.Control, Keys.C } };
        yield return new object[] { "it-IT", Keys.Control | Keys.Add, new Enum[] { Keys.Control, Keys.Add } };
        yield return new object[] { "ko-KR", Keys.Control | Keys.Alt | Keys.D, new Enum[] { Keys.Control, Keys.Alt, Keys.D } };
        yield return new object[] { "ru-RU", Keys.Control | Keys.Alt | Keys.Shift | Keys.A, new Enum[] { Keys.Control, Keys.Alt, Keys.Shift, Keys.A } };
        yield return new object[] { "zh-TW", Keys.Control | Keys.Alt | Keys.Shift | Keys.F1, new Enum[] { Keys.Control, Keys.Alt, Keys.Shift, Keys.F1 } };
    }

    [Theory]
    [MemberData(nameof(ConvertToEnumArray_ShouldConvertKeys_TestData))]
    public void ConvertToEnumArray_ShouldConvertKeys(string cultureName, Keys keys, Enum[] expectedResult)
    {
        KeysConverter converter = new();
        object result = converter.ConvertTo(keys, typeof(Enum[]));
        Assert.Equal(expectedResult, result);

        CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);

            object resultWithoutCulture = converter.ConvertTo(context: null, culture: null, keys, typeof(Enum[]));
            object resultWithUICulture = converter.ConvertTo(context: null, culture: Thread.CurrentThread.CurrentUICulture, keys, typeof(Enum[]));

            Assert.Equal(expectedResult, resultWithoutCulture);
            Assert.Equal(resultWithUICulture, resultWithoutCulture);
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("de-DE", null)]
    [InlineData(null, "de-DE")]
    [InlineData("fr-FR", "en-US")]
    [InlineData("en-US", "fr-FR")]
    [InlineData("zh-CN", "it-IT")]
    [InlineData("it-IT", "zh-CN")]
    [InlineData("ko-KR", "ru-RU")]
    [InlineData("ru-RU", "ko-KR")]
    [InlineData("zh-TW", "cs-CZ")]
    [InlineData("cs-CZ", "zh-TW")]
    [InlineData("ja-JP", "en-US")]
    public void GetStandardValues(string cultureName, string uiCultureName)
    {
        // Record original culture
        CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
        CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;

        // Update CurrentCulture
        Thread.CurrentThread.CurrentCulture = cultureName is not null ? CultureInfo.GetCultureInfo(cultureName) : originalCulture;
        Thread.CurrentThread.CurrentUICulture = uiCultureName is not null ? CultureInfo.GetCultureInfo(uiCultureName) : originalUICulture;

        Keys[] expectedValues =
        [
            Keys.None, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Alt, Keys.Back, Keys.Control,
            Keys.Delete, Keys.End, Keys.Enter, Keys.F1, Keys.F10, Keys.F11, Keys.F12, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8,
            Keys.F9, Keys.Home, Keys.Insert, Keys.Next, Keys.PageUp, Keys.Shift
        ];

        try
        {
            var standardValuesCollection = new KeysConverter().GetStandardValues();
            object[] actualValues = new ArrayList(standardValuesCollection).ToArray();

            Assert.Equal(expectedValues.Length, standardValuesCollection.Count);

            foreach (object key in expectedValues)
            {
                Assert.Contains(key, actualValues);
            }
        }
        finally
        {
            // Restore original Culture
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }
    }
}
