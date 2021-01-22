using SatisfactorySaveParser.ValueTypes;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class StrProperty : SerializedProperty
    {
        public const string TypeName = nameof(StrProperty);
        public override string PropertyType => TypeName;

        [PropertyValue]
        public StrValue Value { get; set; }

        public StrProperty(string propertyName, int index = 0) : base(propertyName, index)
        {
        }
    }
}
