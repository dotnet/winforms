// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the AutoCompleteSource for ComboBox and TextBox AutoComplete Feature.
    /// </devdoc>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Maps to native enum.")]
    public enum AutoCompleteSource
    {
        /// <devdoc>
        /// This option includes the file system as the source.
        /// </devdoc>
        FileSystem = 0x00000001,

        /// <devdoc>
        /// This option includes the URL's in the users history list.
        /// </devdoc>
        HistoryList = 0x00000002,

        /// <devdoc>
        /// This option includes the URL's in the users recently used list.
        /// </devdoc>
        RecentlyUsedList = 0x00000004,

        /// <devdoc>
        /// This option is equivalent to HistoryList | RecentlyUsedList.
        /// </devdoc>
        AllUrl =  HistoryList | RecentlyUsedList,

        /// <devdoc>
        /// This option is equivalent to FILESYSTEM | AllUrl. This is the default
        /// value when the AutoCompleteMode has been set to a non default value.
        /// </devdoc>
        AllSystemSources = FileSystem | AllUrl,

        /// <devdoc>
        /// This option is allows to autoComplete just directory names and not
        /// the files inside.
        /// </devdoc>
        FileSystemDirectories = 0x00000020,

        /// <devdoc>
        /// This option includes stirngs from a built in String Collection object.
        /// </devdoc>
        CustomSource = 0x00000040,

        /// <devdoc>
        /// The default value specifying the no AutoCompleteSource is currently
        /// in use.
        /// </devdoc>
        None = 0x00000080,

        /// <devdoc>
        /// The items of the combobox represent the source.
        /// </devdoc>
        ListItems = 0x00000100
    }
}
