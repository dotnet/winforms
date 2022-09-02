﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        // Win32 file dialog FOS_* option flags.
        private int _options;

        /// <summary>
        ///  Initializes a new instance of the <see cref="FolderBrowserDialog"/> class.
        /// </summary>
        public FolderBrowserDialog()
        {
            Reset();
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box adds the folder being selected to the recent list.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FolderBrowserDialogAddToRecent))]
        public bool AddToRecent
        {
            get => !GetOption((int)FOS.DONTADDTORECENT);
            set => SetOption((int)FOS.DONTADDTORECENT, !value);
        }

        /// <summary>
        ///  Gets or sets whether the dialog will be automatically upgraded to enable new features.
        /// </summary>
        [DefaultValue(true)]
        public bool AutoUpgradeEnabled { get; set; } = true;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? HelpRequest
        {
            add => base.HelpRequest += value;
            remove => base.HelpRequest -= value;
        }

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
        [SRDescription(nameof(SR.FolderBrowserDialogOkRequiresInteraction))]
        public bool OkRequiresInteraction
        {
            get => GetOption((int)FOS.OKBUTTONNEEDSINTERACTION);
            set => SetOption((int)FOS.OKBUTTONNEEDSINTERACTION, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box displays hidden and system files.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FolderBrowserDialogShowHiddenFiles))]
        public bool ShowHiddenFiles
        {
            get => GetOption((int)FOS.FORCESHOWHIDDEN);
            set => SetOption((int)FOS.FORCESHOWHIDDEN, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the items shown by default in the view's
        ///  navigation pane are shown.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FolderBrowserDialogShowPinnedPlaces))]
        public bool ShowPinnedPlaces
        {
            get => !GetOption((int)FOS.HIDEPINNEDPLACES);
            set => SetOption((int)FOS.HIDEPINNEDPLACES, !value);
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
        [MemberNotNull(nameof(_descriptionText))]
        [MemberNotNull(nameof(_selectedPath))]
        [MemberNotNull(nameof(_initialDirectory))]
        public override void Reset()
        {
            _options = (int)(FOS.PICKFOLDERS | FOS.FORCEFILESYSTEM | FOS.FILEMUSTEXIST);
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

            return RunDialogOld((HWND)hWndOwner);
        }

        private bool TryRunDialogVista(IntPtr owner, out bool returnValue)
        {
            Interop.WinFormsComWrappers.FileOpenDialogWrapper dialog;
            try
            {
                // Creating the Vista dialog can fail on Windows Server Core, even if the
                // Server Core App Compatibility FOD is installed.
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
                    .GetOrCreateObjectForComInstance(lpDialogUnknownPtr, CreateObjectFlags.UniqueInstance);
                dialog = (Interop.WinFormsComWrappers.FileOpenDialogWrapper)obj;
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
                if (!hr.Succeeded)
                {
                    if (hr == HRESULT.HRESULT_FROM_WIN32(WIN32_ERROR.ERROR_CANCELLED))
                    {
                        returnValue = false;
                        return true;
                    }

                    throw Marshal.GetExceptionForHR((int)hr)!;
                }

                GetResult(dialog);
                returnValue = true;
                return true;
            }
            finally
            {
                dialog.Dispose();
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
                    var customize = (WinFormsComWrappers.FileOpenDialogWrapper)dialog;
                    customize.AddText(0, _descriptionText);
                }
            }

            dialog.SetOptions((FOS)_options);

            if (!string.IsNullOrEmpty(_initialDirectory))
            {
                try
                {
                    IShellItem initialDirectory = PInvoke.SHCreateShellItem(_initialDirectory);

                    dialog.SetDefaultFolder(initialDirectory);
                    dialog.SetFolder(initialDirectory);
                }
                catch (FileNotFoundException)
                {
                }
            }

            if (!string.IsNullOrEmpty(_selectedPath))
            {
                string? parent = Path.GetDirectoryName(_selectedPath);
                if (parent is null || !string.IsNullOrEmpty(_initialDirectory) || !Directory.Exists(parent))
                {
                    dialog.SetFileName(_selectedPath);
                }
                else
                {
                    string folder = Path.GetFileName(_selectedPath);
                    dialog.SetFolder(PInvoke.SHCreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }
        }

        private bool GetOption(int option) => (_options & option) != 0;

        /// <summary>
        ///  Sets the given option to the given boolean value.
        /// </summary>
        private void SetOption(int option, bool value)
        {
            if (value)
            {
                _options |= option;
            }
            else
            {
                _options &= ~option;
            }
        }

        private void GetResult(IFileDialog dialog)
        {
            dialog.GetResult(out IShellItem? item);
            if (item is not null)
            {
                HRESULT hr = item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out _selectedPath!);
                hr.ThrowOnFailure();
            }
        }

        private unsafe bool RunDialogOld(HWND hWndOwner)
        {
            PInvoke.SHGetSpecialFolderLocation(hWndOwner, (int)_rootFolder, out ITEMIDLIST* listHandle);
            if (listHandle is null)
            {
                PInvoke.SHGetSpecialFolderLocation(hWndOwner, (int)Environment.SpecialFolder.Desktop, out listHandle);
                if (listHandle is null)
                {
                    throw new InvalidOperationException(SR.FolderBrowserDialogNoRootFolder);
                }
            }

            uint mergedOptions = PInvoke.BIF_NEWDIALOGSTYLE;
            if (!ShowNewFolderButton)
            {
                mergedOptions |= PInvoke.BIF_NONEWFOLDERBUTTON;
            }

            // The SHBrowserForFolder dialog is OLE/COM based, and documented as only being safe to use under the STA
            // threading model if the BIF_NEWDIALOGSTYLE flag has been requested (which we always do in mergedOptions
            // above). So make sure OLE is initialized, and throw an exception if caller attempts to invoke dialog
            // under the MTA threading model (...dialog does appear under MTA, but is totally non-functional).
            if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != System.Threading.ApartmentState.STA)
            {
                throw new ThreadStateException(string.Format(SR.DebuggingExceptionOnly, SR.ThreadMustBeSTA));
            }

            delegate* unmanaged[Stdcall]<HWND, uint, LPARAM, LPARAM, int> callback = &FolderBrowserDialog_BrowseCallbackProc;
            char[] displayName = ArrayPool<char>.Shared.Rent(PInvoke.MAX_PATH + 1);
            var handle = GCHandle.Alloc(this);
            try
            {
                fixed (char* pDisplayName = displayName)
                fixed (char* title = _descriptionText)
                {
                    var bi = new BROWSEINFOW
                    {
                        pidlRoot = listHandle,
                        hwndOwner = hWndOwner,
                        pszDisplayName = pDisplayName,
                        lpszTitle = title,
                        ulFlags = mergedOptions,
                        lpfn = callback,
                        lParam = GCHandle.ToIntPtr(handle),
                        iImage = 0
                    };

                    // Show the dialog
                    ITEMIDLIST* browseHandle = PInvoke.SHBrowseForFolder(in bi);
                    {
                        if (browseHandle is null)
                        {
                            return false;
                        }

                        // Retrieve the path from the IDList.
                        fixed (char* path = _selectedPath!)
                        {
                            PInvoke.SHGetPathFromIDList(browseHandle, path);
                            return true;
                        }
                    }
                }
            }
            finally
            {
                handle.Free();
                ArrayPool<char>.Shared.Return(displayName);
            }
        }

        /// <summary>
        ///  Callback function used to enable/disable the OK button,
        /// and select the initial folder.
        /// </summary>
#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
        private unsafe static int FolderBrowserDialog_BrowseCallbackProc(HWND hwnd, uint msg, LPARAM lParam, LPARAM lpData)
        {
            switch ((BFFM)msg)
            {
                case BFFM.INITIALIZED:
                    // Indicates the browse dialog box has finished initializing. The lpData value is zero.

                    var instance = (FolderBrowserDialog)GCHandle.FromIntPtr(lParam).Target!;
                    if (instance._initialDirectory.Length != 0)
                    {
                        // Try to expand the folder specified by initialDir
                        PInvoke.SendMessage(hwnd, (User32.WM)BFFM.SETEXPANDED, (WPARAM)(BOOL)true, instance._initialDirectory);
                    }

                    if (instance._selectedPath.Length != 0)
                    {
                        // Try to select the folder specified by selectedPath
                        PInvoke.SendMessage(hwnd, (User32.WM)BFFM.SETSELECTIONW, (WPARAM)(BOOL)true, instance._selectedPath);
                    }

                    break;
                case BFFM.SELCHANGED:
                    // Indicates the selection has changed. The lpData parameter points to the item identifier list for the newly selected item.
                    ITEMIDLIST* selectedPidl = (ITEMIDLIST*)lParam;
                    if (selectedPidl is not null)
                    {
                        // Try to retrieve the path from the IDList
                        bool isFileSystemFolder = PInvoke.SHGetPathFromIDList(selectedPidl, null);
                        PInvoke.SendMessage(hwnd, (User32.WM)BFFM.ENABLEOK, 0, (nint)(BOOL)isFileSystemFolder);
                    }

                    break;
            }

            return 0;
        }
    }
}
