
using System.Reflection;

namespace Converters
{
    public class FloatConverter : VarConverter
    {
        public FloatConverter() : base(typeof(float))
        {

        }

        public override string Write(object instance, FInfo i, string[] args)
        {
            if(args.Length != 1)
            {
                return $"Writing {Type.Name}: Expected 1 arg, got {args.Length}.";
            }

            bool worked = float.TryParse(args[0], out float value);
            if (worked)
            {
                i.SetValue(instance, value);
            }
            else
            {
                return $"Failed to parse '{args[0]}' as a {Type.Name}.";
            }

            return null;
        }
    }
}
