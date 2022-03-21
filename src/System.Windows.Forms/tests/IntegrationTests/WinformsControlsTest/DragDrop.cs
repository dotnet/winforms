// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

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

        public DragDrop()
        {
            InitializeComponent();

            richTextBox1.Visible = false;

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

            textBox1.AllowDrop = true;
            textBox1.DragEnter += TextBox1_DragEnter;
            textBox1.DragDrop += TextBox1_DragDrop;

            buttonOpenCats.Click += new EventHandler(ButtonOpenCats_Click);
            buttonClear.Click += new EventHandler(ButtonClear_Click);
        }

        private void ButtonClear_Click(object? sender, EventArgs e)
        {
            ClearCats();
            richTextBox1.Clear();
            textBox1.Clear();
        }

        private void ButtonOpenCats_Click(object? sender, EventArgs e)
            => OpenCats();

        private void DragDrop_DragEnter(object? sender, DragEventArgs e)
        {
            e.DropIcon = DropIconType.NoDropIcon;
            e.Effect = DragDropEffects.All;
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
                // Create the cat ascii data object.
                DataObject data = new(nameof(_nyanCatAscii), _nyanCatAscii);

                // Note the outer edges of the drag image are blended out if the width or height exceeds 300 pixels.
                Bitmap dragImage = (Bitmap)Image.FromFile(@"Data\DragDrop\NyanCatAscii_301.bmp");

                // Call DoDragDrop, specifying the drag image bitmap, along with the cursor offset.
                pb.DoDragDrop(data, DragDropEffects.All, dragImage, new Point(0, 111));
            }
        }

        private void PictureBox_GiveFeedback(object? sender, GiveFeedbackEventArgs e)
        {
            Cursor.Current = Cursors.Default;

            // Hide the default cursor.
            e.UseDefaultCursors = false;
        }

        private void TextBox1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e is not null
                && e.Data is not null
                && e.Data.GetDataPresent(nameof(_nyanCatAscii), false)
                && e.Data.GetData(nameof(_nyanCatAscii)) is string asciiCat)
            {
                textBox1.Text += textBox1.Text.Length > 0
                    ? Environment.NewLine + Environment.NewLine + asciiCat
                    : asciiCat;
            }

            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.SelectionLength = 0;
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
    }
}
