using SatisfactorySaveParser.Structures;
using System;
using System.IO;

namespace SatisfactorySaveParser.ValueTypes
{
    public record ObjectValue(string LevelName, string PathName) : InterfaceValueBase(LevelName, PathName), ISerializableValue<ObjectValue>, IObjectReference
    {
        override public ObjectValue Parse(BinaryReader reader) =>
            new(reader.ReadLengthPrefixedString(), reader.ReadLengthPrefixedString());
        public SaveObject ReferencedObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
