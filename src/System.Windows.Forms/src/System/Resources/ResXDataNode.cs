// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace System.Resources
{
    public sealed class ResXDataNode : ISerializable
    {
        private static readonly char[] s_specialChars = new char[] { ' ', '\r', '\n' };

        private DataNodeInfo _nodeInfo;

        private string _name;
        private string _comment;

        private string _typeName; // is only used when we create a resxdatanode manually with an object and contains the FQN

        private string _fileRefFullPath;
        private string _fileRefType;
        private string _fileRefTextEncoding;

        private object _value;
        private ResXFileRef _fileRef;

        private IFormatter _binaryFormatter;

        // this is going to be used to check if a ResXDataNode is of type ResXFileRef
        private static readonly ITypeResolutionService s_internalTypeResolver = new AssemblyNamesTypeResolutionService(new AssemblyName[] { new AssemblyName("System.Windows.Forms") });

        // call back function to get type name for multitargeting.
        // No public property to force using constructors for the following reasons:
        // 1. one of the constructors needs this field (if used) to initialize the object, make it consistent with the other ctrs to avoid errors.
        // 2. once the object is constructed the delegate should not be changed to avoid getting inconsistent results.
        private Func<Type, string> _typeNameConverter;

        private ResXDataNode()
        {
        }

        internal ResXDataNode DeepClone()
        {
            return new ResXDataNode
            {
                // nodeinfo is just made up of immutable objects, we don't need to clone it
                _nodeInfo = _nodeInfo?.Clone(),
                _name = _name,
                _comment = _comment,
                _typeName = _typeName,
                _fileRefFullPath = _fileRefFullPath,
                _fileRefType = _fileRefType,
                _fileRefTextEncoding = _fileRefTextEncoding,
                // we don't clone the value, because we don't know how
                _value = _value,
                _fileRef = _fileRef?.Clone(),
                _typeNameConverter = _typeNameConverter
            };
        }

        public ResXDataNode(string name, object value) : this(name, value, null)
        {
        }

        public ResXDataNode(string name, object value, Func<Type, string> typeNameConverter)
        {
            if (name is null)
            {
                throw (new ArgumentNullException(nameof(name)));
            }

            if (name.Length == 0)
            {
                throw (new ArgumentException(nameof(name)));
            }

            _typeNameConverter = typeNameConverter;

            Type valueType = (value is null) ? typeof(object) : value.GetType();

            if (value != null && !valueType.IsSerializable)
            {
                throw new InvalidOperationException(string.Format(SR.NotSerializableType, name, valueType.FullName));
            }

            if (value != null)
            {
                _typeName = MultitargetUtil.GetAssemblyQualifiedName(valueType, _typeNameConverter);
            }

            _name = name;
            _value = value;
        }

        public ResXDataNode(string name, ResXFileRef fileRef) : this(name, fileRef, null)
        {
        }

        public ResXDataNode(string name, ResXFileRef fileRef, Func<Type, string> typeNameConverter)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _name = name;
            _fileRef = fileRef ?? throw new ArgumentNullException(nameof(fileRef));
            _typeNameConverter = typeNameConverter;
        }

        internal ResXDataNode(DataNodeInfo nodeInfo, string basePath)
        {
            _nodeInfo = nodeInfo;
            InitializeDataNode(basePath);
        }

        private void InitializeDataNode(string basePath)
        {
            // we can only use our internal type resolver here
            // because we only want to check if this is a ResXFileRef node
            // and we can't be sure that we have a typeResolutionService that can
            // recognize this. It's not very clean but this should work.
            Type nodeType = null;
            if (!string.IsNullOrEmpty(_nodeInfo.TypeName)) // can be null if we have a string (default for string is TypeName == null)
            {
                nodeType = s_internalTypeResolver.GetType(_nodeInfo.TypeName, false, true);
            }

            if (nodeType != null && nodeType.Equals(typeof(ResXFileRef)))
            {
                // we have a fileref, split the value data and populate the fields
                string[] fileRefDetails = ResXFileRef.Converter.ParseResxFileRefString(_nodeInfo.ValueData);
                if (fileRefDetails != null && fileRefDetails.Length > 1)
                {
                    if (!Path.IsPathRooted(fileRefDetails[0]) && basePath != null)
                    {
                        _fileRefFullPath = Path.Combine(basePath, fileRefDetails[0]);
                    }
                    else
                    {
                        _fileRefFullPath = fileRefDetails[0];
                    }

                    _fileRefType = fileRefDetails[1];
                    if (fileRefDetails.Length > 2)
                    {
                        _fileRefTextEncoding = fileRefDetails[2];
                    }
                }
            }
        }

        public string Comment
        {
            get
            {
                string result = _comment;
                if (result is null && _nodeInfo != null)
                {
                    result = _nodeInfo.Comment;
                }
                return result ?? string.Empty;
            }
            set
            {
                _comment = value;
            }
        }

        public string Name
        {
            get
            {
                string result = _name;
                if (result is null && _nodeInfo != null)
                {
                    result = _nodeInfo.Name;
                }
                return result;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(Name));
                }
                if (value.Length == 0)
                {
                    throw new ArgumentException(nameof(Name));
                }
                _name = value;
            }
        }

        public ResXFileRef FileRef
        {
            get
            {
                if (FileRefFullPath is null)
                {
                    return null;
                }
                if (_fileRef is null)
                {
                    _fileRef =
                        string.IsNullOrEmpty(_fileRefTextEncoding)
                            ? new ResXFileRef(FileRefFullPath, FileRefType)
                            : new ResXFileRef(FileRefFullPath, FileRefType, Encoding.GetEncoding(FileRefTextEncoding));
                }
                return _fileRef;
            }
        }

        private string FileRefFullPath
        {
            get
            {
                return _fileRef?.FileName ?? _fileRefFullPath;
            }
        }

        private string FileRefType
        {
            get
            {
                return _fileRef?.TypeName ?? _fileRefType;
            }
        }

        private string FileRefTextEncoding
        {
            get
            {
                return _fileRef?.TextFileEncoding?.BodyName ?? _fileRefTextEncoding;
            }
        }

        private static string ToBase64WrappedString(byte[] data)
        {
            const int lineWrap = 80;
            const string crlf = "\r\n";
            const string prefix = "        ";
            string raw = Convert.ToBase64String(data);
            if (raw.Length > lineWrap)
            {
                StringBuilder output = new StringBuilder(raw.Length + (raw.Length / lineWrap) * 3); // word wrap on lineWrap chars, \r\n
                int current = 0;
                for (; current < raw.Length - lineWrap; current += lineWrap)
                {
                    output.Append(crlf);
                    output.Append(prefix);
                    output.Append(raw, current, lineWrap);
                }
                output.Append(crlf);
                output.Append(prefix);
                output.Append(raw, current, raw.Length - current);
                output.Append(crlf);
                return output.ToString();
            }

            return raw;
        }

        private void FillDataNodeInfoFromObject(DataNodeInfo nodeInfo, object value)
        {
            if (value is CultureInfo ci)
            {
                // special-case CultureInfo, cannot use CultureInfoConverter for serialization
                nodeInfo.ValueData = ci.Name;
                nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(CultureInfo), _typeNameConverter);
            }
            else if (value is string str)
            {
                nodeInfo.ValueData = str;
            }
            else if (value is byte[] bytes)
            {
                nodeInfo.ValueData = ToBase64WrappedString(bytes);
                nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(byte[]), _typeNameConverter);
            }
            else
            {
                Type valueType = (value is null) ? typeof(object) : value.GetType();
                if (value != null && !valueType.IsSerializable)
                {
                    throw new InvalidOperationException(string.Format(SR.NotSerializableType, _name, valueType.FullName));
                }

                TypeConverter tc = TypeDescriptor.GetConverter(valueType);
                bool toString = tc.CanConvertTo(typeof(string));
                bool fromString = tc.CanConvertFrom(typeof(string));
                try
                {
                    if (toString && fromString)
                    {
                        nodeInfo.ValueData = tc.ConvertToInvariantString(value);
                        nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(valueType, _typeNameConverter);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // Some custom type converters will throw in ConvertTo(string)
                    // to indicate that this object should be serialized through ISeriazable
                    // instead of as a string. This is semi-wrong, but something we will have to
                    // live with to allow user created Cursors to be serializable.
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }

                bool toByteArray = tc.CanConvertTo(typeof(byte[]));
                bool fromByteArray = tc.CanConvertFrom(typeof(byte[]));
                if (toByteArray && fromByteArray)
                {
                    byte[] data = (byte[])tc.ConvertTo(value, typeof(byte[]));
                    nodeInfo.ValueData = ToBase64WrappedString(data);
                    nodeInfo.MimeType = ResXResourceWriter.ByteArraySerializedObjectMimeType;
                    nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(valueType, _typeNameConverter);
                    return;
                }

                if (value is null)
                {
                    nodeInfo.ValueData = string.Empty;
                    nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXNullRef), _typeNameConverter);
                }
                else
                {
                    if (_binaryFormatter is null)
                    {
                        _binaryFormatter = new BinaryFormatter
                        {
                            Binder = new ResXSerializationBinder(_typeNameConverter)
                        };
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
#pragma warning disable SYSLIB0011
                        _binaryFormatter.Serialize(ms, value);

                        nodeInfo.ValueData = ToBase64WrappedString(ms.ToArray());
                    }

                    nodeInfo.MimeType = ResXResourceWriter.DefaultSerializedObjectMimeType;
                }
            }
        }

        private object GenerateObjectFromDataNodeInfo(DataNodeInfo dataNodeInfo, ITypeResolutionService typeResolver)
        {
            object result = null;
            string mimeTypeName = dataNodeInfo.MimeType;
            // default behavior: if we dont have a type name, it's a string
            string typeName =
                string.IsNullOrEmpty(dataNodeInfo.TypeName)
                    ? MultitargetUtil.GetAssemblyQualifiedName(typeof(string), _typeNameConverter)
                    : dataNodeInfo.TypeName;

            if (!string.IsNullOrEmpty(mimeTypeName))
            {
                if (string.Equals(mimeTypeName, ResXResourceWriter.BinSerializedObjectMimeType))
                {
                    string text = dataNodeInfo.ValueData;
                    byte[] serializedData = FromBase64WrappedString(text);

                    if (_binaryFormatter is null)
                    {
                        _binaryFormatter = new BinaryFormatter
                        {
                            Binder = new ResXSerializationBinder(typeResolver)
                        };
                    }

                    IFormatter formatter = _binaryFormatter;
                    if (serializedData != null && serializedData.Length > 0)
                    {
#pragma warning disable SYSLIB0011
                        result = formatter.Deserialize(new MemoryStream(serializedData));
#pragma warning restore SYSLIB0011
                        if (result is ResXNullRef)
                        {
                            result = null;
                        }
                    }
                }
                else if (string.Equals(mimeTypeName, ResXResourceWriter.ByteArraySerializedObjectMimeType))
                {
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        Type type = ResolveType(typeName, typeResolver);
                        if (type != null)
                        {
                            TypeConverter tc = TypeDescriptor.GetConverter(type);
                            if (tc.CanConvertFrom(typeof(byte[])))
                            {
                                string text = dataNodeInfo.ValueData;
                                byte[] serializedData = FromBase64WrappedString(text);

                                if (serializedData != null)
                                {
                                    result = tc.ConvertFrom(serializedData);
                                }
                            }
                        }
                        else
                        {
                            string newMessage = string.Format(SR.TypeLoadException, typeName, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X);
                            XmlException xml = new XmlException(newMessage, null, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X);
                            TypeLoadException newTle = new TypeLoadException(newMessage, xml);

                            throw newTle;
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(typeName))
            {
                Type type = ResolveType(typeName, typeResolver);
                if (type != null)
                {
                    if (type == typeof(ResXNullRef))
                    {
                        result = null;
                    }
                    else if (typeName.IndexOf("System.Byte[]") != -1 && typeName.IndexOf("mscorlib") != -1)
                    {
                        // Handle byte[]'s, which are stored as base-64 encoded strings.
                        // We can't hard-code byte[] type name due to version number
                        // updates & potential whitespace issues with ResX files.
                        result = FromBase64WrappedString(dataNodeInfo.ValueData);
                    }
                    else
                    {
                        TypeConverter tc = TypeDescriptor.GetConverter(type);
                        if (tc.CanConvertFrom(typeof(string)))
                        {
                            string text = dataNodeInfo.ValueData;
                            try
                            {
                                result = tc.ConvertFromInvariantString(text);
                            }
                            catch (NotSupportedException nse)
                            {
                                string newMessage = string.Format(SR.NotSupported, typeName, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X, nse.Message);
                                XmlException xml = new XmlException(newMessage, nse, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X);
                                NotSupportedException newNse = new NotSupportedException(newMessage, xml);
                                throw newNse;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Converter for " + type.FullName + " doesn't support string conversion");
                        }
                    }
                }
                else
                {
                    string newMessage = string.Format(SR.TypeLoadException, typeName, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X);
                    XmlException xml = new XmlException(newMessage, null, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X);
                    TypeLoadException newTle = new TypeLoadException(newMessage, xml);

                    throw newTle;
                }
            }
            else
            {
                // if mimeTypeName and typeName are not filled in, the value must be a string
                Debug.Assert(_value is string, "Resource entries with no Type or MimeType must be encoded as strings");
            }

            return result;
        }

        internal DataNodeInfo GetDataNodeInfo()
        {
            bool shouldSerialize = true;
            if (_nodeInfo != null)
            {
                shouldSerialize = false;
            }
            else
            {
                _nodeInfo = new DataNodeInfo();
            }
            _nodeInfo.Name = Name;
            _nodeInfo.Comment = Comment;

            // We always serialize if this node represents a FileRef. This is because FileRef is a public property,
            // so someone could have modified it.
            if (shouldSerialize || FileRefFullPath != null)
            {
                // if we dont have a datanodeinfo it could be either
                // a direct object OR a fileref
                if (FileRefFullPath != null)
                {
                    _nodeInfo.ValueData = FileRef.ToString();
                    _nodeInfo.MimeType = null;
                    _nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXFileRef), _typeNameConverter);
                }
                else
                {
                    // serialize to string inside the nodeInfo
                    FillDataNodeInfoFromObject(_nodeInfo, _value);
                }
            }
            return _nodeInfo;
        }

        /// <summary>
        ///  Might return the position in the resx file of the current node, if known
        ///  otherwise, will return Point(0,0) since point is a struct
        /// </summary>
        public Point GetNodePosition()
        {
            return _nodeInfo?.ReaderPosition ?? new Point();
        }

        /// <summary>
        ///  Get the FQ type name for this datanode.
        ///  We return typeof(object) for ResXNullRef
        /// </summary>
        public string GetValueTypeName(ITypeResolutionService typeResolver)
        {
            // the type name here is always a FQN
            if (!string.IsNullOrEmpty(_typeName))
            {
                return
                    _typeName == MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXNullRef), _typeNameConverter)
                        ? MultitargetUtil.GetAssemblyQualifiedName(typeof(object), _typeNameConverter)
                        : _typeName;
            }
            string result = FileRefType;
            Type objectType = null;
            // do we have a fileref?
            if (result != null)
            {
                // try to resolve this type
                objectType = ResolveType(FileRefType, typeResolver);
            }
            else if (_nodeInfo != null)
            {
                // we dont have a fileref, try to resolve the type of the datanode
                result = _nodeInfo.TypeName;
                // if typename is null, the default is just a string
                if (string.IsNullOrEmpty(result))
                {
                    // we still dont know... do we have a mimetype? if yes, our only option is to
                    // deserialize to know what we're dealing with... very inefficient...
                    if (!string.IsNullOrEmpty(_nodeInfo.MimeType))
                    {
                        object insideObject = null;

                        try
                        {
                            insideObject = GenerateObjectFromDataNodeInfo(_nodeInfo, typeResolver);
                        }
                        catch (Exception ex)
                        {
                            // it'd be better to catch SerializationException but the underlying type resolver
                            // can throw things like FileNotFoundException which is kinda confusing, so I am catching all here..
                            if (ClientUtils.IsCriticalException(ex))
                            {
                                throw;
                            }
                            // something went wrong, type is not specified at all or stream is corrupted
                            // return system.object
                            result = MultitargetUtil.GetAssemblyQualifiedName(typeof(object), _typeNameConverter);
                        }

                        if (insideObject != null)
                        {
                            result = MultitargetUtil.GetAssemblyQualifiedName(insideObject.GetType(), _typeNameConverter);
                        }
                    }
                    else
                    {
                        // no typename, no mimetype, we have a string...
                        result = MultitargetUtil.GetAssemblyQualifiedName(typeof(string), _typeNameConverter);
                    }
                }
                else
                {
                    objectType = ResolveType(_nodeInfo.TypeName, typeResolver);
                }
            }
            if (objectType != null)
            {
                if (objectType == typeof(ResXNullRef))
                {
                    result = MultitargetUtil.GetAssemblyQualifiedName(typeof(object), _typeNameConverter);
                }
                else
                {
                    result = MultitargetUtil.GetAssemblyQualifiedName(objectType, _typeNameConverter);
                }
            }
            return result;
        }

        /// <summary>
        ///  Get the FQ type name for this datanode
        /// </summary>
        public string GetValueTypeName(AssemblyName[] names)
        {
            return GetValueTypeName(new AssemblyNamesTypeResolutionService(names));
        }

        /// <summary>
        ///  Get the value contained in this datanode
        /// </summary>
        public object GetValue(ITypeResolutionService typeResolver)
        {
            if (_value != null)
            {
                return _value;
            }

            object result = null;
            if (FileRefFullPath != null)
            {
                Type objectType = ResolveType(FileRefType, typeResolver);
                if (objectType != null)
                {
                    // we have the FQN for this type
                    _fileRef =
                        FileRefTextEncoding != null
                            ? new ResXFileRef(FileRefFullPath, FileRefType, Encoding.GetEncoding(FileRefTextEncoding))
                            : new ResXFileRef(FileRefFullPath, FileRefType);
                    TypeConverter tc = TypeDescriptor.GetConverter(typeof(ResXFileRef));
                    result = tc.ConvertFrom(_fileRef.ToString());
                }
                else
                {
                    string newMessage = string.Format(SR.TypeLoadExceptionShort, FileRefType);
                    TypeLoadException newTle = new TypeLoadException(newMessage);
                    throw (newTle);
                }
            }
            else if (_nodeInfo.ValueData != null)
            {
                // it's embedded, we deserialize it
                result = GenerateObjectFromDataNodeInfo(_nodeInfo, typeResolver);
            }
            else
            {
                // schema is wrong and say minOccur for Value is 0,
                // but it's too late to change it...
                // we need to return null here
                return null;
            }
            return result;
        }

        /// <summary>
        ///  Get the value contained in this datanode
        /// </summary>
        public object GetValue(AssemblyName[] names)
        {
            return GetValue(new AssemblyNamesTypeResolutionService(names));
        }

        private static byte[] FromBase64WrappedString(string text)
        {
            if (text.IndexOfAny(s_specialChars) != -1)
            {
                StringBuilder sb = new StringBuilder(text.Length);
                foreach (var ch in text)
                {
                    switch (ch)
                    {
                        case ' ':
                        case '\r':
                        case '\n':
                            break;
                        default:
                            sb.Append(ch);
                            break;
                    }
                }
                return Convert.FromBase64String(sb.ToString());
            }

            return Convert.FromBase64String(text);
        }

        private Type ResolveType(string typeName, ITypeResolutionService typeResolver)
        {
            Type resolvedType = null;
            if (typeResolver != null)
            {
                // If we cannot find the strong-named type, then try to see
                // if the TypeResolver can bind to partial names. For this,
                // we will strip out the partial names and keep the rest of the
                // strong-name information to try again.

                resolvedType = typeResolver.GetType(typeName, false);
                if (resolvedType is null)
                {
                    string[] typeParts = typeName.Split(',');

                    // Break up the type name from the rest of the assembly strong name.
                    if (typeParts != null && typeParts.Length >= 2)
                    {
                        string partialName = typeParts[0].Trim();
                        string assemblyName = typeParts[1].Trim();
                        partialName = partialName + ", " + assemblyName;
                        resolvedType = typeResolver.GetType(partialName, false);
                    }
                }
            }

            if (resolvedType is null)
            {
                resolvedType = Type.GetType(typeName, false);
            }

            return resolvedType;
        }

        /// <summary>
        ///  Get the value contained in this datanode
        /// </summary>
        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
