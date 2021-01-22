using SatisfactorySaveParser.ValueTypes;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class BoolProperty : SerializedProperty
    {
        public const string TypeName = nameof(BoolProperty);
        public override string PropertyType => TypeName;

        // BoolPropertys are weird, they put their value in the header, and nothing in the "value".
        [PropertyHeader]
        [PropertyValue(SerializeAs = typeof(EmptyValue))]
        public BoolValue Value { get; set; }

        public BoolProperty(string propertyName, int index = 0) : base(propertyName, index)
        {
        }
    }
}
