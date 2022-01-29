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
        private readonly string _dragDropDataDirectory = "Data\\DragDrop";
        private readonly List<PictureBox> _pictureBoxList;

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

            pictureBox2.AllowDrop = true;
            pictureBox2.DragEnter += PictureBox_DragEnter;
            pictureBox2.DragDrop += PictureBox_DragDrop;

            pictureBox3.AllowDrop = true;
            pictureBox3.DragEnter += PictureBox_DragEnter;
            pictureBox3.DragDrop += PictureBox_DragDrop;

            pictureBox4.AllowDrop = true;
            pictureBox4.DragEnter += PictureBox_DragEnter;
            pictureBox4.DragDrop += PictureBox_DragDrop;

            pictureBox5.AllowDrop = true;
            pictureBox5.DragEnter += PictureBox_DragEnter;
            pictureBox5.DragDrop += PictureBox_DragDrop;

            buttonOpenCats.Click += new EventHandler(ButtonOpenCats_Click);
            buttonClearCats.Click += new EventHandler(ButtonClearCats_Click);
        }

        private void ButtonClearCats_Click(object? sender, EventArgs e)
            => ClearCats();

        private void ButtonOpenCats_Click(object? sender, EventArgs e)
            => OpenCats();

        private void DragDrop_DragEnter(object? sender, DragEventArgs e)
            => e.Effect = e.AllowedEffect;

        private void PictureBox_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            if (sender is PictureBox pb
                && e.Data is not null
                && e.Data.GetDataPresent(DataFormats.FileDrop)
                && e.Data.GetData(DataFormats.FileDrop) is string[] files
                && files.Length > 0 && files.Length <= 5)
            {
                if (files.All(file => file.Contains("NyanCat") && file.EndsWith(".bmp")))
                {
                    // Set the drop icon to a plus sign (+).
                    e.DropIcon = DropIconType.Copy;

                    // Set the drop description text.
                    e.Message = $"{e.DropIcon} %1 from Explorer";
                    e.Insert = $"{(files.Length > 1 ? "cats" : "Cat")}";

                    e.Effect = DragDropEffects.Copy;
                }
                else if (files.Length == 1 && files.Any(file => file.Contains("DragAccept") && file.EndsWith(".rtf")))
                {
                    // Set the drop icon to a red bisected circle.
                    e.DropIcon = DropIconType.None;

                    // Set the drop description text.
                    e.Message = $"{Path.GetFileNameWithoutExtension(files[0])}%1";
                    e.Insert = Path.GetExtension(files[0]);

                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void PictureBox_DragDrop(object? sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            if (sender is PictureBox pb
                && e.Data is not null
                && e.Data.GetDataPresent(DataFormats.FileDrop)
                && e.Data.GetData(DataFormats.FileDrop) is string[] files
                && files.Length > 0 && files.Length <= 5
                && files.All(file => file.Contains("NyanCat") && file.EndsWith(".bmp")))
            {
                LoadCats(pb, files);
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
