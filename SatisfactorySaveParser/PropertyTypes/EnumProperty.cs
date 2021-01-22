using SatisfactorySaveParser.ValueTypes;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class EnumProperty : SerializedProperty
    {
        public const string TypeName = nameof(EnumProperty);
        public override string PropertyType => TypeName;

        [PropertyHeader]
        public StrValue Type { get; set; }
        [PropertyValue]
        public StrValue Name { get; set; }

        public EnumProperty(string propertyName, int index = 0) : base(propertyName, index)
        {
        }
    }
}
