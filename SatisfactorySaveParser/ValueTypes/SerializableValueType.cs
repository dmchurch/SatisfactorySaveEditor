using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Linq;

namespace SatisfactorySaveParser.ValueTypes
{
    public class SerializableValueType : TypeDelegator
    {
        public ISerializableValue Serializer { get; protected init; }
        public ISerializableValue CreateInstance(params object[] args)
            => (ISerializableValue)Activator.CreateInstance(UnderlyingSystemType, args, null);

        public ISerializableValue CastOrInstantiate(object value) =>
            value == null ? null :
            value.GetType().IsAssignableTo(this) ? (ISerializableValue)value :
            CreateInstance(value);

        public static SerializableValueType ForType(Type type) =>
            type as SerializableValueType ??
            _registeredTypes.GetValueOrDefault(type.UnderlyingSystemType) ??
            new(type);

        public static SerializableValueType TryForType(Type type)
        {
            try
            {
                return ForType(type);
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        protected SerializableValueType(Type type) : base(type)
        {
            if (!type.IsAssignableTo(typeof(ISerializableValue)))
            {
                throw new InvalidCastException($"Type {type.FullName} does not implement ISerializableValue");
            }
            if (_registeredTypes.ContainsKey(UnderlyingSystemType))
            {
                throw new Exception($"Type {type.FullName} attempting to construct a second SerializableValueType??");
            }
            Serializer = Activator.CreateInstance(UnderlyingSystemType) as ISerializableValue;
            _registeredTypes[UnderlyingSystemType] = this;
        }

        private static Dictionary<Type, SerializableValueType> _registeredTypes = new();
    }
}
