// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms.Layout;

    /// <include file='doc\FlowLayoutSettings.uex' path='docs/doc[@for="FlowLayoutSettings"]/*' />
    [DefaultProperty(nameof(FlowDirection))]
    public class FlowLayoutSettings : LayoutSettings {

        internal FlowLayoutSettings(IArrangedElement owner) : base(owner) {}
        
        /// <include file='doc\FlowLayoutSettings.uex' path='docs/doc[@for="FlowLayoutSettings.LayoutEngine"]/*' />
        public override LayoutEngine LayoutEngine {
            get { return FlowLayout.Instance; }
        }

        /// <include file='doc\FlowLayoutSettings.uex' path='docs/doc[@for="FlowLayoutSettings.FlowDirection"]/*' />
        [SRDescription(nameof(SR.FlowPanelFlowDirectionDescr))]
        [DefaultValue(FlowDirection.LeftToRight)]
        [SRCategory(nameof(SR.CatLayout))]
        public FlowDirection FlowDirection {
            get { return FlowLayout.GetFlowDirection(Owner); }
            set { 
                FlowLayout.SetFlowDirection(Owner, value);
                Debug.Assert(FlowDirection == value, "FlowDirection should be the same as we set it");
            }
        }
        
        /// <include file='doc\FlowLayoutSettings.uex' path='docs/doc[@for="FlowLayoutSettings.WrapContents"]/*' />
        [SRDescription(nameof(SR.FlowPanelWrapContentsDescr))]
        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatLayout))]
        public bool WrapContents {
            get { return FlowLayout.GetWrapContents(Owner); }
            set { 
                FlowLayout.SetWrapContents(Owner, value);
                Debug.Assert(WrapContents == value, "WrapContents should be the same as we set it");
            }
        }

        public void SetFlowBreak(object child, bool value) {
            IArrangedElement element = FlowLayout.Instance.CastToArrangedElement(child);
            if (GetFlowBreak(child) != value) {
                CommonProperties.SetFlowBreak(element, value);
            }
        }

        public bool GetFlowBreak(object child) {
            IArrangedElement element = FlowLayout.Instance.CastToArrangedElement(child);
            return CommonProperties.GetFlowBreak(element);
        }

        
    }
}

