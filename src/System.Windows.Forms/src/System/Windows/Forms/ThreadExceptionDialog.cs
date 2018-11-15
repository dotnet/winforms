// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Text;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Runtime.InteropServices;
    using System.Globalization;

    /// <include file='doc\ThreadExceptionDialog.uex' path='docs/doc[@for="ThreadExceptionDialog"]/*' />
    /// <internalonly/>
    /// <devdoc>
    ///    <para>
    ///       Implements a dialog box that is displayed when an unhandled exception occurs in
    ///       a thread.
    ///    </para>
    /// </devdoc>
    [
        ComVisible(true),
        ClassInterface(ClassInterfaceType.AutoDispatch),
        SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode),
        SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
        UIPermission(SecurityAction.Assert, Window=UIPermissionWindow.AllWindows)]
    public class ThreadExceptionDialog : Form {

        private const string DownBitmapName = "down.bmp";
        private const string UpBitmapName = "up.bmp";

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

        private int scaledMaxWidth = MAXWIDTH;
        private int scaledMaxHeight = MAXHEIGHT;
        private int scaledPaddingWidth = PADDINGWIDTH;
        private int scaledPaddingHeight = PADDINGHEIGHT;
        private int scaledMaxTextWidth = MAXTEXTWIDTH;
        private int scaledMaxTextHeight = MAXTEXTHEIGHT;
        private int scaledButtonTopPadding = BUTTONTOPPADDING;
        private int scaledButtonDetailsLeftPadding = BUTTONDETAILS_LEFTPADDING;
        private int scaledMessageTopPadding = MESSAGE_TOPPADDING;
        private int scaledHeightPadding = HEIGHTPADDING;
        private int scaledButtonWidth = BUTTONWIDTH;
        private int scaledButtonHeight = BUTTONHEIGHT;
        private int scaledButtonAlignmentWidth = BUTTONALIGNMENTWIDTH;
        private int scaledButtonAlignmentPadding = BUTTONALIGNMENTPADDING;
        private int scaledDetailsWidthPadding = DETAILSWIDTHPADDING;
        private int scaledDetailsHeight = DETAILSHEIGHT;
        private int scaledPictureWidth = PICTUREWIDTH;
        private int scaledPictureHeight = PICTUREHEIGHT;
        private int scaledExceptionMessageVerticalPadding = EXCEPTIONMESSAGEVERTICALPADDING;

        private PictureBox pictureBox = new PictureBox();
        private Label message = new Label();
        private Button continueButton = new Button();
        private Button quitButton = new Button();
        private Button detailsButton = new Button();
        private Button helpButton = new Button();
        private TextBox details = new TextBox();
        private Bitmap expandImage = null;
        private Bitmap collapseImage = null;
        private bool detailsVisible = false;

        /// <include file='doc\ThreadExceptionDialog.uex' path='docs/doc[@for="ThreadExceptionDialog.ThreadExceptionDialog"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ThreadExceptionDialog'/> class.
        ///       
        ///    </para>
        /// </devdoc>
        public ThreadExceptionDialog(Exception t) {

            if (DpiHelper.IsScalingRequirementMet) {
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

            string messageRes;
            string messageText;
            Button[] buttons;
            bool detailAnchor = false;

            WarningException w = t as WarningException;
            if (w != null) {
                messageRes = SR.ExDlgWarningText;
                messageText = w.Message;
                if (w.HelpUrl == null) {
                    buttons = new Button[] {continueButton};
                }
                else {
                    buttons = new Button[] {continueButton, helpButton};
                }
            }
            else {
                messageText = t.Message;

                detailAnchor = true;
                
                if (Application.AllowQuit) {
                    if (t is SecurityException) {
                        messageRes = "ExDlgSecurityErrorText";
                    }
                    else {
                        messageRes = "ExDlgErrorText";
                    }
                    buttons = new Button[] {detailsButton, continueButton, quitButton};
                }
                else {
                    if (t is SecurityException) {
                        messageRes = "ExDlgSecurityContinueErrorText";
                    }
                    else {
                        messageRes = "ExDlgContinueErrorText";
                    }
                    buttons = new Button[] {detailsButton, continueButton};
                }
            }

            if (messageText.Length == 0) {
                messageText = t.GetType().Name;
            }
            if (t is SecurityException) {
                messageText = string.Format(messageRes, t.GetType().Name, Trim(messageText));
            }
            else {
                messageText = string.Format(messageRes, Trim(messageText));
            }

            StringBuilder detailsTextBuilder = new StringBuilder();
            string newline = "\r\n";
            string separator = SR.ExDlgMsgSeperator;
            string sectionseparator = SR.ExDlgMsgSectionSeperator;
            if (Application.CustomThreadExceptionHandlerAttached) {
                detailsTextBuilder.Append(SR.ExDlgMsgHeaderNonSwitchable);
            }
            else {
                detailsTextBuilder.Append(SR.ExDlgMsgHeaderSwitchable);
            }
            detailsTextBuilder.Append(string.Format(CultureInfo.CurrentCulture, sectionseparator, SR.ExDlgMsgExceptionSection));
            detailsTextBuilder.Append(t.ToString());
            detailsTextBuilder.Append(newline);
            detailsTextBuilder.Append(newline);
            detailsTextBuilder.Append(string.Format(CultureInfo.CurrentCulture, sectionseparator, SR.ExDlgMsgLoadedAssembliesSection));
            new FileIOPermission(PermissionState.Unrestricted).Assert();
            try {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
                    AssemblyName name = asm.GetName();
                    string fileVer = SR.NotAvailable;

                    try {
                        
                        // 





                        if (name.EscapedCodeBase != null && name.EscapedCodeBase.Length > 0) {
                            Uri codeBase = new Uri(name.EscapedCodeBase);
                            if (codeBase.Scheme == "file") {
                                fileVer = FileVersionInfo.GetVersionInfo(NativeMethods.GetLocalPath(name.EscapedCodeBase)).FileVersion;
                            }
                        }
                    }
                    catch(System.IO.FileNotFoundException){
                    }
                    detailsTextBuilder.Append(string.Format(SR.ExDlgMsgLoadedAssembliesEntry, name.Name, name.Version, fileVer, name.EscapedCodeBase));
                    detailsTextBuilder.Append(separator);
                }
            }
            finally {
                CodeAccessPermission.RevertAssert();
            }
            
            detailsTextBuilder.Append(string.Format(CultureInfo.CurrentCulture, sectionseparator, SR.ExDlgMsgJITDebuggingSection));
            if (Application.CustomThreadExceptionHandlerAttached) {
                detailsTextBuilder.Append(SR.ExDlgMsgFooterNonSwitchable);
            }
            else {
                detailsTextBuilder.Append(SR.ExDlgMsgFooterSwitchable);
            }

            detailsTextBuilder.Append(newline);
            detailsTextBuilder.Append(newline);

            string detailsText = detailsTextBuilder.ToString();

            Graphics g = message.CreateGraphicsInternal();

            Size textSize = new Size(scaledMaxWidth - scaledPaddingWidth, int.MaxValue);

            if (DpiHelper.IsScalingRequirementMet && Label.UseCompatibleTextRenderingDefault == false) {
                // we need to measure string using API that matches the rendering engine - TextRenderer.MeasureText for GDI
                textSize = Size.Ceiling(TextRenderer.MeasureText(messageText, Font, textSize, TextFormatFlags.WordBreak));
            }
            else {
                // if HighDpi improvements are not enabled, or rendering mode is GDI+, use Graphics.MeasureString
                textSize = Size.Ceiling(g.MeasureString(messageText, Font, textSize.Width));
            }

            textSize.Height += scaledExceptionMessageVerticalPadding;
            g.Dispose();

            if (textSize.Width < scaledMaxTextWidth) textSize.Width = scaledMaxTextWidth;
            if (textSize.Height > scaledMaxHeight) textSize.Height = scaledMaxHeight;

            int width = textSize.Width + scaledPaddingWidth;
            int buttonTop = Math.Max(textSize.Height, scaledMaxTextHeight) + scaledPaddingHeight;

            // 


            IntSecurity.GetParent.Assert();
            try {
                Form activeForm = Form.ActiveForm;
                if (activeForm == null || activeForm.Text.Length == 0) {
                    Text = SR.ExDlgCaption;
                }
                else {
                    Text = string.Format(SR.ExDlgCaption2, activeForm.Text);
                }
            }
            finally {
                CodeAccessPermission.RevertAssert();
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

            pictureBox.Location = new Point(scaledPictureWidth/8, scaledPictureHeight/8);
            pictureBox.Size = new Size(scaledPictureWidth*3/4, scaledPictureHeight*3/4);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            if (t is SecurityException) {
                pictureBox.Image = SystemIcons.Information.ToBitmap();
            }
            else {
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
            
            if (detailAnchor) {
                b = detailsButton;

                expandImage = new Bitmap(this.GetType(), DownBitmapName);
                expandImage.MakeTransparent();
                collapseImage = new Bitmap(this.GetType(), UpBitmapName);
                collapseImage.MakeTransparent();

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
            
            for (int i = startIndex; i < buttons.Length; i++) {
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
            if (DpiHelper.IsScalingRequirementMet) {
                DpiChanged += ThreadExceptionDialog_DpiChanged;
            }
        }

        private void ThreadExceptionDialog_DpiChanged(object sender, DpiChangedEventArgs e) {
            if (expandImage != null) {
                expandImage.Dispose();
            }
            expandImage = new Bitmap(this.GetType(), DownBitmapName);
            expandImage.MakeTransparent();

            if (collapseImage != null) {
                collapseImage.Dispose();
            }
            collapseImage = new Bitmap(this.GetType(), UpBitmapName);
            collapseImage.MakeTransparent();

            ScaleBitmapLogicalToDevice(ref expandImage);
            ScaleBitmapLogicalToDevice(ref collapseImage);

            detailsButton.Image = detailsVisible ? collapseImage : expandImage;
        }

        /// <include file='doc\ThreadExceptionDialog.uex' path='docs/doc[@for="ThreadExceptionDialog.AutoSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Hide the property
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
            get
            {
                return base.AutoSize;
            }
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
            set
            {
                base.AutoSize = value;
            }
        }

        /// <include file='doc\ThreadExceptionDialog.uex' path='docs/doc[@for="ThreadExceptionDialog.AutoSizeChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged
        {
            add
            {
                base.AutoSizeChanged += value;
            }
            remove
            {
                base.AutoSizeChanged -= value;
            }
        }                

        /// <include file='doc\ThreadExceptionDialog.uex' path='docs/doc[@for="ThreadExceptionDialog.DetailsClick"]/*' />
        /// <devdoc>
        ///     Called when the details button is clicked.
        /// </devdoc>
        private void DetailsClick(object sender, EventArgs eventargs) {
            int delta = details.Height + scaledHeightPadding;
            if (detailsVisible) delta = -delta;
            Height = Height + delta;
            detailsVisible = !detailsVisible;
            details.Visible = detailsVisible;
            detailsButton.Image = detailsVisible ? collapseImage : expandImage;
        }

        private static string Trim(string s) {
            if (s == null) return s;
            int i = s.Length;
            while (i > 0 && s[i - 1] == '.') i--;
            return s.Substring(0, i);
        }
    }
}
