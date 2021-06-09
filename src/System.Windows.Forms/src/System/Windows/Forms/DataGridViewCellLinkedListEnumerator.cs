// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents an enumerator of elements in a <see cref='DataGridViewCellLinkedList'/>  linked list.
    /// </summary>
    internal class DataGridViewCellLinkedListEnumerator : IEnumerator
    {
        private readonly DataGridViewCellLinkedListElement headElement;
        private DataGridViewCellLinkedListElement current;
        private bool reset;

        public DataGridViewCellLinkedListEnumerator(DataGridViewCellLinkedListElement headElement)
        {
            this.headElement = headElement;
            reset = true;
        }

        object IEnumerator.Current
        {
            get
            {
                Debug.Assert(current is not null); // Since this is for internal use only.
                return current.DataGridViewCell;
            }
        }

        bool IEnumerator.MoveNext()
        {
            if (reset)
            {
                Debug.Assert(current is null);
                current = headElement;
                reset = false;
            }
            else
            {
                Debug.Assert(current is not null); // Since this is for internal use only.
                current = current.Next;
            }

            return (current is not null);
        }

        void IEnumerator.Reset()
        {
            reset = true;
            current = null;
        }
    }
}
