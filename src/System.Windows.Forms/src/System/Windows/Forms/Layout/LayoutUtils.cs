// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;

namespace System.Windows.Forms.Layout;

// Utilities used by layout code. If you use these outside of the layout
// namespace, you should probably move them to WindowsFormsUtils.
internal partial class LayoutUtils
{
    public static readonly Size s_maxSize = new(int.MaxValue, int.MaxValue);
    public static readonly Size s_invalidSize = new(int.MinValue, int.MinValue);

    public static readonly Rectangle s_maxRectangle = new(0, 0, int.MaxValue, int.MaxValue);

    public const ContentAlignment AnyTop = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
    public const ContentAlignment AnyBottom = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
    public const ContentAlignment AnyLeft = ContentAlignment.TopLeft | ContentAlignment.MiddleLeft | ContentAlignment.BottomLeft;
    public const ContentAlignment AnyRight = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;
    public const ContentAlignment AnyCenter = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;
    public const ContentAlignment AnyMiddle = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;

    public const AnchorStyles HorizontalAnchorStyles = AnchorStyles.Left | AnchorStyles.Right;
    public const AnchorStyles VerticalAnchorStyles = AnchorStyles.Top | AnchorStyles.Bottom;

    private static readonly AnchorStyles[] s_dockingToAnchor =
    [
        /* None   */ AnchorStyles.Top | AnchorStyles.Left,
        /* Top    */ AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
        /* Bottom */ AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
        /* Left   */ AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom,
        /* Right  */ AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
        /* Fill   */ AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
    ];

    // A good, short test string for measuring control height.
    public const string TestString = "j^";

    // Returns the size of the largest string in the given collection. Non-string objects are converted
    // with ToString(). Uses OldMeasureString, not GDI+. Does not support multiline.
    public static Size OldGetLargestStringSizeInCollection(Font? font, ICollection? objects)
    {
        Size largestSize = Size.Empty;
        if (objects is not null)
        {
            foreach (object obj in objects)
            {
                Size textSize = TextRenderer.MeasureText(obj.ToString(), font, new Size(short.MaxValue, short.MaxValue), TextFormatFlags.SingleLine);
                largestSize.Width = Math.Max(largestSize.Width, textSize.Width);
                largestSize.Height = Math.Max(largestSize.Height, textSize.Height);
            }
        }

        return largestSize;
    }

    /*
     *  We can cut ContentAlignment from a max index of 1024 (12b) down to 11 (4b) through
     *  bit twiddling. The int result of this function maps to the ContentAlignment as indicated
     *  by the table below:
     *
     *          Left      Center    Right
     *  Top     0000 0x0  0001 0x1  0010 0x2
     *  Middle  0100 0x4  0101 0x5  0110 0x6
     *  Bottom  1000 0x8  1001 0x9  1010 0xA
     *
     *  (The high 2 bits determine T/M/B. The low 2 bits determine L/C/R.)
     */

    public static int ContentAlignmentToIndex(ContentAlignment alignment)
    {
        /*
         *  Here is what content alignment looks like coming in:
         *
         *          Left    Center  Right
         *  Top     0x001   0x002   0x004
         *  Middle  0x010   0x020   0x040
         *  Bottom  0x100   0x200   0x400
         *
         *  (L/C/R determined bit 1,2,4. T/M/B determined by 4 bit shift.)
         */

        int topBits = xContentAlignmentToIndex(((int)alignment) & 0x0F);
        int middleBits = xContentAlignmentToIndex(((int)alignment >> 4) & 0x0F);
        int bottomBits = xContentAlignmentToIndex(((int)alignment >> 8) & 0x0F);

        Debug.Assert((topBits != 0 && (middleBits == 0 && bottomBits == 0))
            || (middleBits != 0 && (topBits == 0 && bottomBits == 0))
            || (bottomBits != 0 && (topBits == 0 && middleBits == 0)),
            "One (and only one) of topBits, middleBits, or bottomBits should be non-zero.");

        int result = (middleBits != 0 ? 0x04 : 0) | (bottomBits != 0 ? 0x08 : 0) | topBits | middleBits | bottomBits;

        // zero isn't used, so we can subtract 1 and start with index 0.
        result--;

        Debug.Assert(result is >= 0x00 and <= 0x0A, "ContentAlignmentToIndex result out of range.");
        Debug.Assert(result != 0x00 || alignment == ContentAlignment.TopLeft, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x01 || alignment == ContentAlignment.TopCenter, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x02 || alignment == ContentAlignment.TopRight, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x03, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x04 || alignment == ContentAlignment.MiddleLeft, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x05 || alignment == ContentAlignment.MiddleCenter, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x06 || alignment == ContentAlignment.MiddleRight, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x07, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x08 || alignment == ContentAlignment.BottomLeft, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x09 || alignment == ContentAlignment.BottomCenter, "Error detected in ContentAlignmentToIndex.");
        Debug.Assert(result != 0x0A || alignment == ContentAlignment.BottomRight, "Error detected in ContentAlignmentToIndex.");

        return result;
    }

    // Converts 0x00, 0x01, 0x02, 0x04 (3b flag) to 0, 1, 2, 3 (2b index)
    private static byte xContentAlignmentToIndex(int threeBitFlag)
    {
        Debug.Assert(threeBitFlag is >= 0x00 and <= 0x04 and not 0x03, "threeBitFlag out of range.");
        byte result = threeBitFlag == 0x04 ? (byte)3 : (byte)threeBitFlag;
        Debug.Assert((result & 0x03) == result, "Result out of range.");
        return result;
    }

    public static Size ConvertZeroToUnbounded(Size size)
    {
        if (size.Width == 0)
        {
            size.Width = int.MaxValue;
        }

        if (size.Height == 0)
        {
            size.Height = int.MaxValue;
        }

        return size;
    }

    // Clamps negative values in Padding struct to zero.
    public static Padding ClampNegativePaddingToZero(Padding padding)
    {
        // Careful: Setting the LRTB properties causes Padding.All to be -1 even if LRTB all agree.
        if (padding.All < 0)
        {
            padding.Left = Math.Max(0, padding.Left);
            padding.Top = Math.Max(0, padding.Top);
            padding.Right = Math.Max(0, padding.Right);
            padding.Bottom = Math.Max(0, padding.Bottom);
        }

        return padding;
    }

    /*
     *  Maps an anchor to its opposite. Does not support combinations. None returns none.
     *
     *  Top     = 0x01
     *  Bottom  = 0x02
     *  Left    = 0x04
     *  Right   = 0x08
     */

    // Returns the positive opposite of the given anchor (e.g., L -> R, LT -> RB, LTR -> LBR, etc.). None return none.
    private static AnchorStyles GetOppositeAnchor(AnchorStyles anchor)
    {
        AnchorStyles result = AnchorStyles.None;
        if (anchor == AnchorStyles.None)
        {
            return result;
        }

        // iterate through T,B,L,R
        // bitwise or      B,T,R,L as appropriate
        for (int i = 1; i <= (int)AnchorStyles.Right; i <<= 1)
        {
            switch (anchor & (AnchorStyles)i)
            {
                case AnchorStyles.None:
                    break;
                case AnchorStyles.Left:
                    result |= AnchorStyles.Right;
                    break;
                case AnchorStyles.Top:
                    result |= AnchorStyles.Bottom;
                    break;
                case AnchorStyles.Right:
                    result |= AnchorStyles.Left;
                    break;
                case AnchorStyles.Bottom:
                    result |= AnchorStyles.Top;
                    break;
                default:
                    break;
            }
        }

        return result;
    }

    public static TextImageRelation GetOppositeTextImageRelation(TextImageRelation relation)
    {
        return (TextImageRelation)GetOppositeAnchor((AnchorStyles)relation);
    }

    public static Size UnionSizes(Size a, Size b)
    {
        return new Size(
            Math.Max(a.Width, b.Width),
            Math.Max(a.Height, b.Height));
    }

    public static Size IntersectSizes(Size a, Size b)
    {
        return new Size(
            Math.Min(a.Width, b.Width),
            Math.Min(a.Height, b.Height));
    }

    public static bool IsIntersectHorizontally(Rectangle rect1, Rectangle rect2)
    {
        if (!rect1.IntersectsWith(rect2))
        {
            return false;
        }

        if (rect1.X <= rect2.X && rect1.X + rect1.Width >= rect2.X + rect2.Width)
        {
            // rect 1 contains rect 2 horizontally
            return true;
        }

        if (rect2.X <= rect1.X && rect2.X + rect2.Width >= rect1.X + rect1.Width)
        {
            // rect 2 contains rect 1 horizontally
            return true;
        }

        return false;
    }

    public static bool IsIntersectVertically(Rectangle rect1, Rectangle rect2)
    {
        if (!rect1.IntersectsWith(rect2))
        {
            return false;
        }

        if (rect1.Y <= rect2.Y && rect1.Y + rect1.Width >= rect2.Y + rect2.Width)
        {
            // rect 1 contains rect 2 vertically
            return true;
        }

        if (rect2.Y <= rect1.Y && rect2.Y + rect2.Width >= rect1.Y + rect1.Width)
        {
            // rect 2 contains rect 1 vertically
            return true;
        }

        return false;
    }

    // returns anchorStyles, transforms from DockStyle if necessary
    internal static AnchorStyles GetUnifiedAnchor(IArrangedElement element)
    {
        DockStyle dockStyle = DefaultLayout.GetDock(element);
        if (dockStyle != DockStyle.None)
        {
            return s_dockingToAnchor[(int)dockStyle];
        }

        return DefaultLayout.GetAnchor(element);
    }

    public static Rectangle AlignAndStretch(Size fitThis, Rectangle withinThis, AnchorStyles anchorStyles)
    {
        return Align(Stretch(fitThis, withinThis.Size, anchorStyles), withinThis, anchorStyles);
    }

    public static Rectangle Align(Size alignThis, Rectangle withinThis, AnchorStyles anchorStyles)
    {
        return VAlign(alignThis, HAlign(alignThis, withinThis, anchorStyles), anchorStyles);
    }

    public static Rectangle Align(Size alignThis, Rectangle withinThis, ContentAlignment align)
    {
        return VAlign(alignThis, HAlign(alignThis, withinThis, align), align);
    }

    public static Rectangle HAlign(Size alignThis, Rectangle withinThis, AnchorStyles anchorStyles)
    {
        if ((anchorStyles & AnchorStyles.Right) != 0)
        {
            withinThis.X += withinThis.Width - alignThis.Width;
        }
        else if (anchorStyles == AnchorStyles.None || (anchorStyles & HorizontalAnchorStyles) == 0)
        {
            withinThis.X += (withinThis.Width - alignThis.Width) / 2;
        }

        withinThis.Width = alignThis.Width;

        return withinThis;
    }

    private static Rectangle HAlign(Size alignThis, Rectangle withinThis, ContentAlignment align)
    {
        if ((align & AnyRight) != 0)
        {
            withinThis.X += withinThis.Width - alignThis.Width;
        }
        else if ((align & AnyCenter) != 0)
        {
            withinThis.X += (withinThis.Width - alignThis.Width) / 2;
        }

        withinThis.Width = alignThis.Width;

        return withinThis;
    }

    public static Rectangle VAlign(Size alignThis, Rectangle withinThis, AnchorStyles anchorStyles)
    {
        if ((anchorStyles & AnchorStyles.Bottom) != 0)
        {
            withinThis.Y += withinThis.Height - alignThis.Height;
        }
        else if (anchorStyles == AnchorStyles.None || (anchorStyles & VerticalAnchorStyles) == 0)
        {
            withinThis.Y += (withinThis.Height - alignThis.Height) / 2;
        }

        withinThis.Height = alignThis.Height;

        return withinThis;
    }

    public static Rectangle VAlign(Size alignThis, Rectangle withinThis, ContentAlignment align)
    {
        if ((align & AnyBottom) != 0)
        {
            withinThis.Y += withinThis.Height - alignThis.Height;
        }
        else if ((align & AnyMiddle) != 0)
        {
            withinThis.Y += (withinThis.Height - alignThis.Height) / 2;
        }

        withinThis.Height = alignThis.Height;

        return withinThis;
    }

    public static Size Stretch(Size stretchThis, Size withinThis, AnchorStyles anchorStyles)
    {
        Size stretchedSize = new(
            (anchorStyles & HorizontalAnchorStyles) == HorizontalAnchorStyles ? withinThis.Width : stretchThis.Width,
            (anchorStyles & VerticalAnchorStyles) == VerticalAnchorStyles ? withinThis.Height : stretchThis.Height);
        if (stretchedSize.Width > withinThis.Width)
        {
            stretchedSize.Width = withinThis.Width;
        }

        if (stretchedSize.Height > withinThis.Height)
        {
            stretchedSize.Height = withinThis.Height;
        }

        return stretchedSize;
    }

    public static Rectangle InflateRect(Rectangle rect, Padding padding)
    {
        rect.X -= padding.Left;
        rect.Y -= padding.Top;
        rect.Width += padding.Horizontal;
        rect.Height += padding.Vertical;
        return rect;
    }

    public static Rectangle DeflateRect(Rectangle rect, Padding padding)
    {
        rect.X += padding.Left;
        rect.Y += padding.Top;
        rect.Width -= padding.Horizontal;
        rect.Height -= padding.Vertical;
        return rect;
    }

    public static Size AddAlignedRegion(Size textSize, Size imageSize, TextImageRelation relation)
    {
        return AddAlignedRegionCore(textSize, imageSize, IsVerticalRelation(relation));
    }

    public static Size AddAlignedRegionCore(Size currentSize, Size contentSize, bool vertical)
    {
        if (vertical)
        {
            currentSize.Width = Math.Max(currentSize.Width, contentSize.Width);
            currentSize.Height += contentSize.Height;
        }
        else
        {
            currentSize.Width += contentSize.Width;
            currentSize.Height = Math.Max(currentSize.Height, contentSize.Height);
        }

        return currentSize;
    }

    public static Padding FlipPadding(Padding padding)
    {
        // If Padding.All != -1, then TLRB are all the same and there is no work to be done.
        if (padding.All != -1)
        {
            return padding;
        }

        // Padding is a stuct (passed by value, no need to make a copy)
        int temp;

        temp = padding.Top;
        padding.Top = padding.Left;
        padding.Left = temp;

        temp = padding.Bottom;
        padding.Bottom = padding.Right;
        padding.Right = temp;

        return padding;
    }

    public static Point FlipPoint(Point point)
    {
        // Point is a struct (passed by value, no need to make a copy)
        (point.Y, point.X) = (point.X, point.Y);
        return point;
    }

    public static Rectangle FlipRectangle(Rectangle rect)
    {
        // Rectangle is a stuct (passed by value, no need to make a copy)
        rect.Location = FlipPoint(rect.Location);
        rect.Size = FlipSize(rect.Size);
        return rect;
    }

    public static Rectangle FlipRectangleIf(bool condition, Rectangle rect)
    {
        return condition ? FlipRectangle(rect) : rect;
    }

    public static Size FlipSize(Size size)
    {
        // Size is a struct (passed by value, no need to make a copy)
        (size.Height, size.Width) = (size.Width, size.Height);
        return size;
    }

    public static Size FlipSizeIf(bool condition, Size size)
    {
        return condition ? FlipSize(size) : size;
    }

    public static bool IsHorizontalAlignment(ContentAlignment align)
    {
        return !IsVerticalAlignment(align);
    }

    // True if text & image should be lined up horizontally. False if vertical or overlay.
    public static bool IsHorizontalRelation(TextImageRelation relation)
    {
        return (relation & (TextImageRelation.TextBeforeImage | TextImageRelation.ImageBeforeText)) != 0;
    }

    public static bool IsVerticalAlignment(ContentAlignment align)
    {
        Debug.Assert(align != ContentAlignment.MiddleCenter, "Result is ambiguous with an alignment of MiddleCenter.");
        return (align & (ContentAlignment.TopCenter | ContentAlignment.BottomCenter)) != 0;
    }

    // True if text & image should be lined up vertically. False if horizontal or overlay.
    public static bool IsVerticalRelation(TextImageRelation relation)
    {
        return (relation & (TextImageRelation.TextAboveImage | TextImageRelation.ImageAboveText)) != 0;
    }

    public static bool IsZeroWidthOrHeight(Rectangle rectangle)
    {
        return (rectangle.Width == 0 || rectangle.Height == 0);
    }

    public static bool IsZeroWidthOrHeight(Size size)
    {
        return (size.Width == 0 || size.Height == 0);
    }

    public static bool AreWidthAndHeightLarger(Size size1, Size size2)
    {
        return ((size1.Width >= size2.Width) && (size1.Height >= size2.Height));
    }

    public static void SplitRegion(Rectangle bounds, Size specifiedContent, AnchorStyles region1Align, out Rectangle region1, out Rectangle region2)
    {
        region1 = region2 = bounds;
        switch (region1Align)
        {
            case AnchorStyles.Left:
                region1.Width = specifiedContent.Width;
                region2.X += specifiedContent.Width;
                region2.Width -= specifiedContent.Width;
                break;
            case AnchorStyles.Right:
                region1.X += bounds.Width - specifiedContent.Width;
                region1.Width = specifiedContent.Width;
                region2.Width -= specifiedContent.Width;
                break;
            case AnchorStyles.Top:
                region1.Height = specifiedContent.Height;
                region2.Y += specifiedContent.Height;
                region2.Height -= specifiedContent.Height;
                break;
            case AnchorStyles.Bottom:
                region1.Y += bounds.Height - specifiedContent.Height;
                region1.Height = specifiedContent.Height;
                region2.Height -= specifiedContent.Height;
                break;
            default:
                Debug.Fail("Unsupported value for region1Align.");
                break;
        }

        Debug.Assert(Rectangle.Union(region1, region2) == bounds,
            "Regions do not add up to bounds.");
    }

    // Expands adjacent regions to bounds. region1Align indicates which way the adjacency occurs.
    public static void ExpandRegionsToFillBounds(Rectangle bounds, AnchorStyles region1Align, ref Rectangle region1, ref Rectangle region2)
    {
        switch (region1Align)
        {
            case AnchorStyles.Left:
                Debug.Assert(region1.Right == region2.Left, "Adjacency error.");
                region1 = SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Right);
                region2 = SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Left);
                break;
            case AnchorStyles.Right:
                Debug.Assert(region2.Right == region1.Left, "Adjacency error.");
                region1 = SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Left);
                region2 = SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Right);
                break;
            case AnchorStyles.Top:
                Debug.Assert(region1.Bottom == region2.Top, "Adjacency error.");
                region1 = SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Bottom);
                region2 = SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Top);
                break;
            case AnchorStyles.Bottom:
                Debug.Assert(region2.Bottom == region1.Top, "Adjacency error.");
                region1 = SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Top);
                region2 = SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Bottom);
                break;
            default:
                Debug.Fail("Unsupported value for region1Align.");
                break;
        }

        Debug.Assert(Rectangle.Union(region1, region2) == bounds, "region1 and region2 do not add up to bounds.");
    }

    public static Size SubAlignedRegion(Size currentSize, Size contentSize, TextImageRelation relation)
    {
        return SubAlignedRegionCore(currentSize, contentSize, IsVerticalRelation(relation));
    }

    public static Size SubAlignedRegionCore(Size currentSize, Size contentSize, bool vertical)
    {
        if (vertical)
        {
            currentSize.Height -= contentSize.Height;
        }
        else
        {
            currentSize.Width -= contentSize.Width;
        }

        return currentSize;
    }

    private static Rectangle SubstituteSpecifiedBounds(Rectangle originalBounds, Rectangle substitutionBounds, AnchorStyles specified)
    {
        int left = (specified & AnchorStyles.Left) != 0 ? substitutionBounds.Left : originalBounds.Left;
        int top = (specified & AnchorStyles.Top) != 0 ? substitutionBounds.Top : originalBounds.Top;
        int right = (specified & AnchorStyles.Right) != 0 ? substitutionBounds.Right : originalBounds.Right;
        int bottom = (specified & AnchorStyles.Bottom) != 0 ? substitutionBounds.Bottom : originalBounds.Bottom;
        return Rectangle.FromLTRB(left, top, right, bottom);
    }

    // given a rectangle, flip to the other side of (withinBounds)
    //
    // Never call this if you derive from ScrollableControl
    public static Rectangle RTLTranslate(Rectangle bounds, Rectangle withinBounds)
    {
        bounds.X = withinBounds.Width - bounds.Right;
        return bounds;
    }
}
