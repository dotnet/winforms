// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using static Interop;

namespace System.Windows.Forms
{
    internal sealed partial class WindowsFormsUtils
    {
        public const ContentAlignment AnyRightAlign = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;
        public const ContentAlignment AnyLeftAlign = ContentAlignment.TopLeft | ContentAlignment.MiddleLeft | ContentAlignment.BottomLeft;
        public const ContentAlignment AnyTopAlign = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
        public const ContentAlignment AnyBottomAlign = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
        public const ContentAlignment AnyMiddleAlign = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;
        public const ContentAlignment AnyCenterAlign = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;

        /// <summary>
        ///  The GetMessagePos function retrieves the cursor position for the last message
        ///  retrieved by the GetMessage function.
        /// </summary>
        public static Point LastCursorPoint
        {
            get
            {
                int lastXY = (int)User32.GetMessagePos();
                return new Point(PARAM.SignedLOWORD(lastXY), PARAM.SignedHIWORD(lastXY));
            }
        }

        /// <summary>
        ///  If you want to know if a piece of text contains one and only one &amp;
        ///  this is your function. If you have a character "t" and want match it to &amp;Text
        ///  Control.IsMnemonic is a better bet.
        /// </summary>
        public static bool ContainsMnemonic(string text)
        {
            if (text != null)
            {
                int textLength = text.Length;
                int firstAmpersand = text.IndexOf('&', 0);
                if (firstAmpersand >= 0 && firstAmpersand <= /*second to last char=*/textLength - 2)
                {
                    // we found one ampersand and it's either the first character
                    // or the second to last character
                    // or a character in between

                    // We're so close!  make sure we don't have a double ampersand now.
                    int secondAmpersand = text.IndexOf('&', firstAmpersand + 1);
                    if (secondAmpersand == -1)
                    {
                        // didn't find a second one in the string.
                        return true;
                    }
                }
            }
            return false;
        }

        internal static Rectangle ConstrainToScreenWorkingAreaBounds(Rectangle bounds)
        {
            return ConstrainToBounds(Screen.GetWorkingArea(bounds), bounds);
        }

        /// <summary>
        ///  Given a rectangle, constrain it to fit onto the current screen.
        /// </summary>
        internal static Rectangle ConstrainToScreenBounds(Rectangle bounds)
        {
            return ConstrainToBounds(Screen.FromRectangle(bounds).Bounds, bounds);
        }

        internal static Rectangle ConstrainToBounds(Rectangle constrainingBounds, Rectangle bounds)
        {
            // use screen instead of SystemInformation.WorkingArea for better multimon support.
            if (!constrainingBounds.Contains(bounds))
            {
                // make sure size does not exceed working area.
                bounds.Size = new Size(Math.Min(constrainingBounds.Width - 2, bounds.Width),
                                       Math.Min(constrainingBounds.Height - 2, bounds.Height));

                // X calculations
                //
                // scooch so it will fit on the screen.
                if (bounds.Right > constrainingBounds.Right)
                {
                    // its too far to the right.
                    bounds.X = constrainingBounds.Right - bounds.Width;
                }
                else if (bounds.Left < constrainingBounds.Left)
                {
                    // its too far to the left.
                    bounds.X = constrainingBounds.Left;
                }

                // Y calculations
                //
                // scooch so it will fit on the screen.
                if (bounds.Bottom > constrainingBounds.Bottom)
                {
                    // its too far to the bottom.
                    bounds.Y = constrainingBounds.Bottom - 1 - bounds.Height;
                }
                else if (bounds.Top < constrainingBounds.Top)
                {
                    // its too far to the top.
                    bounds.Y = constrainingBounds.Top;
                }
            }
            return bounds;
        }

        /// <summary>
        ///  Adds an extra &amp; to to the text so that "Fish &amp; Chips" can be displayed on a menu item
        ///  without underlining anything.
        ///  Fish &amp; Chips --> Fish &amp;&amp; Chips
        /// </summary>
        internal static string EscapeTextWithAmpersands(string text)
        {
            if (text is null)
            {
                return null;
            }

            int index = text.IndexOf('&');
            if (index == -1)
            {
                return text;
            }

            StringBuilder str = new StringBuilder(text.Substring(0, index));
            for (; index < text.Length; ++index)
            {
                if (text[index] == '&')
                {
                    str.Append('&');
                }
                if (index < text.Length)
                {
                    str.Append(text[index]);
                }
            }
            return str.ToString();
        }

        /// <summary>
        ///  helper function for generating information about a particular control
        ///  use AssertControlInformation if sticking in an assert - then the work
        ///  to figure out the control info will only be done when the assertion is false.
        /// </summary>
        internal static string GetControlInformation(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                return "Handle is IntPtr.Zero";
            }

#if DEBUG
            string windowText = User32.GetWindowText(hwnd);
            string typeOfControl = "Unknown";
            string nameOfControl = "Name: ";
            Control c = Control.FromHandle(hwnd);
            if (c != null)
            {
                typeOfControl = c.GetType().ToString();
                if (!string.IsNullOrEmpty(c.Name))
                {
                    nameOfControl += c.Name;
                }
                else
                {
                    nameOfControl += "Unknown";

                    // Add some extra debug info for ToolStripDropDowns.
                    if (c is ToolStripDropDown dd && dd.OwnerItem != null)
                    {
                        nameOfControl += Environment.NewLine + "\tOwnerItem: " + dd.OwnerItem.ToString();
                    }
                }
            }
            return windowText + Environment.NewLine + "\tType: " + typeOfControl + Environment.NewLine + "\t" + nameOfControl + Environment.NewLine;
#else
            return string.Empty;
#endif
        }

        internal static string AssertControlInformation(bool condition, Control control)
        {
            if (condition)
            {
                return string.Empty;
            }
            else
            {
                return GetControlInformation(control.Handle);
            }
        }

        /// <summary>
        ///  Retrieves the mnemonic from a given string, or zero if no mnemonic.
        ///  As used by the Control.Mnemonic to get mnemonic from Control.Text.
        /// </summary>
        public static char GetMnemonic(string text, bool convertToUpperCase)
        {
            char mnemonic = '\0';
            if (text != null)
            {
                int len = text.Length;
                for (int i = 0; i < len - 1; i++)
                {
                    if (text[i] == '&')
                    {
                        if (text[i + 1] == '&')
                        {
                            // we have an escaped &, so we need to skip it.
                            i++;
                            continue;
                        }

                        if (convertToUpperCase)
                        {
                            mnemonic = char.ToUpper(text[i + 1], CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            mnemonic = char.ToLower(text[i + 1], CultureInfo.CurrentCulture);
                        }
                        break;
                    }
                }
            }
            return mnemonic;
        }

        /// <summary>
        ///  Strips all keyboard mnemonic prefixes from a given string, eg. turning "He&amp;lp" into "Help".
        /// </summary>
        /// <remarks>
        ///  Note: Be careful not to call this multiple times on the same string, otherwise you'll turn
        ///  something like "Fi&amp;sh &amp;&amp; Chips" into "Fish &amp; Chips" on the first call, and then "Fish Chips"
        ///  on the second call.
        /// </remarks>
        public static string TextWithoutMnemonics(string text)
        {
            if (text is null)
            {
                return null;
            }

            int index = text.IndexOf('&');
            if (index == -1)
            {
                return text;
            }

            StringBuilder str = new StringBuilder(text.Substring(0, index));
            for (; index < text.Length; ++index)
            {
                if (text[index] == '&')
                {
                    // Skip this & and copy the next character instead
                    index++;
                }

                if (index < text.Length)
                {
                    str.Append(text[index]);
                }
            }

            return str.ToString();
        }

        /// <summary>
        ///  Translates a point from one control's coordinate system to the other
        ///  same as:
        ///  controlTo.PointToClient(controlFrom.PointToScreen(point))
        ///  but slightly more performant.
        /// </summary>
        public static Point TranslatePoint(Point point, Control fromControl, Control toControl)
        {
            User32.MapWindowPoints(new HandleRef(fromControl, fromControl.Handle), new HandleRef(toControl, toControl.Handle), ref point, 1);
            return point;
        }

        /// <summary>
        ///  Compares the strings using invariant culture for Turkish-I support. Returns true if they match.
        ///
        ///  If your strings are symbolic (returned from APIs, not from user) the following calls
        ///  are faster than this method:
        ///
        ///  String.Equals(s1, s2, StringComparison.Ordinal)
        ///  String.Equals(s1, s2, StringComparison.OrdinalIgnoreCase)
        /// </summary>
        public static bool SafeCompareStrings(string string1, string string2, bool ignoreCase)
        {
            if ((string1 is null) || (string2 is null))
            {
                // if either key is null, we should return false
                return false;
            }

            // Because String.Compare returns an ordering, it can not terminate early if lengths are not the same.
            // Also, equivalent characters can be encoded in different byte sequences, so it can not necessarily
            // terminate on the first byte which doesn't match. Hence this optimization.
            if (string1.Length != string2.Length)
            {
                return false;
            }

            return string.Compare(string1, string2, ignoreCase, CultureInfo.InvariantCulture) == 0;
        }

        public static string GetComponentName(IComponent component, string defaultNameValue)
        {
            Debug.Assert(component != null, "component passed here cannot be null");
            if (string.IsNullOrEmpty(defaultNameValue))
            {
                return component.Site?.Name ?? string.Empty;
            }
            else
            {
                return defaultNameValue;
            }
        }

        public static class EnumValidator
        {
            /// <summary>
            ///  Valid values are 0x001,0x002,0x004, 0x010,0x020,0x040, 0x100, 0x200,0x400
            ///  Method for verifying
            ///  Verify that the number passed in has only one bit on
            ///  Verify that the bit that is on is a valid bit by bitwise anding it to a mask.
            /// </summary>
            public static bool IsValidContentAlignment(ContentAlignment contentAlign)
            {
                if (BitOperations.PopCount((uint)contentAlign) != 1)
                {
                    return false;
                }

                // to calculate:
                // foreach (int val in Enum.GetValues(typeof(ContentAlignment))) { mask |= val; }
                int contentAlignmentMask = 0x777;
                return ((contentAlignmentMask & (int)contentAlign) != 0);
            }

            /// <summary>
            ///  shifts off the number of bits specified by numBitsToShift
            ///  -  makes sure the bits we've shifted off are just zeros
            ///  -  then compares if the resulting value is between minValAfterShift and maxValAfterShift
            ///
            ///  EXAMPLE:
            ///  MessageBoxIcon. Valid values are 0x0, 0x10, 0x20, 0x30, 0x40
            ///  Method for verifying: chop off the last 0 by shifting right 4 bits, verify resulting number is between 0 &amp; 4.
            ///
            ///  WindowsFormsUtils.EnumValidator.IsEnumWithinShiftedRange(icon, /*numBitsToShift*/4, /*min*/0x0,/*max*/0x4)
            /// </summary>
            public static bool IsEnumWithinShiftedRange(Enum enumValue, int numBitsToShift, int minValAfterShift, int maxValAfterShift)
            {
                int iValue = Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);
                int remainder = iValue >> numBitsToShift;
                if (remainder << numBitsToShift != iValue)
                {
                    // there were bits that we shifted out.
                    return false;
                }
                return (remainder >= minValAfterShift && remainder <= maxValAfterShift);
            }
        }
    }
}
