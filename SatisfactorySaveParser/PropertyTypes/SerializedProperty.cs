using SatisfactorySaveParser.ValueTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SatisfactorySaveParser.PropertyTypes
{
    public abstract class SerializedProperty
    {
        public string PropertyName { get; }
        public abstract string PropertyType { get; }
        public int Index { get; }

        public virtual int SerializedLength => _propertyValue.SerializedLength;

        /// <summary>
        /// This property is treated as the "value" by the core serialization routines, and by default
        /// is fetched from whatever property is marked with the [PropertyValue] attribute. A subclass
        /// can override this to customize where and how the value is returned, e.g. depending on
        /// what headers have been read.
        /// </summary>
        protected virtual ISerializableValue _propertyValue =>
            PropertyValueInfoFor(GetType()).GetSerializableValue(this);

        /// <summary>
        /// This property is treated as the list of "headers", and by default is a list compiled from all
        /// properties marked with the [PropertyHeader] attribute. A subclass can override this to
        /// customize what headers are written/read.
        /// </summary>
        protected virtual IEnumerable<ISerializableValue> _propertyHeaderValues =>
            PropertyHeaderInfoFor(GetType()).Select(phi => phi.GetSerializableValue(this));

        protected SerializedProperty(string propertyName, int index)
        {
            PropertyName = propertyName;
            Index = index;
        }

        public override string ToString()
        {
            return $"{PropertyType} {PropertyName}#{Index}: {_propertyValue}";
        }

        protected virtual void SerializeHeader(BinaryWriter writer)
        {
            writer.WriteLengthPrefixedString(PropertyName);
            writer.WriteLengthPrefixedString(PropertyType);
            writer.Write(SerializedLength);
            writer.Write(Index);
            foreach (var headerValue in _propertyHeaderValues ?? Array.Empty<ISerializableValue>())
            {
                headerValue.Serialize(writer);
            }
            writer.Write((byte)0);
        }

        protected virtual void SerializeValue(BinaryWriter writer)
        {
            _propertyValue.Serialize(writer);
        }

        public virtual void Serialize(BinaryWriter writer, bool writeHeader = true, bool writeValue = true)
        {
            if (writeHeader)
            {
                SerializeHeader(writer);
            }
            if (writeValue)
            {
                var before = writer.BaseStream.Position;
                SerializeValue(writer);
                var after = writer.BaseStream.Position;
                var serializedBytes = (int)(after - before);
                Trace.Assert(serializedBytes == SerializedLength);
            }
        }

        public static Type GetPropertyType(string propertyType) 
        {
            return propertyType switch
            {
                ArrayProperty.TypeName     => typeof(ArrayProperty),
                FloatProperty.TypeName     => typeof(FloatProperty),
                IntProperty.TypeName       => typeof(IntProperty),
                ByteProperty.TypeName      => typeof(ByteProperty),
                EnumProperty.TypeName      => typeof(EnumProperty),
                BoolProperty.TypeName      => typeof(BoolProperty),
                StrProperty.TypeName       => typeof(StrProperty),
                NameProperty.TypeName      => typeof(NameProperty),
                ObjectProperty.TypeName    => typeof(ObjectProperty),
                StructProperty.TypeName    => typeof(StructProperty),
                MapProperty.TypeName       => typeof(MapProperty),
                TextProperty.TypeName      => typeof(TextProperty),
                SetProperty.TypeName       => typeof(SetProperty),
                Int64Property.TypeName     => typeof(Int64Property),
                Int8Property.TypeName      => typeof(Int8Property),
                InterfaceProperty.TypeName => typeof(InterfaceProperty),
                _                          => throw new NotImplementedException(propertyType),
            };
        }

        public static SerializedProperty CreateProperty(string propertyType, string propertyName, int index)
        {
            return Activator.CreateInstance(GetPropertyType(propertyType), propertyName, index) as SerializedProperty;
        }

        protected virtual void ParseHeaderValues(BinaryReader reader)
        {
            if (_propertyHeaderValues != null)
            {
                foreach (var headerValue in _propertyHeaderValues)
                {
                    headerValue.Parse(reader);
                }
            }
        }

        protected virtual void ParsePropertyValue(BinaryReader reader)
        {
            _propertyValue.Parse(reader);
        }

        public static SerializedProperty Parse(BinaryReader reader)
        {
            SerializedProperty result;

            var propertyName = reader.ReadLengthPrefixedString();
            if (propertyName == "None")
            {
                return null;
            }

            Trace.Assert(!String.IsNullOrEmpty(propertyName));

            var fieldType = reader.ReadLengthPrefixedString();
            var size = reader.ReadInt32();
            var index = reader.ReadInt32();

            result = CreateProperty(fieldType, propertyName, index);
            result.ParseHeaderValues(reader);

            var endOfHeader = reader.ReadByte();
            Trace.Assert(endOfHeader == 0);

            var before = reader.BaseStream.Position;
            result.ParsePropertyValue(reader);
            var after = reader.BaseStream.Position;
            var readBytes = (int)(after - before);

            if (size != readBytes)
            {
                throw new InvalidOperationException($"Expected {size} bytes read but got {readBytes}");
            }

            return result;
        }

        #region Subclass metaprogramming
        protected record SerializablePropertyInfo(PropertyInfo Property, SerializableValueType SerializerType, bool ExplicitSerialization)
        {
            public object GetValue(SerializedProperty propObject) => Property.GetValue(propObject);

            public ISerializableValue GetSerializableValue(SerializedProperty propObject) =>
                (GetValue(propObject), ExplicitSerialization, SerializerType) switch
                {
                    (ISerializableValue isv, false, _) => isv,
                    (object value, _, SerializableValueType svt) => svt.CastOrInstantiate(value),
                    _ => throw new Exception($"Cannot get an ISerializableValue from {propObject.GetType().FullName}.{Property.Name}"),
                };

            public (SerializablePropertyInfo Info, object Value) WithValue(SerializedProperty propObject) =>
                (this, GetValue(propObject));

            public (SerializablePropertyInfo Info, ISerializableValue Value) WithSerializableValue(SerializedProperty propObject) =>
                (this, GetSerializableValue(propObject));
        }

        protected static Dictionary<Type, (SerializablePropertyInfo valueMember, List<SerializablePropertyInfo> headerMembers)> propertyTypeInfo = new();

        protected static (SerializablePropertyInfo valueMember, List<SerializablePropertyInfo> headerMembers) GetPropertyTypeInfo(Type propertyTypeClass)
        {
            SerializablePropertyInfo valueMember = null;
            List<SerializablePropertyInfo> headerMembers;

            MemberInfo[] FindPropertiesWithAttribute<T>(BindingFlags extraBindingFlags = 0) where T: PropertyValueBase
                => propertyTypeClass.FindMembers(
                    MemberTypes.Property,
                    BindingFlags.Instance | BindingFlags.Public | extraBindingFlags,
                    (m, _) => (m.IsDefined(typeof(T), false)),
                    null);

            SerializablePropertyInfo getSerializerInfo<T>(PropertyInfo property) where T : PropertyValueBase
            {
                var attribute = property.GetCustomAttribute<T>();
                var propType = property.PropertyType;
                return new(
                    property,
                    attribute.SerializerType ?? SerializableValueType.TryForType(propType),
                    attribute.SerializerType != null
                    );
            }

            var valueProps = FindPropertiesWithAttribute<PropertyValueAttribute>();
            if (valueProps.Length > 1)
            {
                // try only searching properties on this class in particular
                valueProps = FindPropertiesWithAttribute<PropertyValueAttribute>(BindingFlags.DeclaredOnly);
                if (valueProps.Length > 1)
                {
                    throw new NotSupportedException($"A SerializedProperty class cannot have multiple members with PropertyValueAttribute set; class {propertyTypeClass.FullName} has these: {valueProps.Select(m=>m.Name).StringJoin()}");
                }
            }

            valueMember = valueProps
                .Cast<PropertyInfo>()
                .Select(getSerializerInfo<PropertyValueAttribute>)
                .SingleOrDefault();

            headerMembers = FindPropertiesWithAttribute<PropertyHeaderAttribute>()
                .OrderBy(m => m.GetCustomAttribute<PropertyHeaderAttribute>().Order)
                .Cast<PropertyInfo>()
                .Select(getSerializerInfo<PropertyHeaderAttribute>)
                .ToList();

            return (valueMember, headerMembers);
        }

        protected static SerializablePropertyInfo PropertyValueInfoFor(Type propertyTypeClass) =>
            propertyTypeInfo.GetValueAndSetDefault(propertyTypeClass, GetPropertyTypeInfo).valueMember;

        protected static IReadOnlyList<SerializablePropertyInfo> PropertyHeaderInfoFor(Type propertyTypeClass) =>
            propertyTypeInfo.GetValueAndSetDefault(propertyTypeClass, GetPropertyTypeInfo).headerMembers;
        #endregion

    }

    #region Property header/value attributes
    public abstract class PropertyValueBase: Attribute
    {
        protected SerializableValueType _serializeAs;

        public Type SerializeAs {
            get => _serializeAs;
            set {
                if (value != null)
                {
                    if (!value.IsAssignableTo(typeof(ISerializableValue)))
                    {
                        throw new ArgumentException($"serializeAs must be a class that implements IValueSerializer; got {value.FullName}");
                    }
                    _serializeAs = SerializableValueType.ForType(value);
                }
                else
                {
                    _serializeAs = null;
                }
            }
        }

        public SerializableValueType SerializerType => _serializeAs;
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class PropertyValueAttribute: PropertyValueBase
    {
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class PropertyHeaderAttribute: PropertyValueBase
    {
        public int Order { get; set; } = 0;
    }
    #endregion
}