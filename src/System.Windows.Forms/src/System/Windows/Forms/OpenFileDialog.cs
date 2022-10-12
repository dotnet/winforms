// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Controls.Dialogs;
using static Windows.Win32.UI.Controls.Dialogs.OPEN_FILENAME_FLAGS;
using static Windows.Win32.UI.Shell.FILEOPENDIALOGOPTIONS;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a common dialog box that displays the control that allows the user to open a file.
    ///  This class cannot be inherited.
    /// </summary>
    [SRDescription(nameof(SR.DescriptionOpenFileDialog))]
    public sealed partial class OpenFileDialog : FileDialog
    {
        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box displays a
        ///  warning if the user specifies a file name that does not exist.
        /// </summary>
        [DefaultValue(true)]
        [SRDescription(nameof(SR.OFDcheckFileExistsDescr))]
        public override bool CheckFileExists
        {
            get => base.CheckFileExists;
            set => base.CheckFileExists = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box allows multiple files to be selected.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.OFDmultiSelectDescr))]
        public bool Multiselect
        {
            get => GetOption(OFN_ALLOWMULTISELECT);
            set => SetOption(OFN_ALLOWMULTISELECT, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the read-only check box is selected.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.OFDreadOnlyCheckedDescr))]
        public bool ReadOnlyChecked
        {
            get => GetOption(OFN_READONLY);
            set => SetOption(OFN_READONLY, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box allows to select read-only files.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.OpenFileDialogSelectReadOnlyDescr))]
        public bool SelectReadOnly
        {
            get => !GetOption(OFN_NOREADONLYRETURN);
            set => SetOption(OFN_NOREADONLYRETURN, !value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box shows a preview for selected files.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.OpenFileDialogShowPreviewDescr))]
        public bool ShowPreview
        {
            get => _dialogOptions.HasFlag(FOS_FORCEPREVIEWPANEON);
            set => _dialogOptions.ChangeFlags(FOS_FORCEPREVIEWPANEON, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog contains a read-only check box.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.OFDshowReadOnlyDescr))]
        public bool ShowReadOnly
        {
            get => !GetOption(OFN_HIDEREADONLY);
            set => SetOption(OFN_HIDEREADONLY, !value);
        }

        /// <summary>
        ///  Opens the file selected by the user with read-only permission.  The file
        ///  attempted is specified by the <see cref="FileDialog.FileName"/> property.
        /// </summary>
        public Stream OpenFile()
        {
            string filename = FileNames[0];
            ArgumentNullException.ThrowIfNull(filename);

            return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        ///  Resets all properties to their default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            SetOption(OFN_FILEMUSTEXIST, true);
        }

        /// <summary>
        ///  Displays a file open dialog.
        /// </summary>
        private protected unsafe override bool RunFileDialog(OPENFILENAME* ofn)
        {
            bool result = PInvoke.GetOpenFileName(ofn);
            if (!result)
            {
                // Something may have gone wrong - check for error condition
                switch (PInvoke.CommDlgExtendedError())
                {
                    case COMMON_DLG_ERRORS.FNERR_INVALIDFILENAME:
                        throw new InvalidOperationException(string.Format(SR.FileDialogInvalidFileName, FileName));
                    case COMMON_DLG_ERRORS.FNERR_SUBCLASSFAILURE:
                        throw new InvalidOperationException(SR.FileDialogSubLassFailure);
                    case COMMON_DLG_ERRORS.FNERR_BUFFERTOOSMALL:
                        throw new InvalidOperationException(SR.FileDialogBufferTooSmall);
                }
            }

            return result;
        }

        private protected unsafe override string[] ProcessVistaFiles(IFileDialog* dialog)
        {
            if (!Multiselect)
            {
                IShellItem* item;
                return dialog->GetResult(&item).Failed
                    ? Array.Empty<string>()
                    : new string[] { GetFilePathFromShellItem(item) };
            }

            IShellItemArray* items;
            if (((IFileOpenDialog*)dialog)->GetResults(&items).Failed)
            {
                return Array.Empty<string>();
            }

            try
            {
                items->GetCount(out uint count);
                string[] files = new string[count];
                for (uint i = 0; i < count; ++i)
                {
                    IShellItem* item;
                    items->GetItemAt(i, &item).ThrowOnFailure();
                    files[i] = GetFilePathFromShellItem(item);
                }

                return files;
            }
            finally
            {
                items->Release();
            }
        }

        private protected unsafe override IFileDialog* CreateVistaDialog()
        {
            PInvoke.CoCreateInstance(
                in CLSID.FileOpenDialog,
                pUnkOuter: null,
                CLSCTX.CLSCTX_INPROC_SERVER | CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_REMOTE_SERVER,
                out IFileDialog* fileDialog).ThrowOnFailure();

            return fileDialog;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SafeFileName => Path.GetFileName(FileName) ?? string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] SafeFileNames
        {
            get
            {
                string[] fullPaths = FileNames;
                if (fullPaths is null || 0 == fullPaths.Length)
                {
                    return Array.Empty<string>();
                }

                string[] safePaths = new string[fullPaths.Length];
                for (int i = 0; i < safePaths.Length; ++i)
                {
                    safePaths[i] = Path.GetFileName(fullPaths[i]);
                }

                return safePaths;
            }
        }

        private protected override bool SettingsSupportVistaDialog
        {
            get => base.SettingsSupportVistaDialog && !ShowReadOnly;
        }
    }
}
