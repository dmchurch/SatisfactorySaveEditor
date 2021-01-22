using SatisfactorySaveParser.ValueTypes;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class Int64Property : SerializedProperty
    {
        public const string TypeName = nameof(Int64Property);
        public override string PropertyType => TypeName;

        [PropertyValue]
        public Int64Value Value { get; set; }

        public Int64Property(string propertyName, int index = 0) : base(propertyName, index)
        {
        }

    }
}
