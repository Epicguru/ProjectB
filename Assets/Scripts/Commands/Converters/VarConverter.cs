
using System;
using System.Reflection;

namespace Converters
{
    public abstract class VarConverter
    {
        public Type Type { get; }
        public FInfo Info { get; set; }

        public VarConverter(Type type)
        {
            Type = type ?? throw new ArgumentNullException("type");
        }

        public virtual string MakeString(object instance, FInfo i)
        {
            return Read(instance, i)?.ToString() ?? "null";
        }

        public virtual object Read(object instance, FInfo i)
        {
            return i.GetValue(instance);
        }

        public abstract string Write(object instance, FInfo i, string[] args);
    }
}
