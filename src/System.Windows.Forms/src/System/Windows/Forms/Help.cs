// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents the HTML 1.0 Help engine.
    /// </summary>
    public static class Help
    {
#if DEBUG
        internal static readonly TraceSwitch WindowsFormsHelpTrace = new TraceSwitch("WindowsFormsHelpTrace", "Debug help system");
#else
        internal static readonly TraceSwitch WindowsFormsHelpTrace;
#endif

        private const int HH_DISPLAY_TOPIC = 0x0000;
        private const int HH_DISPLAY_TOC = 0x0001;  // not currently implemented
        private const int HH_DISPLAY_INDEX = 0x0002;  // not currently implemented
        private const int HH_DISPLAY_SEARCH = 0x0003;  // not currently implemented
        private const int HH_KEYWORD_LOOKUP = 0x000D;
        private const int HH_DISPLAY_TEXT_POPUP = 0x000E;  // display string resource id or text in a popup window
        private const int HH_HELP_CONTEXT = 0x000F;  // display mapped numeric value in dwData
        private const int HH_ALINK_LOOKUP = 0x0013;  // ALink version of HH_KEYWORD_LOOKUP

        private const int HTML10HELP = 2;
        private const int HTMLFILE = 3;

        /// <summary>
        ///  Displays
        ///  the contents of the Help file at located at a specified Url.
        /// </summary>
        public static void ShowHelp(Control parent, string url)
        {
            ShowHelp(parent, url, HelpNavigator.TableOfContents, null);
        }

        /// <summary>
        ///  Displays the contents of
        ///  the Help
        ///  file for a specific topic found at the specified Url.
        /// </summary>
        public static void ShowHelp(Control parent, string url, HelpNavigator navigator)
        {
            ShowHelp(parent, url, navigator, null);
        }

        /// <summary>
        ///  Displays the contents of
        ///  the Help
        ///  file for a specific topic found at the specified Url.
        /// </summary>
        public static void ShowHelp(Control parent, string url, string keyword)
        {
            if (keyword != null && keyword.Length != 0)
            {
                ShowHelp(parent, url, HelpNavigator.Topic, keyword);
            }
            else
            {
                ShowHelp(parent, url, HelpNavigator.TableOfContents, null);
            }
        }

        /// <summary>
        ///  Displays the contents of the Help file located at
        ///  the Url
        ///  supplied by the
        ///  user.
        /// </summary>
        public static void ShowHelp(Control parent, string url, HelpNavigator command, object parameter)
        {
            Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "Help:: ShowHelp");

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
        public static void ShowHelpIndex(Control parent, string url)
        {
            Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "Help:: ShowHelpIndex");

            ShowHelp(parent, url, HelpNavigator.Index, null);
        }

        /// <summary>
        ///  Displays a Help pop-up window.
        /// </summary>
        public static void ShowPopup(Control parent, string caption, Point location)
        {
            Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "Help:: ShowPopup");

            NativeMethods.HH_POPUP pop = new NativeMethods.HH_POPUP();

            // We have to marshal the string ourselves to prevent access violations.
            IntPtr pszText = Marshal.StringToCoTaskMemAuto(caption);

            try
            {
                pop.pszText = pszText;
                pop.idString = 0;
                pop.pt = location;

                // Looks like a windows

                pop.clrBackground = Color.FromKnownColor(KnownColor.Window).ToArgb() & 0x00ffffff;

                ShowHTML10Help(parent, null, HelpNavigator.Topic, pop);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pszText);
            }
        }

        /// <summary>
        ///  Displays HTML 1.0 Help with the specified parameters
        /// </summary>
        private static void ShowHTML10Help(Control parent, string url, HelpNavigator command, object param)
        {
            Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "Help:: ShowHTML10Help:: " + url + ", " + command.ToString("G") + ", " + param);

            // See if we can get a full path and file name and if that will
            // resolve the out of memory condition with file names that include spaces.
            // If we can't, though, we can't assume that the path's no good: it might be in
            // the Windows help directory.
            Uri file = null;
            string pathAndFileName = url; //This is our best guess at the path yet.

            file = Resolve(url);
            if (file != null)
            { // Can't assume we have a good url
                pathAndFileName = file.AbsoluteUri;
            }
            if (file is null || file.IsFile)
            {
                StringBuilder newPath = new StringBuilder();
                string localPath = (file != null && file.IsFile) ? file.LocalPath : url;

                // If this is a local path, convert it to a short path name.  Pass 0 as the length the first time
                uint requiredStringSize = UnsafeNativeMethods.GetShortPathName(localPath, newPath, 0);
                if (requiredStringSize > 0)
                {
                    //It's able to make it a short path.  Happy day.
                    newPath.Capacity = (int)requiredStringSize;
                    requiredStringSize = UnsafeNativeMethods.GetShortPathName(localPath, newPath, requiredStringSize);
                    //If it can't make it a  short path, just leave the path we had.
                    pathAndFileName = newPath.ToString();
                }
            }

            HandleRef handle;
            if (parent != null)
            {
                handle = new HandleRef(parent, parent.Handle);
            }
            else
            {
                handle = new HandleRef(null, User32.GetActiveWindow());
            }

            object htmlParam;
            if (param is string stringParam)
            {
                int htmlCommand = MapCommandToHTMLCommand(command, stringParam, out htmlParam);

                if (htmlParam is string stringHtmlParam)
                {
                    SafeNativeMethods.HtmlHelp(handle, pathAndFileName, htmlCommand, stringHtmlParam);
                }
                else if (htmlParam is int)
                {
                    SafeNativeMethods.HtmlHelp(handle, pathAndFileName, htmlCommand, (int)htmlParam);
                }
                else if (htmlParam is NativeMethods.HH_FTS_QUERY)
                {
                    SafeNativeMethods.HtmlHelp(handle, pathAndFileName, htmlCommand, (NativeMethods.HH_FTS_QUERY)htmlParam);
                }
                else if (htmlParam is NativeMethods.HH_AKLINK)
                {
                    // According to MSDN documentation, we have to ensure that the help window is up
                    // before we call ALINK lookup.
                    //
                    SafeNativeMethods.HtmlHelp(NativeMethods.NullHandleRef, pathAndFileName, HH_DISPLAY_TOPIC, (string)null);
                    SafeNativeMethods.HtmlHelp(handle, pathAndFileName, htmlCommand, (NativeMethods.HH_AKLINK)htmlParam);
                }
                else
                {
                    Debug.Fail("Cannot handle HTML parameter of type: " + htmlParam.GetType());
                    SafeNativeMethods.HtmlHelp(handle, pathAndFileName, htmlCommand, (string)param);
                }
            }
            else if (param is null)
            {
                SafeNativeMethods.HtmlHelp(handle, pathAndFileName, MapCommandToHTMLCommand(command, null, out htmlParam), 0);
            }
            else if (param is NativeMethods.HH_POPUP)
            {
                SafeNativeMethods.HtmlHelp(handle, pathAndFileName, HH_DISPLAY_TEXT_POPUP, (NativeMethods.HH_POPUP)param);
            }
            else if (param.GetType() == typeof(int))
            {
                throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(param), "Integer"), nameof(param));
            }
        }

        /// <summary>
        ///  Displays HTMLFile with the specified parameters
        /// </summary>
        private static void ShowHTMLFile(Control parent, string url, HelpNavigator command, object param)
        {
            Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "Help:: ShowHTMLHelp:: " + url + ", " + command.ToString("G") + ", " + param);

            Uri file = Resolve(url);

            if (file is null)
            {
                throw new ArgumentException(string.Format(SR.HelpInvalidURL, url), "url");
            }

            switch (command)
            {
                case HelpNavigator.TableOfContents:
                case HelpNavigator.Find:
                case HelpNavigator.Index:
                    // nothing needed...
                    //
                    break;
                case HelpNavigator.Topic:
                    if (param != null && param is string)
                    {
                        file = new Uri(file.ToString() + "#" + (string)param);
                    }
                    break;
            }

            HandleRef handle;
            if (parent != null)
            {
                handle = new HandleRef(parent, parent.Handle);
            }
            else
            {
                handle = new HandleRef(null, User32.GetActiveWindow());
            }

            Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "\tExecuting '" + file.ToString() + "'");
            Shell32.ShellExecuteW(handle, null, file.ToString(), null, null, User32.SW.NORMAL);
        }

        private static Uri Resolve(string partialUri)
        {
            Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "Help:: Resolve " + partialUri);
            Debug.Indent();

            Uri file = null;

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

            if (file != null && file.Scheme == "file")
            {
                string localPath = NativeMethods.GetLocalPath(partialUri);
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "file, check for existence");

                if (!File.Exists(localPath))
                {
                    // clear, and try relative to AppBase...
                    //
                    file = null;
                }
            }

            if (file is null)
            {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "try appbase relative");
                try
                {
                    // try relative to AppBase...
                    //
                    file = new Uri(new Uri(AppContext.BaseDirectory),
                                   partialUri);
                }
                catch (UriFormatException)
                {
                    // Ignore invalid uris.
                }

                if (file != null && file.Scheme == "file")
                {
                    string localPath = file.LocalPath + file.Fragment;
                    Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "file, check for existence");
                    if (!File.Exists(localPath))
                    {
                        // clear - file isn't there...
                        //
                        file = null;
                    }
                }
            }

            Debug.Unindent();
            return file;
        }

        private static int GetHelpFileType(string url)
        {
            Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "Help:: GetHelpFileType " + url);

            if (url is null)
            {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "\tnull, must be Html File");
                return HTMLFILE;
            }

            Uri file = Resolve(url);

            if (file is null || file.Scheme == "file")
            {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "\tfile");

                string ext = Path.GetExtension(file is null ? url : file.LocalPath + file.Fragment).ToLower(CultureInfo.InvariantCulture);
                if (ext == ".chm" || ext == ".col")
                {
                    Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "\tchm or col, HtmlHelp 1.0 file");
                    return HTML10HELP;
                }
            }

            Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "\tnot file, or odd extension, but be HTML");
            return HTMLFILE;
        }

        /// <summary>
        ///  Maps one of the COMMAND_* constants to the HTML 1.0 Help equivalent.
        /// </summary>
        private static int MapCommandToHTMLCommand(HelpNavigator command, string param, out object htmlParam)
        {
            htmlParam = param;

            if ((string.IsNullOrEmpty(param)) &&
                (command == HelpNavigator.AssociateIndex || command == HelpNavigator.KeywordIndex))
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
                        NativeMethods.HH_FTS_QUERY ftsQuery = new NativeMethods.HH_FTS_QUERY
                        {
                            pszSearchQuery = param
                        };
                        htmlParam = ftsQuery;
                        return HH_DISPLAY_SEARCH;
                    }
                case HelpNavigator.TopicId:
                    {
                        try
                        {
                            htmlParam = int.Parse(param, CultureInfo.InvariantCulture);
                            return HH_HELP_CONTEXT;
                        }
                        catch
                        {
                            // default to just showing the index
                            return HH_DISPLAY_INDEX;
                        }
                    }
                case HelpNavigator.KeywordIndex:
                case HelpNavigator.AssociateIndex:
                    {
                        NativeMethods.HH_AKLINK alink = new NativeMethods.HH_AKLINK
                        {
                            pszKeywords = param,
                            fIndexOnFail = true,
                            fReserved = false
                        };
                        htmlParam = alink;
                        return (command == HelpNavigator.KeywordIndex) ? HH_KEYWORD_LOOKUP : HH_ALINK_LOOKUP;
                    }

                default:
                    return (int)command;
            }
        }
    }
}
