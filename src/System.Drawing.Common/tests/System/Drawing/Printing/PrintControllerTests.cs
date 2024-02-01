// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing.Tests;

public class PrintControllerTests
{
    [Fact]
    public void Ctor_Default()
    {
        SubPrintController controller = new();
        Assert.False(controller.IsPreview);
    }

    [ConditionalFact(Helpers.AnyInstalledPrinters)]
    public void OnStartPage_InvokeWithPrint_ReturnsNull()
    {
        using PrintDocument document = new();
        SubPrintController controller = new();
        controller.OnStartPrint(document, new PrintEventArgs());

        PrintPageEventArgs printEventArgs = new(null, Rectangle.Empty, Rectangle.Empty, null);
        Assert.Null(controller.OnStartPage(document, printEventArgs));

        // Call OnEndPage.
        controller.OnEndPage(document, printEventArgs);

        // Call EndPrint.
        controller.OnEndPrint(document, new PrintEventArgs());
    }

    [Fact]
    public void OnStartPage_Invoke_ReturnsNull()
    {
        using PrintDocument document = new();
        SubPrintController controller = new();
        Assert.Null(controller.OnStartPage(document, new PrintPageEventArgs(null, Rectangle.Empty, Rectangle.Empty, null)));
        Assert.Null(controller.OnStartPage(null, null));
    }

    [Fact]
    public void OnEndPage_InvokeWithoutStarting_Nop()
    {
        using PrintDocument document = new();
        SubPrintController controller = new();
        controller.OnEndPage(document, new PrintPageEventArgs(null, Rectangle.Empty, Rectangle.Empty, null));
        controller.OnEndPage(null, null);
    }

    public static IEnumerable<object[]> PrintEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new PrintEventArgs() };
    }

    [Theory]
    [MemberData(nameof(PrintEventArgs_TestData))]
    public void OnStartPrint_InvokeWithDocument_Success(PrintEventArgs e)
    {
        using PrintDocument document = new();
        SubPrintController controller = new();
        controller.OnStartPrint(document, e);

        // Call OnEndPrint
        controller.OnEndPrint(document, e);
    }

    [Theory]
    [MemberData(nameof(PrintEventArgs_TestData))]
    public void OnStartPrint_InvokeWithDocumentSeveralTimes_Success(PrintEventArgs e)
    {
        using PrintDocument document = new();
        SubPrintController controller = new();
        controller.OnStartPrint(document, e);
        controller.OnStartPrint(document, e);

        // Call OnEndPrint
        controller.OnEndPrint(document, e);
    }

    [Fact]
    public void OnStartPrint_InvokeNullDocument_ThrowsNullReferenceException()
    {
        SubPrintController controller = new();
        Assert.Throws<NullReferenceException>(() => controller.OnStartPrint(null, new PrintEventArgs()));
    }

    [Theory]
    [MemberData(nameof(PrintEventArgs_TestData))]
    public void OnEndPrint_InvokeWithoutStarting_Nop(PrintEventArgs e)
    {
        using PrintDocument document = new();
        SubPrintController controller = new();
        controller.OnEndPrint(document, e);
        controller.OnEndPrint(null, e);
    }

    private class SubPrintController : PrintController
    {
    }
}
