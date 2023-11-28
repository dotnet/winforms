// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal class HelpInfo
{
    public const int HelpFileOption = 1;
    public const int HelpKeywordOption = 2;
    public const int HelpNavigatorOption = 3;
    public const int HelpObjectOption = 4;

    public HelpInfo(string helpfilepath)
    {
        HelpFilePath = helpfilepath;
        Keyword = string.Empty;
        Navigator = HelpNavigator.TableOfContents;
        Param = null;
        Option = HelpFileOption;
    }

    public HelpInfo(string helpfilepath, string keyword)
    {
        HelpFilePath = helpfilepath;
        Keyword = keyword;
        Navigator = HelpNavigator.TableOfContents;
        Param = null;
        Option = HelpKeywordOption;
    }

    public HelpInfo(string helpfilepath, HelpNavigator navigator)
    {
        HelpFilePath = helpfilepath;
        Keyword = string.Empty;
        Navigator = navigator;
        Param = null;
        Option = HelpNavigatorOption;
    }

    public HelpInfo(string helpfilepath, HelpNavigator navigator, object? param)
    {
        HelpFilePath = helpfilepath;
        Keyword = string.Empty;
        Navigator = navigator;
        Param = param;
        Option = HelpObjectOption;
    }

    public int Option { get; }

    public string HelpFilePath { get; }

    public string Keyword { get; }

    public HelpNavigator Navigator { get; }

    public object? Param { get; }

    public override string ToString()
    {
        return $"{{HelpFilePath={HelpFilePath}, keyword ={Keyword}, navigator={Navigator}}}";
    }
}
