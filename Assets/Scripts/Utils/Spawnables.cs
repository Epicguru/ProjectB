
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
                return obj as T;
            }
        }
        else
        {
            Debug.LogWarning($"Could not find {name}.");
            return default(T);
        }
    }

    public static AutoDestroy GetAutoDestroy(ushort netSpawnID)
    {
        if (Instance.autoDestroyMap.ContainsKey(netSpawnID))
        {
            return Instance.autoDestroyMap[netSpawnID];
        }
        else
        {
            return null;
        }
    }

    public static void NetRegisterAll()
    {
        Instance.NetRegister();
    }

    [Command("[SERVER] Spawns a spawnable object at the position.")]
    public static string Spawn(string name, Vector2 pos)
    {
        var obj = Get<GameObject>(name);
        if(obj == null)
        {
            return $"{name} could not be found or is not a GameObject, so cannot be spawned into world.";
        }

        var pool = obj.GetComponent<PoolObject>();
        if(pool == null)
        {
            var spawned = Instantiate(obj);
            spawned.transform.position = pos;

            var net = spawned.GetComponent<NetObject>();
            if(net != null)
            {
                JNet.Spawn(net);
                return "Spawned as networked object.";
            }
            else
            {
                return "Spawned as client-only prefab.";
            }
        }
        else
        {
            var spawned = PoolObject.Spawn(pool);
            spawned.transform.position = pos;
            return "Spawed as pooled object.";
        }
    }

    [Command("[SERVER] Spawns a spawnable object at the center of the view.")]
    public static string SpawnHere(string name)
    {
        return Spawn(name, GameCamera.Camera.transform.position);
    }

    public Object[] Objects;
    public Dictionary<string, Object> map;
    public Dictionary<ushort, AutoDestroy> autoDestroyMap;

    private void Awake()
    {
        map = new Dictionary<string, Object>();
        autoDestroyMap = new Dictionary<ushort, AutoDestroy>();
        ushort nID = 0;
        foreach (var item in Objects)
        {
            if(item != null)
            {
                string name = item.name.Trim().ToLowerInvariant();
                map.Add(name, item);
                Debug.Log($"Registered '{name}' as {item.GetType().Name}.");

                if(item is GameObject)
                {
                    var go = item as GameObject;
                    var autoDestroy = go.GetComponent<AutoDestroy>();
                    if(autoDestroy != null)
                    {
                        autoDestroy.NetSpawnID = nID;
                        nID++;
                        autoDestroyMap.Add(autoDestroy.NetSpawnID, autoDestroy);
                        Debug.Log($"It is autodestroy mapped to ID {autoDestroy.NetSpawnID}");
                    }
                }
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
