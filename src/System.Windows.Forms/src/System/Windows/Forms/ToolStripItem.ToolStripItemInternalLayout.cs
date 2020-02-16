// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    public partial class ToolStripItem
    {
        /// <summary>
        ///  This class helps determine where the image and text should be drawn.
        /// </summary>
        internal class ToolStripItemInternalLayout
        {
            private ToolStripItemLayoutOptions _currentLayoutOptions;
            private readonly ToolStripItem _ownerItem;
            private ButtonBaseAdapter.LayoutData _layoutData;
            private const int BorderWidth = 2;
            private const int BorderHeight = 3;
            private readonly static Size s_invalidSize = new Size(int.MinValue, int.MinValue);

            private Size _lastPreferredSize = s_invalidSize;
            private ToolStripLayoutData _parentLayoutData;

            public ToolStripItemInternalLayout(ToolStripItem ownerItem)
            {
                this._ownerItem = ownerItem ?? throw new ArgumentNullException(nameof(ownerItem));
            }

            // the thing that we fetch properties off of -- this can be different than ownerItem - e.g. case of split button.
            protected virtual ToolStripItem Owner
            {
                get { return _ownerItem; }
            }

            public virtual Rectangle ImageRectangle
            {
                get
                {
                    Rectangle imageRect = LayoutData.imageBounds;
                    imageRect.Intersect(_layoutData.field);
                    return imageRect;
                }
            }

            internal ButtonBaseAdapter.LayoutData LayoutData
            {
                get
                {
                    EnsureLayout();
                    return _layoutData;
                }
            }

            public Size PreferredImageSize
            {
                get
                {
                    return Owner.PreferredImageSize;
                }
            }

            protected virtual ToolStrip ParentInternal
            {
                get
                {
                    return _ownerItem?.ParentInternal;
                }
            }

            public virtual Rectangle TextRectangle
            {
                get
                {
                    Rectangle textRect = LayoutData.textBounds;
                    textRect.Intersect(_layoutData.field);
                    return textRect;
                }
            }

            public virtual Rectangle ContentRectangle
            {
                get
                {
                    return LayoutData.field;
                }
            }

            public virtual TextFormatFlags TextFormat
            {
                get
                {
                    if (_currentLayoutOptions != null)
                    {
                        return _currentLayoutOptions.gdiTextFormatFlags;
                    }
                    return CommonLayoutOptions().gdiTextFormatFlags;
                }
            }

            internal static TextFormatFlags ContentAlignToTextFormat(ContentAlignment alignment, bool rightToLeft)
            {
                TextFormatFlags textFormat = TextFormatFlags.Default;
                if (rightToLeft)
                {
                    //We specifically do not want to turn on TextFormatFlags.Right.
                    textFormat |= TextFormatFlags.RightToLeft;
                }

                // Calculate Text Positioning
                textFormat |= ControlPaint.TranslateAlignmentForGDI(alignment);
                textFormat |= ControlPaint.TranslateLineAlignmentForGDI(alignment);
                return textFormat;
            }

            protected virtual ToolStripItemLayoutOptions CommonLayoutOptions()
            {
                ToolStripItemLayoutOptions layoutOptions = new ToolStripItemLayoutOptions();
                Rectangle bounds = new Rectangle(Point.Empty, _ownerItem.Size);

                layoutOptions.client = bounds;

                layoutOptions.growBorderBy1PxWhenDefault = false;

                layoutOptions.borderSize = BorderWidth;
                layoutOptions.paddingSize = 0;
                layoutOptions.maxFocus = true;
                layoutOptions.focusOddEvenFixup = false;
                layoutOptions.font = _ownerItem.Font;
                layoutOptions.text = ((Owner.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text) ? Owner.Text : string.Empty;
                layoutOptions.imageSize = PreferredImageSize;
                layoutOptions.checkSize = 0;
                layoutOptions.checkPaddingSize = 0;
                layoutOptions.checkAlign = ContentAlignment.TopLeft;
                layoutOptions.imageAlign = Owner.ImageAlign;
                layoutOptions.textAlign = Owner.TextAlign;
                layoutOptions.hintTextUp = false;
                layoutOptions.shadowedText = !_ownerItem.Enabled;
                layoutOptions.layoutRTL = RightToLeft.Yes == Owner.RightToLeft;
                layoutOptions.textImageRelation = Owner.TextImageRelation;
                //set textImageInset to 0 since we don't draw 3D border for ToolStripItems.
                layoutOptions.textImageInset = 0;
                layoutOptions.everettButtonCompat = false;

                // Support RTL
                layoutOptions.gdiTextFormatFlags = ContentAlignToTextFormat(Owner.TextAlign, Owner.RightToLeft == RightToLeft.Yes);

                // Hide underlined &File unless ALT is pressed
                layoutOptions.gdiTextFormatFlags = (Owner.ShowKeyboardCues) ? layoutOptions.gdiTextFormatFlags : layoutOptions.gdiTextFormatFlags | TextFormatFlags.HidePrefix;

                return layoutOptions;
            }

            private bool EnsureLayout()
            {
                if (_layoutData == null || _parentLayoutData == null || !_parentLayoutData.IsCurrent(ParentInternal))
                {
                    PerformLayout();
                    return true;
                }
                return false;
            }

            private ButtonBaseAdapter.LayoutData GetLayoutData()
            {
                _currentLayoutOptions = CommonLayoutOptions();

                if (Owner.TextDirection != ToolStripTextDirection.Horizontal)
                {
                    _currentLayoutOptions.verticalText = true;
                }

                ButtonBaseAdapter.LayoutData data = _currentLayoutOptions.Layout();
                return data;
            }
            public virtual Size GetPreferredSize(Size constrainingSize)
            {
                Size preferredSize = Size.Empty;
                EnsureLayout();
                // we would prefer not to be larger than the ToolStrip itself.
                // so we'll ask the ButtonAdapter layout guy what it thinks
                // its preferred size should be - and we'll tell it to be no
                // bigger than the ToolStrip itself.  Note this is "Parent" not
                // "Owner" because we care in this instance what we're currently displayed on.

                if (_ownerItem != null)
                {
                    _lastPreferredSize = _currentLayoutOptions.GetPreferredSizeCore(constrainingSize);
                    return _lastPreferredSize;
                }
                return Size.Empty;
            }

            internal void PerformLayout()
            {
                _layoutData = GetLayoutData();
                ToolStrip parent = ParentInternal;
                if (parent != null)
                {
                    _parentLayoutData = new ToolStripLayoutData(parent);
                }
                else
                {
                    _parentLayoutData = null;
                }
            }

            internal class ToolStripItemLayoutOptions : ButtonBaseAdapter.LayoutOptions
            {
                Size cachedSize = LayoutUtils.InvalidSize;
                Size cachedProposedConstraints = LayoutUtils.InvalidSize;

                // override GetTextSize to provide simple text caching.
                protected override Size GetTextSize(Size proposedConstraints)
                {
                    if (cachedSize != LayoutUtils.InvalidSize
                        && (cachedProposedConstraints == proposedConstraints
                        || cachedSize.Width <= proposedConstraints.Width))
                    {
                        return cachedSize;
                    }
                    else
                    {
                        cachedSize = base.GetTextSize(proposedConstraints);
                        cachedProposedConstraints = proposedConstraints;
                    }
                    return cachedSize;
                }
            }
            private class ToolStripLayoutData
            {
                private readonly ToolStripLayoutStyle layoutStyle;
                private readonly bool autoSize;
                private Size size;

                public ToolStripLayoutData(ToolStrip toolStrip)
                {
                    layoutStyle = toolStrip.LayoutStyle;
                    autoSize = toolStrip.AutoSize;
                    size = toolStrip.Size;
                }
                public bool IsCurrent(ToolStrip toolStrip)
                {
                    if (toolStrip == null)
                    {
                        return false;
                    }
                    return (toolStrip.Size == size && toolStrip.LayoutStyle == layoutStyle && toolStrip.AutoSize == autoSize);
                }
            }
            }
    }
}
