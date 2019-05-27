// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms.Layout;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ProvideProperty("FlowBreak", typeof(Control))]
    [DefaultProperty(nameof(FlowDirection))]
    [Designer("System.Windows.Forms.Design.FlowLayoutPanelDesigner, " + AssemblyRef.SystemDesign)]
    [Docking(DockingBehavior.Ask)]
    [SRDescription(nameof(SR.DescriptionFlowLayoutPanel))]
    public class FlowLayoutPanel : Panel, IExtenderProvider
    {
        private readonly FlowLayoutSettings _flowLayoutSettings;

        public FlowLayoutPanel()
        {
            _flowLayoutSettings = FlowLayout.CreateSettings(this);
        }

        public override LayoutEngine LayoutEngine
        {
            get { return FlowLayout.Instance; }
        }

        [SRDescription(nameof(SR.FlowPanelFlowDirectionDescr))]
        [DefaultValue(FlowDirection.LeftToRight)]
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        public FlowDirection FlowDirection
        {
            get { return _flowLayoutSettings.FlowDirection; }
            set
            {
                _flowLayoutSettings.FlowDirection = value;
                Debug.Assert(FlowDirection == value, "FlowDirection should be the same as we set it");
            }
        }

        [SRDescription(nameof(SR.FlowPanelWrapContentsDescr))]
        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        public bool WrapContents
        {
            get { return _flowLayoutSettings.WrapContents; }
            set
            {
                _flowLayoutSettings.WrapContents = value;
                Debug.Assert(WrapContents == value, "WrapContents should be the same as we set it");
            }
        }

        #region Provided properties
        bool IExtenderProvider.CanExtend(object obj)
        {
            return obj is Control control && control.Parent == this;
        }

        [DefaultValue(false)]
        [DisplayName("FlowBreak")]
        public bool GetFlowBreak(Control control)
        {
            return _flowLayoutSettings.GetFlowBreak(control);
        }

        [DisplayName("FlowBreak")]
        public void SetFlowBreak(Control control, bool value)
        {
            _flowLayoutSettings.SetFlowBreak(control, value);
        }

        #endregion
    }
}

