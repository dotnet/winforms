// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

internal sealed class ToolStripPanelSelectionGlyph : ControlBodyGlyph
{
    private readonly ToolStripPanel? _relatedPanel;
    private readonly BehaviorService? _behaviorService;
    private Rectangle _glyphBounds;
    private Bitmap? _image;
    private Control? _baseParent;
    private bool _isExpanded;

    private const int ImageWidthOriginal = 50;
    private const int ImageHeightOriginal = 6;

    private readonly int _imageWidth = ImageWidthOriginal;
    private readonly int _imageHeight = ImageHeightOriginal;

    internal ToolStripPanelSelectionGlyph(Rectangle bounds, Cursor cursor, IComponent relatedComponent, IServiceProvider? _provider, ToolStripPanelSelectionBehavior behavior) : base(bounds, cursor, relatedComponent, behavior)
    {
        _relatedPanel = relatedComponent as ToolStripPanel;
        _behaviorService = _provider?.GetService<BehaviorService>();
        if (_behaviorService is null)
        {
            return;
        }

        IDesignerHost? host = _provider?.GetService<IDesignerHost>();
        if (host is null)
        {
            return;
        }

        UpdateGlyph();
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (value != _isExpanded)
            {
                _isExpanded = value;
                UpdateGlyph();
            }
        }
    }

    public void UpdateGlyph()
    {
        if (_behaviorService is null)
        {
            return;
        }

        Rectangle translatedBounds = _behaviorService.ControlRectInAdornerWindow(_relatedPanel!);
        // Reset the glyph.
        _glyphBounds = Rectangle.Empty;

        // Refresh the parent
        ToolStripContainer? parent = _relatedPanel?.Parent as ToolStripContainer;
        if (parent is not null)
        {
            // get the control to which ToolStripContainer is added.
            _baseParent = parent.Parent;
        }

        if (_image is not null)
        {
            _image.Dispose();
            _image = null;
        }

        if (!_isExpanded)
        {
            CollapseGlyph(translatedBounds);
        }
        else
        {
            ExpandGlyph(translatedBounds);
        }
    }

    private void SetBitmap(string fileName)
    {
        _image = new Bitmap(typeof(ToolStripPanelSelectionGlyph), fileName);
        _image.MakeTransparent(Color.Magenta);
        _image = ScaleHelper.ScaleToDpi(_image, ScaleHelper.InitialSystemDpi, disposeBitmap: true);
    }

    private void CollapseGlyph(Rectangle bounds)
    {
        DockStyle? dock = _relatedPanel?.Dock;
        int x;
        int y;

        switch (dock)
        {
            case DockStyle.Top:
                SetBitmap("TopOpen");
                x = (bounds.Width - _imageWidth) / 2;
                if (x > 0)
                {
                    _glyphBounds = new Rectangle(bounds.X + x, bounds.Y + bounds.Height, _imageWidth, _imageHeight);
                }

                break;
            case DockStyle.Bottom:
                SetBitmap("BottomOpen");
                x = (bounds.Width - _imageWidth) / 2;
                if (x > 0)
                {
                    _glyphBounds = new Rectangle(bounds.X + x, bounds.Y - _imageHeight, _imageWidth, _imageHeight);
                }

                break;
            case DockStyle.Left:
                SetBitmap("LeftOpen");
                y = (bounds.Height - _imageWidth) / 2;
                if (y > 0)
                {
                    _glyphBounds = new Rectangle(bounds.X + bounds.Width, bounds.Y + y, _imageHeight, _imageWidth);
                }

                break;
            case DockStyle.Right:
                SetBitmap("RightOpen");
                y = (bounds.Height - _imageWidth) / 2;
                if (y > 0)
                {
                    _glyphBounds = new Rectangle(bounds.X - _imageHeight, bounds.Y + y, _imageHeight, _imageWidth);
                }

                break;
            default:
                throw new NotSupportedException(SR.ToolStripPanelGlyphUnsupportedDock);
        }
    }

    private void ExpandGlyph(Rectangle bounds)
    {
        DockStyle? dock = _relatedPanel?.Dock;
        int x;
        int y;

        switch (dock)
        {
            case DockStyle.Top:
                SetBitmap("TopClose");
                x = (bounds.Width - _imageWidth) / 2;
                if (x > 0)
                {
                    _glyphBounds = new Rectangle(bounds.X + x, bounds.Y + bounds.Height, _imageWidth, _imageHeight);
                }

                break;
            case DockStyle.Bottom:
                SetBitmap("BottomClose");
                x = (bounds.Width - _imageWidth) / 2;
                if (x > 0)
                {
                    _glyphBounds = new Rectangle(bounds.X + x, bounds.Y - _imageHeight, _imageWidth, _imageHeight);
                }

                break;
            case DockStyle.Left:
                SetBitmap("LeftClose");
                y = (bounds.Height - _imageWidth) / 2;
                if (y > 0)
                {
                    _glyphBounds = new Rectangle(bounds.X + bounds.Width, bounds.Y + y, _imageHeight, _imageWidth);
                }

                break;
            case DockStyle.Right:
                SetBitmap("RightClose");
                y = (bounds.Height - _imageWidth) / 2;
                if (y > 0)
                {
                    _glyphBounds = new Rectangle(bounds.X - _imageHeight, bounds.Y + y, _imageHeight, _imageWidth);
                }

                break;
            default:
                throw new InvalidOperationException(SR.ToolStripPanelGlyphUnsupportedDock);
        }
    }

    /// <summary>
    ///  The bounds of this Glyph.
    /// </summary>
    public override Rectangle Bounds => _glyphBounds;

    /// <summary>
    ///  Simple hit test rule: if the point is contained within the bounds
    ///  then it is a positive hit test.
    /// </summary>
    public override Cursor? GetHitTest(Point p)
    {
        if (_behaviorService is null || _baseParent is null)
        {
            return null;
        }

        Rectangle baseParentBounds = _behaviorService.ControlRectInAdornerWindow(_baseParent);
        return _glyphBounds != Rectangle.Empty && baseParentBounds.Contains(_glyphBounds) && _glyphBounds.Contains(p) ? Cursors.Hand : null;
    }

    /// <summary>
    ///  Very simple paint logic.
    /// </summary>
    public override void Paint(PaintEventArgs pe)
    {
        if (_behaviorService is null || _baseParent is null)
        {
            return;
        }

        Rectangle baseParentBounds = _behaviorService.ControlRectInAdornerWindow(_baseParent);
        if (_relatedPanel!.Visible && _image is not null && _glyphBounds != Rectangle.Empty && baseParentBounds.Contains(_glyphBounds))
        {
            pe.Graphics.DrawImage(_image, _glyphBounds.Left, _glyphBounds.Top);
        }
    }
}
