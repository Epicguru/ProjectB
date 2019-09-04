
using JNetworking;
using System.Collections.Generic;
using UnityEngine;

public class Spawnables : MonoBehaviour
{
    private static Spawnables Instance;

    public static T Get<T>(string name) where T : Object
    {
        name = name.Trim().ToLowerInvariant();

        if (Instance.map.ContainsKey(name))
        {
            Object obj = Instance.map[name];
            if(!(obj is T))
            {
                if(obj is GameObject && typeof(Component).IsAssignableFrom(typeof(T)))
                {
                    return (obj as GameObject).GetComponent<T>();
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"Could not find {name}.");
            return default(T);
        }
    }

    public static void NetRegisterAll()
    {
        Instance.NetRegister();
    }

    public Object[] Objects;
    public Dictionary<string, Object> map;

    private void Awake()
    {
        map = new Dictionary<string, Object>();
        foreach (var item in Objects)
        {
            if(item != null)
            {
                string name = item.name.Trim().ToLowerInvariant();
                map.Add(name, item);
                Debug.Log($"Registered '{name}' as {item.GetType().Name}.");
            }
        }
        Instance = this;
    }

    private void NetRegister()
    {
        foreach (var item in Objects)
        {
            if(item != null && item is GameObject)
            {
                var no = (item as GameObject).GetComponent<NetObject>();
                if(no != null)
                {
                    JNet.RegisterPrefab(no);
                    Debug.Log($"Registered net: {item}");
                }
            }
        }
    }
}
