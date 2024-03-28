// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal sealed partial class SelectionUIService
{
    private readonly struct HitTestInfo : IEquatable<HitTestInfo>
    {
        public readonly int hitTest;
        public readonly SelectionUIItem? selectionUIHit;
        public readonly bool containerSelector;

        public HitTestInfo(int hitTest, SelectionUIItem? selectionUIHit)
        {
            this.hitTest = hitTest;
            this.selectionUIHit = selectionUIHit;
        }

        public HitTestInfo(int hitTest, SelectionUIItem selectionUIHit, bool containerSelector)
        {
            this.hitTest = hitTest;
            this.selectionUIHit = selectionUIHit;
            this.containerSelector = containerSelector;
        }

        // Standard 'catch all - rethrow critical' exception pattern
        public override bool Equals(object? obj)
        {
            try
            {
                return Equals((HitTestInfo)obj!);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
            }

            return false;
        }

        public bool Equals(HitTestInfo other)
            => hitTest == other.hitTest
                && selectionUIHit == other.selectionUIHit
                && containerSelector == other.containerSelector;

        public static bool operator ==(HitTestInfo left, HitTestInfo right) => left.Equals(right);

        public static bool operator !=(HitTestInfo left, HitTestInfo right) => !left.Equals(right);

        public override int GetHashCode()
        {
            int hash = hitTest | (selectionUIHit?.GetHashCode() ?? 0);
            if (containerSelector)
            {
                hash |= 0x10000;
            }

            return hash;
        }
    }
}
