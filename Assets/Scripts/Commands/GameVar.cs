
using ProjectB.Commands.Converters;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProjectB.Commands
{
    public class GameVar
    {
        public string Name { get; }
        public GameVarAttribute Attribute { get; }
        public FInfo FInfo { get; }
        public VarConverter Converter;
        public bool IsValid { get; }
        private static Dictionary<Type, VarConverter> converters;

        static GameVar()
        {
            // BOOKMARK where converters are loaded.
            converters = new Dictionary<Type, VarConverter>
        {
            { typeof(string), new StringConverter() },
            { typeof(float), new FloatConverter() },
            { typeof(bool), new BoolConverter() },
            { typeof(int), new IntConverter() },
            { typeof(Vector2), new Vector2Converter() },
        };
        }
        public static VarConverter GetConverter(Type type)
        {
            if (converters.ContainsKey(type))
                return converters[type];
            else
                return null;
        }

        public GameVar(GameVarAttribute atr, FieldInfo field, PropertyInfo prop)
        {
            if (string.IsNullOrWhiteSpace(atr.Name))
            {
                if (prop != null)
                    Name = prop.Name.ToLowerInvariant();
                else
                    Name = field.Name.ToLowerInvariant();
            }
            else
            {
                Name = atr.Name.Trim().ToLowerInvariant();
            }

            Attribute = atr;

            if (prop != null)
                FInfo = new FInfo(prop);
            else
                FInfo = new FInfo(field);

            Converter = GetConverter(FInfo.MemberType);

            if (Converter == null)
            {
                Debug.LogError($"Failed to find converter for GameVar of type: {FInfo.MemberType}. Bad stuff is about to happen.");
                IsValid = false;
            }
            else
            {
                IsValid = true;
            }
        }
    }
}
