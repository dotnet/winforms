// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Collections;


    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='System.Windows.Forms.InputLanguage'/> objects.
    ///    </para>
    /// </summary>
    public class InputLanguageCollection : ReadOnlyCollectionBase
    {

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='System.Windows.Forms.InputLanguageCollection'/> containing any array of <see cref='System.Windows.Forms.InputLanguage'/> objects.
        ///    </para>
        /// </summary>
        internal InputLanguageCollection(InputLanguage[] value)
        {
            InnerList.AddRange(value);
        }

        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='System.Windows.Forms.InputLanguage'/>.</para>
        /// </summary>
        public InputLanguage this[int index]
        {
            get
            {
                return ((InputLanguage)(InnerList[index]));
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='System.Windows.Forms.InputLanguageCollection'/> contains the specified <see cref='System.Windows.Forms.InputLanguage'/>.</para>
        /// </summary>
        public bool Contains(InputLanguage value)
        {
            return InnerList.Contains(value);
        }

        /// <summary>
        /// <para>Copies the <see cref='System.Windows.Forms.InputLanguageCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        public void CopyTo(InputLanguage[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        /// <summary>
        ///    <para>Returns the index of a <see cref='System.Windows.Forms.InputLanguage'/> in 
        ///       the <see cref='System.Windows.Forms.InputLanguageCollection'/> .</para>
        /// </summary>
        public int IndexOf(InputLanguage value)
        {
            return InnerList.IndexOf(value);
        }
    }
}
