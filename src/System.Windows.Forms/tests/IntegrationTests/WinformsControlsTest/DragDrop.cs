// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class DragDrop : Form
{
    private const string DragDropDataDirectory = @"Data\DragDrop";
    private const string NyanCatAsciiTxt = @"NyanCatAscii.txt";
    private const string DragAcceptRtf = @"DragAccept.rtf";
    private readonly string _nyanCatAscii = string.Empty;
    private readonly Bitmap _dragAcceptBmp = new(@"Data\DragDrop\DragAccept.bmp");
    private readonly Bitmap _nyanCatAscii301Bmp = new(@"Data\DragDrop\NyanCatAscii_301.bmp");
    private readonly Bitmap _nyanCatBmp = new(@"Data\DragDrop\NyanCat1.bmp");
    private readonly Bitmap _toolStripAsciiCatBmp = new(@"Data\DragDrop\ToolStripAsciiCat.bmp");
    private readonly Bitmap _toolStripDragAcceptBmp = new(@"Data\DragDrop\ToolStripDragAccept.bmp");
    private readonly Bitmap _toolStripNyanCatBmp = new(@"Data\DragDrop\ToolStripNyanCat.bmp");
    private readonly List<PictureBox> _pictureBoxList;
    private ContextMenuStrip? _catContextMenuStrip;

    public DragDrop()
    {
        InitializeComponent();

        _nyanCatAscii = ReadAsciiText();
        _pictureBoxList =
        [
            pictureBox1,
            pictureBox2,
            pictureBox3,
            pictureBox4,
            pictureBox5
        ];

        AllowDrop = true;
        DragEnter += DragDrop_DragEnter;

        pictureBox1.AllowDrop = true;
        pictureBox1.DragEnter += PictureBox_DragEnter;
        pictureBox1.DragDrop += PictureBox_DragDrop;
        pictureBox1.MouseDown += PictureBox_MouseDown;
        pictureBox1.GiveFeedback += PictureBox_GiveFeedback;

        pictureBox2.AllowDrop = true;
        pictureBox2.DragEnter += PictureBox_DragEnter;
        pictureBox2.DragDrop += PictureBox_DragDrop;
        pictureBox2.MouseDown += PictureBox_MouseDown;
        pictureBox2.GiveFeedback += PictureBox_GiveFeedback;

        pictureBox3.AllowDrop = true;
        pictureBox3.DragEnter += PictureBox_DragEnter;
        pictureBox3.DragDrop += PictureBox_DragDrop;
        pictureBox3.MouseDown += PictureBox_MouseDown;
        pictureBox3.GiveFeedback += PictureBox_GiveFeedback;

        pictureBox4.AllowDrop = true;
        pictureBox4.DragEnter += PictureBox_DragEnter;
        pictureBox4.DragDrop += PictureBox_DragDrop;
        pictureBox4.MouseDown += PictureBox_MouseDown;
        pictureBox4.GiveFeedback += PictureBox_GiveFeedback;

        pictureBox5.AllowDrop = true;
        pictureBox5.DragEnter += PictureBox_DragEnter;
        pictureBox5.DragDrop += PictureBox_DragDrop;
        pictureBox5.MouseDown += PictureBox_MouseDown;
        pictureBox5.GiveFeedback += PictureBox_GiveFeedback;

        textBox.AllowDrop = true;
        textBox.DragEnter += TextBox_DragEnter;
        textBox.DragOver += TextBox_DragOver;
        textBox.DragDrop += TextBox_DragDrop;

        richTextBox.AllowDrop = true;
        richTextBox.EnableAutoDragDrop = true;
        richTextBox.DragEnter += RichTextBox_DragEnter;
        richTextBox.DragDrop += RichTextBox_DragDrop;

        buttonOpenCats.Click += ButtonOpenCats_Click;
        buttonClear.Click += ButtonClear_Click;

        CreateCatToolStrip();
    }

    private void ButtonClear_Click(object? sender, EventArgs e)
    {
        ClearCats();
        richTextBox.Clear();
        textBox.Clear();
    }

    private void ButtonOpenCats_Click(object? sender, EventArgs e)
        => OpenCats();

    private void DragDrop_DragEnter(object? sender, DragEventArgs e)
    {
        e.DropImageType = DropImageType.Warning;
        e.Effect = DragDropEffects.None;
    }

    private void PictureBox_DragEnter(object? sender, DragEventArgs e)
    {
        if (sender is not PictureBox pictureBox || e.Data is null)
        {
            return;
        }

        if (e.Data.GetDataPresent(DataFormats.FileDrop)
            && e.Data.GetData(DataFormats.FileDrop) is string[] files
            && files.Length > 0 && files.Length <= 5)
        {
            if (files.All(file => file.Contains("NyanCat") && file.Contains(".bmp")))
            {
                // Set the target drop image to a plus sign (+).
                e.DropImageType = DropImageType.Copy;

                // Set the target drop text.
                e.Message = $"{e.DropImageType} %1 from Explorer";
                e.MessageReplacementToken = $"{(files.Length > 1 ? "Cats" : "Cat")}";

                // Set the target drop effect.
                e.Effect = DragDropEffects.Copy;
            }
            else if (files.Length == 1 && files.Any(file => file.Contains("DragAccept.rtf")))
            {
                // Set the target drop image to a red bisected circle.
                e.DropImageType = DropImageType.None;

                // Set the target drop text.
                e.Message = $"{Path.GetFileNameWithoutExtension(files[0])}%1";
                e.MessageReplacementToken = Path.GetExtension(files[0]);

                // Set the target drop effect.
                e.Effect = DragDropEffects.None;
            }
        }
        else if (e.Data.GetDataPresent(nameof(_nyanCatBmp)))
        {
            // Set the target drop image to a plus sign (+).
            e.DropImageType = DropImageType.Copy;

            // Set the target drop text.
            e.Message = $"{e.DropImageType} %1 from WinForms";
            e.MessageReplacementToken = "Cat";

            // Set the target drop effect.
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void PictureBox_DragDrop(object? sender, DragEventArgs e)
    {
        if (sender is PictureBox pictureBox && e.Data is not null)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)
                && e.Data.GetData(DataFormats.FileDrop) is string[] files
                && files.Length > 0 && files.Length <= 5
                && files.All(file => file.Contains("NyanCat") && file.Contains(".bmp")))
            {
                LoadCats(pictureBox, files);
            }
            else if (e.Data.GetDataPresent(nameof(_nyanCatBmp))
                && e.Data.GetData(nameof(_nyanCatBmp)) is Bitmap nyanCatBmp)
            {
                LoadCat(pictureBox, nyanCatBmp);
            }
        }
    }

    private void PictureBox_MouseDown(object? sender, MouseEventArgs e)
    {
        if (sender is PictureBox pictureBox)
        {
            // Create the ascii cat data object.
            DataObject data = new(nameof(_nyanCatAscii), _nyanCatAscii);

            // Call DoDragDrop and set the initial drag image.
            // Set useDefaultDragImage to true to specify a layered drag image window with size 96x96.
            pictureBox.DoDragDrop(data, DragDropEffects.All, _nyanCatBmp, new Point(0, 96), true);
        }
    }

    private void PictureBox_GiveFeedback(object? sender, GiveFeedbackEventArgs e)
    {
        if (e.Effect.Equals(DragDropEffects.Copy))
        {
            // Specify a new drag image in GiveFeedback.
            // Note the outer edges of the drag image are blended out if the image width or height exceeds 300 pixels.
            e.DragImage = _nyanCatAscii301Bmp;

            // Set the cursor to the bottom left-hand corner of the drag image.
            e.CursorOffset = new Point(0, 111);

            // Set UseDefaultDragImage to false to remove the 96x96 layered drag image window.
            e.UseDefaultDragImage = false;
        }
        else
        {
            e.DragImage = _nyanCatBmp;
            e.CursorOffset = new Point(0, 96);
            e.UseDefaultDragImage = true;
        }
    }

    private void TextBox_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data is not null
            && e.Data.GetDataPresent(nameof(_nyanCatAscii), false)
            && e.Data.GetData(nameof(_nyanCatAscii)) is string asciiCat)
        {
            textBox.Text += textBox.Text.Length > 0
                ? Environment.NewLine + Environment.NewLine + asciiCat
                : asciiCat;
        }

        textBox.SelectionStart = textBox.Text.Length;
        textBox.SelectionLength = 0;
    }

    private void TextBox_DragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.None;

        if (e.Data is null)
        {
            return;
        }

        if (e.Data.GetDataPresent(nameof(_nyanCatAscii)))
        {
            // Set the target drop image to a plus sign (+).
            e.DropImageType = DropImageType.Copy;

            // Set the target drop text.
            e.Message = "Copy cat to %1";
            e.MessageReplacementToken = "~=[,,_,,]:3";

            // Set the target drop effect.
            e.Effect = DragDropEffects.Copy;
        }
        else if (e.Data.GetDataPresent(nameof(_nyanCatBmp)))
        {
            e.DropImageType = DropImageType.None;
            e.Message = "Nyan %1";
            e.MessageReplacementToken = "Cat";
            e.Effect = DragDropEffects.None;
        }
        else if (e.Data.GetDataPresent(DataFormats.FileDrop)
            && e.Data.GetData(DataFormats.FileDrop) is string[] files
            && files.Length > 0 && files[0].Contains("DragAccept.rtf"))
        {
            // Set the target drop image to a red bisected circle.
            e.DropImageType = DropImageType.None;

            // Set the target drop text.
            e.Message = $"{Path.GetFileNameWithoutExtension(files[0])}%1";
            e.MessageReplacementToken = Path.GetExtension(files[0]);

            // Set the target drop effect.
            e.Effect = DragDropEffects.None;
        }
    }

    private void TextBox_DragOver(object? sender, DragEventArgs e)
    {
        if (e.Data is null)
        {
            return;
        }

        if (e.Data.GetDataPresent(nameof(_nyanCatAscii)))
        {
            // Set the target drop image to a plus sign (+).
            e.DropImageType = DropImageType.Copy;

            // Set the target drop text.
            e.Message = "Copy cat to %1";
            e.MessageReplacementToken = "~=[,,_,,]:3";

            // Set the target drop effect.
            e.Effect = DragDropEffects.Copy;
        }
        else if (e.Data.GetDataPresent(nameof(_nyanCatBmp)))
        {
            e.DropImageType = DropImageType.None;
            e.Message = "Nyan %1";
            e.MessageReplacementToken = "Cat";
            e.Effect = DragDropEffects.None;
        }
        else if (e.Data.GetDataPresent(DataFormats.FileDrop)
            && e.Data.GetData(DataFormats.FileDrop) is string[] files
            && files.Length > 0 && files[0].Contains("DragAccept.rtf"))
        {
            // Set the target drop image to a red bisected circle.
            e.DropImageType = DropImageType.None;

            // Set the target drop text.
            e.Message = $"{Path.GetFileNameWithoutExtension(files[0])}%1";
            e.MessageReplacementToken = Path.GetExtension(files[0]);

            // Set the target drop effect.
            e.Effect = DragDropEffects.None;
        }
    }

    private void RichTextBox_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data is null)
        {
            return;
        }

        if (e.Data.GetDataPresent(DataFormats.FileDrop)
            && e.Data.GetDataPresent("FileName")
            && e.Data.GetData("FileName") is string[] fileNames
            && fileNames.Length > 0 && fileNames[0].Contains("DragAccept.rtf"))
        {
            e.DropImageType = DropImageType.Link;
            e.Message = "%1 (shellapi.h)";
            e.MessageReplacementToken = "DragAcceptFiles";
            e.Effect = DragDropEffects.Link;
        }
        else if (e.Data.GetDataPresent(nameof(_nyanCatAscii)))
        {
            e.DropImageType = DropImageType.None;
            e.Message = "Ascii %1";
            e.MessageReplacementToken = "Cat";
            e.Effect = DragDropEffects.None;
        }
        else if (e.Data.GetDataPresent(nameof(_nyanCatBmp)))
        {
            e.DropImageType = DropImageType.None;
            e.Message = "Nyan %1";
            e.MessageReplacementToken = "Cat";
            e.Effect = DragDropEffects.None;
        }
    }

    private void RichTextBox_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data is not null
            && e.Data.GetDataPresent(DataFormats.FileDrop)
            && e.Data.GetDataPresent("FileName")
            && e.Data.GetData("FileName") is string[] fileNames
            && fileNames.Length > 0 && fileNames[0].Contains("DragAccept.rtf"))
        {
            richTextBox.Clear();
            richTextBox.LoadFile(fileNames[0], RichTextBoxStreamType.RichText);
            e.Effect = DragDropEffects.None;
        }
    }

    private void LoadCat(PictureBox pictureBox, Bitmap image)
    {
        pictureBox.Image = image;
    }

    private void LoadCats(PictureBox pictureBox, string[] bitmapFiles)
    {
        if (bitmapFiles.Length == 1)
        {
            LoadCat(pictureBox, new Bitmap(bitmapFiles[0]));
        }
        else
        {
            for (int i = 0; i < bitmapFiles.Length; i++)
            {
                if (_pictureBoxList[i] is not null)
                {
                    LoadCat(_pictureBoxList[i], new Bitmap(bitmapFiles[0]));
                }
            }
        }
    }

    private void ClearCats()
        => _pictureBoxList.ForEach(pb => pb.Image = null);

    private void OpenCats()
    {
        string dragDropDataDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            DragDropDataDirectory);

        if (Directory.Exists(dragDropDataDirectory))
        {
            ProcessStartInfo startInfo = new()
            {
                Arguments = dragDropDataDirectory,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }
    }

    private string ReadAsciiText()
    {
        string nyanCatAsciiPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            DragDropDataDirectory,
            NyanCatAsciiTxt);

        try
        {
            return File.ReadAllText(nyanCatAsciiPath);
        }
        catch
        {
            return string.Empty;
        }
    }

    private void CreateCatToolStrip()
    {
        TableLayoutPanel tableLayoutPanel = new()
        {
            ColumnCount = 1,
            Dock = DockStyle.Top,
            Height = 35,
            RowCount = 1
        };

        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _catContextMenuStrip = new ContextMenuStrip
        {
            AllowDrop = true,
            AutoSize = true,
            ImageScalingSize = new Size(75, 75)
        };

        _catContextMenuStrip.Opening += ContextMenuStrip_Opening;

        ToolStrip toolStrip = new()
        {
            Dock = DockStyle.Right,
            GripStyle = ToolStripGripStyle.Hidden
        };

        ToolStripDropDownButton catToolStripDropDownButton = new()
        {
            AutoSize = false,
            DropDown = _catContextMenuStrip,
            Image = _toolStripNyanCatBmp,
            Height = 35,
            Name = "catToolStripDropDownButton",
            Text = "Cats",
            Width = 100
        };

        toolStrip.Items.Add(catToolStripDropDownButton);
        tableLayoutPanel.Controls.Add(toolStrip, 0, 0);
        Controls.Add(tableLayoutPanel);
        ContextMenuStrip = _catContextMenuStrip;
    }

    private void ContextMenuStrip_Opening(object? sender, CancelEventArgs e)
    {
        if (_catContextMenuStrip is null)
        {
            return;
        }

        _catContextMenuStrip.Items.Clear();

        ToolStripItem dragAcceptItem = new ToolStripMenuItem()
        {
            AllowDrop = true,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Text = "DragAcceptFiles",
            Name = "DragAcceptItem",
            Image = _toolStripDragAcceptBmp
        };
        dragAcceptItem.DragEnter += DragAcceptItem_DragEnter;
        dragAcceptItem.MouseDown += DragAcceptItem_MouseDown;

        ToolStripItem nyanCatItem = new ToolStripMenuItem()
        {
            AllowDrop = true,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Text = "Nyan Cat",
            Name = "NyanCatItem",
            Image = _toolStripNyanCatBmp
        };
        nyanCatItem.DragEnter += NyanCatItem_DragEnter;
        nyanCatItem.MouseDown += NyanCatItem_MouseDown;

        ToolStripItem asciiCatItem = new ToolStripMenuItem()
        {
            AllowDrop = true,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Text = "Ascii Cat",
            Name = "AsciiCatItem",
            Image = _toolStripAsciiCatBmp
        };
        asciiCatItem.DragEnter += AsciiCatItem_DragEnter;
        asciiCatItem.MouseDown += AsciiCatItem_MouseDown;

        _catContextMenuStrip.Items.Add(dragAcceptItem);
        _catContextMenuStrip.Items.Add(nyanCatItem);
        _catContextMenuStrip.Items.Add(asciiCatItem);

        e.Cancel = false;
    }

    private void DragAcceptItem_DragEnter(object? sender, DragEventArgs e)
    {
        e.DropImageType = DropImageType.Link;
        e.Message = "DragAcceptFiles";
        e.Effect = DragDropEffects.Link;
    }

    private void NyanCatItem_DragEnter(object? sender, DragEventArgs e)
    {
        e.DropImageType = DropImageType.Link;
        e.Message = "NyanCat";
        e.Effect = DragDropEffects.Link;
    }

    private void AsciiCatItem_DragEnter(object? sender, DragEventArgs e)
    {
        e.DropImageType = DropImageType.Link;
        e.Message = "AsciiCat";
        e.Effect = DragDropEffects.Link;
    }

    private void DragAcceptItem_MouseDown(object? sender, MouseEventArgs e)
    {
        if (sender is ToolStripItem toolStripItem)
        {
            // Create a DragAccept.rtf FileDrop data object.
            DataObject data = new(DataFormats.FileDrop,
                new string[]
                {
                    Path.Combine(Directory.GetCurrentDirectory(),
                        DragDropDataDirectory,
                        DragAcceptRtf)
                });

            // Call DoDragDrop and set the initial drag image.
            toolStripItem.DoDragDrop(data, DragDropEffects.All, _dragAcceptBmp, new Point(0, 96), true);
        }
    }

    private void NyanCatItem_MouseDown(object? sender, MouseEventArgs e)
    {
        if (sender is ToolStripItem toolStripItem)
        {
            // Create a nyan cat bitmap data object.
            DataObject data = new(nameof(_nyanCatBmp), _nyanCatBmp);

            // Call DoDragDrop and set the initial drag image.
            toolStripItem.DoDragDrop(data, DragDropEffects.All, _nyanCatBmp, new Point(0, 96), true);
        }
    }

    private void AsciiCatItem_MouseDown(object? sender, MouseEventArgs e)
    {
        if (sender is ToolStripItem toolStripItem)
        {
            // Create a string ascii cat data object.
            DataObject data = new(nameof(_nyanCatAscii), _nyanCatAscii);

            // Call DoDragDrop and set the initial drag image.
            toolStripItem.DoDragDrop(data, DragDropEffects.All, _nyanCatAscii301Bmp, new Point(0, 111), false);
        }
    }
}
