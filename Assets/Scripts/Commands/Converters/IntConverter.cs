
using System.Reflection;

namespace Converters
{
    public class IntConverter : VarConverter
    {
        public IntConverter() : base(typeof(int), 1)
        {

        }

        public override object Convert(string[] args, out string error)
        {
            if(args.Length != 1)
            {
                error = $"Writing {Type.Name}: Expected 1 arg, got {args.Length}.";
                return null;
            }

            bool worked = int.TryParse(args[0], out int value);
            if (worked)
            {
                error = null;
                return value;
            }
            else
            {
                error = $"Failed to parse '{args[0]}' as a {Type.Name}.";
                return null;
            }
        }
    }
}
