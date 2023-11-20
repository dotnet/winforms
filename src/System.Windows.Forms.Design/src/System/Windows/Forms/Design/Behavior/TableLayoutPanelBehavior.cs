// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms.Design.Behavior;

internal class TableLayoutPanelBehavior : Behavior
{
    private TableLayoutPanelDesigner designer; //pointer back to our designer.
    private Point lastMouseLoc; //used to track mouse movement deltas
    private bool pushedBehavior; //tracks if we've pushed ourself onto the stack
    private BehaviorService behaviorService; //used for bounds translation
    private IServiceProvider serviceProvider; //cached to allow our behavior to get services
    private TableLayoutPanelResizeGlyph tableGlyph; //the glyph being resized
    private DesignerTransaction resizeTransaction; //used to make size adjustments within transaction
    private PropertyDescriptor resizeProp; //cached property descriptor representing either the row or column styles
    private PropertyDescriptor changedProp;  //cached property descriptor that refers to the RowSTyles or ColumnStyles collection.
    private TableLayoutPanel table;
    private StyleHelper rightStyle;
    private StyleHelper leftStyle;
    private List<TableLayoutStyle> _styles;
    private bool currentColumnStyles; // is Styles for Columns or Rows
    private static readonly TraceSwitch tlpResizeSwitch = new("TLPRESIZE", "Behavior service drag & drop messages");

    internal TableLayoutPanelBehavior(TableLayoutPanel panel, TableLayoutPanelDesigner designer, IServiceProvider serviceProvider)
    {
        table = panel;
        this.designer = designer;
        this.serviceProvider = serviceProvider;

        behaviorService = serviceProvider.GetService(typeof(BehaviorService)) as BehaviorService;

        if (behaviorService is null)
        {
            Debug.Fail("BehaviorService could not be found!");
            return;
        }

        pushedBehavior = false;
        lastMouseLoc = Point.Empty;
    }

    private void FinishResize()
    {
        //clear state
        pushedBehavior = false;
        behaviorService.PopBehavior(this);
        lastMouseLoc = Point.Empty;
        _styles = null;

        // fire ComponentChange events so this event is undoable
        IComponentChangeService cs = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        if (cs is not null && changedProp is not null)
        {
            cs.OnComponentChanged(table, changedProp, null, null);
            changedProp = null;
        }

        //attempt to refresh the selection
        SelectionManager selManager = serviceProvider.GetService(typeof(SelectionManager)) as SelectionManager;
        selManager?.Refresh();
    }

    public override void OnLoseCapture(Glyph g, EventArgs e)
    {
        if (pushedBehavior)
        {
            FinishResize();

            // If we still have a transaction, roll it back.
            if (resizeTransaction is not null)
            {
                DesignerTransaction t = resizeTransaction;
                resizeTransaction = null;
                using (t)
                {
                    t.Cancel();
                }
            }
        }
    }

    public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
    {
        //we only care about the right mouse button for resizing
        if (button == MouseButtons.Left && g is TableLayoutPanelResizeGlyph)
        {
            tableGlyph = g as TableLayoutPanelResizeGlyph;

            //select the table
            ISelectionService selSvc = serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
            selSvc?.SetSelectedComponents(new object[] { designer.Component }, SelectionTypes.Primary);

            bool isColumn = tableGlyph.Type == TableLayoutPanelResizeGlyph.TableLayoutResizeType.Column;

            //cache some state
            lastMouseLoc = mouseLoc;
            resizeProp = TypeDescriptor.GetProperties(tableGlyph.Style)[isColumn ? "Width" : "Height"];
            Debug.Assert(resizeProp is not null, "Unable to get the resize property for tableGlyph's Style");

            IComponentChangeService cs = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (cs is not null)
            {
                changedProp = TypeDescriptor.GetProperties(table)[isColumn ? "ColumnStyles" : "RowStyles"];
                int[] widths = isColumn ? table.GetColumnWidths() : table.GetRowHeights();

                if (changedProp is not null)
                {
                    GetActiveStyleCollection(isColumn);
                    if (_styles is not null && CanResizeStyle(widths))
                    {
                        IDesignerHost host = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
                        if (host is not null)
                        {
                            resizeTransaction = host.CreateTransaction(string.Format(SR.TableLayoutPanelRowColResize, (isColumn ? "Column" : "Row"), designer.Control.Site.Name));
                        }

                        try
                        {
                            int moveIndex = _styles.IndexOf(tableGlyph.Style);
                            rightStyle.index = IndexOfNextStealableStyle(true /*forward*/, moveIndex, widths);
                            rightStyle.style = _styles[rightStyle.index];
                            rightStyle.styleProp = TypeDescriptor.GetProperties(rightStyle.style)[isColumn ? "Width" : "Height"];

                            leftStyle.index = IndexOfNextStealableStyle(false /*backwards*/, moveIndex, widths);
                            leftStyle.style = _styles[leftStyle.index];
                            leftStyle.styleProp = TypeDescriptor.GetProperties(leftStyle.style)[isColumn ? "Width" : "Height"];

                            Debug.Assert(leftStyle.styleProp is not null && rightStyle.styleProp is not null, "Couldn't find property descriptor for width or height");

                            cs.OnComponentChanging(table, changedProp);
                        }
                        catch (CheckoutException checkoutException)
                        {
                            if (CheckoutException.Canceled.Equals(checkoutException))
                            {
                                if ((resizeTransaction is not null) && (!resizeTransaction.Canceled))
                                {
                                    resizeTransaction.Cancel();
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

            //push this resizebehavior
            behaviorService.PushCaptureBehavior(this);
            pushedBehavior = true;
        }

        return false;
    }

    private void GetActiveStyleCollection(bool isColumn)
    {
        if ((_styles is null || isColumn != currentColumnStyles) && table is not null)
        {
            _styles = ((TableLayoutStyleCollection)changedProp.GetValue(table)).Cast<TableLayoutStyle>().ToList();
            currentColumnStyles = isColumn;
        }
    }

    private bool ColumnResize
    {
        get
        {
            bool ret = false;
            if (tableGlyph is not null)
            {
                ret = tableGlyph.Type == TableLayoutPanelResizeGlyph.TableLayoutResizeType.Column;
            }

            return ret;
        }
    }

    private bool CanResizeStyle(int[] widths)
    {
        int moveIndex = _styles.IndexOf(tableGlyph.Style);
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
                    if (_styles[i].SizeType != SizeType.AutoSize && widths[i] >= DesignerUtils.MINUMUMSTYLESIZEDRAG)
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
                        if (_styles[i].SizeType != SizeType.AutoSize && widths[i] >= DesignerUtils.MINUMUMSTYLESIZEDRAG)
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
        if (pushedBehavior)
        {
            bool isColumn = ColumnResize;
            GetActiveStyleCollection(isColumn);
            if (_styles is not null)
            {
                int rightIndex = rightStyle.index;
                int leftIndex = leftStyle.index;

                int delta = isColumn ? mouseLoc.X - lastMouseLoc.X : mouseLoc.Y - lastMouseLoc.Y;
                if (isColumn && table.RightToLeft == RightToLeft.Yes)
                {
                    delta *= -1;
                }

                if (delta == 0)
                {
                    Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose, "0 mouse delta");
                    return false;
                }

                Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose, "BEGIN RESIZE");
                Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose, "mouse delta: " + delta);

                int[] oldWidths = isColumn ? table.GetColumnWidths() : table.GetRowHeights();

                int[] newWidths = oldWidths.Clone() as int[];

                newWidths[rightIndex] -= delta;
                newWidths[leftIndex] += delta;

                if (newWidths[rightIndex] < DesignerUtils.MINUMUMSTYLESIZEDRAG ||
                    newWidths[leftIndex] < DesignerUtils.MINUMUMSTYLESIZEDRAG)
                {
                    Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose, "Bottomed out.");
                    Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose, "END RESIZE\n");
                    return false;
                }

                // now we must renormalize our new widths into the correct sizes
                table.SuspendLayout();

                int totalPercent = 0;

                //simplest case: two absolute columns just affect each other.
                if (_styles[rightIndex].SizeType == SizeType.Absolute &&
                    _styles[leftIndex].SizeType == SizeType.Absolute)
                {
                    // VSWhidbey 465751
                    // The dimensions reported by GetColumnsWidths() are different
                    // than the style dimensions when the TLP has borders. Instead
                    // of always setting the new size directly based on the reported
                    // sizes, we now base them on the style size if necessary.
                    float newRightSize = newWidths[rightIndex];
                    float rightStyleSize = (float)rightStyle.styleProp.GetValue(rightStyle.style);

                    if (rightStyleSize != oldWidths[rightIndex])
                    {
                        newRightSize = Math.Max(rightStyleSize - delta, DesignerUtils.MINUMUMSTYLESIZEDRAG);
                    }

                    float newLeftSize = newWidths[leftIndex];
                    float leftStyleSize = (float)leftStyle.styleProp.GetValue(leftStyle.style);

                    if (leftStyleSize != oldWidths[leftIndex])
                    {
                        newLeftSize = Math.Max(leftStyleSize + delta, DesignerUtils.MINUMUMSTYLESIZEDRAG);
                    }

                    rightStyle.styleProp.SetValue(rightStyle.style, newRightSize);
                    leftStyle.styleProp.SetValue(leftStyle.style, newLeftSize);
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
                        float newSize = (float)newWidths[i] * 100 / (float)totalPercent;
                        Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose, "NewSize " + newSize);

                        PropertyDescriptor prop = TypeDescriptor.GetProperties(_styles[i])[isColumn ? "Width" : "Height"];
                        if (prop is not null)
                        {
                            prop.SetValue(_styles[i], newSize);
                            Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose, "Resizing column (per) " + i.ToString(CultureInfo.InvariantCulture) + " to " + newSize.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                }
                else
                {
#if DEBUG
                    for (int i = 0; i < oldWidths.Length; i++)
                    {
                        Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose, "Col " + i + ": Old: " + oldWidths[i] + " New: " + newWidths[i]);
                    }
#endif

                    //mixed - just update absolute
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
                                                    DesignerUtils.MINUMUMSTYLESIZEDRAG);
                        }

                        prop.SetValue(_styles[absIndex], newAbsSize);
                        Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose, "Resizing column (abs) " + absIndex.ToString(CultureInfo.InvariantCulture) + " to " + newWidths[absIndex]);
                    }
                    else
                    {
                        Debug.Fail("Can't resize - no propertyescriptor for column");
                    }
                }

                table.ResumeLayout(true);

                // now determine if the values we pushed into the TLP
                // actually had any effect.  If they didn't,
                // we delay updating the last mouse position so that
                // next time a mouse move message comes in the delta is larger.
                bool updatedSize = true;
                int[] updatedWidths = isColumn ? table.GetColumnWidths() : table.GetRowHeights();

                for (int i = 0; i < updatedWidths.Length; i++)
                {
                    if (updatedWidths[i] == oldWidths[i] && newWidths[i] != oldWidths[i])
                    {
                        updatedSize = false;
                    }
                }

                if (updatedSize)
                {
                    lastMouseLoc = mouseLoc;
                }
            }
            else
            {
                lastMouseLoc = mouseLoc;
            }
        }

        Debug.WriteLineIf(tlpResizeSwitch.TraceVerbose && pushedBehavior, "END RESIZE\n");
        return false;
    }

    public override bool OnMouseUp(Glyph g, MouseButtons button)
    {
        if (pushedBehavior)
        {
            FinishResize();
            //commit transaction
            if (resizeTransaction is not null)
            {
                DesignerTransaction t = resizeTransaction;
                resizeTransaction = null;
                using (t)
                {
                    t.Commit();
                }

                resizeProp = null;
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
