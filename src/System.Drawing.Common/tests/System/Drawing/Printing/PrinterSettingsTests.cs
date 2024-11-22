// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Authors:
//  Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Drawing.Imaging;
using System.Globalization;

namespace System.Drawing.Printing.Tests;

public class PrinterSettingsTests
{
    [Fact]
    public void Ctor_Default_Success()
    {
        PrinterSettings printerSettings = new();
        Assert.NotNull(printerSettings.DefaultPageSettings);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void CanDuplex_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        _ = printerSettings.CanDuplex;
    }

    [Fact]
    public void Copies_Default_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        _ = printerSettings.Copies;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(short.MaxValue)]
    public void Copies_SetValue_ReturnsExpected(short copies)
    {
        PrinterSettings printerSettings = new()
        {
            Copies = copies
        };

        Assert.Equal(copies, printerSettings.Copies);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(short.MinValue)]
    public void Copies_SetValue_ThrowsArgumentException(short copies)
    {
        PrinterSettings printerSettings = new();
        AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.Copies = copies);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void Collate_Default_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        _ = printerSettings.Collate;
    }

    [Fact]
    public void Collate_SetValue_ReturnsExpected()
    {
        PrinterSettings printerSettings = new()
        {
            Collate = false
        };

        Assert.False(printerSettings.Collate);
    }

    [Fact]
    public void DefaultPageSettings_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        Assert.NotNull(printerSettings.DefaultPageSettings);
    }

    [Theory]
    [InlineData(Duplex.Simplex)]
    [InlineData(Duplex.Vertical)]
    [InlineData(Duplex.Horizontal)]
    public void Duplex_SetValue_ReturnsExpected(Duplex duplex)
    {
        PrinterSettings printerSettings = new()
        {
            Duplex = duplex
        };

        Assert.Equal(duplex, printerSettings.Duplex);
    }

    [Theory]
    [InlineData(Duplex.Default - 1)]
    [InlineData(Duplex.Horizontal + 1)]
    [InlineData((Duplex)int.MaxValue)]
    [InlineData((Duplex)int.MinValue)]
    public void Duplex_Invalid_ThrowsInvalidEnumArgumentException(Duplex duplex)
    {
        PrinterSettings printerSettings = new();
        Assert.ThrowsAny<ArgumentException>(() => printerSettings.Duplex = duplex);
    }

    [Fact]
    public void FromPage_Default_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();

        Assert.Equal(0, printerSettings.FromPage);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void FromPage_SetValue_ReturnsExpected(int pageNumber)
    {
        PrinterSettings printerSettings = new()
        {
            FromPage = pageNumber
        };

        Assert.Equal(pageNumber, printerSettings.FromPage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void FromPage_Invalid_ThrowsArgumentException(int pageNumber)
    {
        PrinterSettings printerSettings = new();
        AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.FromPage = pageNumber);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void Static_InstalledPrinters_ReturnsExpected()
    {
        Assert.NotNull(Helpers.InstalledPrinters);
    }

    [Fact]
    public void IsDefaultPrinter_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        Assert.True(printerSettings.IsDefaultPrinter);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void IsPlotter_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        Assert.False(printerSettings.IsPlotter);
    }

    [Fact]
    public void IsValid_ReturnsExpected()
    {
        PrinterSettings printerSettings = new()
        {
            PrinterName = "Invalid Printer"
        };

        Assert.False(printerSettings.IsValid);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void LandscapeAngle_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        int[] validValues = [-90, 0, 90, 270];
        Assert.True(validValues.Contains(printerSettings.LandscapeAngle), $"PrinterSettings.LandscapeAngle ({printerSettings.LandscapeAngle}) must be 0, 90, or 270 degrees.");
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void MaximumCopies_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        Assert.True(printerSettings.MaximumCopies >= 0, $"PrinterSettings.MaximumCopies ({printerSettings.MaximumCopies}) should not be negative.");
    }

    [Fact]
    public void MaximumPage_Default_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();

        Assert.Equal(9999, printerSettings.MaximumPage);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(int.MaxValue)]
    public void MaximumPage_SetValue_ReturnsExpected(int maximumPage)
    {
        PrinterSettings printerSettings = new()
        {
            MaximumPage = maximumPage
        };

        Assert.Equal(maximumPage, printerSettings.MaximumPage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void MaximumPage_Invalid_ThrowsArgumentException(int maximumPage)
    {
        PrinterSettings printerSettings = new();
        AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.MaximumPage = maximumPage);
    }

    [Fact]
    public void MinimumPage_Default_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        Assert.Equal(0, printerSettings.MinimumPage);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(int.MaxValue)]
    public void MinimumPage_SetValue_ReturnsExpected(int minimumPage)
    {
        PrinterSettings printerSettings = new()
        {
            MinimumPage = minimumPage
        };

        Assert.Equal(minimumPage, printerSettings.MinimumPage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void MinimumPage_Invalid_ThrowsArgumentException(int minimumPage)
    {
        PrinterSettings printerSettings = new();
        AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.MinimumPage = minimumPage);
    }

    [Fact]
    public void PrintFileName_SetValue_ReturnsExpected()
    {
        string printFileName = "fileName";
        PrinterSettings printerSettings = new()
        {
            PrintFileName = printFileName
        };

        Assert.Equal(printFileName, printerSettings.PrintFileName);
    }

    [Fact]
    public void PrintFileName_Null_ThrowsArgumentNullException()
    {
        PrinterSettings printerSettings = new();
        AssertExtensions.Throws<ArgumentNullException>(null, () => printerSettings.PrintFileName = null);
    }

    [Fact]
    public void PrintFileName_Empty_ThrowsArgumentNullException()
    {
        PrinterSettings printerSettings = new();
        AssertExtensions.Throws<ArgumentNullException>(string.Empty, () => printerSettings.PrintFileName = string.Empty);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void PaperSizes_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        Assert.NotNull(printerSettings.PaperSizes);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void PaperSources_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        Assert.NotNull(printerSettings.PaperSources);
    }

    [Theory]
    [InlineData(PrintRange.AllPages)]
    [InlineData(PrintRange.CurrentPage)]
    [InlineData(PrintRange.Selection)]
    [InlineData(PrintRange.SomePages)]
    public void PrintRange_SetValue_ReturnsExpected(PrintRange printRange)
    {
        PrinterSettings printerSettings = new()
        {
            PrintRange = printRange
        };

        Assert.Equal(printRange, printerSettings.PrintRange);
    }

    [Theory]
    [InlineData(PrintRange.AllPages - 1)]
    [InlineData(PrintRange.SomePages + 1)]
    [InlineData((PrintRange)int.MaxValue)]
    [InlineData((PrintRange)int.MinValue)]
    public void PrintRange_Invalid_ThrowsInvalidEnumArgumentException(PrintRange printRange)
    {
        PrinterSettings printerSettings = new();
        Assert.ThrowsAny<ArgumentException>(() => printerSettings.PrintRange = printRange);
    }

    [Fact]
    public void PrintToFile_SetValue_ReturnsExpected()
    {
        bool printToFile = true;
        PrinterSettings printerSettings = new()
        {
            PrintToFile = printToFile
        };

        Assert.Equal(printToFile, printerSettings.PrintToFile);
    }

    [Theory]
    [InlineData("")]
    [InlineData("My printer")]
    public void PrinterName_SetValue_ReturnsExpected(string printerName)
    {
        PrinterSettings printerSettings = new()
        {
            PrinterName = printerName
        };

        Assert.Equal(printerName, printerSettings.PrinterName);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void PrinterName_Null_ReturnsExpected()
    {
        PrinterSettings printerSettings = new()
        {
            PrinterName = null
        };

        Assert.NotNull(printerSettings.PrinterName);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void PrinterResolutions_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        Assert.NotNull(printerSettings.PrinterResolutions);
    }

    public static IEnumerable<object[]> IsDirectPrintingSupported_ImageFormatSupported_TestData()
    {
        yield return new object[] { ImageFormat.Jpeg };
        yield return new object[] { ImageFormat.Png };
    }

    [ConditionalTheory(Helpers.AnyInstalledPrinters)]
    [MemberData(nameof(IsDirectPrintingSupported_ImageFormatSupported_TestData))]
    public void IsDirectPrintingSupported_ImageFormatSupported_ReturnsExpected(ImageFormat imageFormat)
    {
        PrinterSettings printerSettings = new();
        _ = printerSettings.IsDirectPrintingSupported(imageFormat);
    }

    public static IEnumerable<object[]> IsDirectPrintingSupported_ImageFormatNotSupported_TestData()
    {
        yield return new object[] { ImageFormat.Emf };
        yield return new object[] { ImageFormat.Exif };
        yield return new object[] { ImageFormat.Gif };
        yield return new object[] { ImageFormat.Icon };
        yield return new object[] { ImageFormat.MemoryBmp };
        yield return new object[] { ImageFormat.Tiff };
        yield return new object[] { ImageFormat.Wmf };
        yield return new object[] { ImageFormat.Bmp };
    }

    [Theory]
    [MemberData(nameof(IsDirectPrintingSupported_ImageFormatNotSupported_TestData))]
    public void IsDirectPrintingSupported_ImageFormatNotSupported_ReturnsExpected(ImageFormat imageFormat)
    {
        PrinterSettings printerSettings = new();
        Assert.False(printerSettings.IsDirectPrintingSupported(imageFormat));
    }

    [Fact]
    public void IsDirectPrintingSupported_ImageNotSupported_ReturnsExpected()
    {
        using Bitmap bitmap = new(10, 10);
        PrinterSettings printerSettings = new();
        Assert.False(printerSettings.IsDirectPrintingSupported(bitmap));
    }

    [ConditionalFact(typeof(PrinterSettingsTests), nameof(CanTestSetHdevmode_IntPtr_Success))]
    public void SupportsColor_ReturnsExpected()
    {
        // XPS and PDF printers support color.
        // docs.microsoft.com/en-us/windows-hardware/drivers/print/improved-color-printing
        PrinterSettings printerSettings = new() { PrinterName = GetNameOfTestPrinterSuitableForDevModeTesting() };
        Assert.True(printerSettings.SupportsColor);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(int.MaxValue)]
    public void ToPage_SetValue_ReturnsExpected(int toPage)
    {
        PrinterSettings printerSettings = new()
        {
            ToPage = toPage
        };

        Assert.Equal(toPage, printerSettings.ToPage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void ToPage_Invalid_ThrowsArgumentException(int toPage)
    {
        PrinterSettings printerSettings = new();
        AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.ToPage = toPage);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void Clone_Success()
    {
        PrinterSettings printerSettings = new();
        PrinterSettings clone = Assert.IsAssignableFrom<PrinterSettings>(printerSettings.Clone());
        Assert.False(ReferenceEquals(clone, printerSettings));
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void CreateMeasurementGraphics_Default_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        using Graphics graphic = printerSettings.CreateMeasurementGraphics();
        Assert.NotNull(graphic);
        Assert.Equal((double)printerSettings.DefaultPageSettings.Bounds.X, graphic.VisibleClipBounds.X, 0);
        Assert.Equal((double)printerSettings.DefaultPageSettings.Bounds.Y, graphic.VisibleClipBounds.Y, 0);
        Assert.Equal((double)printerSettings.DefaultPageSettings.PrintableArea.Height, graphic.VisibleClipBounds.Height, 0);
        Assert.Equal((double)printerSettings.DefaultPageSettings.PrintableArea.Width, graphic.VisibleClipBounds.Width, 0);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void CreateMeasurementGraphics_Bool_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        using Graphics graphic = printerSettings.CreateMeasurementGraphics(true);
        Assert.NotNull(graphic);
        Assert.Equal((double)printerSettings.DefaultPageSettings.PrintableArea.Height, graphic.VisibleClipBounds.Height, 0);
        Assert.Equal((double)printerSettings.DefaultPageSettings.PrintableArea.Width, graphic.VisibleClipBounds.Width, 0);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void CreateMeasurementGraphics_PageSettings_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        PageSettings pageSettings = new();
        using Graphics graphic = printerSettings.CreateMeasurementGraphics(pageSettings);
        Assert.NotNull(graphic);
        Assert.Equal((double)printerSettings.DefaultPageSettings.Bounds.X, graphic.VisibleClipBounds.X, 0);
        Assert.Equal((double)printerSettings.DefaultPageSettings.Bounds.Y, graphic.VisibleClipBounds.Y, 0);
        Assert.Equal((double)printerSettings.DefaultPageSettings.PrintableArea.Height, graphic.VisibleClipBounds.Height, 0);
        Assert.Equal((double)printerSettings.DefaultPageSettings.PrintableArea.Width, graphic.VisibleClipBounds.Width, 0);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void CreateMeasurementGraphics_PageSettingsBool_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        PageSettings pageSettings = new();
        using Graphics graphic = printerSettings.CreateMeasurementGraphics(pageSettings, true);
        Assert.NotNull(graphic);
        Assert.Equal((double)printerSettings.DefaultPageSettings.PrintableArea.Height, graphic.VisibleClipBounds.Height, 0);
        Assert.Equal((double)printerSettings.DefaultPageSettings.PrintableArea.Width, graphic.VisibleClipBounds.Width, 0);
    }

    [Fact]
    public void CreateMeasurementGraphics_Null_ThrowsNullReferenceException()
    {
        PrinterSettings printerSettings = new();
        Assert.Throws<NullReferenceException>(() => printerSettings.CreateMeasurementGraphics(null));
        Assert.Throws<NullReferenceException>(() => printerSettings.CreateMeasurementGraphics(null, true));
    }

    [Fact]
    public void GetHdevmode_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        nint handle = printerSettings.GetHdevmode();
        Assert.NotEqual(0, handle);
    }

    [Fact]
    public void GetHdevmode_PageSettings_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        PageSettings pageSettings = new();
        nint handle = printerSettings.GetHdevmode(pageSettings);
        Assert.NotEqual(0, handle);
    }

    [Fact]
    public void GetHdevmode_Null_ThrowsNullReferenceException()
    {
        PrinterSettings printerSettings = new();
        Assert.Throws<NullReferenceException>(() => printerSettings.GetHdevmode(null));
    }

    [Fact]
    public void GetHdevnames_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        nint handle = printerSettings.GetHdevnames();
        Assert.NotEqual(0, handle);
    }

    [ConditionalFact(typeof(PrinterSettingsTests), nameof(CanTestSetHdevmode_IntPtr_Success))]
    public void SetHdevmode_IntPtr_Success()
    {
        string printerName = GetNameOfTestPrinterSuitableForDevModeTesting();
        PrinterSettings printerSettings = new() { PrinterName = printerName, Copies = 3 };
        PrinterSettings newPrinterSettings = new() { PrinterName = printerName, Copies = 6 };

        IntPtr handle = printerSettings.GetHdevmode();
        newPrinterSettings.SetHdevmode(handle);
        Assert.Equal(printerSettings.Copies, newPrinterSettings.Copies);
        Assert.Equal(printerSettings.Collate, newPrinterSettings.Collate);
        Assert.Equal(printerSettings.Duplex, newPrinterSettings.Duplex);
    }

    public static bool CanTestSetHdevmode_IntPtr_Success => GetNameOfTestPrinterSuitableForDevModeTesting() is not null;

    private static string GetNameOfTestPrinterSuitableForDevModeTesting()
    {
        foreach (string candidate in s_testPrinterNames)
        {
            PrinterSettings printerSettings = new() { PrinterName = candidate };
            if (printerSettings.IsValid)
                return candidate;
        }

        return null;
    }

    private static readonly string[] s_testPrinterNames =
    [
        // Our method of testing some apis requires a printer that supports multi-copy printing,
        // collating, color and duplex settings. Not all printers support these so
        // rather than trust the machine running the test to have configured such a printer as the default,
        // use the name of a known compliant printer that ships with Windows 10.
        "Microsoft Print to PDF",
        "Microsoft XPS Document Writer", // Backup for older Windows
    ];

    [Fact]
    public void GetHdevmode_Zero_ThrowsArgumentException()
    {
        PrinterSettings printerSettings = new();
        AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.SetHdevmode(IntPtr.Zero));
    }

    [Fact]
    public void SetHdevnames_IntPtr_Success()
    {
        PrinterSettings printerSettings = new();
        PrinterSettings newPrinterSettings = new();
        IntPtr handle = printerSettings.GetHdevnames();
        newPrinterSettings.SetHdevnames(handle);
        Assert.Equal(newPrinterSettings.PrinterName, printerSettings.PrinterName);
    }

    [Fact]
    public void ToString_ReturnsExpected()
    {
        PrinterSettings printerSettings = new();
        string expected = "[PrinterSettings "
            + printerSettings.PrinterName
            + " Copies=" + printerSettings.Copies.ToString(CultureInfo.InvariantCulture)
            + " Collate=" + printerSettings.Collate.ToString(CultureInfo.InvariantCulture)
            + " Duplex=" + printerSettings.Duplex.ToString()
            + " FromPage=" + printerSettings.FromPage.ToString(CultureInfo.InvariantCulture)
            + " LandscapeAngle=" + printerSettings.LandscapeAngle.ToString(CultureInfo.InvariantCulture)
            + " MaximumCopies=" + printerSettings.MaximumCopies.ToString(CultureInfo.InvariantCulture)
            + " OutputPort=" + printerSettings.PrintFileName.ToString(CultureInfo.InvariantCulture)
            + " ToPage=" + printerSettings.ToPage.ToString(CultureInfo.InvariantCulture)
            + "]";

        Assert.Equal(expected, printerSettings.ToString());
    }

    [Fact]
    public void PrinterSettings_GetInstalledPrinters_Success()
    {
        var installedPrinters = Helpers.InstalledPrinters;
        installedPrinters.Should().NotBeNull();
        foreach (string printer in installedPrinters)
        {
            printer.Should().NotBeNullOrEmpty();
        }
    }
}
