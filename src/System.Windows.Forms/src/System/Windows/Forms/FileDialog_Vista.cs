// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using static Interop;
using static Interop.Shell32;

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

        private protected abstract IFileDialog CreateVistaDialog();

        private bool TryRunDialogVista(IntPtr hWndOwner, out bool returnValue)
        {
            IFileDialog dialog;
            try
            {
                // Creating the Vista dialog can fail on Windows Server Core, even if the
                // Server Core App Compatibility FOD is installed.
                dialog = CreateVistaDialog();
            }
            catch (COMException)
            {
                returnValue = false;
                return false;
            }

            OnBeforeVistaDialog(dialog);
            var events = new VistaDialogEvents(this);
            dialog.Advise(events, out uint eventCookie);
            try
            {
                returnValue = dialog.Show(hWndOwner) == 0;
                return true;
            }
            finally
            {
                dialog.Unadvise(eventCookie);
                // Make sure that the event interface doesn't get collected
                GC.KeepAlive(events);
            }
        }

        private void OnBeforeVistaDialog(IFileDialog dialog)
        {
            if (ClientGuid is { } clientGuid)
            {
                // IFileDialog::SetClientGuid should be called immediately after creation of the dialog object.
                // https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ifiledialog-setclientguid#remarks
                dialog.SetClientGuid(clientGuid);
            }

            dialog.SetDefaultExtension(DefaultExt);
            dialog.SetFileName(FileName);

            if (!string.IsNullOrEmpty(InitialDirectory))
            {
                try
                {
                    IShellItem initialDirectory = GetShellItemForPath(InitialDirectory);

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

        private FOS GetOptions()
        {
            const FOS BlittableOptions =
                FOS.OVERWRITEPROMPT
              | FOS.NOCHANGEDIR
              | FOS.NOVALIDATE
              | FOS.ALLOWMULTISELECT
              | FOS.PATHMUSTEXIST
              | FOS.FILEMUSTEXIST
              | FOS.CREATEPROMPT
              | FOS.NODEREFERENCELINKS;

            const int UnexpectedOptions =
                (int)(Comdlg32.OFN.SHOWHELP // If ShowHelp is true, we don't use the Vista Dialog
                | Comdlg32.OFN.ENABLEHOOK // These shouldn't be set in options (only set in the flags for the legacy dialog)
                | Comdlg32.OFN.ENABLESIZING // These shouldn't be set in options (only set in the flags for the legacy dialog)
                | Comdlg32.OFN.EXPLORER); // These shouldn't be set in options (only set in the flags for the legacy dialog)

            Debug.Assert((UnexpectedOptions & _options) == 0, "Unexpected FileDialog options");

            FOS ret = (FOS)_options & BlittableOptions;

            // Force no mini mode for the SaveFileDialog
            ret |= FOS.DEFAULTNOMINIMODE;

            // Make sure that the Open dialog allows the user to specify
            // non-file system locations. This flag will cause the dialog to copy the resource
            // to a local cache (Temporary Internet Files), and return that path instead. This
            // also affects the Save dialog by disallowing navigation to these areas.
            // An example of a non-file system location is a URL (http://), or a file stored on
            // a digital camera that is not mapped to a drive letter.
            // This reproduces the behavior of the "classic" Open and Save dialogs.
            ret |= FOS.FORCEFILESYSTEM;

            return ret;
        }

        private protected abstract string[] ProcessVistaFiles(IFileDialog dialog);

        private bool HandleVistaFileOk(IFileDialog dialog)
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
                    if ((_options & (int)Comdlg32.OFN.HIDEREADONLY) != 0)
                    {
                        // When the dialog is dismissed OK, the Readonly bit can't
                        // be left on if ShowReadOnly was false
                        // Downlevel this happens automatically, on Vista mode, we need to watch out for it.
                        _options &= ~(int)Comdlg32.OFN.READONLY;
                    }
                }
            }
            return ok;
        }

        private class VistaDialogEvents : IFileDialogEvents
        {
            private readonly FileDialog _dialog;

            public VistaDialogEvents(FileDialog dialog)
            {
                _dialog = dialog;
            }

            public HRESULT OnFileOk(IFileDialog pfd)
            {
                return _dialog.HandleVistaFileOk(pfd) ? HRESULT.S_OK : HRESULT.S_FALSE;
            }

            public HRESULT OnFolderChanging(IFileDialog pfd, IShellItem psiFolder)
            {
                return HRESULT.S_OK;
            }

            public HRESULT OnFolderChange(IFileDialog pfd)
            {
                return HRESULT.S_OK;
            }

            public HRESULT OnSelectionChange(IFileDialog pfd)
            {
                return HRESULT.S_OK;
            }

            public unsafe HRESULT OnShareViolation(IFileDialog pfd, IShellItem psi, FDESVR* pResponse)
            {
                if (pResponse is null)
                {
                    return HRESULT.E_POINTER;
                }

                *pResponse = FDESVR.DEFAULT;
                return HRESULT.S_OK;
            }

            public HRESULT OnTypeChange(IFileDialog pfd)
            {
                return HRESULT.S_OK;
            }

            public unsafe HRESULT OnOverwrite(IFileDialog pfd, IShellItem psi, FDEOR* pResponse)
            {
                if (pResponse is null)
                {
                    return HRESULT.E_POINTER;
                }

                *pResponse = FDEOR.DEFAULT;
                return HRESULT.S_OK;
            }
        }

        private void SetFileTypes(IFileDialog dialog)
        {
            COMDLG_FILTERSPEC[] filterItems = FilterItems;
            HRESULT hr = dialog.SetFileTypes((uint)filterItems.Length, filterItems);
            if (!hr.Succeeded())
            {
                throw Marshal.GetExceptionForHR((int)hr);
            }

            if (filterItems.Length > 0)
            {
                hr = dialog.SetFileTypeIndex(unchecked((uint)FilterIndex));
                if (!hr.Succeeded())
                {
                    throw Marshal.GetExceptionForHR((int)hr);
                }
            }
        }

        private COMDLG_FILTERSPEC[] FilterItems => GetFilterItems(_filter);

        private static COMDLG_FILTERSPEC[] GetFilterItems(string filter)
        {
            // Expected input types
            // "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            // "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
            var extensions = new List<COMDLG_FILTERSPEC>();
            if (!string.IsNullOrEmpty(filter))
            {
                string[] tokens = filter.Split('|');
                if (0 == tokens.Length % 2)
                {
                    // All even numbered tokens should be labels
                    // Odd numbered tokens are the associated extensions
                    for (int i = 1; i < tokens.Length; i += 2)
                    {
                        COMDLG_FILTERSPEC extension;
                        extension.pszSpec = tokens[i];// This may be a semicolon delimeted list of extensions (that's ok)
                        extension.pszName = tokens[i - 1];
                        extensions.Add(extension);
                    }
                }
            }
            return extensions.ToArray();
        }

        private protected static string GetFilePathFromShellItem(IShellItem item)
        {
            HRESULT hr = item.GetDisplayName(SIGDN.DESKTOPABSOLUTEPARSING, out string filename);
            if (!hr.Succeeded())
            {
                throw Marshal.GetExceptionForHR((int)hr);
            }

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
