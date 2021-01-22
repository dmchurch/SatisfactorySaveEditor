using SatisfactorySaveParser.ValueTypes;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class FloatProperty : SerializedProperty
    {
        public const string TypeName = nameof(FloatProperty);
        public override string PropertyType => TypeName;

        [PropertyValue]
        public FloatValue Value { get; set; }

        public FloatProperty(string propertyName, int index = 0) : base(propertyName, index)
        {
        }
    }
}
