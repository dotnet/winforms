// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Resources
{
    internal class DataNodeInfo
    {
        public string Name;
        public string Comment;
        public string TypeName;
        public string MimeType;
        public string ValueData;
        public Point ReaderPosition; //only used to track position in the reader

        internal DataNodeInfo Clone()
        {
            return new DataNodeInfo
            {
                Name = Name,
                Comment = Comment,
                TypeName = TypeName,
                MimeType = MimeType,
                ValueData = ValueData,
                ReaderPosition = new Point(ReaderPosition.X, ReaderPosition.Y)
            };
        }
    }
}
