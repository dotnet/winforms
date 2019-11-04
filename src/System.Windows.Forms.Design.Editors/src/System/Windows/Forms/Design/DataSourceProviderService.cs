// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    public abstract class DataSourceProviderService
    {
        /// <include file='doc\DataSourceProviderService.uex' path='docs/doc[@for="DataSourceProviderService.SupportsAddNewDataSource"]/*' />
        /// <devdoc>
        ///     Returns true if the service supports adding new data sources using InvokeAddNewDataSource().
        /// </devdoc>
        public abstract bool SupportsAddNewDataSource { get; }

        /// <include file='doc\DataSourceProviderService.uex' path='docs/doc[@for="DataSourceProviderService.SupportsConfigureDataSource"]/*' />
        /// <devdoc>
        ///     Returns true if the service supports configuring data sources using InvokeConfigureDataSource().
        /// </devdoc>
        public abstract bool SupportsConfigureDataSource { get; }

        /// <include file='doc\DataSourceProviderService.uex' path='docs/doc[@for="DataSourceProviderService.GetDataSources"]/*' />
        /// <devdoc>
        ///     Retrieves the collection of project level data sources.
        ///     If there are no project level data sources, returns null.
        /// </devdoc>
        public abstract DataSourceGroupCollection GetDataSources();

        /// <include file='doc\DataSourceProviderService.uex' path='docs/doc[@for="DataSourceProviderService.InvokeAddNewDataSource"]/*' />
        /// <devdoc>
        ///     Invokes the "Add New Data Source" wizard and returns a collection of
        ///     newly added data sources, or null if no data sources were added.
        ///
        ///     Note: This is a synchronous call.
        /// </devdoc>
        public abstract DataSourceGroup InvokeAddNewDataSource(IWin32Window parentWindow, FormStartPosition startPosition);

        /// <include file='doc\DataSourceProviderService.uex' path='docs/doc[@for="DataSourceProviderService.InvokeConfigureDataSource"]/*' />
        /// <devdoc>
        ///     Invokes the "Configure Data Source" dialog on the specified data source
        ///     and returns true if any changes were made to that data source. Throws
        ///     an ArgumentException if the specified data source is invalid or null.
        ///
        ///     Note: This is a synchronous call.
        /// </devdoc>
        public abstract bool InvokeConfigureDataSource(IWin32Window parentWindow, FormStartPosition startPosition, DataSourceDescriptor dataSourceDescriptor);

        /// <include file='doc\DataSourceProviderService.uex' path='docs/doc[@for="DataSourceProviderService.AddDataSourceInstance"]/*' />
        /// <devdoc>
        ///     Creates and returns an instance of the given data source, and adds it to the design surface.
        ///     Throws an ArgumentException if the type name cannot be created or resolved. This method should
        ///     only be called on data sources that are designable (ie. DataSourceDescriptor.IsDesignable is true).
        ///
        ///     This method allows the service implementer to perform custom actions when a data source
        ///     is added to the design surface.  For example, when adding a TypedDataSet to the design
        ///     surface, the service could generate code for loading and filling the data set.
        /// </devdoc>
        public abstract object AddDataSourceInstance(IDesignerHost host, DataSourceDescriptor dataSourceDescriptor);

        /// <include file='doc\DataSourceProviderService.uex' path='docs/doc[@for="DataSourceProviderService.NotifyDataSourceComponentAdded"]/*' />
        /// <devdoc>
        ///     Notifies the service that a component representing a data source was added to the design surface.
        ///     This gives the service the chance to configure that component (which may or may not be a data source
        ///     instance created by a prior call to AddDataSourceInstance).
        /// </devdoc>
        public abstract void NotifyDataSourceComponentAdded(object dsc);
    }
}
