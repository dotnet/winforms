// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Text;
    using System.Threading;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Security.Permissions;
    using System.Drawing;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.IO;
    using ArrayList = System.Collections.ArrayList;

    using Encoding = System.Text.Encoding;
    using Microsoft.Win32;
    using System.Security;
    using System.Runtime.Versioning;

    using CharBuffer = System.Windows.Forms.UnsafeNativeMethods.CharBuffer;

    /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Displays a dialog window from which the user can select a file.
    ///    </para>
    /// </devdoc>
    [
    DefaultEvent(nameof(FileOk)),
    DefaultProperty(nameof(FileName))
    ]
    public abstract partial class FileDialog : CommonDialog {

        private const int FILEBUFSIZE = 8192;

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.EventFileOk"]/*' />
        /// <internalonly/>
        protected static readonly object EventFileOk = new object();

        internal const int OPTION_ADDEXTENSION = unchecked(unchecked((int)0x80000000));

        internal int options;

        private string title;
        private string initialDir;
        private string defaultExt;
        private string[] fileNames;
        private bool securityCheckFileNames;
        private string filter;
        private int filterIndex;
        private bool supportMultiDottedExtensions;
        private bool ignoreSecondFileOkNotification;
        private int okNotificationCount;
        private CharBuffer charBuffer;
        private IntPtr dialogHWnd;

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.FileDialog"]/*' />
        /// <devdoc>
        ///    <para>
        ///       In an inherited class,
        ///       initializes a new instance of the <see cref='System.Windows.Forms.FileDialog'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // If the constructor does not call Reset
                                                                                                    // it would be a breaking change.
        ]
        internal FileDialog() {
            Reset();
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.AddExtension"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the
        ///       dialog box automatically adds an extension to a
        ///       file name if the user omits the extension.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(true),
        SRDescription(nameof(SR.FDaddExtensionDescr))
        ]
        public bool AddExtension {
            get {
                return GetOption(OPTION_ADDEXTENSION);
            }

            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();
                SetOption(OPTION_ADDEXTENSION, value);
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.CheckFileExists"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether
        ///       the dialog box displays a warning if the user specifies a file name that does not exist.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(false),
        SRDescription(nameof(SR.FDcheckFileExistsDescr))
        ]
        public virtual bool CheckFileExists {
            get {
                return GetOption(NativeMethods.OFN_FILEMUSTEXIST);
            }

            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();
                SetOption(NativeMethods.OFN_FILEMUSTEXIST, value);
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.CheckPathExists"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the
        ///       dialog box displays a warning if the user specifies a path that does not exist.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(true),
        SRDescription(nameof(SR.FDcheckPathExistsDescr))
        ]
        public bool CheckPathExists {
            get {
                return GetOption(NativeMethods.OFN_PATHMUSTEXIST);
            }

            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();
                SetOption(NativeMethods.OFN_PATHMUSTEXIST, value);
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.DefaultExt"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the default file extension.
        ///       
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(""),
        SRDescription(nameof(SR.FDdefaultExtDescr))
        ]
        public string DefaultExt {
            get {
                return defaultExt == null? "": defaultExt;
            }

            set {
                if (value != null) {
                    if (value.StartsWith("."))
                        value = value.Substring(1);
                    else if (value.Length == 0)
                        value = null;
                }
                defaultExt = value;
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.DereferenceLinks"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value
        ///       indicating whether the dialog box returns the location of the file referenced by the shortcut or
        ///       whether it returns the location of the shortcut (.lnk).
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(true),
        SRDescription(nameof(SR.FDdereferenceLinksDescr))
        ]
        public bool DereferenceLinks {
            get { 
                return !GetOption(NativeMethods.OFN_NODEREFERENCELINKS);
            }
            set { 
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();
                SetOption(NativeMethods.OFN_NODEREFERENCELINKS, !value);
            }
        }

        internal string DialogCaption {
            get {
                int textLen = SafeNativeMethods.GetWindowTextLength(new HandleRef(this, dialogHWnd));
                StringBuilder sb = new StringBuilder(textLen+1);
                UnsafeNativeMethods.GetWindowText(new HandleRef(this, dialogHWnd), sb, sb.Capacity);
                return sb.ToString();
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.FileName"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets a string containing
        ///       the file name selected in the file dialog box.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)), 
        DefaultValue(""),
        SRDescription(nameof(SR.FDfileNameDescr))
        ]
        public string FileName {
            get {
                if (fileNames == null) {
                    return "";
                }
                else {
                    if (fileNames[0].Length > 0) {

                        // See if we need to perform a security check on file names.  We need
                        // to do this if the set of file names was provided by the file dialog.
                        // A developer can set file names through the FileDialog API as well,
                        // but we don't need to check those since the developer can provide any
                        // name s/he wants.  This is important because it is otherwise possible
                        // to get the FileName property to accept garbage, but throw during get.
                        
                        if (securityCheckFileNames) {
                            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileIO(" + fileNames[0] + ") Demanded");
                            IntSecurity.DemandFileIO(FileIOPermissionAccess.AllAccess, fileNames[0]);
                        }

                        return fileNames[0];
                    }
                    else {
                        return "";
                    }
                }
            }
            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();
                if (value == null) {
                    fileNames = null;
                }
                else {
                    fileNames = new string[] {value};
                }

                // As the developer has called this API and set the file name with an arbitrary value,
                // we do not need to perform a security check on the name.
                securityCheckFileNames = false;
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.FileNames"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the file
        ///       names of all selected files in the dialog box.
        ///    </para>
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FDFileNamesDescr))
        ]
        public string[] FileNames {
            get{
                string[] files = FileNamesInternal;

                // See if we need to perform a security check on file names.  We need
                // to do this if the set of file names was provided by the file dialog.
                // A developer can set file names through the FileDialog API as well,
                // but we don't need to check those since the developer can provide any
                // name s/he wants.  This is important because it is otherwise possible
                // to get the FileName property to accept garbage, but throw during get.

                if (securityCheckFileNames) {
                    foreach (string file in files) {
                        Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileIO(" + file + ") Demanded");
                        IntSecurity.DemandFileIO(FileIOPermissionAccess.AllAccess, file);
                    }
                }    
                return files;
            }
        }

        internal string[] FileNamesInternal {
            get {

                if (fileNames == null) {
                    return new string[0];
                }
                else {
                    return(string[])fileNames.Clone();
                }
            }
        }


        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.Filter"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets the current file name filter string,
        ///       which determines the choices that appear in the "Save as file type" or
        ///       "Files of type" box in the dialog box.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(""),
        Localizable(true),
        SRDescription(nameof(SR.FDfilterDescr))
        ]
        public string Filter {
            get {
                return filter == null? "": filter;
            }

            set {
                if (value != filter) {
                    if (value != null && value.Length > 0) {
                        string[] formats = value.Split('|');
                        if (formats == null || formats.Length % 2 != 0) {
                            throw new ArgumentException(SR.FileDialogInvalidFilter);
                        }
                    }
                    else {
                        value = null;
                    }
                    filter = value;
                }
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.FilterExtensions"]/*' />
        /// <devdoc>
        ///     Extracts the file extensions specified by the current file filter into
        ///     an array of strings.  None of the extensions contain .'s, and the 
        ///     default extension is first.
        /// </devdoc>
        private string[] FilterExtensions {
            get {
                string filter = this.filter;
                ArrayList extensions = new ArrayList();
                
                // First extension is the default one.  It's a little strange if DefaultExt
                // is not in the filters list, but I guess it's legal.
                if (defaultExt != null) 
                    extensions.Add(defaultExt);

                if (filter != null) {
                    string[] tokens = filter.Split('|');
                    
                    if ((filterIndex * 2) - 1 >= tokens.Length) {
                        throw new InvalidOperationException(SR.FileDialogInvalidFilterIndex);
                    }
                    
                    if (filterIndex > 0) {
                        string[] exts = tokens[(filterIndex * 2) - 1].Split(';');
                        foreach (string ext in exts) {
                            int i = this.supportMultiDottedExtensions ? ext.IndexOf('.') : ext.LastIndexOf('.');
                            if (i >= 0) {
                                extensions.Add(ext.Substring(i + 1, ext.Length - (i + 1)));
                            }
                        }
                    }
                }
                string[] temp = new string[extensions.Count];
                extensions.CopyTo(temp, 0);
                return temp;
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.FilterIndex"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the index of the filter currently selected in the file dialog box.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(1),
        SRDescription(nameof(SR.FDfilterIndexDescr))
        ]
        public int FilterIndex {
            get {
                return filterIndex;
            }

            set {
                filterIndex = value;
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.InitialDirectory"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the initial directory displayed by the file dialog
        ///       box.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)), 
        DefaultValue(""),
        SRDescription(nameof(SR.FDinitialDirDescr))
        ]
        public string InitialDirectory {
            get {
                return initialDir == null? "": initialDir;
            }
            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();

                initialDir = value;
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.Instance"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets the Win32 instance handle for the application.
        ///    </para>
        /// </devdoc>
        /* SECURITYUNDONE : should require EventQueue permission */
        protected virtual IntPtr Instance {
            [
                SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
                SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
            ]
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            get { return UnsafeNativeMethods.GetModuleHandle(null); }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.Options"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets the Win32 common Open File Dialog OFN_* option flags.
        ///    </para>
        /// </devdoc>
        protected int Options {
            get {
                return options & (NativeMethods.OFN_READONLY | NativeMethods.OFN_HIDEREADONLY |
                                  NativeMethods.OFN_NOCHANGEDIR | NativeMethods.OFN_SHOWHELP | NativeMethods.OFN_NOVALIDATE |
                                  NativeMethods.OFN_ALLOWMULTISELECT | NativeMethods.OFN_PATHMUSTEXIST |
                                  NativeMethods.OFN_NODEREFERENCELINKS);
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.RestoreDirectory"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the dialog box restores the current directory before
        ///       closing.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(false),
        SRDescription(nameof(SR.FDrestoreDirectoryDescr))
        ]
        public bool RestoreDirectory {
            get {
                return GetOption(NativeMethods.OFN_NOCHANGEDIR);
            }
            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();

                SetOption(NativeMethods.OFN_NOCHANGEDIR, value);
            }
        }


        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.ShowHelp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating
        ///       whether whether the Help button is displayed in the file dialog.
        ///       
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(false),
        SRDescription(nameof(SR.FDshowHelpDescr))
        ]
        public bool ShowHelp {
            get {
                return GetOption(NativeMethods.OFN_SHOWHELP);
            }
            set {
                SetOption(NativeMethods.OFN_SHOWHELP, value);
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.SupportMultipleExtensions"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets whether def or abc.def is the extension of the file filename.abc.def
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(false),
        SRDescription(nameof(SR.FDsupportMultiDottedExtensionsDescr))
        ]
        public bool SupportMultiDottedExtensions
        {
            get
            {
                return this.supportMultiDottedExtensions;
            }
            set
            {
                this.supportMultiDottedExtensions = value;
            }
        } 

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.Title"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the file dialog box title.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)), 
        DefaultValue(""),
        Localizable(true),
        SRDescription(nameof(SR.FDtitleDescr))
        ]
        public string Title {
            get {
                return title == null? "": title;
            }
            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();

                title = value;
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.ValidateNames"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the dialog box accepts only valid
        ///       Win32 file names.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(true),
        SRDescription(nameof(SR.FDvalidateNamesDescr))
        ]
        public bool ValidateNames {
            get {
                return !GetOption(NativeMethods.OFN_NOVALIDATE);
            }
            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileDialogCustomization Demanded");
                IntSecurity.FileDialogCustomization.Demand();

                SetOption(NativeMethods.OFN_NOVALIDATE, !value);
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.FileOk"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the user clicks on the Open or Save button on a file dialog
        ///       box.
        ///    </para>
        /// <remarks>
        ///    <para>
        ///       For information about handling events, see <see topic='cpconEventsOverview'/>.
        ///    </para>
        /// </remarks>
        /// </devdoc>
        [SRDescription(nameof(SR.FDfileOkDescr))]
        public event CancelEventHandler FileOk {
            add {
                Events.AddHandler(EventFileOk, value);
            }
            remove {
                Events.RemoveHandler(EventFileOk, value);
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.DoFileOk"]/*' />
        /// <devdoc>
        ///     Processes the CDN_FILEOK notification.
        /// </devdoc>
        private bool DoFileOk(IntPtr lpOFN) {
            NativeMethods.OPENFILENAME_I ofn = (NativeMethods.OPENFILENAME_I)UnsafeNativeMethods.PtrToStructure(lpOFN, typeof(NativeMethods.OPENFILENAME_I));
            int saveOptions = options;
            int saveFilterIndex = filterIndex;
            string[] saveFileNames = fileNames;
            bool saveSecurityCheckFileNames = securityCheckFileNames;
            bool ok = false;
            try {
                options = options & ~NativeMethods.OFN_READONLY |
                          ofn.Flags & NativeMethods.OFN_READONLY;
                filterIndex = ofn.nFilterIndex;
                charBuffer.PutCoTaskMem(ofn.lpstrFile);

                // We are filling in the file names list with secure
                // data.  Any access to this list now will require
                // a security demand.  We set this bit before actually
                // setting the names; otherwise a thread ---- could
                // expose them.
                securityCheckFileNames = true;
                Thread.MemoryBarrier();

                if ((options & NativeMethods.OFN_ALLOWMULTISELECT) == 0) {
                    fileNames = new string[] {charBuffer.GetString()};
                }
                else {
                    fileNames = GetMultiselectFiles(charBuffer);
                }

                if (ProcessFileNames()) {
                    CancelEventArgs ceevent = new CancelEventArgs();
                    if (NativeWindow.WndProcShouldBeDebuggable) {
                        OnFileOk(ceevent);
                        ok = !ceevent.Cancel;
                    }
                    else {
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
            finally {
                if (!ok) {
                    securityCheckFileNames = saveSecurityCheckFileNames;
                    Thread.MemoryBarrier();
                    fileNames = saveFileNames;

                    options = saveOptions;
                    filterIndex = saveFilterIndex;
                }
            }
            return ok;
        }

        /// 


        [SuppressMessage("Microsoft.Security", "CA2103:ReviewImperativeSecurity")]
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        internal static bool FileExists(string fileName)
        {
            bool fileExists = false;
            try {
                // 

                new FileIOPermission(FileIOPermissionAccess.Read, IntSecurity.UnsafeGetFullPath(fileName)).Assert();
                try {
                    fileExists = File.Exists(fileName);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }
            catch (System.IO.PathTooLongException) {
            }
            return fileExists;
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.GetMultiselectFiles"]/*' />
        /// <devdoc>
        ///     Extracts the filename(s) returned by the file dialog.
        /// </devdoc>
        private string[] GetMultiselectFiles(CharBuffer charBuffer) {
            string directory = charBuffer.GetString();
            string fileName = charBuffer.GetString();
            if (fileName.Length == 0) return new string[] {
                    directory
                };
            if (directory[directory.Length - 1] != '\\') {
                directory = directory + "\\";
            }
            ArrayList names = new ArrayList();
            do {
                if (fileName[0] != '\\' && (fileName.Length <= 3 ||
                                            fileName[1] != ':' || fileName[2] != '\\')) {
                    fileName = directory + fileName;
                }
                names.Add(fileName);
                fileName = charBuffer.GetString();
            } while (fileName.Length > 0);
            string[] temp = new string[names.Count];
            names.CopyTo(temp, 0);
            return temp;
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.GetOption"]/*' />
        /// <devdoc>
        ///     Returns the state of the given option flag.
        /// </devdoc>
        /// <internalonly/>

        internal bool GetOption(int option) {
            return(options & option) != 0;
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.HookProc"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Defines the common dialog box hook procedure that is overridden to add
        ///       specific functionality to the file dialog box.
        ///    </para>
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) {
            if (msg == NativeMethods.WM_NOTIFY) {
                dialogHWnd = UnsafeNativeMethods.GetParent(new HandleRef(null, hWnd));
                try {
                    UnsafeNativeMethods.OFNOTIFY notify = (UnsafeNativeMethods.OFNOTIFY)UnsafeNativeMethods.PtrToStructure(lparam, typeof(UnsafeNativeMethods.OFNOTIFY));

                    switch (notify.hdr_code) {
                        case -601: /* CDN_INITDONE */
                            MoveToScreenCenter(dialogHWnd);
                            break;
                        case -602: /* CDN_SELCHANGE */
                            NativeMethods.OPENFILENAME_I ofn = (NativeMethods.OPENFILENAME_I)UnsafeNativeMethods.PtrToStructure(notify.lpOFN, typeof(NativeMethods.OPENFILENAME_I));
                            // Get the buffer size required to store the selected file names.
                            int sizeNeeded = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, dialogHWnd), 1124 /*CDM_GETSPEC*/, System.IntPtr.Zero, System.IntPtr.Zero);
                            if (sizeNeeded > ofn.nMaxFile) {
                                // A bigger buffer is required.
                                try {
                                    int newBufferSize = sizeNeeded + (FILEBUFSIZE / 4);
                                    // Allocate new buffer
                                    CharBuffer charBufferTmp = CharBuffer.CreateBuffer(newBufferSize);
                                    IntPtr newBuffer = charBufferTmp.AllocCoTaskMem();
                                    // Free old buffer
                                    Marshal.FreeCoTaskMem(ofn.lpstrFile);
                                    // Substitute buffer
                                    ofn.lpstrFile = newBuffer;
                                    ofn.nMaxFile = newBufferSize;
                                    this.charBuffer = charBufferTmp;
                                    Marshal.StructureToPtr(ofn, notify.lpOFN, true);
                                    Marshal.StructureToPtr(notify, lparam, true);
                                }
                                catch {
                                    // intentionaly not throwing here.
                                }
                            }
                            this.ignoreSecondFileOkNotification = false;
                            break;
                        case -604: /* CDN_SHAREVIOLATION */
                            // When the selected file is locked for writing,
                            // we get this notification followed by *two* CDN_FILEOK notifications.                            
                            this.ignoreSecondFileOkNotification = true;  // We want to ignore the second CDN_FILEOK
                            this.okNotificationCount = 0;                // to avoid a second prompt by PromptFileOverwrite.
                            break;
                        case -606: /* CDN_FILEOK */
                            if (this.ignoreSecondFileOkNotification)
                            {
                                // We got a CDN_SHAREVIOLATION notification and want to ignore the second CDN_FILEOK notification
                                if (this.okNotificationCount == 0)
                                {
                                    this.okNotificationCount = 1;   // This one is the first and is all right.
                                }
                                else
                                {
                                    // This is the second CDN_FILEOK, so we want to ignore it.
                                    this.ignoreSecondFileOkNotification = false;
                                    UnsafeNativeMethods.SetWindowLong(new HandleRef(null, hWnd), 0, new HandleRef(null, NativeMethods.InvalidIntPtr));
                                    return NativeMethods.InvalidIntPtr;
                                }
                            }
                            if (!DoFileOk(notify.lpOFN)) {
                                UnsafeNativeMethods.SetWindowLong(new HandleRef(null, hWnd), 0, new HandleRef(null, NativeMethods.InvalidIntPtr));
                                return NativeMethods.InvalidIntPtr;
                            }
                            break;
                    }
                }
                catch {
                    if (dialogHWnd != IntPtr.Zero) {
                        UnsafeNativeMethods.EndDialog(new HandleRef(this, dialogHWnd), IntPtr.Zero);
                    }
                    throw;
                }
            }
            return IntPtr.Zero;
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.MakeFilterString"]/*' />
        /// <devdoc>
        ///     Converts the given filter string to the format required in an OPENFILENAME_I
        ///     structure.
        /// </devdoc>
        private static string MakeFilterString(string s, bool dereferenceLinks) {
            if (s == null || s.Length == 0)
            {
                // Workaround for Whidbey 

                if (dereferenceLinks && System.Environment.OSVersion.Version.Major >= 5)
                {
                    s = " |*.*";
                }
                else if (s == null)
                {
                    return null;
                }
            }
            int length = s.Length;
            char[] filter = new char[length + 2];
            s.CopyTo(0, filter, 0, length);
            for (int i = 0; i < length; i++) {
                if (filter[i] == '|') filter[i] = (char)0;
            }
            filter[length + 1] = (char)0;
            return new string(filter);
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.OnFileOk"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.FileDialog.FileOk'/> event.
        ///    </para>
        /// </devdoc>
        protected void OnFileOk(CancelEventArgs e) {
            CancelEventHandler handler = (CancelEventHandler)Events[EventFileOk];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.ProcessFileNames"]/*' />
        /// <devdoc>
        ///     Processes the filenames entered in the dialog according to the settings
        ///     of the "addExtension", "checkFileExists", "createPrompt", and
        ///     "overwritePrompt" properties.
        /// </devdoc>
        private bool ProcessFileNames() {
            if ((options & NativeMethods.OFN_NOVALIDATE) == 0) {
                string[] extensions = FilterExtensions;
                for (int i = 0; i < fileNames.Length; i++) {
                    string fileName = fileNames[i];
                    if ((options & OPTION_ADDEXTENSION) != 0 && !Path.HasExtension(fileName)) {
                        bool fileMustExist = (options & NativeMethods.OFN_FILEMUSTEXIST) != 0;

                        for (int j = 0; j < extensions.Length; j++) {
                            string currentExtension = Path.GetExtension(fileName);
                            
                            Debug.Assert(!extensions[j].StartsWith("."), 
                                         "FileDialog.FilterExtensions should not return things starting with '.'");
                            Debug.Assert(currentExtension.Length == 0 || currentExtension.StartsWith("."), 
                                         "File.GetExtension should return something that starts with '.'");
                            
                            string s = fileName.Substring(0, fileName.Length - currentExtension.Length);

                            // we don't want to append the extension if it contains wild cards
                            if (extensions[j].IndexOfAny(new char[] { '*', '?' }) == -1) {
                                s += "." + extensions[j];
                            }

                            if (!fileMustExist || FileExists(s)) {
                                fileName = s;
                                break;
                            }
                        }
                        fileNames[i] = fileName;
                    }
                    if (!PromptUserIfAppropriate(fileName))
                        return false;
                }
            }
            return true;
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.MessageBoxWithFocusRestore"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Prompts the user with a <see cref='System.Windows.Forms.MessageBox'/>
        ///       with the given parameters. It also ensures that
        ///       the focus is set back on the window that had
        ///       the focus to begin with (before we displayed
        ///       the MessageBox).
        ///    </para>
        /// </devdoc>
        internal bool MessageBoxWithFocusRestore(string message, string caption,
                MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            bool ret;
            IntPtr focusHandle = UnsafeNativeMethods.GetFocus();           
            try {
                ret = RTLAwareMessageBox.Show(null, message, caption, buttons, icon,
                        MessageBoxDefaultButton.Button1, 0) == DialogResult.Yes;
            }
            finally {
                UnsafeNativeMethods.SetFocus(new HandleRef(null, focusHandle));
            }
            return ret;
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.PromptFileNotFound"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Prompts the user with a <see cref='System.Windows.Forms.MessageBox'/>
        ///       when a file
        ///       does not exist.
        ///    </para>
        /// </devdoc>
        private void PromptFileNotFound(string fileName) {
            MessageBoxWithFocusRestore(string.Format(SR.FileDialogFileNotFound, fileName), DialogCaption,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // If it's necessary to throw up a "This file exists, are you sure?" kind of
        // MessageBox, here's where we do it
        // Return value is whether or not the user hit "okay".
        internal virtual bool PromptUserIfAppropriate(string fileName) {
            if ((options & NativeMethods.OFN_FILEMUSTEXIST) != 0) {
                if (!FileExists(fileName)) {
                    PromptFileNotFound(fileName);
                    return false;
                }
            }
            return true;
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.Reset"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Resets all properties to their default values.
        ///    </para>
        /// </devdoc>
        public override void Reset() {
            options = NativeMethods.OFN_HIDEREADONLY | NativeMethods.OFN_PATHMUSTEXIST |
                      OPTION_ADDEXTENSION;
            title = null;
            initialDir = null;
            defaultExt = null;
            fileNames = null;
            filter = null;
            filterIndex = 1;
            supportMultiDottedExtensions = false;
            this._customPlaces.Clear();
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.RunDialog"]/*' />
        /// <devdoc>
        ///    Implements running of a file dialog.
        /// </devdoc>
        /// <internalonly/>
        protected override bool RunDialog(IntPtr hWndOwner) {
            if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != System.Threading.ApartmentState.STA) {
                throw new System.Threading.ThreadStateException(string.Format(SR.DebuggingExceptionOnly, SR.ThreadMustBeSTA));
            }
            EnsureFileDialogPermission();
            if (this.UseVistaDialogInternal)
            {
                return RunDialogVista(hWndOwner);
            }
            else
            {
                return RunDialogOld(hWndOwner);
            }
        }

        internal abstract void EnsureFileDialogPermission();

        private bool RunDialogOld(IntPtr hWndOwner)
        {
            NativeMethods.WndProc hookProcPtr = new NativeMethods.WndProc(this.HookProc);
            NativeMethods.OPENFILENAME_I ofn = new NativeMethods.OPENFILENAME_I();
            try {
                charBuffer = CharBuffer.CreateBuffer(FILEBUFSIZE);
                if (fileNames != null) {
                    charBuffer.PutString(fileNames[0]);
                }
                ofn.lStructSize = Marshal.SizeOf(typeof(NativeMethods.OPENFILENAME_I));
                // Degrade to the older style dialog if we're not on Win2K.
                // We do this by setting the struct size to a different value
                //
                if (Environment.OSVersion.Platform != System.PlatformID.Win32NT ||
                    Environment.OSVersion.Version.Major < 5) {
                    ofn.lStructSize = 0x4C;
                }
                ofn.hwndOwner = hWndOwner;
                ofn.hInstance = Instance;
                ofn.lpstrFilter = MakeFilterString(filter, this.DereferenceLinks);
                ofn.nFilterIndex = filterIndex;
                ofn.lpstrFile = charBuffer.AllocCoTaskMem();
                ofn.nMaxFile = FILEBUFSIZE;
                ofn.lpstrInitialDir = initialDir;
                ofn.lpstrTitle = title;
                ofn.Flags = Options | (NativeMethods.OFN_EXPLORER | NativeMethods.OFN_ENABLEHOOK | NativeMethods.OFN_ENABLESIZING);
                ofn.lpfnHook = hookProcPtr;
                ofn.FlagsEx = NativeMethods.OFN_USESHELLITEM;
                if (defaultExt != null && AddExtension) {
                    ofn.lpstrDefExt = defaultExt;
                }
                //Security checks happen here
                return RunFileDialog(ofn);
            }
            finally {
                charBuffer = null;
                if (ofn.lpstrFile != IntPtr.Zero) {
                    Marshal.FreeCoTaskMem(ofn.lpstrFile);
                }
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.RunFileDialog"]/*' />
        /// <devdoc>
        ///     Implements the actual call to GetOPENFILENAME_I or GetSaveFileName.
        /// </devdoc>
        /// <internalonly/>
        internal abstract bool RunFileDialog(NativeMethods.OPENFILENAME_I ofn);

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.SetOption"]/*' />
        /// <devdoc>
        ///     Sets the given option to the given boolean value.
        /// </devdoc>
        /// <internalonly/>
        internal void SetOption(int option, bool value) {
            if (value) {
                options |= option;
            }
            else {
                options &= ~option;
            }
        }

        /// <include file='doc\FileDialog.uex' path='docs/doc[@for="FileDialog.ToString"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Provides a string version of this Object.
        ///    </para>
        /// </devdoc>
        public override string ToString() {
            StringBuilder sb = new StringBuilder(base.ToString() + ": Title: " + Title + ", FileName: ");
            try
            {
                sb.Append(FileName);
            }
            catch (Exception e)
            {
                sb.Append("<");
                sb.Append(e.GetType().FullName);
                sb.Append(">");
            }
            return sb.ToString();
        }

        
    }
}


