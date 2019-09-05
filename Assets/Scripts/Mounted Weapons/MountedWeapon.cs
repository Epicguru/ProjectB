
using JNetworking;
using UnityEngine;

[RequireComponent(typeof(NetPosSync))]
public class MountedWeapon : NetBehaviour
{
    [SyncVar] public Vector2 TargetPos;
    [SyncVar] public bool Fire;

    private void Awake()
    {
        var sync = GetComponent<NetPosSync>();
        sync.SendRate = 5f;
        sync.SyncRotation = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, TargetPos);
    }
}
