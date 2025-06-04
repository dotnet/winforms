// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the AutoCompleteSource for ComboBox and TextBox AutoComplete Feature.
/// </summary>
public enum AutoCompleteSource
{
    /// <summary>
    ///  This option includes the file system as the source.
    /// </summary>
    FileSystem = 0x00000001,

    /// <summary>
    ///  This option includes the URL's in the users history list.
    /// </summary>
    HistoryList = 0x00000002,

    /// <summary>
    ///  This option includes the URL's in the users recently used list.
    /// </summary>
    RecentlyUsedList = 0x00000004,

    /// <summary>
    ///  This option is equivalent to HistoryList | RecentlyUsedList.
    /// </summary>
    AllUrl = HistoryList | RecentlyUsedList,

    /// <summary>
    ///  This option is equivalent to FILESYSTEM | AllUrl. This is the default
    ///  value when the AutoCompleteMode has been set to a non default value.
    /// </summary>
    AllSystemSources = FileSystem | AllUrl,

    /// <summary>
    ///  This option is allows to autoComplete just directory names and not
    ///  the files inside.
    /// </summary>
    FileSystemDirectories = 0x00000020,

    /// <summary>
    ///  This option includes strings from a built in String Collection object.
    /// </summary>
    CustomSource = 0x00000040,

    /// <summary>
    ///  The default value specifying the no AutoCompleteSource is currently
    ///  in use.
    /// </summary>
    None = 0x00000080,

    /// <summary>
    ///  The items of the ComboBox represent the source.
    /// </summary>
    ListItems = 0x00000100
}
