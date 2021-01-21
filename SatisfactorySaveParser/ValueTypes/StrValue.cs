using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public readonly struct StrValue : ISerializableValue<StrValue>
    {
        public int SerializedLength => Value.GetSerializedLength();
        public readonly string Value { get; init; }

        public StrValue(string value) => Value = value;

        public void Serialize(BinaryWriter writer) => writer.WriteLengthPrefixedString(Value);
        public StrValue Parse(BinaryReader reader) => reader.ReadLengthPrefixedString();

        public override string ToString() => Value;

        public static implicit operator string(StrValue value) => value.Value;
        public static implicit operator StrValue(string value) => new(value);
    }
}
