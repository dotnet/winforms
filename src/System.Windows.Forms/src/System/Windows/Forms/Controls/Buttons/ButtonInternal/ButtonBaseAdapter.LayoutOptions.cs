// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms.ButtonInternal;

internal abstract partial class ButtonBaseAdapter
{
    internal partial class LayoutOptions
    {
        private static readonly int s_combineCheck = BitVector32.CreateMask();
        private static readonly int s_combineImageText = BitVector32.CreateMask(s_combineCheck);

        private bool _disableWordWrapping;

        // If this is changed to a property callers will need to be updated
        // as they modify fields in the Rectangle.
        public Rectangle Client;

        public bool GrowBorderBy1PxWhenDefault { get; set; }
        public bool IsDefault { get; set; }
        public int BorderSize { get; set; }
        public int PaddingSize { get; set; }
        public bool MaxFocus { get; set; }
        public bool FocusOddEvenFixup { get; set; }
        public Font Font { get; set; } = null!;
        public string? Text { get; set; }
        public Size ImageSize { get; set; }
        public int CheckSize { get; set; }
        public int CheckPaddingSize { get; set; }
        public ContentAlignment CheckAlign { get; set; }
        public ContentAlignment ImageAlign { get; set; }
        public ContentAlignment TextAlign { get; set; }
        public TextImageRelation TextImageRelation { get; set; }
        public bool HintTextUp { get; set; }
        public bool TextOffset { get; set; }
        public bool ShadowedText { get; set; }
        public bool LayoutRTL { get; set; }
        public bool VerticalText { get; set; }
        public bool UseCompatibleTextRendering { get; set; }

        /// <summary>
        ///  .NET Framework 1.0/1.1 compatibility
        /// </summary>
        public bool DotNetOneButtonCompat { get; set; } = true;
        public TextFormatFlags GdiTextFormatFlags { get; set; } = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;
        public StringFormatFlags GdiPlusFormatFlags { get; set; }
        public StringTrimming GdiPlusTrimming { get; set; }
        public HotkeyPrefix GdiPlusHotkeyPrefix { get; set; }

        /// <summary>
        ///  Horizontal alignment.
        /// </summary>
        public StringAlignment GdiPlusAlignment { get; set; }

        /// <summary>
        ///  Vertical alignment.
        /// </summary>
        public StringAlignment GdiPlusLineAlignment { get; set; }

        /// <summary>
        ///  We don't cache the <see cref="StringFormat"/> itself because we don't have a deterministic way of
        ///  disposing it, instead we cache the flags that make it up and create it on demand so it can be disposed
        ///  by calling code.
        /// </summary>
        public StringFormat StringFormat
        {
            get
            {
                StringFormat format = new()
                {
                    FormatFlags = GdiPlusFormatFlags,
                    Trimming = GdiPlusTrimming,
                    HotkeyPrefix = GdiPlusHotkeyPrefix,
                    Alignment = GdiPlusAlignment,
                    LineAlignment = GdiPlusLineAlignment
                };

                if (_disableWordWrapping)
                {
                    format.FormatFlags |= StringFormatFlags.NoWrap;
                }

                return format;
            }
            set
            {
                GdiPlusFormatFlags = value.FormatFlags;
                GdiPlusTrimming = value.Trimming;
                GdiPlusHotkeyPrefix = value.HotkeyPrefix;
                GdiPlusAlignment = value.Alignment;
                GdiPlusLineAlignment = value.LineAlignment;
            }
        }

        public TextFormatFlags TextFormatFlags =>
            _disableWordWrapping ? GdiTextFormatFlags & ~TextFormatFlags.WordBreak : GdiTextFormatFlags;

        /// <summary>
        ///  TextImageInset compensates for two factors: 3d text when the button is disabled,
        ///  and moving text on 3d-look buttons. These factors make the text require a couple
        ///  more pixels of space. We inset image by the same amount so they line up.
        /// </summary>
        public int TextImageInset { get; set; } = 2;

        public Padding Padding { get; set; }

        /// <summary>
        ///  Uses <see cref="CheckAlign"/>, <see cref="ImageAlign"/>, and <see cref="TextAlign"/> to compose
        ///  <paramref name="checkSize"/>, <paramref name="imageSize"/>, and <paramref name="textSize"/> into
        ///  the preferred size.
        /// </summary>
        private Size Compose(Size checkSize, Size imageSize, Size textSize)
        {
            Composition hComposition = GetHorizontalComposition();
            Composition vComposition = GetVerticalComposition();
            return new Size(
                xCompose(hComposition, checkSize.Width, imageSize.Width, textSize.Width),
                xCompose(vComposition, checkSize.Height, imageSize.Height, textSize.Height));

            static int xCompose(Composition composition, int checkSize, int imageSize, int textSize)
            {
                switch (composition)
                {
                    case Composition.NoneCombined:
                        return checkSize + imageSize + textSize;
                    case Composition.CheckCombined:
                        return Math.Max(checkSize, imageSize + textSize);
                    case Composition.TextImageCombined:
                        return Math.Max(imageSize, textSize) + checkSize;
                    case Composition.AllCombined:
                        return Math.Max(Math.Max(checkSize, imageSize), textSize);
                    default:
                        Debug.Fail(string.Format(SR.InvalidArgument, nameof(composition), composition.ToString()));
                        return -7107;
                }
            }
        }

        /// <summary>
        ///  Uses <see cref="CheckAlign"/>, <see cref="ImageAlign"/>, and <see cref="TextAlign"/> to decompose
        ///  <paramref name="proposedSize"/> into the space left over for text.
        /// </summary>
        private Size Decompose(Size checkSize, Size imageSize, Size proposedSize)
        {
            Composition hComposition = GetHorizontalComposition();
            Composition vComposition = GetVerticalComposition();
            return new Size(
                xDecompose(hComposition, checkSize.Width, imageSize.Width, proposedSize.Width),
                xDecompose(vComposition, checkSize.Height, imageSize.Height, proposedSize.Height));

            static int xDecompose(Composition composition, int checkSize, int imageSize, int proposedSize)
            {
                switch (composition)
                {
                    case Composition.NoneCombined:
                        return proposedSize - (checkSize + imageSize);
                    case Composition.CheckCombined:
                        return proposedSize - imageSize;
                    case Composition.TextImageCombined:
                        return proposedSize - checkSize;
                    case Composition.AllCombined:
                        return proposedSize;
                    default:
                        Debug.Fail(string.Format(SR.InvalidArgument, nameof(composition), composition.ToString()));
                        return -7109;
                }
            }
        }

        private Composition GetHorizontalComposition()
        {
            BitVector32 action = default;

            // Checks reserve space horizontally if possible, so only AnyLeft/AnyRight prevents combination.
            action[s_combineCheck] =
                CheckAlign == ContentAlignment.MiddleCenter || !LayoutUtils.IsHorizontalAlignment(CheckAlign);

            action[s_combineImageText] = !LayoutUtils.IsHorizontalRelation(TextImageRelation);
            return (Composition)action.Data;
        }

        internal Size GetPreferredSizeCore(Size proposedSize)
        {
            // Get space required for border and padding.
            int linearBorderAndPadding = BorderSize * 2 + PaddingSize * 2;
            if (GrowBorderBy1PxWhenDefault)
            {
                linearBorderAndPadding += 2;
            }

            Size bordersAndPadding = new(linearBorderAndPadding, linearBorderAndPadding);
            proposedSize -= bordersAndPadding;

            // Get space required for check.
            int checkSizeLinear = FullCheckSize;
            Size checkSize = checkSizeLinear > 0 ? new(checkSizeLinear + 1, checkSizeLinear) : Size.Empty;

            // Get space required for Image - textImageInset compensated for by expanding image.
            Size textImageInsetSize = new(TextImageInset * 2, TextImageInset * 2);
            Size requiredImageSize = (ImageSize != Size.Empty) ? ImageSize + textImageInsetSize : Size.Empty;

            // Pack Text into remaining space
            proposedSize -= textImageInsetSize;
            proposedSize = Decompose(checkSize, requiredImageSize, proposedSize);

            Size textSize = Size.Empty;

            if (!string.IsNullOrEmpty(Text))
            {
                // When Button.AutoSizeMode is set to GrowOnly TableLayoutPanel expects buttons not to
                // automatically wrap on word break. If there's enough room for the text to word-wrap then it
                // will happen but the layout would not be adjusted to allow text wrapping. If someone has a
                // carriage return in the text we'll honor that for preferred size, but we won't wrap based
                // on constraints.
                try
                {
                    _disableWordWrapping = true;
                    textSize = GetTextSize(proposedSize) + textImageInsetSize;
                }
                finally
                {
                    _disableWordWrapping = false;
                }
            }

            // Combine pieces to get final preferred size.
            Size requiredSize = Compose(checkSize, ImageSize, textSize);
            requiredSize += bordersAndPadding;

            return requiredSize;
        }

        private Composition GetVerticalComposition()
        {
            BitVector32 action = default;

            // Checks reserve space horizontally if possible, so only Top/Bottom prevents combination.
            action[s_combineCheck] = CheckAlign == ContentAlignment.MiddleCenter || !LayoutUtils.IsVerticalAlignment(CheckAlign);
            action[s_combineImageText] = !LayoutUtils.IsVerticalRelation(TextImageRelation);
            return (Composition)action.Data;
        }

        private int FullBorderSize => OnePixExtraBorder ? BorderSize++ : BorderSize;

        private bool OnePixExtraBorder => GrowBorderBy1PxWhenDefault && IsDefault;

        internal LayoutData Layout()
        {
            LayoutData layout = new(this)
            {
                Client = Client
            };

            // Subtract border size from layout area.
            int fullBorderSize = FullBorderSize;
            layout.Face = Rectangle.Inflate(layout.Client, -fullBorderSize, -fullBorderSize);

            // CheckBounds, CheckArea, Field.
            CalcCheckmarkRectangle(layout);

            // ImageBounds, ImageLocation, TextBounds.
            LayoutTextAndImage(layout);

            // Focus.
            if (MaxFocus)
            {
                layout.Focus = layout.Field;
                layout.Focus.Inflate(-1, -1);

                // Adjust for padding.
                layout.Focus = LayoutUtils.InflateRect(layout.Focus, Padding);
            }
            else
            {
                Rectangle textAdjusted = new(
                    layout.TextBounds.X - 1,
                    layout.TextBounds.Y - 1,
                    layout.TextBounds.Width + 2,
                    layout.TextBounds.Height + 3);

                layout.Focus = ImageSize != Size.Empty
                    ? Rectangle.Union(textAdjusted, layout.ImageBounds)
                    : textAdjusted;
            }

            if (FocusOddEvenFixup)
            {
                if (layout.Focus.Height % 2 == 0)
                {
                    layout.Focus.Y++;
                    layout.Focus.Height--;
                }

                if (layout.Focus.Width % 2 == 0)
                {
                    layout.Focus.X++;
                    layout.Focus.Width--;
                }
            }

            return layout;
        }

        private TextImageRelation RtlTranslateRelation(TextImageRelation relation)
        {
            // If RTL, we swap ImageBeforeText and TextBeforeImage.
            if (LayoutRTL)
            {
                switch (relation)
                {
                    case TextImageRelation.ImageBeforeText:
                        return TextImageRelation.TextBeforeImage;
                    case TextImageRelation.TextBeforeImage:
                        return TextImageRelation.ImageBeforeText;
                }
            }

            return relation;
        }

        internal ContentAlignment RtlTranslateContent(ContentAlignment align)
        {
            if (LayoutRTL)
            {
                ContentAlignment[][] mapping =
                [
                    [ContentAlignment.TopLeft, ContentAlignment.TopRight],
                    [ContentAlignment.MiddleLeft, ContentAlignment.MiddleRight],
                    [ContentAlignment.BottomLeft, ContentAlignment.BottomRight],
                ];

                for (int i = 0; i < 3; ++i)
                {
                    if (mapping[i][0] == align)
                    {
                        return mapping[i][1];
                    }
                    else if (mapping[i][1] == align)
                    {
                        return mapping[i][0];
                    }
                }
            }

            return align;
        }

        private int FullCheckSize => CheckSize + CheckPaddingSize;

        private void CalcCheckmarkRectangle(LayoutData layout)
        {
            int checkSizeFull = FullCheckSize;
            layout.CheckBounds = new Rectangle(Client.X, Client.Y, checkSizeFull, checkSizeFull);

            // Translate checkAlign for Rtl applications
            ContentAlignment align = RtlTranslateContent(CheckAlign);

            Rectangle field = Rectangle.Inflate(layout.Face, -PaddingSize, -PaddingSize);

            layout.Field = field;

            if (checkSizeFull <= 0)
            {
                return;
            }

            if ((align & LayoutUtils.AnyRight) != 0)
            {
                layout.CheckBounds.X = (field.X + field.Width) - layout.CheckBounds.Width;
            }
            else if ((align & LayoutUtils.AnyCenter) != 0)
            {
                layout.CheckBounds.X = field.X + (field.Width - layout.CheckBounds.Width) / 2;
            }

            if ((align & LayoutUtils.AnyBottom) != 0)
            {
                layout.CheckBounds.Y = (field.Y + field.Height) - layout.CheckBounds.Height;
            }
            else if ((align & LayoutUtils.AnyTop) != 0)
            {
                layout.CheckBounds.Y = field.Y + 2; // + 2: this needs to be aligned to the text (
            }
            else
            {
                layout.CheckBounds.Y = field.Y + (field.Height - layout.CheckBounds.Height) / 2;
            }

            switch (align)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    layout.CheckArea.X = field.X;
                    layout.CheckArea.Width = checkSizeFull + 1;

                    layout.CheckArea.Y = field.Y;
                    layout.CheckArea.Height = field.Height;

                    layout.Field.X += checkSizeFull + 1;
                    layout.Field.Width -= checkSizeFull + 1;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    layout.CheckArea.X = field.X + field.Width - checkSizeFull;
                    layout.CheckArea.Width = checkSizeFull + 1;

                    layout.CheckArea.Y = field.Y;
                    layout.CheckArea.Height = field.Height;

                    layout.Field.Width -= checkSizeFull + 1;
                    break;
                case ContentAlignment.TopCenter:
                    layout.CheckArea.X = field.X;
                    layout.CheckArea.Width = field.Width;

                    layout.CheckArea.Y = field.Y;
                    layout.CheckArea.Height = checkSizeFull;

                    layout.Field.Y += checkSizeFull;
                    layout.Field.Height -= checkSizeFull;
                    break;

                case ContentAlignment.BottomCenter:
                    layout.CheckArea.X = field.X;
                    layout.CheckArea.Width = field.Width;

                    layout.CheckArea.Y = field.Y + field.Height - checkSizeFull;
                    layout.CheckArea.Height = checkSizeFull;

                    layout.Field.Height -= checkSizeFull;
                    break;

                case ContentAlignment.MiddleCenter:
                    layout.CheckArea = layout.CheckBounds;
                    break;
            }

            layout.CheckBounds.Width -= CheckPaddingSize;
            layout.CheckBounds.Height -= CheckPaddingSize;
        }

        /// <summary>
        ///  Maps an image align to the set of <see cref="Forms.TextImageRelation"/>s that represent the same edge.
        ///  For example, <see cref="ContentAlignment.TopLeft"/> maps to <see cref="TextImageRelation.ImageAboveText"/>
        ///  and <see cref="TextImageRelation.ImageBeforeText"/>.
        /// </summary>
        private static readonly TextImageRelation[] s_imageAlignToRelation =
        [
            TextImageRelation.ImageAboveText | TextImageRelation.ImageBeforeText,     // TopLeft
            TextImageRelation.ImageAboveText,                                         // TopCenter
            TextImageRelation.ImageAboveText | TextImageRelation.TextBeforeImage,     // TopRight
            0,                                                                        // Invalid
            TextImageRelation.ImageBeforeText,                                        // MiddleLeft
            0,                                                                        // MiddleCenter
            TextImageRelation.TextBeforeImage,                                        // MiddleRight
            0,                                                                        // Invalid
            TextImageRelation.TextAboveImage | TextImageRelation.ImageBeforeText,     // BottomLeft
            TextImageRelation.TextAboveImage,                                         // BottomCenter
            TextImageRelation.TextAboveImage | TextImageRelation.TextBeforeImage      // BottomRight
        ];

        private static TextImageRelation ImageAlignToRelation(ContentAlignment alignment)
            => s_imageAlignToRelation[LayoutUtils.ContentAlignmentToIndex(alignment)];

        private static TextImageRelation TextAlignToRelation(ContentAlignment alignment)
            => LayoutUtils.GetOppositeTextImageRelation(ImageAlignToRelation(alignment));

        internal void LayoutTextAndImage(LayoutData layout)
        {
            // Translate for Rtl applications. This shadows the member variables.
            ContentAlignment imageAlign = RtlTranslateContent(ImageAlign);
            ContentAlignment textAlign = RtlTranslateContent(TextAlign);
            TextImageRelation textImageRelation = RtlTranslateRelation(TextImageRelation);

            // Figure out the maximum bounds for text & image.
            Rectangle maxBounds = Rectangle.Inflate(layout.Field, -TextImageInset, -TextImageInset);
            if (OnePixExtraBorder)
            {
                maxBounds.Inflate(1, 1);
            }

            // Compute the final image and text bounds.
            if (ImageSize == Size.Empty || Text is null || Text.Length == 0 || textImageRelation == TextImageRelation.Overlay)
            {
                // Do not worry about text/image overlaying
                Size textSize = GetTextSize(maxBounds.Size);

                // For .NET Framework 1.1 compatibility.
                Size size = ImageSize;
                if (layout.Options.DotNetOneButtonCompat && ImageSize != Size.Empty)
                {
                    size = new Size(size.Width + 1, size.Height + 1);
                }

                layout.ImageBounds = LayoutUtils.Align(size, maxBounds, imageAlign);
                layout.TextBounds = LayoutUtils.Align(textSize, maxBounds, textAlign);
            }
            else
            {
                // Rearrange text/image to prevent overlay. Pack text into maxBounds - space reserved for image.
                Size maxTextSize = LayoutUtils.SubAlignedRegion(maxBounds.Size, ImageSize, textImageRelation);
                Size textSize = GetTextSize(maxTextSize);
                Rectangle maxCombinedBounds = maxBounds;

                // Combine text & image into one rectangle that we center within maxBounds.
                Size combinedSize = LayoutUtils.AddAlignedRegion(textSize, ImageSize, textImageRelation);
                maxCombinedBounds.Size = LayoutUtils.UnionSizes(maxCombinedBounds.Size, combinedSize);
                Rectangle combinedBounds = LayoutUtils.Align(combinedSize, maxCombinedBounds, ContentAlignment.MiddleCenter);

                // ImageEdge indicates whether the combination of ImageAlign and TextImageRelation place
                // the image along the edge of the control. If so, we can increase the space for text.
                bool imageEdge = (AnchorStyles)(ImageAlignToRelation(imageAlign) & textImageRelation) != AnchorStyles.None;

                // TextEdge indicates whether the combination of TextAlign and TextImageRelation place
                // the text along the edge of the control. If so, we can increase the space for image.
                bool textEdge = (AnchorStyles)(TextAlignToRelation(textAlign) & textImageRelation) != AnchorStyles.None;

                if (imageEdge)
                {
                    // Just split imageSize off of maxCombinedBounds.
                    LayoutUtils.SplitRegion(
                        maxCombinedBounds,
                        ImageSize,
                        (AnchorStyles)textImageRelation,
                        out layout.ImageBounds,
                        out layout.TextBounds);
                }
                else if (textEdge)
                {
                    // Just split textSize off of maxCombinedBounds.
                    LayoutUtils.SplitRegion(
                        maxCombinedBounds,
                        textSize,
                        (AnchorStyles)LayoutUtils.GetOppositeTextImageRelation(textImageRelation),
                        out layout.TextBounds,
                        out layout.ImageBounds);
                }
                else
                {
                    // Expand the adjacent regions to maxCombinedBounds (centered) and split the rectangle into
                    // imageBounds and textBounds.
                    LayoutUtils.SplitRegion(
                        combinedBounds,
                        ImageSize,
                        (AnchorStyles)textImageRelation,
                        out layout.ImageBounds,
                        out layout.TextBounds);
                    LayoutUtils.ExpandRegionsToFillBounds(
                        maxCombinedBounds,
                        (AnchorStyles)textImageRelation,
                        ref layout.ImageBounds,
                        ref layout.TextBounds);
                }

                // Align text/image within their regions.
                layout.ImageBounds = LayoutUtils.Align(ImageSize, layout.ImageBounds, imageAlign);
                layout.TextBounds = LayoutUtils.Align(textSize, layout.TextBounds, textAlign);
            }

            // Don't call "layout.imageBounds = Rectangle.Intersect(layout.imageBounds, maxBounds);"
            // because that is a breaking change that causes images to be scaled to the dimensions of the control.
            // adjust textBounds so that the text is still visible even if the image is larger than the button's size

            // Why do we intersect with layout.field for textBounds while we intersect with maxBounds for imageBounds?
            // this is because there are some legacy code which squeezes the button so small that text will get clipped
            // if we intersect with maxBounds. Have to do this for backward compatibility.

            if (textImageRelation is TextImageRelation.TextBeforeImage or TextImageRelation.ImageBeforeText)
            {
                // Adjust the vertical position of textBounds so that the text doesn't fall off the boundary of the button
                int textBottom = Math.Min(layout.TextBounds.Bottom, layout.Field.Bottom);
                layout.TextBounds.Y = Math.Max(
                    Math.Min(layout.TextBounds.Y, layout.Field.Y + (layout.Field.Height - layout.TextBounds.Height) / 2),
                    layout.Field.Y);
                layout.TextBounds.Height = textBottom - layout.TextBounds.Y;
            }

            if (textImageRelation is TextImageRelation.TextAboveImage or TextImageRelation.ImageAboveText)
            {
                // Adjust the horizontal position of textBounds so that the text doesn't fall off the boundary of the button
                int textRight = Math.Min(layout.TextBounds.Right, layout.Field.Right);
                layout.TextBounds.X = Math.Max(
                    Math.Min(layout.TextBounds.X, layout.Field.X + (layout.Field.Width - layout.TextBounds.Width) / 2),
                    layout.Field.X);
                layout.TextBounds.Width = textRight - layout.TextBounds.X;
            }

            if (textImageRelation == TextImageRelation.ImageBeforeText && layout.ImageBounds.Size.Width != 0)
            {
                // Squeezes imageBounds.Width so that text is visible
                layout.ImageBounds.Width = Math.Max(
                    0,
                    Math.Min(maxBounds.Width - layout.TextBounds.Width, layout.ImageBounds.Width));
                layout.TextBounds.X = layout.ImageBounds.X + layout.ImageBounds.Width;
            }

            if (textImageRelation == TextImageRelation.ImageAboveText && layout.ImageBounds.Size.Height != 0)
            {
                // Squeezes imageBounds.Height so that the text is visible
                layout.ImageBounds.Height = Math.Max(
                    0,
                    Math.Min(maxBounds.Height - layout.TextBounds.Height, layout.ImageBounds.Height));
                layout.TextBounds.Y = layout.ImageBounds.Y + layout.ImageBounds.Height;
            }

            // Make sure that textBound is contained in layout.field
            layout.TextBounds = Rectangle.Intersect(layout.TextBounds, layout.Field);
            if (HintTextUp)
            {
                layout.TextBounds.Y--;
            }

            if (TextOffset)
            {
                layout.TextBounds.Offset(1, 1);
            }

            // For .NET Framework 1.1 compatibility.
            if (layout.Options.DotNetOneButtonCompat)
            {
                layout.ImageStart = layout.ImageBounds.Location;
                layout.ImageBounds = Rectangle.Intersect(layout.ImageBounds, layout.Field);
            }
            else if (!Application.RenderWithVisualStyles)
            {
                // Not sure why this is here, but we can't remove it, since it might break
                // ToolStrips on non-themed machines
                layout.TextBounds.X++;
            }

            // Clip
            int bottom;

            // If we are using GDI to measure text, then we can get into a situation, where
            // the proposed height is ignore. In this case, we want to clip it against maxBounds.
            if (!UseCompatibleTextRendering)
            {
                bottom = Math.Min(layout.TextBounds.Bottom, maxBounds.Bottom);
                layout.TextBounds.Y = Math.Max(layout.TextBounds.Y, maxBounds.Y);
            }
            else
            {
                // If we are using GDI+ (like .NET Framework 1.1), then use the old code.
                // This ensures that we have pixel-level rendering compatibility.
                bottom = Math.Min(layout.TextBounds.Bottom, layout.Field.Bottom);
                layout.TextBounds.Y = Math.Max(layout.TextBounds.Y, layout.Field.Y);
            }

            layout.TextBounds.Height = bottom - layout.TextBounds.Y;
        }

        protected virtual Size GetTextSize(Size proposedSize)
        {
            // Set the Prefix field of TextFormatFlags
            proposedSize = LayoutUtils.FlipSizeIf(VerticalText, proposedSize);
            Size textSize = Size.Empty;

            if (UseCompatibleTextRendering)
            {
                // GDI+ text rendering.
                using var screen = GdiCache.GetScreenDCGraphics();
                using StringFormat stringFormat = StringFormat;
                textSize = Size.Ceiling(
                    screen.Graphics.MeasureString(Text, Font, new SizeF(proposedSize.Width, proposedSize.Height),
                    stringFormat));
            }
            else if (!string.IsNullOrEmpty(Text))
            {
                // GDI text rendering (.NET Framework 2.0 feature).
                textSize = TextRenderer.MeasureText(Text, Font, proposedSize, TextFormatFlags);
            }

            // Else skip calling MeasureText, it should return 0,0

            return LayoutUtils.FlipSizeIf(VerticalText, textSize);
        }

#if DEBUG
        public override string ToString() =>
            $$"""
                { client = {{Client}}
                OnePixExtraBorder = {{OnePixExtraBorder}}
                borderSize = {{BorderSize}}
                paddingSize = {{PaddingSize}}
                maxFocus = {{MaxFocus}}
                font = {{Font}}
                text = {{Text}}
                imageSize = {{ImageSize}}
                checkSize = {{CheckSize}}
                checkPaddingSize = {{CheckPaddingSize}}
                checkAlign = {{CheckAlign}}
                imageAlign = {{ImageAlign}}
                textAlign = {{TextAlign}}
                textOffset = {{TextOffset}}
                shadowedText = {{ShadowedText}}
                textImageRelation = {{TextImageRelation}}
                layoutRTL = {{LayoutRTL}} }
                """;
#endif
    }
}
