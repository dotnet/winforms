﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms;

public partial class LinkLabel
{
    internal class LinkLabelAccessibleObject : LabelAccessibleObject
    {
        private int[]? _runtimeId;

        public LinkLabelAccessibleObject(LinkLabel owner) : base(owner)
        {
        }

        internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            => !this.IsOwnerHandleCreated(out LinkLabel? owner)
                ? base.ElementProviderFromPoint(x, y)
                : HitTest((int)x, (int)y) ?? base.ElementProviderFromPoint(x, y);

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            => direction switch
            {
                UiaCore.NavigateDirection.FirstChild
                    => !this.TryGetOwnerAs(out LinkLabel? owner) ? null : owner.Links.Count != 0
                        ? owner.Links[0].AccessibleObject
                        : null,
                UiaCore.NavigateDirection.LastChild
                    => !this.TryGetOwnerAs(out LinkLabel? owner) ? null : owner.Links.Count != 0
                        ? owner.Links[^1].AccessibleObject
                        : null,
                _ => base.FragmentNavigate(direction),
            };

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

        public override AccessibleObject? GetChild(int index)
        {
            if (this.TryGetOwnerAs(out LinkLabel? owner) && index >= 0 && index < GetChildCount())
            {
                return owner.Links[index].AccessibleObject;
            }

            return null;
        }

        public override int GetChildCount() => this.TryGetOwnerAs(out LinkLabel? owner) ? owner.Links.Count : 0;

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
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

        internal override int[] RuntimeId
            => _runtimeId ??= !this.TryGetOwnerAs(out LinkLabel? owner) ? base.RuntimeId : new int[]
            {
                RuntimeIDFirstItem,
                PARAM.ToInt(owner.InternalHandle),
                owner.GetHashCode()
            };
    }
}
