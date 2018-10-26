// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using CodeAccessPermission = System.Security.CodeAccessPermission;
    using System.IO;
    using System.Drawing;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Permissions;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using System.Runtime.Versioning;

    /// <include file='doc\SaveFileDialog.uex' path='docs/doc[@for="SaveFileDialog"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents
    ///       a common dialog box that allows the user to specify options for saving a
    ///       file. This class cannot be inherited.
    ///    </para>
    /// </devdoc>
    [
    Designer("System.Windows.Forms.Design.SaveFileDialogDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionSaveFileDialog))
    ]
    public sealed class SaveFileDialog : FileDialog {

        /// <include file='doc\SaveFileDialog.uex' path='docs/doc[@for="SaveFileDialog.CreatePrompt"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the dialog box prompts the user for
        ///       permission to create a file if the user specifies a file that does not exist.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(false),
        SRDescription(nameof(SR.SaveFileDialogCreatePrompt))
        ]
        public bool CreatePrompt {
            get {
                return GetOption(NativeMethods.OFN_CREATEPROMPT);
            }
            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();
                SetOption(NativeMethods.OFN_CREATEPROMPT, value);
            }
        }

        /// <include file='doc\SaveFileDialog.uex' path='docs/doc[@for="SaveFileDialog.OverwritePrompt"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the Save As dialog box displays a warning if the user specifies
        ///       a file name that already exists.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(true),
        SRDescription(nameof(SR.SaveFileDialogOverWritePrompt))
        ]
        public bool OverwritePrompt {
            get {
                return GetOption(NativeMethods.OFN_OVERWRITEPROMPT);
            }
            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();
                SetOption(NativeMethods.OFN_OVERWRITEPROMPT, value);
            }
        }

        /// <include file='doc\SaveFileDialog.uex' path='docs/doc[@for="SaveFileDialog.OpenFile"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Opens the file with read/write permission selected by the user.
        ///    </para>
        /// </devdoc>
        
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        /// 


        [SuppressMessage("Microsoft.Security", "CA2103:ReviewImperativeSecurity")]
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        public Stream OpenFile() {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogSaveFile Demanded");
            IntSecurity.FileDialogSaveFile.Demand();

            string filename = FileNamesInternal[0];

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException( "FileName" );
                
            Stream s = null;
            
            // 



            new FileIOPermission(FileIOPermissionAccess.AllAccess, IntSecurity.UnsafeGetFullPath(filename)).Assert();
            try {
                s = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
            }
            finally {
                CodeAccessPermission.RevertAssert();
            }
            return s;
        }

        /// <include file='doc\SaveFileDialog.uex' path='docs/doc[@for="SaveFileDialog.PromptFileCreate"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Prompts the user with a <see cref='System.Windows.Forms.MessageBox'/>
        ///       when a file is about to be created. This method is
        ///       invoked when the CreatePrompt property is true and the specified file
        ///       does not exist. A return value of false prevents the dialog from
        ///       closing.
        ///    </para>
        /// </devdoc>
        private bool PromptFileCreate(string fileName) 
        {
            return MessageBoxWithFocusRestore(string.Format(SR.FileDialogCreatePrompt, fileName),
                    DialogCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        /// <include file='doc\SaveFileDialog.uex' path='docs/doc[@for="SaveFileDialog.PromptFileOverwrite"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Prompts the user when a file is about to be overwritten. This method is
        ///       invoked when the "overwritePrompt" property is true and the specified
        ///       file already exists. A return value of false prevents the dialog from
        ///       closing.
        ///       
        ///    </para>
        /// </devdoc>
        private bool PromptFileOverwrite(string fileName) {
            return MessageBoxWithFocusRestore(string.Format(SR.FileDialogOverwritePrompt, fileName),
                    DialogCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        // If it's necessary to throw up a "This file exists, are you sure?" kind of
        // MessageBox, here's where we do it.
        // Return value is whether or not the user hit "okay".
        internal override bool PromptUserIfAppropriate(string fileName) {
            if (!base.PromptUserIfAppropriate(fileName)) {
                return false;
            }

            //Note: When we are using the Vista dialog mode we get two prompts (one from us and one from the OS) if we do this
            if ((options & NativeMethods.OFN_OVERWRITEPROMPT) != 0 && FileExists(fileName) && !this.UseVistaDialogInternal) {
                if (!PromptFileOverwrite(fileName)) {
                    return false;
                }
            }

            if ((options & NativeMethods.OFN_CREATEPROMPT) != 0 && !FileExists(fileName)) {
                if (!PromptFileCreate(fileName)) {
                    return false;
                }
            }
            
            return true;
        }

        /// <include file='doc\SaveFileDialog.uex' path='docs/doc[@for="SaveFileDialog.Reset"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Resets all dialog box options to their default
        ///       values.
        ///    </para>
        /// </devdoc>
        public override void Reset() {
            base.Reset();
            SetOption(NativeMethods.OFN_OVERWRITEPROMPT, true);
        }

        internal override void EnsureFileDialogPermission()
        {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogSaveFile Demanded in SaveFileDialog.RunFileDialog");
            IntSecurity.FileDialogSaveFile.Demand();
        }

        /// <include file='doc\SaveFileDialog.uex' path='docs/doc[@for="SaveFileDialog.RunFileDialog"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal override bool RunFileDialog(NativeMethods.OPENFILENAME_I ofn) {
            //We have already done the demand in EnsureFileDialogPermission but it doesn't hurt to do it again
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogSaveFile Demanded in SaveFileDialog.RunFileDialog");
            IntSecurity.FileDialogSaveFile.Demand();

            bool result = UnsafeNativeMethods.GetSaveFileName(ofn);

            if (!result) {
                // Something may have gone wrong - check for error condition
                //
                int errorCode = SafeNativeMethods.CommDlgExtendedError();
                switch(errorCode) {
                    case NativeMethods.FNERR_INVALIDFILENAME:
                        throw new InvalidOperationException(string.Format(SR.FileDialogInvalidFileName, FileName));
                }
            }
            
            return result;
         }
        internal override string[] ProcessVistaFiles(FileDialogNative.IFileDialog dialog)
        {
            FileDialogNative.IFileSaveDialog saveDialog = (FileDialogNative.IFileSaveDialog)dialog;
            FileDialogNative.IShellItem item;
            dialog.GetResult(out item);
            return new string[] { GetFilePathFromShellItem(item) };
        }
        internal override FileDialogNative.IFileDialog CreateVistaDialog()
        { return new FileDialogNative.NativeFileSaveDialog(); }


    }
}
