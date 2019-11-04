// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design
{
    public abstract class DataSourceGroup
    {
        /// <include file='doc\DataSourceGroup.uex' path='docs/doc[@for="DataSourceGroup.Name"]/*' />
        /// <devdoc>
        ///     The name of the group. Must be unique across all Project level data sources groups.
        /// </devdoc>
        public abstract string Name { get; }

        /// <include file='doc\DataSourceGroup.uex' path='docs/doc[@for="DataSourceGroup.Image"]/*' />
        /// <devdoc>
        ///     Image that represents the group, typically used in design time pickers. May be null.
        /// </devdoc>
        public abstract Bitmap Image { get; }

        /// <include file='doc\DataSourceGroup.uex' path='docs/doc[@for="DataSourceGroup.DataSources"]/*' />
        /// <devdoc>
        ///     Collection of descriptors for the data sources in this group.
        /// </devdoc>
        public abstract DataSourceDescriptorCollection DataSources { get; }

        /// <include file='doc\DataSourceGroup.uex' path='docs/doc[@for="DataSourceGroup.IsDesignable"]/*' />
        /// <devdoc>
        ///     Indicates whether this group is the default group for this Project (typically the group
        ///     that contains data sources from the Project's default application namespace).
        /// </devdoc>
        public abstract bool IsDefault { get; }
    }
}
