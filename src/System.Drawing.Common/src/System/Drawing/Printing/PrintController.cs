// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
///  Controls how a document is printed.
/// </summary>
public abstract partial class PrintController
{
    private protected SafeDeviceModeHandle? _modeHandle;

    protected PrintController()
    {
    }

    /// <summary>
    ///  Gets a value indicating whether the <see cref="PrintController"/> is used for print preview.
    /// </summary>
    public virtual bool IsPreview => false;

    /// <summary>
    ///  When overridden in a derived class, begins the control sequence of when and how to print a page in a document.
    /// </summary>
    public virtual Graphics? OnStartPage(PrintDocument document, PrintPageEventArgs e) => null;

    /// <summary>
    ///  When overridden in a derived class, completes the control sequence of when and how to print a page in a document.
    /// </summary>
    public virtual void OnEndPage(PrintDocument document, PrintPageEventArgs e)
    {
    }

    /// <remarks>
    ///  <para>
    ///   If you have nested PrintControllers, this method won't get called on the inner one.
    ///   Add initialization code to StartPrint or StartPage instead.
    ///  </para>
    /// </remarks>
    internal void Print(PrintDocument document)
    {
        // Get the PrintAction for this event
        PrintAction printAction = IsPreview
            ? PrintAction.PrintToPreview
            : document.PrinterSettings.PrintToFile ? PrintAction.PrintToFile : PrintAction.PrintToPrinter;

        // Check that user has permission to print to this particular printer
        PrintEventArgs printEvent = new(printAction);
        document.OnBeginPrint(printEvent);
        if (printEvent.Cancel)
        {
            document.OnEndPrint(printEvent);
            return;
        }

        OnStartPrint(document, printEvent);
        if (printEvent.Cancel)
        {
            document.OnEndPrint(printEvent);
            OnEndPrint(document, printEvent);
            return;
        }

        bool canceled = true;

        try
        {
            // To enable optimization of the preview dialog, add the following to the config file:
            // <runtime >
            //     <!-- AppContextSwitchOverrides values are in the form of 'key1=true|false;key2=true|false  -->
            //     <AppContextSwitchOverrides value = "Switch.System.Drawing.Printing.OptimizePrintPreview=true" />
            // </runtime >
            canceled = LocalAppContextSwitches.OptimizePrintPreview ? PrintLoopOptimized(document) : PrintLoop(document);
        }
        finally
        {
            try
            {
                document.OnEndPrint(printEvent);
                printEvent.Cancel = canceled | printEvent.Cancel;
            }
            finally
            {
                OnEndPrint(document, printEvent);
            }
        }
    }

    /// <summary>
    ///  Returns true if print was aborted.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If you have nested <see cref="PrintController"/> objects, this method won't get called on the inner one.
    ///   Add initialization code to <see cref="OnStartPrint(PrintDocument, PrintEventArgs)"/> or
    ///   <see cref="OnStartPage(PrintDocument, PrintPageEventArgs)"/> instead.
    ///  </para>
    /// </remarks>
    private bool PrintLoop(PrintDocument document)
    {
        QueryPageSettingsEventArgs queryEvent = new((PageSettings)document.DefaultPageSettings.Clone());
        while (true)
        {
            document.OnQueryPageSettings(queryEvent);
            if (queryEvent.Cancel)
            {
                return true;
            }

            PrintPageEventArgs pageEvent = CreatePrintPageEvent(queryEvent.PageSettings);
            Graphics? graphics = OnStartPage(document, pageEvent);
            pageEvent.SetGraphics(graphics);

            try
            {
                document.OnPrintPage(pageEvent);
                OnEndPage(document, pageEvent);
            }
            finally
            {
                pageEvent.Dispose();
            }

            if (pageEvent.Cancel)
            {
                return true;
            }
            else if (!pageEvent.HasMorePages)
            {
                return false;
            }
        }
    }

    private bool PrintLoopOptimized(PrintDocument document)
    {
        PrintPageEventArgs? pageEvent = null;
        PageSettings documentPageSettings = (PageSettings)document.DefaultPageSettings.Clone();
        QueryPageSettingsEventArgs queryEvent = new(documentPageSettings);

        while (true)
        {
            queryEvent.PageSettingsChanged = false;
            document.OnQueryPageSettings(queryEvent);
            if (queryEvent.Cancel)
            {
                return true;
            }

            if (!queryEvent.PageSettingsChanged)
            {
                // QueryPageSettings event handler did not change the page settings,
                // thus we use default page settings from the document object.
                if (pageEvent is null)
                {
                    pageEvent = CreatePrintPageEvent(queryEvent.PageSettings);
                }
                else
                {
                    // This is not the first page and the settings had not changed since the previous page,
                    // thus don't re-apply them.
                    pageEvent.CopySettingsToDevMode = false;
                }

                Graphics? graphics = OnStartPage(document, pageEvent);
                pageEvent.SetGraphics(graphics);
            }
            else
            {
                // Page settings were customized, so use the customized ones in the start page event.
                pageEvent = CreatePrintPageEvent(queryEvent.PageSettings);
                Graphics? graphics = OnStartPage(document, pageEvent);
                pageEvent.SetGraphics(graphics);
            }

            try
            {
                document.OnPrintPage(pageEvent);
                OnEndPage(document, pageEvent);
            }
            finally
            {
                pageEvent.Graphics?.Dispose();
                pageEvent.SetGraphics(null);
            }

            if (pageEvent.Cancel)
            {
                return true;
            }
            else if (!pageEvent.HasMorePages)
            {
                return false;
            }
        }
    }

    private PrintPageEventArgs CreatePrintPageEvent(PageSettings pageSettings)
    {
        Debug.Assert(_modeHandle is not null, "modeHandle is null. Someone must have forgot to call base.StartPrint");

        Rectangle pageBounds = pageSettings.GetBounds(_modeHandle);
        Rectangle marginBounds = new(
            pageSettings.Margins.Left,
            pageSettings.Margins.Top,
            pageBounds.Width - (pageSettings.Margins.Left + pageSettings.Margins.Right),
            pageBounds.Height - (pageSettings.Margins.Top + pageSettings.Margins.Bottom));

        PrintPageEventArgs pageEvent = new(null, marginBounds, pageBounds, pageSettings);
        return pageEvent;
    }

    /// <summary>
    ///  When overridden in a derived class, begins the control sequence of when and how to print a document.
    /// </summary>
    public virtual void OnStartPrint(PrintDocument document, PrintEventArgs e)
    {
        _modeHandle = (SafeDeviceModeHandle)document.PrinterSettings.GetHdevmode(document.DefaultPageSettings);
    }

    /// <summary>
    ///  When overridden in a derived class, completes the control sequence of when and how to print a document.
    /// </summary>
    public virtual void OnEndPrint(PrintDocument document, PrintEventArgs e) => _modeHandle?.Close();
}
