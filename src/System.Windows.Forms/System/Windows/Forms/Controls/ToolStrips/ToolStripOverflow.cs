// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public partial class ToolStripOverflow : ToolStripDropDown, IArrangedElement
{
    private readonly ToolStripOverflowButton? _ownerItem;

    public ToolStripOverflow(ToolStripItem parentItem)
        : base(parentItem)
    {
        ArgumentNullException.ThrowIfNull(parentItem);
        _ownerItem = parentItem as ToolStripOverflowButton;
    }

    protected internal override ToolStripItemCollection DisplayedItems
    {
        get
        {
            if (ParentToolStrip is not null)
            {
                ToolStripItemCollection items = ParentToolStrip.OverflowItems;
                return items;
            }

            return new ToolStripItemCollection(owner: null, itemsCollection: false);
        }
    }

    public override ToolStripItemCollection Items
    {
        get
        {
            return new ToolStripItemCollection(owner: null, itemsCollection: false, isReadOnly: true);
        }
    }

    private ToolStrip? ParentToolStrip
    {
        get
        {
            if (_ownerItem is not null)
            {
                return _ownerItem.ParentToolStrip;
            }

            return null;
        }
    }

    ArrangedElementCollection IArrangedElement.Children
    {
        get { return DisplayedItems; }
    }

    IArrangedElement? IArrangedElement.Container
    {
        get { return ParentInternal; }
    }

    bool IArrangedElement.ParticipatesInLayout
    {
        get { return GetState(States.Visible); }
    }

    PropertyStore IArrangedElement.Properties
    {
        get { return Properties; }
    }

    void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified)
    {
        SetBoundsCore(bounds.X, bounds.Y, bounds.Width, bounds.Height, specified);
    }

    public override LayoutEngine LayoutEngine
    {
        get
        {
            return FlowLayout.Instance;
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new ToolStripOverflowAccessibleObject(this);
    }

    public override Size GetPreferredSize(Size constrainingSize)
    {
        constrainingSize.Width = 200;
        return base.GetPreferredSize(constrainingSize);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        if (ParentToolStrip is not null && ParentToolStrip.IsInDesignMode)
        {
            if (FlowLayout.GetFlowDirection(this) != FlowDirection.TopDown)
            {
                FlowLayout.SetFlowDirection(this, FlowDirection.TopDown);
            }

            if (FlowLayout.GetWrapContents(this))
            {
                FlowLayout.SetWrapContents(this, false);
            }
        }
        else
        {
            if (FlowLayout.GetFlowDirection(this) != FlowDirection.LeftToRight)
            {
                FlowLayout.SetFlowDirection(this, FlowDirection.LeftToRight);
            }

            if (!FlowLayout.GetWrapContents(this))
            {
                FlowLayout.SetWrapContents(this, true);
            }
        }

        base.OnLayout(e);
    }

    protected override void SetDisplayedItems()
    {
        // do nothing here.... this is really for the setting the overflow/displayed items on the
        // main ToolStrip. Our working item collection is our displayed item collection... calling
        // base would clear it out.
        Size biggestItemSize = Size.Empty;
        for (int j = 0; j < DisplayedItems.Count; j++)
        {
            ToolStripItem item = DisplayedItems[j];
            if (((IArrangedElement)item).ParticipatesInLayout)
            {
                HasVisibleItems = true;
                biggestItemSize = LayoutUtils.UnionSizes(biggestItemSize, item.Bounds.Size);
            }
        }

        SetLargestItemSize(biggestItemSize);
    }
}
