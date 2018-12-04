// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Drawing;

    using System.ComponentModel;
    using System.Windows.Forms;

    using Microsoft.Win32;
    using System.Security;
    using System.Security.Permissions;

    /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents a common dialog box that displays available colors along with
    ///       controls that allow the user to define custom colors.
    ///    </para>
    /// </devdoc>
    [DefaultProperty(nameof(Color))]
    [SRDescription(nameof(SR.DescriptionColorDialog))]
    // The only event this dialog has is HelpRequest, which isn't very useful
    public class ColorDialog : CommonDialog {

        private int options;
        private int[] customColors;

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.color"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private Color color;

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.ColorDialog"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ColorDialog'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // If the constructor does not call Reset
                                                                                                    // it would be a breaking change.
        ]
        public ColorDialog() {
            customColors = new int[16];
            Reset();
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.AllowFullOpen"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the user can use the dialog box
        ///       to define custom colors.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(true),
        SRDescription(nameof(SR.CDallowFullOpenDescr))
        ]
        public virtual bool AllowFullOpen {
            get {
                return !GetOption(NativeMethods.CC_PREVENTFULLOPEN);
            }

            set {
                SetOption(NativeMethods.CC_PREVENTFULLOPEN, !value);
            }
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.AnyColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the dialog box displays all available colors in
        ///       the set of basic colors.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(false),
        SRDescription(nameof(SR.CDanyColorDescr))
        ]
        public virtual bool AnyColor {
            get {
                return GetOption(NativeMethods.CC_ANYCOLOR);
            }

            set {
                SetOption(NativeMethods.CC_ANYCOLOR, value);
            }
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.Color"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the color selected by the user.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)), 
        SRDescription(nameof(SR.CDcolorDescr))
        ]
        public Color Color {
            get {
                return color;
            }
            set {
                if (!value.IsEmpty) {
                    color = value;
                }
                else {
                    color = Color.Black;
                }
            }
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.CustomColors"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the set of
        ///       custom colors shown in the dialog box.
        ///    </para>
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.CDcustomColorsDescr))
        ]
        public int[] CustomColors {
            get { return(int[]) customColors.Clone();}
            set {
                int length = value == null? 0: Math.Min(value.Length, 16);
                if (length > 0) Array.Copy(value, 0, customColors, 0, length);
                for (int i = length; i < 16; i++) customColors[i] = 0x00FFFFFF;
            }
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.FullOpen"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the controls used to create custom
        ///       colors are visible when the dialog box is opened
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)), 
        DefaultValue(false),
        SRDescription(nameof(SR.CDfullOpenDescr))
        ]
        public virtual bool FullOpen {
            get {
                return GetOption(NativeMethods.CC_FULLOPEN);
            }

            set {
                SetOption(NativeMethods.CC_FULLOPEN, value);
            }
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.Instance"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Our HINSTANCE from Windows.
        ///    </para>
        /// </devdoc>
        protected virtual IntPtr Instance {
            [
                SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
                SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
            ]
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            get { return UnsafeNativeMethods.GetModuleHandle(null);}
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.Options"]/*' />
        /// <devdoc>
        ///    Returns our CHOOSECOLOR options.
        /// </devdoc>
        /// <internalonly/>
        protected virtual int Options {
            get {
                return options;
            }
        }


        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.ShowHelp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether a Help button appears
        ///       in the color dialog box.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(false),
        SRDescription(nameof(SR.CDshowHelpDescr))
        ]
        public virtual bool ShowHelp {
            get {
                return GetOption(NativeMethods.CC_SHOWHELP);
            }
            set {
                SetOption(NativeMethods.CC_SHOWHELP, value);
            }
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.SolidColorOnly"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets a value indicating
        ///       whether the dialog
        ///       box will restrict users to selecting solid colors only.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(false),
        SRDescription(nameof(SR.CDsolidColorOnlyDescr))
        ]
        public virtual bool SolidColorOnly {
            get {
                return GetOption(NativeMethods.CC_SOLIDCOLOR);
            }
            set {
                SetOption(NativeMethods.CC_SOLIDCOLOR, value);
            }
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.GetOption"]/*' />
        /// <devdoc>
        ///     Lets us control the CHOOSECOLOR options.
        /// </devdoc>
        /// <internalonly/>
        private bool GetOption(int option) {
            return(options & option) != 0;
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.Reset"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Resets
        ///       all options to their
        ///       default values, the last selected color to black, and the custom
        ///       colors to their default values.
        ///    </para>
        /// </devdoc>
        public override void Reset() {
            options = 0;
            color = Color.Black;
            CustomColors = null;
        }

	private void ResetColor() {
		Color = Color.Black;
	}

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.RunDialog"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override bool RunDialog(IntPtr hwndOwner) {

            NativeMethods.WndProc hookProcPtr = new NativeMethods.WndProc(this.HookProc);
            NativeMethods.CHOOSECOLOR cc = new NativeMethods.CHOOSECOLOR();
            IntPtr custColorPtr = Marshal.AllocCoTaskMem(64);
            try {
                Marshal.Copy(customColors, 0, custColorPtr, 16);
                cc.hwndOwner = hwndOwner;
                cc.hInstance = Instance;
                cc.rgbResult = ColorTranslator.ToWin32(color);
                cc.lpCustColors = custColorPtr;

                int flags = Options | (NativeMethods.CC_RGBINIT | NativeMethods.CC_ENABLEHOOK);
                // Our docs say AllowFullOpen takes precedence over FullOpen; ChooseColor implements the opposite
                if (!AllowFullOpen)
                    flags &= ~NativeMethods.CC_FULLOPEN;
                cc.Flags = flags;

                cc.lpfnHook = hookProcPtr;
                if (!SafeNativeMethods.ChooseColor(cc)) return false;
                if (cc.rgbResult != ColorTranslator.ToWin32(color)) color = ColorTranslator.FromOle(cc.rgbResult);
                Marshal.Copy(custColorPtr, customColors, 0, 16);
                return true;
            }
            finally {
                Marshal.FreeCoTaskMem(custColorPtr);
            }
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.SetOption"]/*' />
        /// <devdoc>
        ///     Allows us to manipulate the CHOOSECOLOR options
        /// </devdoc>
        /// <internalonly/>
        private void SetOption(int option, bool value) {
            if (value) {
                options |= option;
            }
            else {
                options &= ~option;
            }
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.ShouldSerializeColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates whether the <see cref='System.Windows.Forms.ColorDialog.Color'/> property should be
        ///       persisted.
        ///    </para>
        /// </devdoc>
        private bool ShouldSerializeColor() {
            return !Color.Equals(Color.Black);
        }

        /// <include file='doc\ColorDialog.uex' path='docs/doc[@for="ColorDialog.ToString"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Provides a string version of this object.
        ///    </para>
        /// </devdoc>
        public override string ToString() {
            string s = base.ToString();
            return s + ",  Color: " + Color.ToString();
        }
    }
}
