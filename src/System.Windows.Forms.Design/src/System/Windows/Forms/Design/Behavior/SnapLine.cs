// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///     The SnapLine class represents a UI-guideline that will be rendered
    ///     during control movement (drag, keyboard, and resize) operations.
    ///     SnapLines will assist a user in aligning controls relative to one
    ///     one another.  Each SnapLine will have a type: top, bottom, etc...
    ///     Only SnapLines of like-types are allowed to align with each other.
    ///     The 'offset' will represent the distance from the origin (upper-left
    ///     corner) of the control to where the SnapLine is located.  And finally
    ///     the 'filter' is a string used to define custome types of SnapLines.
    ///     This enables a SnapLine with a filter of "TypeX" to only snap to
    ///     other "TypeX" filtered lines.
    /// </summary>
    public sealed class SnapLine
    {
        /// <summary>
        ///     SnapLine constructor that takes the type and offset of SnapLine.
        /// </summary>
        public SnapLine(SnapLineType type, int offset) : this(type, offset, null, SnapLinePriority.Low)
        {
        }

        /// <summary>
        ///     SnapLine constructor that takes the type, offset and filter of SnapLine.
        /// </summary>
        public SnapLine(SnapLineType type, int offset, string filter) : this(type, offset, filter, SnapLinePriority.Low)
        {
        }

        /// <summary>
        ///     SnapLine constructor that takes the type, offset, and priority of SnapLine.
        /// </summary>
        public SnapLine(SnapLineType type, int offset, SnapLinePriority priority) : this(type, offset, null, priority)
        {
        }

        /// <summary>
        ///     SnapLine constructor that takes the type, offset, filter, and priority of the SnapLine.
        /// </summary>
        public SnapLine(SnapLineType type, int offset, string filter, SnapLinePriority priority)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This property returns a string representing an optional user-defined filter.
        ///     Setting this filter will allow only those SnapLines with similar filters to align
        ///     to one another.
        /// </summary>
        public string Filter => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Returns true if the SnapLine is of a horizontal type.
        /// </summary>
        public bool IsHorizontal => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Returns true if the SnapLine is of a vertical type.
        /// </summary>
        public bool IsVertical => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Read-only property that returns the distance from the origin to where this SnapLine is defined.
        /// </summary>
        public int Offset => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Read-only property that returns the priority of the SnapLine.
        /// </summary>
        public SnapLinePriority Priority => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Read-only property that represents the 'type' of SnapLine.
        /// </summary>
        public SnapLineType SnapLineType => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Adjusts the offset property of the SnapLine.
        /// </summary>
        public void AdjustOffset(int adjustment)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns true if SnapLine s1 should snap to SnapLine s2.
        /// </summary>
        public static bool ShouldSnap(SnapLine line1, SnapLine line2)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     ToString implementation for SnapLines.
        /// </summary>
        public override string ToString()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
