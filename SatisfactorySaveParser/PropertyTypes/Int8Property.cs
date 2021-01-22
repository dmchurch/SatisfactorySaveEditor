using SatisfactorySaveParser.ValueTypes;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class Int8Property : SerializedProperty
    {
        public const string TypeName = nameof(Int8Property);
        public override string PropertyType => TypeName;

        [PropertyValue]
        public ByteValue Value { get; set; }

        public Int8Property(string propertyName, int index = 0) : base(propertyName, index)
        {
        }
    }
}
