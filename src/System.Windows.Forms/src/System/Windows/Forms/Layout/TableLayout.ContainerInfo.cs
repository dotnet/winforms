// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    /// <summary>
    ///  this class contains layout related information pertaining to the container
    ///  being laid out by this instance of the TableLayout. It contains references
    ///  to all the information that should be used from the table layout engine,
    ///  as this class is responsible for caching information about the control and
    ///  it's children being layed out.
    /// </summary>
    internal sealed class ContainerInfo
    {
        private static readonly Strip[] s_emptyStrip = [];

        private static readonly int s_stateValid = BitVector32.CreateMask();
        private static readonly int s_stateChildInfoValid = BitVector32.CreateMask(s_stateValid);
        private static readonly int s_stateChildHasColumnSpan = BitVector32.CreateMask(s_stateChildInfoValid);
        private static readonly int s_stateChildHasRowSpan = BitVector32.CreateMask(s_stateChildHasColumnSpan);

        private int _cellBorderWidth;  // the width for the cell border
        private Strip[] _cols = s_emptyStrip;
        private Strip[] _rows = s_emptyStrip;
        private int _maxRows;
        private int _maxColumns;
        private TableLayoutRowStyleCollection? _rowStyles;
        private TableLayoutColumnStyleCollection? _colStyles;
        private TableLayoutPanelGrowStyle _growStyle;
        private readonly IArrangedElement _container;
        private LayoutInfo[]? _childInfo;
        private int _countFixedChildren;
        private int _minRowsAndColumns; // The minimum space required to put all the controls without overlapping
        private int _minColumns; // The minimum number of columns required in order to put all absolutely positioned control on the table
        private int _minRows; // The minimum number of rows required in order to put all absolutely positioned control on the table

        private BitVector32 _state;

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

                    // invalidate the cache whenever we change the number of rows
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

                    // invalidate the cache whenever we change the number of columns
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
        ///  Gets/sets the grow style for our ContainerInfo. This
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

        [AllowNull]
        public TableLayoutRowStyleCollection RowStyles
        {
            get
            {
                _rowStyles ??= new TableLayoutRowStyleCollection(_container);

                return _rowStyles;
            }
            set
            {
                _rowStyles = value;
                _rowStyles?.EnsureOwnership(_container);
            }
        }

        [AllowNull]
        public TableLayoutColumnStyleCollection ColumnStyles
        {
            get
            {
                _colStyles ??= new TableLayoutColumnStyleCollection(_container);

                return _colStyles;
            }
            set
            {
                _colStyles = value;
                _colStyles?.EnsureOwnership(_container);
            }
        }

        /// <summary>
        ///  gets cached information about the children of the control being layed out.
        /// </summary>
        public LayoutInfo[] ChildrenInfo
        {
            get
            {
                if (!_state[s_stateChildInfoValid])
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

                    _state[s_stateChildInfoValid] = true;
                }

                return _childInfo ?? [];
            }
        }

        public bool ChildInfoValid
        {
            get { return _state[s_stateChildInfoValid]; }
        }

        public LayoutInfo[] FixedChildrenInfo
        {
            get
            {
                Debug.Assert(ChildInfoValid, "Fetched invalid information");
                // we only get this in a cached scenario - so we don't have to worry about caching it.
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
            get { return _state[s_stateValid]; }
            set
            {
                _state[s_stateValid] = value;
                if (!_state[s_stateValid])
                {
                    _state[s_stateChildInfoValid] = false;
                }
            }
        }

        [MemberNotNullWhen(true, nameof(_childInfo))]
        public bool HasChildWithAbsolutePositioning
        {
            get { return _countFixedChildren > 0; }
        }

        public bool HasMultiplePercentColumns
        {
            get
            {
                if (_colStyles is not null)
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
            get { return _state[s_stateChildHasColumnSpan]; }
            set { _state[s_stateChildHasColumnSpan] = value; }
        }

        public bool ChildHasRowSpan
        {
            get { return _state[s_stateChildHasRowSpan]; }
            set { _state[s_stateChildHasRowSpan] = value; }
        }

        public Size GetCachedPreferredSize(Size proposedConstraints, out bool isValid)
        {
            isValid = false;
            if (proposedConstraints.Height == 0 || proposedConstraints.Width == 0)
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
}
