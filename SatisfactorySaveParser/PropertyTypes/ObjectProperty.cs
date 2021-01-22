using SatisfactorySaveParser.Structures;
using SatisfactorySaveParser.ValueTypes;
using System;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class ObjectProperty : SerializedProperty, IObjectReference
    {
        public const string TypeName = nameof(ObjectProperty);
        public override string PropertyType => TypeName;

        [PropertyValue]
        public ObjectValue Value { get; set; } = new();

        public string LevelName { get => Value.LevelName; set => Value.LevelName = value; }
        public string PathName { get => Value.PathName; set => Value.PathName = value; }

        public SaveObject ReferencedObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ObjectProperty(string propertyName, string root = null, string name = null, int index = 0) : base(propertyName, index)
        {
            LevelName = root;
            PathName = name;
        }

        public ObjectProperty(string propertyName, int index) : base(propertyName, index)
        {
        }
    }
}
