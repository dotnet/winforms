// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;
using static Interop;
using static Interop.Shell32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a common dialog box
    ///  that displays the control that allows the user to open a file. This class
    ///  cannot be inherited.
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
        ///  Gets or sets a value
        ///  indicating whether the dialog box allows multiple files to be selected.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.OFDmultiSelectDescr))]
        public bool Multiselect
        {
            get => GetOption((int)Comdlg32.OFN.ALLOWMULTISELECT);
            set => SetOption((int)Comdlg32.OFN.ALLOWMULTISELECT, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether
        ///  the read-only check box is selected.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.OFDreadOnlyCheckedDescr))]
        public bool ReadOnlyChecked
        {
            get => GetOption((int)Comdlg32.OFN.READONLY);
            set => SetOption((int)Comdlg32.OFN.READONLY, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box allows to select read-only files.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.OpenFileDialogSelectReadOnlyDescr))]
        public bool SelectReadOnly
        {
            get => !GetOption((int)Comdlg32.OFN.NOREADONLYRETURN);
            set => SetOption((int)Comdlg32.OFN.NOREADONLYRETURN, !value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box shows a preview for selected files.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.OpenFileDialogShowPreviewDescr))]
        public bool ShowPreview
        {
            get => GetOption((int)FOS.FORCEPREVIEWPANEON);
            set => SetOption((int)FOS.FORCEPREVIEWPANEON, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog contains a read-only check box.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.OFDshowReadOnlyDescr))]
        public bool ShowReadOnly
        {
            get => !GetOption((int)Comdlg32.OFN.HIDEREADONLY);
            set => SetOption((int)Comdlg32.OFN.HIDEREADONLY, !value);
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
            SetOption((int)Comdlg32.OFN.FILEMUSTEXIST, true);
        }

        /// <summary>
        ///  Displays a file open dialog.
        /// </summary>
        private protected override bool RunFileDialog(NativeMethods.OPENFILENAME_I ofn)
        {
            bool result = UnsafeNativeMethods.GetOpenFileName(ofn);
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

        private protected override string[] ProcessVistaFiles(Interop.WinFormsComWrappers.FileDialogWrapper dialog)
        {
            var openDialog = (WinFormsComWrappers.FileOpenDialogWrapper)dialog;
            if (Multiselect)
            {
                openDialog.GetResults(out WinFormsComWrappers.ShellItemArrayWrapper? results);
                if (results is null)
                {
                    return Array.Empty<string>();
                }

                try
                {
                    results.GetCount(out uint count);
                    string[] files = new string[count];
                    for (uint i = 0; i < count; ++i)
                    {
                        results.GetItemAt(i, out IShellItem item);
                        files[unchecked((int)i)] = GetFilePathFromShellItem(item);
                    }

                    return files;
                }
                finally
                {
                    results.Dispose();
                }
            }
            else
            {
                openDialog.GetResult(out IShellItem? item);
                if (item is null)
                {
                    return Array.Empty<string>();
                }

                return new string[] { GetFilePathFromShellItem(item) };
            }
        }

        private protected override WinFormsComWrappers.FileDialogWrapper CreateVistaDialog()
        {
            HRESULT hr = Ole32.CoCreateInstance(
                in CLSID.FileOpenDialog,
                IntPtr.Zero,
                Ole32.CLSCTX.INPROC_SERVER | Ole32.CLSCTX.LOCAL_SERVER | Ole32.CLSCTX.REMOTE_SERVER,
                in NativeMethods.ActiveX.IID_IUnknown,
                out IntPtr lpDialogUnknownPtr);
            if (!hr.Succeeded)
            {
                Marshal.ThrowExceptionForHR((int)hr);
            }

            var obj = WinFormsComWrappers.Instance
                .GetOrCreateObjectForComInstance(lpDialogUnknownPtr, CreateObjectFlags.None);
            return (WinFormsComWrappers.FileDialogWrapper)obj;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SafeFileName
        {
            get
            {
                string fullPath = FileName;
                if (string.IsNullOrEmpty(fullPath))
                {
                    return string.Empty;
                }

                return RemoveSensitivePathInformation(fullPath);
            }
        }

        private static string RemoveSensitivePathInformation(string fullPath)
        {
            return Path.GetFileName(fullPath);
        }

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
                    safePaths[i] = RemoveSensitivePathInformation(fullPaths[i]);
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
