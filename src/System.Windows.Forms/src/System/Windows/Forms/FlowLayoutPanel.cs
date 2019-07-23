// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
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
            _flowLayoutSettings = new FlowLayoutSettings(this);
        }

        public override LayoutEngine LayoutEngine => FlowLayout.Instance;

        [SRDescription(nameof(SR.FlowPanelFlowDirectionDescr))]
        [DefaultValue(FlowDirection.LeftToRight)]
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        public FlowDirection FlowDirection
        {
            get => _flowLayoutSettings.FlowDirection;
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
            get => _flowLayoutSettings.WrapContents;
            set
            {
                _flowLayoutSettings.WrapContents = value;
                Debug.Assert(WrapContents == value, "WrapContents should be the same as we set it");
            }
        }

        bool IExtenderProvider.CanExtend(object obj) => obj is Control control && control.Parent == this;

        [DefaultValue(false)]
        [DisplayName("FlowBreak")]
        public bool GetFlowBreak(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            return _flowLayoutSettings.GetFlowBreak(control);
        }

        [DisplayName("FlowBreak")]
        public void SetFlowBreak(Control control, bool value)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            _flowLayoutSettings.SetFlowBreak(control, value);
        }
    }
}
