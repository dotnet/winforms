// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Runtime.InteropServices;

    /// <include file='doc\HScrollBar.uex' path='docs/doc[@for="HScrollBar"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents
    ///       a standard Windows horizontal scroll bar.
    ///    </para>
    /// </devdoc>
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     SRDescription(nameof(SR.DescriptionHScrollBar))
    ]
    public class HScrollBar : ScrollBar {
        
        /// <include file='doc\HScrollBar.uex' path='docs/doc[@for="HScrollBar.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Returns the parameters needed to create the handle. Inheriting classes
        ///       can override this to provide extra functionality. They should not,
        ///       however, forget to call base.getCreateParams() first to get the struct
        ///       filled up with the basic info.
        ///    </para>
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.Style |= NativeMethods.SBS_HORZ;
                return cp;
            }
        }
        
        /// <include file='doc\HScrollBar.uex' path='docs/doc[@for="HScrollBar.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(80, SystemInformation.HorizontalScrollBarHeight);
            }
        }
    }
}
