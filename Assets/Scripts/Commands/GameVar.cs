
using Converters;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameVar
{
    public string Name { get; }
    public GameVarAttribute Attribute { get; }
    public VarConverter Converter;
    public FInfo FInfo;

    private static Dictionary<Type, Type> converters;

    static GameVar()
    {
        // BOOKMARK where converters are loaded.
        converters = new Dictionary<Type, Type>
        {
            { typeof(string), typeof(StringConverter) },
            { typeof(float), typeof(FloatConverter) },
            { typeof(bool), typeof(BoolConverter) }
        };
    }
    static VarConverter MakeConverter(Type type)
    {
        if (converters.ContainsKey(type))
        {
            var converterType = converters[type];
            return (VarConverter)converterType.GetConstructor(new Type[0] { })?.Invoke(new object[0] { });
        }
        else
        {
            return null;
        }
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
        if (prop == null)
            FInfo = new FInfo(field);
        else
            FInfo = new FInfo(prop);

        Converter = MakeConverter(FInfo.MemberType);

        if(Converter == null)
        {
            Debug.LogError($"Failed to find converter for GameVar of type: {FInfo.MemberType}. Bad stuff is about to happen.");
        }
    }
}