// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.IO;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a common dialog box
    ///  that displays the control that allows the user to open a file. This class
    ///  cannot be inherited.
    /// </summary>
    [SRDescription(nameof(SR.DescriptionOpenFileDialog))]
    public sealed class OpenFileDialog : FileDialog
    {
        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box displays a
        ///  warning if the user specifies a file name that does not exist.
        /// </summary>
        [
        DefaultValue(true),
        SRDescription(nameof(SR.OFDcheckFileExistsDescr))
        ]
        public override bool CheckFileExists
        {
            get
            {
                return base.CheckFileExists;
            }
            set
            {
                base.CheckFileExists = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value
        ///  indicating whether the dialog box allows multiple files to be selected.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.OFDmultiSelectDescr))
        ]
        public bool Multiselect
        {
            get
            {
                return GetOption(NativeMethods.OFN_ALLOWMULTISELECT);
            }
            set
            {
                SetOption(NativeMethods.OFN_ALLOWMULTISELECT, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether
        ///  the read-only check box is selected.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.OFDreadOnlyCheckedDescr))
        ]
        public bool ReadOnlyChecked
        {
            get
            {
                return GetOption(NativeMethods.OFN_READONLY);
            }
            set
            {
                SetOption(NativeMethods.OFN_READONLY, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog contains a read-only check box.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.OFDshowReadOnlyDescr))
        ]
        public bool ShowReadOnly
        {
            get
            {
                return !GetOption(NativeMethods.OFN_HIDEREADONLY);
            }
            set
            {
                SetOption(NativeMethods.OFN_HIDEREADONLY, !value);
            }
        }

        /// <summary>
        ///  Opens the file selected by the user with read-only permission.  The file
        ///  attempted is specified by the <see cref='FileDialog.FileName'/> property.
        /// </summary>
        public Stream OpenFile()
        {
            string filename = FileNames[0];
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(FileName));
            }

            return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        ///  Resets all properties to their default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            SetOption(NativeMethods.OFN_FILEMUSTEXIST, true);
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
                //
                int errorCode = SafeNativeMethods.CommDlgExtendedError();
                switch (errorCode)
                {
                    case NativeMethods.FNERR_INVALIDFILENAME:
                        throw new InvalidOperationException(string.Format(SR.FileDialogInvalidFileName, FileName));

                    case NativeMethods.FNERR_SUBCLASSFAILURE:
                        throw new InvalidOperationException(SR.FileDialogSubLassFailure);

                    case NativeMethods.FNERR_BUFFERTOOSMALL:
                        throw new InvalidOperationException(SR.FileDialogBufferTooSmall);
                }
            }
            return result;
        }

        private protected override string[] ProcessVistaFiles(FileDialogNative.IFileDialog dialog)
        {
            FileDialogNative.IFileOpenDialog openDialog = (FileDialogNative.IFileOpenDialog)dialog;
            if (Multiselect)
            {
                openDialog.GetResults(out FileDialogNative.IShellItemArray results);
                results.GetCount(out uint count);
                string[] files = new string[count];
                for (uint i = 0; i < count; ++i)
                {
                    results.GetItemAt(i, out FileDialogNative.IShellItem item);
                    files[unchecked((int)i)] = GetFilePathFromShellItem(item);
                }
                return files;
            }
            else
            {
                openDialog.GetResult(out FileDialogNative.IShellItem item);
                return new string[] { GetFilePathFromShellItem(item) };
            }
        }

        private protected override FileDialogNative.IFileDialog CreateVistaDialog()
        {
            return new FileDialogNative.NativeFileOpenDialog();
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
                    return "";
                }

                string safePath = RemoveSensitivePathInformation(fullPath);
                return safePath;
            }
        }

        private static string RemoveSensitivePathInformation(string fullPath)
        {
            return System.IO.Path.GetFileName(fullPath);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] SafeFileNames
        {
            get
            {
                string[] fullPaths = FileNames;
                if (null == fullPaths || 0 == fullPaths.Length)
                { return Array.Empty<string>(); }
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
