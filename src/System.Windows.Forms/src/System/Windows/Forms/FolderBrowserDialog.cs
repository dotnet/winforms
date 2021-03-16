﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Buffers;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using static Interop;
using static Interop.Shell32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a common dialog box that allows the user to specify options for
    ///  selecting a folder. This class cannot be inherited.
    /// </summary>
    [DefaultEvent(nameof(HelpRequest))]
    [DefaultProperty(nameof(SelectedPath))]
    [Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, " + AssemblyRef.SystemDesign),]
    [SRDescription(nameof(SR.DescriptionFolderBrowserDialog))]
    public sealed class FolderBrowserDialog : CommonDialog
    {
        // Root node of the tree view.
        private Environment.SpecialFolder _rootFolder;

        // Description text to show.
        private string _descriptionText;

        // Folder picked by the user.
        private string _selectedPath;

        // Initial folder.
        private string _initialDirectory;

        /// <summary>
        ///  Initializes a new instance of the <see cref='FolderBrowserDialog'/> class.
        /// </summary>
        public FolderBrowserDialog()
        {
            Reset();
        }

        /// <summary>
        ///  Gets or sets whether the dialog will be automatically upgraded to enable new features.
        /// </summary>
        [DefaultValue(true)]
        public bool AutoUpgradeEnabled { get; set; } = true;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler HelpRequest
        {
            add => base.HelpRequest += value;
            remove => base.HelpRequest -= value;
        }

        /// <summary>
        ///  Determines if the 'New Folder' button should be exposed.
        ///  This property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Localizable(false)]
        [SRCategory(nameof(SR.CatFolderBrowsing))]
        [SRDescription(nameof(SR.FolderBrowserDialogShowNewFolderButton))]
        public bool ShowNewFolderButton { get; set; }

        /// <summary>
        /// <para>
        /// Gets or sets the GUID to associate with this dialog state. Typically, state such
        /// as the last visited folder and the position and size of the dialog is persisted
        /// based on the name of the executable file. By specifying a GUID, an application can
        /// have different persisted states for different versions of the dialog within the
        /// same application (for example, an import dialog and an open dialog).
        /// </para>
        /// <para>
        /// This functionality is not available if an application is not using visual styles
        /// or if <see cref="FolderBrowserDialog.AutoUpgradeEnabled"/> is set to <see langword="false"/>.
        /// </para>
        /// </summary>
        [Localizable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid? ClientGuid { get; set; }

        /// <summary>
        ///  Gets the directory path of the folder the user picked.
        ///  Sets the directory path of the initial folder shown in the dialog box.
        /// </summary>
        [Browsable(true)]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.SelectedPathEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatFolderBrowsing))]
        [SRDescription(nameof(SR.FolderBrowserDialogSelectedPath))]
        public string SelectedPath
        {
            get => _selectedPath;
            set => _selectedPath = value ?? string.Empty;
        }

        /// <summary>
        ///  Gets or sets the initial directory displayed by the folder browser dialog.
        /// </summary>
        [SRCategory(nameof(SR.CatFolderBrowsing))]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.InitialDirectoryEditor, System.Windows.Forms.Design, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", typeof(UITypeEditor))]
        [SRDescription(nameof(SR.FDinitialDirDescr))]
        public string InitialDirectory
        {
            get => _initialDirectory;
            set => _initialDirectory = value ?? string.Empty;
        }

        /// <summary>
        ///  Gets/sets the root node of the directory tree.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(Environment.SpecialFolder.Desktop)]
        [Localizable(false)]
        [SRCategory(nameof(SR.CatFolderBrowsing))]
        [SRDescription(nameof(SR.FolderBrowserDialogRootFolder))]
        [TypeConverter(typeof(SpecialFolderEnumConverter))]
        public Environment.SpecialFolder RootFolder
        {
            get => _rootFolder;
            set
            {
                if (!Enum.IsDefined(typeof(Environment.SpecialFolder), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Environment.SpecialFolder));
                }

                _rootFolder = value;
            }
        }

        /// <summary>
        ///  Gets or sets a description to show above the folders. Here you can provide
        ///  instructions for selecting a folder.
        /// </summary>
        [Browsable(true)]
        [DefaultValue("")]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatFolderBrowsing))]
        [SRDescription(nameof(SR.FolderBrowserDialogDescription))]
        public string Description
        {
            get => _descriptionText;
            set => _descriptionText = value ?? string.Empty;
        }

        /// <summary>
        ///  Gets or sets a value that indicates whether to use the value of the <see cref="Description" /> property
        ///  as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.
        /// </summary>
        /// <value><see langword="true" /> to indicate that the value of the <see cref="Description" /> property is used as dialog title; <see langword="false" />
        ///  to indicate the value is added as additional text to the dialog. The default is <see langword="false" />.</value>
        [Browsable(true)]
        [DefaultValue(false)]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatFolderBrowsing))]
        [SRDescription(nameof(SR.FolderBrowserDialogUseDescriptionForTitle))]
        public bool UseDescriptionForTitle { get; set; }

        private bool UseVistaDialogInternal
        {
            get => AutoUpgradeEnabled && SystemInformation.BootMode == BootMode.Normal;
        }

        /// <summary>
        ///  Resets all properties to their default values.
        /// </summary>
        public override void Reset()
        {
            _rootFolder = Environment.SpecialFolder.Desktop;
            _descriptionText = string.Empty;
            _selectedPath = string.Empty;
            _initialDirectory = string.Empty;
            ShowNewFolderButton = true;
            ClientGuid = null;
        }

        /// <summary>
        ///  Displays a folder browser dialog box.
        /// </summary>
        /// <param name="hWndOwner">A handle to the window that owns the folder browser dialog.</param>
        /// <returns>
        ///  <see langword="true" /> if the folder browser dialog was successfully run; otherwise, <see langword="false" />.
        /// </returns>
        protected override bool RunDialog(IntPtr hWndOwner)
        {
            // If running the Vista dialog fails (e.g. on Server Core), we fall back to the
            // legacy dialog.
            if (UseVistaDialogInternal && TryRunDialogVista(hWndOwner, out bool returnValue))
                return returnValue;

            return RunDialogOld(hWndOwner);
        }

        private bool TryRunDialogVista(IntPtr owner, out bool returnValue)
        {
            OpenFileDialog.NativeFileOpenDialog dialog;
            try
            {
                // Creating the Vista dialog can fail on Windows Server Core, even if the
                // Server Core App Compatibility FOD is installed.
                dialog = new OpenFileDialog.NativeFileOpenDialog();
            }
            catch (COMException)
            {
                returnValue = false;
                return false;
            }

            try
            {
                SetDialogProperties(dialog);
                HRESULT hr = dialog.Show(owner);
                if (!hr.Succeeded())
                {
                    if (hr == HRESULT.ERROR_CANCELLED)
                    {
                        returnValue = false;
                        return true;
                    }

                    throw Marshal.GetExceptionForHR((int)hr);
                }

                GetResult(dialog);
                returnValue = true;
                return true;
            }
            finally
            {
                if (dialog != null)
                {
                    Marshal.FinalReleaseComObject(dialog);
                }
            }
        }

        private void SetDialogProperties(IFileDialog dialog)
        {
            if (ClientGuid is { } clientGuid)
            {
                // IFileDialog::SetClientGuid should be called immediately after creation of the dialog object.
                // https://docs.microsoft.com/windows/win32/api/shobjidl_core/nf-shobjidl_core-ifiledialog-setclientguid#remarks
                dialog.SetClientGuid(clientGuid);
            }

            // Description
            if (!string.IsNullOrEmpty(_descriptionText))
            {
                if (UseDescriptionForTitle)
                {
                    dialog.SetTitle(_descriptionText);
                }
                else
                {
                    IFileDialogCustomize customize = (IFileDialogCustomize)dialog;
                    customize.AddText(0, _descriptionText);
                }
            }

            dialog.SetOptions(FOS.PICKFOLDERS | FOS.FORCEFILESYSTEM | FOS.FILEMUSTEXIST);

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

            if (!string.IsNullOrEmpty(_selectedPath))
            {
                string parent = Path.GetDirectoryName(_selectedPath);
                if (parent is null || !string.IsNullOrEmpty(InitialDirectory) || !Directory.Exists(parent))
                {
                    dialog.SetFileName(_selectedPath);
                }
                else
                {
                    string folder = Path.GetFileName(_selectedPath);
                    dialog.SetFolder(CreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }
        }

        private static IShellItem CreateItemFromParsingName(string path)
        {
            Guid guid = typeof(IShellItem).GUID;
            HRESULT hr = SHCreateItemFromParsingName(path, IntPtr.Zero, ref guid, out object item);
            if (hr != HRESULT.S_OK)
            {
                throw new Win32Exception((int)hr);
            }

            return (IShellItem)item;
        }

        private void GetResult(IFileDialog dialog)
        {
            dialog.GetResult(out IShellItem item);
            HRESULT hr = item.GetDisplayName(SIGDN.FILESYSPATH, out _selectedPath);
            if (!hr.Succeeded())
            {
                throw Marshal.GetExceptionForHR((int)hr);
            }
        }

        private unsafe bool RunDialogOld(IntPtr hWndOwner)
        {
            SHGetSpecialFolderLocation(hWndOwner, (int)_rootFolder, out CoTaskMemSafeHandle listHandle);
            if (listHandle.IsInvalid)
            {
                SHGetSpecialFolderLocation(hWndOwner, (int)Environment.SpecialFolder.Desktop, out listHandle);
                if (listHandle.IsInvalid)
                {
                    throw new InvalidOperationException(SR.FolderBrowserDialogNoRootFolder);
                }
            }

            using (listHandle)
            {
                uint mergedOptions = BrowseInfoFlags.BIF_NEWDIALOGSTYLE;
                if (!ShowNewFolderButton)
                {
                    mergedOptions |= BrowseInfoFlags.BIF_NONEWFOLDERBUTTON;
                }

                // The SHBrowserForFolder dialog is OLE/COM based, and documented as only being safe to use under the STA
                // threading model if the BIF_NEWDIALOGSTYLE flag has been requested (which we always do in mergedOptions
                // above). So make sure OLE is initialized, and throw an exception if caller attempts to invoke dialog
                // under the MTA threading model (...dialog does appear under MTA, but is totally non-functional).
                if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != System.Threading.ApartmentState.STA)
                {
                    throw new Threading.ThreadStateException(string.Format(SR.DebuggingExceptionOnly, SR.ThreadMustBeSTA));
                }

                var callback = new BrowseCallbackProc(FolderBrowserDialog_BrowseCallbackProc);
                char[] displayName = ArrayPool<char>.Shared.Rent(Kernel32.MAX_PATH + 1);
                try
                {
                    fixed (char* pDisplayName = displayName)
                    {
                        var bi = new BROWSEINFO
                        {
                            pidlRoot = listHandle,
                            hwndOwner = hWndOwner,
                            pszDisplayName = pDisplayName,
                            lpszTitle = _descriptionText,
                            ulFlags = mergedOptions,
                            lpfn = callback,
                            lParam = IntPtr.Zero,
                            iImage = 0
                        };

                        // Show the dialog
                        using (CoTaskMemSafeHandle browseHandle = SHBrowseForFolderW(ref bi))
                        {
                            if (browseHandle.IsInvalid)
                            {
                                return false;
                            }

                            // Retrieve the path from the IDList.
                            SHGetPathFromIDListLongPath(browseHandle.DangerousGetHandle(), out _selectedPath);
                            GC.KeepAlive(callback);
                            return true;
                        }
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(displayName);
                }
            }
        }

        /// <summary>
        ///  Callback function used to enable/disable the OK button,
        ///  and select the initial folder.
        /// </summary>
        private int FolderBrowserDialog_BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData)
        {
            switch ((BFFM)msg)
            {
                case BFFM.INITIALIZED:
                    // Indicates the browse dialog box has finished initializing. The lpData value is zero.

                    if (_initialDirectory.Length != 0)
                    {
                        // Try to expand the folder specified by initialDir
                        User32.SendMessageW(hwnd, (User32.WM)BFFM.SETEXPANDED, PARAM.FromBool(true), _initialDirectory);
                    }

                    if (_selectedPath.Length != 0)
                    {
                        // Try to select the folder specified by selectedPath
                        User32.SendMessageW(hwnd, (User32.WM)BFFM.SETSELECTIONW, PARAM.FromBool(true), _selectedPath);
                    }
                    break;
                case BFFM.SELCHANGED:
                    // Indicates the selection has changed. The lpData parameter points to the item identifier list for the newly selected item.
                    IntPtr selectedPidl = lParam;
                    if (selectedPidl != IntPtr.Zero)
                    {
                        // Try to retrieve the path from the IDList
                        bool isFileSystemFolder = SHGetPathFromIDListLongPath(selectedPidl, out _);
                        User32.SendMessageW(hwnd, (User32.WM)BFFM.ENABLEOK, IntPtr.Zero, PARAM.FromBool(isFileSystemFolder));
                    }
                    break;
            }

            return 0;
        }
    }
}
