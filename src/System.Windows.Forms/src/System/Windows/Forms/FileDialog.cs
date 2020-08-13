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

namespace System.Windows.Forms
{
    /// <summary>
    ///  Displays a dialog window from which the user can select a file.
    /// </summary>
    [DefaultEvent(nameof(FileOk))]
    [DefaultProperty(nameof(FileName))]
    public abstract partial class FileDialog : CommonDialog
    {
        private const int FileBufferSize = 8192;

        protected static readonly object EventFileOk = new object(); // Don't rename (public API)

        private const int AddExtensionOption = unchecked(unchecked((int)0x80000000));

        private protected int _options;

        private string _title;
        private string _initialDir;
        private string _defaultExt;
        private string[] _fileNames;
        private string _filter;
        private bool _ignoreSecondFileOkNotification;
        private int _okNotificationCount;
        private UnicodeCharBuffer _charBuffer;
        private IntPtr _dialogHWnd;

        /// <summary>
        ///  In an inherited class, initializes a new instance of the <see cref='FileDialog'/>
        ///  class.
        /// </summary>
        internal FileDialog()
        {
            Reset();
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box automatically adds an
        ///  extension to a file name if the user omits the extension.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FDaddExtensionDescr))]
        public bool AddExtension
        {
            get => GetOption(AddExtensionOption);
            set => SetOption(AddExtensionOption, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box displays a warning
        ///  if the user specifies a file name that does not exist.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FDcheckFileExistsDescr))]
        public virtual bool CheckFileExists
        {
            get => GetOption((int)Comdlg32.OFN.FILEMUSTEXIST);
            set => SetOption((int)Comdlg32.OFN.FILEMUSTEXIST, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box displays a warning if
        ///  the user specifies a path that does not exist.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FDcheckPathExistsDescr))]
        public bool CheckPathExists
        {
            get => GetOption((int)Comdlg32.OFN.PATHMUSTEXIST);
            set => SetOption((int)Comdlg32.OFN.PATHMUSTEXIST, value);
        }

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
        /// or if <see cref="FileDialog.AutoUpgradeEnabled"/> is set to <see langword="false"/>.
        /// </para>
        /// </summary>
        [Localizable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid? ClientGuid { get; set; }

        /// <summary>
        ///  Gets or sets the default file extension.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue("")]
        [SRDescription(nameof(SR.FDdefaultExtDescr))]
        public string DefaultExt
        {
            get => _defaultExt ?? string.Empty;
            set
            {
                if (value != null)
                {
                    if (value.StartsWith("."))
                    {
                        value = value.Substring(1);
                    }
                    else if (value.Length == 0)
                    {
                        value = null;
                    }
                }

                _defaultExt = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box returns the location
        ///  of the file referenced by the shortcut or whether it returns the location
        ///  of the shortcut (.lnk).
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FDdereferenceLinksDescr))]
        public bool DereferenceLinks
        {
            get => !GetOption((int)Comdlg32.OFN.NODEREFERENCELINKS);
            set => SetOption((int)Comdlg32.OFN.NODEREFERENCELINKS, !value);
        }

        private protected string DialogCaption => User32.GetWindowText(new HandleRef(this, _dialogHWnd));

        /// <summary>
        ///  Gets or sets a string containing the file name selected in the file dialog box.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [DefaultValue("")]
        [SRDescription(nameof(SR.FDfileNameDescr))]
        public string FileName
        {
            get
            {
                if (_fileNames is null || string.IsNullOrEmpty(_fileNames[0]))
                {
                    return string.Empty;
                }

                return _fileNames[0];
            }
            set => _fileNames = value != null ? new string[] { value } : null;
        }

        /// <summary>
        ///  Gets the file names of all selected files in the dialog box.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FDFileNamesDescr))]
        public string[] FileNames
        {
            get => _fileNames != null ? (string[])_fileNames.Clone() : Array.Empty<string>();
        }

        /// <summary>
        ///  Gets or sets the current file name filter string, which determines the choices
        ///  that appear in the "Save as file type" or "Files of type" box in the dialog box.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue("")]
        [Localizable(true)]
        [SRDescription(nameof(SR.FDfilterDescr))]
        public string Filter
        {
            get => _filter ?? string.Empty;
            set
            {
                if (value != _filter)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        string[] formats = value.Split('|');
                        if (formats is null || formats.Length % 2 != 0)
                        {
                            throw new ArgumentException(SR.FileDialogInvalidFilter, nameof(value));
                        }
                    }
                    else
                    {
                        value = null;
                    }

                    _filter = value;
                }
            }
        }

        /// <summary>
        ///  Extracts the file extensions specified by the current file filter into an
        ///  array of strings.  None of the extensions contain .'s, and the  default
        ///  extension is first.
        /// </summary>
        private string[] FilterExtensions
        {
            get
            {
                string filter = _filter;
                List<string> extensions = new List<string>();

                // First extension is the default one. It's not expected that DefaultExt
                // is not in the filters list, but this is legal.
                if (_defaultExt != null)
                {
                    extensions.Add(_defaultExt);
                }

                if (filter != null)
                {
                    string[] tokens = filter.Split('|');

                    if ((FilterIndex * 2) - 1 >= tokens.Length)
                    {
                        throw new InvalidOperationException(SR.FileDialogInvalidFilterIndex);
                    }

                    if (FilterIndex > 0)
                    {
                        string[] exts = tokens[(FilterIndex * 2) - 1].Split(';');
                        foreach (string ext in exts)
                        {
                            int i = SupportMultiDottedExtensions ? ext.IndexOf('.') : ext.LastIndexOf('.');
                            if (i >= 0)
                            {
                                extensions.Add(ext.Substring(i + 1, ext.Length - (i + 1)));
                            }
                        }
                    }
                }

                return extensions.ToArray();
            }
        }

        /// <summary>
        ///  Gets or sets the index of the filter currently selected in the file dialog box.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(1)]
        [SRDescription(nameof(SR.FDfilterIndexDescr))]
        public int FilterIndex { get; set; }

        /// <summary>
        ///  Gets or sets the initial directory displayed by the file dialog box.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [DefaultValue("")]
        [SRDescription(nameof(SR.FDinitialDirDescr))]
        public string InitialDirectory
        {
            get => _initialDir ?? string.Empty;
            set => _initialDir = value;
        }

        /// <summary>
        ///  Gets the Win32 instance handle for the application.
        /// </summary>
        protected virtual IntPtr Instance => Kernel32.GetModuleHandleW(null);

        /// <summary>
        ///  Gets the Win32 common Open File Dialog OFN_* option flags.
        /// </summary>
        protected int Options
        {
            get
            {
                return _options & (int)(Comdlg32.OFN.READONLY | Comdlg32.OFN.HIDEREADONLY |
                                  Comdlg32.OFN.NOCHANGEDIR | Comdlg32.OFN.SHOWHELP | Comdlg32.OFN.NOVALIDATE |
                                  Comdlg32.OFN.ALLOWMULTISELECT | Comdlg32.OFN.PATHMUSTEXIST |
                                  Comdlg32.OFN.NODEREFERENCELINKS);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box restores the current
        ///  directory before closing.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FDrestoreDirectoryDescr))]
        public bool RestoreDirectory
        {
            get => GetOption((int)Comdlg32.OFN.NOCHANGEDIR);
            set => SetOption((int)Comdlg32.OFN.NOCHANGEDIR, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether whether the Help button is displayed
        ///  in the file dialog.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FDshowHelpDescr))]
        public bool ShowHelp
        {
            get => GetOption((int)Comdlg32.OFN.SHOWHELP);
            set => SetOption((int)Comdlg32.OFN.SHOWHELP, value);
        }

        /// <summary>
        ///  Gets or sets whether def or abc.def is the extension of the file filename.abc.def
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FDsupportMultiDottedExtensionsDescr))]
        public bool SupportMultiDottedExtensions { get; set; }

        /// <summary>
        ///  Gets or sets the file dialog box title.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue("")]
        [Localizable(true)]
        [SRDescription(nameof(SR.FDtitleDescr))]
        public string Title
        {
            get => _title ?? string.Empty;
            set => _title = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box accepts only valid
        ///  Win32 file names.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FDvalidateNamesDescr))]
        public bool ValidateNames
        {
            get => !GetOption((int)Comdlg32.OFN.NOVALIDATE);
            set => SetOption((int)Comdlg32.OFN.NOVALIDATE, !value);
        }

        /// <summary>
        ///  Occurs when the user clicks on the Open or Save button on a file dialog
        ///  box.
        /// <remarks>
        ///  For information about handling events, see <see topic='cpconEventsOverview'/>.
        /// </remarks>
        /// </summary>
        [SRDescription(nameof(SR.FDfileOkDescr))]
        public event CancelEventHandler FileOk
        {
            add => Events.AddHandler(EventFileOk, value);
            remove => Events.RemoveHandler(EventFileOk, value);
        }

        /// <summary>
        ///  Processes the CDN_FILEOK notification.
        /// </summary>
        private bool DoFileOk(IntPtr lpOFN)
        {
            NativeMethods.OPENFILENAME_I ofn = Marshal.PtrToStructure<NativeMethods.OPENFILENAME_I>(lpOFN);
            int saveOptions = _options;
            int saveFilterIndex = FilterIndex;
            string[] saveFileNames = _fileNames;
            bool ok = false;
            try
            {
                _options = _options & ~(int)Comdlg32.OFN.READONLY |
                          ofn.Flags & (int)Comdlg32.OFN.READONLY;
                FilterIndex = ofn.nFilterIndex;
                _charBuffer.PutCoTaskMem(ofn.lpstrFile);

                Thread.MemoryBarrier();

                if ((_options & (int)Comdlg32.OFN.ALLOWMULTISELECT) == 0)
                {
                    _fileNames = new string[] { _charBuffer.GetString() };
                }
                else
                {
                    _fileNames = GetMultiselectFiles(_charBuffer);
                }

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
            }

            return ok;
        }

        private protected static bool FileExists(string fileName)
        {
            try
            {
                return File.Exists(fileName);
            }
            catch (PathTooLongException)
            {
                return false;
            }
        }

        /// <summary>
        ///  Extracts the filename(s) returned by the file dialog.
        /// </summary>
        private string[] GetMultiselectFiles(UnicodeCharBuffer charBuffer)
        {
            string directory = charBuffer.GetString();
            string fileName = charBuffer.GetString();
            if (fileName.Length == 0)
            {
                return new string[] { directory };
            }

            if (directory[directory.Length - 1] != '\\')
            {
                directory += "\\";
            }
            List<string> names = new List<string>();
            do
            {
                if (fileName[0] != '\\' && (fileName.Length <= 3 ||
                                            fileName[1] != ':' || fileName[2] != '\\'))
                {
                    fileName = directory + fileName;
                }

                names.Add(fileName);
                fileName = charBuffer.GetString();
            } while (fileName.Length > 0);

            return names.ToArray();
        }

        /// <summary>
        ///  Returns the state of the given option flag.
        /// </summary>
        private protected bool GetOption(int option) => (_options & option) != 0;

        /// <summary>
        ///  Defines the common dialog box hook procedure that is overridden to add
        ///  specific functionality to the file dialog box.
        /// </summary>
        protected unsafe override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            if (msg == (int)User32.WM.NOTIFY)
            {
                _dialogHWnd = User32.GetParent(hWnd);
                try
                {
                    Comdlg32.OFNOTIFYW* notify = (Comdlg32.OFNOTIFYW*)lparam;

                    switch (notify->hdr.code)
                    {
                        case -601: /* CDN_INITDONE */
                            MoveToScreenCenter(_dialogHWnd);
                            break;
                        case -602: /* CDN_SELCHANGE */
                            NativeMethods.OPENFILENAME_I ofn = Marshal.PtrToStructure<NativeMethods.OPENFILENAME_I>(notify->lpOFN);
                            // Get the buffer size required to store the selected file names.
                            int sizeNeeded = (int)User32.SendMessageW(new HandleRef(this, _dialogHWnd), (User32.WM)1124 /*CDM_GETSPEC*/, IntPtr.Zero, IntPtr.Zero);
                            if (sizeNeeded > ofn.nMaxFile)
                            {
                                // A bigger buffer is required.
                                try
                                {
                                    int newBufferSize = sizeNeeded + (FileBufferSize / 4);
                                    // Allocate new buffer
                                    var charBufferTmp = new UnicodeCharBuffer(newBufferSize);
                                    IntPtr newBuffer = charBufferTmp.AllocCoTaskMem();
                                    // Free old buffer
                                    Marshal.FreeCoTaskMem(ofn.lpstrFile);
                                    // Substitute buffer
                                    ofn.lpstrFile = newBuffer;
                                    ofn.nMaxFile = newBufferSize;
                                    _charBuffer = charBufferTmp;
                                    Marshal.StructureToPtr(ofn, notify->lpOFN, true);
                                }
                                catch
                                {
                                    // intentionaly not throwing here.
                                }
                            }
                            _ignoreSecondFileOkNotification = false;
                            break;
                        case -604: /* CDN_SHAREVIOLATION */
                            // When the selected file is locked for writing,
                            // we get this notification followed by *two* CDN_FILEOK notifications.
                            _ignoreSecondFileOkNotification = true;  // We want to ignore the second CDN_FILEOK
                            _okNotificationCount = 0;                // to avoid a second prompt by PromptFileOverwrite.
                            break;
                        case -606: /* CDN_FILEOK */
                            if (_ignoreSecondFileOkNotification)
                            {
                                // We got a CDN_SHAREVIOLATION notification and want to ignore the second CDN_FILEOK notification
                                if (_okNotificationCount == 0)
                                {
                                    // This one is the first and is all right.
                                    _okNotificationCount = 1;
                                }
                                else
                                {
                                    // This is the second CDN_FILEOK, so we want to ignore it.
                                    _ignoreSecondFileOkNotification = false;
                                    User32.SetWindowLong(hWnd, 0, NativeMethods.InvalidIntPtr);
                                    return NativeMethods.InvalidIntPtr;
                                }
                            }
                            if (!DoFileOk(notify->lpOFN))
                            {
                                User32.SetWindowLong(hWnd, 0, NativeMethods.InvalidIntPtr);
                                return NativeMethods.InvalidIntPtr;
                            }
                            break;
                    }
                }
                catch
                {
                    if (_dialogHWnd != IntPtr.Zero)
                    {
                        User32.EndDialog(new HandleRef(this, _dialogHWnd), IntPtr.Zero);
                    }

                    throw;
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        ///  Converts the given filter string to the format required in an OPENFILENAME_I
        ///  structure.
        /// </summary>
        private static string MakeFilterString(string s, bool dereferenceLinks)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (dereferenceLinks)
                {
                    s = " |*.*";
                }
                else if (s is null)
                {
                    return null;
                }
            }

            int length = s.Length;
            char[] filter = new char[length + 2];
            s.CopyTo(0, filter, 0, length);
            for (int i = 0; i < length; i++)
            {
                if (filter[i] == '|')
                {
                    filter[i] = '\0';
                }
            }

            filter[length + 1] = '\0';
            return new string(filter);
        }

        /// <summary>
        ///  Raises the <see cref='FileOk'/> event.
        /// </summary>
        protected void OnFileOk(CancelEventArgs e)
        {
            CancelEventHandler handler = (CancelEventHandler)Events[EventFileOk];
            handler?.Invoke(this, e);
        }

        /// <summary>
        ///  Processes the filenames entered in the dialog according to the settings
        ///  of the "addExtension", "checkFileExists", "createPrompt", and
        ///  "overwritePrompt" properties.
        /// </summary>
        private bool ProcessFileNames()
        {
            if ((_options & (int)Comdlg32.OFN.NOVALIDATE) == 0)
            {
                string[] extensions = FilterExtensions;
                for (int i = 0; i < _fileNames.Length; i++)
                {
                    string fileName = _fileNames[i];
                    if ((_options & AddExtensionOption) != 0 && !Path.HasExtension(fileName))
                    {
                        bool fileMustExist = (_options & (int)Comdlg32.OFN.FILEMUSTEXIST) != 0;

                        for (int j = 0; j < extensions.Length; j++)
                        {
                            string currentExtension = Path.GetExtension(fileName);

                            Debug.Assert(!extensions[j].StartsWith("."),
                                         "FileDialog.FilterExtensions should not return things starting with '.'");
                            Debug.Assert(currentExtension.Length == 0 || currentExtension.StartsWith("."),
                                         "File.GetExtension should return something that starts with '.'");

                            string s = fileName.Substring(0, fileName.Length - currentExtension.Length);

                            // we don't want to append the extension if it contains wild cards
                            if (extensions[j].IndexOfAny(new char[] { '*', '?' }) == -1)
                            {
                                s += "." + extensions[j];
                            }

                            if (!fileMustExist || FileExists(s))
                            {
                                fileName = s;
                                break;
                            }
                        }

                        _fileNames[i] = fileName;
                    }
                    if (!PromptUserIfAppropriate(fileName))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///  Prompts the user with a <see cref='MessageBox'/> with the
        ///  given parameters. It also ensures that the focus is set back on the window that
        ///  had the focus to begin with (before we displayed the MessageBox).
        /// </summary>
        private protected bool MessageBoxWithFocusRestore(string message, string caption,
                MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            IntPtr focusHandle = User32.GetFocus();
            try
            {
                return RTLAwareMessageBox.Show(null, message, caption, buttons, icon,
                        MessageBoxDefaultButton.Button1, 0) == DialogResult.Yes;
            }
            finally
            {
                User32.SetFocus(focusHandle);
            }
        }

        /// <summary>
        ///  Prompts the user with a <see cref='MessageBox'/> when a
        ///  file does not exist.
        /// </summary>
        private void PromptFileNotFound(string fileName)
        {
            MessageBoxWithFocusRestore(string.Format(SR.FileDialogFileNotFound, fileName), DialogCaption,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// If it's necessary to throw up a "This file exists, are you sure?" kind of MessageBox,
        /// here's where we do it. Return value is whether or not the user hit "okay".
        /// </summary>
        private protected virtual bool PromptUserIfAppropriate(string fileName)
        {
            if ((_options & (int)Comdlg32.OFN.FILEMUSTEXIST) != 0)
            {
                if (!FileExists(fileName))
                {
                    PromptFileNotFound(fileName);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  Resets all properties to their default values.
        /// </summary>
        public override void Reset()
        {
            _options = (int)(Comdlg32.OFN.HIDEREADONLY | Comdlg32.OFN.PATHMUSTEXIST) | AddExtensionOption;
            _title = null;
            _initialDir = null;
            _defaultExt = null;
            _fileNames = null;
            _filter = null;
            FilterIndex = 1;
            SupportMultiDottedExtensions = false;
            _customPlaces.Clear();
            ClientGuid = null;
        }

        /// <summary>
        ///  Implements running of a file dialog.
        /// </summary>
        protected override bool RunDialog(IntPtr hWndOwner)
        {
            if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != ApartmentState.STA)
            {
                throw new ThreadStateException(string.Format(SR.DebuggingExceptionOnly, SR.ThreadMustBeSTA));
            }

            // If running the Vista dialog fails (e.g. on Server Core), we fall back to the
            // legacy dialog.
            if (UseVistaDialogInternal && TryRunDialogVista(hWndOwner, out bool returnValue))
            {
                return returnValue;
            }

            return RunDialogOld(hWndOwner);
        }

        private bool RunDialogOld(IntPtr hWndOwner)
        {
            var hookProcPtr = new NativeMethods.WndProc(HookProc);
            var ofn = new NativeMethods.OPENFILENAME_I();
            try
            {
                _charBuffer = new UnicodeCharBuffer(FileBufferSize);
                if (_fileNames != null)
                {
                    _charBuffer.PutString(_fileNames[0]);
                }
                ofn.lStructSize = Marshal.SizeOf<NativeMethods.OPENFILENAME_I>();
                ofn.hwndOwner = hWndOwner;
                ofn.hInstance = Instance;
                ofn.lpstrFilter = MakeFilterString(_filter, DereferenceLinks);
                ofn.nFilterIndex = FilterIndex;
                ofn.lpstrFile = _charBuffer.AllocCoTaskMem();
                ofn.nMaxFile = FileBufferSize;
                ofn.lpstrInitialDir = _initialDir;
                ofn.lpstrTitle = _title;
                ofn.Flags = Options | (int)(Comdlg32.OFN.EXPLORER | Comdlg32.OFN.ENABLEHOOK | Comdlg32.OFN.ENABLESIZING);
                ofn.lpfnHook = hookProcPtr;
                ofn.FlagsEx = (int)Comdlg32.OFN_EX.NONE;
                if (_defaultExt != null && AddExtension)
                {
                    ofn.lpstrDefExt = _defaultExt;
                }

                return RunFileDialog(ofn);
            }
            finally
            {
                _charBuffer = null;
                if (ofn.lpstrFile != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(ofn.lpstrFile);
                }
            }
        }

        /// <summary>
        ///  Implements the actual call to GetOPENFILENAME_I or GetSaveFileName.
        /// </summary>
        private protected abstract bool RunFileDialog(NativeMethods.OPENFILENAME_I ofn);

        /// <summary>
        ///  Sets the given option to the given boolean value.
        /// </summary>
        private protected void SetOption(int option, bool value)
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

        /// <summary>
        ///  Provides a string version of this Object.
        /// </summary>
        public override string ToString()
        {
            return $"{base.ToString()}: Title: {Title}, FileName: {FileName}";
        }
    }
}
