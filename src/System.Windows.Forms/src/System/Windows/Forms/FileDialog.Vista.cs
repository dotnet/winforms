// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

        private protected abstract Interop.WinFormsComWrappers.FileDialogWrapper CreateVistaDialog();

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
                // https://docs.microsoft.com/windows/win32/api/shobjidl_core/nf-shobjidl_core-ifiledialog-setclientguid#remarks
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
              | FOS.NODEREFERENCELINKS
              | FOS.DONTADDTORECENT
              | FOS.NOREADONLYRETURN
              | FOS.NOTESTFILECREATE
              | FOS.FORCESHOWHIDDEN
              | FOS.DEFAULTNOMINIMODE
              | FOS.OKBUTTONNEEDSINTERACTION
              | FOS.HIDEPINNEDPLACES
              | FOS.FORCEPREVIEWPANEON;

            const int UnexpectedOptions =
                (int)(Comdlg32.OFN.SHOWHELP // If ShowHelp is true, we don't use the Vista Dialog
                | Comdlg32.OFN.ENABLEHOOK // These shouldn't be set in options (only set in the flags for the legacy dialog)
                | Comdlg32.OFN.ENABLESIZING // These shouldn't be set in options (only set in the flags for the legacy dialog)
                | Comdlg32.OFN.EXPLORER); // These shouldn't be set in options (only set in the flags for the legacy dialog)

            Debug.Assert((UnexpectedOptions & _options) == 0, "Unexpected FileDialog options");

            FOS ret = (FOS)_options & BlittableOptions;

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

        private protected abstract string[] ProcessVistaFiles(WinFormsComWrappers.FileDialogWrapper dialog);

        private bool HandleVistaFileOk(WinFormsComWrappers.FileDialogWrapper dialog)
        {
            int saveOptions = _options;
            int saveFilterIndex = FilterIndex;
            string[]? saveFileNames = _fileNames;
            bool ok = false;

            try
            {
                Thread.MemoryBarrier();
                dialog.GetFileTypeIndex(out uint filterIndexTemp);
                FilterIndex = unchecked((int)filterIndexTemp);
                _fileNames = ProcessVistaFiles(dialog);
                if (ProcessFileNames(_fileNames))
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

        private void SetFileTypes(IFileDialog dialog)
        {
            COMDLG_FILTERSPEC[] filterItems = GetFilterItems(_filter);
            try
            {
                HRESULT hr = dialog.SetFileTypes((uint)filterItems.Length, filterItems);
                hr.ThrowOnFailure();

                if (filterItems.Length > 0)
                {
                    hr = dialog.SetFileTypeIndex(unchecked((uint)FilterIndex));
                    hr.ThrowOnFailure();
                }
            }
            finally
            {
                foreach (var item in filterItems)
                {
                    Marshal.FreeCoTaskMem(item.pszName);
                    Marshal.FreeCoTaskMem(item.pszSpec);
                }
            }
        }

        private static COMDLG_FILTERSPEC[] GetFilterItems(string? filter)
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
                        extension.pszSpec = Marshal.StringToCoTaskMemUni(tokens[i]);        // This may be a semicolon delimited list of extensions (that's ok)
                        extension.pszName = Marshal.StringToCoTaskMemUni(tokens[i - 1]);
                        extensions.Add(extension);
                    }
                }
            }

            return extensions.ToArray();
        }

        private protected static string GetFilePathFromShellItem(IShellItem item)
        {
            HRESULT hr = item.GetDisplayName(SIGDN.DESKTOPABSOLUTEPARSING, out string? filename);
            hr.ThrowOnFailure();
            return filename!;
        }

        private readonly FileDialogCustomPlacesCollection _customPlaces = new();

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
            get => GetOption((int)FOS.OKBUTTONNEEDSINTERACTION);
            set => SetOption((int)FOS.OKBUTTONNEEDSINTERACTION, value);
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
            get => !GetOption((int)FOS.HIDEPINNEDPLACES);
            set => SetOption((int)FOS.HIDEPINNEDPLACES, !value);
        }
    }
}
