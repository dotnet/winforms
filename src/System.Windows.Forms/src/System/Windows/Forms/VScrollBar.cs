// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Runtime.InteropServices;

    /// <include file='doc\VScrollBar.uex' path='docs/doc[@for="VScrollBar"]/*' />
    /// <devdoc>
    ///    <para>Represents
    ///       a standard Windows vertical scroll bar.</para>
    /// </devdoc>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [SRDescription(nameof(SR.DescriptionVScrollBar))]
    public class VScrollBar : ScrollBar {

        private const int VERTICAL_SCROLLBAR_HEIGHT = 80;

        /// <include file='doc\VScrollBar.uex' path='docs/doc[@for="VScrollBar.CreateParams"]/*' />
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
                cp.Style |= NativeMethods.SBS_VERT;
                return cp;
            }
        }

        /// <include file='doc\VScrollBar.uex' path='docs/doc[@for="VScrollBar.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                if (DpiHelper.IsScalingRequirementMet) {
                    return new Size(SystemInformation.GetVerticalScrollBarWidthForDpi(this.deviceDpi), LogicalToDeviceUnits(VERTICAL_SCROLLBAR_HEIGHT));
                }
                else {
                    return new Size(SystemInformation.VerticalScrollBarWidth, VERTICAL_SCROLLBAR_HEIGHT);
                }
            }
        }

        /// <include file='doc\VScrollBar.uex' path='docs/doc[@for="VScrollBar.RightToLeft"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override RightToLeft RightToLeft {
            get {
                return RightToLeft.No;
            }
            set {
            }
        }
        /// <include file='doc\VScrollBar.uex' path='docs/doc[@for="VScrollBar.RightToLeftChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler RightToLeftChanged {
            add {
                base.RightToLeftChanged += value;
            }
            remove {
                base.RightToLeftChanged -= value;
            }
        }

    }
}
