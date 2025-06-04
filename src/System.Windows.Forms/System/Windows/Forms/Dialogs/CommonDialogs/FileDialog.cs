// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using Windows.Win32.UI.Controls.Dialogs;
using static Windows.Win32.UI.Controls.Dialogs.OPEN_FILENAME_FLAGS;
using static Windows.Win32.UI.Shell.FILEOPENDIALOGOPTIONS;

namespace System.Windows.Forms;

/// <summary>
///  Displays a dialog window from which the user can select a file.
/// </summary>
[DefaultEvent(nameof(FileOk))]
[DefaultProperty(nameof(FileName))]
public abstract partial class FileDialog : CommonDialog
{
    private const int FileBufferSize = 8192;
    private static readonly char[] s_wildcards = ['*', '?'];

    protected static readonly object EventFileOk = new();

    private protected OPEN_FILENAME_FLAGS _fileNameFlags;
    private protected FILEOPENDIALOGOPTIONS _dialogOptions;

    private string? _title;
    private string? _initialDirectory;
    private string? _defaultExtension;
    private string[]? _fileNames;
    private string? _filter;
    private bool _ignoreSecondFileOkNotification;
    private int _okNotificationCount;
    private char[]? _charBuffer;
    private HWND _dialogHWnd;

    /// <summary>
    ///  In an inherited class, initializes a new instance of the <see cref="FileDialog"/> class.
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
    public bool AddExtension { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box adds the file being opened
    ///  or saved to the recent list.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.FileDialogAddToRecentDescr))]
    public bool AddToRecent
    {
        get => !GetOption(OFN_DONTADDTORECENT);
        set => SetOption(OFN_DONTADDTORECENT, !value);
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
        get => GetOption(OFN_FILEMUSTEXIST);
        set => SetOption(OFN_FILEMUSTEXIST, value);
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
        get => GetOption(OFN_PATHMUSTEXIST);
        set => SetOption(OFN_PATHMUSTEXIST, value);
    }

    /// <summary>
    ///  <para>
    ///   Gets or sets the GUID to associate with this dialog state. Typically, state such
    ///   as the last visited folder and the position and size of the dialog is persisted
    ///   based on the name of the executable file. By specifying a GUID, an application can
    ///   have different persisted states for different versions of the dialog within the
    ///   same application (for example, an import dialog and an open dialog).
    ///  </para>
    ///  <para>
    ///   This functionality is not available if an application is not using visual styles
    ///   or if <see cref="AutoUpgradeEnabled"/> is set to <see langword="false"/>.
    ///  </para>
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
    [AllowNull]
    public string DefaultExt
    {
        get => _defaultExtension ?? string.Empty;
        set
        {
            string? defaultExt = value;
            if (defaultExt is not null)
            {
                if (defaultExt.StartsWith('.'))
                {
                    defaultExt = defaultExt[1..];
                }
                else if (defaultExt.Length == 0)
                {
                    defaultExt = null;
                }
            }

            _defaultExtension = defaultExt;
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
        get => !GetOption(OFN_NODEREFERENCELINKS);
        set => SetOption(OFN_NODEREFERENCELINKS, !value);
    }

    private protected string DialogCaption => PInvokeCore.GetWindowText(_dialogHWnd);

    /// <summary>
    ///  Gets or sets a string containing the file name selected in the file dialog box.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue("")]
    [SRDescription(nameof(SR.FDfileNameDescr))]
    [AllowNull]
    public string FileName
    {
        get => _fileNames is { } names && names.Length > 0 ? names[0] : string.Empty;
        set => _fileNames = value is not null ? [value] : null;
    }

    /// <summary>
    ///  Gets the file names of all selected files in the dialog box.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.FDFileNamesDescr))]
    [AllowNull]
    public string[] FileNames
    {
        get => _fileNames is not null ? (string[])_fileNames.Clone() : [];
    }

    /// <summary>
    ///  Gets or sets the current file name filter string, which determines the choices
    ///  that appear in the "Save as file type" or "Files of type" box in the dialog box.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue("")]
    [Localizable(true)]
    [SRDescription(nameof(SR.FDfilterDescr))]
    [AllowNull]
    public string Filter
    {
        get => _filter ?? string.Empty;
        set
        {
            if (value == _filter)
            {
                return;
            }

            if (!string.IsNullOrEmpty(value))
            {
                int pipeCount = value.AsSpan().Count('|');
                if (pipeCount % 2 == 0)
                {
                    throw new ArgumentException(SR.FileDialogInvalidFilter, nameof(value));
                }
            }
            else
            {
                value = null!;
            }

            _filter = value;
        }
    }

    /// <summary>
    ///  Extracts the file extensions specified by the current file filter into an
    ///  array of strings. None of the extensions contain .'s, and the  default
    ///  extension is first.
    /// </summary>
    private string[] FilterExtensions
    {
        get
        {
            string? filter = _filter;
            List<string> extensions = [];

            // First extension is the default one. It's not expected that DefaultExt
            // is not in the filters list, but this is legal.
            if (_defaultExtension is not null)
            {
                extensions.Add(_defaultExtension);
            }

            if (filter is not null)
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
                            extensions.Add(ext[(i + 1)..]);
                        }
                    }
                }
            }

            return [.. extensions];
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
    [Editor("System.Windows.Forms.Design.InitialDirectoryEditor, System.Windows.Forms.Design, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", typeof(UITypeEditor))]
    [SRDescription(nameof(SR.FDinitialDirDescr))]
    [AllowNull]
    public string InitialDirectory
    {
        get => _initialDirectory ?? string.Empty;
        set => _initialDirectory = value;
    }

    /// <summary>
    ///  Gets the Win32 instance handle for the application.
    /// </summary>
    protected virtual nint Instance => PInvoke.GetModuleHandle((PCWSTR)null);

    /// <summary>
    ///  Gets the Win32 common Open File Dialog OFN_* and FOS_* option flags.
    /// </summary>
    protected int Options =>
        (int)(_fileNameFlags & (OFN_READONLY | OFN_HIDEREADONLY | OFN_NOCHANGEDIR | OFN_SHOWHELP | OFN_NOVALIDATE
          | OFN_ALLOWMULTISELECT | OFN_PATHMUSTEXIST | OFN_NODEREFERENCELINKS | OFN_DONTADDTORECENT
          | OFN_NOREADONLYRETURN | OFN_NOTESTFILECREATE | OFN_FORCESHOWHIDDEN))
          |
        (int)(_dialogOptions & (FOS_OKBUTTONNEEDSINTERACTION | FOS_HIDEPINNEDPLACES | FOS_DEFAULTNOMINIMODE | FOS_FORCEPREVIEWPANEON));

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box restores the current
    ///  directory before closing.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FDrestoreDirectoryDescr))]
    public bool RestoreDirectory
    {
        get => GetOption(OFN_NOCHANGEDIR);
        set => SetOption(OFN_NOCHANGEDIR, value);
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
        get => GetOption(OFN_SHOWHELP);
        set => SetOption(OFN_SHOWHELP, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box displays hidden and system files.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FileDialogShowHiddenFilesDescr))]
    public bool ShowHiddenFiles
    {
        get => GetOption(OFN_FORCESHOWHIDDEN);
        set => SetOption(OFN_FORCESHOWHIDDEN, value);
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
    [AllowNull]
    public string Title
    {
        get => _title ?? string.Empty;
        set => _title = value;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box accepts only valid Win32 file names.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.FDvalidateNamesDescr))]
    public bool ValidateNames
    {
        get => !GetOption(OFN_NOVALIDATE);
        set => SetOption(OFN_NOVALIDATE, !value);
    }

    /// <summary>
    ///  Occurs when the user clicks on the Open or Save button on a file dialog box.
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
    private unsafe bool DoFileOk(OPENFILENAME* lpOFN)
    {
        OPEN_FILENAME_FLAGS saveOptions = _fileNameFlags;
        int saveFilterIndex = FilterIndex;
        string[]? saveFileNames = _fileNames;
        bool ok = false;
        try
        {
            _fileNameFlags = _fileNameFlags & ~OFN_READONLY | lpOFN->Flags & OFN_READONLY;
            FilterIndex = (int)lpOFN->nFilterIndex;

            Thread.MemoryBarrier();

            _fileNames = _fileNameFlags.HasFlag(OFN_ALLOWMULTISELECT)
                ? GetMultiselectFiles(new((char*)lpOFN->lpstrFile, (int)lpOFN->nMaxFile))
                : [lpOFN->lpstrFile.ToString()];

            if (!ProcessFileNames(_fileNames))
            {
                return ok;
            }

            CancelEventArgs ceevent = new();
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

            return ok;
        }
        finally
        {
            if (!ok)
            {
                Thread.MemoryBarrier();
                _fileNames = saveFileNames;

                _fileNameFlags = saveOptions;
                FilterIndex = saveFilterIndex;
            }
        }
    }

    private protected static bool FileExists(string? fileName)
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
    private static string[] GetMultiselectFiles(ReadOnlySpan<char> fileBuffer)
    {
        var directory = fileBuffer.SliceAtFirstNull();
        var fileNames = fileBuffer[(directory.Length + 1)..];

        // When a single file is returned, the directory is not null delimited.
        // So we check here to see if the filename starts with a null.
        if (fileNames.Length == 0 || fileNames[0] == '\0')
        {
            return [directory.ToString()];
        }

        List<string> names = [];
        var fileName = fileNames.SliceAtFirstNull();
        while (fileName.Length > 0)
        {
            names.Add(Path.IsPathFullyQualified(fileName)
                ? fileName.ToString()
                : Path.Join(directory, fileName));

            fileNames = fileNames[(fileName.Length + 1)..];
            fileName = fileNames.SliceAtFirstNull();
        }

        return [.. names];
    }

    /// <summary>
    ///  Returns the state of the given option flag.
    /// </summary>
    private protected bool GetOption(OPEN_FILENAME_FLAGS option) => (_fileNameFlags & option) != 0;

    /// <summary>
    ///  Defines the common dialog box hook procedure that is overridden to add
    ///  specific functionality to the file dialog box.
    /// </summary>
    protected override unsafe IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        if (msg != (int)PInvokeCore.WM_NOTIFY)
        {
            return IntPtr.Zero;
        }

        _dialogHWnd = PInvoke.GetParent((HWND)hWnd);
        try
        {
            OFNOTIFY* notify = (OFNOTIFY*)lparam;

            switch (notify->hdr.code)
            {
                case PInvoke.CDN_INITDONE:
                    MoveToScreenCenter(_dialogHWnd);
                    break;
                case PInvoke.CDN_SELCHANGE:
                    // Get the buffer size required to store the selected file names.
                    int sizeNeeded = (int)PInvokeCore.SendMessage(_dialogHWnd, PInvoke.CDM_GETSPEC);
                    if (sizeNeeded > notify->lpOFN->nMaxFile)
                    {
                        // A bigger buffer is required.
                        int newBufferSize = sizeNeeded + (FileBufferSize / 4);

                        // Allocate new buffer
                        _charBuffer = GC.AllocateArray<char>(newBufferSize, pinned: true);

                        fixed (char* buffer = _charBuffer)
                        {
                            // Substitute buffer
                            notify->lpOFN->lpstrFile = buffer;
                            notify->lpOFN->nMaxFile = (uint)newBufferSize;
                        }
                    }

                    _ignoreSecondFileOkNotification = false;
                    break;
                case PInvoke.CDN_SHAREVIOLATION:
                    // When the selected file is locked for writing,
                    // we get this notification followed by *two* CDN_FILEOK notifications.
                    _ignoreSecondFileOkNotification = true;  // We want to ignore the second CDN_FILEOK
                    _okNotificationCount = 0;                // to avoid a second prompt by PromptFileOverwrite.
                    break;
                case PInvoke.CDN_FILEOK:
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
                            PInvokeCore.SetWindowLong((HWND)hWnd, 0, -1);
                            return -1;
                        }
                    }

                    if (!DoFileOk(notify->lpOFN))
                    {
                        PInvokeCore.SetWindowLong((HWND)hWnd, 0, -1);
                        return -1;
                    }

                    break;
            }
        }
        catch
        {
            if (!_dialogHWnd.IsNull)
            {
                PInvoke.EndDialog(_dialogHWnd, 0);
            }

            throw;
        }

        return IntPtr.Zero;
    }

    /// <summary>
    ///  Converts the given filter string to the format required in an OPENFILENAME structure.
    /// </summary>
    private static string? MakeFilterString(string? s, bool dereferenceLinks)
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

        return string.Create(s.Length + 2, s, static (span, s) =>
        {
            s.AsSpan().Replace(span, '|', '\0');
            span[^1] = '\0';
        });
    }

    /// <summary>
    ///  Raises the <see cref="FileOk"/> event.
    /// </summary>
    protected void OnFileOk(CancelEventArgs e)
    {
        CancelEventHandler? handler = (CancelEventHandler?)Events[EventFileOk];
        handler?.Invoke(this, e);
    }

    /// <summary>
    ///  Processes the filenames entered in the dialog according to the settings of the <see cref="AddExtension"/>,
    ///  <see cref="CheckFileExists"/>, and <see cref="ValidateNames"/> properties.
    /// </summary>
    private bool ProcessFileNames(string[] fileNames)
    {
        if (!ValidateNames)
        {
            return true;
        }

        string[] extensions = FilterExtensions;
        for (int i = 0; i < fileNames.Length; i++)
        {
            string fileName = fileNames[i];
            if (AddExtension && !Path.HasExtension(fileName))
            {
                bool fileMustExist = CheckFileExists;

                for (int j = 0; j < extensions.Length; j++)
                {
                    var currentExtension = Path.GetExtension(fileName.AsSpan());

                    Debug.Assert(!extensions[j].StartsWith('.'),
                        "FileDialog.FilterExtensions should not return things starting with '.'");
                    Debug.Assert(currentExtension.Length == 0 || currentExtension[0] == '.',
                        "File.GetExtension should return something that starts with '.'");

                    // We don't want to append the extension if it contains wild cards
                    string s = extensions[j].IndexOfAny(s_wildcards) == -1
                        ? $"{fileName[..^currentExtension.Length]}.{extensions[j]}"
                        : fileName[..^currentExtension.Length];

                    if (!fileMustExist || FileExists(s))
                    {
                        fileName = s;
                        break;
                    }
                }

                fileNames[i] = fileName;
            }

            if (!PromptUserIfAppropriate(fileName))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///  Prompts the user with a <see cref="MessageBox"/> with the given parameters. It also ensures that the
    ///  focus is set back on the window that had the focus to begin with (before we displayed the MessageBox).
    /// </summary>
    private protected static bool MessageBoxWithFocusRestore(
        string message,
        string caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon)
    {
        HWND focusHandle = PInvoke.GetFocus();
        try
        {
            return RTLAwareMessageBox.Show(null, message, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0)
                == DialogResult.Yes;
        }
        finally
        {
            PInvoke.SetFocus(focusHandle);
        }
    }

    /// <summary>
    ///  If it's necessary to throw up a "This file exists, are you sure?" kind of MessageBox,
    ///  here's where we do it. Return value is whether or not the user hit "okay".
    /// </summary>
    private protected virtual bool PromptUserIfAppropriate(string fileName)
    {
        if (_fileNameFlags.HasFlag(OFN_FILEMUSTEXIST) && !FileExists(fileName))
        {
            MessageBoxWithFocusRestore(
                string.Format(SR.FileDialogFileNotFound, fileName),
                DialogCaption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    /// <summary>
    ///  Resets all properties to their default values.
    /// </summary>
    public override void Reset()
    {
        _fileNameFlags = OFN_HIDEREADONLY | OFN_PATHMUSTEXIST;
        _dialogOptions = default;
        AddExtension = true;
        _title = null;
        _initialDirectory = null;
        _defaultExtension = null;
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
#pragma warning disable CA1725 // Parameter names should match base declaration - shipped and documented with this casing
    protected override bool RunDialog(IntPtr hWndOwner)
#pragma warning restore CA1725
    {
        if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(string.Format(SR.DebuggingExceptionOnly, SR.ThreadMustBeSTA));
        }

        // If running the Vista dialog fails (e.g. on Server Core), we fall back to the legacy dialog.
        if (UseVistaDialogInternal && TryRunDialogVista((HWND)hWndOwner, out bool returnValue))
        {
            return returnValue;
        }

        return RunDialogOld((HWND)hWndOwner);
    }

    private unsafe bool RunDialogOld(HWND owner)
    {
        _charBuffer = GC.AllocateArray<char>(FileBufferSize, pinned: true);
        FileName.CopyTo(_charBuffer);

        string? filterString = MakeFilterString(_filter, DereferenceLinks);

        fixed (char* buffer = _charBuffer)
        fixed (char* filter = filterString)
        fixed (char* initialDirectory = _initialDirectory)
        fixed (char* title = _title)
        fixed (char* extension = _defaultExtension)
        {
            OPENFILENAME ofn = new()
            {
                lStructSize = (uint)sizeof(OPENFILENAME),
                hwndOwner = owner,
                hInstance = (HINSTANCE)Instance,
                lpstrFilter = filter,
                nFilterIndex = (uint)FilterIndex,
                lpstrFile = buffer,
                nMaxFile = (uint)_charBuffer.Length,
                lpstrInitialDir = initialDirectory,
                lpstrTitle = title,
                Flags = (OPEN_FILENAME_FLAGS)Options | OFN_EXPLORER | OFN_ENABLEHOOK | OFN_ENABLESIZING,
                FlagsEx = OPEN_FILENAME_FLAGS_EX.OFN_EX_NONE,
                lpstrDefExt = AddExtension ? extension : null,
                lpfnHook = HookProcFunctionPointer
            };

            try
            {
                return RunFileDialog(&ofn);
            }
            finally
            {
                _charBuffer = null;
            }
        }
    }

    /// <summary>
    ///  Implements the actual call to GetOpenFileName or GetSaveFileName.
    /// </summary>
    private protected abstract unsafe bool RunFileDialog(OPENFILENAME* ofn);

    /// <summary>
    ///  Sets the given option to the given boolean value.
    /// </summary>
    private protected void SetOption(OPEN_FILENAME_FLAGS option, bool value)
    {
        if (value)
        {
            _fileNameFlags |= option;
        }
        else
        {
            _fileNameFlags &= ~option;
        }
    }

    public override string ToString() => $"{base.ToString()}: Title: {Title}, FileName: {FileName}";
}
