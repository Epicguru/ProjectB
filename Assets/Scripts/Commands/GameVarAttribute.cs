
using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class GameVarAttribute : Attribute
{
    public string Name { get; set; }

    public GameVarAttribute()
    {
        // Nothing to do here.
    }
}
