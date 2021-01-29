using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SatisfactorySaveParser.PropertyTypes;
using SatisfactorySaveParser.PropertyTypes.Structs;

using YamlDotNet.Serialization;

namespace SatisfactorySaveParser.CLITool
{
    class PropertyInspector : ITypeInspector
    {
        private readonly ITypeInspector Inner;
        public PropertyInspector(ITypeInspector inner) => Inner = inner;

        public IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            var properties = Inner.GetProperties(type, container);
            if (type.BaseType == typeof(SerializedProperty))
            {
                properties = properties.Where(prop => prop.Name switch
                {
                    "PropertyName" => false,
                    "Index" => false,
                    _ => true,
                });
            }
            if (type.IsAssignableTo(typeof(IStructData))
                || type == typeof(StructProperty)
                || type == typeof(ByteProperty)
                || type == typeof(EnumProperty))
            {
                properties = properties.Where(prop => prop.Name != "Type");
            }

            return properties;
        }

        public IPropertyDescriptor GetProperty(Type type, object container, string name, [MaybeNullWhen(true)] bool ignoreUnmatched) => Inner.GetProperty(type, container, name, ignoreUnmatched);
    }
}
