
using System.Reflection;

namespace Converters
{
    public class IntConverter : VarConverter
    {
        public IntConverter() : base(typeof(int))
        {

        }

        public override string Write(object instance, FInfo i, string[] args)
        {
            if(args.Length != 1)
            {
                return $"Writing {Type.Name}: Expected 1 arg, got {args.Length}.";
            }

            bool worked = int.TryParse(args[0], out int value);
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
