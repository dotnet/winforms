// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Design
{
    public class DataSourceDescriptorCollection : CollectionBase
    {
        /// <include file='doc\DataSourceDescriptorCollection.uex' path='docs/doc[@for="DataSourceDescriptorCollection.DataSourceDescriptorCollection"]/*' />
        /// <devdoc>
        /// </devdoc>
        public DataSourceDescriptorCollection() : base()
        {
        }

        /// <include file='doc\DataSourceDescriptorCollection.uex' path='docs/doc[@for="DataSourceDescriptorCollection.Add"]/*' />
        /// <devdoc>
        /// </devdoc>
        public int Add(DataSourceDescriptor value)
        {
            return List.Add(value);
        }

        /// <include file='doc\DataSourceDescriptorCollection.uex' path='docs/doc[@for="DataSourceDescriptorCollection.IndexOf"]/*' />
        /// <devdoc>
        /// </devdoc>
        public int IndexOf(DataSourceDescriptor value)
        {
            return List.IndexOf(value);
        }

        /// <include file='doc\DataSourceDescriptorCollection.uex' path='docs/doc[@for="DataSourceDescriptorCollection.Insert"]/*' />
        /// <devdoc>
        /// </devdoc>
        public void Insert(int index, DataSourceDescriptor value)
        {
            List.Insert(index, value);
        }

        /// <include file='doc\DataSourceDescriptorCollection.uex' path='docs/doc[@for="DataSourceDescriptorCollection.Contains"]/*' />
        /// <devdoc>
        /// </devdoc>
        public bool Contains(DataSourceDescriptor value)
        {
            return List.Contains(value);
        }

        /// <include file='doc\DataSourceDescriptorCollection.uex' path='docs/doc[@for="DataSourceDescriptorCollection.CopyTo"]/*' />
        /// <devdoc>
        /// </devdoc>
        public void CopyTo(DataSourceDescriptor[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <include file='doc\DataSourceDescriptorCollection.uex' path='docs/doc[@for="DataSourceDescriptorCollection.Remove"]/*' />
        /// <devdoc>
        /// </devdoc>
        public void Remove(DataSourceDescriptor value)
        {
            List.Remove(value);
        }

        /// <include file='doc\DataSourceDescriptorCollection.uex' path='docs/doc[@for="DataSourceDescriptorCollection.this"]/*' />
        /// <devdoc>
        /// </devdoc>
        public DataSourceDescriptor this[int index]
        {
            get
            {
                return (DataSourceDescriptor)List[index];
            }

            set
            {
                List[index] = value;
            }
        }
    }
}
