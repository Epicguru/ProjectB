
using System;

namespace Converters
{
    public abstract class VarConverter
    {
        public Type Type { get; }
        public int ExpectedArgCount { get; }

        public VarConverter(Type type, int expectedArgCount)
        {
            this.Type = type ?? throw new ArgumentNullException("type");
            this.ExpectedArgCount = expectedArgCount <= 0 ? 1 : expectedArgCount;
        }

        public virtual string MakeString(object instance, FInfo i)
        {
            return Read(instance, i)?.ToString() ?? "null";
        }

        public virtual object Read(object instance, FInfo i)
        {
            return i.GetValue(instance);
        }

        public abstract object Convert(string[] args, out string error);

        public virtual string Write(object instance, FInfo i, string[] args)
        {
            if (args.Length != ExpectedArgCount)
            {
                return $"Writing {Type.Name}: Expected {ExpectedArgCount} arg, got {args.Length}.";
            }

            object o = Convert(args, out string error);
            if (error != null)
                return error;

            i.SetValue(instance, o);
            return null;
        }
    }
}
