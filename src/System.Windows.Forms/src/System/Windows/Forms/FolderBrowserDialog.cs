// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;

namespace System.Windows.Forms 
{
    using System;
    using System.IO;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Diagnostics.CodeAnalysis;
    
    /// <include file='doc\FolderBrowserDialog.uex' path='docs/doc[@for="FolderBrowserDialog"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents a common dialog box that allows the user to specify options for 
    ///       selecting a folder. This class cannot be inherited.
    ///    </para>
    /// </devdoc>
    [
    DefaultEvent(nameof(HelpRequest)),
    DefaultProperty(nameof(SelectedPath)),
    Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, " + AssemblyRef.SystemDesign),    
    SRDescription(nameof(SR.DescriptionFolderBrowserDialog))
    ]
    public sealed class FolderBrowserDialog : CommonDialog
    {
        // Root node of the tree view.
        private Environment.SpecialFolder rootFolder;
    
        // Description text to show.
        private string descriptionText;
    
        // Folder picked by the user.
        private string selectedPath;

        // Show the 'New Folder' button?
        private bool showNewFolderButton;

        // Callback function for the folder browser dialog
        private UnsafeNativeMethods.BrowseCallbackProc callback;

        /// <include file='doc\FolderBrowserDialog.uex' path='docs/doc[@for="FolderBrowserDialog.FolderBrowserDialog"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.FolderBrowserDialog'/> class.
        ///    </para>
        /// </devdoc>
        public FolderBrowserDialog() 
        {
            Reset();
        }


        /// <summary>
        /// Gets or Sets whether the dialog will be automatically upgraded to enable new features.
        /// </summary>
        [
            DefaultValue(true)
        ]
        public bool AutoUpgradeEnabled { get; set; } = true;

        /// <include file='doc\FolderBrowserDialog.uex' path='docs/doc[@for="FolderBrowserDialog.HelpRequest"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler HelpRequest 
        {
            add 
            {
                base.HelpRequest += value;
            }
            remove 
            {
                base.HelpRequest -= value;
            }
        }

        /// <include file='doc\FolderBrowserDialog.uex' path='docs/doc[@for="FolderBrowserDialog.ShowNewFolderButton"]/*' />
        /// <devdoc>
        ///     Determines if the 'New Folder' button should be exposed.
        ///     This property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown.
        /// </devdoc>
        [
        Browsable(true),
        DefaultValue(true),
        Localizable(false),
        SRCategory(nameof(SR.CatFolderBrowsing)),
        SRDescription(nameof(SR.FolderBrowserDialogShowNewFolderButton))
        ]
        public bool ShowNewFolderButton
        {
            get
            {
                return showNewFolderButton;
            }
            set
            {
                showNewFolderButton = value;
            }
        }

        /// <include file='doc\FolderBrowserDialog.uex' path='docs/doc[@for="FolderBrowserDialog.SelectedPath"]/*' />
        /// <devdoc>
        ///     Gets the directory path of the folder the user picked.
        ///     Sets the directory path of the initial folder shown in the dialog box.
        /// </devdoc>
        [
        Browsable(true),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.SelectedPathEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Localizable(true),
        SRCategory(nameof(SR.CatFolderBrowsing)),
        SRDescription(nameof(SR.FolderBrowserDialogSelectedPath))
        ]
        public string SelectedPath
        {
            get
            {
                return selectedPath;
            }
            set
            {
                selectedPath = (value == null) ? String.Empty : value;
            }
        }

        /// <include file='doc\FolderBrowserDialog.uex' path='docs/doc[@for="FolderBrowserDialog.RootFolder"]/*' />
        /// <devdoc>
        ///     Gets/sets the root node of the directory tree.
        /// </devdoc>
        [
        Browsable(true),
        DefaultValue(System.Environment.SpecialFolder.Desktop),
        Localizable(false),
        SRCategory(nameof(SR.CatFolderBrowsing)),
        SRDescription(nameof(SR.FolderBrowserDialogRootFolder)),
        TypeConverter(typeof(SpecialFolderEnumConverter))
        ]
        public System.Environment.SpecialFolder RootFolder
        {
            get
            {
                return rootFolder;
            }            
            [SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible")]            
            set
            {
                // FXCop:
                // leaving in Enum.IsDefined because this Enum is likely to grow and we dont own it.
                if (!Enum.IsDefined(typeof(System.Environment.SpecialFolder), value)) 
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(System.Environment.SpecialFolder));
                }
                rootFolder = value;
            }
        }
    
        /// <include file='doc\FolderBrowserDialog.uex' path='docs/doc[@for="FolderBrowserDialog.Description"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a description to show above the folders. Here you can provide instructions for
        ///       selecting a folder.
        ///    </para>
        /// </devdoc>
        [
        Browsable(true),
        DefaultValue(""),
        Localizable(true),
        SRCategory(nameof(SR.CatFolderBrowsing)),
        SRDescription(nameof(SR.FolderBrowserDialogDescription))
        ]
        public string Description
        {
            get
            {
                return descriptionText;
            }
            set
            {
                descriptionText = (value == null) ? String.Empty : value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to use the value of the <see cref="Description" /> property
        /// as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.
        /// </summary>
        /// <value><see langword="true" /> to indicate that the value of the <see cref="Description" /> property is used as dialog title; <see langword="false" />
        /// to indicate the value is added as additional text to the dialog. The default is <see langword="false" />.</value>
        [
        Browsable(true),
        DefaultValue(false),
        Localizable(true),
        SRCategory(nameof(SR.CatFolderBrowsing)),
        Description(nameof(SR.FolderBrowserDialogUseDescriptionForTitle))
        ]
        public bool UseDescriptionForTitle { get; set; }

        internal bool UseVistaDialogInternal
        {
            get
            {
                if (AutoUpgradeEnabled)
                {
                    return SystemInformation.BootMode == BootMode.Normal; 
                }

                return false;
            }
        }

        /// <devdoc>
        ///     Helper function that returns the IMalloc interface used by the shell.
        /// </devdoc>
        private static UnsafeNativeMethods.IMalloc GetSHMalloc()
        {
            UnsafeNativeMethods.IMalloc[] malloc = new UnsafeNativeMethods.IMalloc[1];
            UnsafeNativeMethods.Shell32.SHGetMalloc(malloc);
            return malloc[0];
        }
    
        /// <include file='doc\FolderBrowserDialog.uex' path='docs/doc[@for="FolderBrowserDialog.Reset"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Resets all properties to their default values.
        ///    </para>
        /// </devdoc>
        public override void Reset() 
        {
            rootFolder = System.Environment.SpecialFolder.Desktop;
            descriptionText = String.Empty;
            selectedPath = String.Empty;
            showNewFolderButton = true;
        }

        /// <include file='doc\FolderBrowserDialog.uex' path='docs/doc[@for="FolderBrowserDialog.RunDialog"]/*' />
        /// <devdoc>
        ///    Implements running of a folder browser dialog.
        /// </devdoc>
        protected override bool RunDialog(IntPtr hWndOwner) 
        {
            return UseVistaDialogInternal ? RunDialogVista(hWndOwner) : RunDialogOld(hWndOwner);
        }

        private bool RunDialogVista(IntPtr owner)
        {
            FileDialogNative.IFileDialog dialog = null;
            try
            {
                dialog = new FileDialogNative.NativeFileOpenDialog();
                SetDialogProperties(dialog);
                int result = dialog.Show(owner);
                if (result < 0)
                {
                    if ((uint)result == (uint)NativeMethods.HRESULT.ERROR_CANCELLED)
                        return false;
                    else
                        throw Marshal.GetExceptionForHR(result);
                }
                GetResult(dialog);
                return true;
            }
            finally
            {
                if (dialog != null)
                    Marshal.FinalReleaseComObject(dialog);
            }
        }

        private void SetDialogProperties(FileDialogNative.IFileDialog dialog)
        {
            // Description
            if (!string.IsNullOrEmpty(descriptionText))
            {
                if (UseDescriptionForTitle)
                {
                    dialog.SetTitle(descriptionText);
                }
                else
                {
                    FileDialogNative.IFileDialogCustomize customize = (FileDialogNative.IFileDialogCustomize)dialog;
                    customize.AddText(0, descriptionText);
                }
            }

            dialog.SetOptions(FileDialogNative.FOS.FOS_PICKFOLDERS | FileDialogNative.FOS.FOS_FORCEFILESYSTEM | FileDialogNative.FOS.FOS_FILEMUSTEXIST);

            if (!string.IsNullOrEmpty(selectedPath))
            {
                string parent = Path.GetDirectoryName(selectedPath);
                if (parent == null || !Directory.Exists(parent))
                {
                    dialog.SetFileName(selectedPath);
                }
                else
                {
                    string folder = Path.GetFileName(selectedPath);
                    dialog.SetFolder(FileDialogNative.CreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }
        }

        private void GetResult(FileDialogNative.IFileDialog dialog)
        {
            dialog.GetResult(out FileDialogNative.IShellItem item);
            item.GetDisplayName(FileDialogNative.SIGDN.SIGDN_FILESYSPATH, out selectedPath);
        }

        private bool RunDialogOld(IntPtr hWndOwner)
        {
            IntPtr pidlRoot = IntPtr.Zero;
            bool returnValue = false;

            UnsafeNativeMethods.Shell32.SHGetSpecialFolderLocation(hWndOwner, (int)rootFolder, ref pidlRoot);
            if (pidlRoot == IntPtr.Zero)
            {
                UnsafeNativeMethods.Shell32.SHGetSpecialFolderLocation(hWndOwner, NativeMethods.CSIDL_DESKTOP, ref pidlRoot);
                if (pidlRoot == IntPtr.Zero)
                {
                    throw new InvalidOperationException(SR.FolderBrowserDialogNoRootFolder);
                }
            }

            int mergedOptions = unchecked((int)(long)UnsafeNativeMethods.BrowseInfos.NewDialogStyle);
            if (!showNewFolderButton)
            {
                mergedOptions += unchecked((int)(long)UnsafeNativeMethods.BrowseInfos.HideNewFolderButton);
            }

            // The SHBrowserForFolder dialog is OLE/COM based, and documented as only being safe to use under the STA
            // threading model if the BIF_NEWDIALOGSTYLE flag has been requested (which we always do in mergedOptions
            // above). So make sure OLE is initialized, and throw an exception if caller attempts to invoke dialog
            // under the MTA threading model (...dialog does appear under MTA, but is totally non-functional).
            if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != System.Threading.ApartmentState.STA)
            {
                throw new System.Threading.ThreadStateException(string.Format(SR.DebuggingExceptionOnly, SR.ThreadMustBeSTA));
            }

            IntPtr pidlRet = IntPtr.Zero;
            IntPtr pszDisplayName = IntPtr.Zero;
            IntPtr pszSelectedPath = IntPtr.Zero;

            try
            {
                // Construct a BROWSEINFO
                UnsafeNativeMethods.BROWSEINFO bi = new UnsafeNativeMethods.BROWSEINFO();

                pszDisplayName = Marshal.AllocHGlobal(NativeMethods.MAX_PATH * sizeof(char));
                pszSelectedPath = Marshal.AllocHGlobal((NativeMethods.MAX_PATH + 1) * sizeof(char));
                this.callback = new UnsafeNativeMethods.BrowseCallbackProc(this.FolderBrowserDialog_BrowseCallbackProc);

                bi.pidlRoot = pidlRoot;
                bi.hwndOwner = hWndOwner;
                bi.pszDisplayName = pszDisplayName;
                bi.lpszTitle = descriptionText;
                bi.ulFlags = mergedOptions;
                bi.lpfn = callback;
                bi.lParam = IntPtr.Zero;
                bi.iImage = 0;

                // And show the dialog
                pidlRet = UnsafeNativeMethods.Shell32.SHBrowseForFolder(bi);

                if (pidlRet != IntPtr.Zero)
                {
                    // Then retrieve the path from the IDList
                    UnsafeNativeMethods.Shell32.SHGetPathFromIDListLongPath(pidlRet, ref pszSelectedPath);

                    // Convert to a string
                    selectedPath = Marshal.PtrToStringAuto(pszSelectedPath);

                    returnValue = true;
                }
            }
            finally
            {
                UnsafeNativeMethods.CoTaskMemFree(pidlRoot);
                if (pidlRet != IntPtr.Zero)
                {
                    UnsafeNativeMethods.CoTaskMemFree(pidlRet);
                }

                // Then free all the stuff we've allocated or the SH API gave us
                if (pszSelectedPath != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pszSelectedPath);
                }
                if (pszDisplayName != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pszDisplayName);
                }

                this.callback = null;
            }
            return returnValue;
        }

        /// <devdoc>
        ///    Callback function used to enable/disable the OK button,
        ///    and select the initial folder.
        /// </devdoc>
        private int FolderBrowserDialog_BrowseCallbackProc(IntPtr hwnd,
                                                           int msg, 
                                                           IntPtr lParam, 
                                                           IntPtr lpData)
        {
            switch (msg)
            {
                case NativeMethods.BFFM_INITIALIZED: 
                    // Indicates the browse dialog box has finished initializing. The lpData value is zero. 
                    if (selectedPath.Length != 0) 
                    {
                        // Try to select the folder specified by selectedPath
                        UnsafeNativeMethods.SendMessage(new HandleRef(null, hwnd), (int) NativeMethods.BFFM_SETSELECTION, 1, selectedPath);
                    }
                    break;
                case NativeMethods.BFFM_SELCHANGED: 
                    // Indicates the selection has changed. The lpData parameter points to the item identifier list for the newly selected item. 
                    IntPtr selectedPidl = lParam;
                    if (selectedPidl != IntPtr.Zero)
                    {
                        IntPtr pszSelectedPath = Marshal.AllocHGlobal((NativeMethods.MAX_PATH + 1) * sizeof(char));
                        // Try to retrieve the path from the IDList
                        bool isFileSystemFolder = UnsafeNativeMethods.Shell32.SHGetPathFromIDListLongPath(selectedPidl, ref pszSelectedPath);
                        Marshal.FreeHGlobal(pszSelectedPath);
                        UnsafeNativeMethods.SendMessage(new HandleRef(null, hwnd), (int) NativeMethods.BFFM_ENABLEOK, 0, isFileSystemFolder ? 1 : 0);
                    }
                    break;
            }
            return 0;
        }
    }
}
