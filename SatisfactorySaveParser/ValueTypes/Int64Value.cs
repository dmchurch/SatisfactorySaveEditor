using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public readonly struct Int64Value : ISerializableValue<Int64Value>
    {
        public int SerializedLength => 8;
        public readonly long Value { get; init; }

        public Int64Value(long value) => Value = value;

        public void Serialize(BinaryWriter writer) => writer.Write(Value);
        public Int64Value Parse(BinaryReader reader) => reader.ReadInt64();

        public override string ToString() => Value.ToString();

        public static implicit operator Int64Value(long value) => new(value);
        public static implicit operator long(Int64Value value) => value.Value;
    }
}
