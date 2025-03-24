// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms;

public partial class ToolStripItem
{
    /// <summary>
    ///  This class helps determine where the image and text should be drawn.
    /// </summary>
    internal partial class ToolStripItemInternalLayout
    {
        private ToolStripItemLayoutOptions? _currentLayoutOptions;
        private readonly ToolStripItem _ownerItem;
        private ButtonBaseAdapter.LayoutData? _layoutData;
        private const int BorderWidth = 2;
        private static readonly Size s_invalidSize = new(int.MinValue, int.MinValue);

        private Size _lastPreferredSize = s_invalidSize;
        private ToolStripLayoutData? _parentLayoutData;

        public ToolStripItemInternalLayout(ToolStripItem ownerItem)
        {
            _ownerItem = ownerItem.OrThrowIfNull();
        }

        protected virtual ToolStripItem Owner => _ownerItem;

        public virtual Rectangle ImageRectangle
        {
            get
            {
                ButtonBaseAdapter.LayoutData layoutData = LayoutData;
                Rectangle imageRect = layoutData.ImageBounds;
                imageRect.Intersect(layoutData.Field);
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

        protected virtual ToolStrip? ParentInternal => _ownerItem?.ParentInternal;

        public virtual Rectangle TextRectangle
        {
            get
            {
                ButtonBaseAdapter.LayoutData layoutData = LayoutData;
                Rectangle textRect = layoutData.TextBounds;
                textRect.Intersect(layoutData.Field);
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

        internal static TextFormatFlags ContentAlignmentToTextFormat(ContentAlignment alignment, bool rightToLeft)
        {
            TextFormatFlags textFormat = TextFormatFlags.Default;
            if (rightToLeft)
            {
                // We specifically do not want to turn on TextFormatFlags.Right.
                textFormat |= TextFormatFlags.RightToLeft;
            }

            // Calculate Text Positioning
            textFormat |= ControlPaint.ConvertAlignmentToTextFormat(alignment);
            return textFormat;
        }

        protected virtual ToolStripItemLayoutOptions CommonLayoutOptions()
        {
            ToolStripItemLayoutOptions layoutOptions = new();
            Rectangle bounds = new(Point.Empty, _ownerItem.Size);

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
            layoutOptions.LayoutRTL = Owner.RightToLeft == RightToLeft.Yes;
            layoutOptions.TextImageRelation = Owner.TextImageRelation;
            // Set textImageInset to 0 since we don't draw 3D border for ToolStripItems.
            layoutOptions.TextImageInset = 0;
            layoutOptions.DotNetOneButtonCompat = false;

            // Support RTL
            layoutOptions.GdiTextFormatFlags = ContentAlignmentToTextFormat(Owner.TextAlign, Owner.RightToLeft == RightToLeft.Yes);

            // Hide underlined &File unless ALT is pressed
            layoutOptions.GdiTextFormatFlags = Owner.ShowKeyboardCues ? layoutOptions.GdiTextFormatFlags : layoutOptions.GdiTextFormatFlags | TextFormatFlags.HidePrefix;

            return layoutOptions;
        }

        [MemberNotNull(nameof(_layoutData))]
        private void EnsureLayout()
        {
            if (_layoutData is null || _parentLayoutData is null || !_parentLayoutData.IsCurrent(ParentInternal))
            {
                PerformLayout();
            }
        }

        [MemberNotNull(nameof(_currentLayoutOptions))]
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
            EnsureLayout();
            // we would prefer not to be larger than the ToolStrip itself.
            // so we'll ask the ButtonAdapter layout guy what it thinks
            // its preferred size should be - and we'll tell it to be no
            // bigger than the ToolStrip itself. Note this is "Parent" not
            // "Owner" because we care in this instance what we're currently displayed on.

            if (_ownerItem is not null)
            {
                // _currentLayoutOptions will always be initialised if EnsureLayout() is called,
                // because it will get called at least once (for _layoutData is null) and, in turn,
                // it'll invoke PerformLayout() that'll unconditionally invoke GetLayoutData().
                _lastPreferredSize = _currentLayoutOptions!.GetPreferredSizeCore(constrainingSize);
                return _lastPreferredSize;
            }

            return Size.Empty;
        }

        [MemberNotNull(nameof(_layoutData))]
        [MemberNotNull(nameof(_currentLayoutOptions))]
        internal void PerformLayout()
        {
            _layoutData = GetLayoutData();
            ToolStrip? parent = ParentInternal;
            if (parent is not null)
            {
                _parentLayoutData = new ToolStripLayoutData(parent);
            }
            else
            {
                _parentLayoutData = null;
            }
        }
    }
}
