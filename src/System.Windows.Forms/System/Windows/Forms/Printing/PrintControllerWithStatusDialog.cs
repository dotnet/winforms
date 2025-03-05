// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Printing;

namespace System.Windows.Forms;

public partial class PrintControllerWithStatusDialog : PrintController
{
    private readonly PrintController _underlyingController;
    private PrintDocument? _document;
    private BackgroundThread? _backgroundThread;
    private int _pageNumber;
    private readonly string _dialogTitle;

    public PrintControllerWithStatusDialog(PrintController underlyingController)
        : this(underlyingController, SR.PrintControllerWithStatusDialog_DialogTitlePrint)
    {
    }

    public PrintControllerWithStatusDialog(PrintController underlyingController, string dialogTitle)
    {
        _underlyingController = underlyingController;
        _dialogTitle = dialogTitle;
    }

    public override bool IsPreview => _underlyingController?.IsPreview ?? false;

    /// <summary>
    ///  Begins the control sequence that determines when and how to print a document.
    /// </summary>
    public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
    {
        base.OnStartPrint(document, e);

        _document = document;
        _pageNumber = 1;

        if (SystemInformation.UserInteractive)
        {
            _backgroundThread = new BackgroundThread(this); // starts running & shows dialog automatically
        }

        try
        {
            _underlyingController.OnStartPrint(document, e);
        }
        catch
        {
            _backgroundThread?.Stop();
            throw;
        }
        finally
        {
            if (_backgroundThread is not null && _backgroundThread._canceled)
            {
                e.Cancel = true;
            }
        }
    }

    /// <summary>
    ///  Begins the control sequence that determines when and how to print a page of a document.
    /// </summary>
    public override Graphics? OnStartPage(PrintDocument document, PrintPageEventArgs e)
    {
        base.OnStartPage(document, e);

        _backgroundThread?.UpdateLabel();

        Graphics? result = _underlyingController.OnStartPage(document, e);
        if (_backgroundThread is not null && _backgroundThread._canceled)
        {
            e.Cancel = true;
        }

        return result;
    }

    /// <summary>
    ///  Completes the control sequence that determines when and how to print a page of a document.
    /// </summary>
    public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
    {
        _underlyingController.OnEndPage(document, e);
        if (_backgroundThread is not null && _backgroundThread._canceled)
        {
            e.Cancel = true;
        }

        _pageNumber++;

        base.OnEndPage(document, e);
    }

    /// <summary>
    ///  Completes the control sequence that determines when and how to print a document.
    /// </summary>
    public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
    {
        _underlyingController.OnEndPrint(document, e);
        if (_backgroundThread is not null && _backgroundThread._canceled)
        {
            e.Cancel = true;
        }

        _backgroundThread?.Stop();

        base.OnEndPrint(document, e);
    }
}
