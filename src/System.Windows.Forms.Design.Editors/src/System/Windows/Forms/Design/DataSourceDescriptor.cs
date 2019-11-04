// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design
{
    public abstract class DataSourceDescriptor
    {
        /// <include file='doc\DataSourceDescriptor.uex' path='docs/doc[@for="DataSourceDescriptor.Name"]/*' />
        /// <devdoc>
        ///     The name of the data source. Must be unique across all Project level data sources.
        /// </devdoc>
        public abstract string Name { get; }

        /// <include file='doc\DataSourceDescriptor.uex' path='docs/doc[@for="DataSourceDescriptor.Image"]/*' />
        /// <devdoc>
        ///     Image that represents the data source, typically used in design time pickers. May be null.
        /// </devdoc>
        public abstract Bitmap Image { get; }

        /// <include file='doc\DataSourceDescriptor.uex' path='docs/doc[@for="DataSourceDescriptor.TypeName"]/*' />
        /// <devdoc>
        ///     Fully qualified type name of the data source.
        /// </devdoc>
        public abstract string TypeName { get; }

        /// <include file='doc\DataSourceDescriptor.uex' path='docs/doc[@for="DataSourceDescriptor.IsDesignable"]/*' />
        /// <devdoc>
        ///     Indicates whether data source is designable, meaning that an instance of this type will
        ///     be added to the design surface when binding. Designable data sources are bound by
        ///     instance rather than by type. Data sources of type IComponent are designable.
        /// </devdoc>
        public abstract bool IsDesignable { get; }
    }
}
