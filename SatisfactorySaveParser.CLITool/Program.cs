using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

using YamlDotNet.Serialization;

using SatisfactorySaveParser;
using SatisfactorySaveParser.PropertyTypes;
using SatisfactorySaveParser.Structures;
using YamlDotNet.Serialization.EventEmitters;
using System.ComponentModel;
using SatisfactorySaveParser.PropertyTypes.Structs;
using YamlDotNet.Core;

namespace SatisfactorySaveParser.CLITool
{
    static class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("Satisfactory save parsing tool")
            {
                new Argument<FileInfo>("filename", "Input save file").ExistingOnly(),
                new Option<FileInfo>(new[] { "--output", "-o" }, "Output file (optional)").LegalFilePathsOnly(),
            };
            rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo>(Run);
            return rootCommand.Invoke(args);
        }

        private static int Run(FileInfo filename, FileInfo output)
        {
            SatisfactorySave saveFile;
            try
            {
                Console.WriteLine($"Loading {filename.FullName}...");
                saveFile = new(filename.FullName);
                Console.WriteLine("Load successful.");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Load unsuccessful: {e.Message}");
                Console.Error.WriteLine(e.StackTrace);
                Console.ResetColor();
                return 1;
            }
            var emptyString = new DefaultValueAttribute("");
            var serializerBuilder = new SerializerBuilder()
                .WithAttributeOverride<ObjectProperty>(o => o.ReferencedObject, new YamlIgnoreAttribute())
                .WithAttributeOverride<ObjectReference>(o => o.ReferencedObject, new YamlIgnoreAttribute())
                .WithAttributeOverride<SaveEntity>(e => e.Rotation, new DefaultValueAttribute(new Vector4(0,0,0,1)))
                .WithAttributeOverride<SaveEntity>(e => e.Position, new DefaultValueAttribute(new Vector3(0,0,0)))
                .WithAttributeOverride<SaveEntity>(e => e.Scale, new DefaultValueAttribute(new Vector3(1,1,1)))
                .WithAttributeOverride<SaveEntity>(e => e.ParentObjectName, emptyString)
                .WithAttributeOverride<SaveEntity>(e => e.ParentObjectRoot, emptyString)
                .WithAttributeOverride<ObjectProperty>(o => o.LevelName, emptyString)
                .WithAttributeOverride<ObjectProperty>(o => o.PathName, emptyString)
                .WithAttributeOverride<InterfaceProperty>(o => o.LevelName, emptyString)
                .WithAttributeOverride<InterfaceProperty>(o => o.PathName, emptyString)
                .WithAttributeOverride<InventoryItem>(i=>i.ItemType, emptyString)
                .WithAttributeOverride<InventoryItem>(i=>i.Unknown2, emptyString)
                .WithAttributeOverride<InventoryItem>(i=>i.Unknown3, emptyString)
                .WithAttributeOverride<MapProperty>(mp => mp.Data, new YamlIgnoreAttribute())
                .WithTypeConverter(new VectorConverter())
                .WithTypeInspector<PropertyInspector>(inner => new(inner))
                .WithEventEmitter<PropertyTagEmitter>(inner => new(inner))
                .WithEventEmitter<PropertyTagEmitter.Fixup>(inner => new(inner), w => w.OnBottom())
                .WithEmissionPhaseObjectGraphVisitor<SuppressEmptyArraysVisitor>(args => new(args.InnerVisitor))
                .WithEmissionPhaseObjectGraphVisitor<SimplePropertyVisitor>(args => new(args))
                .DisableAliases()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults);
            foreach (var type in typeof(SatisfactorySave).Assembly.GetTypes())
            {
                if ((type.IsClass || type.IsValueType) && !type.IsAbstract)
                {
                    //serializerBuilder.WithTagMapping("!"+type.Name, type);
                    if (type.GetProperty("PropertyType")?.PropertyType == typeof(string))
                    {
                        serializerBuilder.WithAttributeOverride(type, "PropertyType", new DefaultValueAttribute(type.Name));
                    }
                    else if (type.GetInterfaces().Contains(typeof(IStructData)) && type.GetProperty("Type")?.PropertyType == typeof(string))
                    {
                        serializerBuilder.WithAttributeOverride(type, "Type", new DefaultValueAttribute(type.Name));
                    }
                    if (type.GetProperty("SerializedLength")?.PropertyType == typeof(int))
                    {
                        serializerBuilder.WithAttributeOverride(type, "SerializedLength", new YamlIgnoreAttribute());
                    }
                }
            }
            var serializer = Serializer.FromValueSerializer(
                serializerBuilder.BuildValueSerializer(),
                EmitterSettings.Default.WithBestWidth(200)
            );
            if (output != null)
            {
                using (var writer = output.CreateText())
                {
                    serializer.Serialize(writer, saveFile);
                }
            } else
            {
                serializer.Serialize(Console.Out, saveFile);
            }
            return 0;
        }
    }
}
