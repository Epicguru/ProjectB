
using System;

namespace ProjectB
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class GameVarAttribute : Attribute
    {
        public string Name { get; set; }

        public GameVarAttribute()
        {
            // Nothing to do here.
        }
    }
}
