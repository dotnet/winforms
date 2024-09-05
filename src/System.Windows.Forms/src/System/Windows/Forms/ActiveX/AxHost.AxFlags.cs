// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public abstract unsafe partial class AxHost
{
    /// <summary>
    ///  Flags which may be passed to the AxHost constructor
    /// </summary>
    internal static class AxFlags
    {
        /// <summary>
        ///  Indicates that the context menu for the control should not contain an
        ///  "Edit" verb unless the activeX controls itself decides to proffer it.
        ///  By default, all wrapped activeX controls will contain an edit verb.
        /// </summary>
        internal const int PreventEditMode = 0x1;

        /// <summary>
        ///  Indicated that the context menu for the control should contain
        ///  a "Properties..." verb which may be used to show the property
        ///  pages for the control. Note that even if this flag is
        ///  specified, the verb will not appear unless the control
        ///  proffers a set of property pages.
        ///  [Since most activeX controls already have their own properties verb
        ///  on the context menu, the default is not to include one specified by
        ///  this flag.]
        /// </summary>
        internal const int IncludePropertiesVerb = 0x2;

        /// <summary>
        /// </summary>
        internal const int IgnoreThreadModel = 0x10000000;
    }
}
