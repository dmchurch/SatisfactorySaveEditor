using SatisfactorySaveParser.ValueTypes;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class InterfaceProperty : SerializedProperty
    {
        public const string TypeName = nameof(InterfaceProperty);
        public override string PropertyType => TypeName;

        [PropertyValue]
        public InterfaceValue Value { get; set; } = new();

        public string LevelName { get => Value.LevelName; set => Value.LevelName = value; }
        public string PathName { get => Value.PathName; set => Value.PathName = value; }

        public InterfaceProperty(string propertyName, string root = null, string name = null, int index = 0) : base(propertyName, index)
        {
            LevelName = root;
            PathName = name;
        }

        public InterfaceProperty(string propertyName, int index) : base(propertyName, index)
        {
        }
    }
}
