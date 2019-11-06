// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    internal class MaskedTextBoxTextEditorDropDown : UserControl
    {
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
            _cloneMtb.MaskInputRejected += new MaskInputRejectedEventHandler(maskedTextBox_MaskInputRejected);
            _cloneMtb.KeyDown += new KeyEventHandler(maskedTextBox_KeyDown);

            // 
            // errorProvider
            // 
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            _errorProvider.ContainerControl = this;

            // 
            // MaskedTextBoxTextEditorDropDown
            // 
            Controls.Add(_cloneMtb);

            BackColor = System.Drawing.SystemColors.Control;
            BorderStyle = BorderStyle.FixedSingle;
            Name = "MaskedTextBoxTextEditorDropDown";
            Padding = new Padding(16);
            Size = new System.Drawing.Size(100, 52);
            ((System.ComponentModel.ISupportInitialize)(_errorProvider)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        public string Value
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

        private void maskedTextBox_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            _errorProvider.SetError(_cloneMtb, MaskedTextBoxDesigner.GetMaskInputRejectedErrorMessage(e));
        }

        private void maskedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _errorProvider.Clear();
        }
    }
}
