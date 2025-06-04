// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
///  Implements a dialog box that is displayed when an unhandled exception occurs in a thread.
/// </summary>
public class ThreadExceptionDialog : Form
{
    private const string DownBitmapName = "down";
    private const string UpBitmapName = "up";

    private const int MAXWIDTH = 440;
    private const int MAXHEIGHT = 325;
    private const int PADDINGWIDTH = 84;
    private const int PADDINGHEIGHT = 26;
    private const int MAXTEXTWIDTH = 180;
    private const int MAXTEXTHEIGHT = 40;
    private const int BUTTONTOPPADDING = 31;
    private const int BUTTONDETAILS_LEFTPADDING = 8;
    private const int MESSAGE_TOPPADDING = 8;
    private const int HEIGHTPADDING = 8;
    private const int BUTTONWIDTH = 130;
    private const int BUTTONHEIGHT = 23;
    private const int BUTTONALIGNMENTWIDTH = 135;
    private const int BUTTONALIGNMENTPADDING = 5;
    private const int DETAILSWIDTHPADDING = 16;
    private const int DETAILSHEIGHT = 154;
    private const int PICTUREWIDTH = 64;
    private const int PICTUREHEIGHT = 64;
    private const int EXCEPTIONMESSAGEVERTICALPADDING = 4;

    private readonly int _scaledMaxWidth = MAXWIDTH;
    private readonly int _scaledMaxHeight = MAXHEIGHT;
    private readonly int _scaledPaddingWidth = PADDINGWIDTH;
    private readonly int _scaledPaddingHeight = PADDINGHEIGHT;
    private readonly int _scaledMaxTextWidth = MAXTEXTWIDTH;
    private readonly int _scaledMaxTextHeight = MAXTEXTHEIGHT;
    private readonly int _scaledButtonTopPadding = BUTTONTOPPADDING;
    private readonly int _scaledButtonDetailsLeftPadding = BUTTONDETAILS_LEFTPADDING;
    private readonly int _scaledMessageTopPadding = MESSAGE_TOPPADDING;
    private readonly int _scaledButtonWidth = BUTTONWIDTH;
    private readonly int _scaledButtonHeight = BUTTONHEIGHT;
    private readonly int _scaledButtonAlignmentWidth = BUTTONALIGNMENTWIDTH;
    private readonly int _scaledButtonAlignmentPadding = BUTTONALIGNMENTPADDING;
    private readonly int _scaledDetailsWidthPadding = DETAILSWIDTHPADDING;
    private readonly int _scaledDetailsHeight = DETAILSHEIGHT;
    private readonly int _scaledPictureWidth = PICTUREWIDTH;
    private readonly int _scaledPictureHeight = PICTUREHEIGHT;
    private readonly int _scaledExceptionMessageVerticalPadding = EXCEPTIONMESSAGEVERTICALPADDING;

    private readonly PictureBox _pictureBox = new();
    private readonly Label _message = new();
    private readonly Button _continueButton = new();
    private readonly Button _quitButton = new();
    private readonly Button _detailsButton = new();
    private readonly Button _helpButton = new();
    private readonly TextBox _details = new();
    private Bitmap? _expandImage;
    private Bitmap? _collapseImage;
    private bool _detailsVisible;
    private int _scaledHeightPadding = HEIGHTPADDING;

    /// <summary>
    ///  Initializes a new instance of the <see cref="ThreadExceptionDialog"/> class.
    /// </summary>
    public ThreadExceptionDialog(Exception t)
    {
        _scaledMaxWidth = LogicalToDeviceUnits(MAXWIDTH);
        _scaledMaxHeight = LogicalToDeviceUnits(MAXHEIGHT);
        _scaledPaddingWidth = LogicalToDeviceUnits(PADDINGWIDTH);
        _scaledPaddingHeight = LogicalToDeviceUnits(PADDINGHEIGHT);
        _scaledMaxTextWidth = LogicalToDeviceUnits(MAXTEXTWIDTH);
        _scaledMaxTextHeight = LogicalToDeviceUnits(MAXTEXTHEIGHT);
        _scaledButtonTopPadding = LogicalToDeviceUnits(BUTTONTOPPADDING);
        _scaledButtonDetailsLeftPadding = LogicalToDeviceUnits(BUTTONDETAILS_LEFTPADDING);
        _scaledMessageTopPadding = LogicalToDeviceUnits(MESSAGE_TOPPADDING);
        _scaledHeightPadding = LogicalToDeviceUnits(HEIGHTPADDING);
        _scaledButtonWidth = LogicalToDeviceUnits(BUTTONWIDTH);
        _scaledButtonHeight = LogicalToDeviceUnits(BUTTONHEIGHT);
        _scaledButtonAlignmentWidth = LogicalToDeviceUnits(BUTTONALIGNMENTWIDTH);
        _scaledButtonAlignmentPadding = LogicalToDeviceUnits(BUTTONALIGNMENTPADDING);
        _scaledDetailsWidthPadding = LogicalToDeviceUnits(DETAILSWIDTHPADDING);
        _scaledDetailsHeight = LogicalToDeviceUnits(DETAILSHEIGHT);
        _scaledPictureWidth = LogicalToDeviceUnits(PICTUREWIDTH);
        _scaledPictureHeight = LogicalToDeviceUnits(PICTUREHEIGHT);
        _scaledExceptionMessageVerticalPadding = LogicalToDeviceUnits(EXCEPTIONMESSAGEVERTICALPADDING);

        string messageFormat;
        string messageText;
        Button[] buttons;
        bool detailAnchor = false;

        if (t is WarningException w)
        {
            messageFormat = SR.ExDlgWarningText;
            messageText = w.Message;
            if (w.HelpUrl is null)
            {
                buttons = [_continueButton];
            }
            else
            {
                buttons = [_continueButton, _helpButton];
            }
        }
        else
        {
            messageText = t.Message;

            detailAnchor = true;

            if (Application.AllowQuit)
            {
                if (t is Security.SecurityException)
                {
                    messageFormat = SR.ExDlgSecurityErrorText;
                }
                else
                {
                    messageFormat = SR.ExDlgErrorText;
                }

                buttons = [_detailsButton, _continueButton, _quitButton];
            }
            else
            {
                if (t is Security.SecurityException)
                {
                    messageFormat = SR.ExDlgSecurityContinueErrorText;
                }
                else
                {
                    messageFormat = SR.ExDlgContinueErrorText;
                }

                buttons = [_detailsButton, _continueButton];
            }
        }

        if (messageText.Length == 0)
        {
            messageText = t.GetType().Name;
        }

        if (t is Security.SecurityException)
        {
            messageText = string.Format(messageFormat, t.GetType().Name, Trim(messageText));
        }
        else
        {
            messageText = string.Format(messageFormat, Trim(messageText));
        }

        StringBuilder detailsTextBuilder = new();
        string separator = SR.ExDlgMsgSeparator;
        string sectionseparator = SR.ExDlgMsgSectionSeparator;
        if (Application.CustomThreadExceptionHandlerAttached)
        {
            detailsTextBuilder.Append(SR.ExDlgMsgHeaderNonSwitchable);
        }
        else
        {
            detailsTextBuilder.Append(SR.ExDlgMsgHeaderSwitchable);
        }

        detailsTextBuilder.AppendFormat(CultureInfo.CurrentCulture, sectionseparator, SR.ExDlgMsgExceptionSection);
        detailsTextBuilder.AppendLine(t.ToString());
        detailsTextBuilder.AppendLine();
        detailsTextBuilder.AppendFormat(CultureInfo.CurrentCulture, sectionseparator, SR.ExDlgMsgLoadedAssembliesSection);

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            AssemblyName name = assembly.GetName();
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file
            string location = assembly.Location;
#pragma warning restore IL3000

            detailsTextBuilder.AppendFormat(
                SR.ExDlgMsgLoadedAssembliesEntry,
                name.Name,
                name.Version,
                string.IsNullOrEmpty(location) ? AppContext.BaseDirectory : location);
            detailsTextBuilder.Append(separator);
        }

        detailsTextBuilder.AppendFormat(CultureInfo.CurrentCulture, sectionseparator, SR.ExDlgMsgJITDebuggingSection);
        if (Application.CustomThreadExceptionHandlerAttached)
        {
            detailsTextBuilder.AppendLine(SR.ExDlgMsgFooterNonSwitchable);
        }
        else
        {
            detailsTextBuilder.AppendLine(SR.ExDlgMsgFooterSwitchable);
        }

        detailsTextBuilder.AppendLine();

        string detailsText = detailsTextBuilder.ToString();

        Size textSize = new(_scaledMaxWidth - _scaledPaddingWidth, int.MaxValue);

        if (ScaleHelper.IsScalingRequirementMet && !UseCompatibleTextRenderingDefault)
        {
            // we need to measure string using API that matches the rendering engine - TextRenderer.MeasureText for GDI
            textSize = Size.Ceiling(TextRenderer.MeasureText(messageText, Font, textSize, TextFormatFlags.WordBreak));
        }
        else
        {
            using Graphics g = _message.CreateGraphicsInternal();
            // if HighDpi improvements are not enabled, or rendering mode is GDI+, use Graphics.MeasureString
            textSize = Size.Ceiling(g.MeasureString(messageText, Font, textSize.Width));
        }

        textSize.Height += _scaledExceptionMessageVerticalPadding;
        if (textSize.Width < _scaledMaxTextWidth)
        {
            textSize.Width = _scaledMaxTextWidth;
        }

        if (textSize.Height > _scaledMaxHeight)
        {
            textSize.Height = _scaledMaxHeight;
        }

        int width = textSize.Width + _scaledPaddingWidth;
        int buttonTop = Math.Max(textSize.Height, _scaledMaxTextHeight) + _scaledPaddingHeight;

        Form? activeForm = ActiveForm;
        if (activeForm is null || activeForm.Text.Length == 0)
        {
            Text = SR.ExDlgCaption;
        }
        else
        {
            Text = string.Format(SR.ExDlgCaption2, activeForm.Text);
        }

        AcceptButton = _continueButton;
        CancelButton = _continueButton;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Icon = null;
        ClientSize = new Size(width, buttonTop + _scaledButtonTopPadding);
        TopMost = true;

        _pictureBox.Location = new Point(_scaledPictureWidth / 8, _scaledPictureHeight / 8);
        _pictureBox.Size = new Size(_scaledPictureWidth * 3 / 4, _scaledPictureHeight * 3 / 4);
        _pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        StockIconId stockIconId = (t is Security.SecurityException) ? StockIconId.Info : StockIconId.Error;
        using Icon icon = SystemIcons.GetStockIcon(stockIconId, _scaledPictureWidth);
        _pictureBox.Image = icon.ToBitmap();

        Controls.Add(_pictureBox);
        _message.SetBounds(_scaledPictureWidth,
                          _scaledMessageTopPadding + (_scaledMaxTextHeight - Math.Min(textSize.Height, _scaledMaxTextHeight)) / 2,
                          textSize.Width, textSize.Height);
        _message.Text = messageText;
        Controls.Add(_message);

        _continueButton.Text = SR.ExDlgContinue;
        _continueButton.FlatStyle = FlatStyle.Standard;
        _continueButton.DialogResult = DialogResult.Cancel;

        _quitButton.Text = SR.ExDlgQuit;
        _quitButton.FlatStyle = FlatStyle.Standard;
        _quitButton.DialogResult = DialogResult.Abort;

        _helpButton.Text = SR.ExDlgHelp;
        _helpButton.FlatStyle = FlatStyle.Standard;
        _helpButton.DialogResult = DialogResult.Yes;

        _detailsButton.Text = SR.ExDlgShowDetails;
        _detailsButton.FlatStyle = FlatStyle.Standard;
        _detailsButton.Click += DetailsClick;

        Button? button = null;
        int startIndex = 0;

        if (detailAnchor)
        {
            button = _detailsButton;

            _expandImage = ScaleHelper.GetSmallIconResourceAsBitmap(
                GetType(),
                DownBitmapName,
                DeviceDpi);
            _collapseImage = ScaleHelper.GetSmallIconResourceAsBitmap(
                GetType(),
                UpBitmapName,
                DeviceDpi);

            button.SetBounds(_scaledButtonDetailsLeftPadding, buttonTop, _scaledButtonWidth, _scaledButtonHeight);
            button.Image = _expandImage;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            Controls.Add(button);
            startIndex = 1;
        }

        int buttonLeft = width - _scaledButtonDetailsLeftPadding
            - ((buttons.Length - startIndex) * _scaledButtonAlignmentWidth - _scaledButtonAlignmentPadding);

        for (int i = startIndex; i < buttons.Length; i++)
        {
            button = buttons[i];
            button.SetBounds(buttonLeft, buttonTop, _scaledButtonWidth, _scaledButtonHeight);
            Controls.Add(button);
            buttonLeft += _scaledButtonAlignmentWidth;
        }

        _details.Text = detailsText;
        _details.ScrollBars = ScrollBars.Both;
        _details.Multiline = true;
        _details.ReadOnly = true;
        _details.WordWrap = false;
        _details.TabStop = false;
        _details.AcceptsReturn = false;

        _details.SetBounds(_scaledButtonDetailsLeftPadding, buttonTop + _scaledButtonTopPadding, width - _scaledDetailsWidthPadding, _scaledDetailsHeight);
        _details.Visible = _detailsVisible;
        Controls.Add(_details);

        AutoScaleMode = AutoScaleMode.Dpi;

        if (ScaleHelper.IsScalingRequirementMet)
        {
            DpiChanged += ThreadExceptionDialog_DpiChanged;
        }
    }

    private void ThreadExceptionDialog_DpiChanged(object? sender, DpiChangedEventArgs e)
    {
        _expandImage?.Dispose();
        _expandImage = ScaleHelper.GetSmallIconResourceAsBitmap(GetType(), DownBitmapName, DeviceDpi);
        _collapseImage = ScaleHelper.GetSmallIconResourceAsBitmap(GetType(), UpBitmapName, DeviceDpi);
        _detailsButton.Image = _detailsVisible ? _collapseImage : _expandImage;

        if (e.DeviceDpiNew != e.DeviceDpiOld)
        {
            _scaledHeightPadding = (int)Math.Round(HEIGHTPADDING * ((float)e.DeviceDpiNew / e.DeviceDpiOld));
        }
    }

    /// <summary>
    ///  Hide the property
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set => base.AutoSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

    /// <summary>
    ///  Called when the details button is clicked.
    /// </summary>
    private void DetailsClick(object? sender, EventArgs eventargs)
    {
        int delta = _details.Height + _scaledHeightPadding;
        if (_detailsVisible)
        {
            delta = -delta;
        }

        Height += delta;
        _detailsVisible = !_detailsVisible;
        _details.Visible = _detailsVisible;
        _detailsButton.Image = _detailsVisible ? _collapseImage : _expandImage;
    }

    private static string? Trim(string s)
    {
        if (s is null)
        {
            return s;
        }

        int i = s.Length;
        while (i > 0 && s[i - 1] == '.')
        {
            i--;
        }

        return s[..i];
    }
}
