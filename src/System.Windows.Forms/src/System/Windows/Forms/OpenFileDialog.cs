// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{

    using System.Diagnostics;

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using CodeAccessPermission = System.Security.CodeAccessPermission;
    using System.Security.Permissions;
    using System.IO;
    using System.ComponentModel;
    using Microsoft.Win32;
    using System.Runtime.Versioning;

    /// <include file='doc\OpenFileDialog.uex' path='docs/doc[@for="OpenFileDialog"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents a common dialog box
    ///       that displays the control that allows the user to open a file. This class
    ///       cannot be inherited.
    ///    </para>
    /// </devdoc>
    [SRDescription(nameof(SR.DescriptionOpenFileDialog))]
    public sealed class OpenFileDialog : FileDialog
    {

        /// <include file='doc\OpenFileDialog.uex' path='docs/doc[@for="OpenFileDialog.CheckFileExists"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the dialog box displays a
        ///       warning if the user specifies a file name that does not exist.
        ///    </para>
        /// </devdoc>
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

        /// <include file='doc\OpenFileDialog.uex' path='docs/doc[@for="OpenFileDialog.Multiselect"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value
        ///       indicating whether the dialog box allows multiple files to be selected.
        ///    </para>
        /// </devdoc>
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

        /// <include file='doc\OpenFileDialog.uex' path='docs/doc[@for="OpenFileDialog.ReadOnlyChecked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether
        ///       the read-only check box is selected.
        ///    </para>
        /// </devdoc>
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

        /// <include file='doc\OpenFileDialog.uex' path='docs/doc[@for="OpenFileDialog.ShowReadOnly"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the dialog contains a read-only check box.
        ///    </para>
        /// </devdoc>
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

        /// <include file='doc\OpenFileDialog.uex' path='docs/doc[@for="OpenFileDialog.OpenFile"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Opens the file selected by the user with read-only permission.  The file
        ///       attempted is specified by the <see cref='System.Windows.Forms.FileDialog.FileName'/> property.
        ///    </para>
        /// </devdoc>        
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        /// 


        [SuppressMessage("Microsoft.Security", "CA2103:ReviewImperativeSecurity")]
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        public Stream OpenFile()
        {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogOpenFile Demanded");
            IntSecurity.FileDialogOpenFile.Demand();

            string filename = FileNamesInternal[0];

            if (filename == null || (filename.Length == 0))
                throw new ArgumentNullException(nameof(FileName));

            Stream s = null;

            // 



            new FileIOPermission(FileIOPermissionAccess.Read, IntSecurity.UnsafeGetFullPath(filename)).Assert();
            try
            {
                s = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            return s;
        }

        /// <include file='doc\OpenFileDialog.uex' path='docs/doc[@for="OpenFileDialog.Reset"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Resets all properties to their default values.
        ///    </para>
        /// </devdoc>
        public override void Reset()
        {
            base.Reset();
            SetOption(NativeMethods.OFN_FILEMUSTEXIST, true);
        }

        internal override void EnsureFileDialogPermission()
        {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogOpenFile Demanded in OpenFileDialog.RunFileDialog");
            IntSecurity.FileDialogOpenFile.Demand();
        }

        /// <include file='doc\OpenFileDialog.uex' path='docs/doc[@for="OpenFileDialog.RunFileDialog"]/*' />
        /// <devdoc>
        ///     Displays a file open dialog.
        /// </devdoc>
        /// <internalonly/>
        internal override bool RunFileDialog(NativeMethods.OPENFILENAME_I ofn)
        {
            //We have already done the demand in EnsureFileDialogPermission but it doesn't hurt to do it again
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogOpenFile Demanded in OpenFileDialog.RunFileDialog");
            IntSecurity.FileDialogOpenFile.Demand();

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

        internal override string[] ProcessVistaFiles(FileDialogNative.IFileDialog dialog)
        {
            FileDialogNative.IFileOpenDialog openDialog = (FileDialogNative.IFileOpenDialog)dialog;
            if (Multiselect)
            {
                FileDialogNative.IShellItemArray results;
                openDialog.GetResults(out results);
                uint count;
                results.GetCount(out count);
                string[] files = new string[count];
                for (uint i = 0; i < count; ++i)
                { 
                    FileDialogNative.IShellItem item;
                    results.GetItemAt(i, out item);
                    files[unchecked((int)i)] = GetFilePathFromShellItem(item);
                }
                return files;
            }
            else
            { 
                FileDialogNative.IShellItem item;
                openDialog.GetResult(out item);
                return new string[] { GetFilePathFromShellItem(item) };
            }
        }

        internal override FileDialogNative.IFileDialog CreateVistaDialog()
        {
            return new FileDialogNative.NativeFileOpenDialog();
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts")
        ]
        public string SafeFileName
        {
            get
            {
                new FileIOPermission(PermissionState.Unrestricted).Assert();
                string fullPath = FileName;
                CodeAccessPermission.RevertAssert();
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

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"),
            SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts")
        ]
        public string[] SafeFileNames
        {
            get
            {
                new FileIOPermission(PermissionState.Unrestricted).Assert();
                string[] fullPaths = FileNames;
                CodeAccessPermission.RevertAssert();
                if (null == fullPaths || 0 == fullPaths.Length)
                { return new string[0]; }
                string[] safePaths = new string[fullPaths.Length];
                for (int i = 0; i < safePaths.Length; ++i)
                {
                    safePaths[i] = RemoveSensitivePathInformation(fullPaths[i]);
                }
                return safePaths;
            }
        }

        internal override bool SettingsSupportVistaDialog
        { 
            get
            {
                return base.SettingsSupportVistaDialog && !this.ShowReadOnly;
            }
        }
    }
}
