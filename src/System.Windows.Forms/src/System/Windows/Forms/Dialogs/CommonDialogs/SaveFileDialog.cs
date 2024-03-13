// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Controls.Dialogs;
using static Windows.Win32.UI.Controls.Dialogs.OPEN_FILENAME_FLAGS;
using static Windows.Win32.UI.Shell.FILEOPENDIALOGOPTIONS;

namespace System.Windows.Forms;

/// <summary>
///  Represents common dialog box that allows the user to specify options for saving a
///  file. This class cannot be inherited.
/// </summary>
[Designer($"System.Windows.Forms.Design.SaveFileDialogDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionSaveFileDialog))]
public sealed partial class SaveFileDialog : FileDialog
{
    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box verifies if the creation of the specified file will
    ///  be successful. If this flag is not set, the calling application must handle errors, such as denial of access,
    ///  discovered when the item is created.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.SaveFileDialogCheckWriteAccess))]
    public bool CheckWriteAccess
    {
        get => !GetOption(OFN_NOTESTFILECREATE);
        set => SetOption(OFN_NOTESTFILECREATE, !value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box prompts the user for
    ///  permission to create a file if the user specifies a file that does not exist.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.SaveFileDialogCreatePrompt))]
    public bool CreatePrompt
    {
        get => GetOption(OFN_CREATEPROMPT);
        set => SetOption(OFN_CREATEPROMPT, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box is always opened in the expanded mode.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.SaveFileDialogExpandedMode))]
    public bool ExpandedMode
    {
        get => _dialogOptions.HasFlag(FOS_DEFAULTNOMINIMODE);
        set => _dialogOptions.ChangeFlags(FOS_DEFAULTNOMINIMODE, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the Save As dialog box displays a warning if the user specifies
    ///  a file name that already exists.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.SaveFileDialogOverWritePrompt))]
    public bool OverwritePrompt
    {
        get => GetOption(OFN_OVERWRITEPROMPT);
        set => SetOption(OFN_OVERWRITEPROMPT, value);
    }

    /// <summary>
    ///  Opens the file with read/write permission selected by the user.
    /// </summary>
    public Stream OpenFile()
    {
        string filename = FileNames[0];
        filename.ThrowIfNullOrEmpty();
        return new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
    }

    /// <summary>
    ///  Prompts the user with a <see cref="MessageBox"/> when a file is about to be created. This method is
    ///  invoked when the <see cref="CreatePrompt"/> property is true and the specified file does not exist. A
    ///  return value of <see langword="false"/> prevents the dialog from closing.
    /// </summary>
    private bool PromptFileCreate(string fileName)
        => MessageBoxWithFocusRestore(
            string.Format(SR.FileDialogCreatePrompt, fileName),
            DialogCaption,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

    /// <summary>
    ///  Prompts the user when a file is about to be overwritten. This method is invoked when the
    ///  <see cref="OverwritePrompt"/> property is true and the specified file already exists. A return value
    ///  of <see langword="false"/> prevents the dialog from closing.
    /// </summary>
    private bool PromptFileOverwrite(string fileName)
        => MessageBoxWithFocusRestore(
            string.Format(SR.FileDialogOverwritePrompt, fileName),
            DialogCaption,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

    private protected override bool PromptUserIfAppropriate(string fileName)
    {
        if (!base.PromptUserIfAppropriate(fileName))
        {
            return false;
        }

        // Note: Vista dialog mode automatically prompts for overwrite.
        if (_fileNameFlags.HasFlag(OFN_OVERWRITEPROMPT)
            && !UseVistaDialogInternal
            && FileExists(fileName)
            && !PromptFileOverwrite(fileName))
        {
            return false;
        }

        if (_fileNameFlags.HasFlag(OFN_CREATEPROMPT) && !FileExists(fileName) && !PromptFileCreate(fileName))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///  Resets all dialog box options to their default values.
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        _dialogOptions |= FOS_DEFAULTNOMINIMODE;
        SetOption(OFN_OVERWRITEPROMPT, true);
    }

    private protected override unsafe bool RunFileDialog(OPENFILENAME* ofn)
    {
        bool result = PInvoke.GetSaveFileName(ofn);

        if (!result)
        {
            // Something may have gone wrong - check for error condition
            switch (PInvoke.CommDlgExtendedError())
            {
                case COMMON_DLG_ERRORS.FNERR_INVALIDFILENAME:
                    throw new InvalidOperationException(string.Format(SR.FileDialogInvalidFileName, FileName));
            }
        }

        return result;
    }

    private protected override unsafe string[] ProcessVistaFiles(IFileDialog* dialog)
    {
        using ComScope<IShellItem> item = new(null);
        dialog->GetResult(item);
        return item.IsNull ? [] : [GetFilePathFromShellItem(item)];
    }

    private protected override unsafe ComScope<IFileDialog> CreateVistaDialog()
    {
        HRESULT hr = PInvokeCore.CoCreateInstance(
            CLSID.FileSaveDialog,
            pUnkOuter: null,
            CLSCTX.CLSCTX_INPROC_SERVER | CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_REMOTE_SERVER,
            out IFileDialog* fileDialog);

        Debug.Assert(hr.Succeeded);
        return new(fileDialog);
    }
}
