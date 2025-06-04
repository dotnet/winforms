// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

internal class TableLayoutPanelBehavior : Behavior
{
    private readonly TableLayoutPanelDesigner _designer; // pointer back to our designer.
    private Point _lastMouseLoc; // used to track mouse movement deltas
    private bool _pushedBehavior; // tracks if we've pushed ourself onto the stack
    private readonly BehaviorService _behaviorService; // used for bounds translation
    private readonly IServiceProvider _serviceProvider; // cached to allow our behavior to get services
    private TableLayoutPanelResizeGlyph _tableGlyph; // the glyph being resized
    private DesignerTransaction _resizeTransaction; // used to make size adjustments within transaction
    private PropertyDescriptor _resizeProp; // cached property descriptor representing either the row or column styles
    private PropertyDescriptor _changedProp;  // cached property descriptor that refers to the RowSTyles or ColumnStyles collection.
    private readonly TableLayoutPanel _table;
    private StyleHelper _rightStyle;
    private StyleHelper _leftStyle;
    private List<TableLayoutStyle> _styles;
    private bool _currentColumnStyles; // is Styles for Columns or Rows

    internal TableLayoutPanelBehavior(TableLayoutPanel panel, TableLayoutPanelDesigner designer, IServiceProvider serviceProvider)
    {
        _table = panel;
        _designer = designer;
        _serviceProvider = serviceProvider;

        _behaviorService = serviceProvider.GetService(typeof(BehaviorService)) as BehaviorService;

        if (_behaviorService is null)
        {
            Debug.Fail("BehaviorService could not be found!");
            return;
        }

        _pushedBehavior = false;
        _lastMouseLoc = Point.Empty;
    }

    private void FinishResize()
    {
        // clear state
        _pushedBehavior = false;
        _behaviorService.PopBehavior(this);
        _lastMouseLoc = Point.Empty;
        _styles = null;

        // fire ComponentChange events so this event is undoable
        IComponentChangeService cs = _serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        if (cs is not null && _changedProp is not null)
        {
            cs.OnComponentChanged(_table, _changedProp, null, null);
            _changedProp = null;
        }

        // attempt to refresh the selection
        SelectionManager selManager = _serviceProvider.GetService(typeof(SelectionManager)) as SelectionManager;
        selManager?.Refresh();
    }

    public override void OnLoseCapture(Glyph g, EventArgs e)
    {
        if (_pushedBehavior)
        {
            FinishResize();

            // If we still have a transaction, roll it back.
            if (_resizeTransaction is not null)
            {
                DesignerTransaction t = _resizeTransaction;
                _resizeTransaction = null;
                using (t)
                {
                    t.Cancel();
                }
            }
        }
    }

    public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
    {
        // we only care about the right mouse button for resizing
        if (button == MouseButtons.Left && g is TableLayoutPanelResizeGlyph)
        {
            _tableGlyph = g as TableLayoutPanelResizeGlyph;

            // select the table
            ISelectionService selSvc = _serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
            selSvc?.SetSelectedComponents(new object[] { _designer.Component }, SelectionTypes.Primary);

            bool isColumn = _tableGlyph.Type == TableLayoutPanelResizeGlyph.TableLayoutResizeType.Column;

            // cache some state
            _lastMouseLoc = mouseLoc;
            _resizeProp = TypeDescriptor.GetProperties(_tableGlyph.Style)[isColumn ? "Width" : "Height"];
            Debug.Assert(_resizeProp is not null, "Unable to get the resize property for tableGlyph's Style");

            IComponentChangeService cs = _serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (cs is not null)
            {
                _changedProp = TypeDescriptor.GetProperties(_table)[isColumn ? "ColumnStyles" : "RowStyles"];
                int[] widths = isColumn ? _table.GetColumnWidths() : _table.GetRowHeights();

                if (_changedProp is not null)
                {
                    GetActiveStyleCollection(isColumn);
                    if (_styles is not null && CanResizeStyle(widths))
                    {
                        IDesignerHost host = _serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
                        if (host is not null)
                        {
                            _resizeTransaction = host.CreateTransaction(string.Format(SR.TableLayoutPanelRowColResize, (isColumn ? "Column" : "Row"), _designer.Control.Site.Name));
                        }

                        try
                        {
                            int moveIndex = _styles.IndexOf(_tableGlyph.Style);
                            _rightStyle.index = IndexOfNextStealableStyle(true /*forward*/, moveIndex, widths);
                            _rightStyle.style = _styles[_rightStyle.index];
                            _rightStyle.styleProp = TypeDescriptor.GetProperties(_rightStyle.style)[isColumn ? "Width" : "Height"];

                            _leftStyle.index = IndexOfNextStealableStyle(false /*backwards*/, moveIndex, widths);
                            _leftStyle.style = _styles[_leftStyle.index];
                            _leftStyle.styleProp = TypeDescriptor.GetProperties(_leftStyle.style)[isColumn ? "Width" : "Height"];

                            Debug.Assert(_leftStyle.styleProp is not null && _rightStyle.styleProp is not null, "Couldn't find property descriptor for width or height");

                            cs.OnComponentChanging(_table, _changedProp);
                        }
                        catch (CheckoutException checkoutException)
                        {
                            if (CheckoutException.Canceled.Equals(checkoutException))
                            {
                                if ((_resizeTransaction is not null) && (!_resizeTransaction.Canceled))
                                {
                                    _resizeTransaction.Cancel();
                                }
                            }

                            throw;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // push this resizebehavior
            _behaviorService.PushCaptureBehavior(this);
            _pushedBehavior = true;
        }

        return false;
    }

    private void GetActiveStyleCollection(bool isColumn)
    {
        if ((_styles is null || isColumn != _currentColumnStyles) && _table is not null)
        {
            _styles = [..((TableLayoutStyleCollection)_changedProp.GetValue(_table)).Cast<TableLayoutStyle>()];
            _currentColumnStyles = isColumn;
        }
    }

    private bool ColumnResize
    {
        get
        {
            bool ret = false;
            if (_tableGlyph is not null)
            {
                ret = _tableGlyph.Type == TableLayoutPanelResizeGlyph.TableLayoutResizeType.Column;
            }

            return ret;
        }
    }

    private bool CanResizeStyle(int[] widths)
    {
        int moveIndex = _styles.IndexOf(_tableGlyph.Style);
        if (moveIndex <= -1 || moveIndex == _styles.Count)
        {
            Debug.Fail($"Can't find style {moveIndex}");
            return false;
        }

        bool canStealFromRight = IndexOfNextStealableStyle(true, moveIndex, widths) != -1;
        bool canStealFromLeft = IndexOfNextStealableStyle(false, moveIndex, widths) != -1;

        return canStealFromRight && canStealFromLeft;
    }

    private int IndexOfNextStealableStyle(bool forward, int startIndex, int[] widths)
    {
        int stealIndex = -1;

        if (_styles is not null)
        {
            if (forward)
            {
                for (int i = startIndex + 1; ((i < _styles.Count) && (i < widths.Length)); i++)
                {
                    if (_styles[i].SizeType != SizeType.AutoSize && widths[i] >= DesignerUtils.s_minimumSizeDrag)
                    {
                        stealIndex = i;
                        break;
                    }
                }
            }
            else
            {
                if (startIndex < widths.Length)
                {
                    for (int i = startIndex; i >= 0; i--)
                    {
                        if (_styles[i].SizeType != SizeType.AutoSize && widths[i] >= DesignerUtils.s_minimumSizeDrag)
                        {
                            stealIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        return stealIndex;
    }

    public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
    {
        if (_pushedBehavior)
        {
            bool isColumn = ColumnResize;
            GetActiveStyleCollection(isColumn);
            if (_styles is not null)
            {
                int rightIndex = _rightStyle.index;
                int leftIndex = _leftStyle.index;

                int delta = isColumn ? mouseLoc.X - _lastMouseLoc.X : mouseLoc.Y - _lastMouseLoc.Y;
                if (isColumn && _table.RightToLeft == RightToLeft.Yes)
                {
                    delta *= -1;
                }

                if (delta == 0)
                {
                    return false;
                }

                int[] oldWidths = isColumn ? _table.GetColumnWidths() : _table.GetRowHeights();

                int[] newWidths = oldWidths.Clone() as int[];

                newWidths[rightIndex] -= delta;
                newWidths[leftIndex] += delta;

                if (newWidths[rightIndex] < DesignerUtils.s_minimumSizeDrag ||
                    newWidths[leftIndex] < DesignerUtils.s_minimumSizeDrag)
                {
                    return false;
                }

                // now we must renormalize our new widths into the correct sizes
                _table.SuspendLayout();

                int totalPercent = 0;

                // simplest case: two absolute columns just affect each other.
                if (_styles[rightIndex].SizeType == SizeType.Absolute &&
                    _styles[leftIndex].SizeType == SizeType.Absolute)
                {
                    // VSWhidbey 465751
                    // The dimensions reported by GetColumnsWidths() are different
                    // than the style dimensions when the TLP has borders. Instead
                    // of always setting the new size directly based on the reported
                    // sizes, we now base them on the style size if necessary.
                    float newRightSize = newWidths[rightIndex];
                    float rightStyleSize = (float)_rightStyle.styleProp.GetValue(_rightStyle.style);

                    if (rightStyleSize != oldWidths[rightIndex])
                    {
                        newRightSize = Math.Max(rightStyleSize - delta, DesignerUtils.s_minimumSizeDrag);
                    }

                    float newLeftSize = newWidths[leftIndex];
                    float leftStyleSize = (float)_leftStyle.styleProp.GetValue(_leftStyle.style);

                    if (leftStyleSize != oldWidths[leftIndex])
                    {
                        newLeftSize = Math.Max(leftStyleSize + delta, DesignerUtils.s_minimumSizeDrag);
                    }

                    _rightStyle.styleProp.SetValue(_rightStyle.style, newRightSize);
                    _leftStyle.styleProp.SetValue(_leftStyle.style, newLeftSize);
                }
                else if (_styles[rightIndex].SizeType == SizeType.Percent &&
                    _styles[leftIndex].SizeType == SizeType.Percent)
                {
                    for (int i = 0; i < _styles.Count; i++)
                    {
                        if (_styles[i].SizeType == SizeType.Percent)
                        {
                            totalPercent += oldWidths[i];
                        }
                    }

                    for (int j = 0; j < 2; j++)
                    {
                        int i = j == 0 ? rightIndex : leftIndex;
                        float newSize = (float)newWidths[i] * 100 / totalPercent;

                        PropertyDescriptor prop = TypeDescriptor.GetProperties(_styles[i])[isColumn ? "Width" : "Height"];
                        prop?.SetValue(_styles[i], newSize);
                    }
                }
                else
                {
                    // mixed - just update absolute
                    int absIndex = _styles[rightIndex].SizeType == SizeType.Absolute ? rightIndex : leftIndex;
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(_styles[absIndex])[isColumn ? "Width" : "Height"];
                    if (prop is not null)
                    {
                        // VSWhidbey 465751
                        // The dimensions reported by GetColumnsWidths() are different
                        // than the style dimensions when the TLP has borders. Instead
                        // of always setting the new size directly based on the reported
                        // sizes, we now base them on the style size if necessary.
                        float newAbsSize = newWidths[absIndex];
                        float curAbsStyleSize = (float)prop.GetValue(_styles[absIndex]);

                        if (curAbsStyleSize != oldWidths[absIndex])
                        {
                            newAbsSize = Math.Max(absIndex == rightIndex ? curAbsStyleSize - delta : curAbsStyleSize + delta,
                                                    DesignerUtils.s_minimumSizeDrag);
                        }

                        prop.SetValue(_styles[absIndex], newAbsSize);
                    }
                    else
                    {
                        Debug.Fail("Can't resize - no propertyescriptor for column");
                    }
                }

                _table.ResumeLayout(true);

                // now determine if the values we pushed into the TLP
                // actually had any effect. If they didn't,
                // we delay updating the last mouse position so that
                // next time a mouse move message comes in the delta is larger.
                bool updatedSize = true;
                int[] updatedWidths = isColumn ? _table.GetColumnWidths() : _table.GetRowHeights();

                for (int i = 0; i < updatedWidths.Length; i++)
                {
                    if (updatedWidths[i] == oldWidths[i] && newWidths[i] != oldWidths[i])
                    {
                        updatedSize = false;
                    }
                }

                if (updatedSize)
                {
                    _lastMouseLoc = mouseLoc;
                }
            }
            else
            {
                _lastMouseLoc = mouseLoc;
            }
        }

        return false;
    }

    public override bool OnMouseUp(Glyph g, MouseButtons button)
    {
        if (_pushedBehavior)
        {
            FinishResize();
            // commit transaction
            if (_resizeTransaction is not null)
            {
                DesignerTransaction t = _resizeTransaction;
                _resizeTransaction = null;
                using (t)
                {
                    t.Commit();
                }

                _resizeProp = null;
            }
        }

        return false;
    }

    internal struct StyleHelper
    {
        public int index;
        public PropertyDescriptor styleProp;
        public TableLayoutStyle style;
    }
}
