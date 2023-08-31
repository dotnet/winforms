// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolStripComboBox
{
    internal partial class ToolStripComboBoxControl : ComboBox
    {
        public ToolStripComboBoxControl()
        {
            FlatStyle = FlatStyle.Popup;
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public ToolStripComboBox? Owner { get; set; }

        private ProfessionalColorTable ColorTable
        {
            get
            {
                if (Owner is not null)
                {
                    if (Owner.Renderer is ToolStripProfessionalRenderer renderer)
                    {
                        return renderer.ColorTable;
                    }
                }

                return ProfessionalColors.ColorTable;
            }
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this ToolStripComboBoxControl.
        /// </summary>
        /// <returns>
        ///  The new instance of the accessibility object for this ToolStripComboBoxControl item
        /// </returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripComboBoxControlAccessibleObject(this);
        }

        internal override FlatComboAdapter CreateFlatComboAdapterInstance()
        {
            return new ToolStripComboBoxFlatComboAdapter(this);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Down:
                    case Keys.Up:
                        return true;
                }
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            Invalidate();
            Update();
        }

        internal override bool SupportsUiaProviders => true;
    }
}
