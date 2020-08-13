// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements a dialog box that is displayed when an unhandled exception occurs in
    ///  a thread.
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
        private const int BUTTONWIDTH = 100;
        private const int BUTTONHEIGHT = 23;
        private const int BUTTONALIGNMENTWIDTH = 105;
        private const int BUTTONALIGNMENTPADDING = 5;
        private const int DETAILSWIDTHPADDING = 16;
        private const int DETAILSHEIGHT = 154;
        private const int PICTUREWIDTH = 64;
        private const int PICTUREHEIGHT = 64;
        private const int EXCEPTIONMESSAGEVERTICALPADDING = 4;

        private readonly int scaledMaxWidth = MAXWIDTH;
        private readonly int scaledMaxHeight = MAXHEIGHT;
        private readonly int scaledPaddingWidth = PADDINGWIDTH;
        private readonly int scaledPaddingHeight = PADDINGHEIGHT;
        private readonly int scaledMaxTextWidth = MAXTEXTWIDTH;
        private readonly int scaledMaxTextHeight = MAXTEXTHEIGHT;
        private readonly int scaledButtonTopPadding = BUTTONTOPPADDING;
        private readonly int scaledButtonDetailsLeftPadding = BUTTONDETAILS_LEFTPADDING;
        private readonly int scaledMessageTopPadding = MESSAGE_TOPPADDING;
        private readonly int scaledHeightPadding = HEIGHTPADDING;
        private readonly int scaledButtonWidth = BUTTONWIDTH;
        private readonly int scaledButtonHeight = BUTTONHEIGHT;
        private readonly int scaledButtonAlignmentWidth = BUTTONALIGNMENTWIDTH;
        private readonly int scaledButtonAlignmentPadding = BUTTONALIGNMENTPADDING;
        private readonly int scaledDetailsWidthPadding = DETAILSWIDTHPADDING;
        private readonly int scaledDetailsHeight = DETAILSHEIGHT;
        private readonly int scaledPictureWidth = PICTUREWIDTH;
        private readonly int scaledPictureHeight = PICTUREHEIGHT;
        private readonly int scaledExceptionMessageVerticalPadding = EXCEPTIONMESSAGEVERTICALPADDING;

        private readonly PictureBox pictureBox = new PictureBox();
        private readonly Label message = new Label();
        private readonly Button continueButton = new Button();
        private readonly Button quitButton = new Button();
        private readonly Button detailsButton = new Button();
        private readonly Button helpButton = new Button();
        private readonly TextBox details = new TextBox();
        private Bitmap expandImage;
        private Bitmap collapseImage;
        private bool detailsVisible;

        /// <summary>
        ///  Initializes a new instance of the <see cref='ThreadExceptionDialog'/> class.
        /// </summary>
        public ThreadExceptionDialog(Exception t)
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                scaledMaxWidth = LogicalToDeviceUnits(MAXWIDTH);
                scaledMaxHeight = LogicalToDeviceUnits(MAXHEIGHT);
                scaledPaddingWidth = LogicalToDeviceUnits(PADDINGWIDTH);
                scaledPaddingHeight = LogicalToDeviceUnits(PADDINGHEIGHT);
                scaledMaxTextWidth = LogicalToDeviceUnits(MAXTEXTWIDTH);
                scaledMaxTextHeight = LogicalToDeviceUnits(MAXTEXTHEIGHT);
                scaledButtonTopPadding = LogicalToDeviceUnits(BUTTONTOPPADDING);
                scaledButtonDetailsLeftPadding = LogicalToDeviceUnits(BUTTONDETAILS_LEFTPADDING);
                scaledMessageTopPadding = LogicalToDeviceUnits(MESSAGE_TOPPADDING);
                scaledHeightPadding = LogicalToDeviceUnits(HEIGHTPADDING);
                scaledButtonWidth = LogicalToDeviceUnits(BUTTONWIDTH);
                scaledButtonHeight = LogicalToDeviceUnits(BUTTONHEIGHT);
                scaledButtonAlignmentWidth = LogicalToDeviceUnits(BUTTONALIGNMENTWIDTH);
                scaledButtonAlignmentPadding = LogicalToDeviceUnits(BUTTONALIGNMENTPADDING);
                scaledDetailsWidthPadding = LogicalToDeviceUnits(DETAILSWIDTHPADDING);
                scaledDetailsHeight = LogicalToDeviceUnits(DETAILSHEIGHT);
                scaledPictureWidth = LogicalToDeviceUnits(PICTUREWIDTH);
                scaledPictureHeight = LogicalToDeviceUnits(PICTUREHEIGHT);
                scaledExceptionMessageVerticalPadding = LogicalToDeviceUnits(EXCEPTIONMESSAGEVERTICALPADDING);
            }

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
                    buttons = new Button[] { continueButton };
                }
                else
                {
                    buttons = new Button[] { continueButton, helpButton };
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
                    buttons = new Button[] { detailsButton, continueButton, quitButton };
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
                    buttons = new Button[] { detailsButton, continueButton };
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

            StringBuilder detailsTextBuilder = new StringBuilder();
            string newline = "\r\n";
            string separator = SR.ExDlgMsgSeperator;
            string sectionseparator = SR.ExDlgMsgSectionSeperator;
            if (Application.CustomThreadExceptionHandlerAttached)
            {
                detailsTextBuilder.Append(SR.ExDlgMsgHeaderNonSwitchable);
            }
            else
            {
                detailsTextBuilder.Append(SR.ExDlgMsgHeaderSwitchable);
            }
            detailsTextBuilder.Append(string.Format(CultureInfo.CurrentCulture, sectionseparator, SR.ExDlgMsgExceptionSection));
            detailsTextBuilder.Append(t.ToString());
            detailsTextBuilder.Append(newline);
            detailsTextBuilder.Append(newline);
            detailsTextBuilder.Append(string.Format(CultureInfo.CurrentCulture, sectionseparator, SR.ExDlgMsgLoadedAssembliesSection));

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName name = asm.GetName();
                string fileVer = SR.NotAvailable;

                try
                {
                    if (name.EscapedCodeBase != null && name.EscapedCodeBase.Length > 0)
                    {
                        Uri codeBase = new Uri(name.EscapedCodeBase);
                        if (codeBase.Scheme == "file")
                        {
                            fileVer = FileVersionInfo.GetVersionInfo(NativeMethods.GetLocalPath(name.EscapedCodeBase)).FileVersion;
                        }
                    }
                }
                catch (IO.FileNotFoundException)
                {
                }
                detailsTextBuilder.Append(string.Format(SR.ExDlgMsgLoadedAssembliesEntry, name.Name, name.Version, fileVer, name.EscapedCodeBase));
                detailsTextBuilder.Append(separator);
            }

            detailsTextBuilder.Append(string.Format(CultureInfo.CurrentCulture, sectionseparator, SR.ExDlgMsgJITDebuggingSection));
            if (Application.CustomThreadExceptionHandlerAttached)
            {
                detailsTextBuilder.Append(SR.ExDlgMsgFooterNonSwitchable);
            }
            else
            {
                detailsTextBuilder.Append(SR.ExDlgMsgFooterSwitchable);
            }

            detailsTextBuilder.Append(newline);
            detailsTextBuilder.Append(newline);

            string detailsText = detailsTextBuilder.ToString();

            Graphics g = message.CreateGraphicsInternal();

            Size textSize = new Size(scaledMaxWidth - scaledPaddingWidth, int.MaxValue);

            if (DpiHelper.IsScalingRequirementMet && !Control.UseCompatibleTextRenderingDefault)
            {
                // we need to measure string using API that matches the rendering engine - TextRenderer.MeasureText for GDI
                textSize = Size.Ceiling(TextRenderer.MeasureText(messageText, Font, textSize, TextFormatFlags.WordBreak));
            }
            else
            {
                // if HighDpi improvements are not enabled, or rendering mode is GDI+, use Graphics.MeasureString
                textSize = Size.Ceiling(g.MeasureString(messageText, Font, textSize.Width));
            }

            textSize.Height += scaledExceptionMessageVerticalPadding;
            g.Dispose();

            if (textSize.Width < scaledMaxTextWidth)
            {
                textSize.Width = scaledMaxTextWidth;
            }

            if (textSize.Height > scaledMaxHeight)
            {
                textSize.Height = scaledMaxHeight;
            }

            int width = textSize.Width + scaledPaddingWidth;
            int buttonTop = Math.Max(textSize.Height, scaledMaxTextHeight) + scaledPaddingHeight;

            Form activeForm = Form.ActiveForm;
            if (activeForm is null || activeForm.Text.Length == 0)
            {
                Text = SR.ExDlgCaption;
            }
            else
            {
                Text = string.Format(SR.ExDlgCaption2, activeForm.Text);
            }

            AcceptButton = continueButton;
            CancelButton = continueButton;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Icon = null;
            ClientSize = new Size(width, buttonTop + scaledButtonTopPadding);
            TopMost = true;

            pictureBox.Location = new Point(scaledPictureWidth / 8, scaledPictureHeight / 8);
            pictureBox.Size = new Size(scaledPictureWidth * 3 / 4, scaledPictureHeight * 3 / 4);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            if (t is Security.SecurityException)
            {
                pictureBox.Image = SystemIcons.Information.ToBitmap();
            }
            else
            {
                pictureBox.Image = SystemIcons.Error.ToBitmap();
            }
            Controls.Add(pictureBox);
            message.SetBounds(scaledPictureWidth,
                              scaledMessageTopPadding + (scaledMaxTextHeight - Math.Min(textSize.Height, scaledMaxTextHeight)) / 2,
                              textSize.Width, textSize.Height);
            message.Text = messageText;
            Controls.Add(message);

            continueButton.Text = SR.ExDlgContinue;
            continueButton.FlatStyle = FlatStyle.Standard;
            continueButton.DialogResult = DialogResult.Cancel;

            quitButton.Text = SR.ExDlgQuit;
            quitButton.FlatStyle = FlatStyle.Standard;
            quitButton.DialogResult = DialogResult.Abort;

            helpButton.Text = SR.ExDlgHelp;
            helpButton.FlatStyle = FlatStyle.Standard;
            helpButton.DialogResult = DialogResult.Yes;

            detailsButton.Text = SR.ExDlgShowDetails;
            detailsButton.FlatStyle = FlatStyle.Standard;
            detailsButton.Click += new EventHandler(DetailsClick);

            Button b = null;
            int startIndex = 0;

            if (detailAnchor)
            {
                b = detailsButton;

                expandImage = DpiHelper.GetBitmapFromIcon(GetType(), DownBitmapName);
                collapseImage = DpiHelper.GetBitmapFromIcon(GetType(), UpBitmapName);

                if (DpiHelper.IsScalingRequirementMet)
                {
                    ScaleBitmapLogicalToDevice(ref expandImage);
                    ScaleBitmapLogicalToDevice(ref collapseImage);
                }

                b.SetBounds(scaledButtonDetailsLeftPadding, buttonTop, scaledButtonWidth, scaledButtonHeight);
                b.Image = expandImage;
                b.ImageAlign = ContentAlignment.MiddleLeft;
                Controls.Add(b);
                startIndex = 1;
            }

            int buttonLeft = (width - scaledButtonDetailsLeftPadding - ((buttons.Length - startIndex) * scaledButtonAlignmentWidth - scaledButtonAlignmentPadding));

            for (int i = startIndex; i < buttons.Length; i++)
            {
                b = buttons[i];
                b.SetBounds(buttonLeft, buttonTop, scaledButtonWidth, scaledButtonHeight);
                Controls.Add(b);
                buttonLeft += scaledButtonAlignmentWidth;
            }

            details.Text = detailsText;
            details.ScrollBars = ScrollBars.Both;
            details.Multiline = true;
            details.ReadOnly = true;
            details.WordWrap = false;
            details.TabStop = false;
            details.AcceptsReturn = false;

            details.SetBounds(scaledButtonDetailsLeftPadding, buttonTop + scaledButtonTopPadding, width - scaledDetailsWidthPadding, scaledDetailsHeight);
            details.Visible = detailsVisible;
            Controls.Add(details);
            if (DpiHelper.IsScalingRequirementMet)
            {
                DpiChanged += ThreadExceptionDialog_DpiChanged;
            }
        }

        private void ThreadExceptionDialog_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            if (expandImage != null)
            {
                expandImage.Dispose();
            }
            expandImage = DpiHelper.GetBitmapFromIcon(GetType(), DownBitmapName);

            if (collapseImage != null)
            {
                collapseImage.Dispose();
            }
            collapseImage = DpiHelper.GetBitmapFromIcon(GetType(), UpBitmapName);

            ScaleBitmapLogicalToDevice(ref expandImage);
            ScaleBitmapLogicalToDevice(ref collapseImage);

            detailsButton.Image = detailsVisible ? collapseImage : expandImage;
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
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  Called when the details button is clicked.
        /// </summary>
        private void DetailsClick(object sender, EventArgs eventargs)
        {
            int delta = details.Height + scaledHeightPadding;
            if (detailsVisible)
            {
                delta = -delta;
            }

            Height += delta;
            detailsVisible = !detailsVisible;
            details.Visible = detailsVisible;
            detailsButton.Image = detailsVisible ? collapseImage : expandImage;
        }

        private static string Trim(string s)
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

            return s.Substring(0, i);
        }
    }
}
