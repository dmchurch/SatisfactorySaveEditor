using System.Diagnostics;
using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public readonly struct BoolValue : ISerializableValue<BoolValue>
    {
        public int SerializedLength => 1; // the value itself has length 1 despite BoolProperty having SerializedLength 0.
        public readonly bool Value { get; init; }

        public BoolValue(bool value) => Value = value;

        public void Serialize(BinaryWriter writer) => writer.Write((byte)(Value ? 1 : 0));
        public BoolValue Parse(BinaryReader reader)
        {
            var b = reader.ReadByte();
            Trace.Assert(b == 0 || b == 1);
            return b == 1;
        }

        public override string ToString() => Value.ToString();

        public static implicit operator bool(BoolValue value) => value.Value;
        public static implicit operator BoolValue(bool value) => new(value);
    }
}
