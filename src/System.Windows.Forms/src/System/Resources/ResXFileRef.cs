// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.Resources
{
    /// <summary>
    ///  ResX File Reference class. This allows the developer to represent
    ///  a link to an external resource. When the resource manager asks
    ///  for the value of the resource item, the external resource is loaded.
    /// </summary>
    [TypeConverter(typeof(Converter))]
    public partial class ResXFileRef
    {
        private string fileName;
        private readonly string typeName;
        private Encoding textFileEncoding;

        /// <summary>
        ///  Creates a new ResXFileRef that points to the specified file.
        ///  The type refered to by typeName must support a constructor
        ///  that accepts a System.IO.Stream as a parameter.
        /// </summary>
        public ResXFileRef(string fileName, string typeName)
        {
            this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            this.typeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        /// <summary>
        ///  Creates a new ResXFileRef that points to the specified file.
        ///  The type refered to by typeName must support a constructor
        ///  that accepts a System.IO.Stream as a parameter.
        /// </summary>
        public ResXFileRef(string fileName, string typeName, Encoding textFileEncoding) : this(fileName, typeName)
        {
            this.textFileEncoding = textFileEncoding;
        }

        internal ResXFileRef Clone()
        {
            return new ResXFileRef(fileName, typeName, textFileEncoding);
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        public string TypeName
        {
            get
            {
                return typeName;
            }
        }

        public Encoding TextFileEncoding
        {
            get
            {
                return textFileEncoding;
            }
        }

        /// <summary>
        ///  path1+result = path2
        ///  A string which is the relative path difference between path1 and
        ///  path2 such that if path1 and the calculated difference are used
        ///  as arguments to Combine(), path2 is returned
        /// </summary>
        private static string PathDifference(string path1, string path2, bool compareCase)
        {
            int i;
            int si = -1;

            for (i = 0; (i < path1.Length) && (i < path2.Length); ++i)
            {
                if ((path1[i] != path2[i]) && (compareCase || (char.ToLower(path1[i], CultureInfo.InvariantCulture) != char.ToLower(path2[i], CultureInfo.InvariantCulture))))
                {
                    break;
                }
                if (path1[i] == Path.DirectorySeparatorChar)
                {
                    si = i;
                }
            }

            if (i == 0)
            {
                return path2;
            }
            if ((i == path1.Length) && (i == path2.Length))
            {
                return string.Empty;
            }

            StringBuilder relPath = new StringBuilder();

            for (; i < path1.Length; ++i)
            {
                if (path1[i] == Path.DirectorySeparatorChar)
                {
                    relPath.Append(".." + Path.DirectorySeparatorChar);
                }
            }
            return relPath.ToString() + path2.Substring(si + 1);
        }

        internal void MakeFilePathRelative(string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                return;
            }
            fileName = PathDifference(basePath, fileName, false);
        }

        public override string ToString()
        {
            string result = string.Empty;

            if (fileName.IndexOf(';') != -1 || fileName.IndexOf('\"') != -1)
            {
                result += ("\"" + fileName + "\";");
            }
            else
            {
                result += (fileName + ";");
            }
            result += typeName;
            if (textFileEncoding != null)
            {
                result += (";" + textFileEncoding.WebName);
            }
            return result;
        }
    }
}
