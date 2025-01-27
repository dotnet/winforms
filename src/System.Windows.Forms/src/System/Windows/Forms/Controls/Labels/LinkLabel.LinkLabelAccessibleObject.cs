// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class LinkLabel
{
    internal sealed class LinkLabelAccessibleObject : LabelAccessibleObject
    {
        private int[]? _runtimeId;

        public LinkLabelAccessibleObject(LinkLabel owner) : base(owner)
        {
        }

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
            => !this.IsOwnerHandleCreated(out LinkLabel? _)
                ? base.ElementProviderFromPoint(x, y)
                : HitTest((int)x, (int)y) ?? base.ElementProviderFromPoint(x, y);

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild
                    => !this.TryGetOwnerAs(out LinkLabel? owner) ? null : owner.Links.Count != 0
                        ? owner.Links[0].AccessibleObject
                        : null,
                NavigateDirection.NavigateDirection_LastChild
                    => !this.TryGetOwnerAs(out LinkLabel? owner) ? null : owner.Links.Count != 0
                        ? owner.Links[^1].AccessibleObject
                        : null,
                _ => base.FragmentNavigate(direction),
            };

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

        public override AccessibleObject? GetChild(int index) =>
            this.TryGetOwnerAs(out LinkLabel? owner) && index >= 0 && index < GetChildCount()
                ? owner.Links[index].AccessibleObject
                : null;

        private protected override bool IsInternal => true;

        public override int GetChildCount() => this.TryGetOwnerAs(out LinkLabel? owner) ? owner.Links.Count : 0;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };

        public override AccessibleObject? HitTest(int x, int y)
        {
            if (!this.IsOwnerHandleCreated(out LinkLabel? owner))
            {
                return null;
            }

            Point p = owner.PointToClient(new Point(x, y));
            Link? hit = owner.PointInLink(p.X, p.Y);

            if (hit is not null)
            {
                return hit.AccessibleObject;
            }

            return Bounds.Contains(x, y) ? this : null;
        }

        internal override bool IsIAccessibleExSupported() => true;

        internal override int[] RuntimeId => _runtimeId ??= base.RuntimeId;
    }
}
