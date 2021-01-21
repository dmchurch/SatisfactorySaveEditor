using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public abstract record InterfaceValueBase(string LevelName, string PathName)
    {
        public int SerializedLength => LevelName.GetSerializedLength() + PathName.GetSerializedLength();

        public string LevelName { get; set; }
        public string PathName { get; set; }

        public abstract InterfaceValueBase Parse(BinaryReader reader);

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteLengthPrefixedString(LevelName);
            writer.WriteLengthPrefixedString(PathName);
        }
    }

    public record InterfaceValue(string LevelName, string PathName): InterfaceValueBase(LevelName, PathName), ISerializableValue<InterfaceValue> {
        override public InterfaceValue Parse(BinaryReader reader) =>
            new(reader.ReadLengthPrefixedString(), reader.ReadLengthPrefixedString());
    }
}
