// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a common dialog box that displays available colors along with
    ///  controls that allow the user to define custom colors.
    /// </summary>
    [DefaultProperty(nameof(Color))]
    [SRDescription(nameof(SR.DescriptionColorDialog))]
    // The only event this dialog has is HelpRequest, which isn't very useful
    public class ColorDialog : CommonDialog
    {
        private int options;
        private readonly int[] customColors;

        private Color color;

        /// <summary>
        ///  Initializes a new instance of the <see cref='ColorDialog'/>
        ///  class.
        /// </summary>
        public ColorDialog()
        {
            customColors = new int[16];
            Reset();
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the user can use the dialog box
        ///  to define custom colors.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(true),
            SRDescription(nameof(SR.CDallowFullOpenDescr))
        ]
        public virtual bool AllowFullOpen
        {
            get
            {
                return !GetOption(NativeMethods.CC_PREVENTFULLOPEN);
            }

            set
            {
                SetOption(NativeMethods.CC_PREVENTFULLOPEN, !value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box displays all available colors in
        ///  the set of basic colors.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(false),
            SRDescription(nameof(SR.CDanyColorDescr))
        ]
        public virtual bool AnyColor
        {
            get
            {
                return GetOption(NativeMethods.CC_ANYCOLOR);
            }

            set
            {
                SetOption(NativeMethods.CC_ANYCOLOR, value);
            }
        }

        /// <summary>
        ///  Gets or sets the color selected by the user.
        /// </summary>
        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.CDcolorDescr))
        ]
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (!value.IsEmpty)
                {
                    color = value;
                }
                else
                {
                    color = Color.Black;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the set of
        ///  custom colors shown in the dialog box.
        /// </summary>
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.CDcustomColorsDescr))
        ]
        public int[] CustomColors
        {
            get { return (int[])customColors.Clone(); }
            set
            {
                int length = value == null ? 0 : Math.Min(value.Length, 16);
                if (length > 0)
                {
                    Array.Copy(value, 0, customColors, 0, length);
                }

                for (int i = length; i < 16; i++)
                {
                    customColors[i] = 0x00FFFFFF;
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the controls used to create custom
        ///  colors are visible when the dialog box is opened
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(false),
            SRDescription(nameof(SR.CDfullOpenDescr))
        ]
        public virtual bool FullOpen
        {
            get
            {
                return GetOption(NativeMethods.CC_FULLOPEN);
            }

            set
            {
                SetOption(NativeMethods.CC_FULLOPEN, value);
            }
        }

        /// <summary>
        ///  Our HINSTANCE from Windows.
        /// </summary>
        protected virtual IntPtr Instance => Kernel32.GetModuleHandleW(null);

        /// <summary>
        ///  Returns our CHOOSECOLOR options.
        /// </summary>
        protected virtual int Options
        {
            get
            {
                return options;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether a Help button appears
        ///  in the color dialog box.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(false),
            SRDescription(nameof(SR.CDshowHelpDescr))
        ]
        public virtual bool ShowHelp
        {
            get
            {
                return GetOption(NativeMethods.CC_SHOWHELP);
            }
            set
            {
                SetOption(NativeMethods.CC_SHOWHELP, value);
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets a value indicating
        ///  whether the dialog
        ///  box will restrict users to selecting solid colors only.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(false),
            SRDescription(nameof(SR.CDsolidColorOnlyDescr))
        ]
        public virtual bool SolidColorOnly
        {
            get
            {
                return GetOption(NativeMethods.CC_SOLIDCOLOR);
            }
            set
            {
                SetOption(NativeMethods.CC_SOLIDCOLOR, value);
            }
        }

        /// <summary>
        ///  Lets us control the CHOOSECOLOR options.
        /// </summary>
        private bool GetOption(int option)
        {
            return (options & option) != 0;
        }

        /// <summary>
        ///  Resets
        ///  all options to their
        ///  default values, the last selected color to black, and the custom
        ///  colors to their default values.
        /// </summary>
        public override void Reset()
        {
            options = 0;
            color = Color.Black;
            CustomColors = null;
        }

        private void ResetColor()
        {
            Color = Color.Black;
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            NativeMethods.WndProc hookProcPtr = new NativeMethods.WndProc(HookProc);
            NativeMethods.CHOOSECOLOR cc = new NativeMethods.CHOOSECOLOR();
            IntPtr custColorPtr = Marshal.AllocCoTaskMem(64);
            try
            {
                Marshal.Copy(customColors, 0, custColorPtr, 16);
                cc.hwndOwner = hwndOwner;
                cc.hInstance = Instance;
                cc.rgbResult = ColorTranslator.ToWin32(color);
                cc.lpCustColors = custColorPtr;

                int flags = Options | (NativeMethods.CC_RGBINIT | NativeMethods.CC_ENABLEHOOK);
                // Our docs say AllowFullOpen takes precedence over FullOpen; ChooseColor implements the opposite
                if (!AllowFullOpen)
                {
                    flags &= ~NativeMethods.CC_FULLOPEN;
                }

                cc.Flags = flags;

                cc.lpfnHook = hookProcPtr;
                if (!SafeNativeMethods.ChooseColor(cc))
                {
                    return false;
                }

                if (cc.rgbResult != ColorTranslator.ToWin32(color))
                {
                    color = ColorTranslator.FromOle(cc.rgbResult);
                }

                Marshal.Copy(custColorPtr, customColors, 0, 16);
                return true;
            }
            finally
            {
                Marshal.FreeCoTaskMem(custColorPtr);
            }
        }

        /// <summary>
        ///  Allows us to manipulate the CHOOSECOLOR options
        /// </summary>
        private void SetOption(int option, bool value)
        {
            if (value)
            {
                options |= option;
            }
            else
            {
                options &= ~option;
            }
        }

        /// <summary>
        ///  Indicates whether the <see cref='Color'/> property should be
        ///  persisted.
        /// </summary>
        private bool ShouldSerializeColor()
        {
            return !Color.Equals(Color.Black);
        }

        /// <summary>
        ///  Provides a string version of this object.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return s + ",  Color: " + Color.ToString();
        }
    }
}
