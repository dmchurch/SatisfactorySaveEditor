using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public interface ISerializableValue
    {
        int SerializedLength { get; }
        void Serialize(BinaryWriter writer);
        ISerializableValue Parse(BinaryReader reader);
    }

    public interface ISerializableValue<out T>: ISerializableValue where T: ISerializableValue<T>
    {
        new T Parse(BinaryReader reader);

        ISerializableValue ISerializableValue.Parse(BinaryReader reader) => Parse(reader);
    }
}
