using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SatisfactorySaveParser.PropertyTypes;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace SatisfactorySaveParser.CLITool
{
    class SimplePropertyVisitor : ChainedObjectGraphVisitor
    {
        private IEventEmitter eventEmitter;
        private ObjectSerializer nestedSerializer;
        public SimplePropertyVisitor(EmissionPhaseObjectGraphVisitorArgs args) : base(args.InnerVisitor)
        {
            eventEmitter = args.EventEmitter;
            nestedSerializer = args.NestedObjectSerializer;
        }

        public override bool Enter(IObjectDescriptor value, IEmitter context)
        {
            if (value.Value is SerializedProperty serializedProperty)
            {
                // If the only emittable property here besides the generic ones is Value,
                // and Value is a simple type, just emit that
                var uniqueProps = value.Value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Except(typeof(SerializedProperty).GetProperties(BindingFlags.Public | BindingFlags.Instance), new PropNameComparer())
                    .ToArray();
                if (value.Value is ByteProperty)
                {
                    uniqueProps = new[] { value.Value.GetType().GetProperty("Value") };
                }
                if (uniqueProps.Length == 1 && uniqueProps[0].Name == "Value"
                    && (uniqueProps[0].PropertyType.IsPrimitive || uniqueProps[0].PropertyType == typeof(string)))
                {
                    var valueDescriptor = new ObjectDescriptor(serializedProperty, uniqueProps[0].PropertyType, value.Type);
                    eventEmitter.Emit(new ScalarEventInfo(valueDescriptor), context);
                    return false;
                }
                else if (value.Value is EnumProperty enumProp)
                {
                    var valueDescriptor = new ObjectDescriptor(serializedProperty, enumProp.Name.GetType(), value.Type);
                    eventEmitter.Emit(new ScalarEventInfo(valueDescriptor), context);
                    return false;
                }
            }
            if (value.Value is ArrayProperty arrayProp)
            {
                if (arrayProp.Type == "ByteProperty"
                    && arrayProp.Elements.Count > 0 &&
                    (arrayProp.Elements[0] as ByteProperty)?.Type == "None")
                {
                    // compact byte-arrays
                    byteArray = arrayProp.Elements;
                }
            }
            if (value.Value == byteArray)
            {
                nestedSerializer(byteArray.Cast<ByteProperty>().Select(bp => byte.Parse(bp.Value)));
                byteArray = null;
                return false;
            }
            return base.Enter(value, context);
        }

        private static IList<SerializedProperty> byteArray = null;

        private class PropNameComparer : EqualityComparer<PropertyInfo>
        {
            public override bool Equals(PropertyInfo x, PropertyInfo y) => x.Name == y.Name;
            public override int GetHashCode([DisallowNull] PropertyInfo obj) => obj.Name.GetHashCode();
        }
    }
}
