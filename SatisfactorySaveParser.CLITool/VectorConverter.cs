using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SatisfactorySaveParser.PropertyTypes.Structs;
using SatisfactorySaveParser.Structures;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace SatisfactorySaveParser.CLITool
{
    class VectorConverter : IYamlTypeConverter
    {
        static readonly Type[] types = new[] {
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Vector),
            typeof(Vector2D),
            typeof(Quat),
        };
        public bool Accepts(Type type) => types.Contains(type);
        public object ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

        private static float[] GetVectorCoords(object value) => value switch
        {
            Vector2 v2 => new[] { v2.X, v2.Y },
            Vector3 v3 => new[] { v3.X, v3.Y, v3.Z },
            Vector4 v4 => new[] { v4.X, v4.Y, v4.Z, v4.W },
            Vector vs => GetVectorCoords(vs.Data),
            Vector2D vs2d => GetVectorCoords(vs2d.Data),
            Quat q => new[] { q.X, q.Y, q.Z, q.W },
            _ => throw new NotImplementedException("bad type in VectorConverter"),
        };

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            float[] coords = GetVectorCoords(value);
            emitter.Emit(new Scalar(null,$"!{type.Name}:",$"({string.Join(", ", coords)})",ScalarStyle.Plain, false, false));
        }
    }
}
