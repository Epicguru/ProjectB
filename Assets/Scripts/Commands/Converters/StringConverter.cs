
using System.Reflection;

namespace Converters
{
    public class StringConverter : VarConverter
    {
        public StringConverter() : base(typeof(string), 1)
        {

        }

        public override object Convert(string[] args, out string error)
        {
            if(args.Length != 1)
            {
                error = $"Writing {Type.Name}: Expected 1 arg, got {args.Length}.";
                return null;
            }

            error = null;
            return args[0];
        }
    }
}
