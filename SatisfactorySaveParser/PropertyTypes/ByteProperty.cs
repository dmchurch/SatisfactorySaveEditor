using SatisfactorySaveParser.ValueTypes;

namespace SatisfactorySaveParser.PropertyTypes
{
    public class ByteProperty : SerializedProperty
    {
        public const string TypeName = nameof(ByteProperty);
        public override string PropertyType => TypeName;

        [PropertyHeader]
        public StrValue Type { get; set; }

        public ByteValue PropertyByteValue { get; set; } = new();
        public StrValue PropertyStrValue { get; set; } = new();

        [PropertyValue]
        public ISerializableValue EitherValue {
            get => Type == "None" ? PropertyByteValue : PropertyStrValue;
            set
            {
                if (Type == "None")
                {
                    PropertyByteValue = (ByteValue)value;
                }
                else
                {
                    PropertyStrValue = (StrValue)value;
                }
            }
        }

        public string Value {
            get => EitherValue.ToString();
            set {
                if (Type == "None")
                {
                    PropertyByteValue = byte.Parse(Value);
                }
                else
                {
                    PropertyStrValue = value;
                }
            }
        }
        public ByteProperty(string propertyName, int index = 0) : base(propertyName, index)
        {
        }

    }
}
