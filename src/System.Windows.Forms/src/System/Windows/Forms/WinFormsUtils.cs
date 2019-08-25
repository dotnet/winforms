// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.Internal;
using static Interop;


namespace System.Windows.Forms
{
    internal sealed class WindowsFormsUtils
    {
        public static readonly ContentAlignment AnyRightAlign = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;
        public static readonly ContentAlignment AnyLeftAlign = ContentAlignment.TopLeft | ContentAlignment.MiddleLeft | ContentAlignment.BottomLeft;
        public static readonly ContentAlignment AnyTopAlign = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
        public static readonly ContentAlignment AnyBottomAlign = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
        public static readonly ContentAlignment AnyMiddleAlign = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;
        public static readonly ContentAlignment AnyCenterAlign = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;

        /// <summary>
        ///  The GetMessagePos function retrieves the cursor position for the last message
        ///  retrieved by the GetMessage function.
        /// </summary>
        public static Point LastCursorPoint
        {
            get
            {
                int lastXY = SafeNativeMethods.GetMessagePos();
                return new Point(NativeMethods.Util.SignedLOWORD(lastXY), NativeMethods.Util.SignedHIWORD(lastXY));
            }
        }

        /// <remarks>
        ///  this graphics requires disposal.
        /// </remarks>
        public static Graphics CreateMeasurementGraphics()
        {
            return Graphics.FromHdcInternal(WindowsGraphicsCacheManager.MeasurementGraphics.DeviceContext.Hdc);
        }

        /// <summary>
        ///  If you want to know if a piece of text contains one and only one &
        ///  this is your function. If you have a character "t" and want match it to &Text
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
        ///  Adds an extra & to to the text so that "Fish & Chips" can be displayed on a menu item
        ///  without underlining anything.
        ///  Fish & Chips --> Fish && Chips
        /// </summary>
        internal static string EscapeTextWithAmpersands(string text)
        {
            if (text == null)
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
        /// <summary>
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
        ///  Finds the top level handle for a given handle.
        /// </summary>
        public static HandleRef GetRootHWnd(HandleRef hwnd)
        {
            IntPtr rootHwnd = UnsafeNativeMethods.GetAncestor(new HandleRef(hwnd, hwnd.Handle), NativeMethods.GA_ROOT);
            return new HandleRef(hwnd.Wrapper, rootHwnd);
        }

        /// <summary>
        ///  Finds the top level handle for a given handle.
        /// </summary>
        public static HandleRef GetRootHWnd(Control control)
        {
            return GetRootHWnd(new HandleRef(control, control.Handle));
        }

        /// <summary>
        ///  Strips all keyboard mnemonic prefixes from a given string, eg. turning "He&lp" into "Help".
        /// </summary>
        /// <remarks>
        ///  Note: Be careful not to call this multiple times on the same string, otherwise you'll turn
        ///  something like "Fi&sh && Chips" into "Fish & Chips" on the first call, and then "Fish Chips"
        ///  on the second call.
        /// </remarks>
        public static string TextWithoutMnemonics(string text)
        {
            if (text == null)
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
            UnsafeNativeMethods.MapWindowPoints(new HandleRef(fromControl, fromControl.Handle), new HandleRef(toControl, toControl.Handle), ref point, 1);
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
            if ((string1 == null) || (string2 == null))
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
                if (ClientUtils.GetBitCount((uint)contentAlign) != 1)
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
            ///  Method for verifying: chop off the last 0 by shifting right 4 bits, verify resulting number is between 0 & 4.
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

        public class ArraySubsetEnumerator : IEnumerator
        {
            private readonly object[] _array;
            private readonly int _total;
            private int _current;

            public ArraySubsetEnumerator(object[] array, int count)
            {
                Debug.Assert(count == 0 || array != null, "if array is null, count should be 0");
                Debug.Assert(array == null || count <= array.Length, "Trying to enumerate more than the array contains");
                _array = array;
                _total = count;
                _current = -1;
            }

            public bool MoveNext()
            {
                if (_current < _total - 1)
                {
                    _current++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _current = -1;
            }

            public object Current => _current == -1 ? null : _array[_current];
        }

        /// <summary>
        ///  This is a ControlCollection which can be made readonly. In readonly mode, this
        ///  ControlCollection throws NotSupportedExceptions for any operation that attempts
        ///  to modify the collection.
        /// </summary>
        internal class ReadOnlyControlCollection : Control.ControlCollection
        {
            private readonly bool _isReadOnly;

            public ReadOnlyControlCollection(Control owner, bool isReadOnly) : base(owner)
            {
                _isReadOnly = isReadOnly;
            }

            public override void Add(Control value)
            {
                if (IsReadOnly)
                {
                    throw new NotSupportedException(SR.ReadonlyControlsCollection);
                }

                AddInternal(value);
            }

            internal virtual void AddInternal(Control value) => base.Add(value);

            public override void Clear()
            {
                if (IsReadOnly)
                {
                    throw new NotSupportedException(SR.ReadonlyControlsCollection);
                }

                base.Clear();
            }

            internal virtual void RemoveInternal(Control value) => base.Remove(value);

            public override void RemoveByKey(string key)
            {
                if (IsReadOnly)
                {
                    throw new NotSupportedException(SR.ReadonlyControlsCollection);
                }

                base.RemoveByKey(key);
            }

            public override bool IsReadOnly => _isReadOnly;
        }

        /// <summary>
        ///  This control collection only allows a specific type of control
        ///  into the controls collection. It optionally supports readonlyness.
        /// </summary>
        internal class TypedControlCollection : ReadOnlyControlCollection
        {
            private readonly Type _typeOfControl;
            private readonly Control _ownerControl;

            public TypedControlCollection(Control owner, Type typeOfControl, bool isReadOnly) : base(owner, isReadOnly)
            {
                _typeOfControl = typeOfControl;
                _ownerControl = owner;
            }

            public TypedControlCollection(Control owner, Type typeOfControl) : base(owner, /*isReadOnly*/false)
            {
                _typeOfControl = typeOfControl;
                _ownerControl = owner;
            }

            public override void Add(Control value)
            {
                // Check parenting first for consistency
                Control.CheckParentingCycle(_ownerControl, value);

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (IsReadOnly)
                {
                    throw new NotSupportedException(SR.ReadonlyControlsCollection);
                }
                if (!_typeOfControl.IsAssignableFrom(value.GetType()))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, string.Format(SR.TypedControlCollectionShouldBeOfType, _typeOfControl.Name)), value.GetType().Name);
                }

                base.Add(value);
            }
        }

        /// <summary>
        ///  DCMapping is used to change the mapping and clip region of the
        ///  the specified device context to the given bounds. When the
        ///  DCMapping is disposed, the original mapping and clip rectangle
        ///  are restored.
        ///
        ///  Example:
        ///
        ///  using(WindowsFormsUtils.DCMapping mapping = new WindowsFormsUtils.DCMapping(hDC, new Rectangle(10,10, 50, 50) {
        ///  // inside here the hDC's mapping of (0,0) is inset by (10,10) and
        ///  // all painting is clipped at (0,0) - (50,50)
        ///  }
        ///
        ///  To use with GDI+ you can get the hDC from the Graphics object. You'd want to do this in a situation where
        ///  you're handing off a graphics object to someone, and you want the world translated some amount X,Y. This
        ///  works better than g.TranslateTransform(x,y) - as if someone calls g.GetHdc and does a GDI operation - their
        ///  world is NOT transformed.
        ///
        ///  HandleRef hDC = new HandleRef(this, originalGraphics.GetHdc());
        ///  try {
        ///  using(WindowsFormsUtils.DCMapping mapping = new WindowsFormsUtils.DCMapping(hDC, new Rectangle(10,10, 50, 50) {
        ///
        ///  // DO NOT ATTEMPT TO USE originalGraphics here - you'll get an Object Busy error
        ///  // rather ask the mapping object for a graphics object.
        ///  mapping.Graphics.DrawRectangle(Pens.Black, rect);
        ///  }
        ///  }
        ///  finally { g.ReleaseHdc(hDC.Handle);}
        ///
        ///  PERF: DCMapping is a structure so that it will allocate on the stack rather than in GC managed
        ///  memory. This way disposing the object does not force a GC. Since DCMapping objects aren't
        ///  likely to be passed between functions rather used and disposed in the same one, this reduces
        ///  overhead.
        /// </summary>
        internal struct DCMapping : IDisposable
        {
            private DeviceContext _dc;
            private Graphics _graphics;
            private Rectangle _translatedBounds;

            public unsafe DCMapping(HandleRef hDC, Rectangle bounds)
            {
                if (hDC.Handle == IntPtr.Zero)
                {
                    throw new ArgumentNullException(nameof(hDC));
                }

                bool success;
                IntPtr hOriginalClippingRegion = IntPtr.Zero;

                _translatedBounds = bounds;
                _graphics = null;
                _dc = DeviceContext.FromHdc(hDC.Handle);
                _dc.SaveHdc();

                // Retrieve the x-coordinates and y-coordinates of the viewport origin for the specified device context.
                success = SafeNativeMethods.GetViewportOrgEx(hDC, out Point viewportOrg);
                Debug.Assert(success, "GetViewportOrgEx() failed.");

                // Create a new rectangular clipping region based off of the bounds specified, shifted over by the x & y specified in the viewport origin.
                IntPtr hClippingRegion = Gdi32.CreateRectRgn(viewportOrg.X + bounds.Left, viewportOrg.Y + bounds.Top, viewportOrg.X + bounds.Right, viewportOrg.Y + bounds.Bottom);
                Debug.Assert(hClippingRegion != IntPtr.Zero, "CreateRectRgn() failed.");

                try
                {
                    // Create an empty region oriented at 0,0 so we can populate it with the original clipping region of the hDC passed in.
                    hOriginalClippingRegion = Gdi32.CreateRectRgn(0, 0, 0, 0);
                    Debug.Assert(hOriginalClippingRegion != IntPtr.Zero, "CreateRectRgn() failed.");

                    // Get the clipping region from the hDC: result = {-1 = error, 0 = no region, 1 = success} per MSDN
                    int result = Gdi32.GetClipRgn(hDC, hOriginalClippingRegion);
                    Debug.Assert(result != -1, "GetClipRgn() failed.");

                    // Shift the viewpoint origint by coordinates specified in "bounds".
                    var lastViewPort = new Point();
                    success = SafeNativeMethods.SetViewportOrgEx(hDC, viewportOrg.X + bounds.Left, viewportOrg.Y + bounds.Top, &lastViewPort);
                    Debug.Assert(success, "SetViewportOrgEx() failed.");

                    RegionType originalRegionType;
                    if (result != 0)
                    {
                        // Get the origninal clipping region so we can determine its type (we'll check later if we've restored the region back properly.)
                        RECT originalClipRect = new RECT();
                        originalRegionType = Gdi32.GetRgnBox(hOriginalClippingRegion, ref originalClipRect);
                        Debug.Assert(originalRegionType != RegionType.ERROR, "ERROR returned from SelectClipRgn while selecting the original clipping region..");

                        if (originalRegionType == RegionType.SIMPLEREGION)
                        {
                            // Find the intersection of our clipping region and the current clipping region (our parent's)
                            //      Returns a NULLREGION, the two didn't intersect.
                            //      Returns a SIMPLEREGION, the two intersected
                            //      Resulting region (stuff that was in hOriginalClippingRegion AND hClippingRegion is placed in hClippingRegion
                            RegionType combineResult = Gdi32.CombineRgn(hClippingRegion, hClippingRegion, hOriginalClippingRegion, Gdi32.CombineMode.RGN_AND);
                            Debug.Assert((combineResult == RegionType.SIMPLEREGION) ||
                                            (combineResult == RegionType.NULLREGION),
                                            "SIMPLEREGION or NULLREGION expected.");
                        }
                    }
                    else
                    {
                        // If there was no clipping region, then the result is a simple region.
                        // We don't need to keep track of the original now, since it is empty.
                        Gdi32.DeleteObject(hOriginalClippingRegion);
                        hOriginalClippingRegion = IntPtr.Zero;
                        originalRegionType = RegionType.SIMPLEREGION;
                    }

                    // Select the new clipping region; make sure it's a SIMPLEREGION or NULLREGION
                    RegionType selectResult = Gdi32.SelectClipRgn(hDC, hClippingRegion);
                    Debug.Assert((selectResult == RegionType.SIMPLEREGION ||
                                  selectResult == RegionType.NULLREGION),
                                  "SIMPLEREGION or NULLLREGION expected.");

                }
                catch (Exception ex) when (!ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    _dc.RestoreHdc();
                    _dc.Dispose();
                }
                finally
                {
                    // Delete the new clipping region, as the clipping region for the HDC is now set
                    // to this rectangle. Hold on to hOriginalClippingRegion, as we'll need to restore
                    // it when this object is disposed.
                    success = Gdi32.DeleteObject(hClippingRegion) != BOOL.FALSE;
                    Debug.Assert(success, "DeleteObject(hClippingRegion) failed.");

                    if (hOriginalClippingRegion != IntPtr.Zero)
                    {
                        success = Gdi32.DeleteObject(hOriginalClippingRegion) != BOOL.FALSE;
                        Debug.Assert(success, "DeleteObject(hOriginalClippingRegion) failed.");
                    }
                }
            }

            public void Dispose()
            {
                if (_graphics != null)
                {
                    // Reset GDI+ if used.
                    // we need to dispose the graphics object first, as it will do
                    // some restoration to the ViewPort and ClipRectangle to restore the hDC to
                    // the same state it was created in
                    _graphics.Dispose();
                    _graphics = null;
                }

                if (_dc != null)
                {
                    // Now properly reset GDI.
                    _dc.RestoreHdc();
                    _dc.Dispose();
                    _dc = null;
                }
            }

            /// <summary>
            ///  Allows you to get the graphics object based off of the translated HDC.
            ///  Note this will be disposed when the DCMapping object is disposed.
            /// </summary>
            public Graphics Graphics
            {
                get
                {
                    Debug.Assert(_dc != null, "unexpected null dc!");

                    if (_graphics == null)
                    {
                        _graphics = Graphics.FromHdcInternal(_dc.Hdc);
                        _graphics.SetClip(new Rectangle(Point.Empty, _translatedBounds.Size));
                    }

                    return _graphics;
                }
            }
        }
    }
}
