// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal class MaskedTextBoxTextEditorDropDown : UserControl
{
    // Logical (96 DPI) constants. The actual device-pixel values are recomputed in
    // RescaleConstantsForDpi so that PerMonitorV2 DPI changes are honoured.
    private const int LogicalPadding = 16;
    private static readonly Drawing.Size s_logicalSize = new(100, 52);

    private bool _cancel;
    private readonly MaskedTextBox _cloneMtb;
    private readonly ErrorProvider _errorProvider;

    public MaskedTextBoxTextEditorDropDown(MaskedTextBox maskedTextBox)
    {
        _cloneMtb = MaskedTextBoxDesigner.GetDesignMaskedTextBox(maskedTextBox);
        _errorProvider = new ErrorProvider();
        ((System.ComponentModel.ISupportInitialize)(_errorProvider)).BeginInit();

        SuspendLayout();
        //
        // maskedTextBox
        //
        _cloneMtb.Dock = DockStyle.Fill;

        // Include prompt and literals always so editor can process the text value in a consistent way.
        _cloneMtb.TextMaskFormat = MaskFormat.IncludePromptAndLiterals;

        // Escape prompt, literals and space so input is not rejected due to one of these characters.
        _cloneMtb.ResetOnPrompt = true;
        _cloneMtb.SkipLiterals = true;
        _cloneMtb.ResetOnSpace = true;

        _cloneMtb.Name = "MaskedTextBoxClone";
        _cloneMtb.TabIndex = 0;
        _cloneMtb.MaskInputRejected += maskedTextBox_MaskInputRejected;
        _cloneMtb.KeyDown += maskedTextBox_KeyDown;

        //
        // errorProvider
        //
        _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        _errorProvider.ContainerControl = this;

        //
        // MaskedTextBoxTextEditorDropDown
        //
        Controls.Add(_cloneMtb);

        BackColor = Drawing.SystemColors.Control;
        BorderStyle = BorderStyle.FixedSingle;
        Name = "MaskedTextBoxTextEditorDropDown";

        // Apply the initial DPI-scaled padding/size. Subsequent DPI changes are
        // picked up automatically through RescaleConstantsForDpi.
        RescaleConstantsForDpi(DeviceDpi, DeviceDpi);

        ((System.ComponentModel.ISupportInitialize)(_errorProvider)).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    public string? Value
    {
        get
        {
            if (_cancel)
            {
                return null;
            }

            // Output will include prompt and literals always to be able to get the characters at the right positions in case
            // some of them are not set (particularly at lower positions).
            return _cloneMtb.Text;
        }
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            _cancel = true;
        }

        return base.ProcessDialogKey(keyData);
    }

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

        // DeviceDpi has already been updated to deviceDpiNew by the framework before
        // this call, so LogicalToDeviceUnits returns values scaled to the new DPI.
        Padding = new Padding(LogicalToDeviceUnits(LogicalPadding));
        Size = LogicalToDeviceUnits(s_logicalSize);
    }

    private void maskedTextBox_MaskInputRejected(object? sender, MaskInputRejectedEventArgs e)
    {
        _errorProvider.SetError(_cloneMtb, MaskedTextBoxDesigner.GetMaskInputRejectedErrorMessage(e));
    }

    private void maskedTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        _errorProvider.Clear();
    }
}
