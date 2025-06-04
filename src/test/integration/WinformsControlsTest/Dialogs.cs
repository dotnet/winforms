// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Design;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class Dialogs : Form
{
    private readonly ToolStripButton _btnOpen;

    public Dialogs()
    {
        InitializeComponent();

        // expose ClientGuid to be configurable in the property grid by overriding the metadata,
        // but only for these specific instance of the dialogs, not in general
        TypeDescriptor.AddProvider(new AssociatedMetadataTypeTypeDescriptionProvider(openFileDialog1.GetType(), typeof(ExposedClientGuidMetadata)), openFileDialog1);
        TypeDescriptor.AddProvider(new AssociatedMetadataTypeTypeDescriptionProvider(saveFileDialog1.GetType(), typeof(ExposedClientGuidMetadata)), saveFileDialog1);
        TypeDescriptor.AddProvider(new AssociatedMetadataTypeTypeDescriptionProvider(folderBrowserDialog1.GetType(), typeof(ExposedClientGuidMetadata)), folderBrowserDialog1);

        _btnOpen = new("&Open dialog")
        {
            Image = (Bitmap?)resources.GetObject("OpenDialog"),
            Enabled = false,
        };

        _btnOpen.Click += (s, e) =>
        {
            if (propertyGrid1.SelectedObject is OpenFileDialog openFileDialog)
            {
                openFileDialog.ShowDialog(this);
                MessageBox.Show(string.Join(',', openFileDialog.FileNames), "File Names");
                return;
            }

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

    private void DisposeIfNeeded()
    {
        if (propertyGrid1.SelectedObject is IDisposable disposable)
        {
            disposable.Dispose();
        }
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

    private void btnColorDialog_Click(object sender, EventArgs e)
    {
        DisposeIfNeeded();
        propertyGrid1.SelectedObject = null;

        Type? typeCustomColorDialog = typeof(ColorEditor).Assembly.GetTypes().SingleOrDefault(t => t.Name == "CustomColorDialog")
            ?? throw new InvalidOperationException("Unable to locate 'CustomColorDialog' type.");

        using ColorDialog dialog = (ColorDialog)Activator.CreateInstance(typeCustomColorDialog)!;
        dialog.ShowDialog(this);
    }

    private void btnOpenFileDialog_Click(object sender, EventArgs e)
    {
        DisposeIfNeeded();
        propertyGrid1.SelectedObject = openFileDialog1;
    }

    private void btnSaveFileDialog_Click(object sender, EventArgs e)
    {
        DisposeIfNeeded();
        propertyGrid1.SelectedObject = saveFileDialog1;
    }

    private void btnFolderBrowserDialog_Click(object sender, EventArgs e)
    {
        DisposeIfNeeded();
        propertyGrid1.SelectedObject = folderBrowserDialog1;
    }

    private void btnPrintDialog_Click(object sender, EventArgs e)
    {
        DisposeIfNeeded();
        propertyGrid1.SelectedObject = printDialog1;
    }

    private void btnThreadExceptionDialog_Click(object sender, EventArgs e)
    {
        DisposeIfNeeded();
        propertyGrid1.SelectedObject = null;

        using ThreadExceptionDialog dialog = new(new InvalidOperationException("Really long exception description string, because we want to see if it properly wraps around or is truncated."));
        dialog.ShowDialog(this);
    }

    private void propertyGrid1_SelectedObjectsChanged(object sender, EventArgs e)
    {
        _btnOpen.Enabled = propertyGrid1.SelectedObject is not null;
    }
}
