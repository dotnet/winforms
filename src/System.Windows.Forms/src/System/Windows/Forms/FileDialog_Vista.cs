// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Diagnostics.CodeAnalysis;

    public partial class FileDialog
    {
        private bool _autoUpgradeEnabled = true;

        internal virtual bool SettingsSupportVistaDialog
        {
            get
            {
                return !this.ShowHelp &&
                        (Application.VisualStyleState == VisualStyles.VisualStyleState.ClientAreaEnabled ||
                         Application.VisualStyleState == VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled);
            }
        }


        [SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts")]
        internal bool UseVistaDialogInternal
        {
            get 
            {
                if (UnsafeNativeMethods.IsVista && this._autoUpgradeEnabled && SettingsSupportVistaDialog)
                {
                    // 

                    new EnvironmentPermission(PermissionState.Unrestricted).Assert();
                    try
                    {
                        return SystemInformation.BootMode == BootMode.Normal;
                    }
                    finally
                    {
                        CodeAccessPermission.RevertAssert();
                    }
                }

                return false;
            }
        }

        internal abstract FileDialogNative.IFileDialog CreateVistaDialog();

        [
            SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode),
            SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive")
        ]
        private bool RunDialogVista(IntPtr hWndOwner)
        {
            FileDialogNative.IFileDialog dialog = CreateVistaDialog();
            OnBeforeVistaDialog(dialog);
            VistaDialogEvents events = new VistaDialogEvents(this);
            uint eventCookie;
            dialog.Advise(events, out eventCookie);
            try
            {
                int result = dialog.Show(hWndOwner);
                return 0 == result;
            }
            finally
            {
                dialog.Unadvise(eventCookie);
                //Make sure that the event interface doesn't get collected
                GC.KeepAlive(events);
            }
        }

        internal virtual void OnBeforeVistaDialog(FileDialogNative.IFileDialog dialog)
        {
            dialog.SetDefaultExtension(this.DefaultExt);

            dialog.SetFileName(this.FileName);

            if (!string.IsNullOrEmpty(this.InitialDirectory))
            {
                try
                {
                    FileDialogNative.IShellItem initialDirectory = GetShellItemForPath(this.InitialDirectory);

                    dialog.SetDefaultFolder(initialDirectory);
                    dialog.SetFolder(initialDirectory);
                }
                catch (FileNotFoundException)
                {
                }
            }

            dialog.SetTitle(this.Title);

            dialog.SetOptions(GetOptions());
            
            SetFileTypes(dialog);

            this._customPlaces.Apply(dialog);
        }
        
        private FileDialogNative.FOS GetOptions()
        {
            const FileDialogNative.FOS BlittableOptions = 
                FileDialogNative.FOS.FOS_OVERWRITEPROMPT
              | FileDialogNative.FOS.FOS_NOCHANGEDIR
              | FileDialogNative.FOS.FOS_NOVALIDATE
              | FileDialogNative.FOS.FOS_ALLOWMULTISELECT
              | FileDialogNative.FOS.FOS_PATHMUSTEXIST
              | FileDialogNative.FOS.FOS_FILEMUSTEXIST
              | FileDialogNative.FOS.FOS_CREATEPROMPT
              | FileDialogNative.FOS.FOS_NODEREFERENCELINKS
            ;
            const int UnexpectedOptions = 
                NativeMethods.OFN_USESHELLITEM //This is totally bogus (only used in FileDialog by accident to ensure that places are shown
              | NativeMethods.OFN_SHOWHELP //If ShowHelp is true, we don't use the Vista Dialog
              | NativeMethods.OFN_ENABLEHOOK //These shouldn't be set in options (only set in the flags for the legacy dialog)
              | NativeMethods.OFN_ENABLESIZING //These shouldn't be set in options (only set in the flags for the legacy dialog)
              | NativeMethods.OFN_EXPLORER //These shouldn't be set in options (only set in the flags for the legacy dialog)
            ;
            System.Diagnostics.Debug.Assert(0==(UnexpectedOptions & options), "Unexpected FileDialog options");

            FileDialogNative.FOS ret = (FileDialogNative.FOS)options & BlittableOptions;

            //Force no mini mode for the SaveFileDialog
            ret |= FileDialogNative.FOS.FOS_DEFAULTNOMINIMODE;

            // Make sure that the Open dialog allows the user to specify
            // non-file system locations. This flag will cause the dialog to copy the resource
            // to a local cache (Temporary Internet Files), and return that path instead. This
            // also affects the Save dialog by disallowing navigation to these areas.
            // An example of a non-file system location is a URL (http://), or a file stored on
            // a digital camera that is not mapped to a drive letter.
            // This reproduces the behavior of the "classic" Open and Save dialogs.
            ret |= FileDialogNative.FOS.FOS_FORCEFILESYSTEM;

            return ret;
        }

        internal abstract string[] ProcessVistaFiles(FileDialogNative.IFileDialog dialog);

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool HandleVistaFileOk(FileDialogNative.IFileDialog dialog)
        {
            int saveOptions = options;
            int saveFilterIndex = filterIndex;
            string[] saveFileNames = fileNames;
            bool saveSecurityCheckFileNames = securityCheckFileNames;
            bool ok = false;

            try
            {
                securityCheckFileNames = true;
                Thread.MemoryBarrier();
                uint filterIndexTemp;
                dialog.GetFileTypeIndex(out filterIndexTemp);
                filterIndex = unchecked((int)filterIndexTemp);
                fileNames = ProcessVistaFiles(dialog);
                if (ProcessFileNames())
                {
                    CancelEventArgs ceevent = new CancelEventArgs();
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
                    //Order here is important.  We don't want a window where securityCheckFileNames is false, but the temporary fileNames is still in place
                    securityCheckFileNames = saveSecurityCheckFileNames;
                    Thread.MemoryBarrier();
                    fileNames = saveFileNames;

                    options = saveOptions;
                    filterIndex = saveFilterIndex;
                }
                else
                {
                    if (0 != (options & NativeMethods.OFN_HIDEREADONLY))
                    {
                        //When the dialog is dismissed OK, the Readonly bit can't
                        // be left on if ShowReadOnly was false
                        // Downlevel this happens automatically, on Vista mode, we need to watch out for it.

                        options &= ~ NativeMethods.OFN_READONLY;
                    }
                }
            }
            return ok;
        }

        private class VistaDialogEvents : FileDialogNative.IFileDialogEvents
        {
            private FileDialog _dialog;

            public VistaDialogEvents(FileDialog dialog)
            { 
                this._dialog = dialog;
            }

            public int OnFileOk(FileDialogNative.IFileDialog pfd)
            {
                return this._dialog.HandleVistaFileOk(pfd) ? NativeMethods.S_OK : NativeMethods.S_FALSE;
            }

            public int OnFolderChanging(FileDialogNative.IFileDialog pfd, FileDialogNative.IShellItem psiFolder)
            {
                return NativeMethods.S_OK;
            }

            public void OnFolderChange(FileDialogNative.IFileDialog pfd)
            {
            }

            public void OnSelectionChange(FileDialogNative.IFileDialog pfd)
            {
            }

            public void OnShareViolation(FileDialogNative.IFileDialog pfd, FileDialogNative.IShellItem psi, out FileDialogNative.FDE_SHAREVIOLATION_RESPONSE pResponse)
            {
                pResponse = FileDialogNative.FDE_SHAREVIOLATION_RESPONSE.FDESVR_DEFAULT;
            }

            public void OnTypeChange(FileDialogNative.IFileDialog pfd)
            {
            }

            public void OnOverwrite(FileDialogNative.IFileDialog pfd, FileDialogNative.IShellItem psi, out FileDialogNative.FDE_OVERWRITE_RESPONSE pResponse)
            {
                pResponse = FileDialogNative.FDE_OVERWRITE_RESPONSE.FDEOR_DEFAULT;
            }
        }

        private void SetFileTypes(FileDialogNative.IFileDialog dialog)
        {
            FileDialogNative.COMDLG_FILTERSPEC[] filterItems = FilterItems;
            dialog.SetFileTypes((uint)filterItems.Length, filterItems);
            if (filterItems.Length > 0)
            {
                dialog.SetFileTypeIndex(unchecked((uint)filterIndex));
            }
        }

        private FileDialogNative.COMDLG_FILTERSPEC[] FilterItems
        {
            get
            {
                return GetFilterItems(this.filter);
            }
        }

        private static FileDialogNative.COMDLG_FILTERSPEC[] GetFilterItems(string filter)
        {
            //Expected input types 
            //"Text files (*.txt)|*.txt|All files (*.*)|*.*"
            //"Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
            List<FileDialogNative.COMDLG_FILTERSPEC> extensions = new List<FileDialogNative.COMDLG_FILTERSPEC>();
            if (!string.IsNullOrEmpty(filter))
            {
                string[] tokens = filter.Split('|');
                if (0 == tokens.Length % 2)
                {
                    //All even numbered tokens should be labels
                    //Odd numbered tokens are the associated extensions
                    for (int i = 1; i < tokens.Length; i += 2)
                    {
                        FileDialogNative.COMDLG_FILTERSPEC extension;
                        extension.pszSpec = tokens[i];//This may be a semicolon delimeted list of extensions (that's ok)
                        extension.pszName = tokens[i - 1];
                        extensions.Add(extension);
                    }
                }
            }
            return extensions.ToArray();
        }

        internal static FileDialogNative.IShellItem GetShellItemForPath(string path)
        {
            FileDialogNative.IShellItem ret = null;
            IntPtr pidl = IntPtr.Zero;
            uint zero = 0;
            if (0 <= UnsafeNativeMethods.Shell32.SHILCreateFromPath(path, out pidl, ref zero))
            {
                if (0 <= UnsafeNativeMethods.Shell32.SHCreateShellItem(
                    IntPtr.Zero, //No parent specified
                    IntPtr.Zero,
                    pidl,
                    out ret))
                {
                    return ret;
                }
            }
            throw new System.IO.FileNotFoundException();
        }

        internal static string GetFilePathFromShellItem(FileDialogNative.IShellItem item)
        {
            string filename;
            item.GetDisplayName(FileDialogNative.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out filename);
            return filename;
        }

        private FileDialogCustomPlacesCollection _customPlaces = new FileDialogCustomPlacesCollection();

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public FileDialogCustomPlacesCollection CustomPlaces
        {
            get
            {
                return this._customPlaces;
            }
        }

        /// <summary>
        /// Gets or Sets whether the dialog will be automatically upgraded to enable new features.
        /// </summary>
        [
            DefaultValue(true)
        ]
        public bool AutoUpgradeEnabled
        {
            get
            {
                return this._autoUpgradeEnabled;
            }
            set
            {
                this._autoUpgradeEnabled = value;
            }
        }
    }
}
