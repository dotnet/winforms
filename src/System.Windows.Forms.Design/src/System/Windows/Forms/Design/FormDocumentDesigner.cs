// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  The FormDocumentDesigner class builds on the DocumentDesigner. It adds shadowing for form properties that need
///  to be shadowed and it also adds logic to properly paint the form's title bar to match the active document window.
/// </summary>
internal class FormDocumentDesigner : DocumentDesigner
{
    private Size _autoScaleBaseSize = Size.Empty;
    private bool _initializing;
    private bool _autoSize;
    private ToolStripAdornerWindowService _toolStripAdornerWindowService;

    /// <summary>
    ///  Shadow the AcceptButton property at design-time so that we can preserve it when the form is rebuilt.
    ///  Otherwise, form.Controls.Clear() will clear it out when we don't want it to.
    /// </summary>
    private IButtonControl AcceptButton
    {
        get => ShadowProperties[nameof(AcceptButton)] as IButtonControl;
        set
        {
            ((Form)Component).AcceptButton = value;
            ShadowProperties[nameof(AcceptButton)] = value;
        }
    }

    /// <summary>
    ///  Shadow the CancelButton property at design-time so that we can preserve it when the form is rebuilt.
    ///  Otherwise, form.Controls.Clear() will clear it out when we don't want it to.
    /// </summary>
    private IButtonControl CancelButton
    {
        get => ShadowProperties[nameof(CancelButton)] as IButtonControl;
        set
        {
            ((Form)Component).CancelButton = value;
            ShadowProperties[nameof(CancelButton)] = value;
        }
    }

    /// <summary>
    ///  Shadowed version of the AutoScaleBaseSize property. We shadow this so that it always persists. Normally
    ///  only properties that differ from the default values at instantiation are persisted, but this should always
    ///  be written. So, we shadow it and add our own ShouldSerialize method.
    /// </summary>
    private Size AutoScaleBaseSize
    {
        get
        {
            // we don't want to get inherited value from a base form that might have been designed in a different
            // DPI so we recalculate the thing instead of getting AutoScaleBaseSize (QFE 2280)
            SizeF real = Form.GetAutoScaleSize(((Form)Component).Font);
            return new Size((int)Math.Round(real.Width), (int)Math.Round(real.Height));
        }
        set
        {
            // We do nothing at design time for this property; we always want to use the calculated value from the component.
            _autoScaleBaseSize = value;
            ShadowProperties[nameof(AutoScaleBaseSize)] = value;
        }
    }

    /// <summary>
    ///  We shadow the AutoSize property at design-time so that the form doesn't grow and shrink as users fiddle
    ///  with autosize related properties.
    /// </summary>
    private bool AutoSize
    {
        get => _autoSize;
        set => _autoSize = value;
    }

    private bool ShouldSerializeAutoScaleBaseSize()
    {
        // Never serialize this unless AutoScale is turned on
        return !_initializing && ((Form)Component).AutoScale && ShadowProperties.Contains(nameof(AutoScaleBaseSize));
    }

    /// <summary>
    ///  Shadow property for the ClientSize property -- this allows us to intercept client size changes and apply
    ///  the new menu height if necessary
    /// </summary>
    private Size ClientSize
    {
        get
        {
            if (_initializing)
            {
                return new Size(-1, -1);
            }
            else
            {
                Size size = new(-1, -1);
                if (Component is Form form)
                {
                    size = form.ClientSize;

                    // Don't report the size decremented by the scroll bars, otherwise, we'll just lose that size
                    // when we run because the form doesn't take that into consideration (it's too early, it hasn't
                    // layed out and doesn't know it needs scrollbars) when sizing.
                    if (form.HorizontalScroll.Visible)
                    {
                        size.Height += SystemInformation.HorizontalScrollBarHeight;
                    }

                    if (form.VerticalScroll.Visible)
                    {
                        size.Width += SystemInformation.VerticalScrollBarWidth;
                    }
                }

                return size;
            }
        }
        set
        {
            GetService<IDesignerHost>();
            ((Form)Component).ClientSize = value;
        }
    }

    /// <summary>
    ///  Shadow property for the IsMDIContainer property on a form.
    /// </summary>
    private bool IsMdiContainer
    {
        get => ((Form)Control).IsMdiContainer;
        set
        {
            if (!value)
            {
                UnhookChildControls(Control);
            }

            ((Form)Control).IsMdiContainer = value;
            if (value)
            {
                HookChildControls(Control);
            }
        }
    }

    /// <summary>
    ///  Opacity property on control. We shadow this property at design time.
    /// </summary>
    private double Opacity
    {
        get => (double)ShadowProperties[nameof(Opacity)];
        set
        {
            if (value is < (double)0.0f or > (double)1.0f)
            {
                throw new ArgumentException(
                    string.Format(
                        SR.InvalidBoundArgument,
                        "value",
                        value.ToString(CultureInfo.CurrentCulture),
                        (0.0f).ToString(CultureInfo.CurrentCulture),
                        (1.0f).ToString(CultureInfo.CurrentCulture)),
                    nameof(value));
            }

            ShadowProperties[nameof(Opacity)] = value;
        }
    }

    /// <summary>
    ///  Overrides the default implementation of ParentControlDesigner SnapLines. Note that if the Padding property
    ///  is not set on our Form - we'll special case this and add default Padding values to our SnapLines. This was
    ///  a usability request specific to the Form itself. Note that a Form only has Padding SnapLines.
    /// </summary>
    public override IList SnapLines
    {
        get
        {
            IList<SnapLine> snapLines = null;
            AddPaddingSnapLines(ref snapLines);
            if (snapLines is null)
            {
                Debug.Fail("why did base.AddPaddingSnapLines return null?");
                snapLines = new List<SnapLine>(4);
            }

            // if the padding has not been set - then we'll auto-add padding to form - this is a Usability request
            if (Control.Padding == Padding.Empty && snapLines is not null)
            {
                int paddingsFound = 0; // used to short-circuit once we find 4 paddings
                for (int i = 0; i < snapLines.Count; i++)
                {
                    // remove previous padding snaplines
                    if (snapLines[i] is SnapLine snapLine && snapLine.Filter is not null && snapLine.Filter.StartsWith(SnapLine.Padding, StringComparison.Ordinal))
                    {
                        if (snapLine.Filter.Equals(SnapLine.PaddingLeft) || snapLine.Filter.Equals(SnapLine.PaddingTop))
                        {
                            snapLine.AdjustOffset(DesignerUtils.s_defaultFormPadding);
                            paddingsFound++;
                        }

                        if (snapLine.Filter.Equals(SnapLine.PaddingRight) || snapLine.Filter.Equals(SnapLine.PaddingBottom))
                        {
                            snapLine.AdjustOffset(-DesignerUtils.s_defaultFormPadding);
                            paddingsFound++;
                        }

                        if (paddingsFound == 4)
                        {
                            break; // we adjusted all of our paddings
                        }
                    }
                }
            }

            return (IList)snapLines;
        }
    }

    private Size Size
    {
        get => Control.Size;
        set
        {
            IComponentChangeService changeService = GetService<IComponentChangeService>();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Component);
            changeService?.OnComponentChanging(Component, props["ClientSize"]);

            Control.Size = value;
            changeService?.OnComponentChanged(Component, props["ClientSize"]);
        }
    }

    /// <summary>
    ///  Accessor method for the showInTaskbar property on control. We shadow this property at design time.
    /// </summary>
    private bool ShowInTaskbar
    {
        get => (bool)ShadowProperties[nameof(ShowInTaskbar)];
        set => ShadowProperties[nameof(ShowInTaskbar)] = value;
    }

    /// <summary>
    ///  Accessor method for the windowState property on control. We shadow this property at design time.
    /// </summary>
    private FormWindowState WindowState
    {
        get => (FormWindowState)ShadowProperties[nameof(WindowState)];
        set => ShadowProperties[nameof(WindowState)] = value;
    }

    private static void ApplyAutoScaling(SizeF baseVar, Form form)
    {
        // We also don't do this if the property is empty. Otherwise we will perform two GetAutoScaleBaseSize calls
        // only to find that they returned the same value.
        if (!baseVar.IsEmpty)
        {
            SizeF newVarF = Form.GetAutoScaleSize(form.Font);
            Size newVar = new((int)Math.Round(newVarF.Width), (int)Math.Round(newVarF.Height));
            // We save a significant amount of time by bailing early if there's no work to be done
            if (baseVar.Equals(newVar))
            {
                return;
            }

            float percY = newVar.Height / ((float)baseVar.Height);
            float percX = newVar.Width / ((float)baseVar.Width);
            form.Scale(percX, percY);
        }
    }

    /// <summary>
    ///  Disposes of this designer.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            IDesignerHost host = GetService<IDesignerHost>();
            Debug.Assert(host is not null, "Must have a designer host on dispose");

            if (host is not null)
            {
                host.LoadComplete -= OnLoadComplete;
                host.Activated -= OnDesignerActivate;
                host.Deactivated -= OnDesignerDeactivate;
            }

            IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (cs is not null)
            {
                cs.ComponentAdded -= OnComponentAdded;
                cs.ComponentRemoved -= OnComponentRemoved;
            }
        }

        base.Dispose(disposing);
    }

    private void EnsureToolStripWindowAdornerService()
    {
        _toolStripAdornerWindowService ??= GetService<ToolStripAdornerWindowService>();
    }

    /// <summary>
    ///  Initializes the designer with the given component. The designer can get the component's site and request
    ///  services from it in this call.
    /// </summary>
    public override void Initialize(IComponent component)
    {
        // We have to shadow the WindowState before we call base.Initialize
        PropertyDescriptor windowStateProp = TypeDescriptor.GetProperties(component.GetType())["WindowState"];
        if (windowStateProp is not null && windowStateProp.PropertyType == typeof(FormWindowState))
        {
            WindowState = (FormWindowState)windowStateProp.GetValue(component);
        }

        _initializing = true;
        base.Initialize(component);
        _initializing = false;
        AutoResizeHandles = true;

        Debug.Assert(component is Form, "FormDocumentDesigner expects its component to be a form.");

        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is not null)
        {
            host.LoadComplete += OnLoadComplete;
            host.Activated += OnDesignerActivate;
            host.Deactivated += OnDesignerDeactivate;
        }

        Form form = (Form)Control;
        form.WindowState = FormWindowState.Normal;
        ShadowProperties[nameof(AcceptButton)] = form.AcceptButton;
        ShadowProperties[nameof(CancelButton)] = form.CancelButton;

        // Monitor component/remove add events for our tray
        IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        if (cs is not null)
        {
            cs.ComponentAdded += OnComponentAdded;
            cs.ComponentRemoved += OnComponentRemoved;
        }
    }

    /// <summary>
    ///  Called when a component is added to the design container. If the component isn't a control, this will
    ///  demand create the component tray and add the component to it.
    /// </summary>
    private void OnComponentAdded(object source, ComponentEventArgs ce)
    {
        if (ce.Component is ToolStrip && _toolStripAdornerWindowService is null && TryGetService(out IDesignerHost _))
        {
            EnsureToolStripWindowAdornerService();
        }
    }

    /// <summary>
    ///  Called when a component is removed from the design container. Here, we check if a menu is being removed
    ///  and handle removing the Form's mainmenu vs. other menus properly.
    /// </summary>
    private void OnComponentRemoved(object source, ComponentEventArgs ce)
    {
        if (ce.Component is ToolStrip && _toolStripAdornerWindowService is not null)
        {
            _toolStripAdornerWindowService = null;
        }

        if (ce.Component is IButtonControl)
        {
            if (ce.Component == ShadowProperties[nameof(AcceptButton)])
            {
                AcceptButton = null;
            }

            if (ce.Component == ShadowProperties[nameof(CancelButton)])
            {
                CancelButton = null;
            }
        }
    }

    // Called when our document becomes active. We paint our form's border the appropriate color here.
    private unsafe void OnDesignerActivate(object source, EventArgs evevent)
    {
        // Paint the form's title bar UI-active
        if (Control is { } control && control.IsHandleCreated)
        {
            PInvokeCore.SendMessage(control, PInvokeCore.WM_NCACTIVATE, (WPARAM)(BOOL)true);
            PInvoke.RedrawWindow(control, lprcUpdate: null, HRGN.Null, REDRAW_WINDOW_FLAGS.RDW_FRAME);
        }
    }

    /// <summary>
    ///  Called by the host when we become inactive. Here we update the title bar of our form so it's the inactive color.
    /// </summary>
    private unsafe void OnDesignerDeactivate(object sender, EventArgs e)
    {
        if (Control is { } control && control.IsHandleCreated)
        {
            PInvokeCore.SendMessage(control, PInvokeCore.WM_NCACTIVATE, (WPARAM)(BOOL)false);
            PInvoke.RedrawWindow(control, lprcUpdate: null, HRGN.Null, REDRAW_WINDOW_FLAGS.RDW_FRAME);
        }
    }

    /// <summary>
    ///  Called when our code loads. Here we connect us as the selection UI handler for ourselves. This is a
    ///  special case because for the top level document, we are our own selection UI handler.
    /// </summary>
    private void OnLoadComplete(object source, EventArgs evevent)
    {
        if (Control is Form form)
        {
            // The form's ClientSize is reported including the ScrollBar's height. We need to account for this in
            // order to display the form with scrollbars correctly.
            int clientWidth = form.ClientSize.Width;
            int clientHeight = form.ClientSize.Height;
            if (form.HorizontalScroll.Visible && form.AutoScroll)
            {
                clientHeight += SystemInformation.HorizontalScrollBarHeight;
            }

            if (form.VerticalScroll.Visible && form.AutoScroll)
            {
                clientWidth += SystemInformation.VerticalScrollBarWidth;
            }

            // ApplyAutoScaling causes WmWindowPosChanging to be called and there we calculate if we need to
            // compensate for a menu being visible we were causing that calculation to fail if we set ClientSize
            // too early. We now do the right thing and check again if we need to compensate for the menu.
            ApplyAutoScaling(_autoScaleBaseSize, form);
            ClientSize = new Size(clientWidth, clientHeight);
            GetService<BehaviorService>()?.SyncSelection();

            form.PerformLayout();
        }
    }

    /// <summary>
    ///  Allows a designer to filter the set of properties the component it is designing
    ///  will expose through the TypeDescriptor object. This method is called immediately before
    ///  its corresponding "Post" method. If you are overriding this method you should
    ///  call the base implementation before you perform your own filtering.
    /// </summary>
    protected override void PreFilterProperties(IDictionary properties)
    {
        PropertyDescriptor prop;
        base.PreFilterProperties(properties);
        // Handle shadowed properties
        string[] shadowProps = ["Opacity", "IsMdiContainer", "Size", "ShowInTaskBar", "WindowState", "AutoSize", "AcceptButton", "CancelButton"];
        Attribute[] empty = [];
        for (int i = 0; i < shadowProps.Length; i++)
        {
            prop = (PropertyDescriptor)properties[shadowProps[i]];
            if (prop is not null)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(FormDocumentDesigner), prop, empty);
            }
        }

        // Mark auto scale base size as serializable again so we can monitor it for backwards compatibility.
        prop = (PropertyDescriptor)properties["AutoScaleBaseSize"];
        if (prop is not null)
        {
            properties["AutoScaleBaseSize"] = TypeDescriptor.CreateProperty(typeof(FormDocumentDesigner), prop, DesignerSerializationVisibilityAttribute.Visible);
        }

        // And set the new default value attribute for client base size, and shadow it as well.
        prop = (PropertyDescriptor)properties["ClientSize"];
        if (prop is not null)
        {
            properties["ClientSize"] = TypeDescriptor.CreateProperty(typeof(FormDocumentDesigner), prop, new DefaultValueAttribute(new Size(-1, -1)));
        }
    }
}
