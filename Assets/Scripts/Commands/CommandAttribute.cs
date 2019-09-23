
using System;

namespace ProjectB.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CommandAttribute : Attribute
    {
        public string Tooltip { get; private set; }
        public string Name;

        public CommandAttribute(string tooltip = "Not specfied")
        {
            this.Tooltip = tooltip;
        }
    }
}
