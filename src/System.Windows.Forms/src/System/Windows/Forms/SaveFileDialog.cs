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
    ///  Represents
    ///  a common dialog box that allows the user to specify options for saving a
    ///  file. This class cannot be inherited.
    /// </summary>
    [Designer("System.Windows.Forms.Design.SaveFileDialogDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionSaveFileDialog))]
    public sealed partial class SaveFileDialog : FileDialog
    {
        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box verifies if the creation of the specified file will be successful.
        ///  If this flag is not set, the calling application must handle errors, such as denial of access, discovered when the item is created.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.SaveFileDialogCheckWriteAccess))]
        public bool CheckWriteAccess
        {
            get => !GetOption((int)Comdlg32.OFN.NOTESTFILECREATE);
            set => SetOption((int)Comdlg32.OFN.NOTESTFILECREATE, !value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box prompts the user for
        ///  permission to create a file if the user specifies a file that does not exist.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.SaveFileDialogCreatePrompt))]
        public bool CreatePrompt
        {
            get => GetOption((int)Comdlg32.OFN.CREATEPROMPT);
            set => SetOption((int)Comdlg32.OFN.CREATEPROMPT, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box is always opened in the expanded mode.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.SaveFileDialogExpandedMode))]
        public bool ExpandedMode
        {
            get => GetOption((int)FOS.DEFAULTNOMINIMODE);
            set => SetOption((int)FOS.DEFAULTNOMINIMODE, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Save As dialog box displays a warning if the user specifies
        ///  a file name that already exists.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.SaveFileDialogOverWritePrompt))]
        public bool OverwritePrompt
        {
            get => GetOption((int)Comdlg32.OFN.OVERWRITEPROMPT);
            set => SetOption((int)Comdlg32.OFN.OVERWRITEPROMPT, value);
        }

        /// <summary>
        ///  Opens the file with read/write permission selected by the user.
        /// </summary>
        public Stream OpenFile()
        {
            string filename = FileNames[0];
            filename.ThrowIfNullOrEmpty();
            return new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
        }

        /// <summary>
        ///  Prompts the user with a <see cref="MessageBox"/>
        ///  when a file is about to be created. This method is
        ///  invoked when the CreatePrompt property is true and the specified file
        ///  does not exist. A return value of false prevents the dialog from
        ///  closing.
        /// </summary>
        private bool PromptFileCreate(string fileName)
        {
            return MessageBoxWithFocusRestore(string.Format(SR.FileDialogCreatePrompt, fileName),
                    DialogCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        /// <summary>
        ///  Prompts the user when a file is about to be overwritten. This method is
        ///  invoked when the "overwritePrompt" property is true and the specified
        ///  file already exists. A return value of false prevents the dialog from
        ///  closing.
        /// </summary>
        private bool PromptFileOverwrite(string fileName)
        {
            return MessageBoxWithFocusRestore(string.Format(SR.FileDialogOverwritePrompt, fileName),
                    DialogCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        // If it's necessary to throw up a "This file exists, are you sure?" kind of
        // MessageBox, here's where we do it.
        // Return value is whether or not the user hit "okay".
        private protected override bool PromptUserIfAppropriate(string fileName)
        {
            if (!base.PromptUserIfAppropriate(fileName))
            {
                return false;
            }

            //Note: When we are using the Vista dialog mode we get two prompts (one from us and one from the OS) if we do this
            if ((_options & (int)Comdlg32.OFN.OVERWRITEPROMPT) != 0 && FileExists(fileName) && !UseVistaDialogInternal)
            {
                if (!PromptFileOverwrite(fileName))
                {
                    return false;
                }
            }

            if ((_options & (int)Comdlg32.OFN.CREATEPROMPT) != 0 && !FileExists(fileName))
            {
                if (!PromptFileCreate(fileName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  Resets all dialog box options to their default
        ///  values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            SetOption((int)FOS.DEFAULTNOMINIMODE, true);
            SetOption((int)Comdlg32.OFN.OVERWRITEPROMPT, true);
        }

        private protected override bool RunFileDialog(NativeMethods.OPENFILENAME_I ofn)
        {
            bool result = UnsafeNativeMethods.GetSaveFileName(ofn);

            if (!result)
            {
                // Something may have gone wrong - check for error condition
                switch (PInvoke.CommDlgExtendedError())
                {
                    case COMMON_DLG_ERRORS.FNERR_INVALIDFILENAME:
                        throw new InvalidOperationException(string.Format(SR.FileDialogInvalidFileName, FileName));
                }
            }

            return result;
        }

        private protected override string[] ProcessVistaFiles(WinFormsComWrappers.FileDialogWrapper dialog)
        {
            dialog.GetResult(out IShellItem? item);
            if (item is null)
            {
                return Array.Empty<string>();
            }

            return new string[] { GetFilePathFromShellItem(item) };
        }

        private protected override Interop.WinFormsComWrappers.FileDialogWrapper CreateVistaDialog()
        {
            HRESULT hr = Ole32.CoCreateInstance(
                in CLSID.FileSaveDialog,
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
    }
}
