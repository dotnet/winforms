// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.UI.Controls.Dialogs;
using static Windows.Win32.UI.Controls.Dialogs.OPEN_FILENAME_FLAGS;
using static Windows.Win32.UI.Shell.FILEOPENDIALOGOPTIONS;

namespace System.Windows.Forms;

public partial class FileDialog
{
    private protected virtual bool SettingsSupportVistaDialog
        => !ShowHelp && Application.VisualStyleState.HasFlag(VisualStyles.VisualStyleState.ClientAreaEnabled);

    internal bool UseVistaDialogInternal
        => AutoUpgradeEnabled
            && SettingsSupportVistaDialog
            && SystemInformation.BootMode == BootMode.Normal;

    private protected abstract unsafe ComScope<IFileDialog> CreateVistaDialog();

    private unsafe bool TryRunDialogVista(HWND hWndOwner, out bool returnValue)
    {
        using ComScope<IFileDialog> dialog = CreateVistaDialog();

        if (dialog.IsNull)
        {
            // Creating the Vista dialog can fail on Windows Server Core, even if the
            // Server Core App Compatibility FOD is installed.
            returnValue = false;
            return false;
        }

        OnBeforeVistaDialog(dialog);
        using var events = ComHelpers.GetComScope<IFileDialogEvents>(new VistaDialogEvents(this));

        dialog.Value->Advise(events, out uint eventCookie);
        try
        {
            returnValue = dialog.Value->Show(hWndOwner) == HRESULT.S_OK;
            return true;
        }
        finally
        {
            dialog.Value->Unadvise(eventCookie);
        }
    }

    private unsafe void OnBeforeVistaDialog(IFileDialog* dialog)
    {
        if (ClientGuid is { } clientGuid)
        {
            // IFileDialog::SetClientGuid should be called immediately after creation of the dialog object.
            // https://learn.microsoft.com/windows/win32/api/shobjidl_core/nf-shobjidl_core-ifiledialog-setclientguid#remarks
            dialog->SetClientGuid(in clientGuid);
        }

        dialog->SetDefaultExtension(DefaultExt);
        dialog->SetFileName(FileName);

        if (!string.IsNullOrEmpty(InitialDirectory))
        {
            using ComScope<IShellItem> initialDirectory = new(PInvoke.SHCreateShellItem(InitialDirectory));
            if (!initialDirectory.IsNull)
            {
                dialog->SetDefaultFolder(initialDirectory);
                dialog->SetFolder(initialDirectory);
            }
        }

        dialog->SetTitle(Title);
        dialog->SetOptions(GetOptions());
        SetFileTypes(dialog);

        _customPlaces.Apply(dialog);
    }

    private FILEOPENDIALOGOPTIONS GetOptions()
    {
        const FILEOPENDIALOGOPTIONS BlittableOptions =
            FOS_OVERWRITEPROMPT
            | FOS_NOCHANGEDIR
            | FOS_NOVALIDATE
            | FOS_ALLOWMULTISELECT
            | FOS_PATHMUSTEXIST
            | FOS_FILEMUSTEXIST
            | FOS_CREATEPROMPT
            | FOS_NODEREFERENCELINKS
            | FOS_DONTADDTORECENT
            | FOS_NOREADONLYRETURN
            | FOS_NOTESTFILECREATE
            | FOS_FORCESHOWHIDDEN
            | FOS_DEFAULTNOMINIMODE
            | FOS_OKBUTTONNEEDSINTERACTION
            | FOS_HIDEPINNEDPLACES
            | FOS_FORCEPREVIEWPANEON;

#if DEBUG
        const OPEN_FILENAME_FLAGS UnexpectedOptions =
            OFN_SHOWHELP        // If ShowHelp is true, we don't use the Vista Dialog
            | OFN_ENABLEHOOK    // These shouldn't be set in options (only set in the flags for the legacy dialog)
            | OFN_ENABLESIZING  // These shouldn't be set in options (only set in the flags for the legacy dialog)
            | OFN_EXPLORER;     // These shouldn't be set in options (only set in the flags for the legacy dialog)

        Debug.Assert((UnexpectedOptions & _fileNameFlags) == 0, "Unexpected FileDialog options");
#endif

        FILEOPENDIALOGOPTIONS result = (FILEOPENDIALOGOPTIONS)_fileNameFlags & BlittableOptions;

        // Make sure that the Open dialog allows the user to specify
        // non-file system locations. This flag will cause the dialog to copy the resource
        // to a local cache (Temporary Internet Files), and return that path instead. This
        // also affects the Save dialog by disallowing navigation to these areas.
        // An example of a non-file system location is a URL (http://), or a file stored on
        // a digital camera that is not mapped to a drive letter.
        // This reproduces the behavior of the "classic" Open and Save dialogs.
        result |= FOS_FORCEFILESYSTEM;

        return result;
    }

    private protected abstract unsafe string[] ProcessVistaFiles(IFileDialog* dialog);

    private unsafe bool HandleVistaFileOk(IFileDialog* dialog)
    {
        OPEN_FILENAME_FLAGS saveOptions = _fileNameFlags;
        int saveFilterIndex = FilterIndex;
        string[]? saveFileNames = _fileNames;
        bool ok = false;

        try
        {
            Thread.MemoryBarrier();
            dialog->GetFileTypeIndex(out uint filterIndexTemp);
            FilterIndex = unchecked((int)filterIndexTemp);
            _fileNames = ProcessVistaFiles(dialog);
            if (ProcessFileNames(_fileNames))
            {
                CancelEventArgs ceevent = new();
                if (NativeWindow.WndProcShouldBeDebuggable)
                {
                    OnFileOk(ceevent);
                    ok = !ceevent.Cancel;
                }
                else
                {
                    try
                    {
                        OnFileOk(ceevent);
                        ok = !ceevent.Cancel;
                    }
                    catch (Exception e)
                    {
                        Application.OnThreadException(e);
                    }
                }
            }
        }
        finally
        {
            if (!ok)
            {
                Thread.MemoryBarrier();
                _fileNames = saveFileNames;

                _fileNameFlags = saveOptions;
                FilterIndex = saveFilterIndex;
            }
            else
            {
                if (_fileNameFlags.HasFlag(OFN_HIDEREADONLY))
                {
                    // When the dialog is dismissed OK, the Readonly bit can't be left on if ShowReadOnly was false.
                    // Downlevel this happens automatically, on Vista mode, we need to watch out for it.
                    _fileNameFlags &= ~OFN_READONLY;
                }
            }
        }

        return ok;
    }

    private unsafe void SetFileTypes(IFileDialog* dialog)
    {
        COMDLG_FILTERSPEC[] filterItems = GetFilterItems(_filter);
        if (filterItems.Length > 0)
        {
            fixed (COMDLG_FILTERSPEC* fi = filterItems)
            {
                dialog->SetFileTypes((uint)filterItems.Length, fi);
            }

            dialog->SetFileTypeIndex(unchecked((uint)FilterIndex));
        }
    }

    private static unsafe COMDLG_FILTERSPEC[] GetFilterItems(string? filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return [];
        }

        // Expected input types:
        //
        //  "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        //  "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"

        string[] tokens = filter.Split('|');
        if (tokens.Length % 2 != 0)
        {
            return [];
        }

        var extensions = new COMDLG_FILTERSPEC[tokens.Length / 2];

        // All even numbered tokens should be labels
        // Odd numbered tokens are the associated extensions
        for (int i = 1; i < tokens.Length; i += 2)
        {
            fixed (char* tokenName = tokens[i - 1])
            fixed (char* tokenSpec = tokens[i])
            {
                COMDLG_FILTERSPEC extension = new()
                {
                    pszName = tokenName,
                    // This may be a semicolon delimited list of extensions (that's ok)
                    pszSpec = tokenSpec
                };

                extensions[(i - 1) / 2] = extension;
            }
        }

        return extensions;
    }

    private protected static unsafe string GetFilePathFromShellItem(IShellItem* item)
    {
        item->GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out PWSTR ppszName);
        return ppszName.ToStringAndCoTaskMemFree()!;
    }

    private readonly FileDialogCustomPlacesCollection _customPlaces = [];

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public FileDialogCustomPlacesCollection CustomPlaces => _customPlaces;

    /// <summary>
    ///  Gets or sets whether the dialog will be automatically upgraded to enable new features.
    /// </summary>
    [DefaultValue(true)]
    public bool AutoUpgradeEnabled { get; set; } = true;

    /// <summary>
    ///  Gets or sets a value indicating whether the OK button of the dialog box is
    ///  disabled until the user navigates the view or edits the filename (if applicable).
    /// </summary>
    /// <remarks>
    ///  <para>
    ///  Note: Disabling of the OK button does not prevent the dialog from being submitted by the Enter key.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FileDialogOkRequiresInteractionDescr))]
    public bool OkRequiresInteraction
    {
        get => _dialogOptions.HasFlag(FOS_OKBUTTONNEEDSINTERACTION);
        set => _dialogOptions.ChangeFlags(FOS_OKBUTTONNEEDSINTERACTION, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the items shown by default in the view's
    ///  navigation pane are shown.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.FileDialogShowPinnedPlacesDescr))]
    public bool ShowPinnedPlaces
    {
        get => !_dialogOptions.HasFlag(FOS_HIDEPINNEDPLACES);
        set => _dialogOptions.ChangeFlags(FOS_HIDEPINNEDPLACES, !value);
    }
}
