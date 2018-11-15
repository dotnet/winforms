// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System;
    using System.Security.Permissions;
    using System.Runtime.Serialization.Formatters;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Windows.Forms;
    



    /// <include file='doc\HScrollProperties.uex' path='docs/doc[@for="HScrollProperties"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Basic Properties for VScroll.
    ///    </para>
    /// </devdoc>
    public class HScrollProperties : ScrollProperties {

        /// <include file='doc\HScrollProperties.uex' path='docs/doc[@for="HScrollProperties.HScrollProperties"]/*' />
        public HScrollProperties(ScrollableControl container) : base(container) {
        }

        internal override int PageSize  {
            get {
                return ParentControl.ClientRectangle.Width;
            }
        }

        internal override int Orientation  {
            get  {
                return NativeMethods.SB_HORZ;
            }
        }

        internal override int HorizontalDisplayPosition  {
             get  {
                 return -this.value;
             }
        }

        internal override int VerticalDisplayPosition  {
             get  {
                  return ParentControl.DisplayRectangle.Y;
             }
        }        
    }
}
