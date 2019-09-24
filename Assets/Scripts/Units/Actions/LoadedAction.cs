
using System;

namespace ProjectB.Units.Actions
{
    /// <summary>
    /// Add this attribute to your custom UnitActions for them to be loaded automatically.
    /// Note that this is slower than registering them using <see cref="UnitAction.RegisterAction(UnitAction)"/>
    /// so only use this if you are particulary lazy (efficiency is just clever lazyness).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class LoadedAction : Attribute
    {

    }
}
