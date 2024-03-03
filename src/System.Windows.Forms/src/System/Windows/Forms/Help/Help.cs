// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Globalization;
using Windows.Win32.Data.HtmlHelp;
using static Windows.Win32.Data.HtmlHelp.HTML_HELP_COMMAND;

namespace System.Windows.Forms;

/// <summary>
///  Represents the HTML 1.0 Help engine.
/// </summary>
public static class Help
{
    private const int HTML10HELP = 2;
    private const int HTMLFILE = 3;

    /// <summary>
    ///  Displays the contents of the Help file at located at a specified Url.
    /// </summary>
    public static void ShowHelp(Control? parent, string? url)
    {
        ShowHelp(parent, url, HelpNavigator.TableOfContents, null);
    }

    /// <summary>
    ///  Displays the contents of the Help file for a specific topic found at the specified Url.
    /// </summary>
    public static void ShowHelp(Control? parent, string? url, HelpNavigator navigator)
    {
        ShowHelp(parent, url, navigator, null);
    }

    /// <summary>
    ///  Displays the contents of the Help file for a specific topic found at the specified Url.
    /// </summary>
    public static void ShowHelp(Control? parent, string? url, string? keyword)
    {
        if (keyword is not null && keyword.Length != 0)
        {
            ShowHelp(parent, url, HelpNavigator.Topic, keyword);
        }
        else
        {
            ShowHelp(parent, url, HelpNavigator.TableOfContents, null);
        }
    }

    /// <summary>
    ///  Displays the contents of the Help file located at the Url supplied by the user.
    /// </summary>
    public static void ShowHelp(Control? parent, string? url, HelpNavigator command, object? parameter)
    {
        switch (GetHelpFileType(url))
        {
            case HTML10HELP:
                ShowHTML10Help(parent, url, command, parameter);
                break;
            case HTMLFILE:
                ShowHTMLFile(parent, url, command, parameter);
                break;
        }
    }

    /// <summary>
    ///  Displays the index of the specified file.
    /// </summary>
    public static void ShowHelpIndex(Control? parent, string? url)
    {
        ShowHelp(parent, url, HelpNavigator.Index, null);
    }

    /// <summary>
    ///  Displays a Help pop-up window.
    /// </summary>
    public static unsafe void ShowPopup(Control? parent, string caption, Point location)
    {
        HH_POPUP pop = new()
        {
            cbStruct = sizeof(HH_POPUP),
            pt = location,
            rcMargins = new RECT(-1, -1, -1, -1),               // Ignore
            clrForeground = new COLORREF(unchecked((uint)-1)),  // Ignore
            clrBackground = Application.SystemColors.Window
        };

        Font font = SystemFonts.StatusFont ?? SystemFonts.DefaultFont;
        string captionFont = $"{font.Name}, {font.SizeInPoints}, , {(font.Bold ? "BOLD" : "")}{(font.Italic ? "ITALIC" : "")}{(font.Underline ? "UNDERLINE" : "")}";

        fixed (char* pszText = caption, pszFont = captionFont)
        {
            pop.pszText = (sbyte*)pszText;
            pop.pszFont = (sbyte*)pszFont;
            ShowHTML10Help(parent, null, HelpNavigator.Topic, pop);
        }
    }

    /// <summary>
    ///  Displays HTML 1.0 Help with the specified parameters.
    /// </summary>
    private static unsafe void ShowHTML10Help(Control? parent, string? url, HelpNavigator command, object? param)
    {
        // See if we can get a full path and file name and if that will
        // resolve the out of memory condition with file names that include spaces.
        // If we can't, though, we can't assume that the path's no good: it might be in
        // the Windows help directory.
        Uri? file = null;
        string? pathAndFileName = url; // This is our best guess at the path yet.

        file = Resolve(url);
        if (file is not null)
        {
            // Can't assume we have a good url
            pathAndFileName = file.AbsoluteUri;
        }

        if (file is null || file.IsFile)
        {
            string? localPath = (file is not null && file.IsFile) ? file.LocalPath : url;

            // If this is a local path, convert it to a short path name. Pass 0 as the length the first time
            uint requiredStringSize = PInvoke.GetShortPathName(localPath, null, 0);
            if (requiredStringSize > 0)
            {
                // It's able to make it a short path.
                using BufferScope<char> shortName = new((int)requiredStringSize);
                fixed (char* pShortName = shortName)
                {
                    requiredStringSize = PInvoke.GetShortPathName(localPath, pShortName, requiredStringSize);
                    // If it can't make it a  short path, just leave the path we had.
                    pathAndFileName = shortName[..(int)requiredStringSize].ToString();
                }
            }
        }

        HandleRef<HWND> handle = parent is not null ? (new(parent)) : Control.GetHandleRef(PInvoke.GetActiveWindow());

        object? htmlParam;
        if (param is string stringParam)
        {
            HTML_HELP_COMMAND htmlCommand = MapCommandToHTMLCommand(command, stringParam, out htmlParam);
            if (htmlParam is string stringHtmlParam)
            {
                PInvoke.HtmlHelp(handle, pathAndFileName, htmlCommand, stringHtmlParam);
            }
            else if (htmlParam is int intParam)
            {
                PInvoke.HtmlHelp(handle, pathAndFileName, htmlCommand, in intParam);
            }
            else if (htmlParam is HH_FTS_QUERY query)
            {
                fixed (char* pszSearchQuery = stringParam)
                {
                    query.pszSearchQuery = (sbyte*)pszSearchQuery;
                    PInvoke.HtmlHelp(handle, pathAndFileName, htmlCommand, in query);
                }
            }
            else if (htmlParam is HH_AKLINK aLink)
            {
                // According to MSDN documentation, we have to ensure that the help window is up
                // before we call ALINK lookup.
                PInvoke.HtmlHelp(HWND.Null, pathAndFileName, HH_DISPLAY_TOPIC, null);

                fixed (char* pszKeywords = stringParam)
                {
                    aLink.pszKeywords = (sbyte*)pszKeywords;
                    PInvoke.HtmlHelp(handle, pathAndFileName, htmlCommand, in aLink);
                }
            }
            else
            {
                Debug.Fail($"Cannot handle HTML parameter of type: {htmlParam!.GetType()}");
                PInvoke.HtmlHelp(handle, pathAndFileName, htmlCommand, (string)param);
            }
        }
        else if (param is null)
        {
            PInvoke.HtmlHelp(handle, pathAndFileName, MapCommandToHTMLCommand(command, null, out htmlParam), null);
        }
        else if (param is HH_POPUP popup)
        {
            PInvoke.HtmlHelp(handle, pathAndFileName, HH_DISPLAY_TEXT_POPUP, ref popup);
        }
        else if (param.GetType() == typeof(int))
        {
            throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(param), "Integer"), nameof(param));
        }
    }

    /// <summary>
    ///  Displays HTMLFile with the specified parameters
    /// </summary>
    private static void ShowHTMLFile(Control? parent, string? url, HelpNavigator command, object? param)
    {
        Uri? file = Resolve(url) ?? throw new ArgumentException(string.Format(SR.HelpInvalidURL, url), nameof(url));

        switch (command)
        {
            case HelpNavigator.TableOfContents:
            case HelpNavigator.Find:
            case HelpNavigator.Index:
                // Nothing needed.
                break;
            case HelpNavigator.Topic:
                if (param is string stringParam)
                {
                    file = new Uri($"{file}#{stringParam}");
                }

                break;
        }

        HandleRef<HWND> handle = parent is not null ? new(parent) : Control.GetHandleRef(PInvoke.GetActiveWindow());
        string fileName = file.ToString();
        string? executable = file.IsFile ? FindExecutableInternal(file.LocalPath.ToString()) : null;
        PInvoke.ShellExecute(handle.Handle, lpOperation: null, executable ?? fileName, executable is not null ? fileName : null, lpDirectory: null, SHOW_WINDOW_CMD.SW_NORMAL);
        GC.KeepAlive(handle.Wrapper);
    }

    private static unsafe string? FindExecutableInternal(string uri)
    {
        HINSTANCE result;
        Span<char> buffer = stackalloc char[PInvoke.MAX_PATH + 1];
        fixed (char* lpFileLocal = uri)
        {
            fixed (char* b = buffer)
            {
                result = PInvoke.FindExecutable(lpFileLocal, lpDirectory: null, lpResult: b);
            }
        }

        return result <= 32 ? null : buffer.SliceAtFirstNull().ToString();
    }

    private static Uri? Resolve(string? partialUri)
    {
        Uri? file = null;

        if (!string.IsNullOrEmpty(partialUri))
        {
            try
            {
                file = new Uri(partialUri);
            }
            catch (UriFormatException)
            {
                // Ignore invalid uris.
            }
        }

        if (file is not null && file.Scheme == "file")
        {
            string localPath = file.LocalPath + file.Fragment;

            if (!File.Exists(localPath))
            {
                // Clear, and try relative to AppBase.
                file = null;
            }
        }

        if (file is null)
        {
            try
            {
                // Try relative to AppBase.
                file = new Uri(new Uri(AppContext.BaseDirectory),
                               partialUri);
            }
            catch (UriFormatException)
            {
                // Ignore invalid uris.
            }

            if (file is not null && file.Scheme == "file")
            {
                string localPath = file.LocalPath + file.Fragment;
                if (!File.Exists(localPath))
                {
                    // Clear - file isn't there.
                    file = null;
                }
            }
        }

        return file;
    }

    private static int GetHelpFileType(string? url)
    {
        if (url is null)
        {
            return HTMLFILE;
        }

        Uri? file = Resolve(url);

        if (file is null || file.Scheme == "file")
        {
            string ext = Path.GetExtension(file is null ? url : file.LocalPath + file.Fragment).ToLower(CultureInfo.InvariantCulture);
            if (ext is ".chm" or ".col")
            {
                return HTML10HELP;
            }
        }

        return HTMLFILE;
    }

    /// <summary>
    ///  Maps one of the COMMAND_* constants to the HTML 1.0 Help equivalent.
    /// </summary>
    private static unsafe HTML_HELP_COMMAND MapCommandToHTMLCommand(HelpNavigator command, string? param, out object? htmlParam)
    {
        htmlParam = param;

        if (string.IsNullOrEmpty(param) && (command == HelpNavigator.AssociateIndex || command == HelpNavigator.KeywordIndex))
        {
            return HH_DISPLAY_INDEX;
        }

        switch (command)
        {
            case HelpNavigator.Topic:
                return HH_DISPLAY_TOPIC;

            case HelpNavigator.TableOfContents:
                return HH_DISPLAY_TOC;

            case HelpNavigator.Index:
                return HH_DISPLAY_INDEX;

            case HelpNavigator.Find:
                {
                    HH_FTS_QUERY ftsQuery = new()
                    {
                        cbStruct = sizeof(HH_FTS_QUERY),
                        iProximity = (int)HH_FTS_DEFAULT_PROXIMITY,
                        fExecute = true,
                        fUniCodeStrings = true
                    };

                    htmlParam = ftsQuery;
                    return HH_DISPLAY_SEARCH;
                }

            case HelpNavigator.TopicId:
                {
                    if (int.TryParse(param, out int htmlParamAsInt))
                    {
                        htmlParam = htmlParamAsInt;
                        return HH_HELP_CONTEXT;
                    }

                    // default to just showing the index
                    return HH_DISPLAY_INDEX;
                }

            case HelpNavigator.KeywordIndex:
            case HelpNavigator.AssociateIndex:
                {
                    HH_AKLINK alink = new()
                    {
                        cbStruct = sizeof(HH_AKLINK),
                        fIndexOnFail = true,
                        fReserved = false
                    };
                    htmlParam = alink;
                    return command == HelpNavigator.KeywordIndex ? HH_KEYWORD_LOOKUP : HH_ALINK_LOOKUP;
                }

            default:
                return (HTML_HELP_COMMAND)command;
        }
    }
}
