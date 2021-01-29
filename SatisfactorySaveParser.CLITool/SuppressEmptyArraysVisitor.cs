using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace SatisfactorySaveParser.CLITool
{
    class SuppressEmptyArraysVisitor : ChainedObjectGraphVisitor
    {
        public SuppressEmptyArraysVisitor(IObjectGraphVisitor<IEmitter> next): base(next) { }

        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            if (key.Name == "Fields" || key.Name == "Components" && value.Value != null)
            {
                //Console.WriteLine($"key: {key}, {key.Name}, {key.Type}, {key.Type.IsArray}");
                //Console.WriteLine($"value: {value.Value}, {value.Value?.GetType()}, {(value.Value as Array)?.Length}, {(value.Value as IList)?.Count}");
                //Console.Out.Flush();
                //Environment.Exit(0);
                if ((value.Value as Array)?.Length == 0 || (value.Value as IList)?.Count == 0)
                {
                    return false;
                }
            }
            return base.EnterMapping(key, value, context);
        }
    }
}
