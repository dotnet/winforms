// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class LinkLabel
    {
        internal class LinkLabelAccessibleObject : LabelAccessibleObject
        {
            public LinkLabelAccessibleObject(LinkLabel owner) : base(owner)
            {
            }

            internal override bool IsIAccessibleExSupported() => true;

            public override AccessibleObject? GetChild(int index)
            {
                if (index >= 0 && index < ((LinkLabel)Owner).Links.Count)
                {
                    return new LinkAccessibleObject(((LinkLabel)Owner).Links[index]);
                }
                else
                {
                    return null;
                }
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.IsEnabledPropertyId)
                {
                    if (!Owner.Enabled)
                    {
                        return false;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!Owner.IsHandleCreated)
                {
                    return null;
                }

                Point p = Owner.PointToClient(new Point(x, y));
                Link hit = ((LinkLabel)Owner).PointInLink(p.X, p.Y);

                if (hit is not null)
                {
                    return new LinkAccessibleObject(hit);
                }

                if (Bounds.Contains(x, y))
                {
                    return this;
                }

                return null;
            }

            /// <summary>
            /// </summary>
            public override int GetChildCount()
            {
                return ((LinkLabel)Owner).Links.Count;
            }
        }
    }
}
