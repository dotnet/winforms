// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
        var result = (Keys?)converter.ConvertFrom(null, culture, localizedKeyName);

        Assert.Equal(expectedKey, result);
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
        var result = converter.ConvertToString(null, CultureInfo.InvariantCulture, keys);
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
        yield return new object[] { Keys.None, new Enum[] { Keys.None } };
        yield return new object[] { Keys.S, new Enum[] { Keys.S } };
        yield return new object[] { Keys.Control | Keys.C, new Enum[] { Keys.Control, Keys.C } };
        yield return new object[] { Keys.Control | Keys.Add, new Enum[] { Keys.Control, Keys.Add } };
        yield return new object[] { Keys.Control | Keys.Alt | Keys.D, new Enum[] { Keys.Control, Keys.Alt, Keys.D } };
        yield return new object[] { Keys.Control | Keys.Alt | Keys.Shift | Keys.A, new Enum[] { Keys.Control, Keys.Alt, Keys.Shift, Keys.A } };
        yield return new object[] { Keys.Control | Keys.Alt | Keys.Shift | Keys.F1, new Enum[] { Keys.Control, Keys.Alt, Keys.Shift, Keys.F1 } };
    }

    [Theory]
    [MemberData(nameof(ConvertToEnumArray_ShouldConvertKeys_TestData))]
    public void ConvertToEnumArray_ShouldConvertKeys(Keys keys, Enum[] expectedResult)
    {
        KeysConverter converter = new();
        var result = converter.ConvertTo(keys, typeof(Enum[]));
        Assert.Equal(expectedResult, result);
    }
}
