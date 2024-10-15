// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class handles all design time behavior for the group box class. Group
///  boxes may contain sub-components and therefore use the frame designer.
/// </summary>
internal class GroupBoxDesigner : ParentControlDesigner
{
    private InheritanceUI _inheritanceUI;

    /// <summary>
    /// Determines the default location for a control added to this designer.
    /// it is usually (0,0), but may be modified if the container has special borders, etc.
    /// </summary>
    protected override Point DefaultControlLocation
    {
        get
        {
            GroupBox gb = (GroupBox)Control;
            return new Point(gb.DisplayRectangle.X, gb.DisplayRectangle.Y);
        }
    }

#if TESTVALUEUI
    /// <summary>
    ///  Initializes the designer with the given component. The designer can
    ///  get the component's site and request services from it in this call.
    /// </summary>
    public override void Initialize(IComponent component) {
        base.Initialize(component);
        
        AutoResizeHandles = true;
        
        IPropertyValueUIService pvUISvc = (IPropertyValueUIService)component.Site.GetService(typeof(IPropertyValueUIService));
        
        if (pvUISvc != null) {
            pvUISvc.AddPropertyValueUIHandler(new PropertyValueUIHandler(this.OnGetUIValueItem));
        }
    }
    
    private void OnGetUIValueItem(object component, PropertyDescriptor propDesc, ArrayList valueUIItemList){
    
        if (propDesc.PropertyType == typeof(string)) {
            Bitmap bmp = new(typeof(GroupBoxDesigner), "BoundProperty.bmp");
            bmp.MakeTransparent();
            valueUIItemList.Add(new LocalUIItem(bmp, new PropertyValueUIItemInvokeHandler(this.OnPropertyValueUIItemInvoke), "Data Can"));
            
            //bmp = new Bitmap("BoundProperty.bmp");
            valueUIItemList.Add(new LocalUIItem(bmp, new PropertyValueUIItemInvokeHandler(this.OnPropertyValueUIItemInvoke), "Little Button"));
        }
        
        
    }

    private void OnPropertyValueUIItemInvoke(ITypeDescriptorContext context, PropertyDescriptor descriptor, PropertyValueUIItem invokedItem) {
        Debug.Fail("propertyuivalue '" + invokedItem.ToolTip + "' invoked");
    }
    
#endif

    /// <summary>
    ///  We override this because even though we still want to
    ///  offset our grid for our display rectangle, we still want
    ///  to align to our parent's grid - so we don't look funny
    /// </summary>
    protected override void OnPaintAdornments(PaintEventArgs pe)
    {
        if (DrawGrid)
        {
            Control control = Control;
            Rectangle rectangle = Control.DisplayRectangle;

            rectangle.Width++; // gpr: FillRectangle with a TextureBrush comes up one pixel short
            rectangle.Height++;
            ControlPaint.DrawGrid(pe.Graphics, rectangle, GridSize, control.BackColor);
        }

        // If this control is being inherited, paint it
        //
        if (Inherited)
        {
            _inheritanceUI ??= (InheritanceUI)GetService(typeof(InheritanceUI));

            if (_inheritanceUI is not null)
            {
                pe.Graphics.DrawImage(InheritanceUI.InheritanceGlyph, 0, 0);
            }
        }
    }

    /// <summary>
    ///  We override our base class's WndProc here because
    ///  the group box always returns HTTRANSPARENT. This
    ///  causes the mouse to go "through" the group box, but
    ///  that's not what we want at design time.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case (int)PInvokeCore.WM_NCHITTEST:
                // The group box always fires HTTRANSPARENT, which causes the message to go to our parent. We want
                // the group box's designer to get these messages, however, so change this.
                base.WndProc(ref m);

                if (m.ResultInternal == PInvoke.HTTRANSPARENT)
                {
                    m.ResultInternal = (LRESULT)(nint)PInvoke.HTCLIENT;
                }

                break;

            default:
                base.WndProc(ref m);
                break;
        }
    }

#if TESTVALUEUI
    
    internal class LocalUIItem : PropertyValueUIItem {
        private string itemName;
        
        public LocalUIItem(Image img, PropertyValueUIItemInvokeHandler handler, string itemName) : base(img, handler, itemName) {
            this.itemName = itemName;
        }
    }
#endif
}
