using SatisfactorySaveParser.ValueTypes;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class IntProperty : SerializedProperty
    {
        public const string TypeName = nameof(IntProperty);
        public override string PropertyType => TypeName;

        [PropertyValue]
        public IntValue Value { get; set; }

        public IntProperty(string propertyName, int index = 0) : base(propertyName, index)
        {
        }
    }
}
