using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SatisfactorySaveParser.PropertyTypes;
using SatisfactorySaveParser.PropertyTypes.Structs;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace SatisfactorySaveParser.CLITool
{
    class PropertyTagEmitter : ChainedEventEmitter
    {
        public PropertyTagEmitter(IEventEmitter nextEmitter) : base(nextEmitter) { }

        public static string GetTagForProperty(SerializedProperty property)
        {
            if (property == null)
            {
                return null;
            }
            var tag = $"!{property.GetType().Name}";
            tag += property switch
            {
                StructProperty structProp => $".{structProp.Type}",
                ByteProperty byteProp => byteProp.Type == "None" ? "" : $".{byteProp.Type}",
                EnumProperty enumProp => $".{enumProp.Type}",
                _ => "",
            };
            tag += $"({property?.PropertyName.Replace(' ','+')})";
            if (property?.Index is not null and not 0)
            {
                tag += $"[{property.Index}]";
            }
            return tag;
        }

        public static string GetTagForDynamicStruct(DynamicStructData structData)
        {
            return structData == null ? null : $"!{structData.GetType().Name}.{structData.Type}";
        }

        private void SetTagInfo(ObjectEventInfo eventInfo)
        {
            if (eventInfo.Source.Value?.GetType().Assembly != typeof(SerializedProperty).Assembly)
            {
                return;
            }
            eventInfo.Tag = GetTagForProperty(eventInfo.Source.Value as SerializedProperty)
                ?? GetTagForDynamicStruct(eventInfo.Source.Value as DynamicStructData)
                ?? $"!{eventInfo.Source.Value.GetType().Name}";
        }

        public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
        {
            if (eventInfo.Source.Value is IEnumerable<byte>)
            {
                eventInfo.Style = SequenceStyle.Flow;
            }
            base.Emit(eventInfo, emitter);
        }

        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            SetTagInfo(eventInfo);
            if (eventInfo.Source.Value is SerializedProperty serializedProperty)
            {
                var valueProp = serializedProperty.GetType().GetProperty(serializedProperty is EnumProperty ? "Name" : "Value");
                Trace.Assert(valueProp.PropertyType == eventInfo.Source.Type);
                var valueDescriptor = new ObjectDescriptor(
                    valueProp.GetValue(eventInfo.Source.Value),
                    eventInfo.Source.Type,
                    eventInfo.Source.StaticType);
                eventInfo = new ScalarEventInfo(valueDescriptor)
                {
                    Anchor = eventInfo.Anchor,
                    Tag = eventInfo.Tag,
                    IsPlainImplicit = eventInfo.IsPlainImplicit,
                    IsQuotedImplicit = eventInfo.IsQuotedImplicit,
                    RenderedValue = eventInfo.RenderedValue,
                    Style = eventInfo.Style,
                };
                Fixup.ToFix = eventInfo;
                Fixup.FixTag = eventInfo.Tag + ":";
            }
            base.Emit(eventInfo, emitter);
        }

        public override void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
        {
            SetTagInfo(eventInfo);
            base.Emit(eventInfo, emitter);
        }

        internal class Fixup : ChainedEventEmitter
        {
            public static ScalarEventInfo ToFix;
            public static string FixTag;

            public Fixup(IEventEmitter nextEmitter) : base(nextEmitter) { }
            public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
            {
                if (eventInfo == ToFix)
                {
                    eventInfo.Tag = FixTag;
                    eventInfo.IsPlainImplicit = false;
                    ToFix = null;
                    FixTag = null;
                }
                base.Emit(eventInfo, emitter);
            }
        }
    }
}
