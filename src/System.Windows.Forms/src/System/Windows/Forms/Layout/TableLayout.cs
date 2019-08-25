// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Layout
{
    internal class TableLayout : LayoutEngine
    {
        // This code was taken from ndp\clr\src\BCL\System\Array.cs
        // This changeset replaced the sorting algorithm when elements with the same
        // value are sorted. While Array.Sort() was documented as not a being stable sort,
        // we need to preserve the order of same elements as it used to be with the old algorithm since
        // some customers are putting more than one control in the same TableLayout cell
        // and we rely on this order to resovle the conflict

        private static int GetMedian(int low, int hi)
        {
            // Note both may be negative, if we are dealing with arrays w/ negative lower bounds.
            return low + ((hi - low) >> 1);
        }

        // Private value type used by the Sort methods.
        private struct SorterObjectArray
        {
            private readonly object[] keys;
            private readonly IComparer comparer;

            internal SorterObjectArray(object[] keys, IComparer comparer)
            {
                if (comparer == null)
                {
                    comparer = Comparer.Default;
                }

                this.keys = keys;
                this.comparer = comparer;
            }

            internal void SwapIfGreaterWithItems(int a, int b)
            {
                if (a != b)
                {
                    try
                    {
                        if (comparer.Compare(keys[a], keys[b]) > 0)
                        {
                            object temp = keys[a];
                            keys[a] = keys[b];
                            keys[b] = temp;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new ArgumentException();
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            internal void QuickSort(int left, int right)
            {
                // Can use the much faster jit helpers for array access.
                do
                {
                    int i = left;
                    int j = right;

                    // pre-sort the low, middle (pivot), and high values in place.
                    // this improves performance in the face of already sorted data, or
                    // data that is made up of multiple sorted runs appended together.
                    int middle = GetMedian(i, j);
                    SwapIfGreaterWithItems(i, middle); // swap the low with the mid point
                    SwapIfGreaterWithItems(i, j);      // swap the low with the high
                    SwapIfGreaterWithItems(middle, j); // swap the middle with the high

                    object x = keys[middle];
                    do
                    {
                        // Add a try block here to detect IComparers (or their
                        // underlying IComparables, etc) that are bogus.
                        try
                        {
                            while (comparer.Compare(keys[i], x) < 0)
                            {
                                i++;
                            }

                            while (comparer.Compare(x, keys[j]) < 0)
                            {
                                j--;
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            throw new ArgumentException();
                        }
                        catch (Exception)
                        {
                            throw new InvalidOperationException();
                        }
                        if (i > j)
                        {
                            break;
                        }

                        if (i < j)
                        {
                            object key = keys[i];
                            keys[i] = keys[j];
                            keys[j] = key;
                        }
                        i++;
                        j--;
                    } while (i <= j);
                    if (j - left <= right - i)
                    {
                        if (left < j)
                        {
                            QuickSort(left, j);
                        }

                        left = i;
                    }
                    else
                    {
                        if (i < right)
                        {
                            QuickSort(i, right);
                        }

                        right = j;
                    }
                } while (left < right);
            }
        }

        private static void Sort(object[] array, IComparer comparer)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Length > 1)
            {
                SorterObjectArray sorter = new SorterObjectArray(array, comparer);
                sorter.QuickSort(0, array.Length - 1);
            }
        }

        // End of sorting code

        // Singleton instance shared by all containers.
        internal static readonly TableLayout Instance = new TableLayout();

        private static readonly int _containerInfoProperty = PropertyStore.CreateKey();
        private static readonly int _layoutInfoProperty = PropertyStore.CreateKey();

        private static readonly string[] _propertiesWhichInvalidateCache = new string[] {
           //suspend layout before changing one of the above property will cause the AffectedProperty of LayoutEventArgs to be set to null
           //for more information, see http://wiki/default.aspx/Microsoft.Projects.DotNetClient.LayoutEventArgs

           null,
           PropertyNames.ChildIndex,   // Changing Z-order changes the row/column assignments
           PropertyNames.Parent,        // So does adding/removing controls
           PropertyNames.Visible,      // as well as visibility
           PropertyNames.Items,        // Changing toolstrip items collection
           PropertyNames.Rows,
           PropertyNames.Columns,
           PropertyNames.RowStyles,
           PropertyNames.ColumnStyles,
           // RowSpan, ColumnSpan, TableIndex manually call ClearCachedAssignments.
        };

        internal static TableLayoutSettings CreateSettings(IArrangedElement owner)
        {
            return new TableLayoutSettings(owner);
        }

        internal override void ProcessSuspendedLayoutEventArgs(IArrangedElement container, LayoutEventArgs args)
        {
            ContainerInfo containerInfo = GetContainerInfo(container);

            foreach (string propertyName in _propertiesWhichInvalidateCache)
            {
                if (Object.ReferenceEquals(args.AffectedProperty, propertyName))
                {
                    ClearCachedAssignments(containerInfo);
                    break;
                }
            }
        }

        /// <summary>
        ///  LayoutCore: EntryPoint from LayoutEngine.
        ///  Container: IArrangedElement to layout (could be table layout panel but doesnt have to be - eg. ToolStrip)
        ///  LayoutEventArgs: args created from PerformLayout.
        ///
        ///  Summary of algorithm:
        ///  (1).  Determine the row and column assignments of all the children of the container. (This can be cached)
        ///  (2).  Apply column styles, then row styles for all the rows:
        ///  (a).  Create a list of column or row sizes (Strip[]) - initialize absolute columns/rows sizes.
        ///  (b).  Determine the minimum size of all the controls
        ///  (c).  Determine the maximum size of all the controls
        ///  (d).  Distribute the minimum size of the control to the corresponding Strip for column or row
        ///  (e).  Distribute the remaining size to the column or row as according to the Row/Column style.
        ///  (3).  Expand the last row/column to fit the table
        ///  (4).  Set the bounds of the child elements as according to the row/column heights specified in Strip[]
        ///  (a)   Calculate bounds of item
        ///  (b)   Align and stretch item to fill column/row as according to Dock&Anchor properties.
        /// </summary>
        private protected override bool LayoutCore(IArrangedElement container, LayoutEventArgs args)
        {
            ProcessSuspendedLayoutEventArgs(container, args);

            ContainerInfo containerInfo = GetContainerInfo(container);
            EnsureRowAndColumnAssignments(container, containerInfo, /* doNotCache = */false);

            int cellBorderWidth = containerInfo.CellBorderWidth;

            //shrink the size of the display rectangle so that we have space to draw the border
            Size containerSize = container.DisplayRectangle.Size - new Size(cellBorderWidth, cellBorderWidth);

            //make sure our sizes are non-negative
            containerSize.Width = Math.Max(containerSize.Width, 1);
            containerSize.Height = Math.Max(containerSize.Height, 1);

            Size usedSpace = ApplyStyles(containerInfo, containerSize, /*measureOnly=*/false);
            ExpandLastElement(containerInfo, usedSpace, containerSize);
            RectangleF displayRect = (RectangleF)container.DisplayRectangle;
            displayRect.Inflate(-(cellBorderWidth / 2.0f), -(cellBorderWidth) / 2.0f);
            SetElementBounds(containerInfo, displayRect);

            // ScrollableControl will first try to get the layoutbounds from the derived control when
            // trying to figure out if ScrollBars should be added.
            CommonProperties.SetLayoutBounds(containerInfo.Container, new Size(SumStrips(containerInfo.Columns, 0, containerInfo.Columns.Length),
                                                            SumStrips(containerInfo.Rows, 0, containerInfo.Rows.Length)));

            return CommonProperties.GetAutoSize(container);
        }

        /// <summary>
        ///  GetPreferredSize:  Called on the container to determine the size that best fits its contents.
        ///  Container: IArrangedElement to determine preferredSize (could be table layout panel but doesnt have to be - eg. ToolStrip)
        ///  ProposedContstraints: the suggested size that the table layout should fit into.  If either argument is 0,
        ///             TableLayout pretends it's unconstrained for perfomance reasons.
        ///
        ///  Summary of Algorithm:
        ///  Similar to LayoutCore.  Row/Column assignments are NOT cached.  TableLayout uses AGRESSIVE
        ///  caching for performance reasons.
        /// </summary>
        internal override Size GetPreferredSize(IArrangedElement container, Size proposedConstraints)
        {
            ContainerInfo containerInfo = GetContainerInfo(container);

            // PERF: Optimizing nested table layouts.
            // The problem:  TableLayout asks for GPS(0,0) GPS(1,0) GPS(0,1) and GPS(w,0) and GPS(w,h).
            // if the table layout is nested, this becomes pretty nasty, as we dont cache row, column
            // assignments in preferred size.
            // GPS(0,1) GPS(1,0) should return same as GPS(0,0)- if that's already cached return it.
            float oldWidth = -1f;
            Size prefSize = containerInfo.GetCachedPreferredSize(proposedConstraints, out bool isCacheValid);
            if (isCacheValid)
            {
                return prefSize;
            }

            //create a dummy containerInfo and we operate on the dummy containerInfo only
            //the reason to do so is that when we call ApplyStyles, we change the information
            //in the containerInfo. Specifically it will change the strip size so that the
            //size information may no longer be accurate.
            ContainerInfo tempInfo = new ContainerInfo(containerInfo);
            int cellBorderWidth = containerInfo.CellBorderWidth;

            //pretend the last column is the size of the container if it is absolutely sized
            if (containerInfo.MaxColumns == 1 && containerInfo.ColumnStyles.Count > 0 && containerInfo.ColumnStyles[0].SizeType == SizeType.Absolute)
            {
                //shrink the size of the display rectangle so that we have space to draw the border
                Size containerSize = container.DisplayRectangle.Size - new Size(cellBorderWidth * 2, cellBorderWidth * 2);

                //make sure our sizes are non-negative
                containerSize.Width = Math.Max(containerSize.Width, 1);
                containerSize.Height = Math.Max(containerSize.Height, 1);
                oldWidth = containerInfo.ColumnStyles[0].Size;
                containerInfo.ColumnStyles[0].SetSize(Math.Max(oldWidth, Math.Min(proposedConstraints.Width, containerSize.Width)));
            }

            //notice we always assign rows and columns here since the user may change the properties of the table while the layout is
            //suspended, so we have no way to know whether the cache is invalid or not here.
            EnsureRowAndColumnAssignments(container, tempInfo, /* doNotCache = */ true);

            //deduct the padding for cell border before doing layout
            Size cellBorderSize = new Size(cellBorderWidth, cellBorderWidth);
            proposedConstraints -= cellBorderSize;
            proposedConstraints.Width = Math.Max(proposedConstraints.Width, 1);
            proposedConstraints.Height = Math.Max(proposedConstraints.Height, 1);
            if (tempInfo.Columns != null && containerInfo.Columns != null && (tempInfo.Columns.Length != containerInfo.Columns.Length))
            {
                ClearCachedAssignments(containerInfo);
            }
            if (tempInfo.Rows != null && containerInfo.Rows != null && (tempInfo.Rows.Length != containerInfo.Rows.Length))
            {
                ClearCachedAssignments(containerInfo);
            }

            prefSize = ApplyStyles(tempInfo, proposedConstraints, /*measureOnly=*/true);
            //prefSize = MeasureStyles(tempInfo);

            //restores the old strip size
            if (oldWidth >= 0)
            {
                containerInfo.ColumnStyles[0].SetSize(oldWidth);
            }

            //add back the padding for the cell border
            return (prefSize + cellBorderSize);
        }

        /// <summary>
        ///  EnsureRowAndColumnAssignments: Sets up Row/Column assignments for all the children of the container
        ///  - Does nothing if Cache is valid
        ///  - sets RowStart,RowSpan,ColumnStart,ColumnSpan into the LayoutInfo[] collection (containerInfo.ChildrenInfo)
        /// </summary>
        private void EnsureRowAndColumnAssignments(IArrangedElement container, ContainerInfo containerInfo, bool doNotCache)
        {
            // Assign new rows and columns if the cache is invalid or if we are in GetPreferredSize
            if (!HasCachedAssignments(containerInfo) || doNotCache)
            {
                AssignRowsAndColumns(containerInfo);
            }

            Debug_VerifyAssignmentsAreCurrent(container, containerInfo);

        }

        /// <summary>
        ///  ExpandLastElement: expands the row/column to fill the rest of the space in the container.
        /// </summary>
        private void ExpandLastElement(ContainerInfo containerInfo, Size usedSpace, Size totalSpace)
        {
            Strip[] rows = containerInfo.Rows;
            Strip[] cols = containerInfo.Columns;
            if (cols.Length != 0 && totalSpace.Width > usedSpace.Width)
            {
                cols[cols.Length - 1].MinSize += totalSpace.Width - usedSpace.Width;
            }
            if (rows.Length != 0 && totalSpace.Height > usedSpace.Height)
            {
                rows[rows.Length - 1].MinSize += totalSpace.Height - usedSpace.Height;
            }
        }

        /// <summary>
        ///  AssignRowsAndColumns: part of EnsureRowAndColumnAssignments.
        ///  determines the number of rows and columns we need to create
        /// </summary>
        private void AssignRowsAndColumns(ContainerInfo containerInfo)
        {
            int numCols = containerInfo.MaxColumns;
            int numRows = containerInfo.MaxRows;

            // forces iteration over child collection - this is cached and
            // sets containerInfo MinRowsAndColumns and MinColumns/MinRows
            LayoutInfo[] childrenInfo = containerInfo.ChildrenInfo;
            int minSpace = containerInfo.MinRowsAndColumns;
            int minColumn = containerInfo.MinColumns;
            int minRow = containerInfo.MinRows;

            TableLayoutPanelGrowStyle growStyle = containerInfo.GrowStyle;
            if (growStyle == TableLayoutPanelGrowStyle.FixedSize)
            {
                //if we're a fixed size - check to see if we have enough room
                if (containerInfo.MinRowsAndColumns > numCols * numRows)
                {
                    throw new ArgumentException(SR.TableLayoutPanelFullDesc);
                }

                if ((minColumn > numCols) || (minRow > numRows))
                {
                    throw new ArgumentException(SR.TableLayoutPanelSpanDesc);
                }

                numRows = Math.Max(1, numRows);
                numCols = Math.Max(1, numCols);
                // numRows = numRows - numRows indicates that columns are specified and rows should grow
            }
            else if (growStyle == TableLayoutPanelGrowStyle.AddRows)
            {
                numRows = 0;//indicates that columns are specified and rows should grow
            }
            else
            {//must be addcolumns
                numCols = 0;//indicates that rows are specified and columns should grow
            }

            if (numCols > 0)
            {
                // The user specified the number of column (the simple/fast case)
                xAssignRowsAndColumns(containerInfo, childrenInfo, numCols, numRows == 0 ? int.MaxValue : numRows, growStyle);
            }
            else if (numRows > 0)
            {
                // The user specified rows only (we need to compute the number of columns)

                int estimatedCols = Math.Max((int)Math.Ceiling((float)minSpace / numRows), minColumn);

                //make sure that estimatedCols is positive
                estimatedCols = Math.Max(estimatedCols, 1);

                while (!xAssignRowsAndColumns(containerInfo, childrenInfo, estimatedCols, numRows, growStyle))
                {
                    // I am assuming that the division will put us pretty close to the right
                    // number of columns.  If this assumption is wrong, a binary search
                    // between the number we get by dividing and childrenInfos.Count
                    // could be more efficient.  It would certainly degenerate better.
                    estimatedCols++;
                }
            }
            else
            {
                // No rows or columns specified, just do a vertical stack.
                xAssignRowsAndColumns(containerInfo, childrenInfo, /* numCols = */  Math.Max(minColumn, 1), /* numRows = */ int.MaxValue, growStyle);
            }
        }

        /// <summary>
        ///  xAssignRowsAndColumns: part of AssignRowsAndColumns.
        ///  def: fixed element: has a specific row/column assignment (assigned by SetRow,SetColumn, or Add(c,row,column)
        ///  def: flow element: does NOT have a specific row/column assignment.
        ///
        ///  Determines the placement of fixed and flow elements.  Walks through the rows/columns - if there's a
        ///  spot for the fixed element, place it, else place the next flow element.
        /// </summary>
        private bool xAssignRowsAndColumns(ContainerInfo containerInfo, LayoutInfo[] childrenInfo, int maxColumns, int maxRows, TableLayoutPanelGrowStyle growStyle)
        {
            Debug.Assert(maxColumns > 0, "maxColumn must be positive");

            int numColumns = 0;
            int numRows = 0;
            ReservationGrid reservationGrid = new ReservationGrid();

            int currentRow = 0;
            int currentCol = 0;

            int fixedElementIndex = -1;
            int flowElementIndex = -1;

            // make sure to snap these two collections as we're not in a "containerInfo.Valid" state
            // so we'll wind up building up the lists over and over again.
            LayoutInfo[] fixedChildrenInfo = containerInfo.FixedChildrenInfo;

            //the element at the head of the absolutely positioned element queue
            LayoutInfo fixedElement = GetNextLayoutInfo(fixedChildrenInfo, ref fixedElementIndex, /*absolutelyPositioned*/true);

            //the element at the head of the non-absolutely positioned element queue
            LayoutInfo flowElement = GetNextLayoutInfo(childrenInfo, ref flowElementIndex, /*absolutelyPositioned*/false);

            while (fixedElement != null || flowElement != null)
            {

                int colStop = currentCol;
                int rowStop;
                if (flowElement != null)
                {
                    flowElement.RowStart = currentRow;
                    flowElement.ColumnStart = currentCol;
                    //try to layout the flowElement to see if it overlaps with the fixedElement
                    AdvanceUntilFits(maxColumns, reservationGrid, flowElement, out colStop);
                    //we have exceeded the row limit. just return
                    if (flowElement.RowStart >= maxRows)
                    {
                        return false;
                    }
                }
                //test to see if either the absolutely positioned element is null or it fits.
                if (flowElement != null && (fixedElement == null || (!IsCursorPastInsertionPoint(fixedElement, flowElement.RowStart, colStop) && !IsOverlappingWithReservationGrid(fixedElement, reservationGrid, currentRow))))
                {
                    //Place the flow element.

                    //advance the rows in reservation grid
                    for (int j = 0; j < flowElement.RowStart - currentRow; j++)
                    {
                        reservationGrid.AdvanceRow();
                    }

                    currentRow = flowElement.RowStart;
                    rowStop = Math.Min(currentRow + flowElement.RowSpan, maxRows);

                    //reserve spaces in the reservationGrid
                    reservationGrid.ReserveAll(flowElement, rowStop, colStop);
                    flowElement = GetNextLayoutInfo(childrenInfo, ref flowElementIndex, /*absolutelyPositioned*/false);
                }
                else
                {
                    //
                    //otherwise we place the fixed element.
                    //
                    if (currentCol >= maxColumns)
                    {
                        //we have already passed the boundary. Go to next row
                        currentCol = 0;
                        currentRow++;
                        reservationGrid.AdvanceRow();
                    }
                    //set the rowStart and columnStart to fixedElement's specifed position
                    fixedElement.RowStart = Math.Min(fixedElement.RowPosition, maxRows - 1);
                    fixedElement.ColumnStart = Math.Min(fixedElement.ColumnPosition, maxColumns - 1);
                    if (currentRow > fixedElement.RowStart)
                    {
                        //we have already passed the specifed position. set the start column to the current column
                        fixedElement.ColumnStart = currentCol;
                    }
                    else if (currentRow == fixedElement.RowStart)
                    {
                        //set the start column to be the max of the specifed column and current column
                        fixedElement.ColumnStart = Math.Max(fixedElement.ColumnStart, currentCol);
                    }
                    else
                    {
                        //set the start column to the specified column, which we have already done
                    }
                    fixedElement.RowStart = Math.Max(fixedElement.RowStart, currentRow);

                    //advance the reservation grid
                    int j;
                    for (j = 0; j < fixedElement.RowStart - currentRow; j++)
                    {
                        reservationGrid.AdvanceRow();
                    }

                    //try to layout the absolutely positioned element as if it were non-absolutely positioned.
                    //In this way we can tell whether this element overlapps with others or fits on the table.
                    AdvanceUntilFits(maxColumns, reservationGrid, fixedElement, out colStop);

                    //we have exceeded the row limit. just return
                    if (fixedElement.RowStart >= maxRows)
                    {
                        return false;
                    }

                    for (; j < fixedElement.RowStart - currentRow; j++)
                    {
                        //advance the reservation grid if the fixedElement's row position has changed during layout
                        reservationGrid.AdvanceRow();
                    }
                    currentRow = fixedElement.RowStart;

                    //make sure that we truncate the element's column span if it is too big
                    colStop = Math.Min(fixedElement.ColumnStart + fixedElement.ColumnSpan, maxColumns);
                    rowStop = Math.Min(fixedElement.RowStart + fixedElement.RowSpan, maxRows);

                    //reserve space in the reservation grid
                    reservationGrid.ReserveAll(fixedElement, rowStop, colStop);

                    fixedElement = GetNextLayoutInfo(fixedChildrenInfo, ref fixedElementIndex, /*absolutelyPositioned*/true);
                }
                currentCol = colStop;
                numRows = (numRows == int.MaxValue) ? rowStop : Math.Max(numRows, rowStop);
                numColumns = (numColumns == int.MaxValue) ? colStop : Math.Max(numColumns, colStop);
            }

            Debug.Assert(numRows <= maxRows, "number of rows allocated shouldn't exceed max number of rows");
            Debug.Assert(numColumns <= maxColumns, "number of columns allocated shouldn't exceed max number of columns");

            // we should respect columncount and rowcount as according to GrowStyle.
            if (growStyle == TableLayoutPanelGrowStyle.FixedSize)
            {
                // now that we've calculated the assignments - use the "max" as the actual number of rows.
                numColumns = maxColumns;
                numRows = maxRows;
            }
            else if (growStyle == TableLayoutPanelGrowStyle.AddRows)
            {
                numColumns = maxColumns;
                numRows = Math.Max(containerInfo.MaxRows, numRows);
            }
            else
            { // add columns
                numRows = (maxRows == int.MaxValue) ? numRows : maxRows;
                numColumns = Math.Max(containerInfo.MaxColumns, numColumns);
            }

            // PERF: prevent overallocation of Strip[] arrays.  We're going to null these guys out
            // anyways... so only allocate when the number of rows and columns is different.
            if (containerInfo.Rows == null || containerInfo.Rows.Length != numRows)
            {
                containerInfo.Rows = new Strip[numRows];
            }
            if (containerInfo.Columns == null || containerInfo.Columns.Length != numColumns)
            {
                containerInfo.Columns = new Strip[numColumns];
            }

            containerInfo.Valid = true;
            return true;

        }

        /// <summary>
        ///  GetNextLayoutInfo: part of xAssignRowsAndColumns.
        ///  helper function that walks through the collection picking out the next flow element or fixed element.
        /// </summary>
        private static LayoutInfo GetNextLayoutInfo(LayoutInfo[] layoutInfo, ref int index, bool absolutelyPositioned)
        {
            for (int i = ++index; i < layoutInfo.Length; i++)
            {
                if (absolutelyPositioned == layoutInfo[i].IsAbsolutelyPositioned)
                {
                    index = i;
                    return layoutInfo[i];
                }
            }
            index = layoutInfo.Length;
            return null;
        }

        /// <summary>
        ///  IsCursorPastInsertionPoint: part of xAssignRowsAndColumns.
        ///  check to see if the user specified location for fixedLayoutInfo has passed the insertion point specified by the cursor
        /// </summary>
        private bool IsCursorPastInsertionPoint(LayoutInfo fixedLayoutInfo, int insertionRow, int insertionCol)
        {
            Debug.Assert(fixedLayoutInfo.IsAbsolutelyPositioned, "should only check for those elements which are absolutely positioned");

            //if the element is bumped to a row below its specified row position, it means that the element overlaps with previous controls
            if (fixedLayoutInfo.RowPosition < insertionRow)
            {
                return true;
            }
            //if the element is bumped to a column after its specified column position, it also means that the element overlaps with previous controls
            if (fixedLayoutInfo.RowPosition == insertionRow && fixedLayoutInfo.ColumnPosition < insertionCol)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///  IsOverlappingWithReservationGrid: part of xAssignRowsAndColumns.
        ///  check to see if the absolutely positioned layoutInfo fits in the reservation grid
        /// </summary>
        private bool IsOverlappingWithReservationGrid(LayoutInfo fixedLayoutInfo, ReservationGrid reservationGrid, int currentRow)
        {
            //since we shall not put anything above our current row, this means that the fixedLayoutInfo overlaps with something already placed on the table
            if (fixedLayoutInfo.RowPosition < currentRow)
            {
                return true;
            }
            for (int rowOffset = fixedLayoutInfo.RowPosition - currentRow; rowOffset < fixedLayoutInfo.RowPosition - currentRow + fixedLayoutInfo.RowSpan; rowOffset++)
            {
                for (int colOffset = fixedLayoutInfo.ColumnPosition; colOffset < fixedLayoutInfo.ColumnPosition + fixedLayoutInfo.ColumnSpan; colOffset++)
                {
                    if (reservationGrid.IsReserved(colOffset, rowOffset))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///  AdvanceUntilFits: part of xAssignRowsAndColumns.
        ///  Advances the position of layoutInfo until we have enough space and do not
        ///  collide with a rowSpanned element.  ColStop will be the column on which the
        ///  element ends (exclusive).
        /// </summary>
        private void AdvanceUntilFits(int maxColumns, ReservationGrid reservationGrid, LayoutInfo layoutInfo, out int colStop)
        {
            int prevRow = layoutInfo.RowStart;
            do
            {
                GetColStartAndStop(maxColumns, reservationGrid, layoutInfo, out colStop);
            } while (ScanRowForOverlap(maxColumns, reservationGrid, layoutInfo, colStop, layoutInfo.RowStart - prevRow));
        }

        /// <summary>
        ///  GetColStartAndStop: part of xAssignRowsAndColumns.
        /// </summary>
        private void GetColStartAndStop(int maxColumns, ReservationGrid reservationGrid, LayoutInfo layoutInfo, out int colStop)
        {
            // Compute the column our element ends on
            colStop = layoutInfo.ColumnStart + layoutInfo.ColumnSpan;
            if (colStop > maxColumns)
            {
                if (layoutInfo.ColumnStart != 0)
                {
                    // If we are not already at the beginning or a row, move down
                    // to the next row.
                    layoutInfo.ColumnStart = 0;
                    layoutInfo.RowStart++;
                }
                // Cap colStop in case we have a element too large to fit on any row.
                colStop = Math.Min(layoutInfo.ColumnSpan, maxColumns);
            }
        }

        private bool ScanRowForOverlap(int maxColumns, ReservationGrid reservationGrid, LayoutInfo layoutInfo, int stopCol, int rowOffset)
        {
            for (int i = layoutInfo.ColumnStart; i < stopCol; i++)
            {
                if (reservationGrid.IsReserved(i, rowOffset))
                {
                    // If we hit reserved space, advance startCol past it.  If we hit the end of the row,
                    // just stop.  AdvanceUntilFits will move to the next row and call us again.
                    for (layoutInfo.ColumnStart = i + 1;
                        layoutInfo.ColumnStart < maxColumns && reservationGrid.IsReserved(layoutInfo.ColumnStart, rowOffset);
                        layoutInfo.ColumnStart++)
                    {
                        ;
                    }

                    return true;
                }
            }
            return false;
        }

        private Size ApplyStyles(ContainerInfo containerInfo, Size proposedConstraints, bool measureOnly)
        {
            Size preferredSize = Size.Empty;

            //allocate space for all absolutely sized strips. Set the size of the rest of the strips to 0.
            InitializeStrips(containerInfo.Columns, containerInfo.ColumnStyles);
            InitializeStrips(containerInfo.Rows, containerInfo.RowStyles);

            // perf optimization - detect if we should worry about column spans
            containerInfo.ChildHasColumnSpan = false;
            containerInfo.ChildHasRowSpan = false;

            //detect all rows/columns which have control starting from it
            foreach (LayoutInfo layoutInfo in containerInfo.ChildrenInfo)
            {
                containerInfo.Columns[layoutInfo.ColumnStart].IsStart = true;
                containerInfo.Rows[layoutInfo.RowStart].IsStart = true;
                if (layoutInfo.ColumnSpan > 1)
                {
                    containerInfo.ChildHasColumnSpan = true;
                }
                if (layoutInfo.RowSpan > 1)
                {
                    containerInfo.ChildHasRowSpan = true;
                }
            }
            preferredSize.Width = InflateColumns(containerInfo, proposedConstraints, measureOnly);
            int expandLastElementWidth = Math.Max(0, proposedConstraints.Width - preferredSize.Width);
            preferredSize.Height = InflateRows(containerInfo, proposedConstraints, expandLastElementWidth, measureOnly);
            return preferredSize;
        }

        //allocate space for all absolutely sized strips. Set the size of the rest of the strips to 0.
        private void InitializeStrips(Strip[] strips, IList styles)
        {
            Strip strip;
            for (int i = 0; i < strips.Length; i++)
            {
                TableLayoutStyle style = i < styles.Count ? (TableLayoutStyle)styles[i] : null;
                strip = strips[i];
                if (style != null && style.SizeType == SizeType.Absolute)
                {
                    strip.MinSize = (int)Math.Round((double)((TableLayoutStyle)styles[i]).Size);
                    strip.MaxSize = strip.MinSize;
                }
                else
                {
                    strip.MinSize = 0;
                    strip.MaxSize = 0;
                }
                strip.IsStart = false;
                strips[i] = strip;
            }
        }

        private int InflateColumns(ContainerInfo containerInfo, Size proposedConstraints, bool measureOnly)
        {
            bool dontHonorConstraint = measureOnly;

            LayoutInfo[] sortedChildren = containerInfo.ChildrenInfo;
            if (containerInfo.ChildHasColumnSpan)
            {
                Sort(sortedChildren, ColumnSpanComparer.GetInstance);
            }

            // Normally when we are called from GetPreferredSize (measureOnly is true), we want to treat ourselves
            // as being unbounded. But in some scenarios, we actually do want to honor the constraints. When
            // we are docked or anchored in the right combination, and our parent's layout engine supports
            // dock and anchoring, and we are actually constrained in at least one direction,
            // then we should honor the constraint.

            // The Int16.MaxValue check will tell us whether we are actually constrained or not. This is not ideal.

            if (dontHonorConstraint && (proposedConstraints.Width < short.MaxValue))
            {
                if (containerInfo.Container is TableLayoutPanel tlp && tlp.ParentInternal != null && tlp.ParentInternal.LayoutEngine == DefaultLayout.Instance)
                {
                    if (tlp.Dock == DockStyle.Top || tlp.Dock == DockStyle.Bottom || tlp.Dock == DockStyle.Fill)
                    {
                        dontHonorConstraint = false; // we want to honor constraints
                    }

                    if ((tlp.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right))
                    {
                        dontHonorConstraint = false; // we want to honor constraints
                    }
                }
            }

            foreach (LayoutInfo layoutInfo in sortedChildren)
            {
                IArrangedElement element = layoutInfo.Element;

                int columnSpan = layoutInfo.ColumnSpan;

                // since InitializeStrips already allocated absolutely sized columns, we can skip over them
                if (columnSpan > 1 || !IsAbsolutelySized(layoutInfo.ColumnStart, containerInfo.ColumnStyles))
                {
                    int minWidth = 0, maxWidth = 0;
                    // optimize for the case where one of the parameters is known.
                    if ((columnSpan == 1 && layoutInfo.RowSpan == 1) &&
                        (IsAbsolutelySized(layoutInfo.RowStart, containerInfo.RowStyles)))
                    {
                        int constrainingHeight = (int)containerInfo.RowStyles[layoutInfo.RowStart].Size;
                        minWidth = GetElementSize(element, new Size(0, constrainingHeight)).Width;
                        maxWidth = minWidth;
                    }
                    else
                    {
                        minWidth = GetElementSize(element, new Size(1, 0)).Width;
                        maxWidth = GetElementSize(element, Size.Empty).Width;
                    }

                    // tack on margins
                    Padding margin = CommonProperties.GetMargin(element);
                    minWidth += margin.Horizontal;
                    maxWidth += margin.Horizontal;

                    // distribute the minimum size, then maximum size over the columns
                    int colStop = Math.Min(layoutInfo.ColumnStart + layoutInfo.ColumnSpan, containerInfo.Columns.Length);
                    DistributeSize(containerInfo.ColumnStyles, containerInfo.Columns, layoutInfo.ColumnStart, colStop, minWidth, maxWidth, containerInfo.CellBorderWidth);
                }
            }

            int width = DistributeStyles(containerInfo.CellBorderWidth, containerInfo.ColumnStyles, containerInfo.Columns, proposedConstraints.Width, dontHonorConstraint);

            // TLP doesn't honor proposedConstraints
            if (dontHonorConstraint && width > proposedConstraints.Width && proposedConstraints.Width > 1)
            {
                // Step 1: iterate through the rows or columns,
                //  - calculate the amount of space allocated
                //     - for percent size columns
                //  - sum up the total "%"s being used
                //     - eg totalPercent = 22 + 22 + 22 = 66%. each column should take up 1/3 of the remaining space.

                Strip[] strips = containerInfo.Columns;
                float totalPercent = 0;
                int totalPercentAllocatedSpace = 0;
                TableLayoutStyleCollection styles = containerInfo.ColumnStyles;

                for (int i = 0; i < strips.Length; i++)
                {
                    Strip strip = strips[i];
                    if (i < styles.Count)
                    {
                        TableLayoutStyle style = (TableLayoutStyle)styles[i];
                        if (style.SizeType == SizeType.Percent)
                        {
                            totalPercent += style.Size;
                            totalPercentAllocatedSpace += strip.MinSize;
                        }
                    }
                }

                // We will attempt to steal from percentage size columns in order to
                // meet the proposed constraints as closely as possible
                int currentOverflow = width - proposedConstraints.Width;

                int stealAmount = Math.Min(currentOverflow, totalPercentAllocatedSpace);

                for (int i = 0; i < strips.Length; i++)
                {
                    if (i < styles.Count)
                    {
                        TableLayoutStyle style = (TableLayoutStyle)styles[i];
                        if (style.SizeType == SizeType.Percent)
                        {
                            float percentageOfTotal = style.Size / (float)totalPercent;
                            strips[i].MinSize -= (int)(percentageOfTotal * stealAmount);
                        }
                    }
                }

                return width - stealAmount;
            }

            return width;
        }

        private int InflateRows(ContainerInfo containerInfo, Size proposedConstraints, int expandLastElementWidth, bool measureOnly)
        {
            bool dontHonorConstraint = measureOnly;

            LayoutInfo[] sortedChildren = containerInfo.ChildrenInfo;
            if (containerInfo.ChildHasRowSpan)
            {
                Sort(sortedChildren, RowSpanComparer.GetInstance);
            }

            bool multiplePercent = containerInfo.HasMultiplePercentColumns;

            // Normally when we are called from GetPreferredSize (measureOnly is true), we want to treat ourselves
            // as being unbounded. But in some scenarios, we actually do want to honor the constraints. When
            // we are docked or anchored in the right combination, and our parent's layout engine supports
            // dock and anchoring, and we are actually constrained in at least one direction,
            // then we should honor the constraint.

            // The Int16.MaxValue check will tell us whether we are actually constrained or not. This is not ideal.

            if (dontHonorConstraint && (proposedConstraints.Height < short.MaxValue))
            {
                if (containerInfo.Container is TableLayoutPanel tlp && tlp.ParentInternal != null && tlp.ParentInternal.LayoutEngine == DefaultLayout.Instance)
                {
                    if (tlp.Dock == DockStyle.Left || tlp.Dock == DockStyle.Right || tlp.Dock == DockStyle.Fill)
                    {
                        dontHonorConstraint = false; // we want to honor constraints
                    }

                    if ((tlp.Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom))
                    {
                        dontHonorConstraint = false; // we want to honor constraints
                    }
                }
            }

            foreach (LayoutInfo layoutInfo in sortedChildren)
            {
                IArrangedElement element = layoutInfo.Element;
                int rowSpan = layoutInfo.RowSpan;

                // we can skip over absolute row styles, as they've been preallocated
                if (rowSpan > 1 || !IsAbsolutelySized(layoutInfo.RowStart, containerInfo.RowStyles))
                {

                    int currentWidth = SumStrips(containerInfo.Columns, layoutInfo.ColumnStart, layoutInfo.ColumnSpan);
                    //make sure that the total width is the actual final width to avoid
                    //inconsistency of width between the ApplyStyles and SetElementBounds
                    // Only apply when there is one multiple percentage column
                    if (!dontHonorConstraint && layoutInfo.ColumnStart + layoutInfo.ColumnSpan >= containerInfo.MaxColumns && !multiplePercent)
                    {
                        currentWidth += expandLastElementWidth;
                    }

                    // since we know the width at this point, use that as the constraining width.
                    Padding margin = CommonProperties.GetMargin(element);
                    int minHeight = GetElementSize(element, new Size(currentWidth - margin.Horizontal, 0)).Height + margin.Vertical;
                    int maxHeight = minHeight;
                    int rowStop = Math.Min(layoutInfo.RowStart + layoutInfo.RowSpan, containerInfo.Rows.Length);
                    DistributeSize(containerInfo.RowStyles, containerInfo.Rows, layoutInfo.RowStart, rowStop, minHeight, maxHeight, containerInfo.CellBorderWidth);
                }
            }
            return DistributeStyles(containerInfo.CellBorderWidth, containerInfo.RowStyles, containerInfo.Rows, proposedConstraints.Height, dontHonorConstraint);

        }

        private Size GetElementSize(IArrangedElement element, Size proposedConstraints)
        {
            if (CommonProperties.GetAutoSize(element))
            {
                return element.GetPreferredSize(proposedConstraints);
            }
            else
            {
                return CommonProperties.GetSpecifiedBounds(element).Size;
            }
        }

        internal int SumStrips(Strip[] strips, int start, int span)
        {
            int size = 0;
            for (int i = start; i < Math.Min(start + span, strips.Length); i++)
            {
                Strip strip = strips[i];
                size += strip.MinSize;
            }
            return size;
        }

        /// <summary>
        ///  Sets the minimum size for each element
        /// </summary>
        private void DistributeSize(IList styles, Strip[] strips, int start, int stop, int min, int max, int cellBorderWidth)
        {
            xDistributeSize(styles, strips, start, stop, min, MinSizeProxy.GetInstance, cellBorderWidth);
            xDistributeSize(styles, strips, start, stop, max, MaxSizeProxy.GetInstance, cellBorderWidth);
        }

        private void xDistributeSize(IList styles, Strip[] strips, int start, int stop, int desiredLength, SizeProxy sizeProxy, int cellBorderWidth)
        {
            int currentLength = 0;   //total length allocated so far
            int numUninitializedStrips = 0;  //number of strips whose Size is 0 and is not absolutely positioned

            //subtract the space for cell borders. Notice if a control spans two columns its
            //proposed size is 10 and the border width is 3, we actually only need to distribute
            //7 pixels among the two cells it spans
            desiredLength -= cellBorderWidth * (stop - start - 1);
            desiredLength = Math.Max(0, desiredLength);

            for (int i = start; i < stop; i++)
            {
                sizeProxy.Strip = strips[i];
                if (!IsAbsolutelySized(i, styles) && sizeProxy.Size == 0)
                {
                    //the strip is not absolutely sized and it hasn't been initialized
                    numUninitializedStrips++;
                }
                currentLength += sizeProxy.Size;
            }
            int missingLength = desiredLength - currentLength;

            if (missingLength <= 0)
            {
                //no extra space left. Simply return.
                return;
            }

            if (numUninitializedStrips == 0)
            {
                //look for any strip whose style is percent. If there is one, dump all space in it
                int lastPercent;
                for (lastPercent = stop - 1; lastPercent >= start; lastPercent--)
                {
                    if (lastPercent < styles.Count && ((TableLayoutStyle)styles[lastPercent]).SizeType == SizeType.Percent)
                    {
                        break;
                    }
                }
                //we have found one strip whose style is percent.
                //make sure that the for loop below only looks at this strip
                if (lastPercent != start - 1)
                {
                    stop = lastPercent + 1;
                }
                //every strip has absolute width or all of them have already been allocated space
                //walk backwards
                for (int i = stop - 1; i >= start; i--)
                {
                    if (!IsAbsolutelySized(i, styles))
                    {
                        //dump the extra space to this strip
                        sizeProxy.Strip = strips[i];

                        if (!(i == strips.Length - 1)               //this is not the last strip
                            && !strips[i + 1].IsStart                 //and there is no control starting from the strip next to it
                            && !IsAbsolutelySized(i + 1, styles))
                        {  // and the strip next to it is not absolutely sized
                            //try to "borrow" space from the strip next to it
                            sizeProxy.Strip = strips[i + 1];
                            int offset = Math.Min(sizeProxy.Size, missingLength);
                            sizeProxy.Size -= offset;
                            strips[i + 1] = sizeProxy.Strip;

                            sizeProxy.Strip = strips[i];
                        }
                        //put whatever left into this strip
                        sizeProxy.Size += missingLength;
                        strips[i] = sizeProxy.Strip;
                        break;
                    }
                }
                //if we fall through here, everything is absolutely positioned. discard the extra space
            }
            //there are some uninitialized strips
            else
            {
                //average space to be distributed
                int average = missingLength / numUninitializedStrips;
                //total number of uninitialized strips encountered so far
                int uninitializedStripIndex = 0;
                for (int i = start; i < stop; i++)
                {
                    sizeProxy.Strip = strips[i];
                    //this is an uninitialized strip
                    if (!IsAbsolutelySized(i, styles) && sizeProxy.Size == 0)
                    {
                        uninitializedStripIndex++;
                        if (uninitializedStripIndex == numUninitializedStrips)
                        {
                            //we are at the last strip. place round off error here
                            average = missingLength - average * (numUninitializedStrips - 1);
                        }
                        sizeProxy.Size += average;
                        strips[i] = sizeProxy.Strip;
                    }
                }
            }
        }

        //determines whether strip[index]'s style is absolutely sized
        private bool IsAbsolutelySized(int index, IList styles)
        {
            return (index < styles.Count) && ((TableLayoutStyle)styles[index]).SizeType == SizeType.Absolute;
        }

        ///<summary>
        ///  Now that we've allocated minimum and maximum sizes to everyone (the strips), distribute the extra space
        ///  as according to the Row/Column styles.
        ///</summary>
        private int DistributeStyles(int cellBorderWidth, IList styles, Strip[] strips, int maxSize, bool dontHonorConstraint)
        {
            int usedSpace = 0;
            //first, allocate the minimum space required for each element

            float totalPercent = 0;
            float totalPercentAllocatedSpace = 0;
            float totalAbsoluteAndAutoSizeAllocatedSpace = 0;
            bool hasAutoSizeColumn = false;

            // Step 1: iterate through the rows or columns,
            //  - calculate the amount of space allocated
            //     - for autosize and fixed size columns
            //     - for percent size columns
            //  - sum up the total "%"s being used
            //     - eg totalPercent = 22 + 22 + 22 = 66%. each column should take up 1/3 of the remaining space.
            for (int i = 0; i < strips.Length; i++)
            {
                Strip strip = strips[i];
                if (i < styles.Count)
                {
                    TableLayoutStyle style = (TableLayoutStyle)styles[i];
                    switch (style.SizeType)
                    {
                        case SizeType.Absolute:
                            totalAbsoluteAndAutoSizeAllocatedSpace += strip.MinSize;
                            // We gaurantee a strip will be exactly abs pixels
                            Debug.Assert((strip.MinSize == style.Size), "absolutely sized strip's size should be set before we call ApplyStyles");
                            break;
                        case SizeType.Percent:
                            totalPercent += style.Size;
                            totalPercentAllocatedSpace += strip.MinSize;
                            break;
                        default:
                            totalAbsoluteAndAutoSizeAllocatedSpace += strip.MinSize;
                            hasAutoSizeColumn = true;
                            break;
                    }
                }
                else
                {
                    hasAutoSizeColumn = true;
                }
                strip.MaxSize += cellBorderWidth;
                strip.MinSize += cellBorderWidth;  //add the padding for the cell border

                strips[i] = strip;
                usedSpace += strip.MinSize;
            }

            int remainingSpace = maxSize - usedSpace;

            // Step 2: (ONLY if we have % style column)
            //   - distribute unused space for absolute/autosize columns to percentage columns
            //          - determine the extra space that is not being used for autosize and fixed columns
            //          - divide space amongst % style columns using ratio of %/total % * total extra space
            if (totalPercent > 0)
            {

                if (!dontHonorConstraint)
                {

                    if (totalPercentAllocatedSpace > maxSize - totalAbsoluteAndAutoSizeAllocatedSpace)
                    {
                        // fixup for the case where we've actually allocated more space than we have.
                        // this can happen when the sum of the widths/heights of the controls are larger than the size of the
                        // table (aka maxSize)

                        //Don't want negative size...
                        totalPercentAllocatedSpace = Math.Max(0, maxSize - totalAbsoluteAndAutoSizeAllocatedSpace);
                    }

                    if (remainingSpace > 0)
                    {
                        // If there's space left over, then give it to the percentage columns/rows
                        totalPercentAllocatedSpace += remainingSpace;
                    }

                    else if (remainingSpace < 0)
                    {
                        // If there's not enough space, then remove space from the percentage columns.
                        // We do this by recalculating the space available.
                        totalPercentAllocatedSpace = maxSize - totalAbsoluteAndAutoSizeAllocatedSpace - (strips.Length * cellBorderWidth);
                        remainingSpace = 0;
                    }

                    // in this case the strips fill up the remaining space.
                    for (int i = 0; i < strips.Length; i++)
                    {
                        Strip strip = strips[i];
                        SizeType sizeType = i < styles.Count ? ((TableLayoutStyle)styles[i]).SizeType : SizeType.AutoSize;
                        if (sizeType == SizeType.Percent)
                        {
                            TableLayoutStyle style = (TableLayoutStyle)styles[i];

                            // cast to int / (take the floor) so we know we dont accidentally go over our limit.
                            // the rest will be distributed later.
                            int stripSize = (int)(style.Size * totalPercentAllocatedSpace / totalPercent);
                            usedSpace -= strip.MinSize; // back out the size we thought we were allocating before.
                            usedSpace += stripSize + cellBorderWidth;  // add in the new size we think we're going to use.
                            strip.MinSize = stripSize + cellBorderWidth;
                            strips[i] = strip;
                        }

                    }
                }
                else
                {
                    // the size of the item defines the size allocated to the percentage style columns.
                    // this supports [Ok][Cancel] in a 50% 50% column arrangement.  When one grows it pushes the whole table
                    // larger.
                    int maxPercentWidth = 0;
                    for (int i = 0; i < strips.Length; i++)
                    {
                        Strip strip = strips[i];
                        SizeType sizeType = i < styles.Count ? ((TableLayoutStyle)styles[i]).SizeType : SizeType.AutoSize;

                        // Performing the inverse calculation for GetPreferredSize:
                        //
                        //               stylePercent * totalWidth                      colWidth * totalPercent
                        //  colWidth =   -------------------------  --->   totalWidth = -----------------------
                        //                     totalPercent                                   stylePercent
                        //
                        // we'll take the max of the total widths as the one for our preferred size.
                        if (sizeType == SizeType.Percent)
                        {
                            TableLayoutStyle style = (TableLayoutStyle)styles[i];

                            int totalWidth = (int)Math.Round(((strip.MinSize * totalPercent) / style.Size));
                            maxPercentWidth = Math.Max(maxPercentWidth, totalWidth);
                            usedSpace -= strip.MinSize;
                        }
                    }
                    usedSpace += maxPercentWidth;
                }

            }
            remainingSpace = maxSize - usedSpace;

            // Step 3: add remaining space to autosize columns
            //  - usually we only do this if we're not in preferred size (remaingSpace would be < 0)
            //  - and there are no % style columns

            if (hasAutoSizeColumn && remainingSpace > 0)
            {
                for (int i = 0; i < strips.Length; i++)
                {
                    Strip strip = strips[i];
                    SizeType sizeType = i < styles.Count ? ((TableLayoutStyle)styles[i]).SizeType : SizeType.AutoSize;
                    if (sizeType == SizeType.AutoSize)
                    {
                        int delta = Math.Min(strip.MaxSize - strip.MinSize, remainingSpace);
                        if (delta > 0)
                        {
                            usedSpace += delta;
                            remainingSpace -= delta;
                            strip.MinSize += delta;
                            strips[i] = strip;
                        }
                    }
                }
            }
            Debug.Assert((dontHonorConstraint || (usedSpace == SumStrips(strips, 0, strips.Length))), "Error computing usedSpace.");
            return usedSpace;
        }

        private void SetElementBounds(ContainerInfo containerInfo, RectangleF displayRectF)
        {
            int cellBorderWidth = containerInfo.CellBorderWidth;

            float top = displayRectF.Y;
            int currentCol = 0;
            int currentRow = 0;
            bool isContainerRTL = false;
            Rectangle displayRect = Rectangle.Truncate(displayRectF);

            if (containerInfo.Container is Control)
            {
                Control control = containerInfo.Container as Control;
                isContainerRTL = control.RightToLeft == RightToLeft.Yes;
            }
            LayoutInfo[] childrenInfo = containerInfo.ChildrenInfo;
            float startX = isContainerRTL ? displayRectF.Right : displayRectF.X;

            //sort everything according to row major, column minor order
            //so that we can ensure that as we walk through all elements
            //the cursor always goes from left to right and from top to bottom.
            Sort(childrenInfo, PostAssignedPositionComparer.GetInstance);
            for (int i = 0; i < childrenInfo.Length; i++)
            {
                LayoutInfo layoutInfo = (LayoutInfo)childrenInfo[i];

                IArrangedElement element = layoutInfo.Element;

                // Advance top to the beginning of this elements row.
                Debug.Assert(currentRow <= layoutInfo.RowStart, "RowStart should increase in forward Z-order.");
                if (currentRow != layoutInfo.RowStart)
                {
                    for (; currentRow < layoutInfo.RowStart; currentRow++)
                    {
                        top += containerInfo.Rows[currentRow].MinSize;
                    }
                    startX = isContainerRTL ? displayRectF.Right : displayRectF.X;
                    currentCol = 0;
                }

                // Advance left to the beginning of this elements column.
                Debug.Assert(currentCol <= layoutInfo.ColumnStart, "ColumnStart should increase in forward Z-order.");
                for (; currentCol < layoutInfo.ColumnStart; currentCol++)
                {
                    if (isContainerRTL)
                    {
                        startX -= containerInfo.Columns[currentCol].MinSize;
                    }
                    else
                    {
                        startX += containerInfo.Columns[currentCol].MinSize;
                    }
                }

                // Sum the total width of the span.  We increment currentCol as we
                // do this.
                int colStop = currentCol + layoutInfo.ColumnSpan;
                int width = 0;
                for (; currentCol < colStop && currentCol < containerInfo.Columns.Length; currentCol++)
                {
                    width += containerInfo.Columns[currentCol].MinSize;
                }

                if (isContainerRTL)
                {
                    startX -= width;
                }

                // Sum the total height of the span.  We do not increment RowSpan
                // as we do this because there may be more elements on this row.
                int rowStop = currentRow + layoutInfo.RowSpan;
                int height = 0;
                for (int rowIndex = currentRow; rowIndex < rowStop && rowIndex < containerInfo.Rows.Length; rowIndex++)
                {
                    height += containerInfo.Rows[rowIndex].MinSize;
                }

                Rectangle cellBounds = new Rectangle((int)(startX + cellBorderWidth / 2.0f), (int)(top + cellBorderWidth / 2.0f), width - cellBorderWidth, height - cellBorderWidth);

                // We laid out the rows and columns with the element's margins included.
                // We now deflate the rect to get the actual element bounds.
                Padding elementMargin = CommonProperties.GetMargin(element);
                if (isContainerRTL)
                {
                    int temp = elementMargin.Right;
                    elementMargin.Right = elementMargin.Left;
                    elementMargin.Left = temp;
                }

                cellBounds = LayoutUtils.DeflateRect(cellBounds, elementMargin);

                //make sure our sizes are non-negative
                cellBounds.Width = Math.Max(cellBounds.Width, 1);
                cellBounds.Height = Math.Max(cellBounds.Height, 1);

                AnchorStyles anchorStyles = LayoutUtils.GetUnifiedAnchor(element);

                Rectangle elementBounds = LayoutUtils.AlignAndStretch(GetElementSize(element, cellBounds.Size), cellBounds, anchorStyles);

                // If the element was not BoxStretch.Both, AlignAndStretch does not gaurantee
                // that the element has been clipped to the cell bounds.
                elementBounds.Width = Math.Min(cellBounds.Width, elementBounds.Width);
                elementBounds.Height = Math.Min(cellBounds.Height, elementBounds.Height);

                if (isContainerRTL)
                {
                    elementBounds.X = cellBounds.X + (cellBounds.Right - elementBounds.Right);
                }

                element.SetBounds(elementBounds, BoundsSpecified.None);
                if (!isContainerRTL)
                {
                    startX += width;
                }
            }

            Debug_VerifyNoOverlapping(containerInfo.Container);
        }

        internal IArrangedElement GetControlFromPosition(IArrangedElement container, int column, int row)
        {
            if (row < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(row), row, string.Format(SR.InvalidArgument, "RowPosition", row));
            }
            if (column < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(column), column, string.Format(SR.InvalidArgument, "ColumnPosition", column));
            }
            ArrangedElementCollection children = container.Children;
            ContainerInfo containerInfo = GetContainerInfo(container);

            if (children == null || children.Count == 0)
            {
                //nothing in the container. returns null.
                return null;
            }
            if (!containerInfo.Valid)
            {
                //hasn't performed layout yet. assign rows and columns first
                EnsureRowAndColumnAssignments(container, containerInfo, /* doNotCache = */ true);
            }
            for (int i = 0; i < children.Count; i++)
            {
                LayoutInfo layoutInfo = GetLayoutInfo(children[i]);
                //the row and column specified is within the region enclosed by the element.
                if (layoutInfo.ColumnStart <= column && (layoutInfo.ColumnStart + layoutInfo.ColumnSpan - 1) >= column &&
                    layoutInfo.RowStart <= row && (layoutInfo.RowStart + layoutInfo.RowSpan - 1) >= row)
                {
                    return layoutInfo.Element;
                }
            }
            return null;
        }

        internal TableLayoutPanelCellPosition GetPositionFromControl(IArrangedElement container, IArrangedElement child)
        {
            if (container == null || child == null)
            {
                return new TableLayoutPanelCellPosition(-1, -1);
            }
            ArrangedElementCollection children = container.Children;
            ContainerInfo containerInfo = GetContainerInfo(container);

            if (children == null || children.Count == 0)
            {
                //nothing in the container. returns null.
                return new TableLayoutPanelCellPosition(-1, -1);
            }
            if (!containerInfo.Valid)
            {
                //hasn't performed layout yet. assign rows and columns first
                EnsureRowAndColumnAssignments(container, containerInfo, /* doNotCache = */ true);
            }
            LayoutInfo layoutInfo = GetLayoutInfo(child);
            return new TableLayoutPanelCellPosition(layoutInfo.ColumnStart, layoutInfo.RowStart);

        }
        internal static LayoutInfo GetLayoutInfo(IArrangedElement element)
        {
            LayoutInfo layoutInfo = (LayoutInfo)element.Properties.GetObject(_layoutInfoProperty);
            if (layoutInfo == null)
            {
                layoutInfo = new LayoutInfo(element);
                SetLayoutInfo(element, layoutInfo);
            }
            return layoutInfo;
        }

        internal static void SetLayoutInfo(IArrangedElement element, LayoutInfo value)
        {
            element.Properties.SetObject(_layoutInfoProperty, value);
            Debug.Assert(GetLayoutInfo(element) == value, "GetLayoutInfo should return the same value as we set it to");
        }

        ///<summary>
        ///  This class contains layout related information pertaining to a child control of the
        ///  container being laid out. It contains Row,column assignments as well as RowSpan/ColumnSpan.
        ///  This class is used from ContainerInfo as a way of caching information about child controls.
        /// </summary>
        internal sealed class LayoutInfo
        {
            public LayoutInfo(IArrangedElement element)
            {
                Element = element;
            }

            public bool IsAbsolutelyPositioned => RowPosition >= 0 && ColumnPosition >= 0;

            public IArrangedElement Element { get; }

            /// <summary>
            ///  Corresponds to TableLayoutSettings.SetRow. Can be -1 indicating that it is a
            ///  "flow" element and will fit in as necessary. This occurs when a control is
            ///  just added without specific position.
            /// </summary>
            public int RowPosition { get; set; } = -1;

            /// <summary>
            ///  Corresponds to TableLayoutSettings.SetColumn. Can be -1 indicating that it is a
            ///  "flow" element and will fit in as necessary. This occurs when a control is
            ///  just added without specific position.
            /// </summary>
            public int ColumnPosition { get; set; } = -1;

            public int RowStart { get; set; } = -1;

            public int ColumnStart { get; set; } = -1;

            public int ColumnSpan { get; set; } = 1;

            public int RowSpan { get; set; } = 1;

#if DEBUG
            public LayoutInfo Clone() => new LayoutInfo(Element)
            {
                RowStart = RowStart,
                ColumnStart = ColumnStart,
                RowSpan = RowSpan,
                ColumnSpan = ColumnSpan,
                RowPosition = RowPosition,
                ColumnPosition = ColumnPosition
            };

            public override bool Equals(object obj)
            {
                if (!(obj is LayoutInfo other))
                {
                    return false;
                }

                return other.RowStart == RowStart
                    && other.ColumnStart == ColumnStart
                    && other.RowSpan == RowSpan
                    && other.ColumnSpan == ColumnSpan
                    && other.RowPosition == RowPosition
                    && other.ColumnPosition == ColumnPosition;
            }

            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(RowStart);
                hash.Add(ColumnStart);
                hash.Add(RowSpan);
                hash.Add(ColumnSpan);
                hash.Add(RowPosition);
                hash.Add(ColumnPosition);
                return hash.ToHashCode();
            }
#endif
        }

        #region ContainerInfo
        internal static bool HasCachedAssignments(ContainerInfo containerInfo)
        {
            return containerInfo.Valid;
        }

        internal static void ClearCachedAssignments(ContainerInfo containerInfo)
        {
            containerInfo.Valid = false;
        }

        //we make sure that our conatinerInfo never returns null. If there is no
        //existing containerInfo, instantiate a new one and store it in the property
        //store.
        internal static ContainerInfo GetContainerInfo(IArrangedElement container)
        {
            ContainerInfo containerInfo = (ContainerInfo)container.Properties.GetObject(_containerInfoProperty);
            if (containerInfo == null)
            {
                containerInfo = new ContainerInfo(container);
                container.Properties.SetObject(_containerInfoProperty, containerInfo);
            }
            return containerInfo;
        }

        ///<summary>
        ///  this class contains layout related information pertaining to the container
        ///  being laid out by this instance of the TableLayout.  It contains references
        ///  to all the information that should be used from the table layout engine,
        ///  as this class is responsible for caching information about the control and
        ///  it's children being layed out.
        /// </summary>
        internal sealed class ContainerInfo
        {
            private static readonly Strip[] emptyStrip = Array.Empty<Strip>();

            private static readonly int stateValid = BitVector32.CreateMask();
            private static readonly int stateChildInfoValid = BitVector32.CreateMask(stateValid);
            private static readonly int stateChildHasColumnSpan = BitVector32.CreateMask(stateChildInfoValid);
            private static readonly int stateChildHasRowSpan = BitVector32.CreateMask(stateChildHasColumnSpan);

            private int _cellBorderWidth;  //the width for the cell border
            private Strip[] _cols = emptyStrip;
            private Strip[] _rows = emptyStrip;
            private int _maxRows;
            private int _maxColumns;
            private TableLayoutRowStyleCollection _rowStyles;
            private TableLayoutColumnStyleCollection _colStyles;
            private TableLayoutPanelGrowStyle _growStyle;
            private readonly IArrangedElement _container;
            private LayoutInfo[] _childInfo;
            private int _countFixedChildren;
            private int _minRowsAndColumns; // The minimum space required to put all the controls without overlapping
            private int _minColumns; // The minimum number of columns required in order to put all absolutely positioned control on the table
            private int _minRows; // The minimum number of rows required in order to put all absolutely positioned control on the table

            private BitVector32 _state = new BitVector32();

            public ContainerInfo(IArrangedElement container)
            {
                _container = container;
                _growStyle = TableLayoutPanelGrowStyle.AddRows;
            }

            public ContainerInfo(ContainerInfo containerInfo)
            {
                _cellBorderWidth = containerInfo.CellBorderWidth;
                _maxRows = containerInfo.MaxRows;
                _maxColumns = containerInfo.MaxColumns;
                _growStyle = containerInfo.GrowStyle;
                _container = containerInfo.Container;
                _rowStyles = containerInfo.RowStyles;
                _colStyles = containerInfo.ColumnStyles;
            }

            /// <summary>
            ///  the container being laid out
            /// </summary>
            public IArrangedElement Container
            {
                get { return _container; }
            }

            /* Unused
            //indicates whether the user has only specified the row number of the table
            public bool IsRowDefined {
                get { return (_maxRows != 0 && _maxColumns == 0); }
            }
            */

            public int CellBorderWidth
            {
                get { return _cellBorderWidth; }
                set { _cellBorderWidth = value; }
            }

            /// <summary>
            ///  list of ints that represent the sizes of individual columns
            /// </summary>
            public Strip[] Columns
            {
                get { return _cols; }
                set
                {
                    Debug.Assert(_cols.Length != value.Length, "PERF: should not allocate strips, we've already got an array");
                    _cols = value;
                }
            }

            ///
            ///  list of ints that represent the sizes of individual rows
            ///
            public Strip[] Rows
            {
                get { return _rows; }

                set
                {
                    Debug.Assert(_rows.Length != value.Length, "PERF: should not allocate strips, we've already got an array");
                    _rows = value;

                }

            }

            /// <summary>
            ///  Same as TableLayoutSettings.RowCount
            /// </summary>
            public int MaxRows
            {

                get { return _maxRows; }
                set
                {
                    if (_maxRows != value)
                    {
                        _maxRows = value;

                        //invalidate the cache whenever we change the number of rows
                        Valid = false;
                    }

                }
            }

            /// <summary>
            ///  Same as TableLayoutSettings.ColumnCount
            /// </summary>
            public int MaxColumns
            {

                get { return _maxColumns; }

                set
                {
                    if (_maxColumns != value)
                    {
                        _maxColumns = value;

                        //invalidate the cache whenever we change the number of columns
                        Valid = false;
                    }
                }
            }

            ///  Cached information
            public int MinRowsAndColumns
            {
                get
                {
                    Debug.Assert(ChildInfoValid, "Fetching invalid information");
                    return _minRowsAndColumns;
                }
            }

            ///  Cached information
            public int MinColumns
            {
                get
                {
                    Debug.Assert(ChildInfoValid, "Fetching invalid information");
                    return _minColumns;

                }
            }

            ///  Cached information
            public int MinRows
            {
                get
                {
                    Debug.Assert(ChildInfoValid, "Fetching invalid information");
                    return _minRows;

                }
            }

            /// <summary>
            ///  Gets/sets the grow style for our containerinfo.  This
            ///  is used to determine if we will add rows/cols/or throw
            ///  when the table gets full.
            /// </summary>
            public TableLayoutPanelGrowStyle GrowStyle
            {
                get
                {
                    return _growStyle;
                }
                set
                {
                    if (_growStyle != value)
                    {
                        _growStyle = value;
                        Valid = false; // throw away cached row and column assignments
                    }
                }
            }

            public TableLayoutRowStyleCollection RowStyles
            {
                get
                {
                    if (_rowStyles == null)
                    {
                        _rowStyles = new TableLayoutRowStyleCollection(_container);
                    }
                    return _rowStyles;

                }
                set
                {
                    _rowStyles = value;
                    if (_rowStyles != null)
                    {
                        _rowStyles.EnsureOwnership(_container);
                    }

                }
            }

            public TableLayoutColumnStyleCollection ColumnStyles
            {
                get
                {
                    if (_colStyles == null)
                    {
                        _colStyles = new TableLayoutColumnStyleCollection(_container);
                    }
                    return _colStyles;
                }
                set
                {
                    _colStyles = value;
                    if (_colStyles != null)
                    {
                        _colStyles.EnsureOwnership(_container);
                    }
                }
            }

            /// <summary>
            ///  gets cached information about the children of the control being layed out.
            /// </summary>
            public LayoutInfo[] ChildrenInfo
            {
                get
                {
                    if (!_state[stateChildInfoValid])
                    {
                        _countFixedChildren = 0;

                        _minRowsAndColumns = 0;
                        _minColumns = 0;
                        _minRows = 0;
                        ArrangedElementCollection children = Container.Children;
                        LayoutInfo[] childInfo = new LayoutInfo[children.Count];
                        int nonParticipatingElements = 0;
                        int index = 0;
                        for (int i = 0; i < children.Count; i++)
                        {
                            IArrangedElement element = children[i];
                            if (!element.ParticipatesInLayout)
                            {
                                // If the element does not participate in layout (i.e., Visible = false), we
                                // exclude it from the childrenInfos list and it is ignored by the engine.
                                nonParticipatingElements++;
                                continue;
                            }

                            LayoutInfo layoutInfo = GetLayoutInfo(element);
                            if (layoutInfo.IsAbsolutelyPositioned)
                            {
                                _countFixedChildren++;

                            }
                            childInfo[index++] = layoutInfo;
                            _minRowsAndColumns += layoutInfo.RowSpan * layoutInfo.ColumnSpan;
                            if (layoutInfo.IsAbsolutelyPositioned)
                            {
                                _minColumns = Math.Max(_minColumns, layoutInfo.ColumnPosition + layoutInfo.ColumnSpan);
                                _minRows = Math.Max(_minRows, layoutInfo.RowPosition + layoutInfo.RowSpan);
                            }

                        }

                        // shorten the array if necessary.
                        if (nonParticipatingElements > 0)
                        {
                            LayoutInfo[] trimmedChildInfo = new LayoutInfo[childInfo.Length - nonParticipatingElements];
                            Array.Copy(childInfo, trimmedChildInfo, trimmedChildInfo.Length);
                            _childInfo = trimmedChildInfo;
                        }
                        else
                        {
                            _childInfo = childInfo;
                        }
                        _state[stateChildInfoValid] = true;
                    }
                    return _childInfo ?? (Array.Empty<LayoutInfo>());
                }
            }

            public bool ChildInfoValid
            {
                get { return _state[stateChildInfoValid]; }
            }
            public LayoutInfo[] FixedChildrenInfo
            {
                get
                {
                    Debug.Assert(ChildInfoValid, "Fetched invalid information");
                    // we only get this in a cached scenario - so we dont have to worry about caching it.
                    LayoutInfo[] fixedChildren = new LayoutInfo[_countFixedChildren];
                    if (HasChildWithAbsolutePositioning)
                    {
                        int index = 0;
                        for (int i = 0; i < _childInfo.Length; i++)
                        {
                            if (_childInfo[i].IsAbsolutelyPositioned)
                            {
                                fixedChildren[index++] = _childInfo[i];
                            }
                        }
                        Sort(fixedChildren, PreAssignedPositionComparer.GetInstance);
                    }
                    return fixedChildren;
                }
            }
            public bool Valid
            {
                get { return _state[stateValid]; }
                set
                {
                    _state[stateValid] = value;
                    if (!_state[stateValid])
                    {
                        _state[stateChildInfoValid] = false;
                    }
                }
            }

            public bool HasChildWithAbsolutePositioning
            {
                get { return _countFixedChildren > 0; }
            }

            public bool HasMultiplePercentColumns
            {
                get
                {
                    if (_colStyles != null)
                    {
                        bool foundAny = false;
                        foreach (ColumnStyle style in _colStyles)
                        {
                            if (style.SizeType == SizeType.Percent)
                            {
                                if (foundAny)
                                {
                                    return true;
                                }
                                foundAny = true;
                            }
                        }
                    }

                    return false;
                }
            }

            public bool ChildHasColumnSpan
            {
                get { return _state[stateChildHasColumnSpan]; }
                set { _state[stateChildHasColumnSpan] = value; }
            }

            public bool ChildHasRowSpan
            {
                get { return _state[stateChildHasRowSpan]; }
                set { _state[stateChildHasRowSpan] = value; }
            }

            public Size GetCachedPreferredSize(Size proposedContstraints, out bool isValid)
            {
                isValid = false;
                if (proposedContstraints.Height == 0 || proposedContstraints.Width == 0)
                {
                    Size cachedSize = CommonProperties.xGetPreferredSizeCache(Container);

                    if (!cachedSize.IsEmpty)
                    {
                        isValid = true;
                        return cachedSize;
                    }
                }
                return Size.Empty;
            }

        }
        #endregion

        #region SizeProxy implementations
        //sizeProxy. Takes a strip and return its minSize or maxSize accordingly
        private abstract class SizeProxy
        {
            protected Strip strip;
            public Strip Strip
            {
                get { return strip; }
                set { strip = value; }
            }

            public abstract int Size
            {
                get; set;
            }
        }

        private class MinSizeProxy : SizeProxy
        {
            private static readonly MinSizeProxy instance = new MinSizeProxy();
            public override int Size
            {
                get { return strip.MinSize; }
                set { strip.MinSize = value; }
            }

            public static MinSizeProxy GetInstance
            {
                get
                {
                    return instance;
                }
            }
        }

        private class MaxSizeProxy : SizeProxy
        {
            private static readonly MaxSizeProxy instance = new MaxSizeProxy();
            public override int Size
            {
                get { return strip.MaxSize; }
                set { strip.MaxSize = value; }
            }

            public static MaxSizeProxy GetInstance
            {
                get
                {
                    return instance;
                }
            }
        }
        #endregion

        #region ICompare implementations
        private abstract class SpanComparer : IComparer
        {
            public abstract int GetSpan(LayoutInfo layoutInfo);

            public int Compare(object x, object y)
            {
                LayoutInfo xInfo = (LayoutInfo)x;
                LayoutInfo yInfo = (LayoutInfo)y;
                return GetSpan(xInfo) - GetSpan(yInfo);
            }
        }

        private class RowSpanComparer : SpanComparer
        {
            private static readonly RowSpanComparer instance = new RowSpanComparer();

            public override int GetSpan(LayoutInfo layoutInfo)
            {
                return layoutInfo.RowSpan;
            }

            public static RowSpanComparer GetInstance
            {
                get { return instance; }
            }
        }

        private class ColumnSpanComparer : SpanComparer
        {
            private static readonly ColumnSpanComparer instance = new ColumnSpanComparer();

            public override int GetSpan(LayoutInfo layoutInfo)
            {
                return layoutInfo.ColumnSpan;
            }

            public static ColumnSpanComparer GetInstance
            {
                get { return instance; }
            }
        }

        private class PostAssignedPositionComparer : IComparer
        {
            private static readonly PostAssignedPositionComparer instance = new PostAssignedPositionComparer();

            public static PostAssignedPositionComparer GetInstance
            {
                get { return instance; }
            }

            public int Compare(object x, object y)
            {
                LayoutInfo xInfo = (LayoutInfo)x;
                LayoutInfo yInfo = (LayoutInfo)y;
                if (xInfo.RowStart < yInfo.RowStart)
                {
                    return -1;
                }
                if (xInfo.RowStart > yInfo.RowStart)
                {
                    return 1;
                }
                if (xInfo.ColumnStart < yInfo.ColumnStart)
                {
                    return -1;
                }
                if (xInfo.ColumnStart > yInfo.ColumnStart)
                {
                    return 1;
                }
                return 0;
            }
        }

        private class PreAssignedPositionComparer : IComparer
        {
            private static readonly PreAssignedPositionComparer instance = new PreAssignedPositionComparer();

            public static PreAssignedPositionComparer GetInstance
            {
                get { return instance; }
            }

            public int Compare(object x, object y)
            {
                LayoutInfo xInfo = (LayoutInfo)x;
                LayoutInfo yInfo = (LayoutInfo)y;
                if (xInfo.RowPosition < yInfo.RowPosition)
                {
                    return -1;
                }
                if (xInfo.RowPosition > yInfo.RowPosition)
                {
                    return 1;
                }
                if (xInfo.ColumnPosition < yInfo.ColumnPosition)
                {
                    return -1;
                }
                if (xInfo.ColumnPosition > yInfo.ColumnPosition)
                {
                    return 1;
                }
                return 0;
            }
        }
        #endregion

        #region ReservationGrid
        // The ReservationGrid is used to track elements which span rows to prevent overlap.
        private sealed class ReservationGrid
        {
            int _numColumns = 1;
            readonly ArrayList _rows = new ArrayList();

            public bool IsReserved(int column, int rowOffset)
            {
                if (rowOffset >= _rows.Count)
                {
                    return false;
                }
                if (column >= ((BitArray)_rows[rowOffset]).Length)
                {
                    return false;
                }
                return ((BitArray)_rows[rowOffset])[column];
            }

            public void Reserve(int column, int rowOffset)
            {
                Debug.Assert(!IsReserved(column, rowOffset), "we should not be reserving already reserved space.");
                while (rowOffset >= _rows.Count)
                {
                    _rows.Add(new BitArray(_numColumns));
                }
                //Debug.Assert(_numColumns == ((BitArray)_rows[rowOffset]).Length, "length doesn't match");
                //increase the length of the _rows[rowOffset] if necessary
                if (column >= ((BitArray)_rows[rowOffset]).Length)
                {
                    ((BitArray)_rows[rowOffset]).Length = column + 1;
                    if (column >= _numColumns)
                    {
                        _numColumns = column + 1;
                    }
                }

                ((BitArray)_rows[rowOffset])[column] = true;
                Debug.Assert(IsReserved(column, rowOffset), "IsReserved/Reserved mismatch.");
            }

            //reserve all spaces taken by layoutInfo.Element, up till colStop
            public void ReserveAll(LayoutInfo layoutInfo, int rowStop, int colStop)
            {
                for (int rowOffset = 1; rowOffset < rowStop - layoutInfo.RowStart; rowOffset++)
                {
                    for (int reservedCol = layoutInfo.ColumnStart; reservedCol < colStop; reservedCol++)
                    {
                        Reserve(reservedCol, rowOffset);
                    }
                }
            }

            public void AdvanceRow()
            {
                if (_rows.Count > 0)
                {
                    _rows.RemoveAt(0);
                }
            }

        }
        #endregion ReservationGrid

        internal struct Strip
        {
            private int _maxSize;
            private int _minSize;
            private bool _isStart;  //whether there is an element starting in this strip

            public int MinSize
            {
                get { return _minSize; }
                set { _minSize = value; }
            }

            public int MaxSize
            {
                get { return _maxSize; }
                set { _maxSize = value; }
            }

            public bool IsStart
            {
                get { return _isStart; }
                set { _isStart = value; }
            }
        }

        #region DEBUG
        // Verify that the Row/Column assignments on the control are current.
        [Conditional("DEBUG_LAYOUT")]
        private void Debug_VerifyAssignmentsAreCurrent(IArrangedElement container, ContainerInfo containerInfo)
        {
#if DEBUG
            Hashtable oldLayoutInfo = new Hashtable();
            ArrangedElementCollection children = container.Children;
            ArrayList childrenInfo = new ArrayList(children.Count);

            int minSpace = 0;
            int minColumn = 0;
            for (int i = 0; i < children.Count; i++)
            {
                IArrangedElement element = children[i];
                if (!element.ParticipatesInLayout)
                {
                    // If the element does not participate in layout (i.e., Visible = false), we
                    // exclude it from the childrenInfos list and it is ignored by the engine.
                    continue;
                }

                LayoutInfo layoutInfo = GetLayoutInfo(element);
                childrenInfo.Add(layoutInfo);
                minSpace += layoutInfo.RowSpan * layoutInfo.ColumnSpan;
                if (layoutInfo.IsAbsolutelyPositioned)
                {
                    minColumn = Math.Max(minColumn, layoutInfo.ColumnPosition + layoutInfo.ColumnSpan);
                }
            }

            // Create a copy of the layoutInfos so we can restore to our original state
            foreach (LayoutInfo layoutInfo in childrenInfo)
            {
                oldLayoutInfo[layoutInfo.Element] = layoutInfo.Clone();
            }

            Strip[] rows = containerInfo.Rows;
            Strip[] cols = containerInfo.Columns;

            AssignRowsAndColumns(containerInfo);

            Debug.Assert((containerInfo.Columns == null && cols == null) || containerInfo.Columns.Length == cols.Length,
                "Cached assignment info is invalid: Number of required columns has changed.");
            Debug.Assert((containerInfo.Rows == null && rows == null) || containerInfo.Rows.Length == rows.Length,
                "Cached assignment info is invalid: Number of required rows has changed.");

            foreach (LayoutInfo layoutInfo in childrenInfo)
            {
                Debug.Assert(layoutInfo.Equals(oldLayoutInfo[layoutInfo.Element]),
                    "Cached assignment info is invalid: LayoutInfo has changed."
                    + " old layoutinfo: " + ((LayoutInfo)oldLayoutInfo[layoutInfo.Element]).RowStart + " " + ((LayoutInfo)oldLayoutInfo[layoutInfo.Element]).ColumnStart
                    + " new layoutinfo: " + layoutInfo.RowStart + " " + layoutInfo.ColumnStart
                    + " and the element is " + layoutInfo.Element.ToString());
                SetLayoutInfo(layoutInfo.Element, (LayoutInfo)oldLayoutInfo[layoutInfo.Element]);
            }

            // Restore the information in row and column strips. Note that whenever we do a AssignRowAndColumns()
            // we instantiate new row and column strip collections, we have to restore the value back later.

            containerInfo.Rows = rows;
            containerInfo.Columns = cols;
#endif // DEBUG
        }

        // Verifies that there is no overlapping of controls on the table (unless forced to do so via abs. positioning)
        [Conditional("DEBUG_LAYOUT")]
        private void Debug_VerifyNoOverlapping(IArrangedElement container)
        {
            // this code may be useful for debugging, but doesnt work well with
            // row styles

            ArrayList layoutInfos = new ArrayList(container.Children.Count);
            ContainerInfo containerInfo = GetContainerInfo(container);
            Strip[] rows = containerInfo.Rows;
            Strip[] columns = containerInfo.Columns;

            foreach (IArrangedElement element in container.Children)
            {
                if (!element.ParticipatesInLayout)
                {
                    // If the element does not participate in layout (i.e., Visible = false), we
                    // exclude it from the layoutInfos list and it is ignored by the engine.
                    continue;
                }

                layoutInfos.Add(GetLayoutInfo(element));
            }

            for (int i = 0; i < layoutInfos.Count; i++)
            {
                LayoutInfo layoutInfo1 = (LayoutInfo)layoutInfos[i];

                Rectangle elementBounds1 = layoutInfo1.Element.Bounds;
                Rectangle cellsOccupied1 = new Rectangle(layoutInfo1.ColumnStart, layoutInfo1.RowStart, layoutInfo1.ColumnSpan, layoutInfo1.RowSpan);
                for (int j = i + 1; j < layoutInfos.Count; j++)
                {
                    LayoutInfo layoutInfo2 = (LayoutInfo)layoutInfos[j];
                    Rectangle elementBounds2 = layoutInfo2.Element.Bounds;
                    Rectangle cellsOccupied2 = new Rectangle(layoutInfo2.ColumnStart, layoutInfo2.RowStart, layoutInfo2.ColumnSpan, layoutInfo2.RowSpan);
                    Debug.Assert(!cellsOccupied1.IntersectsWith(cellsOccupied2), "controls overlap in the same cell");
                    // The actual control overlaps horizontally. this can only happen if all columns are absolutely sized
                    if (LayoutUtils.IsIntersectHorizontally(elementBounds1, elementBounds2))
                    {
                        int k;
                        Debug.Assert(containerInfo.ColumnStyles.Count >= layoutInfo1.ColumnStart + layoutInfo1.ColumnSpan, "length of column style too short");
                        Debug.Assert(containerInfo.ColumnStyles.Count >= layoutInfo1.ColumnStart + layoutInfo2.ColumnSpan, "length of column style too short");
                        for (k = layoutInfo1.ColumnStart; k < layoutInfo1.ColumnStart + layoutInfo1.ColumnSpan; k++)
                        {
                            Debug.Assert(containerInfo.ColumnStyles[k].SizeType == SizeType.Absolute, "column " + k + " is not absolutely sized");
                        }
                        for (k = layoutInfo2.ColumnStart; k < layoutInfo2.ColumnStart + layoutInfo2.ColumnSpan; k++)
                        {
                            Debug.Assert(containerInfo.ColumnStyles[k].SizeType == SizeType.Absolute, "column " + k + " is not absolutely sized");
                        }
                    }

                    // The actual control overlaps vertically.
                    if (LayoutUtils.IsIntersectVertically(elementBounds1, elementBounds2))
                    {
                        int k;
                        Debug.Assert(containerInfo.RowStyles.Count >= layoutInfo1.RowStart + layoutInfo1.RowSpan, "length of row style too short");
                        Debug.Assert(containerInfo.RowStyles.Count >= layoutInfo2.RowStart + layoutInfo2.RowSpan, "length of row style too short");
                        for (k = layoutInfo1.RowStart; k < layoutInfo1.RowStart + layoutInfo1.RowSpan; k++)
                        {
                            Debug.Assert(containerInfo.RowStyles[k].SizeType == SizeType.Absolute, "column " + k + " is not absolutely sized");
                        }
                        for (k = layoutInfo2.RowStart; k < layoutInfo2.RowStart + layoutInfo2.RowSpan; k++)
                        {
                            Debug.Assert(containerInfo.RowStyles[k].SizeType == SizeType.Absolute, "column " + k + " is not absolutely sized");
                        }
                    }
                }
            }
        }
        #endregion
    }
}
