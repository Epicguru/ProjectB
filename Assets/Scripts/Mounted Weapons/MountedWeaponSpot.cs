
using JNetworking;
using UnityEngine;

namespace ProjectB.Vehicles.Weapons
{
    public class MountedWeaponSpot : MonoBehaviour, IParentNode
    {
        public NetObject NetObject
        {
            get
            {
                if (_no == null)
                    _no = GetComponentInParent<NetObject>();
                return _no;
            }
        }
        private NetObject _no;

        public string Name = "Spot A";
        public MountedWeaponSize Size = MountedWeaponSize.SMALL;

        public VehicleMountedWeapons Vehicle { get; set; }

        // NOTE - Only has current value on the server.
        public MountedWeapon CurrentWeapon { get; private set; }

        private byte nodeID;

        public NetObject GetNetObject()
        {
            return NetObject;
        }

        public Transform GetTransform()
        {
            return this.transform;
        }

        public byte GetNodeID()
        {
            return nodeID;
        }

        public void SetNodeID(byte id)
        {
            this.nodeID = id;
        }

        /// <summary>
        /// Sets the currently mounted weapon instance. Assumes that the instance is either null to remove current weapon, or is already spawned and networked.
        /// </summary>
        public void SetMountedWeapon(MountedWeapon instance)
        {
            if (!JNet.IsServer)
            {
                Debug.LogError("Cannot set mounted weapon when not on server.");
                return;
            }
            if (this.Size != instance.Size)
            {
                Debug.LogError($"Size is not correct. Expected {this.Size}, got {instance.Size}... Weapon will not be placed.");
                return;
            }

            if (instance == null)
            {
                if (CurrentWeapon != null)
                {
                    CurrentWeapon = null;
                    Destroy(CurrentWeapon);
                }
            }
            else
            {
                SetMountedWeapon(null);
                instance.GetComponent<NetPosSync>().SetParent(this);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                CurrentWeapon = instance;
            }
        }
    }
}
