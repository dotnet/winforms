// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

public class DataFormatNamesTests
{
    [Theory]
    [InlineData(DataFormatNames.Text, true)]
    [InlineData(DataFormatNames.UnicodeText, true)]
    [InlineData(DataFormatNames.Dib, true)]
    [InlineData(DataFormatNames.Bitmap, true)]
    public void RestrictDeserializationToSafeTypes(string format, bool expected)
    {
        Assert.Equal(expected, DataFormatNames.IsPredefinedFormat(format));
    }

    [Theory]
    [InlineData(DataFormatNames.Emf, true)]
    [InlineData(DataFormatNames.Text, true)]
    [InlineData(DataFormatNames.UnicodeText, true)]
    public void IsRestrictedFormat(string format, bool expected)
    {
        Assert.Equal(expected, DataFormatNames.IsPredefinedFormat(format));
    }
}
