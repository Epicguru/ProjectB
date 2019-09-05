using JNetworking;
using Lidgren.Network;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class AutoDestroy : MonoBehaviour
{
    public ParticleSystem Particles;

    public PoolObject PoolObject
    {
        get
        {
            if (_po == null)
                _po = GetComponent<PoolObject>();
            return _po;
        }
    }
    private PoolObject _po;

    public float Time = 1f;
    [ReadOnly]
    public ushort NetSpawnID;

    private void UponSpawn()
    {
        if(Particles != null)
            Particles.Play();
        Invoke("Kill", Time);
    }

    public void NetSpawn()
    {
        if (!JNet.IsServer)
        {
            Debug.LogError("Cannot network spawn auto destroy effect when not on server.");
            return;
        }

        ushort id = NetSpawnID;
        Vector2 pos = transform.position;

        var msg = JNet.CreateCustomMessage(true, CustomMsg.AUTO_DESTROY_SPAWN, 14);
        msg.Write(id);
        msg.Write(pos);
        JNet.SendCustomMessageToAll(JNet.GetServer().LocalClientConnection, msg, Lidgren.Network.NetDeliveryMethod.Unreliable, 0);
    }

    private void UponDespawn()
    {
        if(Particles != null)
            Particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void Kill()
    {
        PoolObject.Despawn(PoolObject);
    }

    public static void ProcessMessage(NetIncomingMessage msg)
    {
        ushort id = msg.ReadUInt16();
        Vector2 pos = msg.ReadVector2();

        var spawned = PoolObject.Spawn(Spawnables.GetAutoDestroy(id));
        spawned.transform.position = pos;
    }
}
