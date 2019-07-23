// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    internal class HelpInfo
    {
        public HelpInfo(string helpfilepath)
        {
            HelpFilePath = helpfilepath;
            Keyword = string.Empty;
            Navigator = HelpNavigator.TableOfContents;
            Param = null;
            Option = NativeMethods.HLP_FILE;
        }

        public HelpInfo(string helpfilepath, string keyword)
        {
            HelpFilePath = helpfilepath;
            Keyword = keyword;
            Navigator = HelpNavigator.TableOfContents;
            Param = null;
            Option = NativeMethods.HLP_KEYWORD;
        }

        public HelpInfo(string helpfilepath, HelpNavigator navigator)
        {
            HelpFilePath = helpfilepath;
            Keyword = string.Empty;
            Navigator = navigator;
            Param = null;
            Option = NativeMethods.HLP_NAVIGATOR;
        }

        public HelpInfo(string helpfilepath, HelpNavigator navigator, object param)
        {
            HelpFilePath = helpfilepath;
            Keyword = string.Empty;
            Navigator = navigator;
            Param = param;
            Option = NativeMethods.HLP_OBJECT;
        }

        public int Option { get; }

        public string HelpFilePath { get; }

        public string Keyword { get; }

        public HelpNavigator Navigator { get; }

        public object Param { get; }

        public override string ToString()
        {
            return "{HelpFilePath=" + HelpFilePath + ", keyword =" + Keyword + ", navigator=" + Navigator.ToString() + "}";
        }
    }
}
