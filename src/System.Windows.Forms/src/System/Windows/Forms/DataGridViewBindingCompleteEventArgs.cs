// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.ComponentModel;

    public class DataGridViewBindingCompleteEventArgs : EventArgs
    {
        private ListChangedType listChangedType;

        public DataGridViewBindingCompleteEventArgs(ListChangedType listChangedType)
        {
            this.listChangedType = listChangedType;
        }

        public ListChangedType ListChangedType
        {
            get
            {
                return this.listChangedType;
            }
        }
    }
}
