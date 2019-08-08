// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace System.Windows.Forms
{
    public partial class FileDialog
    {
        private protected virtual bool SettingsSupportVistaDialog
        {
            get => !ShowHelp && ((Application.VisualStyleState & VisualStyles.VisualStyleState.ClientAreaEnabled) == VisualStyles.VisualStyleState.ClientAreaEnabled);
        }

        internal bool UseVistaDialogInternal
        {
            get
            {
                if (AutoUpgradeEnabled && SettingsSupportVistaDialog)
                {
                    return SystemInformation.BootMode == BootMode.Normal;
                }

                return false;
            }
        }

        private protected abstract FileDialogNative.IFileDialog CreateVistaDialog();

        private bool RunDialogVista(IntPtr hWndOwner)
        {
            FileDialogNative.IFileDialog dialog = CreateVistaDialog();
            OnBeforeVistaDialog(dialog);
            var events = new VistaDialogEvents(this);
            dialog.Advise(events, out uint eventCookie);
            try
            {
                return dialog.Show(hWndOwner) == 0;
            }
            finally
            {
                dialog.Unadvise(eventCookie);
                // Make sure that the event interface doesn't get collected
                GC.KeepAlive(events);
            }
        }

        private void OnBeforeVistaDialog(FileDialogNative.IFileDialog dialog)
        {
            dialog.SetDefaultExtension(DefaultExt);
            dialog.SetFileName(FileName);

            if (!string.IsNullOrEmpty(InitialDirectory))
            {
                try
                {
                    FileDialogNative.IShellItem initialDirectory = GetShellItemForPath(InitialDirectory);

                    dialog.SetDefaultFolder(initialDirectory);
                    dialog.SetFolder(initialDirectory);
                }
                catch (FileNotFoundException)
                {
                }
            }

            dialog.SetTitle(Title);
            dialog.SetOptions(GetOptions());
            SetFileTypes(dialog);

            _customPlaces.Apply(dialog);
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
                NativeMethods.OFN_USESHELLITEM // This is totally bogus (only used in FileDialog by accident to ensure that places are shown
              | NativeMethods.OFN_SHOWHELP // If ShowHelp is true, we don't use the Vista Dialog
              | NativeMethods.OFN_ENABLEHOOK // These shouldn't be set in options (only set in the flags for the legacy dialog)
              | NativeMethods.OFN_ENABLESIZING // These shouldn't be set in options (only set in the flags for the legacy dialog)
              | NativeMethods.OFN_EXPLORER // These shouldn't be set in options (only set in the flags for the legacy dialog)
            ;
            Debug.Assert((UnexpectedOptions & _options) == 0, "Unexpected FileDialog options");

            FileDialogNative.FOS ret = (FileDialogNative.FOS)_options & BlittableOptions;

            // Force no mini mode for the SaveFileDialog
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

        private protected abstract string[] ProcessVistaFiles(FileDialogNative.IFileDialog dialog);

        private bool HandleVistaFileOk(FileDialogNative.IFileDialog dialog)
        {
            int saveOptions = _options;
            int saveFilterIndex = FilterIndex;
            string[] saveFileNames = _fileNames;
            bool ok = false;

            try
            {
                Thread.MemoryBarrier();
                dialog.GetFileTypeIndex(out uint filterIndexTemp);
                FilterIndex = unchecked((int)filterIndexTemp);
                _fileNames = ProcessVistaFiles(dialog);
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
                    Thread.MemoryBarrier();
                    _fileNames = saveFileNames;

                    _options = saveOptions;
                    FilterIndex = saveFilterIndex;
                }
                else
                {
                    if ((_options & NativeMethods.OFN_HIDEREADONLY) != 0)
                    {
                        // When the dialog is dismissed OK, the Readonly bit can't
                        // be left on if ShowReadOnly was false
                        // Downlevel this happens automatically, on Vista mode, we need to watch out for it.
                        _options &= ~NativeMethods.OFN_READONLY;
                    }
                }
            }
            return ok;
        }

        private class VistaDialogEvents : FileDialogNative.IFileDialogEvents
        {
            private readonly FileDialog _dialog;

            public VistaDialogEvents(FileDialog dialog)
            {
                _dialog = dialog;
            }

            public int OnFileOk(FileDialogNative.IFileDialog pfd)
            {
                return _dialog.HandleVistaFileOk(pfd) ? NativeMethods.S_OK : NativeMethods.S_FALSE;
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
                dialog.SetFileTypeIndex(unchecked((uint)FilterIndex));
            }
        }

        private FileDialogNative.COMDLG_FILTERSPEC[] FilterItems => GetFilterItems(_filter);

        private static FileDialogNative.COMDLG_FILTERSPEC[] GetFilterItems(string filter)
        {
            // Expected input types
            // "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            // "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
            var extensions = new List<FileDialogNative.COMDLG_FILTERSPEC>();
            if (!string.IsNullOrEmpty(filter))
            {
                string[] tokens = filter.Split('|');
                if (0 == tokens.Length % 2)
                {
                    // All even numbered tokens should be labels
                    // Odd numbered tokens are the associated extensions
                    for (int i = 1; i < tokens.Length; i += 2)
                    {
                        FileDialogNative.COMDLG_FILTERSPEC extension;
                        extension.pszSpec = tokens[i];// This may be a semicolon delimeted list of extensions (that's ok)
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
            if (UnsafeNativeMethods.Shell32.SHILCreateFromPath(path, out pidl, ref zero) >= 0)
            {
                if (UnsafeNativeMethods.Shell32.SHCreateShellItem(
                    IntPtr.Zero, // No parent specified
                    IntPtr.Zero,
                    pidl,
                    out ret) >= 0)
                {
                    return ret;
                }
            }

            throw new FileNotFoundException();
        }

        private protected static string GetFilePathFromShellItem(FileDialogNative.IShellItem item)
        {
            item.GetDisplayName(FileDialogNative.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out string filename);
            return filename;
        }

        private readonly FileDialogCustomPlacesCollection _customPlaces = new FileDialogCustomPlacesCollection();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileDialogCustomPlacesCollection CustomPlaces => _customPlaces;

        /// <summary>
        ///  Gets or sets whether the dialog will be automatically upgraded to enable new features.
        /// </summary>
        [DefaultValue(true)]
        public bool AutoUpgradeEnabled { get; set; } = true;
    }
}
