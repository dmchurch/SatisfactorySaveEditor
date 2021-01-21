using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public readonly struct ByteValue : ISerializableValue<ByteValue>
    {
        // note that ByteValue is *specifically* for single-byte values! ByteProperty occasionally does not
        // use ByteValue. Also, Int8Property uses ByteValue, since the encoding is identical, and I'm
        // going with the C# naming convention here: all 8-bit values are "byte", not "int8"
        public int SerializedLength => 1;
        public readonly byte Value { get; init; }

        public ByteValue(byte value) => Value = value;

        public void Serialize(BinaryWriter writer) => writer.Write(Value);
        public ByteValue Parse(BinaryReader reader) => reader.ReadByte();

        public override string ToString() => Value.ToString();

        public static implicit operator byte(ByteValue value) => value.Value;
        public static implicit operator ByteValue(byte value) => new(value);
    }
}
