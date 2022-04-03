// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class DragDrop : Form
    {
        private readonly string _nyanCatAscii =
            ".,__,.........,__,.....╭¬¬¬¬¬━━╮" + Environment.NewLine +
            "`•.,¸,.•*¯`•.,¸,.•*|:¬¬¬¬¬¬::::|:^--------^ " + Environment.NewLine +
            "`•.,¸,.•*¯`•.,¸,.•*|:¬¬¬¬¬¬::::||｡◕‿‿◕｡| " + Environment.NewLine +
            "-........--\"\"-.......--\"╰O━━━━O╯╰--O-O--╯";
        private readonly string _dragDropDataDirectory = "Data\\DragDrop";
        private readonly List<PictureBox> _pictureBoxList;
        private readonly Bitmap _nyanCatAscii301Bmp = new(@"Data\DragDrop\NyanCatAscii_301.bmp");
        private readonly Bitmap _nyanCatBmp = new(@"Data\DragDrop\NyanCat1.bmp");
        private readonly Bitmap _toolStripAsciiCatBmp = new(@"Data\DragDrop\ToolStripAsciiCat.bmp");
        private readonly Bitmap _toolStripNyanCatBmp = new(@"Data\DragDrop\ToolStripNyanCat.bmp");
        private ContextMenuStrip? _catContextMenuStrip;

        public DragDrop()
        {
            InitializeComponent();

            _pictureBoxList = new()
            {
                pictureBox1,
                pictureBox2,
                pictureBox3,
                pictureBox4,
                pictureBox5
            };

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
            textBox.DragEnter += TextBox1_DragEnter;
            textBox.DragOver += TextBox1_DragOver;
            textBox.DragDrop += TextBox1_DragDrop;

            richTextBox.AllowDrop = true;
            richTextBox.EnableAutoDragDrop = true;
            richTextBox.DragEnter += RichTextBox_DragEnter;
            richTextBox.DragOver += RichTextBox_DragOver;

            buttonOpenCats.Click += new EventHandler(ButtonOpenCats_Click);
            buttonClear.Click += new EventHandler(ButtonClear_Click);

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
            e.DropIcon = DropIconType.Warning;
            e.Effect = DragDropEffects.None;
        }

        private void PictureBox_DragEnter(object? sender, DragEventArgs e)
        {
            if (sender is PictureBox pb
                && e.Data is not null
                && e.Data.GetDataPresent(DataFormats.FileDrop)
                && e.Data.GetData(DataFormats.FileDrop) is string[] files
                && files.Length > 0 && files.Length <= 5)
            {
                if (files.All(file => file.Contains("NyanCat") && file.EndsWith(".bmp")))
                {
                    // Set the target drop icon to a plus sign (+).
                    e.DropIcon = DropIconType.Copy;

                    // Set the target drop text.
                    e.Message = $"{e.DropIcon} %1 from Explorer";
                    e.Insert = $"{(files.Length > 1 ? "Cats" : "Cat")}";

                    // Set the target drop effect.
                    e.Effect = DragDropEffects.Copy;
                }
                else if (files.Length == 1 && files.Any(file => file.Contains("DragAccept") && file.EndsWith(".rtf")))
                {
                    // Set the target drop icon to a red bisected circle.
                    e.DropIcon = DropIconType.None;

                    // Set the target drop text.
                    e.Message = $"{Path.GetFileNameWithoutExtension(files[0])}%1";
                    e.Insert = Path.GetExtension(files[0]);

                    // Set the target drop effect.
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void PictureBox_DragDrop(object? sender, DragEventArgs e)
        {
            if (sender is PictureBox pb
                && e.Data is not null
                && e.Data.GetDataPresent(DataFormats.FileDrop)
                && e.Data.GetData(DataFormats.FileDrop) is string[] files
                && files.Length > 0 && files.Length <= 5
                && files.All(file => file.Contains("NyanCat") && file.EndsWith(".bmp")))
            {
                LoadCats(pb, files);
            }
        }

        private void PictureBox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is PictureBox pb && pb is not null)
            {
                // Create the ascii cat data object.
                DataObject data = new(nameof(_nyanCatAscii), _nyanCatAscii);

                // Call DoDragDrop and set the initial drag image.
                // Set useDefaultDragImage to true to specify a layered drag image window with size 96x96.
                pb.DoDragDrop(data, DragDropEffects.All, _nyanCatBmp, new Point(0, 96), true);
            }
        }

        private void PictureBox_GiveFeedback(object? sender, GiveFeedbackEventArgs e)
        {
            // Hide the default cursor.
            Cursor.Current = Cursors.Default;
            e.UseDefaultCursors = false;

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

        private void TextBox1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e is not null
                && e.Data is not null
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

        private void TextBox1_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            if (e.Data is not null && e.Data.GetDataPresent(nameof(_nyanCatAscii)))
            {
                // Set the target drop icon to a plus sign (+).
                e.DropIcon = DropIconType.Copy;

                // Set the target drop text.
                e.Message = "Copy cat to %1";
                e.Insert = "~=[,,_,,]:3";

                // Set the target drop effect.
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void TextBox1_DragOver(object? sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            if (e.Data is not null && e.Data.GetDataPresent(nameof(_nyanCatAscii)))
            {
                // Set the target drop icon to a plus sign (+).
                e.DropIcon = DropIconType.Copy;

                // Set the target drop text.
                e.Message = "Copy cat to %1";
                e.Insert = "~=[,,_,,]:3";

                // Set the target drop effect.
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void RichTextBox_DragEnter(object? sender, DragEventArgs e)
        {
            e.DropIcon = DropIconType.Copy;
            e.Message = "RichTextBox.DragEnter";
            e.Effect = DragDropEffects.Copy;
        }

        private void RichTextBox_DragOver(object? sender, DragEventArgs e)
        {
            e.DropIcon = DropIconType.Copy;
            e.Message = "RichTextBox.DragOver";
            e.Effect = DragDropEffects.Copy;
        }

        private void LoadCats(PictureBox pb, string[] bitmaps)
        {
            ClearCats();

            if (bitmaps.Length == 1)
            {
                pb.Image = new Bitmap(bitmaps[0]);
            }
            else
            {
                for (int i = 0; i < bitmaps.Length; i++)
                {
                    _pictureBoxList[i].Image = new Bitmap(bitmaps[i]);
                }
            }
        }

        private void ClearCats()
            => _pictureBoxList.ForEach(pb => pb.Image = null);

        private void OpenCats()
        {
            string dragDropDataDirectory = Path.Combine(
                Directory.GetCurrentDirectory(),
                _dragDropDataDirectory);

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

        private void CreateCatToolStrip()
        {
            TableLayoutPanel tableLayoutPanel = new()
            {
                ColumnCount = 1,
                Dock = DockStyle.Top,
                Height = 30,
                RowCount = 1
            };

            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _catContextMenuStrip = new ContextMenuStrip
            {
                AllowDrop = true,
                AutoSize = true,
                ImageScalingSize = new Size(75, 75)
            };

            _catContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);

            ToolStrip toolStrip = new()
            {
                Dock = DockStyle.Top,
            };

            ToolStripDropDownButton catToolStripDropDownButton = new("Cats", null, null, "Cats")
            {
                DropDown = _catContextMenuStrip,
            };

            toolStrip.Items.Add(catToolStripDropDownButton);
            tableLayoutPanel.Controls.Add(toolStrip, 0, 0);
            Controls.Add(tableLayoutPanel);
            ContextMenuStrip = _catContextMenuStrip;
        }

        void ContextMenuStrip_Opening(object? sender, CancelEventArgs e)
        {
            if (_catContextMenuStrip is null)
            {
                return;
            }

            _catContextMenuStrip.Items.Clear();
            Control control = _catContextMenuStrip.SourceControl;

            if (control is not null)
            {
                _catContextMenuStrip.Items.Add("Source: " + control.GetType().ToString());
            }
            else if (_catContextMenuStrip.OwnerItem is ToolStripDropDownItem toolStripDropDownItem)
            {
                _catContextMenuStrip.Items.Add("Source: " + toolStripDropDownItem.GetType().ToString());
            }

            ToolStripItem asciiCatItem = new ToolStripMenuItem()
            {
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Text = "Ascii Cat",
                Name = "AsciiCatItem",
                Image = _toolStripAsciiCatBmp
            };

            ToolStripItem nyanCatItem = new ToolStripMenuItem()
            {
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Text = "Nyan Cat",
                Name = "NyanCatItem",
                Image = _toolStripNyanCatBmp
            };

            asciiCatItem.AllowDrop = true;
            asciiCatItem.DragEnter += AsciiCatItem_DragEnter;
            asciiCatItem.MouseDown += AsciiCatItem_MouseDown;
            asciiCatItem.GiveFeedback += AsciiCatItem_GiveFeedback;

            nyanCatItem.AllowDrop = true;
            nyanCatItem.DragEnter += NyanCatItem_DragEnter;
            nyanCatItem.MouseDown += NyanCatItem_MouseDown;
            nyanCatItem.GiveFeedback += NyanCatItem_GiveFeedback;

            _catContextMenuStrip.Items.Add("-");
            _catContextMenuStrip.Items.Add(asciiCatItem);
            _catContextMenuStrip.Items.Add(nyanCatItem);

            e.Cancel = false;
        }

        private void NyanCatItem_DragEnter(object? sender, DragEventArgs e)
        {
            e.DropIcon = DropIconType.Copy;
            e.Message = "NyanCat";
            e.Effect = DragDropEffects.Copy;
        }

        private void AsciiCatItem_DragEnter(object? sender, DragEventArgs e)
        {
            e.DropIcon = DropIconType.Copy;
            e.Message = "AsciiCat";
            e.Effect = DragDropEffects.Copy;
        }

        private void NyanCatItem_GiveFeedback(object? sender, GiveFeedbackEventArgs e)
        {
            // Hide the default cursor.
            Cursor.Current = Cursors.Default;
            e.UseDefaultCursors = false;
        }

        private void AsciiCatItem_GiveFeedback(object? sender, GiveFeedbackEventArgs e)
        {
            // Hide the default cursor.
            Cursor.Current = Cursors.Default;
            e.UseDefaultCursors = false;
        }

        private void NyanCatItem_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripItem toolStripItem)
            {
                // Create the ascii cat data object.
                DataObject data = new(nameof(_nyanCatAscii), _nyanCatAscii);

                // Call DoDragDrop and set the initial drag image.
                toolStripItem.DoDragDrop(data, DragDropEffects.Copy, _nyanCatBmp, new Point(0, 96), true);
            }
        }

        private void AsciiCatItem_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripItem toolStripItem)
            {
                // Create the ascii cat data object.
                DataObject data = new(nameof(_nyanCatAscii), _nyanCatAscii);

                // Call DoDragDrop and set the initial drag image.
                toolStripItem.DoDragDrop(data, DragDropEffects.Copy, _nyanCatAscii301Bmp, new Point(0, 111), false);
            }
        }
    }
}
