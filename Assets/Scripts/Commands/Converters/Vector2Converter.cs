
using System.Reflection;
using UnityEngine;

namespace ProjectB.Commands.Converters
{
    public class Vector2Converter : VarConverter
    {
        public Vector2Converter() : base(typeof(Vector2), 2)
        {

        }

        public override object Convert(string[] args, out string error)
        {
            if(args.Length != 2)
            {
                error = $"Writing {Type.Name}: Expected 2 arg, got {args.Length}.";
                return null;
            }

            bool worked = float.TryParse(args[0], out float x);
            if(!worked)
            {
                error = $"Failed to parse X '{args[0]}' as a {Type.Name}.";
                return null;
            }

            worked = float.TryParse(args[1], out float y);
            if(!worked)
            {
                error = $"Failed to parse Y '{args[1]}' as a {Type.Name}.";
                return null;
            }

            error = null;
            return new Vector2(x, y);
        }
    }
}
