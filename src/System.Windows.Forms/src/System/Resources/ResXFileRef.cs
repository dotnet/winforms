// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace System.Resources
{
    /// <summary>
    ///  ResX File Reference class. This allows the developer to represent
    ///  a link to an external resource. When the resource manager asks
    ///  for the value of the resource item, the external resource is loaded.
    /// </summary>
    [TypeConverter(typeof(Converter))]
    public class ResXFileRef
    {
        private string fileName;
        private readonly string typeName;
        [OptionalField(VersionAdded = 2)]
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

        [OnDeserializing]
        private void OnDeserializing(StreamingContext ctx)
        {
            textFileEncoding = null;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
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

        public class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context,
                                                Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                return false;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context,
                                              Type destinationType)
            {
                return destinationType == typeof(string);
            }

            public override object ConvertTo(ITypeDescriptorContext context,
                                             CultureInfo culture,
                                             object value,
                                             Type destinationType)
            {
                object created = null;
                if (destinationType == typeof(string))
                {
                    created = ((ResXFileRef)value).ToString();
                }
                return created;
            }

            // "value" is the parameter name of ConvertFrom, which calls this method.
            internal static string[] ParseResxFileRefString(string stringValue)
            {
                string[] result = null;
                if (stringValue != null)
                {
                    stringValue = stringValue.Trim();
                    string fileName;
                    string remainingString;
                    if (stringValue.StartsWith("\""))
                    {
                        int lastIndexOfQuote = stringValue.LastIndexOf('\"');
                        if (lastIndexOfQuote - 1 < 0)
                        {
                            throw new ArgumentException(nameof(stringValue));
                        }

                        fileName = stringValue.Substring(1, lastIndexOfQuote - 1); // remove the quotes in" ..... "
                        if (lastIndexOfQuote + 2 > stringValue.Length)
                        {
                            throw new ArgumentException(nameof(stringValue));
                        }

                        remainingString = stringValue.Substring(lastIndexOfQuote + 2);
                    }
                    else
                    {
                        int nextSemiColumn = stringValue.IndexOf(';');
                        if (nextSemiColumn == -1)
                        {
                            throw new ArgumentException(nameof(stringValue));
                        }

                        fileName = stringValue.Substring(0, nextSemiColumn);
                        if (nextSemiColumn + 1 > stringValue.Length)
                        {
                            throw new ArgumentException(nameof(stringValue));
                        }

                        remainingString = stringValue.Substring(nextSemiColumn + 1);
                    }
                    string[] parts = remainingString.Split(';');
                    if (parts.Length > 1)
                    {
                        result = new string[] { fileName, parts[0], parts[1] };
                    }
                    else if (parts.Length > 0)
                    {
                        result = new string[] { fileName, parts[0] };
                    }
                    else
                    {
                        result = new string[] { fileName };
                    }
                }
                return result;
            }

            public override object ConvertFrom(ITypeDescriptorContext context,
                                               CultureInfo culture,
                                               object value)
            {
                if (value is string stringValue)
                {
                    string[] parts = ParseResxFileRefString(stringValue);
                    string fileName = parts[0];
                    Type toCreate = Type.GetType(parts[1], true);

                    // special case string and byte[]
                    if (toCreate == typeof(string))
                    {
                        // we have a string, now we need to check the encoding
                        Encoding textFileEncoding =
                            parts.Length > 2
                                ? Encoding.GetEncoding(parts[2])
                                : Encoding.Default;
                        using (StreamReader sr = new StreamReader(fileName, textFileEncoding))
                        {
                            return sr.ReadToEnd();
                        }
                    }

                    // this is a regular file, we call it's constructor with a stream as a parameter
                    // or if it's a byte array we just return that
                    byte[] temp = null;

                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        Debug.Assert(fileStream != null, "Couldn't open " + fileName);
                        temp = new byte[fileStream.Length];
                        fileStream.Read(temp, 0, (int)fileStream.Length);
                    }

                    if (toCreate == typeof(byte[]))
                    {
                        return temp;
                    }

                    MemoryStream memStream = new MemoryStream(temp);
                    if (toCreate == typeof(MemoryStream))
                    {
                        return memStream;
                    }
                    if (toCreate == typeof(Bitmap) && fileName.EndsWith(".ico"))
                    {
                        // we special case the .ico bitmaps because GDI+ destroy the alpha channel component and
                        // we don't want that to happen
                        Icon ico = new Icon(memStream);
                        return ico.ToBitmap();
                    }

                    return Activator.CreateInstance(toCreate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, new object[] { memStream }, null);
                }
                return null;
            }
        }
    }
}
