﻿
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

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

                strings[i] = t.FullName.Trim();
            }

            return strings;
        }

        public static Type[] GetAllActionClasses(string[] names, Assembly a)
        {
            Type[] array = new Type[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i].Trim();
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

        public static UnitAction[] LoadInstances(Assembly a)
        {
            var s = new System.Diagnostics.Stopwatch();
            s.Start();

            Debug.Log($"Loading UnitActions from file, searching assembly {a.FullName}.");
            TextAsset text = Resources.Load<TextAsset>("UnitActions");
            string[] names = text.text.Trim().Split('#');
            var loaded = MakeInstances(GetAllActionClasses(names, a));

            for (int i = 0; i < loaded.Length; i++)
            {
                if(loaded[i] == null)
                {
                    Debug.LogError($"Failed to load UnitAction for class name {names[i]}.");
                }
            }

            s.Stop();
            Debug.Log($"Done, loaded {loaded.Length} actions. Took {s.Elapsed.TotalMilliseconds:F1}ms.");

            return loaded;
        }

        public static void LoadAndRegisterLocal()
        {
            var actions = LoadInstances(Assembly.GetCallingAssembly());
            foreach (var action in actions)
            {
                UnitAction.RegisterNew(action);
            }
        }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("Bake/Bake Unit Actions")]
        public static void BakePaths()
        {
            Assembly a = Assembly.GetCallingAssembly();
            Debug.Log($"Finding all unit actions in {a.FullName}...");
            string[] paths = GetTypeNames(GetAllAssemblyActions(a));

            StringBuilder str = new StringBuilder();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                str.Append(path);
                if(i != paths.Length - 1)
                    str.Append('#');
            }

            string saveFile = System.IO.Path.Combine(Application.dataPath, "Resources", "UnitActions.txt");
            File.WriteAllText(saveFile, str.ToString());
            Debug.Log($"Done, saved {paths.Length}.");
        }

#endif
    }
}