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

    /// <summary>
    ///  This is new public property which notifies if this controller is used for PrintPreview.. so get the underlying Controller
    ///  and return its IsPreview Property.
    /// </summary>
    public override bool IsPreview
    {
        get
        {
            if (_underlyingController is not null)
            {
                return _underlyingController.IsPreview;
            }

            return false;
        }
    }

    /// <summary>
    ///  Implements StartPrint by delegating to the underlying controller.
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

        // OnStartPrint does the security check... lots of
        // extra setup to make sure that we tear down
        // correctly...
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
    ///  Implements StartPage by delegating to the underlying controller.
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
    ///  Implements EndPage by delegating to the underlying controller.
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
    ///  Implements EndPrint by delegating to the underlying controller.
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
