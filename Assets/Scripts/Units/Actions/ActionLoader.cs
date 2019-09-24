
using System;
using System.Linq;
using System.Reflection;

namespace ProjectB.Units.Actions
{
    /// <summary>
    /// Utility class that allows for the use of the 
    /// </summary>
    public static class ActionLoader
    {
        public static Type[] GetAllAssemblyActions(Assembly a)
        {
            var types = from type in a.GetTypes()
            let atr = type.GetCustomAttribute<LoadedAction>()
            where atr != null
            select type;

            return types.ToArray();
        }

        public static string[] GetTypeNames(Type[] types)
        {
            string[] strings = new string[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                Type t = types[i];

                strings[i] = t.FullName;
            }

            return strings;
        }

        public static Type[] GetAllActionClasses(string[] names, Assembly a)
        {
            Type[] array = new Type[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                Type t = a.GetType(name, false, true);

                array[i] = t;
            }

            return array;
        }

        public static UnitAction[] MakeInstances(Type[] types)
        {
            if (types == null)
                return null;

            UnitAction[] actions = new UnitAction[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                Type t = types[i];
                if (t == null)
                    continue;

                if (!t.IsSubclassOf(typeof(UnitAction)))
                    continue;

                actions[i] = (UnitAction)t.GetConstructor(new Type[] { })?.Invoke(new object[] { });
            }

            return actions;
        }
    }
}