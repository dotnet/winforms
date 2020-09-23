// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class UpDownBase
    {
        /// <summary>
        ///  A control representing the pair of buttons on the end of the upDownEdit control. This class handles
        ///  drawing the updown buttons, and detecting mouse actions on these buttons. Acceleration on the buttons is
        ///  handled. The control sends UpDownEventArgss to the parent UpDownBase class when a button is pressed, or
        ///  when the acceleration determines that another event should be generated.
        /// </summary>
        internal class UpDownButtons : Control
        {
            private readonly UpDownBase _parent;

            // Button state
            private ButtonID _pushed = ButtonID.None;
            private ButtonID _captured = ButtonID.None;
            private ButtonID _mouseOver = ButtonID.None;

            private UpDownEventHandler _upDownEventHandler;

            private Timer _timer; // generates UpDown events
            private int _timerInterval; // milliseconds between events

            private bool _doubleClickFired;

            internal UpDownButtons(UpDownBase parent) : base()
            {
                SetStyle(ControlStyles.Opaque | ControlStyles.FixedHeight | ControlStyles.FixedWidth, true);
                SetStyle(ControlStyles.Selectable, false);

                _parent = parent;
            }

            /// <summary>
            ///  Adds a handler for the updown button event.
            /// </summary>
            public event UpDownEventHandler UpDown
            {
                add => _upDownEventHandler += value;
                remove => _upDownEventHandler -= value;
            }

            /// <remarks>
            ///  Called when the mouse button is pressed - we need to start spinning the value of the updown.
            /// </remarks>
            private void BeginButtonPress(MouseEventArgs e)
            {
                int half_height = Size.Height / 2;

                if (e.Y < half_height)
                {
                    // Up button
                    _pushed = _captured = ButtonID.Up;
                    Invalidate();
                }
                else
                {
                    // Down button
                    _pushed = _captured = ButtonID.Down;
                    Invalidate();
                }

                // Capture the mouse
                Capture = true;

                // Generate UpDown event
                OnUpDown(new UpDownEventArgs((int)_pushed));

                // Start the timer for new updown events
                StartTimer();
            }

            protected override AccessibleObject CreateAccessibilityInstance()
                => new UpDownButtonsAccessibleObject(this);

            /// <remarks>
            ///  Called when the mouse button is released - we need to stop spinning the value of the updown.
            /// </remarks>
            private void EndButtonPress()
            {
                _pushed = ButtonID.None;
                _captured = ButtonID.None;

                // Stop the timer
                StopTimer();

                // Release the mouse
                Capture = false;

                // Redraw the buttons
                Invalidate();
            }

            /// <summary>
            ///  Handles detecting mouse hits on the buttons. This method detects
            ///  which button was hit (up or down), fires a updown event, captures
            ///  the mouse, and starts a timer for repeated updown events.
            /// </summary>
            protected override void OnMouseDown(MouseEventArgs e)
            {
                // Begin spinning the value
                // Focus the parent
                _parent.Focus();

                if (!_parent.ValidationCancelled && e.Button == MouseButtons.Left)
                {
                    BeginButtonPress(e);
                }

                if (e.Clicks == 2 && e.Button == MouseButtons.Left)
                {
                    _doubleClickFired = true;
                }

                // At no stage should a button be pushed, and the mouse
                // not captured.
                Debug.Assert(
                    !(_pushed != ButtonID.None && _captured == ButtonID.None),
                    "Invalid button pushed/captured combination");

                _parent.OnMouseDown(_parent.TranslateMouseEvent(this, e));
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                // If the mouse is captured by the buttons (i.e. an updown button
                // was pushed, and the mouse button has not yet been released),
                // determine the new state of the buttons depending on where
                // the mouse pointer has moved.

                if (Capture)
                {
                    // Determine button area
                    Rectangle rect = ClientRectangle;
                    rect.Height /= 2;

                    if (_captured == ButtonID.Down)
                    {
                        rect.Y += rect.Height;
                    }

                    // Test if the mouse has moved outside the button area
                    if (rect.Contains(e.X, e.Y))
                    {
                        // Inside button, repush the button if necessary

                        if (_pushed != _captured)
                        {
                            // Restart the timer
                            StartTimer();

                            _pushed = _captured;
                            Invalidate();
                        }
                    }
                    else
                    {
                        // Outside button
                        //
                        // Retain the capture, but pop the button up whilst the mouse
                        // remains outside the button and the mouse button remains pressed.
                        if (_pushed != ButtonID.None)
                        {
                            // Stop the timer for updown events
                            StopTimer();

                            _pushed = ButtonID.None;
                            Invalidate();
                        }
                    }
                }

                // Logic for seeing which button is Hot if any
                Rectangle rectUp = ClientRectangle, rectDown = ClientRectangle;
                rectUp.Height /= 2;
                rectDown.Y += rectDown.Height / 2;

                // Check if the mouse is on the upper or lower button. Note that it could be in neither.
                if (rectUp.Contains(e.X, e.Y))
                {
                    _mouseOver = ButtonID.Up;
                    Invalidate();
                }
                else if (rectDown.Contains(e.X, e.Y))
                {
                    _mouseOver = ButtonID.Down;
                    Invalidate();
                }

                // At no stage should a button be pushed, and the mouse not captured.
                Debug.Assert(
                    !(_pushed != ButtonID.None && _captured == ButtonID.None),
                    "Invalid button pushed/captured combination");

                _parent.OnMouseMove(_parent.TranslateMouseEvent(this, e));
            }

            /// <summary>
            ///  Handles detecting when the mouse button is released.
            /// </summary>
            protected override void OnMouseUp(MouseEventArgs e)
            {
                if (!_parent.ValidationCancelled && e.Button == MouseButtons.Left)
                {
                    EndButtonPress();
                }

                // At no stage should a button be pushed, and the mouse not captured.
                Debug.Assert(
                    !(_pushed != ButtonID.None && _captured == ButtonID.None),
                    "Invalid button pushed/captured combination");

                Point pt = new Point(e.X, e.Y);
                pt = PointToScreen(pt);

                MouseEventArgs me = _parent.TranslateMouseEvent(this, e);
                if (e.Button == MouseButtons.Left)
                {
                    if (!_parent.ValidationCancelled && User32.WindowFromPoint(pt) == Handle)
                    {
                        if (!_doubleClickFired)
                        {
                            _parent.OnClick(me);
                        }
                        else
                        {
                            _doubleClickFired = false;
                            _parent.OnDoubleClick(me);
                            _parent.OnMouseDoubleClick(me);
                        }
                    }

                    _doubleClickFired = false;
                }

                _parent.OnMouseUp(me);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                _mouseOver = ButtonID.None;
                Invalidate();

                _parent.OnMouseLeave(e);
            }

            /// <summary>
            ///  Handles painting the buttons on the control.
            /// </summary>
            protected override void OnPaint(PaintEventArgs e)
            {
                int half_height = ClientSize.Height / 2;

                // Draw the up and down buttons
                if (Application.RenderWithVisualStyles)
                {
                    var vsr = new VisualStyleRenderer(_mouseOver == ButtonID.Up
                        ? VisualStyleElement.Spin.Up.Hot
                        : VisualStyleElement.Spin.Up.Normal);

                    if (!Enabled)
                    {
                        vsr.SetParameters(VisualStyleElement.Spin.Up.Disabled);
                    }
                    else if (_pushed == ButtonID.Up)
                    {
                        vsr.SetParameters(VisualStyleElement.Spin.Up.Pressed);
                    }

                    using var hdc = new DeviceContextHdcScope(e);
                    vsr.DrawBackground(
                        hdc,
                        new Rectangle(0, 0, _parent._defaultButtonsWidth, half_height),
                        HandleInternal);

                    if (!Enabled)
                    {
                        vsr.SetParameters(VisualStyleElement.Spin.Down.Disabled);
                    }
                    else if (_pushed == ButtonID.Down)
                    {
                        vsr.SetParameters(VisualStyleElement.Spin.Down.Pressed);
                    }
                    else
                    {
                        vsr.SetParameters(_mouseOver == ButtonID.Down
                            ? VisualStyleElement.Spin.Down.Hot
                            : VisualStyleElement.Spin.Down.Normal);
                    }

                    vsr.DrawBackground(
                        hdc,
                        new Rectangle(0, half_height, _parent._defaultButtonsWidth, half_height),
                        HandleInternal);
                }
                else
                {
                    ControlPaint.DrawScrollButton(
                        e.GraphicsInternal,
                        new Rectangle(0, 0, _parent._defaultButtonsWidth, half_height),
                        ScrollButton.Up,
                        _pushed == ButtonID.Up ? ButtonState.Pushed : (Enabled ? ButtonState.Normal : ButtonState.Inactive));

                    ControlPaint.DrawScrollButton(
                        e.GraphicsInternal,
                        new Rectangle(0, half_height, _parent._defaultButtonsWidth, half_height),
                        ScrollButton.Down,
                        _pushed == ButtonID.Down ? ButtonState.Pushed : (Enabled ? ButtonState.Normal : ButtonState.Inactive));
                }

                if (half_height != (ClientSize.Height + 1) / 2)
                {
                    // When control has odd height, a line needs to be drawn below the buttons with the backcolor.

                    Color color = _parent.BackColor;

                    Rectangle clientRect = ClientRectangle;
                    Point pt1 = new Point(clientRect.Left, clientRect.Bottom - 1);
                    Point pt2 = new Point(clientRect.Right, clientRect.Bottom - 1);

                    using var hdc = new DeviceContextHdcScope(e);
                    using var hpen = new Gdi32.CreatePenScope(color);
                    hdc.DrawLine(hpen, pt1, pt2);
                }

                // Raise the paint event, just in case this inner class goes public some day
                base.OnPaint(e);
            }

            /// <summary>
            ///  Occurs when the UpDown buttons are pressed and when the acceleration timer tick event is raised.
            /// </summary>
            protected virtual void OnUpDown(UpDownEventArgs upevent)
                => _upDownEventHandler?.Invoke(this, upevent);

            /// <summary>
            ///  Starts the timer for generating updown events
            /// </summary>
            protected void StartTimer()
            {
                _parent.OnStartTimer();
                if (_timer is null)
                {
                    // Generates UpDown events
                    _timer = new Timer();
                    // Add the timer handler
                    _timer.Tick += new EventHandler(TimerHandler);
                }

                _timerInterval = DefaultTimerInterval;

                _timer.Interval = _timerInterval;
                _timer.Start();
            }

            /// <summary>
            ///  Stops the timer for generating updown events
            /// </summary>
            protected void StopTimer()
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }

                _parent.OnStopTimer();
            }

            internal override bool SupportsUiaProviders => true;

            /// <summary>
            ///  Generates updown events when the timer calls this function.
            /// </summary>
            private void TimerHandler(object source, EventArgs args)
            {
                // Make sure we've got mouse capture
                if (!Capture)
                {
                    EndButtonPress();
                    return;
                }

                // onUpDown method calls customer's ValueCHanged event handler which might enter the message loop and
                // process the mouse button up event, which results in timer being disposed
                OnUpDown(new UpDownEventArgs((int)_pushed));

                if (_timer != null)
                {
                    // Accelerate timer.
                    _timerInterval *= 7;
                    _timerInterval /= 10;

                    if (_timerInterval < 1)
                    {
                        _timerInterval = 1;
                    }

                    _timer.Interval = _timerInterval;
                }
            }

            internal class UpDownButtonsAccessibleObject : ControlAccessibleObject
            {
                private DirectionButtonAccessibleObject upButton;
                private DirectionButtonAccessibleObject downButton;

                private UpDownButtons _owner;

                public UpDownButtonsAccessibleObject(UpDownButtons owner) : base(owner)
                {
                    _owner = owner;
                }

                internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
                {
                    AccessibleObject element = HitTest((int)x, (int)y);

                    if (element != null)
                    {
                        return element;
                    }

                    return base.ElementProviderFromPoint(x, y);
                }

                internal override UiaCore.IRawElementProviderFragment FragmentNavigate(
                    UiaCore.NavigateDirection direction) => direction switch
                    {
                        UiaCore.NavigateDirection.FirstChild => GetChild(0),
                        UiaCore.NavigateDirection.LastChild => GetChild(1),
                        _ => base.FragmentNavigate(direction),
                    };

                internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

                private DirectionButtonAccessibleObject UpButton
                    => upButton ??= new DirectionButtonAccessibleObject(this, true);

                private DirectionButtonAccessibleObject DownButton
                    => downButton ??= new DirectionButtonAccessibleObject(this, false);

                public override AccessibleObject GetChild(int index) => index switch
                {
                    0 => UpButton,
                    1 => DownButton,
                    _ => null,
                };

                public override int GetChildCount() => 2;

                internal override object GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.SpinnerControlTypeId,
                    UiaCore.UIA.BoundingRectanglePropertyId => Bounds,
                    UiaCore.UIA.LegacyIAccessibleStatePropertyId => State,
                    UiaCore.UIA.LegacyIAccessibleRolePropertyId => Role,
                    _ => base.GetPropertyValue(propertyID),
                };

                public override AccessibleObject HitTest(int x, int y)
                {
                    if (UpButton.Bounds.Contains(x, y))
                    {
                        return UpButton;
                    }

                    if (DownButton.Bounds.Contains(x, y))
                    {
                        return DownButton;
                    }

                    return null;
                }

                internal override UiaCore.IRawElementProviderSimple HostRawElementProvider
                {
                    get
                    {
                        if (HandleInternal == IntPtr.Zero)
                        {
                            return null;
                        }

                        UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, HandleInternal), out UiaCore.IRawElementProviderSimple provider);
                        return provider;
                    }
                }

                public override string Name
                {
                    get
                    {
                        string baseName = base.Name;
                        if (string.IsNullOrEmpty(baseName))
                        {
                            return SR.DefaultUpDownButtonsAccessibleName;
                        }

                        return baseName;
                    }
                    set => base.Name = value;
                }

                public override AccessibleObject Parent => _owner.AccessibilityObject;

                public override AccessibleRole Role
                {
                    get
                    {
                        AccessibleRole role = Owner.AccessibleRole;
                        if (role != AccessibleRole.Default)
                        {
                            return role;
                        }

                        return AccessibleRole.SpinButton;
                    }
                }

                /// <summary>
                ///  Gets the runtime ID.
                /// </summary>
                internal override int[] RuntimeId
                {
                    get
                    {
                        if (_owner is null)
                        {
                            return base.RuntimeId;
                        }

                        // We need to provide a unique ID others are implementing this in the same manner first item
                        // is static - 0x2a (RuntimeIDFirstItem) second item can be anything, but here it is a hash.

                        var runtimeId = new int[3];
                        runtimeId[0] = RuntimeIDFirstItem;
                        runtimeId[1] = (int)(long)_owner.InternalHandle;
                        runtimeId[2] = _owner.GetHashCode();

                        return runtimeId;
                    }
                }

                internal class DirectionButtonAccessibleObject : AccessibleObject
                {
                    private readonly bool _up;
                    private readonly UpDownButtonsAccessibleObject _parent;

                    public DirectionButtonAccessibleObject(UpDownButtonsAccessibleObject parent, bool up)
                    {
                        _parent = parent;
                        _up = up;
                    }

                    /// <summary>
                    ///  Gets the runtime ID.
                    /// </summary>
                    internal override int[] RuntimeId => new int[]
                    {
                        _parent.RuntimeId[0],
                        _parent.RuntimeId[1],
                        _parent.RuntimeId[2],
                        _up ? 1 : 0
                    };

                    internal override object GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
                    {
                        UiaCore.UIA.NamePropertyId => Name,
                        UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                        UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                        UiaCore.UIA.BoundingRectanglePropertyId => Bounds,
                        UiaCore.UIA.LegacyIAccessibleStatePropertyId => State,
                        UiaCore.UIA.LegacyIAccessibleRolePropertyId => Role,
                        _ => base.GetPropertyValue(propertyID),
                    };

                    internal override UiaCore.IRawElementProviderFragment FragmentNavigate(
                        UiaCore.NavigateDirection direction) => direction switch
                        {
                            UiaCore.NavigateDirection.Parent => Parent,
                            UiaCore.NavigateDirection.NextSibling => _up ? Parent.GetChild(1) : null,
                            UiaCore.NavigateDirection.PreviousSibling => _up ? null : Parent.GetChild(0),
                            _ => base.FragmentNavigate(direction),
                        };

                    internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => Parent;

                    public override Rectangle Bounds
                    {
                        get
                        {
                            if (!_parent.Owner.IsHandleCreated)
                            {
                                return Rectangle.Empty;
                            }

                            // Get button bounds
                            Rectangle bounds = ((UpDownButtons)_parent.Owner).Bounds;
                            bounds.Height /= 2;

                            if (!_up)
                            {
                                bounds.Y += bounds.Height;
                            }

                            // Convert to screen co-ords
                            return (((UpDownButtons)_parent.Owner).ParentInternal).RectangleToScreen(bounds);
                        }
                    }

                    public override string Name
                    {
                        get => _up ? SR.UpDownBaseUpButtonAccName : SR.UpDownBaseDownButtonAccName;
                        set { }
                    }

                    public override AccessibleObject Parent => _parent;

                    public override AccessibleRole Role => AccessibleRole.PushButton;
                }
            }
        }
    }
}
