using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public readonly struct FloatValue : ISerializableValue<FloatValue>
    {
        public int SerializedLength => 4;
        public readonly float Value { get; init; }

        public FloatValue(float value) => Value = value;

        public void Serialize(BinaryWriter writer) => writer.Write(Value);
        public FloatValue Parse(BinaryReader reader) => reader.ReadSingle();

        public override string ToString() => Value.ToString();

        public static implicit operator float(FloatValue value) => value.Value;
        public static implicit operator FloatValue(float value) => new(value);
    }
}
