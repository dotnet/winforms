// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using Microsoft.VisualBasic.Devices;
using Xunit;
using DataFormats = System.Windows.Forms.DataFormats;
using TextDataFormat = System.Windows.Forms.TextDataFormat;

namespace Microsoft.VisualBasic.MyServices.Tests
{
    public class ClipboardProxyTests
    {
        [WinFormsFact]
        public void Clear()
        {
            var clipboard = (new Computer()).Clipboard;
            var text = GetUniqueText();
            clipboard.SetText(text);
            Assert.True(System.Windows.Forms.Clipboard.ContainsText());
            clipboard.Clear();
            Assert.False(System.Windows.Forms.Clipboard.ContainsText());
        }

        [WinFormsFact]
        public void Text()
        {
            var clipboard = (new Computer()).Clipboard;
            var text = GetUniqueText();
            clipboard.SetText(text, TextDataFormat.UnicodeText);
            Assert.Equal(System.Windows.Forms.Clipboard.ContainsText(), clipboard.ContainsText());
            Assert.Equal(System.Windows.Forms.Clipboard.GetText(), clipboard.GetText());
            Assert.Equal(System.Windows.Forms.Clipboard.GetText(TextDataFormat.UnicodeText), clipboard.GetText(TextDataFormat.UnicodeText));
            Assert.Equal(text, clipboard.GetText(TextDataFormat.UnicodeText));
        }

        [WinFormsFact]
        public void Image()
        {
            var clipboard = (new Computer()).Clipboard;
            var image = new Bitmap(2, 2);
            Assert.Equal(System.Windows.Forms.Clipboard.ContainsImage(), clipboard.ContainsImage());
            Assert.Equal(System.Windows.Forms.Clipboard.GetImage(), clipboard.GetImage());
            clipboard.SetImage(image);
        }

        [WinFormsFact]
        public void Audio()
        {
            var clipboard = (new Computer()).Clipboard;
            Assert.Equal(System.Windows.Forms.Clipboard.ContainsAudio(), clipboard.ContainsAudio());
            // Not tested:
            //   Public Function GetAudioStream() As Stream
            //   Public Sub SetAudio(ByVal audioBytes As Byte())
            //   Public Sub SetAudio(ByVal audioStream As Stream)
        }

        [WinFormsFact]
        public void FileDropList()
        {
            var clipboard = (new Computer()).Clipboard;
            Assert.Equal(System.Windows.Forms.Clipboard.ContainsFileDropList(), clipboard.ContainsFileDropList());
            // Not tested:
            //   Public Function GetFileDropList() As StringCollection
            //   Public Sub SetFileDropList(ByVal filePaths As StringCollection)
        }

        [WinFormsFact]
        public void Data()
        {
            var clipboard = (new Computer()).Clipboard;
            object data = GetUniqueText();
            Assert.Equal(System.Windows.Forms.Clipboard.ContainsData(DataFormats.UnicodeText), clipboard.ContainsData(DataFormats.UnicodeText));
            Assert.Equal(System.Windows.Forms.Clipboard.GetData(DataFormats.UnicodeText), clipboard.GetData(DataFormats.UnicodeText));
            clipboard.SetData(DataFormats.UnicodeText, data);
        }

        [WinFormsFact]
        public void DataObject()
        {
            var clipboard = (new Computer()).Clipboard;
            object data = GetUniqueText();
            Assert.Equal(System.Windows.Forms.Clipboard.GetDataObject().GetData(DataFormats.UnicodeText), clipboard.GetDataObject().GetData(DataFormats.UnicodeText));
            clipboard.SetDataObject(new System.Windows.Forms.DataObject(data));
        }

        private static string GetUniqueText() => Guid.NewGuid().ToString("D");
    }
}
