using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public readonly struct IntValue : ISerializableValue<IntValue>
    {
        public int SerializedLength => 4;
        public readonly int Value { get; init; }

        public IntValue(int value) => Value = value;

        public void Serialize(BinaryWriter writer) => writer.Write(Value);
        public IntValue Parse(BinaryReader reader) => reader.ReadInt32();

        public override string ToString() => Value.ToString();

        public static implicit operator int(IntValue value) => value.Value;
        public static implicit operator IntValue(int value) => new(value);
    }
}
