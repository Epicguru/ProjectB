﻿
using System.Reflection;

namespace ProjectB.Commands.Converters
{
    public class BoolConverter : VarConverter
    {
        public BoolConverter() : base(typeof(bool), 1)
        {

        }

        public override object Convert(string[] args, out string error)
        {
            if(args.Length != 1)
            {
                error = $"Writing {Type.Name}: Expected 1 arg, got {args.Length}.";
                return null;
            }

            bool worked = bool.TryParse(args[0], out bool value);
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
