// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    [DefaultProperty(nameof(FlowDirection))]
    public class FlowLayoutSettings : LayoutSettings
    {
        internal FlowLayoutSettings(IArrangedElement owner) : base(owner)
        {
        }

        public override LayoutEngine LayoutEngine => FlowLayout.Instance;

        [SRDescription(nameof(SR.FlowPanelFlowDirectionDescr))]
        [DefaultValue(FlowDirection.LeftToRight)]
        [SRCategory(nameof(SR.CatLayout))]
        public FlowDirection FlowDirection
        {
            get => FlowLayout.GetFlowDirection(Owner);
            set
            {
                FlowLayout.SetFlowDirection(Owner, value);
                Debug.Assert(FlowDirection == value, "FlowDirection should be the same as we set it");
            }
        }

        [SRDescription(nameof(SR.FlowPanelWrapContentsDescr))]
        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatLayout))]
        public bool WrapContents
        {
            get => FlowLayout.GetWrapContents(Owner);
            set
            {
                FlowLayout.SetWrapContents(Owner, value);
                Debug.Assert(WrapContents == value, "WrapContents should be the same as we set it");
            }
        }

        public void SetFlowBreak(object child, bool value)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            IArrangedElement element = FlowLayout.Instance.CastToArrangedElement(child);
            if (GetFlowBreak(child) != value)
            {
                CommonProperties.SetFlowBreak(element, value);
            }
        }

        public bool GetFlowBreak(object child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            IArrangedElement element = FlowLayout.Instance.CastToArrangedElement(child);
            return CommonProperties.GetFlowBreak(element);
        }
    }
}
