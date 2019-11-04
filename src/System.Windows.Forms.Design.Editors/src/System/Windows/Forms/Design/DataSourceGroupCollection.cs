// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Design
{
    public class DataSourceGroupCollection : CollectionBase
    {
        /// <include file='doc\DataSourceGroupCollection.uex' path='docs/doc[@for="DataSourceGroupCollection.DataSourceGroupCollection"]/*' />
        /// <devdoc>
        /// </devdoc>
        public DataSourceGroupCollection() : base()
        {
        }

        /// <include file='doc\DataSourceGroupCollection.uex' path='docs/doc[@for="DataSourceGroupCollection.Add"]/*' />
        /// <devdoc>
        /// </devdoc>
        public int Add(DataSourceGroup value)
        {
            return List.Add(value);
        }

        /// <include file='doc\DataSourceGroupCollection.uex' path='docs/doc[@for="DataSourceGroupCollection.IndexOf"]/*' />
        /// <devdoc>
        /// </devdoc>
        public int IndexOf(DataSourceGroup value)
        {
            return List.IndexOf(value);
        }

        /// <include file='doc\DataSourceGroupCollection.uex' path='docs/doc[@for="DataSourceGroupCollection.Insert"]/*' />
        /// <devdoc>
        /// </devdoc>
        public void Insert(int index, DataSourceGroup value)
        {
            List.Insert(index, value);
        }

        /// <include file='doc\DataSourceGroupCollection.uex' path='docs/doc[@for="DataSourceGroupCollection.Contains"]/*' />
        /// <devdoc>
        /// </devdoc>
        public bool Contains(DataSourceGroup value)
        {
            return List.Contains(value);
        }

        /// <include file='doc\DataSourceGroupCollection.uex' path='docs/doc[@for="DataSourceGroupCollection.CopyTo"]/*' />
        /// <devdoc>
        /// </devdoc>
        public void CopyTo(DataSourceGroup[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <include file='doc\DataSourceGroupCollection.uex' path='docs/doc[@for="DataSourceGroupCollection.Remove"]/*' />
        /// <devdoc>
        /// </devdoc>
        public void Remove(DataSourceGroup value)
        {
            List.Remove(value);
        }

        /// <include file='doc\DataSourceGroupCollection.uex' path='docs/doc[@for="DataSourceGroupCollection.this"]/*' />
        /// <devdoc>
        /// </devdoc>
        public DataSourceGroup this[int index]
        {
            get
            {
                return (DataSourceGroup)List[index];
            }

            set
            {
                List[index] = value;
            }
        }
    }
}
