﻿// Licensed to the .NET Foundation under one or more agreements.
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
            private readonly static Size s_invalidSize = new Size(int.MinValue, int.MinValue);

            private Size _lastPreferredSize = s_invalidSize;
            private ToolStripLayoutData _parentLayoutData;

            public ToolStripItemInternalLayout(ToolStripItem ownerItem)
            {
                _ownerItem = ownerItem ?? throw new ArgumentNullException(nameof(ownerItem));
            }

            protected virtual ToolStripItem Owner => _ownerItem;

            public virtual Rectangle ImageRectangle
            {
                get
                {
                    Rectangle imageRect = LayoutData.ImageBounds;
                    imageRect.Intersect(_layoutData.Field);
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

            public Size PreferredImageSize => Owner.PreferredImageSize;

            protected virtual ToolStrip ParentInternal => _ownerItem?.ParentInternal;

            public virtual Rectangle TextRectangle
            {
                get
                {
                    Rectangle textRect = LayoutData.TextBounds;
                    textRect.Intersect(_layoutData.Field);
                    return textRect;
                }
            }

            public virtual Rectangle ContentRectangle => LayoutData.Field;

            public virtual TextFormatFlags TextFormat
            {
                get
                {
                    if (_currentLayoutOptions is not null)
                    {
                        return _currentLayoutOptions.GdiTextFormatFlags;
                    }

                    return CommonLayoutOptions().GdiTextFormatFlags;
                }
            }

            internal static TextFormatFlags ContentAlignToTextFormat(ContentAlignment alignment, bool rightToLeft)
            {
                TextFormatFlags textFormat = TextFormatFlags.Default;
                if (rightToLeft)
                {
                    // We specifically do not want to turn on TextFormatFlags.Right.
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

                layoutOptions.Client = bounds;

                layoutOptions.GrowBorderBy1PxWhenDefault = false;

                layoutOptions.BorderSize = BorderWidth;
                layoutOptions.PaddingSize = 0;
                layoutOptions.MaxFocus = true;
                layoutOptions.FocusOddEvenFixup = false;
                layoutOptions.Font = _ownerItem.Font;
                layoutOptions.Text = ((Owner.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text) ? Owner.Text : string.Empty;
                layoutOptions.ImageSize = PreferredImageSize;
                layoutOptions.CheckSize = 0;
                layoutOptions.CheckPaddingSize = 0;
                layoutOptions.CheckAlign = ContentAlignment.TopLeft;
                layoutOptions.ImageAlign = Owner.ImageAlign;
                layoutOptions.TextAlign = Owner.TextAlign;
                layoutOptions.HintTextUp = false;
                layoutOptions.ShadowedText = !_ownerItem.Enabled;
                layoutOptions.LayoutRTL = RightToLeft.Yes == Owner.RightToLeft;
                layoutOptions.TextImageRelation = Owner.TextImageRelation;
                // Set textImageInset to 0 since we don't draw 3D border for ToolStripItems.
                layoutOptions.TextImageInset = 0;
                layoutOptions.DotNetOneButtonCompat = false;

                // Support RTL
                layoutOptions.GdiTextFormatFlags = ContentAlignToTextFormat(Owner.TextAlign, Owner.RightToLeft == RightToLeft.Yes);

                // Hide underlined &File unless ALT is pressed
                layoutOptions.GdiTextFormatFlags = (Owner.ShowKeyboardCues) ? layoutOptions.GdiTextFormatFlags : layoutOptions.GdiTextFormatFlags | TextFormatFlags.HidePrefix;

                return layoutOptions;
            }

            private bool EnsureLayout()
            {
                if (_layoutData is null || _parentLayoutData is null || !_parentLayoutData.IsCurrent(ParentInternal))
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
                    _currentLayoutOptions.VerticalText = true;
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

                if (_ownerItem is not null)
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
                if (parent is not null)
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
                private Size _cachedSize = LayoutUtils.s_invalidSize;
                private Size _cachedProposedConstraints = LayoutUtils.s_invalidSize;

                // override GetTextSize to provide simple text caching.
                protected override Size GetTextSize(Size proposedConstraints)
                {
                    if (_cachedSize != LayoutUtils.s_invalidSize
                        && (_cachedProposedConstraints == proposedConstraints
                        || _cachedSize.Width <= proposedConstraints.Width))
                    {
                        return _cachedSize;
                    }

                    _cachedSize = base.GetTextSize(proposedConstraints);
                    _cachedProposedConstraints = proposedConstraints;
                    return _cachedSize;
                }
            }

            private class ToolStripLayoutData
            {
                private readonly ToolStripLayoutStyle _layoutStyle;
                private readonly bool _autoSize;
                private Size _size;

                public ToolStripLayoutData(ToolStrip toolStrip)
                {
                    _layoutStyle = toolStrip.LayoutStyle;
                    _autoSize = toolStrip.AutoSize;
                    _size = toolStrip.Size;
                }

                public bool IsCurrent(ToolStrip toolStrip)
                    => toolStrip is not null && toolStrip.Size == _size && toolStrip.LayoutStyle == _layoutStyle && toolStrip.AutoSize == _autoSize;
            }
        }
    }
}
