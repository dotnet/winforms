// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;
using static System.Windows.Forms.ImageList;
using static System.Windows.Forms.ListViewItem;
using static System.Windows.Forms.TableLayoutSettings;
using static Interop;

namespace System.Windows.Forms.Tests.Serialization
{
    // NB: doesn't require thread affinity
    public class SerializableTypesTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void AxHostState_RoundTripAndExchangeWithNet()
        {
            string payload = "abc";
            string licenseKey = "licenseKey";
            string netBlob;
            AxHost.State state;

            using (var stream = new MemoryStream(256))
            {
                var bytes = Encoding.UTF8.GetBytes(payload);
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(bytes.Length);
                    writer.Write(bytes);
                    stream.Seek(0, SeekOrigin.Begin);

                    state = new AxHost.State(stream, 1, true, licenseKey);
                    netBlob = BinarySerialization.ToBase64String(state);
                }
            }

            // ensure we can deserialise NET serialised data and continue to match the payload
            ValidateResult(netBlob);
            // ensure we can deserialise NET Fx serialised data and continue to match the payload
            ValidateResult(ClassicAxHostState);

            void ValidateResult(string blob)
            {
                AxHost.State result = BinarySerialization.EnsureDeserialize<AxHost.State>(blob);

                Assert.Null(result.GetPropBag());
                Assert.Null(state.GetPropBag());

                Assert.Equal(1, result.Type);
                Assert.Equal(1, state.Type);

                Assert.True(result._GetManualUpdate());
                Assert.True(state._GetManualUpdate());

                Assert.Equal(licenseKey, result._GetLicenseKey());
                Assert.Equal(licenseKey, state._GetLicenseKey());

                var streamOut = result.GetStream() as Ole32.GPStream;
                Assert.NotNull(streamOut);
                Stream bufferStream = streamOut.GetDataStream();
                byte[] buffer = new byte[3];
                bufferStream.Read(buffer, 0, buffer.Length);
                Assert.Equal(payload, Encoding.UTF8.GetString(buffer));
            }
        }

        [Fact]
        public void ImageListStreamer_RoundTripAndExchangeWithNet()
        {
            string netBlob;

            using (var imageList = new ImageList()
            {
                ImageSize = new Size(16, 16),
                TransparentColor = Color.White
            })
            {
                imageList.Images.Add(new Bitmap(16, 16));
                netBlob = BinarySerialization.ToBase64String(imageList.ImageStream);
            }

            // ensure we can deserialise NET serialised data and continue to match the payload
            ValidateResult(netBlob);
            // ensure we can deserialise NET Fx serialised data and continue to match the payload
            ValidateResult(ClassicImageListStreamer);

            void ValidateResult(string blob)
            {
                using ImageListStreamer result = BinarySerialization.EnsureDeserialize<ImageListStreamer>(blob);
                using (NativeImageList nativeImageList = result.GetNativeImageList())
                {
                    Assert.True(ComCtl32.ImageList.GetIconSize(new HandleRef(this, nativeImageList.Handle), out int x, out int y).IsTrue());
                    Assert.Equal(16, x);
                    Assert.Equal(16, y);
                    var imageInfo = new ComCtl32.IMAGEINFO();
                    Assert.True(ComCtl32.ImageList.GetImageInfo(new HandleRef(this, nativeImageList.Handle), 0, ref imageInfo).IsTrue());
                    Assert.False(imageInfo.hbmImage.IsNull);
                }
            }
        }

        [Fact]
        public void LinkArea_RoundTripAndExchangeWithNet()
        {
            var linkArea = new LinkArea(5, 7);
            var netBlob = BinarySerialization.ToBase64String(linkArea);

            // ensure we can deserialise NET serialised data and continue to match the payload
            ValidateResult(netBlob);
            // ensure we can deserialise NET Fx serialised data and continue to match the payload
            ValidateResult(ClassicLinkArea);

            void ValidateResult(string blob)
            {
                LinkArea result = BinarySerialization.EnsureDeserialize<LinkArea>(blob);

                Assert.Equal(linkArea, result);
            }
        }

        [Fact]
        public void ListViewGroup_RoundTripAndExchangeWithNet()
        {
            var listViewGroup = new ListViewGroup("Header", HorizontalAlignment.Center)
            {
                Tag = "Tag",
                Name = "GroupName",
            };
            listViewGroup.Items.Add(new ListViewItem("Item"));

            var netBlob = BinarySerialization.ToBase64String(listViewGroup);

            // ensure we can deserialise NET serialised data and continue to match the payload
            ValidateResult(netBlob);
            // ensure we can deserialise NET Fx serialised data and continue to match the payload
            ValidateResult(ClassicListViewGroup);

            void ValidateResult(string blob)
            {
                ListViewGroup result = BinarySerialization.EnsureDeserialize<ListViewGroup>(blob);

                Assert.Equal("Header", result.Header);
                Assert.Equal(HorizontalAlignment.Center, result.HeaderAlignment);
                Assert.Equal("Tag", result.Tag);
                Assert.Equal("GroupName", result.Name);
                var item = Assert.Single(result.Items) as ListViewItem;
                Assert.NotNull(item);
                Assert.Equal("Item", item.Text);
            }
        }

        [Fact]
        public void ListViewItem_RoundTripAndExchangeWithNet()
        {
            string netBlob;

            using (var font = new Font(FontFamily.GenericSansSerif, 9f))
            {
                var listViewItem = new ListViewItem("Item1", 0)
                {
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    Font = font,
                    Checked = true
                };

                netBlob = BinarySerialization.ToBase64String(listViewItem);
            }

            // ensure we can deserialise NET serialised data and continue to match the payload
            ValidateResult(netBlob);
            // ensure we can deserialise NET Fx serialised data and continue to match the payload
            ValidateResult(ClassicListViewItem);

            void ValidateResult(string blob)
            {
                ListViewItem result = BinarySerialization.EnsureDeserialize<ListViewItem>(blob);

                Assert.Equal("Item1", result.Text);
                Assert.Equal(0, result.ImageIndex);
                Assert.Empty(result.ImageKey);
                var item = Assert.Single(result.SubItems) as ListViewSubItem;
                Assert.Equal("Item1", item.Text);
                Assert.Equal(Color.Black, result.BackColor);
                Assert.Equal(Color.White, result.ForeColor);
                Assert.True(result.Checked);
                Assert.Equal(FontFamily.GenericSansSerif.Name, result.Font.FontFamily.Name);
                Assert.True(result.UseItemStyleForSubItems);
                Assert.Null(result.Group);

                Assert.Null(result.Tag);
                Assert.Null(result.ImageList);
                Assert.Equal(-1, result.Index);
                Assert.Equal(0, result.IndentCount);
            }
        }

        [Fact]
        public void ListViewSubItemAndSubItemStyle_RoundTripAndExchangeWithNet()
        {
            string netBlob;

            using (var font = new Font(FontFamily.GenericSansSerif, 9f))
            {
                var listViewSubItem = new ListViewSubItem(
                    new ListViewItem(),
                    "SubItem1",
                    foreColor: Color.White,
                    backColor: Color.Black,
                    font)
                {
                    Tag = "UserData"
                };

                netBlob = BinarySerialization.ToBase64String(listViewSubItem);
            }

            // ensure we can deserialise NET serialised data and continue to match the payload
            ValidateResult(netBlob);
            // ensure we can deserialise NET Fx serialised data and continue to match the payload
            ValidateResult(ClassicListViewSubItem);

            void ValidateResult(string blob)
            {
                ListViewSubItem result = BinarySerialization.EnsureDeserialize<ListViewSubItem>(blob);

                Assert.Equal("SubItem1", result.Text);
                Assert.True(result.CustomStyle);
                Assert.True(result.CustomForeColor);
                Assert.True(result.CustomBackColor);
                Assert.True(result.CustomFont);
                Assert.Equal(Color.White, result.ForeColor);
                Assert.Equal(Color.Black, result.BackColor);
                Assert.Equal(FontFamily.GenericSansSerif.Name, result.Font.FontFamily.Name);
                Assert.Null(result.Tag); // UserData is wiped on deserialization
            }
        }

        [Fact]
        public void Padding_RoundTripAndExchangeWithNet()
        {
            var padding = new Padding(1, 2, 3, 4);
            var netBlob = BinarySerialization.ToBase64String(padding);

            // ensure we can deserialise NET serialised data and continue to match the payload
            ValidateResult(netBlob);
            // ensure we can deserialise NET Fx serialised data and continue to match the payload
            ValidateResult(ClassicPadding);

            void ValidateResult(string blob)
            {
                Padding result = BinarySerialization.EnsureDeserialize<Padding>(blob);

                Assert.Equal(padding, result);
            }
        }

        [Fact]
        public void TableLayoutSettings_RoundTripAndExchangeWithNet()
        {
            string netBlob;

            using (var tableLayoutPanel = new TableLayoutPanel
            {
                Name = "table",
                CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble,
                ColumnCount = 3,
                RowCount = 2
            })
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

                var button00_10 = new Button() { Name = "button00_10" };
                tableLayoutPanel.Controls.Add(button00_10, 0, 0);
                tableLayoutPanel.SetColumnSpan(button00_10, 2);
                var label20_21 = new Label() { Name = "label20_21" };
                tableLayoutPanel.Controls.Add(label20_21, 2, 0);
                tableLayoutPanel.SetRowSpan(label20_21, 2);
                tableLayoutPanel.Controls.Add(new RadioButton() { Name = "radioButton01" }, 0, 1);
                tableLayoutPanel.Controls.Add(new CheckBox() { Name = "checkBox11" }, 1, 1);

                netBlob = BinarySerialization.ToBase64String(tableLayoutPanel.LayoutSettings as TableLayoutSettings);
            }

            // ensure we can deserialise NET serialised data and continue to match the payload
            ValidateResult(netBlob);
            // ensure we can deserialise NET Fx serialised data and continue to match the payload
            ValidateResult(ClassicTableLayoutSettings);

            void ValidateResult(string blob)
            {
                TableLayoutSettings result = BinarySerialization.EnsureDeserialize<TableLayoutSettings>(blob);

                Assert.NotNull(result);
                Assert.True(result.IsStub); // This class is not associated with an owner control.
                Assert.Equal(TableLayoutPanelCellBorderStyle.None, result.CellBorderStyle); // This property is not serialized.
                Assert.NotNull(result.LayoutEngine);
                // These values will be accessible when the owner is set.
                Assert.Throws<NullReferenceException>(() => result.CellBorderWidth);
                Assert.Throws<NullReferenceException>(() => result.ColumnCount);
                Assert.Throws<NullReferenceException>(() => result.GrowStyle);
                Assert.Throws<NullReferenceException>(() => result.RowCount);
                Assert.Equal(3, result.ColumnStyles.Count);
                Assert.Equal(2, result.RowStyles.Count);
                Assert.Equal(SizeType.Percent, result.ColumnStyles[0].SizeType);
                Assert.Equal(SizeType.Percent, result.ColumnStyles[1].SizeType);
                Assert.Equal(SizeType.Percent, result.ColumnStyles[2].SizeType);
                Assert.Equal(SizeType.Absolute, result.RowStyles[0].SizeType);
                Assert.Equal(20, result.RowStyles[0].Height);
                Assert.Equal(30, result.RowStyles[1].Height);

                List<ControlInformation> controls = result.GetControlsInformation();
                ValidateControlInformation("button00_10", 0, 0, 2, 1, controls[0]);
                ValidateControlInformation("label20_21", 2, 0, 1, 2, controls[1]);
                ValidateControlInformation("radioButton01", 0, 1, 1, 1, controls[2]);
                ValidateControlInformation("checkBox11", 1, 1, 1, 1, controls[3]);

                void ValidateControlInformation(string name, int column, int row, int columnSpan, int rowSpan, ControlInformation control)
                {
                    Assert.Equal(name, control.Name);
                    Assert.Equal(column, control.Column);
                    Assert.Equal(row, control.Row);
                    Assert.Equal(columnSpan, control.ColumnSpan);
                    Assert.Equal(rowSpan, control.RowSpan);
                }
            }
        }

        [Fact]
        public void TreeNodeAndPropertyBag_RoundTripAndExchangeWithNet()
        {
            var children = new TreeNode[] { new TreeNode("node2"), new TreeNode("node3") };
            TreeNode treeNodeIn = new TreeNode("node1", 1, 2, children)
            {
                ToolTipText = "tool tip text",
                Name = "node1",
                SelectedImageKey = "key",
                Checked = true,
                BackColor = Color.Yellow, // Colors and Font are serialized into the property bag.
                ForeColor = Color.Green,
                NodeFont = new Font(FontFamily.GenericSansSerif, 9f)
            };

            var netBlob = BinarySerialization.ToBase64String(treeNodeIn);

            // ensure we can deserialise NET serialised data and continue to match the payload
            ValidateResult(netBlob);
            // ensure we can deserialise NET Fx serialised data and continue to match the payload
            ValidateResult(ClassicTreeNode);

            void ValidateResult(string blob)
            {
                TreeNode result = BinarySerialization.EnsureDeserialize<TreeNode>(blob);

                Assert.Equal("node1", result.Text);
                Assert.Equal(-1, result.ImageIndex); // No image list
                Assert.Equal("key", result.SelectedImageKey);
                Assert.Equal(2, result.childCount);
                Assert.Equal("node2", result.FirstNode.Text);
                Assert.Equal("node3", result.LastNode.Text);
                Assert.Equal("tool tip text", result.ToolTipText);
                Assert.Equal("node1", result.Name);
                Assert.True(result.Checked);

                Assert.Equal(Color.Yellow, result.BackColor);
                Assert.Equal(Color.Green, result.ForeColor);
                Assert.Equal(FontFamily.GenericSansSerif.Name, result.NodeFont.FontFamily.Name);
            }
        }

        private const string ClassicAxHostState =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAACFTeXN0ZW0uV2luZG93cy5Gb3Jtcy5BeEhvc3QrU3RhdGUBAAAABERhdGEHAgIAAAAJAwAAAA8DAAAAIgAAAAIBAAAAAQAAAAEKAAAAbGljZW5zZUtleQAAAAADAAAAYWJjCw==";

        public const string ClassicImageListStreamer =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAACZTeXN0ZW0uV2luZG93cy5Gb3Jtcy5JbWFnZUxpc3RTdHJlYW1lcgEAAAAERGF0YQcCAgAAAAkDAAAADwMAAAAqBwAAAk1TRnQBSQFMAwEBAAEIAQABCAEAARABAAEQAQAE/wEJAQAI/wFCAU0BNgEEBgABNgEEAgABKAMAAUADAAEQAwABAQEAAQgGAAEEGAABgAIAAYADAAKAAQABgAMAAYABAAGAAQACgAIAA8ABAAHAAdwBwAEAAfABygGmAQABMwUAATMBAAEzAQABMwEAAjMCAAMWAQADHAEAAyIBAAMpAQADVQEAA00BAANCAQADOQEAAYABfAH/AQACUAH/AQABkwEAAdYBAAH/AewBzAEAAcYB1gHvAQAB1gLnAQABkAGpAa0CAAH/ATMDAAFmAwABmQMAAcwCAAEzAwACMwIAATMBZgIAATMBmQIAATMBzAIAATMB/wIAAWYDAAFmATMCAAJmAgABZgGZAgABZgHMAgABZgH/AgABmQMAAZkBMwIAAZkBZgIAApkCAAGZAcwCAAGZAf8CAAHMAwABzAEzAgABzAFmAgABzAGZAgACzAIAAcwB/wIAAf8BZgIAAf8BmQIAAf8BzAEAATMB/wIAAf8BAAEzAQABMwEAAWYBAAEzAQABmQEAATMBAAHMAQABMwEAAf8BAAH/ATMCAAMzAQACMwFmAQACMwGZAQACMwHMAQACMwH/AQABMwFmAgABMwFmATMBAAEzAmYBAAEzAWYBmQEAATMBZgHMAQABMwFmAf8BAAEzAZkCAAEzAZkBMwEAATMBmQFmAQABMwKZAQABMwGZAcwBAAEzAZkB/wEAATMBzAIAATMBzAEzAQABMwHMAWYBAAEzAcwBmQEAATMCzAEAATMBzAH/AQABMwH/ATMBAAEzAf8BZgEAATMB/wGZAQABMwH/AcwBAAEzAv8BAAFmAwABZgEAATMBAAFmAQABZgEAAWYBAAGZAQABZgEAAcwBAAFmAQAB/wEAAWYBMwIAAWYCMwEAAWYBMwFmAQABZgEzAZkBAAFmATMBzAEAAWYBMwH/AQACZgIAAmYBMwEAA2YBAAJmAZkBAAJmAcwBAAFmAZkCAAFmAZkBMwEAAWYBmQFmAQABZgKZAQABZgGZAcwBAAFmAZkB/wEAAWYBzAIAAWYBzAEzAQABZgHMAZkBAAFmAswBAAFmAcwB/wEAAWYB/wIAAWYB/wEzAQABZgH/AZkBAAFmAf8BzAEAAcwBAAH/AQAB/wEAAcwBAAKZAgABmQEzAZkBAAGZAQABmQEAAZkBAAHMAQABmQMAAZkCMwEAAZkBAAFmAQABmQEzAcwBAAGZAQAB/wEAAZkBZgIAAZkBZgEzAQABmQEzAWYBAAGZAWYBmQEAAZkBZgHMAQABmQEzAf8BAAKZATMBAAKZAWYBAAOZAQACmQHMAQACmQH/AQABmQHMAgABmQHMATMBAAFmAcwBZgEAAZkBzAGZAQABmQLMAQABmQHMAf8BAAGZAf8CAAGZAf8BMwEAAZkBzAFmAQABmQH/AZkBAAGZAf8BzAEAAZkC/wEAAcwDAAGZAQABMwEAAcwBAAFmAQABzAEAAZkBAAHMAQABzAEAAZkBMwIAAcwCMwEAAcwBMwFmAQABzAEzAZkBAAHMATMBzAEAAcwBMwH/AQABzAFmAgABzAFmATMBAAGZAmYBAAHMAWYBmQEAAcwBZgHMAQABmQFmAf8BAAHMAZkCAAHMAZkBMwEAAcwBmQFmAQABzAKZAQABzAGZAcwBAAHMAZkB/wEAAswCAALMATMBAALMAWYBAALMAZkBAAPMAQACzAH/AQABzAH/AgABzAH/ATMBAAGZAf8BZgEAAcwB/wGZAQABzAH/AcwBAAHMAv8BAAHMAQABMwEAAf8BAAFmAQAB/wEAAZkBAAHMATMCAAH/AjMBAAH/ATMBZgEAAf8BMwGZAQAB/wEzAcwBAAH/ATMB/wEAAf8BZgIAAf8BZgEzAQABzAJmAQAB/wFmAZkBAAH/AWYBzAEAAcwBZgH/AQAB/wGZAgAB/wGZATMBAAH/AZkBZgEAAf8CmQEAAf8BmQHMAQAB/wGZAf8BAAH/AcwCAAH/AcwBMwEAAf8BzAFmAQAB/wHMAZkBAAH/AswBAAH/AcwB/wEAAv8BMwEAAcwB/wFmAQAC/wGZAQAC/wHMAQACZgH/AQABZgH/AWYBAAFmAv8BAAH/AmYBAAH/AWYB/wEAAv8BZgEAASEBAAGlAQADXwEAA3cBAAOGAQADlgEAA8sBAAOyAQAD1wEAA90BAAPjAQAD6gEAA/EBAAP4AQAB8AH7Af8BAAGkAqABAAOAAwAB/wIAAf8DAAL/AQAB/wMAAf8BAAH/AQAC/wIAA///AP8A/wD/AAUAAUIBTQE+BwABPgMAASgDAAFAAwABEAMAAQEBAAEBBQABgBcAA/8BAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAL";

        private const string ClassicLinkArea =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAAB1TeXN0ZW0uV2luZG93cy5Gb3Jtcy5MaW5rQXJlYQIAAAAFc3RhcnQGbGVuZ3RoAAAICAIAAAAFAAAABwAAAAs=";

        private const string ClassicListViewGroup =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAACJTeXN0ZW0uV2luZG93cy5Gb3Jtcy5MaXN0Vmlld0dyb3VwBgAAAAZIZWFkZXIPSGVhZGVyQWxpZ25tZW50A1RhZwROYW1lCkl0ZW1zQ291bnQFSXRlbTABBAEBAAQoU3lzdGVtLldpbmRvd3MuRm9ybXMuSG9yaXpvbnRhbEFsaWdubWVudAIAAAAIIVN5c3RlbS5XaW5kb3dzLkZvcm1zLkxpc3RWaWV3SXRlbQIAAAACAAAABgMAAAAGSGVhZGVyBfz///8oU3lzdGVtLldpbmRvd3MuRm9ybXMuSG9yaXpvbnRhbEFsaWdubWVudAEAAAAHdmFsdWVfXwAIAgAAAAIAAAAGBQAAAANUYWcGBgAAAAlHcm91cE5hbWUBAAAACQcAAAAMCAAAAFFTeXN0ZW0uRHJhd2luZywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWIwM2Y1ZjdmMTFkNTBhM2EFBwAAACFTeXN0ZW0uV2luZG93cy5Gb3Jtcy5MaXN0Vmlld0l0ZW0IAAAABFRleHQKSW1hZ2VJbmRleAlCYWNrQ29sb3IHQ2hlY2tlZARGb250CUZvcmVDb2xvchdVc2VJdGVtU3R5bGVGb3JTdWJJdGVtcwVHcm91cAEABAAEBAAECBRTeXN0ZW0uRHJhd2luZy5Db2xvcggAAAABE1N5c3RlbS5EcmF3aW5nLkZvbnQIAAAAFFN5c3RlbS5EcmF3aW5nLkNvbG9yCAAAAAEiU3lzdGVtLldpbmRvd3MuRm9ybXMuTGlzdFZpZXdHcm91cAIAAAACAAAABgkAAAAESXRlbf////8F9v///xRTeXN0ZW0uRHJhd2luZy5Db2xvcgQAAAAEbmFtZQV2YWx1ZQprbm93bkNvbG9yBXN0YXRlAQAAAAkHBwgAAAAKAAAAAAAAAAAYAAEAAAkLAAAAAfT////2////CgAAAAAAAAAAGgABAAEJAQAAAAULAAAAE1N5c3RlbS5EcmF3aW5nLkZvbnQEAAAABE5hbWUEU2l6ZQVTdHlsZQRVbml0AQAEBAsYU3lzdGVtLkRyYXdpbmcuRm9udFN0eWxlCAAAABtTeXN0ZW0uRHJhd2luZy5HcmFwaGljc1VuaXQIAAAACAAAAAYOAAAAFE1pY3Jvc29mdCBTYW5zIFNlcmlmAAAEQQXx////GFN5c3RlbS5EcmF3aW5nLkZvbnRTdHlsZQEAAAAHdmFsdWVfXwAICAAAAAAAAAAF8P///xtTeXN0ZW0uRHJhd2luZy5HcmFwaGljc1VuaXQBAAAAB3ZhbHVlX18ACAgAAAADAAAACw==";

        private const string ClassicListViewItem =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkMAwAAAFFTeXN0ZW0uRHJhd2luZywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWIwM2Y1ZjdmMTFkNTBhM2EFAQAAACFTeXN0ZW0uV2luZG93cy5Gb3Jtcy5MaXN0Vmlld0l0ZW0HAAAABFRleHQKSW1hZ2VJbmRleAlCYWNrQ29sb3IHQ2hlY2tlZARGb250CUZvcmVDb2xvchdVc2VJdGVtU3R5bGVGb3JTdWJJdGVtcwEABAAEBAAIFFN5c3RlbS5EcmF3aW5nLkNvbG9yAwAAAAETU3lzdGVtLkRyYXdpbmcuRm9udAMAAAAUU3lzdGVtLkRyYXdpbmcuQ29sb3IDAAAAAQIAAAAGBAAAAAVJdGVtMQAAAAAF+////xRTeXN0ZW0uRHJhd2luZy5Db2xvcgQAAAAEbmFtZQV2YWx1ZQprbm93bkNvbG9yBXN0YXRlAQAAAAkHBwMAAAAKAAAAAAAAAAAjAAEAAQkGAAAAAfn////7////CgAAAAAAAAAApAABAAEFBgAAABNTeXN0ZW0uRHJhd2luZy5Gb250BAAAAAROYW1lBFNpemUFU3R5bGUEVW5pdAEABAQLGFN5c3RlbS5EcmF3aW5nLkZvbnRTdHlsZQMAAAAbU3lzdGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0AwAAAAMAAAAGCAAAABRNaWNyb3NvZnQgU2FucyBTZXJpZgAAEEEF9////xhTeXN0ZW0uRHJhd2luZy5Gb250U3R5bGUBAAAAB3ZhbHVlX18ACAMAAAAAAAAABfb///8bU3lzdGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0AQAAAAd2YWx1ZV9fAAgDAAAAAwAAAAs=";

        private const string ClassicListViewSubItem =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAADFTeXN0ZW0uV2luZG93cy5Gb3Jtcy5MaXN0Vmlld0l0ZW0rTGlzdFZpZXdTdWJJdGVtBAAAAAR0ZXh0BG5hbWUFc3R5bGUIdXNlckRhdGEBAQQCPlN5c3RlbS5XaW5kb3dzLkZvcm1zLkxpc3RWaWV3SXRlbStMaXN0Vmlld1N1Ykl0ZW0rU3ViSXRlbVN0eWxlAgAAAAIAAAAGAwAAAAhTdWJJdGVtMQoJBAAAAAYFAAAACFVzZXJEYXRhDAYAAABRU3lzdGVtLkRyYXdpbmcsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iMDNmNWY3ZjExZDUwYTNhBQQAAAA+U3lzdGVtLldpbmRvd3MuRm9ybXMuTGlzdFZpZXdJdGVtK0xpc3RWaWV3U3ViSXRlbStTdWJJdGVtU3R5bGUDAAAACWJhY2tDb2xvcglmb3JlQ29sb3IEZm9udAQEBBRTeXN0ZW0uRHJhd2luZy5Db2xvcgYAAAAUU3lzdGVtLkRyYXdpbmcuQ29sb3IGAAAAE1N5c3RlbS5EcmF3aW5nLkZvbnQGAAAAAgAAAAX5////FFN5c3RlbS5EcmF3aW5nLkNvbG9yBAAAAARuYW1lBXZhbHVlCmtub3duQ29sb3IFc3RhdGUBAAAACQcHBgAAAAoAAAAAAAAAACMAAQAB+P////n///8KAAAAAAAAAACkAAEACQkAAAAFCQAAABNTeXN0ZW0uRHJhd2luZy5Gb250BAAAAAROYW1lBFNpemUFU3R5bGUEVW5pdAEABAQLGFN5c3RlbS5EcmF3aW5nLkZvbnRTdHlsZQYAAAAbU3lzdGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0BgAAAAYAAAAGCgAAABRNaWNyb3NvZnQgU2FucyBTZXJpZgAAEEEF9f///xhTeXN0ZW0uRHJhd2luZy5Gb250U3R5bGUBAAAAB3ZhbHVlX18ACAYAAAAAAAAABfT///8bU3lzdGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0AQAAAAd2YWx1ZV9fAAgGAAAAAwAAAAs=";

        private const string ClassicPadding =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAABxTeXN0ZW0uV2luZG93cy5Gb3Jtcy5QYWRkaW5nBQAAAARfYWxsBF90b3AFX2xlZnQGX3JpZ2h0B19ib3R0b20AAAAAAAEICAgIAgAAAAACAAAAAQAAAAMAAAAEAAAACw==";

        private const string ClassicTableLayoutSettings =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAAChTeXN0ZW0uV2luZG93cy5Gb3Jtcy5UYWJsZUxheW91dFNldHRpbmdzAQAAABBTZXJpYWxpemVkU3RyaW5nAQIAAAAGAwAAAPUDPD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTE2Ij8+PFRhYmxlTGF5b3V0U2V0dGluZ3M+PENvbnRyb2xzPjxDb250cm9sIE5hbWU9ImJ1dHRvbjAwXzEwIiBSb3c9IjAiIFJvd1NwYW49IjEiIENvbHVtbj0iMCIgQ29sdW1uU3Bhbj0iMiIgLz48Q29udHJvbCBOYW1lPSJsYWJlbDIwXzIxIiBSb3c9IjAiIFJvd1NwYW49IjIiIENvbHVtbj0iMiIgQ29sdW1uU3Bhbj0iMSIgLz48Q29udHJvbCBOYW1lPSJyYWRpb0J1dHRvbjAxIiBSb3c9IjEiIFJvd1NwYW49IjEiIENvbHVtbj0iMCIgQ29sdW1uU3Bhbj0iMSIgLz48Q29udHJvbCBOYW1lPSJjaGVja0JveDExIiBSb3c9IjEiIFJvd1NwYW49IjEiIENvbHVtbj0iMSIgQ29sdW1uU3Bhbj0iMSIgLz48L0NvbnRyb2xzPjxDb2x1bW5zIFN0eWxlcz0iUGVyY2VudCw1MCxQZXJjZW50LDUwLFBlcmNlbnQsNTAiIC8+PFJvd3MgU3R5bGVzPSJBYnNvbHV0ZSwyMCxBYnNvbHV0ZSwzMCIgLz48L1RhYmxlTGF5b3V0U2V0dGluZ3M+Cw==";

        private const string ClassicTreeNode =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAAB1TeXN0ZW0uV2luZG93cy5Gb3Jtcy5UcmVlTm9kZQwAAAAHUHJvcEJhZwRUZXh0C1Rvb2xUaXBUZXh0BE5hbWUJSXNDaGVja2VkCkltYWdlSW5kZXgISW1hZ2VLZXkSU2VsZWN0ZWRJbWFnZUluZGV4EFNlbGVjdGVkSW1hZ2VLZXkKQ2hpbGRDb3VudAljaGlsZHJlbjAJY2hpbGRyZW4xBAEBAQAAAQABAAQEKVN5c3RlbS5XaW5kb3dzLkZvcm1zLk93bmVyRHJhd1Byb3BlcnR5QmFnAgAAAAEICAgdU3lzdGVtLldpbmRvd3MuRm9ybXMuVHJlZU5vZGUCAAAAHVN5c3RlbS5XaW5kb3dzLkZvcm1zLlRyZWVOb2RlAgAAAAIAAAAJAwAAAAYEAAAABW5vZGUxBgUAAAANdG9vbCB0aXAgdGV4dAkEAAAAAQEAAAAGBwAAAAD/////BggAAAADa2V5AgAAAAkJAAAACQoAAAAMCwAAAFFTeXN0ZW0uRHJhd2luZywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWIwM2Y1ZjdmMTFkNTBhM2EFAwAAAClTeXN0ZW0uV2luZG93cy5Gb3Jtcy5Pd25lckRyYXdQcm9wZXJ0eUJhZwMAAAAJQmFja0NvbG9yCUZvcmVDb2xvcgRGb250BAQEFFN5c3RlbS5EcmF3aW5nLkNvbG9yCwAAABRTeXN0ZW0uRHJhd2luZy5Db2xvcgsAAAATU3lzdGVtLkRyYXdpbmcuRm9udAsAAAACAAAABfT///8UU3lzdGVtLkRyYXdpbmcuQ29sb3IEAAAABG5hbWUFdmFsdWUKa25vd25Db2xvcgVzdGF0ZQEAAAAJBwcLAAAACgAAAAAAAAAApgABAAHz////9P///woAAAAAAAAAAE8AAQAJDgAAAAUJAAAAHVN5c3RlbS5XaW5kb3dzLkZvcm1zLlRyZWVOb2RlCQAAAARUZXh0C1Rvb2xUaXBUZXh0BE5hbWUJSXNDaGVja2VkCkltYWdlSW5kZXgISW1hZ2VLZXkSU2VsZWN0ZWRJbWFnZUluZGV4EFNlbGVjdGVkSW1hZ2VLZXkKQ2hpbGRDb3VudAEBAQAAAQABAAEICAgCAAAABg8AAAAFbm9kZTIJBwAAAAkHAAAAAP////8JBwAAAP////8JBwAAAAAAAAAFCgAAAB1TeXN0ZW0uV2luZG93cy5Gb3Jtcy5UcmVlTm9kZQkAAAAEVGV4dAtUb29sVGlwVGV4dAROYW1lCUlzQ2hlY2tlZApJbWFnZUluZGV4CEltYWdlS2V5ElNlbGVjdGVkSW1hZ2VJbmRleBBTZWxlY3RlZEltYWdlS2V5CkNoaWxkQ291bnQBAQEAAAEAAQABCAgIAgAAAAYRAAAABW5vZGUzCQcAAAAJBwAAAAD/////CQcAAAD/////CQcAAAAAAAAABQ4AAAATU3lzdGVtLkRyYXdpbmcuRm9udAQAAAAETmFtZQRTaXplBVN0eWxlBFVuaXQBAAQECxhTeXN0ZW0uRHJhd2luZy5Gb250U3R5bGULAAAAG1N5c3RlbS5EcmF3aW5nLkdyYXBoaWNzVW5pdAsAAAALAAAABhMAAAAUTWljcm9zb2Z0IFNhbnMgU2VyaWYAABBBBez///8YU3lzdGVtLkRyYXdpbmcuRm9udFN0eWxlAQAAAAd2YWx1ZV9fAAgLAAAAAAAAAAXr////G1N5c3RlbS5EcmF3aW5nLkdyYXBoaWNzVW5pdAEAAAAHdmFsdWVfXwAICwAAAAMAAAAL";
    }
}
