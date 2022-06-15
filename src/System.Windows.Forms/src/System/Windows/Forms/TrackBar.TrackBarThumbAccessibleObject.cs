// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class TrackBar
    {
        internal class TrackBarThumbAccessibleObject : TrackBarChildAccessibleObject
        {
            public TrackBarThumbAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
            { }

            public override string? Name => SR.TrackBarPositionButtonName;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!OwningTrackBar.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.PreviousSibling => ParentInternal.FirstButtonAccessibleObject.IsDisplayed ? ParentInternal.FirstButtonAccessibleObject : null,
                    UiaCore.NavigateDirection.NextSibling => ParentInternal.LastButtonAccessibleObject.IsDisplayed ? ParentInternal.LastButtonAccessibleObject : null,
                    _ => base.FragmentNavigate(direction)
                };
            }

            internal override int GetChildId() => 2;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ThumbControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.InvokePatternId => false,
                    _ => base.IsPatternSupported(patternId)
                };
        }
    }
}
