// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class Dialogs : Form
    {
        private sealed class ClientGuidConverter : GuidConverter
        {
            private StandardValuesCollection? _values;

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

            public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                // base class is for plain Guid, but ClientGuid is nullable Guid, which we need to implement separately
                if ((value is null) || (value is string str && str.Length == 0))
                    return null;

                return base.ConvertFrom(context, culture, value);
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (_values is null)
                {
                    // chose from two GUIDs we pregenerated for testing
                    _values = new StandardValuesCollection(new Guid?[]
                    {
                        null,
                        new Guid("38EA9AE9-13BE-4992-9482-DAD370894BBD"),
                        new Guid("46DFEE70-A89E-4D9A-8842-6D46DBC1F195"),
                    });
                }

                return _values;
            }
        }

        private sealed class ExposedClientGuidMetadata
        {
            [Browsable(true)]
            [TypeConverter(typeof(ClientGuidConverter))]
            public Guid? ClientGuid { get; set; }
        }

        private readonly ToolStripButton _btnOpen;

        public Dialogs()
        {
            InitializeComponent();

            TypeDescriptor.AddProvider(new AssociatedMetadataTypeTypeDescriptionProvider(openFileDialog1.GetType(), typeof(ExposedClientGuidMetadata)), openFileDialog1);
            TypeDescriptor.AddProvider(new AssociatedMetadataTypeTypeDescriptionProvider(saveFileDialog1.GetType(), typeof(ExposedClientGuidMetadata)), saveFileDialog1);
            TypeDescriptor.AddProvider(new AssociatedMetadataTypeTypeDescriptionProvider(folderBrowserDialog1.GetType(), typeof(ExposedClientGuidMetadata)), folderBrowserDialog1);

            _btnOpen = new("Open dialog")
            {
                Image = (System.Drawing.Bitmap)(resources.GetObject("OpenDialog")),
                Enabled = false
            };

            _btnOpen.Click += (s, e) =>
            {
                if (propertyGrid1.SelectedObject is CommonDialog dialog)
                {
                    dialog.ShowDialog(this);
                    return;
                }

                if (propertyGrid1.SelectedObject is Form form)
                {
                    form.ShowDialog(this);
                    return;
                }
            };

            ToolStrip toolbar = GetToolbar();
            toolbar.Items.Add(new ToolStripSeparator { Visible = true });
            toolbar.Items.Add(_btnOpen);
        }

        private ToolStrip GetToolbar()
        {
            foreach (Control control in propertyGrid1.Controls)
            {
                ToolStrip? toolStrip = control as ToolStrip;
                if (toolStrip is not null)
                {
                    return toolStrip;
                }
            }

            throw new MissingMemberException("Unable to find the toolstrip in the PropertyGrid.");
        }

        private void btnOpenFileDialog_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = openFileDialog1;
        }

        private void btnSaveFileDialog_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = saveFileDialog1;
        }

        private void btnFolderBrowserDialog_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = folderBrowserDialog1;
        }

        private void btnPrintDialog_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = printDialog1;
        }

        private void btnThreadExceptionDialog_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = null;

            using ThreadExceptionDialog dialog = new(new Exception("Really long exception description string, because we want to see if it properly wraps around or is truncated."));
            dialog.ShowDialog(this);
        }

        private void propertyGrid1_SelectedObjectsChanged(object sender, EventArgs e)
        {
            _btnOpen.Enabled = propertyGrid1.SelectedObject is not null;
        }
    }
}
