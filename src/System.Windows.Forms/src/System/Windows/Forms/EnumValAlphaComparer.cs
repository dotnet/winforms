// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.Globalization;
 
    internal class EnumValAlphaComparer : IComparer {
        private CompareInfo m_compareInfo;
        internal static readonly EnumValAlphaComparer Default = new EnumValAlphaComparer();
        
        internal EnumValAlphaComparer() {
            m_compareInfo = CultureInfo.InvariantCulture.CompareInfo;
        }
  
        public int Compare(Object a, Object b) {
            return m_compareInfo.Compare(a.ToString(), b.ToString());
        }
    }
}

