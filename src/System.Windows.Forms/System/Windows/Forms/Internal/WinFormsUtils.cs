// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms;

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
            int lastXY = (int)PInvoke.GetMessagePos();
            return new Point(PARAM.SignedLOWORD(lastXY), PARAM.SignedHIWORD(lastXY));
        }
    }

    /// <summary>
    ///  If you want to know if a piece of text contains one and only one &amp;
    ///  this is your function. If you have a character "t" and want match it to &amp;Text
    ///  Control.IsMnemonic is a better bet.
    /// </summary>
    public static bool ContainsMnemonic([NotNullWhen(true)] string? text)
    {
        if (text is not null)
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
    internal static string? EscapeTextWithAmpersands(string? text)
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

        StringBuilder str = new(text.Length);
        str.Append(text.AsSpan(0, index));
        for (; index < text.Length; ++index)
        {
            if (text[index] == '&')
            {
                str.Append('&');
            }

            str.Append(text[index]);
        }

        return str.ToString();
    }

    /// <summary>
    ///  Helper function for generating information about a particular control.
    /// </summary>
    internal static string GetControlInformation(HWND hwnd)
    {
        if (hwnd.IsNull)
        {
            return "Handle is null";
        }

#if DEBUG
        string windowText = PInvokeCore.GetWindowText(hwnd);
        string typeOfControl = "Unknown";
        string nameOfControl = "";
        Control? c = Control.FromHandle(hwnd);
        if (c is not null)
        {
            typeOfControl = c.GetType().ToString();
            if (!string.IsNullOrEmpty(c.Name))
            {
                nameOfControl = c.Name;
            }
            else
            {
                nameOfControl = "Unknown";

                // Add some extra debug info for ToolStripDropDowns.
                if (c is ToolStripDropDown dd && dd.OwnerItem is not null)
                {
                    nameOfControl += $"{Environment.NewLine}\tOwnerItem: {dd.OwnerItem}";
                }
            }
        }

        return $"""
            {windowText}
                Type: {typeOfControl}
                Name: {nameOfControl}

            """;
#else
        return string.Empty;
#endif
    }

    /// <summary>
    ///  Retrieves the mnemonic from a given string, or zero if no mnemonic.
    ///  As used by the Control.Mnemonic to get mnemonic from Control.Text.
    /// </summary>
    public static char GetMnemonic(string? text, bool convertToUpperCase)
    {
        char mnemonic = '\0';
        if (text is not null)
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
    ///  <para>
    ///   Note: Be careful not to call this multiple times on the same string, otherwise you'll turn
    ///   something like "Fi&amp;sh &amp;&amp; Chips" into "Fish &amp; Chips" on the first call, and then "Fish Chips"
    ///   on the second call.
    ///  </para>
    /// </remarks>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? TextWithoutMnemonics(string? text)
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

        StringBuilder str = new(text.Length);
        str.Append(text.AsSpan(0, index));
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
    ///  Translates a point from one control's coordinate system to the other.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Same as controlTo.PointToClient(controlFrom.PointToScreen(point)), but more slightly more performant.
    ///  </para>
    /// </remarks>
    public static Point TranslatePoint(Point point, Control fromControl, Control toControl)
    {
        PInvokeCore.MapWindowPoints(fromControl, toControl, ref point);
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
    public static bool SafeCompareStrings(string? string1, string? string2, bool ignoreCase)
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

    public static string GetComponentName(IComponent component, string? defaultNameValue)
    {
        Debug.Assert(component is not null, "component passed here cannot be null");
        if (string.IsNullOrEmpty(defaultNameValue))
        {
            return component.Site?.Name ?? string.Empty;
        }
        else
        {
            return defaultNameValue;
        }
    }
}
