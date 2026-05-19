// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

using Windows.Win32.System.Threading;
using Windows.Win32.System.Power;

namespace System.Windows.Forms;

[DefaultProperty(nameof(ToggleFullScreenKey))]
[ToolboxItemFilter("System.Windows.Forms")]
public class KioskModeManager : Component, ISupportInitialize
{
    private const uint POWER_REQUEST_CONTEXT_VERSION = 0;
    private const POWER_REQUEST_CONTEXT_FLAGS POWER_REQUEST_CONTEXT_SIMPLE_STRING = (POWER_REQUEST_CONTEXT_FLAGS)0x00000001;

    private ContainerControl? _containerControl;
    private Form? _targetForm;
    private bool _isFullScreen;
    private bool _initializing;

    // Saved form state for restoring
    private FormBorderStyle _savedBorderStyle;
    private FormWindowState _savedWindowState;
    private Rectangle _savedBounds;
    private bool _savedTopMost;

    // Configuration backing fields
    private bool _hideTaskbar;
    private bool _topMostInFullScreen;
    private Keys _toggleFullScreenKey = Keys.F11;
    private bool _escapeExitsFullScreen = true;
    private bool _suppressPowerSaving;

    private HANDLE _powerRequestHandle;

    public KioskModeManager()
    {
    }

    public KioskModeManager(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.Add(this);
    }

    [Category("Kiosk")]
    [Description("The Form whose fullscreen state this component controls.")]
    [DefaultValue(null)]
    public ContainerControl? ContainerControl
    {
        get => _containerControl;
        set
        {
            if (_containerControl == value)
            {
                return;
            }

            DetachFromForm();
            _containerControl = value;
            AttachToForm();
            OnContainerControlChanged(EventArgs.Empty);
        }
    }

    public event EventHandler? ContainerControlChanged;

    protected virtual void OnContainerControlChanged(EventArgs e)
        => ContainerControlChanged?.Invoke(this, e);

    [Category("Kiosk")]
    [Description("When true, fullscreen mode overlays the OS taskbar. When false, the taskbar remains visible.")]
    [DefaultValue(false)]
    public bool HideTaskbar
    {
        get => _hideTaskbar;
        set
        {
            if (_hideTaskbar == value)
            {
                return;
            }

            _hideTaskbar = value;

            OnHideTaskbarChanged(EventArgs.Empty);

            // Re-apply if currently in fullscreen
            if (_isFullScreen && _targetForm is not null)
            {
                ApplyFullScreen(_targetForm);
            }
        }
    }

    public event EventHandler? HideTaskbarChanged;

    protected virtual void OnHideTaskbarChanged(EventArgs e)
        => HideTaskbarChanged?.Invoke(this, e);

    [Category("Kiosk")]
    [Description("When true, the form becomes TopMost in fullscreen mode, preventing other windows from appearing in front.")]
    [DefaultValue(false)]
    public bool TopMostInFullScreen
    {
        get => _topMostInFullScreen;
        set
        {
            if (_topMostInFullScreen == value)
            {
                return;
            }

            _topMostInFullScreen = value;
            OnTopMostInFullScreenChanged(EventArgs.Empty);

            if (_isFullScreen && _targetForm is not null)
            {
                _targetForm.TopMost = value;
            }
        }
    }

    public event EventHandler? TopMostInFullScreenChanged;

    protected virtual void OnTopMostInFullScreenChanged(EventArgs e)
        => TopMostInFullScreenChanged?.Invoke(this, e);

    [Category("Kiosk")]
    [Description("The key that toggles between fullscreen and windowed mode.")]
    [DefaultValue(Keys.F11)]
    public Keys ToggleFullScreenKey
    {
        get => _toggleFullScreenKey;
        set
        {
            if (_toggleFullScreenKey == value)
            {
                return;
            }

            _toggleFullScreenKey = value;
            OnToggleFullScreenKeyChanged(EventArgs.Empty);
        }
    }

    public event EventHandler? ToggleFullScreenKeyChanged;

    protected virtual void OnToggleFullScreenKeyChanged(EventArgs e)
        => ToggleFullScreenKeyChanged?.Invoke(this, e);

    [Category("Kiosk")]
    [Description("When true, pressing Escape exits fullscreen mode.")]
    [DefaultValue(true)]
    public bool EscapeExitsFullScreen
    {
        get => _escapeExitsFullScreen;
        set
        {
            if (_escapeExitsFullScreen == value)
            {
                return;
            }

            _escapeExitsFullScreen = value;
            OnEscapeExitsFullScreenChanged(EventArgs.Empty);
        }
    }

    public event EventHandler? EscapeExitsFullScreenChanged;

    protected virtual void OnEscapeExitsFullScreenChanged(EventArgs e)
        => EscapeExitsFullScreenChanged?.Invoke(this, e);

    [Category("Kiosk")]
    [Description("When true, prevents the machine from entering screen-saver, sleep, or hibernation.")]
    [DefaultValue(false)]
    public bool SuppressPowerSaving
    {
        get => _suppressPowerSaving;
        set
        {
            if (_suppressPowerSaving == value)
            {
                return;
            }

            _suppressPowerSaving = value;

            if (value)
            {
                CreatePowerRequest();
            }
            else
            {
                ReleasePowerRequest();
            }

            OnSuppressPowerSavingChanged(EventArgs.Empty);
        }
    }

    public event EventHandler? SuppressPowerSavingChanged;

    protected virtual void OnSuppressPowerSavingChanged(EventArgs e)
        => SuppressPowerSavingChanged?.Invoke(this, e);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsFullScreen
    {
        get => _isFullScreen;
        private set
        {
            if (_isFullScreen == value)
            {
                return;
            }

            _isFullScreen = value;
            OnFullScreenChanged(EventArgs.Empty);
        }
    }

    [Category("Kiosk")]
    [Description("Occurs when the fullscreen state changes.")]
    public event EventHandler? FullScreenChanged;

    protected virtual void OnFullScreenChanged(EventArgs e)
        => FullScreenChanged?.Invoke(this, e);

    public void EnterFullScreen()
    {
        if (_isFullScreen || _targetForm is null)
        {
            return;
        }

        // Save current state
        _savedBorderStyle = _targetForm.FormBorderStyle;
        _savedWindowState = _targetForm.WindowState;
        _savedBounds = _targetForm.Bounds;
        _savedTopMost = _targetForm.TopMost;

        ApplyFullScreen(_targetForm);
        IsFullScreen = true;
    }

    public void ExitFullScreen()
    {
        if (!_isFullScreen || _targetForm is null)
        {
            return;
        }

        _targetForm.FormBorderStyle = _savedBorderStyle;
        _targetForm.TopMost = _savedTopMost;
        _targetForm.WindowState = _savedWindowState;
        _targetForm.Bounds = _savedBounds;

        IsFullScreen = false;
    }

    public void ToggleFullScreen()
    {
        if (_isFullScreen)
        {
            ExitFullScreen();
        }
        else
        {
            EnterFullScreen();
        }
    }

    void ISupportInitialize.BeginInit()
    {
        _initializing = true;
    }

    void ISupportInitialize.EndInit()
    {
        _initializing = false;

        // Auto-discover the parent form if ContainerControl was set during initialization
        if (_containerControl is not null)
        {
            AttachToForm();
        }
    }

    private void AttachToForm()
    {
        if (_initializing)
        {
            return;
        }

        _targetForm = _containerControl as Form ?? _containerControl?.FindForm();

        if (_targetForm is null)
        {
            return;
        }

        // Enable KeyPreview so we get key events before child controls
        _targetForm.KeyPreview = true;
        _targetForm.KeyDown += OnFormKeyDown;
    }

    private void DetachFromForm()
    {
        _targetForm?.KeyDown -= OnFormKeyDown;
        _targetForm = null;
    }

    private void OnFormKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == _toggleFullScreenKey && e.Modifiers == Keys.None)
        {
            ToggleFullScreen();
            e.Handled = true;
            e.SuppressKeyPress = true;

            return;
        }

        if (e.KeyCode == Keys.Escape && e.Modifiers == Keys.None
            && _escapeExitsFullScreen && _isFullScreen)
        {
            ExitFullScreen();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void ApplyFullScreen(Form form)
    {
        form.FormBorderStyle = FormBorderStyle.None;

        if (_topMostInFullScreen)
        {
            form.TopMost = true;
        }

        if (_hideTaskbar)
        {
            // Cover the entire screen including taskbar
            form.WindowState = FormWindowState.Normal;
            Screen screen = Screen.FromControl(form);
            form.Bounds = screen.Bounds;
        }
        else
        {
            // Standard maximized — taskbar remains accessible
            form.WindowState = FormWindowState.Maximized;
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DetachFromForm();

            if (_suppressPowerSaving)
            {
                ReleasePowerRequest();
            }
        }

        base.Dispose(disposing);
    }

    private void CreatePowerRequest()
    {
        if (!_powerRequestHandle.IsNull)
        {
            return;
        }

        var context = new REASON_CONTEXT
        {
            Version = POWER_REQUEST_CONTEXT_VERSION,
            Flags = POWER_REQUEST_CONTEXT_SIMPLE_STRING,
        };

        nint reasonString = Marshal.StringToHGlobalUni(
            "WinForms KioskModeManager: Preventing screen saver and sleep");

        try
        {
            context.Reason.Detailed.ReasonStrings = (PWSTR*)reasonString;
            _powerRequestHandle = PInvoke.PowerCreateRequest(in context);
        }
        finally
        {
            Marshal.FreeHGlobal(reasonString);
        }

        PInvoke.PowerSetRequest(_powerRequestHandle, POWER_REQUEST_TYPE.PowerRequestDisplayRequired);
        PInvoke.PowerSetRequest(_powerRequestHandle, POWER_REQUEST_TYPE.PowerRequestSystemRequired);
    }

    private void ReleasePowerRequest()
    {
        if (_powerRequestHandle.IsNull)
        {
            return;
        }

        PInvoke.PowerClearRequest(_powerRequestHandle, POWER_REQUEST_TYPE.PowerRequestDisplayRequired);
        PInvoke.PowerClearRequest(_powerRequestHandle, POWER_REQUEST_TYPE.PowerRequestSystemRequired);
    }
}
