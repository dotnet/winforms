// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    
    /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the AutoCompleteSource for ComboBox and TextBox AutoComplete Feature.
    ///    </para>
    /// </devdoc>
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum AutoCompleteSource {

        /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource.FileSystem"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This option includes the file system as the source.
        ///    </para>
        /// </devdoc>
        FileSystem = 0x00000001,
        
        
        /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource.HistoryList"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This option includes the URL's in the users history list.
        ///    </para>
        /// </devdoc>
        HistoryList = 0x00000002,


        /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource.RecentlyUsedList"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This option includes the URL's in the users recently used list.
        ///    </para>
        /// </devdoc>
        RecentlyUsedList = 0x00000004,
        
        /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource.AllUrl"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This option is equivalent to HistoryList | RecentlyUsedList.
        ///    </para>
        /// </devdoc>
        AllUrl =  HistoryList | RecentlyUsedList,

        /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource.AllSystemSources"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This option is equivalent to FILESYSTEM | AllUrl. This is the default value
        ///       when the AutoCompleteMode has been set to a non default value.
        ///    </para>
        /// </devdoc>
        AllSystemSources = FileSystem | AllUrl,


        /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource.FileSystemDirectories"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This option is allows to autoComplete just directory names and not the files inside.
        ///    </para>
        /// </devdoc>
        FileSystemDirectories = 0x00000020,
        
        
        /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This option includes stirngs from a built in String Collection object.
        ///    </para>
        /// </devdoc>
        CustomSource = 0x00000040,

        /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The default value specifying the no AutoCompleteSource is currently in use.
        ///    </para>
        /// </devdoc>
        None = 0x00000080,

        /// <include file='doc\AutoCompleteSource.uex' path='docs/doc[@for="AutoCompleteSource"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The items of the combobox represent the source.
        ///    </para>
        /// </devdoc>
        ListItems = 0x00000100
    }
}
