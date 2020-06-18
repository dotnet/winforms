// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;

namespace System.Windows.Forms.Design
{
    internal class DataGridViewRowCollectionCodeDomSerializer : CollectionCodeDomSerializer
    {
        private static DataGridViewRowCollectionCodeDomSerializer s_defaultSerializer;

        // FxCop made me add this constructor
        private DataGridViewRowCollectionCodeDomSerializer() { }

        /// <summery>
        ///  Retrieves a default static instance of this serializer.
        /// </summery>
        internal static DataGridViewRowCollectionCodeDomSerializer DefaultSerializer
        {
            get
            {
                if (s_defaultSerializer is null)
                {
                    s_defaultSerializer = new DataGridViewRowCollectionCodeDomSerializer();
                }

                return s_defaultSerializer;
            }
        }

        /// <summery>
        ///  Serializes the given collection.  targetExpression will refer to the expression used to rever to the
        ///  collection, but it can be null.
        /// </summery>
        protected override object SerializeCollection(IDesignerSerializationManager manager, CodeExpression targetExpression, Type targetType, ICollection originalCollection, ICollection valuesToSerialize)
        {
#if DEBUG
            // some checks
            DataGridViewRowCollection rowCollection = originalCollection as DataGridViewRowCollection;

            if (rowCollection.Count > 0)
            {
                Debug.Assert(rowCollection.Count == 1, " we should have only the add new row");
                DataGridView dataGridView = rowCollection[0].DataGridView;
                Debug.Assert(dataGridView.AllowUserToAddRows, "we only have the add new row when the data grid view allows users to add rows");
            }
#endif // DEBUG

            // with the new dataGridView designer we don't serialize any rows any more.
            // the only purpose of this serializer is to block serialization of the DataGridView add new row.
            // which is accomplished by returning an empty codeStatementCollection;

            return new CodeStatementCollection();
        }
    }
}
