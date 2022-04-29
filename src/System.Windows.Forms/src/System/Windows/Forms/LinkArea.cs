// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms
{
    [TypeConverter(typeof(LinkAreaConverter))]
    [Serializable] // This type is participating in resx serialization scenarios.
    public partial struct LinkArea : IEquatable<LinkArea>
    {
#pragma warning disable IDE1006
        private int start; // Do NOT rename (binary serialization).
        private int length; // Do NOT rename (binary serialization).
#pragma warning restore IDE1006

        public LinkArea(int start, int length)
        {
            this.start = start;
            this.length = length;
        }

        public int Start
        {
            get => start;
            set => start = value;
        }

        public int Length
        {
            get => length;
            set => length = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEmpty => length == start && start == 0;

        public override bool Equals(object o)
        {
            if (o is not LinkArea a)
            {
                return false;
            }

            return this == a;
        }

        public bool Equals(LinkArea other)
            => other.Start == start && other.Length == length;

        public override string ToString() => $"{{Start={Start}, Length={Length}}}";

        public static bool operator ==(LinkArea linkArea1, LinkArea linkArea2)
        {
            return linkArea1.start == linkArea2.start && linkArea1.length == linkArea2.length;
        }

        public static bool operator !=(LinkArea linkArea1, LinkArea linkArea2)
        {
            return !(linkArea1 == linkArea2);
        }

        public override int GetHashCode() => HashCode.Combine(start, length);
    }
}
