
using System.Reflection;

namespace Converters
{
    public class StringConverter : VarConverter
    {
        public StringConverter() : base(typeof(string))
        {

        }

        public override string Write(object instance, FInfo i, string[] args)
        {
            if(args.Length != 1)
            {
                return $"Writing {Type.Name}: Expected 1 arg, got {args.Length}.";
            }

            i.SetValue(instance, args[0]);
            return null;
        }
    }
}
