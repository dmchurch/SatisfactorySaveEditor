using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public readonly struct EmptyValue : ISerializableValue<EmptyValue>
    {
        public int SerializedLength => 0;
        public void Serialize(BinaryWriter writer) { }
        public EmptyValue Parse(BinaryReader reader) => new();
    }

    public readonly struct EmptyValue<T> : ISerializableValue<T> where T: ISerializableValue<T>
    {
        public int SerializedLength => 0;
        public void Serialize(BinaryWriter writer) { }
        public T Parse(BinaryReader reader) => default;
    }
}
