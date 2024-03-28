// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal partial class SplitContainerDesigner
{
    /// <summary>
    ///  This class is used to provide the horizontal or vertical splitter orientation action items.
    /// </summary>
    private class OrientationActionList : DesignerActionList
    {
        private string? _actionName;
        private readonly SplitContainerDesigner _owner;
        private readonly Component? _ownerComponent;

        public OrientationActionList(SplitContainerDesigner owner)
            : base(owner.Component)
        {
            _owner = owner;
            _ownerComponent = owner.Component as Component;

            if (_ownerComponent is not null)
            {
                PropertyDescriptor? orientationProp = TypeDescriptor.GetProperties(_ownerComponent)["Orientation"];
                if (orientationProp is not null)
                {
                    bool needsVertical = ((Orientation)orientationProp.GetValue(_ownerComponent)!) == Orientation.Horizontal;
                    _actionName = needsVertical ? SR.DesignerShortcutVerticalOrientation : SR.DesignerShortcutHorizontalOrientation;
                }
            }
        }

        private void OnOrientationActionClick(object? sender, EventArgs e)
        {
            if (sender is not DesignerVerb verb)
            {
                return;
            }

            Orientation orientation = verb.Text.Equals(SR.DesignerShortcutHorizontalOrientation) ? Orientation.Horizontal : Orientation.Vertical;

            // switch the text of the orientation action from vertical to horizontal or visa-versa
            _actionName = (orientation == Orientation.Horizontal) ? SR.DesignerShortcutVerticalOrientation : SR.DesignerShortcutHorizontalOrientation;

            // get the prop and actually modify the orientation
            PropertyDescriptor? orientationProp = TypeDescriptor.GetProperties(_ownerComponent!)["Orientation"];
            if (orientationProp is not null && ((Orientation)orientationProp.GetValue(_ownerComponent)!) != orientation)
            {
                orientationProp.SetValue(_ownerComponent, orientation);
            }

            DesignerActionUIService actionUIService = _owner.GetRequiredService<DesignerActionUIService>();
            actionUIService.Refresh(_ownerComponent);
        }

        public override DesignerActionItemCollection GetSortedActionItems() =>
        [
            new DesignerActionVerbItem(new DesignerVerb(_actionName!, OnOrientationActionClick))
        ];
    }
}
