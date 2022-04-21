// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms
{
    public partial class DataGridViewRowCollection
    {
        private class UnsharingRowEnumerator : IEnumerator
        {
            private readonly DataGridViewRowCollection owner;
            private int current;

            /// <summary>
            ///  Creates a new enumerator that will enumerate over the rows and unshare the accessed rows if needed.
            /// </summary>
            public UnsharingRowEnumerator(DataGridViewRowCollection owner)
            {
                this.owner = owner;
                current = -1;
            }

            /// <summary>
            ///  Moves to the next element, or returns false if at the end.
            /// </summary>
            bool IEnumerator.MoveNext()
            {
                if (current < owner.Count - 1)
                {
                    current++;
                    return true;
                }
                else
                {
                    current = owner.Count;
                    return false;
                }
            }

            /// <summary>
            ///  Resets the enumeration back to the beginning.
            /// </summary>
            void IEnumerator.Reset()
            {
                current = -1;
            }

            /// <summary>
            ///  Retrieves the current value in the enumerator.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (current == -1)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowCollection_EnumNotStarted);
                    }

                    if (current == owner.Count)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowCollection_EnumFinished);
                    }

                    return owner[current];
                }
            }
        }
    }
}
